using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameScript : MonoBehaviour
{
    public void OnPlayButtonPress() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("minegame");
    }
    
}
