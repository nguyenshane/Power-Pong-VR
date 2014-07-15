using UnityEngine;
using System.Collections;

public class ScoreScreen : MonoBehaviour {

	public bool showFPS;
	public int maxLevels;

	public GUIStyle blank;
	public GUIStyle box;
	public GUIStyle boxG;
	public GUIStyle boxO;
	public GUIStyle label, labelG, labelO;
	public GUIStyle button;
	public GUIStyle checkboxL;
	public GUIStyle checkboxR;
	public Texture level1, level2, level3;

	public int currentLevel;
	public int greenLives, orangeLives, greenScore, orangeScore, greenWins, orangeWins, greenTotalWins, orangeTotalWins;

	static int instanceCount = 0;

	int padding = 0;
	int width = 240;
	bool showing, escShowing;
	bool greenWon = false, orangeWon = false;
	public static int greenAISelection, orangeAISelection, greenLivesSelection, orangeLivesSelection;
	public static int levelSelection;
	string[] AIOptions = new string[] {"Human", "Easy   AI", "Medium   AI", "Hard   AI"};
	string[] livesOptions = new string[] {"3   Lives", "5   Lives"};

	// Use this for initialization
	void Start () {
		instanceCount++;

		if (instanceCount > 1) {
			instanceCount--;
			Destroy(gameObject);
			return;
		}
		
		greenLives = orangeLives = greenScore = orangeScore = greenWins = orangeWins = greenTotalWins = orangeTotalWins = 0;
		greenAISelection = orangeAISelection = greenLivesSelection = orangeLivesSelection = 0;
		levelSelection = -1;
		showing = escShowing = false;
		DontDestroyOnLoad(transform.gameObject);
		activateBefore();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape) && !showing) {
			if (!escShowing) activateEscMenu();
			else deactivateEscMenu();
		}
	}

	void OnGUI() {
		//Showing level selection menu
		if (showing == true) {
			if (greenWon) {
				GUI.Box(new Rect(padding, padding, Screen.width - padding*2, Screen.height - padding*2), "G R E E N    W I N S!", boxG);

				//Return to menu button
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 120, 240, 60), "Main  Menu", button)) {
					greenScore = 0;
					orangeScore = 0;
					greenWins = 0;
					orangeWins = 0;
					greenWon = false;

					showing = false;
					Time.timeScale = 1;
					Screen.showCursor = false;
					instanceCount--;
					Destroy(gameObject);
					Application.LoadLevel(0);
				}
			} else if (orangeWon) {
				GUI.Box(new Rect(padding, padding, Screen.width - padding*2, Screen.height - padding*2), "O R A N G E    W I N S!", boxO);

				//Return to menu button
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 120, 240, 60), "Main  Menu", button)) {
					greenScore = 0;
					orangeScore = 0;
					greenWins = 0;
					orangeWins = 0;
					orangeWon = false;
					
					showing = false;
					Time.timeScale = 1;
					Screen.showCursor = false;
					instanceCount--;
					Destroy(gameObject);
					Application.LoadLevel(0);
				}
			}  else {
				GUI.Box(new Rect(padding, padding , Screen.width - padding*2, Screen.height - padding*2), "S  t a  t u  s", box);

				//Level selection buttons
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 240, width, 40), "Choose next level:", label);

				if (GUI.Button(new Rect(Screen.width / 2 - 300 - 112, Screen.height - 200, 224, 128), level1, blank)) {
					currentLevel = 1;
					goToCurrentLevel();
				}
				
				if (GUI.Button(new Rect(Screen.width / 2 - 112, Screen.height - 200, 224, 128), level2, blank)) {
					currentLevel = 2;
					goToCurrentLevel();
				}
				
				if (GUI.Button(new Rect(Screen.width / 2 + 300 - 112, Screen.height - 200, 224, 128), level3, blank)) {
					currentLevel = 3;
					goToCurrentLevel();
				}
			}

			//GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - 120, width, 40), "Match    Lives: ", label);
			//GUI.Label(new Rect(Screen.width / 2 + width / 2, Screen.height / 2 - 120, width, 40), greenLives.ToString() + " : " + orangeLives.ToString(), label);

			GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - 210, width, 40), "Match    Scores: ", label);
			GUI.Label(new Rect(Screen.width / 2 + width / 2, Screen.height / 2 - 210, width, 40), greenScore.ToString() + " : " + orangeScore.ToString(), label);

			GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - 180, width, 40), "Current    Wins: ", label);
			GUI.Label(new Rect(Screen.width / 2 + width / 2, Screen.height / 2 - 180, width, 40), greenWins.ToString() + " : " + orangeWins.ToString(), label);

			//GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 + 120, width, 40), "Total    Wins: ", label);
			//GUI.Label(new Rect(Screen.width / 2 + width / 2, Screen.height / 2 + 120, width, 40), greenTotalWins.ToString() + " : " + orangeTotalWins.ToString(), label);

			//Green lives selections
