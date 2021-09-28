using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHMD : MonoBehaviour
{
    private Transform root;

    // Start is called before the first frame update
    void Start()
    {
        root = GameObject.Find("Maniken_skeletool:root").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null) return;

        //root.transform.position = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
    }
}
