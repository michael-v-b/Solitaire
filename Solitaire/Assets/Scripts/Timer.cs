using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameManager gm;
    bool gameStarted = false;
    bool gamePaused = false;
    [SerializeField] float seconds = 0;
    public void startGame() {
        gameStarted = true;
    }
    public void stopTimer() {
        gamePaused = true;
    }
    public void resumeTimer() {
        gamePaused = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameStarted && !gamePaused && !gm.paused) {
            seconds+=Time.deltaTime;
        }
        if((seconds/3600f) >= 1) {
            timerText.SetText("Timer: " +(int)(seconds/3600)+":"+(int)(seconds/60)+":"+((int)seconds%60));
        } else if((int)(seconds%60) >= 10) {
            timerText.SetText("Timer: " + (int)(seconds/60) + ":" + ((int)seconds%60));
        } else {
            timerText.SetText("Timer: " + (int)(seconds/60) + ":0" + ((int)seconds%60));
        }

    }
}
