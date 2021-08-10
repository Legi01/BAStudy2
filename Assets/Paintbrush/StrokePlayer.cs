using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokePlayer : MonoBehaviour
{

    public GameObject[] pathNode;
    private GameObject paintbrush;
    private const float moveSpeed = 1;
    float timer;
    int currentNode;
    private Vector3 startPosition;
    static Vector3 currentPositionHolder;

    // Start is called before the first frame update
    void Start()
    {
        paintbrush = this.gameObject;

        currentNode = 0;
        startPosition = pathNode[currentNode].transform.position;
        currentPositionHolder = pathNode[currentNode].transform.position;

        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * moveSpeed;

        if (paintbrush.transform.position != currentPositionHolder)
        {
            paintbrush.transform.position = Vector3.Lerp(startPosition, currentPositionHolder, timer);
        }
        else
        {
            if (currentNode >= 0 && currentNode < pathNode.Length - 1)
            {
                currentNode++;
            }
            else if (currentNode >= pathNode.Length - 1)
            {
                currentNode--;
            }
            checkNode();
        }
    }

    void checkNode()
    {
        timer = 0;
        startPosition = paintbrush.transform.position;
        currentPositionHolder = pathNode[currentNode].transform.position;
    }

}
