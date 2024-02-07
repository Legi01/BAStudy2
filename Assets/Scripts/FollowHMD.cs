using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHMD : MonoBehaviour
{
	public Transform root;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (Camera.main == null) return;

		root.transform.position = new Vector3(Camera.main.transform.position.x, root.transform.position.y, Camera.main.transform.position.z);
	}
}
