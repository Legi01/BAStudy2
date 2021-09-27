using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeslasuitAPI;
using UnityEngine;
using Logger = TeslasuitAPI.Logger;

public class PaintbrushHaptics : MonoBehaviour
{
    public HapticMaterialAsset materialAsset;

    public float duration = 10;     // Duration in milliseconds
    public float force = 0.5f;      // Force in range 0.0 to 1.0

    public Camera camera;

    void Start()
    {
        Logger.Enabled = true;
        //Logger.AutoWrite = false;
        //Logger.stacktrace_level = Logger.STACKTRACE_LEVEL.API_ONLY;
        Logger.OnLogMessage += Logger_OnLogMessage;
        Teslasuit.SDKError += (sender, code) =>
        {
            Debug.Log(string.Format("SDK error : {0} sent {1}", sender, code));
        };

        Teslasuit.PluginError += (o, ex) =>
        {
            Debug.Log(string.Format("Plugin error : {0} message: {1}", o.ToString(), ex.Message));
        };
    }

    private void Logger_OnLogMessage(string obj)
    {
        Debug.Log("log : " + obj);
    }

    HapticHitInfo GetHapticHitInfo()
    {
        HapticHitInfo hitInfo = new HapticHitInfo();

        hitInfo.material = materialAsset;                        // IHapticMaterial
        hitInfo.hitEvent = HapticHitEvent.HitStay;               // Type of Hit Event
        hitInfo.duration_ms = (uint)duration;                    // Duration in milliseconds
        hitInfo.impact = force;                                  // Force in range 0.0 to 1.0

        return hitInfo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            HapticRaycastHit hit;
            if (HapticHitRaycaster.Raycast(ray, out hit))
            {
                HapticReceiver receiver = hit.hapticReceiver;
                var poly = hit.channelPoly;
                HapticHitInfo hapticHitInfo = GetHapticHitInfo();

                HapticPolyHit polyHit = new HapticPolyHit(poly, hapticHitInfo);
                receiver.PolyHit(polyHit);
            }
        }
    }
}
