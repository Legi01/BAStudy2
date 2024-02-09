using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateHeading : MonoBehaviour
{
    private Transform target;
    
    private float offset = 0;

    public float floorOffset = 0.05f;
    public float hipsOffset = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        if (target == null)
        {
            Debug.LogError("FollowHMD: Player root bone not found");
        }

        CalculateOffset();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateEuler();

        FollowHMD();
    }

    public void CalculateOffset()
	{
        Vector3 offsetVector = target.eulerAngles - transform.eulerAngles;
        offset += offsetVector.y;
        Debug.Log("New offset x:" + offsetVector.x + " y: " + offsetVector.y + " y: " + offsetVector.z);
    }

    private void UpdateEuler()
    {
        Vector3 r = target.eulerAngles;

        r.y = target.eulerAngles.y - offset;
        target.eulerAngles = r;
    }

    private void FollowHMD()
	{
        if (Camera.main == null) return;

        // Get the player's head position
        Vector3 playerHeadPos = Camera.main.transform.position;

        // Ensure the character is in front of the player's hips
        Vector3 forwardDirection = target.transform.forward;

        Vector3 targetPosition = playerHeadPos + forwardDirection.normalized * hipsOffset;
        targetPosition.y = target.transform.position.y + floorOffset;

        // Update character position
        target.transform.position = targetPosition;
    }
}
