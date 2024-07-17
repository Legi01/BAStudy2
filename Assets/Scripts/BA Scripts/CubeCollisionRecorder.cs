using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CubeCollisionRecorder : MonoBehaviour
{
    public GameObject redCube; // Reference to the red cube
    public GameObject greenCube; // Reference to the green cube
    public Transform[] positions; // Array of possible positions for the cubes
    public float rotationTolerance = 15f; // Tolerance for rotation difference in degrees
    public Toggle hapticsToggle;
    public Toggle longArmsToggle;
    public TMP_InputField subjectIDInputField; // InputField for the subject ID

    private float greenCubeHalfSize; // Half size of the green cube
    private int previousRedCubeIndex = -1; // Last index used for the red cube
    private int previousGreenCubeIndex = -1; // Last index used for the green cube
    private Quaternion[] cubeSymmetries;
    private DataLogger dataLogger; // Reference to the DataLogger


    void Start()
    {
        // Calculate half size of the green cube
        greenCubeHalfSize = greenCube.transform.localScale.x / 2f;
        SetRandomPositions();

        // Definiere alle 24 möglichen Rotationen eines Würfels
        cubeSymmetries = new Quaternion[24];
        int index = 0;
        foreach (var faceRotation in new[] { Quaternion.identity, Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 270, 0), Quaternion.Euler(90, 0, 0), Quaternion.Euler(-90, 0, 0) })
        {
            for (int i = 0; i < 4; i++)
            {
                cubeSymmetries[index++] = faceRotation * Quaternion.Euler(0, 0, i * 90);
            }
        }
    }

    void Update()
    {
        // Check distance between the cubes' centers
        float distance = Vector3.Distance(redCube.transform.position, greenCube.transform.position);

        // If distance is less than half the green cube size and rotation difference is within tolerance, deactivate and respawn cubes
        if (distance < greenCubeHalfSize && AreRotationsEquivalent(redCube.transform.rotation, greenCube.transform.rotation))
        {
            // Log data
            dataLogger.LogData(redCube.transform.position, redCube.transform.rotation, greenCube.transform.position, greenCube.transform.rotation);

            redCube.SetActive(false);
            greenCube.SetActive(false);

            // Set cubes to random positions and reactivate them
            SetRandomPositions();
            redCube.SetActive(true);
            greenCube.SetActive(true);
        }
    }

    bool AreRotationsEquivalent(Quaternion q1, Quaternion q2)
    {
        foreach (var symmetry in cubeSymmetries)
        {
            float angle = Quaternion.Angle(q1, q2 * symmetry);
            if (angle <= rotationTolerance)
            {
                return true;
            }
        }
        return false;
    }

    private void SetRandomPositions()
    {
        int redCubeIndex;
        int greenCubeIndex;

        // Choose a random position for the red cube that is different from the previous one
        do
        {
            redCubeIndex = Random.Range(0, positions.Length);
        } while (redCubeIndex == previousRedCubeIndex);

        // Choose a random position for the green cube that is different from the previous one and not the same as the red cube's position
        do
        {
            greenCubeIndex = Random.Range(0, positions.Length);
        } while (greenCubeIndex == previousGreenCubeIndex || greenCubeIndex == redCubeIndex);

        // Set cubes to the new positions
        redCube.transform.position = positions[redCubeIndex].position;
        greenCube.transform.position = positions[greenCubeIndex].position;

        // Set cubes to a new random rotation
        redCube.transform.rotation = GetRandomCubeRotation();
        //greenCube.transform.rotation = GetRandomCubeRotation();

        // Update previous positions
        previousRedCubeIndex = redCubeIndex;
        previousGreenCubeIndex = greenCubeIndex;
    }

    private Quaternion GetRandomCubeRotation()
    {
        return new Quaternion(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            1).normalized;
    }

    public void SaveLoggedData()
    {
        dataLogger.SaveData();
    }

    public void InitializeDataLogger(string subjectID, string condition)
    {
        dataLogger = new DataLogger(subjectID + ".csv", subjectID, condition);
    }

    public string GetCondition()
    {
        bool hapticsOn = hapticsToggle.isOn;
        bool longArms = longArmsToggle.isOn;

        if (hapticsOn && longArms)
        {
            return "HapticFeedbackLongArms";
        }
        else if (hapticsOn && !longArms)
        {
            return "HapticFeedbackShortArms";
        }
        else if (!hapticsOn && longArms)
        {
            return "NoHapticFeedbackLongArms";
        }
        else // if (!hapticsOn && !longArms)
        {
            return "NoHapticFeedbackShortArms";
        }
    }
}