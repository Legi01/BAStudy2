using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAnimator : MonoBehaviour
{
    private Quaternion bodyInitialRotation; // Initial rotation of the avatar

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHeadRotations();
    }

    public void UpdateOffset(float offset)
	{
        // Save initial rotation of the body
        bodyInitialRotation = Quaternion.Euler(0f, offset, 0f);
    }

    private void AnimateHeadRotations()
    {
        // Calculate current relative rotation between body and HMD
        Quaternion currentHMDRotation = Camera.main.transform.rotation;
        Quaternion currentRelativeRotation = Quaternion.Inverse(bodyInitialRotation) * currentHMDRotation;

        // Apply head rotation to character's head
        //transform.rotation = bodyInitialRotation * Quaternion.Inverse(currentRelativeRotation);

        // Animate avatar head
        transform.rotation =  Quaternion.Inverse(bodyInitialRotation) * currentHMDRotation;
    }
}
