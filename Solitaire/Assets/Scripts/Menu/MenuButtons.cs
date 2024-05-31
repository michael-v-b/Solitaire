using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void openGame() {
        SceneManager.LoadScene("Solitaire");
    }
    public void quitGame() {
        Application.Quit();
    }
}
