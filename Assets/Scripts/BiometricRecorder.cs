using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeslasuitAPI;
using System;

public class BiometricRecorder : MonoBehaviour
{

    public SuitAPIObject suitApi;

    private ECG_MV[] ecgData;
    private GSRBufferedData[] gsrData;

    // Start is called before the first frame update
    void Start()
    {
        /*IBiometry biometry = suitApi.Biometry;
        biometry.StartECG();
        biometry.StartGSR();*/

        StartCoroutine(UpdateBiometriyOptions());
    }

    // Update is called once per frame
    void Update()
    {

     
    }

    private IEnumerator UpdateBiometriyOptions()
    {
        yield return new WaitUntil(() => suitApi.Biometry is { ECGStarted: true });
        yield return new WaitUntil(() => suitApi.Biometry is { GSRStarted: true });

        suitApi.Biometry.ECGUpdated += OnECGUpdate;
        suitApi.Biometry.GSRUpdated += OnGSRUpdate;

        ECGFrequency ecgFrequency = ECGFrequency.TS_ECG_FPS_20;
        GSRFrequency gsrFrequency = GSRFrequency.TS_GSR_FPS_60;
        suitApi.Biometry.UpdateECGFrequency(ECGFrequency.TS_ECG_FPS_20);
        suitApi.Biometry.UpdateGSRFrequency(GSRFrequency.TS_GSR_FPS_60);

        Debug.Log($"Updated biometry options: ECG frequency is {ecgFrequency}. GSR frequency is {gsrFrequency}.");

        //OnECGUpdated();
    }

    private void OnECGUpdate(ref ECGBuffer_MV ECGBuffer, IntPtr opaque, ResultCode resultCode)
    {
        ecgData = ECGBuffer.data;
        Debug.Log(ECGBuffer.data.GetValue(ECGBuffer.data.Length - 1));
    }

    private void OnGSRUpdate(ref GSRBuffer GSRBuffer, IntPtr opaque, ResultCode resultCode)
    {
        gsrData = GSRBuffer.data;
        Debug.Log(GSRBuffer.data.GetValue(GSRBuffer.data.Length - 1));
    }

    /*void OnApplicationQuit()
    {
        Debug.Log("Stopping ECG and GSR");
        suitApi.Biometry.StopECG();
        suitApi.Biometry.StopGSR();
    }*/
}
