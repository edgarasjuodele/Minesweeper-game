using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartNormalScript : MonoBehaviour
{
    public void OnRestartButtonPress() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("minegamenormal");
    }
}
