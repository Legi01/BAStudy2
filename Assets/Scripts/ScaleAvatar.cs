using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAvatar : MonoBehaviour
{
	private Transform avatarHead;
	private FollowHMD followHMDScript;

	void Start()
	{
		avatarHead = GameObject.FindGameObjectWithTag("Head").transform;
		followHMDScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FollowHMD>();
	}

	public void ReScale()
	{
		if (Camera.main != null)
		{
			if (Camera.main.transform.position.y != 0)
			{
				// Computes the scale factor of the avatar based on the height of the player.
				float avatarHeigth = avatarHead.position.y;
				float playerHeight = Camera.main.transform.position.y;
				float scaleFactor = playerHeight / avatarHeigth;
				Debug.Log("Player height " + playerHeight + "; Avatar height " + avatarHeigth + "; Scale factor " + scaleFactor);

				// Adjusts model size to the height of the user
				this.transform.localScale = Vector3.one * scaleFactor;

				followHMDScript.UpdateHeight();
			}
		}
	}
}
