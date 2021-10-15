using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PaintbrushAnimator : MonoBehaviour
{
    public bool reverse;
    public GameObject[] pathNode;

    private const float moveSpeed = 0.5f;
    private int currentNode;
    private int step;
    private float timer;

    private bool animatePaintbrush;

    private Stopwatch stopwatch;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Vector3 finalPosition;
    private Quaternion finalRotation;

    void Awake()
    {
        animatePaintbrush = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        stopwatch = new Stopwatch();

        if (!reverse)
        {
            currentNode = 0;
            step = 1;
        }
        else
        {
            currentNode = pathNode.Length - 1;
            step = -1;
        }

        startPosition = pathNode[currentNode].transform.position;
        startRotation = pathNode[currentNode].transform.rotation;

        finalPosition = pathNode[currentNode + step].transform.position;
        finalRotation = pathNode[currentNode + step].transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (animatePaintbrush)
		{
            stopwatch.Start();
            AnimatePaintbrush();

            // Introduce a threat after 3 minutes
            if (stopwatch.ElapsedMilliseconds > 180000)
            {
                Label label = new Label(DateTime.Now, "Stop stroking");
                FileManager.Instance().SaveLabels(label);
                Debug.Log(label.GetLabel());

                animatePaintbrush = false;

                stopwatch.Stop();
                stopwatch.Reset();

                switch (GameObject.FindGameObjectWithTag("UI").GetComponent<UI>().threatToggle.value)
                {
                    case 0:
                        // Attack the player
                        GameObject.FindGameObjectWithTag("Attacker").GetComponent<AnimatorController>().OnStab();
                        break;
                    case 1:
                        // Break the hand
                        GameObject.FindGameObjectWithTag("Hand").GetComponent<BoneBreaker>().BreakBone();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void AnimatePaintbrush()
    {
        finalPosition = pathNode[currentNode].transform.position;
        finalRotation = pathNode[currentNode].transform.rotation;

        float distance = Vector3.Distance(transform.position, finalPosition);
        if (distance > 0.05)
        {
            const float speedup = 10;
            timer += Time.deltaTime * moveSpeed * distance * speedup;
            //Debug.Log("Speed-up = " + distance * moveSpeed * speedup);
        }
        else
        {
            timer += Time.deltaTime * moveSpeed;
            //Debug.Log("Normal speed = " + moveSpeed);
        }

        // Move paintbrush from point to point
        if (transform.position != finalPosition)
        {
            // Move the paintbrush towards the next node
            transform.position = Vector3.Lerp(startPosition, finalPosition, timer);

            // Rotate the paintbrush towards the next node (y-axis)
            transform.rotation = Quaternion.Lerp(startRotation, finalRotation, timer);
        }
        else
        {
            if (currentNode <= 0) step = 1;
            else if (currentNode >= pathNode.Length - 1) step = -1;

            currentNode += step;
            timer = 0;

            startPosition = transform.position;
            startRotation = transform.rotation;
        }
    }

    public void OnAnimatePaintbrush(bool synchronous)
    {
        animatePaintbrush = true;

        if (synchronous && reverse)
        {
            gameObject.SetActive(false);
        }
        else if (!synchronous && !reverse)
        {
            GetComponentInChildren<Renderer>().enabled = false;
        }

        string strokeLabel = synchronous ? "Start stroking synchronously" : "Start stroking asynchronously";
        Debug.Log(strokeLabel);
        Label label = new Label(DateTime.Now, strokeLabel);
        FileManager.Instance().SaveLabels(label);
    }

}
