using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    private Transform camera;
    public Transform avatarHead;
    public Transform avatarRoot;

    private float offset = 0.5f; // See avatar from behind
    private bool scale;

    void Start()
    {
        camera = Camera.main.transform;
        scale = false;
    }

    private void ScaleAvatar()
    {
        // Computes the scale factor of the avatar based on the height of the player.
        float avatarHeigth = avatarHead.position.y;
        float playerHeight = camera.position.y;
        float scaleFactor = playerHeight / avatarHeigth;
        Debug.Log("Player hight " + playerHeight + "Avatar hight " + avatarHeigth + "Scale factor " + scaleFactor);

        // Adjusts model size to the height of the user
        this.transform.localScale = Vector3.one * scaleFactor;

    }

    // Update is called once per frame
    void Update()
    {
        if (!scale && camera.position.y != 0)
        {
            ScaleAvatar();
            scale = true;
        }

        //avatarRoot.position = new Vector3(camera.position.x, 0, camera.position.z + offset);
        //head.eulerAngles = new Vector3(HMD.eulerAngles.x, HMD.eulerAngles.y, HMD.eulerAngles.z);
    }
}
