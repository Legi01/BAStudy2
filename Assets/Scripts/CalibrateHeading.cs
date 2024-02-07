using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateHeading : MonoBehaviour
{
    public Transform target;
    
    private float offset = 0;

    // Start is called before the first frame update
    void Start()
    {
        CalculateOffset();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateEuler();
    }

    public void CalculateOffset()
	{
        Vector3 offsetVector = target.eulerAngles - transform.eulerAngles;
        offset += offsetVector.y;
        Debug.Log("New offset x:" + offsetVector.x + " y: " + offsetVector.y + " y: " + offsetVector.z);
    }

    void UpdateEuler()
    {
        Vector3 r = target.eulerAngles;

        r.y = target.eulerAngles.y - offset;
        target.eulerAngles = r;
    }
}
