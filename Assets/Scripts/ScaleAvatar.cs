using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAvatar : MonoBehaviour
{
	private Transform avatarHead;
	private Vector3 originalScale;

	void Start()
	{
		avatarHead = GameObject.FindGameObjectWithTag("Head").transform;
		originalScale = this.transform.localScale;
	}

	public void ReScale()
	{
		if (Camera.main != null)
		{
			// Computes the scale factor of the avatar based on the height of the player.
			float avatarHeight = avatarHead.position.y;
			float playerHeight = Camera.main.transform.position.y - 0.1f;
			float scaleFactor = playerHeight / avatarHeight;
			Debug.Log("Player height " + playerHeight + "; Avatar height " + avatarHeight + "; Scale factor " + scaleFactor);

			// Adjusts model size to the height of the user
			this.transform.localScale = originalScale * scaleFactor;
		}
	}
}
