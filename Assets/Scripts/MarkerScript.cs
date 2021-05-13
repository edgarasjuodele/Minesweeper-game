using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkerScript : MonoBehaviour {
    public static int markerCount = 0;
    Text marker;

    // Start is called before the first frame update
    void Start() {
        marker = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        marker.text = "Markers: " + markerCount;
    }
}