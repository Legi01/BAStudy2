using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateAvatar : MonoBehaviour
{
    private GameObject player;
    private Transform targetHips;
    private GameObject avatarHead;

    private float offset = 0;

    public float floorOffset = 0.05f;
    public float hipsOffset = 0.2f;

	// Start is called before the first frame update
	void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetHips = GameObject.FindGameObjectWithTag("Player").transform.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        if (targetHips == null)
        {
            Debug.LogError("CalibrateAvatar: Player root bone not found");
        }
        else
		{
            Debug.Log("CalibrateAvatar: " + targetHips.name);
		}

        avatarHead = GameObject.FindGameObjectWithTag("Head");

        CalculateOffset();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateEuler();

        FollowHMD();
    }

    public void Calibrate()
	{
        player.GetComponent<TsHumanAnimator>().Calibrate();

        player.GetComponent<ScaleAvatar>().ReScale();

        CalculateOffset();

        avatarHead.GetComponent<HeadAnimator>().UpdateOffset(offset);
    }

    private void CalculateOffset()
	{
        Vector3 offsetVector = targetHips.eulerAngles - transform.eulerAngles;
        offset += offsetVector.y;
        Debug.Log("New offset x:" + offsetVector.x + " y: " + offsetVector.y + " y: " + offsetVector.z);
    }

    private void UpdateEuler()
    {
        Vector3 r = targetHips.eulerAngles;

        r.y = targetHips.eulerAngles.y - offset;
        targetHips.eulerAngles = r;
    }

    private void FollowHMD()
	{
        if (Camera.main == null) return;

        // Get the player's head position
        Vector3 playerHeadPos = Camera.main.transform.position;

        // Ensure the character is in front of the player's hips
        Vector3 forwardDirection = targetHips.transform.forward;

        Vector3 targetPosition = playerHeadPos + forwardDirection.normalized * hipsOffset;
        targetPosition.y = targetHips.transform.position.y + floorOffset;

        // Update character position
        targetHips.transform.position = targetPosition;
    }
}
