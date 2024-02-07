using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHMD : MonoBehaviour
{
	public Transform root;

	private float hipsHeight;
	private float hmdOffset = -1f;

	// Start is called before the first frame update
	void Start()
	{
		hipsHeight = root.transform.position.y;

		Debug.Log("Initial hips heigh" + hipsHeight);
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (Camera.main == null) return;

		root.transform.position = new Vector3(Camera.main.transform.position.x, hipsHeight, Camera.main.transform.position.z);
	}
	public void UpdateHeight()
	{
		hipsHeight = root.transform.position.y;

		Debug.Log("New hips heigh" + hipsHeight);
	}
}
