using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
	private Animator anim;
	private GameObject knife;

	private bool stab;

	// Start is called before the first frame update
	void Start()
	{
		anim = GetComponent<Animator>();
		knife = GameObject.FindGameObjectWithTag("Knife");
		knife.SetActive(false);

		stab = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (stab && anim.GetCurrentAnimatorStateInfo(0).IsName("Knife Stabbing"))
		{
			Label label = new Label(DateTime.Now, "Start stabbing");
			FileManager.Instance().SaveLabels(label);
			Debug.Log(label.GetLabel());

			stab = false;

			StartCoroutine("OnCompleteAttackAnimation");
		}
	}

	public IEnumerator OnStab()
	{
		yield return new WaitForSeconds(5);

		stab = true;
		knife.SetActive(true);
		anim.SetTrigger("Stab");

		Label label = new Label(DateTime.Now, "Knife appears");
		FileManager.Instance().SaveLabels(label);
		Debug.Log(label.GetLabel());
	}

	IEnumerator OnCompleteAttackAnimation()
	{
		while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
			yield return null;

		Label label = new Label(DateTime.Now, "Stop stabbing");
		FileManager.Instance().SaveLabels(label);
		Debug.Log(label.GetLabel());
	}
}
