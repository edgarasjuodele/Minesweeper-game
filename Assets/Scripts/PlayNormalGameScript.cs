using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNormalGameScript : MonoBehaviour
{
    public void OnPlayButtonPress() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("minegamenormal");
    }
}
