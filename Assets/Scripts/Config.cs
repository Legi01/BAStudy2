using System;
using System.Collections.Generic;

public class Config
{
    public const bool synchronousHapticFeedback = true;

    public static List<String> propertyNames = new List<string>
        {
            "quat9x", "quat6x", "gyroscope", "magnetometer",
            "accelerometer", "linearAccel", "temperature"
        };

    /**
     * Properties that should be streamed from the TeslaSuit to Unity
     */
    public readonly static Dictionary<String, bool> streamedProperties = new Dictionary<string, bool>
        {
            {"quat9x", true},
            {"quat6x", false},
            {"gyroscope", true},
            {"magnetometer", true},
            {"accelerometer", true},
            {"linearAccel", true},
            {"temperature", false},
        };

    private readonly static Dictionary<String, uint> sensorMaskMap = new Dictionary<string, uint>
        {
            {"quat9x", 1},
            {"quat6x", 2},
            {"gyroscope", 4},
            {"magnetometer", 8},
            {"accelerometer", 16},
            {"linearAccel", 32},
            {"temperature", 64},
        };

    public static uint TsMocapSensorMask()
    {
        uint sensorMask = 0;
        foreach (var keyValuePair in sensorMaskMap)
        {
            if (streamedProperties[keyValuePair.Key])
            {
                sensorMask = sensorMask | keyValuePair.Value;
            }
        }

        return sensorMask;
    }
}