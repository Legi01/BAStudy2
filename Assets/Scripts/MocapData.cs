using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MocapData : MonoBehaviour
{

    public Button recordButton;
    public Button replayButton;

    // Start is called before the first frame update
    void Start()
    {
        Button btn_record = recordButton.GetComponent<Button>();
        btn_record.onClick.AddListener(Record);

        Button btn_replay = recordButton.GetComponent<Button>();
        btn_replay.onClick.AddListener(Replay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Record()
    {
    }

    void Replay()
    {
    }
}
