using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokePlayer : MonoBehaviour
{

    public GameObject[] pathNode;
    private const float moveSpeed = 0.5f;
    private int currentNode;
    private int step;
    private float timer;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        currentNode = 0;
        step = 1;

        startPosition = pathNode[currentNode].transform.position;
        startRotation = pathNode[currentNode].transform.rotation;
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime * moveSpeed;

        if (transform.position != pathNode[currentNode].transform.position)
        {
            // Move the paintbrush towards the second node
            transform.position = Vector3.Lerp(startPosition, pathNode[currentNode].transform.position, timer);

            // Rotate the paintbrush towards the second node (y-axis)
            transform.rotation = Quaternion.Lerp(startRotation, pathNode[currentNode].transform.rotation, timer);
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

}
