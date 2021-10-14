using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneBreaker : MonoBehaviour
{

    private bool breakBone;

    // Start is called before the first frame update
    void Start()
    {
        breakBone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (breakBone)
        {
            if (UnwrapAngle(transform.localEulerAngles.y) > 230)
            {
                transform.Rotate(0, -20.0f * Time.deltaTime, 0, Space.Self);
            }
            else
            {
                breakBone = false;

                Label label = new Label(DateTime.Now, "Stop rotating hand");
                FileManager.Instance().SaveToCSV(label);
                Debug.Log(label.GetLabel());
            }
        }
    }

    public void BreakBone()
    {
        breakBone = true;

        Label label = new Label(DateTime.Now, "Start rotating hand");
        FileManager.Instance().SaveToCSV(label);
        Debug.Log(label.GetLabel());
    }

    private float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

}