/*
			greenLivesSelection = GUI.SelectionGrid(new Rect(Screen.width / 4 - 60, Screen.height / 2 - 148, 128, 64), greenLivesSelection, livesOptions, 1, checkboxL);

			//Green AI selections
			GUI.Label(new Rect(Screen.width / 4 - 60, Screen.height / 2 - 100, 256, 256), "Player  1", labelG);
			greenAISelection = GUI.SelectionGrid(new Rect(Screen.width / 4 - 60, Screen.height / 2 - 60, 128, 128), greenAISelection, AIOptions, 1, checkboxL);

			//Orange lives selections
			orangeLivesSelection = GUI.SelectionGrid(new Rect(Screen.width / 4 * 3 - 60, Screen.height / 2 - 148, 128, 64), orangeLivesSelection, livesOptions, 1, checkboxL);

			//Orange AI selections
			GUI.Label(new Rect(Screen.width / 4 * 3 - 60, Screen.height / 2 - 100, 256, 256), "Player  2", labelO);
			orangeAISelection = GUI.SelectionGrid(new Rect(Screen.width / 4 * 3 - 60, Screen.height / 2 - 60, 128, 128), orangeAISelection, AIOptions, 1, checkboxL);
*/
			greenLivesSelection = GUI.SelectionGrid(new Rect(Screen.width / 4 - 60, Screen.height / 2, 128, 64), greenLivesSelection, livesOptions, 1, checkboxL);

			//Green AI selections
			GUI.Label(new Rect(Screen.width / 4 - 60, Screen.height / 2 - 140, 256, 256), "Player  1", labelG);
			greenAISelection = GUI.SelectionGrid(new Rect(Screen.width / 4 - 60, Screen.height / 2 - 100, 128, 128), greenAISelection, AIOptions, 1, checkboxL);

			//Orange lives selections
			orangeLivesSelection = GUI.SelectionGrid(new Rect(Screen.width / 4 * 3 - 60, Screen.height / 2 , 128, 64), orangeLivesSelection, livesOptions, 1, checkboxL);

			//Orange AI selections
			GUI.Label(new Rect(Screen.width / 4 * 3 - 60, Screen.height / 2 - 140, 256, 256), "Player  2", labelO);
			orangeAISelection = GUI.SelectionGrid(new Rect(Screen.width / 4 * 3 - 60, Screen.height / 2 - 100, 128, 128), orangeAISelection, AIOptions, 1, checkboxL);
		//Showing escape menu
		} else if (escShowing) {
			GUI.Box(new Rect(padding, padding, Screen.width - padding*2, Screen.height - padding*2), "S  t a  t u  s", box);
			
			//Continue button
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 200, 240, 60), "Continue", button)) {
				deactivateEscMenu();
			}

			//Return to menu button
			if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 100, 240, 60), "Menu", button)) {
				deactivateEscMenu();
				instanceCount--;
				Destroy(gameObject);
				Application.LoadLevel(0);
			}

			GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - 240, width, 40), "Match    Scores: ", label);
			GUI.Label(new Rect(Screen.width / 2 + width / 2, Screen.height / 2 - 240, width, 40), greenScore.ToString() + " : " + orangeScore.ToString(), label);
			
			GUI.Label(new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - 200, width, 40), "Current    Wins: ", label);
			GUI.Label(new Rect(Screen.width / 2 + width / 2, Screen.height / 2 - 200, width, 40), greenWins.ToString() + " : " + orangeWins.ToString(), label);
		//In-game
		} else {
			if (showFPS) {
				GUI.Label(new Rect(24, 24, 200, 40), "FPS: " + (1 / Time.deltaTime).ToString(), label);
			}
		}
	}

	private void goToCurrentLevel() {
		showing = false;
		Time.timeScale = 1;
		Screen.showCursor = false;
		Application.LoadLevel(currentLevel);
	}
		
	public int getCurrentLevel() {
		return currentLevel;
	}

	public void setCurrentLevel(int one) {
		currentLevel = one;
	}

	public void handleScore() {
		if (greenLives + orangeLives <= 0) {
			if (greenScore >= orangeScore) {
				greenWins++;
			} else {
				orangeWins++;
			}
			activate();
		}
	}
	
	public void activate() {
		if (greenWins >= maxLevels / 2 + maxLevels % 2) {
			//greenTotalWins++;
			greenWon = true;
			currentLevel = 0;
		} else if (orangeWins >= maxLevels / 2 + maxLevels % 2) {
			//orangeTotalWins++;
			orangeWon = true;
			currentLevel = 0;
		} else {
			currentLevel++;
		}

		levelSelection = -1;
		Screen.showCursor = true;
		Time.timeScale = 0;
		showing = true;
	}

	public void activateBefore() {
		levelSelection = -1;
		Screen.showCursor = true;
		Time.timeScale = 0;
		showing = true;
	}

	public void activateEscMenu() {
		Screen.showCursor = true;
		Time.timeScale = 0;
		escShowing = true;
	}

	private void deactivateEscMenu() {
		Screen.showCursor = false;
		Time.timeScale = 1;
		escShowing = false;
	}
}