using UnityEngine;
using System.Collections;

/*
 * Handles and draws overlay menus (esc and beginning of each match), handles level transitions
 * Draws in-game UI elements (score/lives indicators and optional framerate display)
 */

public class ScoreScreen : MonoBehaviour {

	public bool seebrightEnabled;
	public bool showFPS;
	public int maxLevels;

	public GUIStyle blank;
	public GUIStyle box, boxG, boxO;
	public GUIStyle label, labelG, labelO;
	public GUIStyle button;
	public GUIStyle checkboxL, checkboxR;
	public Texture level1, level2, level3;

	public int currentLevel;
	public int greenLives, orangeLives, greenScore, orangeScore, greenWins, orangeWins, greenTotalWins, orangeTotalWins;

	static int instanceCount = 0;

	int padding = 0;
	int width = 240;
	int screenWidth, screenHeight;
	bool showing, escShowing;
	bool greenWon = false, orangeWon = false;
	public static int greenAISelection, orangeAISelection, greenLivesSelection, orangeLivesSelection;
	public static int levelSelection;
	string[] AIOptions = new string[] {"Human", "Easy   AI", "Medium   AI", "Hard   AI"};
	string[] livesOptions = new string[] {"3   Lives", "5   Lives"};
	float screenRatio;
	float waitTime = 3.0f;
	float seebrightTimer;

	// Use this for initialization
	void Start () {
		instanceCount++;

		if (instanceCount > 1) {
			instanceCount--;
			Destroy(gameObject);
			return;
		}

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		if (seebrightEnabled) {
			screenWidth /= 2;
		}

		screenRatio = screenWidth / 1080.0f;
		padding = (int)(padding * screenRatio);
		width = (int)(width * screenRatio);

		//Adjust font size and other properties of the GUIStyles based on screen size
		blank.fontSize = (int)(blank.fontSize * screenRatio);
		box.fontSize = (int)(box.fontSize * screenRatio);
		boxG.fontSize = (int)(boxG.fontSize * screenRatio);
		boxO.fontSize = (int)(boxO.fontSize * screenRatio);
		label.fontSize = (int)(label.fontSize * screenRatio);
		labelG.fontSize = (int)(labelG.fontSize * screenRatio);
		labelO.fontSize = (int)(labelO.fontSize * screenRatio);
		button.fontSize = (int)(button.fontSize * screenRatio);
		checkboxL.fontSize = (int)(checkboxL.fontSize * screenRatio);
		checkboxR.fontSize = (int)(checkboxR.fontSize * screenRatio);
		checkboxL.margin.top = (int)(checkboxL.margin.top * screenRatio);
		checkboxL.margin.bottom = (int)(checkboxL.margin.bottom * screenRatio);
		checkboxR.margin.top = (int)(checkboxR.margin.top * screenRatio);
		checkboxR.margin.bottom = (int)(checkboxR.margin.bottom * screenRatio);
		checkboxL.contentOffset *= screenRatio;
		checkboxR.contentOffset *= screenRatio;
		checkboxL.fixedHeight = (int)(checkboxL.fixedHeight * screenRatio);
		checkboxR.fixedHeight = (int)(checkboxR.fixedHeight * screenRatio);
		checkboxL.fixedWidth = (int)(checkboxL.fixedWidth * screenRatio);
		checkboxR.fixedWidth = (int)(checkboxR.fixedWidth * screenRatio);

		seebrightTimer = waitTime;
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

		if (seebrightTimer > 0) seebrightTimer -= 0.01f;
	}

