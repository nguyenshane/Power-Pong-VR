using UnityEngine;
using System.Collections;

public enum eScore {
	Green,
	Orange
}

public class Scores : MonoBehaviour {

	public eScore Score;

	public GameObject scoreNumber;
	public GameObject livesNumber;

	public float goalSpeedMultiplier;
	public int maxLives;

	private float currentMultiplier;
	private int score;
	private int lives;

	private ScoreScreen scoreScreen;


	// Use this for initialization
	void Start () {
		scoreScreen = GameObject.Find ("ScoreScreen").GetComponent<ScoreScreen> ();

		if (Score == eScore.Green) {
			switch (ScoreScreen.greenLivesSelection) {
			case 0:
				maxLives = 3;
				break;

			case 1:
				maxLives = 5;
				break;
			}
			scoreScreen.greenLives = maxLives;
		} else {
			switch (ScoreScreen.orangeLivesSelection) {
			case 0:
				maxLives = 3;
				break;
				
			case 1:
				maxLives = 5;
				break;
			}
			scoreScreen.orangeLives = maxLives;
		}

		score = 0;
		lives = maxLives;
		scoreNumber.guiText.text = score.ToString();
		livesNumber.guiText.text = lives.ToString();
		currentMultiplier = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetScore (int newScore) {
		score = newScore;
		scoreNumber.guiText.text = score.ToString();

		if (Score == eScore.Green) scoreScreen.greenScore = score;
		else scoreScreen.orangeScore = score;
	}

	public void AddScore (int newScore) {
		score += newScore;
		scoreNumber.guiText.text = score.ToString();

		if (Score == eScore.Green) scoreScreen.greenScore = score;
		else scoreScreen.orangeScore = score;
	}

	public void RemoveLife() {
		if (lives > 0 ) {
			lives--;
		}
		livesNumber.guiText.text = lives.ToString();

		if (Score == eScore.Green) scoreScreen.greenLives = lives;
		else if (Score == eScore.Orange) scoreScreen.orangeLives = lives;

		//if (lives <= 0) {
			//if (Score == eScore.Green) scoreScreen.orangeWins++;
			//else if (Score == eScore.Orange) scoreScreen.greenWins++;

			//scoreScreen.activate();
			scoreScreen.handleScore();
		//}
	}

	public int getLives() {
		return lives;
	}

	public int getScore() {
		return score;
	}

	public float getMultiplier() {
		return currentMultiplier;
	}

	public void increaseMultiplier() {
		currentMultiplier += goalSpeedMultiplier;
	}
}
