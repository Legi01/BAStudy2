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
	private bool synchronous;
	private bool haptics;

	private Stopwatch stopwatch;

	private Vector3 startPosition;
	private Quaternion startRotation;

	private Vector3 finalPosition;
	private Quaternion finalRotation;

	void Awake()
	{
		animatePaintbrush = false;
		synchronous = false;
		haptics = false;
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

			// Introduce a threat after 3 minutes (180000 ms)
			if (stopwatch.ElapsedMilliseconds > 180000)
			{
				animatePaintbrush = false;

				stopwatch.Stop();
				stopwatch.Reset();

				GetComponentInChildren<Renderer>().enabled = false;

				if (!reverse)
				{
					string str = "";
					if (!haptics)
					{
						str = "Stop stroking without haptics";
					}
					else
					{
						if (synchronous)
						{
							str = "Stop stroking synchronously";
						}
						else
						{
							str = "Stop stroking asynchronously⁄";
						}
					}
					Label label = new Label(DateTime.Now, str);
					Debug.Log(label.GetLabel());
					//FileManager.Instance().SaveLabels(label);

					switch (GameObject.FindGameObjectWithTag("UI").GetComponent<UI>().threatDropdown.value)
					{
						case 0:
							// Attack the player
							//StartCoroutine(GameObject.FindGameObjectWithTag("Attacker").GetComponent<AnimatorController>().OnStab());
							break;
						case 1:
							// Break the hand
							//StartCoroutine(GameObject.FindGameObjectWithTag("Hand").GetComponent<BoneBreaker>().BreakBone());
							break;
						default:
							break;
					}
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

	public void OnAnimatePaintbrush(bool synchronous, bool haptics)
	{
		animatePaintbrush = true;
		this.synchronous = synchronous;
		this.haptics = haptics;

		if ((synchronous && reverse && haptics) || (!synchronous && !reverse && haptics) || (!haptics && !reverse))
		{
			GetComponentInChildren<Renderer>().enabled = false;
		}
		else
		{
			GetComponentInChildren<Renderer>().enabled = true;
		}

		if (!reverse)
		{
			string str = "";
			if (!haptics)
			{
                str = "Start stroking without haptics";
			}
			else
			{
				if (synchronous)
				{
					str = "Start stroking synchronously";
				}
				else
				{
					str = "Start stroking asynchronously⁄";
				}
			}
			Label label = new Label(DateTime.Now, str);
			Debug.Log(label.GetLabel());
			//FileManager.Instance().SaveLabels(label);
		}
	}

}
