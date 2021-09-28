using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHMD : MonoBehaviour
{

    private GameObject camera;
    public GameObject root;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camera = GameObject.Find("/[CameraRig]/Camera");

        if (camera != null)
        {
            root.transform.position = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);
        }

    }
}
