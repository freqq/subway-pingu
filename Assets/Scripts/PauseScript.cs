using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour {

    public bool paused;
    public Text pausedText, buttonText;
    public Button buttonPause;
    
    void Start(){
        paused = false;
        buttonPause.interactable = true;
    }
    
    public void Pause(){
        paused = !paused;

        if(paused){
            Time.timeScale = 0;
            pausedText.text = "PAUSED";
            buttonText.text = "►";
        }
        else if(!paused){
            Time.timeScale = 1;
            pausedText.text = "";
            buttonText.text = "||";
        }
    }
}
