using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetHeading : MonoBehaviour {
    public Transform target;
    private Vector3 offset = Vector3.zero;

    private MocapReplay motionReplay;

    // Use this for initialization
    void Start () {
        motionReplay = GameObject.FindGameObjectWithTag("Teslasuit").GetComponent<MocapReplay>();
        offset = target.eulerAngles - transform.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        if (!motionReplay.IsReplaying()) {
            UpdateEuler();
        }
    }

    public void UpdateEuler()
    {
        Vector3 r = transform.eulerAngles;

        r.y = target.eulerAngles.y - offset.y;
        transform.eulerAngles = r;
    }
}
