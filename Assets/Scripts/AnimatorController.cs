using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
	enum AttackerState
	{
		Idle,
		Walking,
		Stabbing
	}
	AttackerState currentState;

	private Animator anim;
	private GameObject knife;
	private GameObject mirror;

	private Transform target;
	private float movementSpeed = 2f;
	private float stoppingDistance = 1f;

	private float timer = 180;
	private float countdown;
	private bool startCountdown;

	private bool moveMirror;

	// Start is called before the first frame update
	void Start()
	{
		anim = GetComponent<Animator>();

		knife = GameObject.FindGameObjectWithTag("Knife");
		mirror = GameObject.FindGameObjectWithTag("Mirror");

		target = GameObject.FindGameObjectWithTag("Player").transform.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
		if (target == null)
		{
			Debug.LogError("AnimatorController: Player root bone not found");
		}

		currentState = AttackerState.Idle;

		moveMirror = false;
		countdown = 0;
	}

	// Update is called once per frame
	void Update()
	{
		switch (currentState)
		{
			case AttackerState.Idle:
				Debug.Log("Idle");

				// Introduce a threat after 3 minutes (180 s or 180000 ms)
				if (startCountdown)
				{
					countdown += Time.deltaTime;
					if (countdown > timer)
					{
						moveMirror = true;

						StopStopwatch();
						StartCoroutine(ChangeState(AttackerState.Walking, 3));
					}
				}
				break;

			case AttackerState.Walking:
				Debug.Log("Walking");
				if (target != null)
				{
					// Teslasuit moves the hips, not the parent game objects.
					Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
					Vector3 direction = (targetPosition - transform.position).normalized;
					float distance = Vector3.Distance(transform.position, targetPosition);
					
					// Move towards the target
					if (distance > stoppingDistance)
					{
						anim.SetBool("IsWalking", true);
						MoveCharacter(direction);
					}
					else
					{
						anim.SetBool("IsWalking", false);
						StartCoroutine(ChangeState(AttackerState.Stabbing, 0));
					}
				}
				break;

			case AttackerState.Stabbing:
				Debug.Log("Stabbing");

				anim.SetBool("Stabbing", true);

				// Once animation is finished, change state back to idle
				StartCoroutine(ChangeState(AttackerState.Idle, 0));

				break;
		}

		if (moveMirror)
			mirror.transform.position = new Vector3(
				mirror.transform.position.x, 
				mirror.transform.position.y - 0.01f, 
				mirror.transform.position.z);
	}

	private IEnumerator ChangeState(AttackerState newState, float time)
	{
		yield return new WaitForSeconds(time);
		currentState = newState;
	}

	public void StartStopwatch()
	{
		startCountdown = true;
	}

	public void StopStopwatch()
	{
		startCountdown = false;
		countdown = 0;
	}

	private void MoveCharacter(Vector3 direction)
	{
		// Move the character towards the target
		Vector3 movement = direction * movementSpeed * Time.deltaTime;
		transform.Translate(movement, Space.World);

		// Rotate the character to face the target only around the y-axis
		Vector3 targetPosition = target.position;
		targetPosition.y = transform.position.y; // Ignore y-axis of target
		transform.LookAt(targetPosition);
	}
}
