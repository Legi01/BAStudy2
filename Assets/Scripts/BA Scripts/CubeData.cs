using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CubeData
{
    public string subjectID;
    public string condition;
    public Vector3 redCubePosition;
    public Quaternion redCubeRotation;
    public Vector3 greenCubePosition;
    public Quaternion greenCubeRotation;
    public DateTime timestamp;

    public CubeData(string subjectID, string condition, Vector3 redPos, Quaternion redRot, Vector3 greenPos, Quaternion greenRot)
    {
        this.subjectID = subjectID;
        this.condition = condition;
        redCubePosition = redPos;
        redCubeRotation = redRot;
        greenCubePosition = greenPos;
        greenCubeRotation = greenRot;
        timestamp = DateTime.Now;
    }

    public string ToCSV(string separator)
    {
        return $"{timestamp}{separator}{subjectID}{separator}{condition}{separator}{redCubePosition.x}{separator}{redCubePosition.y}{separator}{redCubePosition.z}{separator}{redCubeRotation.x}{separator}{redCubeRotation.y}{separator}{redCubeRotation.z}{separator}{redCubeRotation.w}{separator}{greenCubePosition.x}{separator}{greenCubePosition.y}{separator}{greenCubePosition.z}{separator}{greenCubeRotation.x}{separator}{greenCubeRotation.y}{separator}{greenCubeRotation.z}{separator}{greenCubeRotation.w}";
    }

    public static string GetCSVHeader(string separator)
    {
        return $"Timestamp{separator}SubjectID{separator}Condition{separator}RedCubePosX{separator}RedCubePosY{separator}RedCubePosZ{separator}RedCubeRotX{separator}RedCubeRotY{separator}RedCubeRotZ{separator}RedCubeRotW{separator}GreenCubePosX{separator}GreenCubePosY{separator}GreenCubePosZ{separator}GreenCubeRotX{separator}GreenCubeRotY{separator}GreenCubeRotZ{separator}GreenCubeRotW";
    }
}

