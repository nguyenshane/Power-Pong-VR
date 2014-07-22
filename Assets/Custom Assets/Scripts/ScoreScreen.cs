using UnityEngine;
using System.Collections;

/*
 * Handles and draws overlay menus (esc and beginning of each match), handles level transitions
 * Draws in-game UI elements (score/lives indicators and optional framerate display)
 */

public class ScoreScreen : MonoBehaviour {

	private int padding = 0;
	private int width = 320;
	private const float waitTime = 1.0f; //Click timer duration
	private const float waitDistance = 600.0f; //Degrees per frame that the device must be rotated in order to reset the click timer (60 = 1 degree per second)
	private const float cursorSensitivity = 2.0f; //Cursor movement speed multiplier
	private string[] AIOptions = new string[] {"Easy", "Medium", "Hard"};
	private string[] livesOptions = new string[] {"3   Lives", "5   Lives"};

	public bool seebrightEnabled;
	public bool showFPS;
	public int maxLevels;

	public GUIStyle blank;
	public GUIStyle box, boxG, boxO;
	public GUIStyle label, labelG, labelO;
	public GUIStyle button;
	public GUIStyle checkboxL, checkboxR;
	public GUIStyle cursor;
	public Texture level1, level2, level3;

	public int currentLevel;
	public int greenLives, orangeLives, greenScore, orangeScore, greenWins, orangeWins, greenTotalWins, orangeTotalWins;
	public static int greenAISelection, orangeAISelection, greenLivesSelection, orangeLivesSelection;
	public static int levelSelection, dummy;

	static int instanceCount = 0;

	int screenWidth, screenHeight;
	bool showing, escShowing;
	bool greenWon = false, orangeWon = false;
	float screenRatio;
	float seebrightTimer;
	float cursorX, cursorY;
	Quaternion previousAttitude, currentAttitude;
	Player leftPlayer, rightPlayer;
	Rect returnToMenuButton, continueButton, AIOptionsBox, livesOptionsBox, level1Box, level2Box, level3Box;
	Rect AIOptions0Box, AIOptions1Box, AIOptions2Box, livesOptions0Box, livesOptions1Box;

	// Use this for initialization
	void Start () {
		//Singleton
		instanceCount++;

		if (instanceCount > 1) {
			instanceCount--;
			Destroy(gameObject);
			return;
		}

		leftPlayer = GameObject.Find ("Player Left").GetComponent<Player>();
		rightPlayer = GameObject.Find ("Player Right").GetComponent<Player>();

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		if (seebrightEnabled) {
			screenWidth /= 2;
		}

		//Initial cursor location in center of screen
		cursorX = screenWidth / 2;
		cursorY = screenHeight / 2;

		//Initial gyroscope orientation
		previousAttitude = currentAttitude = Input.gyro.attitude;

		screenRatio = screenWidth / 1000.0f;
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
		cursor.fixedHeight = (int)(cursor.fixedHeight * screenRatio);
		cursor.fixedWidth = (int)(cursor.fixedWidth * screenRatio);

		//Bounding boxes for interactable UI elements
		returnToMenuButton = new Rect (screenWidth / 2 - 100 * screenRatio, screenHeight - 120 * screenRatio, 240 * screenRatio, 60 * screenRatio);
		continueButton = new Rect (screenWidth / 2 - 100 * screenRatio, screenHeight - 200 * screenRatio, 240 * screenRatio, 60 * screenRatio);

		AIOptionsBox = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 - 40 * screenRatio, 256 * screenRatio, (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 3);
		AIOptions0Box = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 - (40 * screenRatio - (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 0), 256 * screenRatio, checkboxL.fixedHeight);
		AIOptions1Box = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 - (40 * screenRatio - (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 1), 256 * screenRatio, checkboxL.fixedHeight);
		AIOptions2Box = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 - (40 * screenRatio - (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 2), 256 * screenRatio, checkboxL.fixedHeight);

		livesOptionsBox = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 + 100 * screenRatio, 256 * screenRatio, (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 2);
		livesOptions0Box = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 + (100 * screenRatio + (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 0), 256 * screenRatio, checkboxL.fixedHeight);
		livesOptions1Box = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 + (100 * screenRatio + (checkboxL.fixedHeight + checkboxL.margin.top + checkboxL.margin.bottom) * 1), 256 * screenRatio, checkboxL.fixedHeight);

