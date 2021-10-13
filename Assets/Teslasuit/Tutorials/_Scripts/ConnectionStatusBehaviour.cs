using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeslasuitAPI;

namespace TeslasuitAPI.Tutorials
{
    public class ConnectionStatusBehaviour : MonoBehaviour
    {
        public Text text;
        private SuitAPIObject aPIObject;

        // Use this for initialization
        void Start()
        {
            aPIObject = GameObject.FindGameObjectWithTag("Teslasuit").GetComponentInChildren<SuitAPIObject>();
            aPIObject.BecameAvailable += delegate { text.text = "Connected"; };
            aPIObject.BecameUnavailable += delegate { text.text = "Not Connected"; };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}