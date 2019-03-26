using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerGame : MonoBehaviour {
	private const int COIN_SCORE_AMOUNT = 5;

	public static ManagerGame Instance { set; get; }
	private bool isStarted = false;
	private PlayerMotor motor;
    public bool isDead{set; get;}

	//UI 
	public Text scoreText, coinText, modifierText, hiScoreText;
	private float score, coinScore, modifierScore;
	private int lastScore;
    public Animator gameCanvas, menuAnim, diamondAnim;

        //gameplay
    private bool isMoving = false;
    
    //death menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadCoinsText;
    
	private void Awake(){
		Instance = this;
		modifierScore = 1;
		motor = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMotor> ();
		modifierText.text = "x" + modifierScore.ToString ("0.0");
		coinText.text = coinScore.ToString ("0");
        hiScoreText.text = PlayerPrefs.GetInt("Hiscore").ToString();
	}

	private void Update(){
		if (MobileInput.Instance.Tap && !isStarted) {
			isStarted = true;
			motor.StartGame ();
            FindObjectOfType<GlacialSpawner>().isScrolling = true;
            FindObjectOfType<CameraMotor>().isMoving = true;
            gameCanvas.SetTrigger("Show");
            menuAnim.SetTrigger("Hide");
		}

		if (isStarted && !isDead) {
			//bump the score up
			score += (Time.deltaTime * modifierScore);
			if (lastScore != (int)score) {
				lastScore = (int)score;
				scoreText.text = score.ToString ("0");
			}
		}		
	}

	public void GetCoin(){
        diamondAnim.SetTrigger("Collect");
		coinScore++;
		coinText.text = coinScore.ToString ("0");
		score += COIN_SCORE_AMOUNT;
		scoreText.text = score.ToString ("0");

	}

	public void updateModifier(float modifierAmount){
		modifierScore = 1.0f + modifierAmount;
		modifierText.text = "x" + modifierScore.ToString ("0.0");
	}
    
        public void OnPlayButton(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    public void OnDeath(){
        deadScoreText.text = score.ToString("0");
        deadCoinsText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        isDead = true;
        FindObjectOfType<GlacialSpawner>().isScrolling = false;
        gameCanvas.SetTrigger("Hide");
        
        if(score > PlayerPrefs.GetInt("Hiscore")){
            float s = score;
            if(s%1 == 0)
                s+=1;
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
    }


}
