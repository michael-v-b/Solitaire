using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] GameManager gm;
    [SerializeField] Button undo;
    [SerializeField] Button redo;
    [SerializeField] Button newGame;
    [SerializeField] Button restart;
    bool paused = false;

    public void pause() {
            paused = true;
            gm.paused = true;
            pauseCanvas.gameObject.SetActive(true);
            undo.enabled = false;
            redo.enabled = false;
            newGame.enabled = false;
            restart.enabled = false;
    }
    public void resume() {
        paused = false;
        Debug.Log("test");
        gm.paused = false;
        pauseCanvas.gameObject.SetActive(false);
        undo.enabled = true;
        redo.enabled = true;
        newGame.enabled = true;
        restart.enabled = true;

    }
    public void menu() {
        SceneManager.LoadScene("MainMenu");
    }
    public void quitGame() {
        Application.Quit();
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(paused) {
                resume();
                paused = false;
            } else {
                pause();
                paused = true;
            }
        }
    }
}
