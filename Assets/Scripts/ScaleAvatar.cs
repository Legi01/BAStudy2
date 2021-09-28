using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAvatar : MonoBehaviour
{
    private GameObject camera;
    public GameObject avatarHead;
    public GameObject avatarRoot;

    private bool scale;

    void Start()
    {
        scale = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!scale)
        {
            camera = GameObject.Find("/[CameraRig]/Camera");
            if (camera != null && camera.transform.position.y != 0) {
                // Computes the scale factor of the avatar based on the height of the player.
                float avatarHeigth = avatarHead.transform.position.y;
                float playerHeight = camera.transform.position.y;
                float scaleFactor = playerHeight / avatarHeigth;
                Debug.Log("Player hight " + playerHeight + "; Avatar hight " + avatarHeigth + "; Scale factor " + scaleFactor);

                // Adjusts model size to the height of the user
                this.transform.localScale = Vector3.one * scaleFactor;

                scale = true;
            }
        }
    }
}
