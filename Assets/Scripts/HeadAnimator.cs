using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAnimator : MonoBehaviour
{
    private Transform avatarHead;

    // Start is called before the first frame update
    void Start()
    {
        avatarHead = GameObject.FindGameObjectWithTag("Head").transform;
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHeadRotations();
    }

    private void AnimateHeadRotations()
    {
        // Animate avatar head
        avatarHead.rotation = Camera.main.transform.rotation;
    }
}
