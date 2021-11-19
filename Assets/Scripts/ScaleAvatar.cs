using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAvatar : MonoBehaviour
{
	private Transform avatarHead;

	private bool scaled;

	void Start()
	{
		scaled = false;
		avatarHead = GameObject.Find("Maniken_skeletool:head").transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (!scaled)
		{
			if (Camera.main != null)
			{
				if (Camera.main.transform.position.y != 0)
				{
					// Computes the scale factor of the avatar based on the height of the player.
					float avatarHeigth = avatarHead.position.y;
					float playerHeight = Camera.main.transform.position.y;
					float scaleFactor = playerHeight / avatarHeigth;
					Debug.Log("Player hight " + playerHeight + "; Avatar hight " + avatarHeigth + "; Scale factor " + scaleFactor);

					// Adjusts model size to the height of the user
					this.transform.localScale = Vector3.one * scaleFactor;

					scaled = true;
				}
			}
		}
	}
}
