using UnityEngine;
using System.Collections;

/*
 * Handles lives and score (although ScoreScreen does a lot of this as well, functionality should probably be moved around for consistency but I'm lazy)
 */

public enum eScore {
	Green,
	Orange
}

public class Scores : MonoBehaviour {

	public eScore Score;

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
		currentMultiplier = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetScore (int newScore) {
		score = newScore;

		if (Score == eScore.Green) scoreScreen.greenScore = score;
		else scoreScreen.orangeScore = score;
	}

	public void AddScore (int newScore) {
		score += newScore;

		if (Score == eScore.Green) scoreScreen.greenScore = score;
		else scoreScreen.orangeScore = score;
	}

	public void RemoveLife() {
		if (lives > 0) lives--;

		if (Score == eScore.Green) scoreScreen.greenLives = lives;
		else if (Score == eScore.Orange) scoreScreen.orangeLives = lives;

		scoreScreen.handleScore();
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