	void OnGUI() {
		//Showing level selection menu
		if (showing == true) {
			if (seebrightEnabled) {
				//currently skips the screen and goes to level 1 with a hard AI for opposing player, put a customized level selection screen here if we want one or make it go to other levels when they're ready for seebright
				if (seebrightTimer > 0) {

					//Green has won
					if (greenWon) {
						GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "G R E E N    W I N S!", boxG);
						
						//Return to menu button
						if (GUI.Button(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 120 * screenRatio, 240 * screenRatio, 60 * screenRatio), "Main  Menu", button)) {
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
						
						//Orange has won
					} else if (orangeWon) {
						GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "O R A N G E    W I N S!", boxO);
						
						//Return to menu button
						if (GUI.Button(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 120 * screenRatio, 240 * screenRatio, 60 * screenRatio), "Main  Menu", button)) {
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
						
						//No winner yet
					} else {
						GUI.Box(new Rect(padding, padding , screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);
						
						//Level selection buttons
						GUI.Label(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 240 * screenRatio, width, 40 * screenRatio), "Choose next level:", label);
						
						if (GUI.Button(new Rect(screenWidth / 2 - (300 + 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level1, blank)) {
							currentLevel = 1;
							goToCurrentLevel();
						}
						
						if (GUI.Button(new Rect(screenWidth / 2 - 112 * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level2, blank)) {
							currentLevel = 2;
							goToCurrentLevel();
						}
						
						if (GUI.Button(new Rect(screenWidth / 2 + (300 - 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level3, blank)) {
							currentLevel = 3;
							goToCurrentLevel();
						}
					}
					
					//Statistics
					GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 210 * screenRatio, width, 40 * screenRatio), "Match    Scores: ", label);
					GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 210 * screenRatio, width, 40 * screenRatio), greenScore.ToString() + " : " + orangeScore.ToString(), label);
					
					GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 180 * screenRatio, width, 40 * screenRatio), "Current    Wins: ", label);
					GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 180 * screenRatio, width, 40 * screenRatio), greenWins.ToString() + " : " + orangeWins.ToString(), label);
					
					//Green selections
					GUI.Label(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2 - 140 * screenRatio, 256 * screenRatio, 256 * screenRatio), "Player  1", labelG);
					greenLivesSelection = GUI.SelectionGrid(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2, 128 * screenRatio, 64 * screenRatio), greenLivesSelection, livesOptions, 1, checkboxL);
					greenAISelection = GUI.SelectionGrid(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2 - 100 * screenRatio, 128 * screenRatio, 128 * screenRatio), greenAISelection, AIOptions, 1, checkboxL);
					
					//Orange selections
					GUI.Label(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 - 140 * screenRatio, 256 * screenRatio, 256 * screenRatio), "Player  2", labelO);
					orangeLivesSelection = GUI.SelectionGrid(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 , 128 * screenRatio, 64 * screenRatio), orangeLivesSelection, livesOptions, 1, checkboxL);
					orangeAISelection = GUI.SelectionGrid(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 - 100 * screenRatio, 128 * screenRatio, 128 * screenRatio), orangeAISelection, AIOptions, 1, checkboxL);

				} else {
					seebrightTimer = waitTime;
					greenScore = orangeScore = 0;
					orangeAISelection = 3;
					currentLevel = 1;
					goToCurrentLevel();
					return;
				}
			} else {
				//Green has won
				if (greenWon) {
					GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "G R E E N    W I N S!", boxG);

					//Return to menu button
					if (GUI.Button(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 120 * screenRatio, 240 * screenRatio, 60 * screenRatio), "Main  Menu", button)) {
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

				//Orange has won
				} else if (orangeWon) {
					GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "O R A N G E    W I N S!", boxO);

					//Return to menu button
					if (GUI.Button(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 120 * screenRatio, 240 * screenRatio, 60 * screenRatio), "Main  Menu", button)) {
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

				//No winner yet
				} else {
					GUI.Box(new Rect(padding, padding , screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);

					//Level selection buttons
					GUI.Label(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 240 * screenRatio, width, 40 * screenRatio), "Choose next level:", label);

					if (GUI.Button(new Rect(screenWidth / 2 - (300 + 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level1, blank)) {
						currentLevel = 1;
						goToCurrentLevel();
					}
					
					if (GUI.Button(new Rect(screenWidth / 2 - 112 * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level2, blank)) {
						currentLevel = 2;
						goToCurrentLevel();
					}
					
					if (GUI.Button(new Rect(screenWidth / 2 + (300 - 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level3, blank)) {
						currentLevel = 3;
						goToCurrentLevel();
					}
				}

				//Statistics
				GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 210 * screenRatio, width, 40 * screenRatio), "Match    Scores: ", label);
				GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 210 * screenRatio, width, 40 * screenRatio), greenScore.ToString() + " : " + orangeScore.ToString(), label);

				GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 180 * screenRatio, width, 40 * screenRatio), "Current    Wins: ", label);
				GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 180 * screenRatio, width, 40 * screenRatio), greenWins.ToString() + " : " + orangeWins.ToString(), label);

				//Green selections
				GUI.Label(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2 - 140 * screenRatio, 256 * screenRatio, 256 * screenRatio), "Player  1", labelG);
				greenLivesSelection = GUI.SelectionGrid(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2, 128 * screenRatio, 64 * screenRatio), greenLivesSelection, livesOptions, 1, checkboxL);
				greenAISelection = GUI.SelectionGrid(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2 - 100 * screenRatio, 128 * screenRatio, 128 * screenRatio), greenAISelection, AIOptions, 1, checkboxL);

				//Orange selections
				GUI.Label(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 - 140 * screenRatio, 256 * screenRatio, 256 * screenRatio), "Player  2", labelO);
				orangeLivesSelection = GUI.SelectionGrid(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 , 128 * screenRatio, 64 * screenRatio), orangeLivesSelection, livesOptions, 1, checkboxL);
				orangeAISelection = GUI.SelectionGrid(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 - 100 * screenRatio, 128 * screenRatio, 128 * screenRatio), orangeAISelection, AIOptions, 1, checkboxL);
			}
		//Showing escape menu
		} else if (escShowing) {
			GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);
			
			//Continue button
			if (GUI.Button(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 200 * screenRatio, 240 * screenRatio, 60 * screenRatio), "Continue", button)) {
				deactivateEscMenu();
			}

			//Return to menu button
			if (GUI.Button(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 100 * screenRatio, 240 * screenRatio, 60 * screenRatio), "Menu", button)) {
				deactivateEscMenu();
				instanceCount--;
				Destroy(gameObject);
				Application.LoadLevel(0);
			}

			GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 240 * screenRatio, width, 40 * screenRatio), "Match    Scores: ", label);
			GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 240 * screenRatio, width, 40 * screenRatio), greenScore.ToString() + " : " + orangeScore.ToString(), label);
			
			GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 200 * screenRatio, width, 40 * screenRatio), "Current    Wins: ", label);
			GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 200 * screenRatio, width, 40 * screenRatio), greenWins.ToString() + " : " + orangeWins.ToString(), label);

		//In-game
		} else {
			if (showFPS) {
				GUI.Label(new Rect(24 * screenRatio, 24 * screenRatio, 200 * screenRatio, 40 * screenRatio), "FPS: " + (1 / Time.deltaTime).ToString(), label);
			}

			//Draw scores and lives
			GUI.Label(new Rect(80 * screenRatio, screenHeight - 100 * screenRatio, 200 * screenRatio, 40 * screenRatio), "S C O R E :     " + greenScore.ToString(), labelG);
			GUI.Label(new Rect(80 * screenRatio, screenHeight - 60 * screenRatio, 200 * screenRatio, 40 * screenRatio), "LI V E S :      " + greenLives.ToString(), labelG);

			GUI.Label(new Rect(screenWidth - 200 * screenRatio, screenHeight - 100 * screenRatio, 200 * screenRatio, 40 * screenRatio), "S C O R E :     " + orangeScore.ToString(), labelO);
			GUI.Label(new Rect(screenWidth - 200 * screenRatio, screenHeight - 60 * screenRatio, 200 * screenRatio, 40 * screenRatio), "LI V E S :      " + orangeLives.ToString(), labelO);
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

		seebrightTimer = waitTime;
		levelSelection = -1;
		Screen.showCursor = true;
		Time.timeScale = 0;
		showing = true;
	}

	public void activateBefore() {
		seebrightTimer = waitTime;
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