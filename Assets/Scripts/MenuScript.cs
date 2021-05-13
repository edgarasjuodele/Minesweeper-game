using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public void BackToMenuButton() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }
}
