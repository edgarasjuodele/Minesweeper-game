using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryTextScript : MonoBehaviour
{
    public static bool victory = false;
    TMPro.TextMeshProUGUI vicText;

    // Start is called before the first frame update
    void Start() {
        vicText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update() {
        if (victory) {
            vicText.text = "Congratulations you win!";
        }
    }
}