		level1Box = new Rect (screenWidth / 2 - (300 + 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio);
		level2Box = new Rect (screenWidth / 2 - 112 * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio);
		level3Box = new Rect (screenWidth / 2 + (300 - 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio);

		seebrightTimer = waitTime;
		greenLives = orangeLives = greenScore = orangeScore = greenWins = orangeWins = greenTotalWins = orangeTotalWins = 0;
		greenAISelection = orangeAISelection = greenLivesSelection = orangeLivesSelection = 0;
		levelSelection = -1;

		//Green player is human (3)
		greenAISelection = 3;
		//Initial difficulty selection is hard (2)
		orangeAISelection = 2;

		showing = escShowing = false;
		DontDestroyOnLoad(transform.gameObject); //Keeps the object persistent between levels, allows it to retain information although static variables might be able to do the same
		activateBefore();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape) && !showing) {
			if (!escShowing) activateEscMenu();
			else deactivateEscMenu();
		}

		//Framerate dependent since timeScale = 0
		if ((showing || escShowing) && seebrightEnabled) {
			currentAttitude = Input.gyro.attitude;

			float gyroChange = Mathf.Abs(Quaternion.Angle(currentAttitude, previousAttitude));

			if (gyroChange > waitDistance) seebrightTimer = waitTime;
			else if (seebrightTimer > 0) seebrightTimer -= 1f / 60;

			if (currentAttitude.x - previousAttitude.x < 0) cursorX += (currentAttitude.x - previousAttitude.x + 360f) * cursorSensitivity;
			else cursorX += (currentAttitude.x - previousAttitude.x) * cursorSensitivity;

			if (currentAttitude.y - previousAttitude.y < 0) cursorY += (currentAttitude.y - previousAttitude.y + 360f) * cursorSensitivity;
			else cursorY += (currentAttitude.y - previousAttitude.y) * cursorSensitivity;

			//cursorX = Input.mousePosition.x;
			//cursorY = -Input.mousePosition.y * 2;

			previousAttitude = currentAttitude;

			if (cursorX < 0) cursorX = 0;
			else if (cursorX > screenWidth) cursorX = screenWidth;

			if (cursorY < 0) cursorY = 0;
			else if (cursorY > screenHeight) cursorY = screenHeight;
		}
	}

	void OnGUI() {
		//Showing level selection menu
		if (showing == true) {
			if (seebrightEnabled) {

				//Green has won
				if (greenWon) {
					GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "G R E E N    W I N S!", boxG);
					
					//Return to menu button
					if (GUI.Button(returnToMenuButton, "Main  Menu", button) || returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
						if (seebrightTimer <= 0) {
							seebrightTimer = waitTime;
							greenWon = false;
							returnToMenu();
						}
					}
					
				//Orange has won
				} else if (orangeWon) {
					GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "O R A N G E    W I N S!", boxO);
					
					//Return to menu button
					if (GUI.Button(returnToMenuButton, "Main  Menu", button) || returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
						if (seebrightTimer <= 0) {
							seebrightTimer = waitTime;
							orangeWon = false;
							returnToMenu();
						}
					}
					
				//No winner yet
				} else {
					GUI.Box(new Rect(padding, padding , screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);
					
					//Level selection buttons
					GUI.Label(new Rect(screenWidth / 2 - 160 * screenRatio, screenHeight - 240 * screenRatio, width * 2, 60 * screenRatio), "Choose next level:", label);
					
					if (GUI.Button(level1Box, level1, blank) || level1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
						if (seebrightTimer <= 0) {
							seebrightTimer = waitTime;
							currentLevel = 1;
							goToCurrentLevel();
						}
					}
					
					if (GUI.Button(level2Box, level2, blank) || level2Box.Contains(new Vector3(cursorX, cursorY, 0))) {
						if (seebrightTimer <= 0) {
							seebrightTimer = waitTime;
							currentLevel = 2;
							goToCurrentLevel();
						}
					}
					
					if (GUI.Button(level3Box, level3, blank) || level3Box.Contains(new Vector3(cursorX, cursorY, 0))) {
						if (seebrightTimer <= 0) {
							seebrightTimer = waitTime;
							currentLevel = 3;
							goToCurrentLevel();
						}
					}
				}
				
				//Statistics
				GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 280 * screenRatio, width, 60 * screenRatio), "Match    Scores: ", label);
				GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 280 * screenRatio, width, 60 * screenRatio), greenScore.ToString() + " : " + orangeScore.ToString(), label);
				
				GUI.Label(new Rect(screenWidth / 2 - width / 2, screenHeight / 2 - 200 * screenRatio, width, 60 * screenRatio), "Current    Wins: ", label);
				GUI.Label(new Rect(screenWidth / 2 + width / 2, screenHeight / 2 - 200 * screenRatio, width, 60 * screenRatio), greenWins.ToString() + " : " + orangeWins.ToString(), label);
				
				//Options
				orangeAISelection = GUI.SelectionGrid(AIOptionsBox, orangeAISelection, AIOptions, 1, checkboxL);
				orangeLivesSelection = GUI.SelectionGrid(livesOptionsBox, orangeLivesSelection, livesOptions, 1, checkboxL);

				if (AIOptions0Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeAISelection = 0;
					}
				} else if (AIOptions1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeAISelection = 1;
					}
				} else if (AIOptions2Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeAISelection = 2;
					}
				}

				if (livesOptions0Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeLivesSelection = greenLivesSelection = 0;
					}
				} else if (livesOptions1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeLivesSelection = greenLivesSelection = 1;
					}
				}

				//Draw cursor
				GUI.Label(new Rect(cursorX - cursor.fixedWidth / 2, cursorY - cursor.fixedHeight / 2, cursor.fixedWidth, cursor.fixedHeight), "", cursor);

			//Non-Seebright GUI
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
			if (GUI.Button(continueButton, "Continue", button)) {
				deactivateEscMenu();
			}

			//Return to menu button
			if (GUI.Button(returnToMenuButton, "Menu", button)) {
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
			if (showFPS) GUI.Label(new Rect(24 * screenRatio, 24 * screenRatio, 400 * screenRatio, 80 * screenRatio), "FPS: " + (1 / Time.deltaTime).ToString(), label);

			//Draw scores and lives
			GUI.Label(new Rect(80 * screenRatio, screenHeight - 120 * screenRatio, 200 * screenRatio, 40 * screenRatio), "S C O R E :     " + greenScore.ToString(), labelG);
			GUI.Label(new Rect(80 * screenRatio, screenHeight - 80 * screenRatio, 200 * screenRatio, 40 * screenRatio), "LI V E S :      " + greenLives.ToString(), labelG);

			GUI.Label(new Rect(screenWidth - 300 * screenRatio, screenHeight - 120 * screenRatio, 200 * screenRatio, 40 * screenRatio), "S C O R E :     " + orangeScore.ToString(), labelO);
			GUI.Label(new Rect(screenWidth - 300 * screenRatio, screenHeight - 80 * screenRatio, 200 * screenRatio, 40 * screenRatio), "LI V E S :      " + orangeLives.ToString(), labelO);
		}
	}

	private void goToCurrentLevel() {
		leftPlayer.updateOptions();
		rightPlayer.updateOptions();
		greenScore = orangeScore = 0;
		showing = false;
		Time.timeScale = 1;
		Screen.showCursor = false;
		Application.LoadLevel(currentLevel);
	}
		
	public int getCurrentLevel() {
		return currentLevel;
	}

	public void setCurrentLevel(int level) {
		currentLevel = level;
	}

	public void handleScore() {
		if (greenLives + orangeLives <= 0) {
			if (greenScore >= orangeScore) greenWins++;
			else orangeWins++;

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

	//Activates score screen but doesn't do the automatic level transition (no effect since the player now selects them manually) and doesn't determine a winner
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

	//Returns to main menu (initial game state) and clears the score screen instance completely
	private void returnToMenu() {
		showing = false;
		Time.timeScale = 1;
		Screen.showCursor = false;
		instanceCount--;
		Destroy(gameObject);
		Application.LoadLevel(0);
	}
}