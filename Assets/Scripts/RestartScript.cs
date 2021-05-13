using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartScript : MonoBehaviour
{
    public void OnRestartButtonPress() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("minegame");
        VictoryTextScript.victory = false;
    }
}
