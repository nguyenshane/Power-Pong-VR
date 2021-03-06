﻿using UnityEngine;
using System.Collections;

/*
 * Handles and draws overlay menus (esc and beginning of each match), handles level transitions
 * Draws in-game UI elements (score/lives indicators and optional framerate display)
 */

public class ScoreScreen : MonoBehaviour {
	
	private int padding = 0;
	private int width = 440;
	private const float waitTime = 1.0f; //Click timer duration
	private const float waitDistance = 600.0f; //Degrees per frame that the device must be rotated in order to reset the click timer (60 = 1 degree per second)
	private const float cursorSensitivity = 600.0f; //Cursor movement speed multiplier
	private string[] AIOptions = new string[] {"Easy", "Medium", "Hard"};
	private string[] livesOptions = new string[] {"3   Lives", "5   Lives"};
	
	public bool seebrightEnabled;
	public bool useMouseForMenu;
	public bool showFPS;
	public int maxLevels;
	
	public GUIStyle blank;
	public GUIStyle box, boxG, boxO;
	public GUIStyle label, labelG, labelO;
	public GUIStyle button;
	public GUIStyle checkboxL, checkboxR;
	public GUIStyle cursor;
	public GUIStyle border;
	public Texture level1, level2, level3, victory;
	
	public int currentLevel;
	public int greenLives, orangeLives, greenScore, orangeScore, greenWins, orangeWins, greenTotalWins, orangeTotalWins;
	public static int greenAISelection, orangeAISelection, greenLivesSelection, orangeLivesSelection;
	public static bool SeebrightEnabled;
	
	static int instanceCount = 0;
	
	int screenWidth, screenHeight;
	bool showing, escShowing;
	bool greenWon = false, orangeWon = false;
	float screenRatio;
	float seebrightTimer;
	float cursorX, cursorY, originalCursorX, originalCursorY;
	bool isActive;
	Quaternion previousAttitude, currentAttitude;
	Player leftPlayer, rightPlayer;
	Rect returnToMenuButton, continueButton, AIOptionsBox, livesOptionsBox, level1Box, level2Box, level3Box, victoryBox;
	Rect AIOptions0Box, AIOptions1Box, AIOptions2Box, livesOptions0Box, livesOptions1Box;
	Color greenColor, orangeColor;
	
	// Use this for initialization
	void Start () {
		//Singleton
		instanceCount++;
		
		if (instanceCount > 1) {
			instanceCount--;
			Destroy(gameObject);
			return;
		}

		SeebrightEnabled = seebrightEnabled;

		leftPlayer = GameObject.Find ("Player Left").GetComponent<Player>();
		rightPlayer = GameObject.Find ("Player Right").GetComponent<Player>();

		greenColor = GameObject.Find ("Player Left").renderer.material.GetColor("_Color");
		orangeColor = GameObject.Find ("Player Right").renderer.material.GetColor("_Color");
		
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		
		if (seebrightEnabled) {
			screenWidth /= 2;
			Screen.showCursor = false;
		}
		
		//Initial cursor location in center of screen
		originalCursorX = screenWidth / 2;
		originalCursorY = screenHeight / 5 * 2;

		cursorX = originalCursorX;
		cursorY = originalCursorY;
		
		//Initial gyroscope orientation
		Input.gyro.enabled = true;
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
		returnToMenuButton = new Rect (screenWidth / 2 - 120 * screenRatio, screenHeight - 120 * screenRatio, 240 * screenRatio, 60 * screenRatio);
		continueButton = new Rect (screenWidth / 2 - 120 * screenRatio, screenHeight - 200 * screenRatio, 240 * screenRatio, 60 * screenRatio);
		
		AIOptionsBox = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 - 40 * screenRatio, 180 * screenRatio, (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 3);
		AIOptions0Box = new Rect (screenWidth / 2 - 70 * screenRatio - 2, screenHeight / 2 - (40 * screenRatio - (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 0) - 1, 180 * screenRatio + 4, checkboxL.fixedHeight + 4);
		AIOptions1Box = new Rect (screenWidth / 2 - 70 * screenRatio - 2, screenHeight / 2 - (40 * screenRatio - (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 1) - 1, 180 * screenRatio + 4, checkboxL.fixedHeight + 4);
		AIOptions2Box = new Rect (screenWidth / 2 - 70 * screenRatio - 2, screenHeight / 2 - (40 * screenRatio - (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 2) - 1, 180 * screenRatio + 4, checkboxL.fixedHeight + 4);
		
		livesOptionsBox = new Rect (screenWidth / 2 - 70 * screenRatio, screenHeight / 2 + 100 * screenRatio, 180 * screenRatio, (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 2);
		livesOptions0Box = new Rect (screenWidth / 2 - 70 * screenRatio - 2, screenHeight / 2 + (100 * screenRatio + (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 0) - 1, 180 * screenRatio + 4, checkboxL.fixedHeight + 4);
		livesOptions1Box = new Rect (screenWidth / 2 - 70 * screenRatio - 2, screenHeight / 2 + (100 * screenRatio + (checkboxL.fixedHeight + checkboxL.margin.top / 2 + checkboxL.margin.bottom / 2) * 1) - 1, 180 * screenRatio + 4, checkboxL.fixedHeight + 4);
		
		level1Box = new Rect (screenWidth / 2 - (300 + 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio);
		level2Box = new Rect (screenWidth / 2 - 112 * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio);
		level3Box = new Rect (screenWidth / 2 + (300 - 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio);
		victoryBox = new Rect (screenWidth / 2 - 192 * screenRatio, screenHeight / 2 - 92 * screenRatio, 384 * screenRatio, 384 * screenRatio);
		
		seebrightTimer = waitTime;
		greenLives = orangeLives = greenScore = orangeScore = greenWins = orangeWins = greenTotalWins = orangeTotalWins = 0;
		greenAISelection = orangeAISelection = greenLivesSelection = orangeLivesSelection = 0;
		
		//Green player is human (3)
		greenAISelection = 3;
		//Initial difficulty selection is hard (2)
		orangeAISelection = 2;
		
		showing = escShowing = false;
		currentLevel = 2;
		DontDestroyOnLoad(transform.gameObject); //Keeps the object persistent between levels, allows it to retain information although static variables might be able to do the same
		activateBefore();
	}
	
	// Update is called once per frame
	void Update () {
		currentAttitude = Input.gyro.attitude;

		if (Input.GetKeyUp(KeyCode.Escape) && !showing) {
			if (!escShowing) activateEscMenu();
			else deactivateEscMenu();
		}

		/*
		//Activating menu by looking down (or up?)
		if (Mathf.Abs(Vector3.Dot(Input.gyro.gravity, Vector3.down) / Input.gyro.gravity.magnitude) > 0.5f && !showing && !escShowing) {
			activateEscMenu();
		}
		*/
				
		if (showing || escShowing) {
			//Cursor
			//Framerate dependent since timeScale = 0
			if ((showing || escShowing) && seebrightEnabled) {
				float gyroChange = Mathf.Abs(Quaternion.Angle(currentAttitude, previousAttitude));

				bool prevActive = isActive;
				if (checkActive() && !prevActive) GameObject.Find("PongSound").audio.Play();

				//Reset timer if movement is too fast or the cursor isn't over something
				if (gyroChange > waitDistance || !isActive) seebrightTimer = waitTime;
				else if (seebrightTimer > 0) seebrightTimer -= 1f / 60;

				//Cursor movement
				if (useMouseForMenu) {
					cursorX = Input.mousePosition.x;
					cursorY = -Input.mousePosition.y + screenHeight;
				} else {
					/*
					//Accelerometer
					cursorX += Input.acceleration.x * cursorSensitivity;
					cursorY += Input.acceleration.y * cursorSensitivity;
					*/

					/*
					//Working gyroscope
					Quaternion offset = Quaternion.Inverse(previousAttitude) * currentAttitude;
					cursorX += offset.y * cursorSensitivity;
					cursorY += -offset.x * cursorSensitivity;
					*/

					//Better working gyroscope
					Quaternion offset = Quaternion.Inverse(previousAttitude) * currentAttitude;
					//cursorX = originalCursorX + (offset.y * cursorSensitivity);
					cursorY = originalCursorY + (-offset.x * cursorSensitivity);

					/*
					//Not working gyroscope
					if (currentAttitude.x - previousAttitude.x < 0) cursorX += (currentAttitude.x - previousAttitude.x + 360f) * cursorSensitivity;
					else cursorX += (currentAttitude.x - previousAttitude.x) * cursorSensitivity;

					if (currentAttitude.y - previousAttitude.y < 0) cursorY += (currentAttitude.y - previousAttitude.y + 360f) * cursorSensitivity;
					else cursorY += (currentAttitude.y - previousAttitude.y) * cursorSensitivity;
					*/
				}

				//previousAttitude = currentAttitude;
				
				if (cursorX < 0) cursorX = 0;
				else if (cursorX > screenWidth) cursorX = screenWidth;
				
				if (cursorY < 0) cursorY = 0;
				else if (cursorY > screenHeight) cursorY = screenHeight;
			}
		}
	}
	
	void OnGUI() {
		if (seebrightEnabled) {
			drawSeebrightGUI(0); //Left GUI
			drawSeebrightGUI(screenWidth); //Right GUI
			handleCursor(); //Both cursors
		} else {
			drawNormalGUI();
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
			currentLevel = 2;
		} else if (orangeWins >= maxLevels / 2 + maxLevels % 2) {
			//orangeTotalWins++;
			orangeWon = true;
			currentLevel = 2;
		} else {
			currentLevel++;
		}
		
		if (seebrightEnabled) {
			cursorX = screenWidth / 2;
			cursorY = screenHeight / 5 * 2;
			seebrightTimer = waitTime;
		} else Screen.showCursor = true;

		Time.timeScale = 0;
		showing = true;
		GameObject camera = GameObject.Find("SBCamera");
		camera.GetComponent<CameraShake> ().notStopped = false;
	}
	
	//Activates score screen but doesn't do the automatic level transition (no effect since the player now selects them manually) and doesn't determine a winner
	public void activateBefore() {
		if (seebrightEnabled) {
			cursorX = screenWidth / 2;
			cursorY = screenHeight / 5 * 2;
			seebrightTimer = waitTime;
		} else Screen.showCursor = true;

		Time.timeScale = 0;
		showing = true;
		GameObject camera = GameObject.Find("SBCamera");
		camera.GetComponent<CameraShake> ().notStopped = false;
	}
	
	public void activateEscMenu() {
		if (seebrightEnabled) {
			cursorX = screenWidth / 2;
			cursorY = screenHeight / 5 * 2;
			seebrightTimer = waitTime;
		} else Screen.showCursor = true;

		Time.timeScale = 0;
		escShowing = true;
		GameObject camera = GameObject.Find("SBCamera");
		camera.GetComponent<CameraShake> ().notStopped = false;
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
		Application.LoadLevel(1);
	}

	//Checks if the cursor is over an element
	private bool checkActive() {
		isActive = false;

		if (showing) {
			if (greenWon || orangeWon) {
				//Return to menu button
				if (returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				}
			} else {
				/*
				//Level selection buttons
				if (level1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				} else if (level2Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				} else if (level3Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				}
				*/

				//Continue button
				if (continueButton.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				}
				
				//Options
				if (AIOptions0Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				} else if (AIOptions1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				} else if (AIOptions2Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				}
				
				if (livesOptions0Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				} else if (livesOptions1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					isActive = true;
					return true;
				}
			}
		} else if (escShowing) {
			//Continue button
			if (continueButton.Contains(new Vector3(cursorX, cursorY, 0))) {
				isActive = true;
				return true;
			}
			
			//Return to menu button
			if (returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
				isActive = true;
				return true;
			}
		}

		return false;
	}

	private void drawBorder(Rect box) {
		GUI.Label(box, "", border);
	}

	//Handles cursor interactions, draws border for selected object, and draws the cursor (should only be called once per GUI frame)
	private void handleCursor() {
		if (showing) {
			if (greenWon || orangeWon) {
				//Return to menu button
				if (returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(returnToMenuButton);
					drawBorder(new Rect(returnToMenuButton.left + screenWidth, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						greenWon = false;
						returnToMenu();
					}
				}
			} else {
				//Continue button
				if (continueButton.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(continueButton);
					drawBorder(new Rect(continueButton.left + screenWidth, continueButton.top, continueButton.width, continueButton.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						goToCurrentLevel();
					}
				}

				/*
				//Level selection buttons
				if (level1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(level1Box);
					drawBorder(new Rect(level1Box.left + screenWidth, level1Box.top, level1Box.width, level1Box.height));

					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						currentLevel = 2;
						goToCurrentLevel();
					}
				} else if (level2Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(level2Box);
					drawBorder(new Rect(level2Box.left + screenWidth, level2Box.top, level2Box.width, level2Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						currentLevel = 3;
						goToCurrentLevel();
					}
				} else if (level3Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(level3Box);
					drawBorder(new Rect(level3Box.left + screenWidth, level3Box.top, level3Box.width, level3Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						currentLevel = 4;
						goToCurrentLevel();
					}
				}
				*/

				//Options
				if (AIOptions0Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(AIOptions0Box);
					drawBorder(new Rect(AIOptions0Box.left + screenWidth, AIOptions0Box.top, AIOptions0Box.width, AIOptions0Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeAISelection = 0;
					}
				} else if (AIOptions1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(AIOptions1Box);
					drawBorder(new Rect(AIOptions1Box.left + screenWidth, AIOptions1Box.top, AIOptions1Box.width, AIOptions1Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeAISelection = 1;
					}
				} else if (AIOptions2Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(AIOptions2Box);
					drawBorder(new Rect(AIOptions2Box.left + screenWidth, AIOptions2Box.top, AIOptions2Box.width, AIOptions2Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeAISelection = 2;
					}
				}
				
				if (livesOptions0Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(livesOptions0Box);
					drawBorder(new Rect(livesOptions0Box.left + screenWidth, livesOptions0Box.top, livesOptions0Box.width, livesOptions0Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeLivesSelection = greenLivesSelection = 0;
					}
				} else if (livesOptions1Box.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(livesOptions1Box);
					drawBorder(new Rect(livesOptions1Box.left + screenWidth, livesOptions1Box.top, livesOptions1Box.width, livesOptions1Box.height));
					
					if (seebrightTimer <= 0) {
						seebrightTimer = waitTime;
						orangeLivesSelection = greenLivesSelection = 1;
					}
				}
			}
		} else if (escShowing) {
			//Continue button
			if (continueButton.Contains(new Vector3(cursorX, cursorY, 0))) {
				drawBorder(continueButton);
				drawBorder(new Rect(continueButton.left + screenWidth, continueButton.top, continueButton.width, continueButton.height));
				
				if (seebrightTimer <= 0) {
					seebrightTimer = waitTime;
					deactivateEscMenu();
				}
			}
			
			//Return to menu button
			if (returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
				drawBorder(returnToMenuButton);
				drawBorder(new Rect(returnToMenuButton.left + screenWidth, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height));
				
				if (seebrightTimer <= 0) {
					seebrightTimer = waitTime;
					greenWon = false;
					returnToMenu();
				}
			}
		} else {
			//Don't draw cursor
			return;
		}
		
		//Draw cursor sprites
		GUI.Label(new Rect(cursorX - cursor.fixedWidth / 2.75f, cursorY - cursor.fixedHeight / 4.5f, cursor.fixedWidth, cursor.fixedHeight), "", cursor);
		GUI.Label(new Rect(cursorX - cursor.fixedWidth / 2.75f + screenWidth, cursorY - cursor.fixedHeight / 4.5f, cursor.fixedWidth, cursor.fixedHeight), "", cursor);
	}

	private void drawStatistics(int offset) {
		GUI.Label(new Rect(screenWidth / 2 - width / 2 + offset, screenHeight / 2 - 320 * screenRatio, width, 40 * screenRatio), "Match    Scores: ", label);
		GUI.Label(new Rect(screenWidth / 2 + width / 3 + offset, screenHeight / 2 - 320 * screenRatio, width / 1.5f, 40 * screenRatio), greenScore.ToString() + " : " + orangeScore.ToString(), label);
		
		GUI.Label(new Rect(screenWidth / 2 - width / 2 + offset, screenHeight / 2 - 200 * screenRatio, width, 40 * screenRatio), "Current    Wins: ", label);
		GUI.Label(new Rect(screenWidth / 2 + width / 3 + offset, screenHeight / 2 - 200 * screenRatio, width / 1.5f, 40 * screenRatio), greenWins.ToString() + " : " + orangeWins.ToString(), label);
	}

	//Draws the level selection menu on one side, does not handle the cursor
	private void drawSeebrightGUI(int offset) {
		if (showing) {
			//Green has won
			if (greenWon) {
				GUI.Box(new Rect(padding + offset, padding, screenWidth - padding*2, screenHeight - padding*2), "G R E E N    W I N S!", boxG);
				
				//Return to menu button
				if (GUI.Button(new Rect(returnToMenuButton.left + offset, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height), "Main  Menu", button)) {
					drawBorder(new Rect(returnToMenuButton.left + offset, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height));
					
					greenWon = false;
					returnToMenu();
				}

				//Green victory icon
				GUI.contentColor = greenColor;
				GUI.Label(new Rect(victoryBox.left + offset, victoryBox.top, victoryBox.width, victoryBox.height), victory, blank);
				GUI.contentColor = Color.white;
				
			//Orange has won
			} else if (orangeWon) {
				GUI.Box(new Rect(padding + offset, padding, screenWidth - padding*2, screenHeight - padding*2), "O R A N G E    W I N S!", boxO);
				
				//Return to menu button
				if (GUI.Button(new Rect(returnToMenuButton.left + offset, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height), "Main  Menu", button) || returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
					drawBorder(new Rect(returnToMenuButton.left + offset, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height));

					orangeWon = false;
					returnToMenu();
				}

				//Orange victory icon
				GUI.contentColor = orangeColor;
				GUI.Label(new Rect(victoryBox.left + offset, victoryBox.top, victoryBox.width, victoryBox.height), victory, blank);
				GUI.contentColor = Color.white;
				
			//No winner yet
			} else {
				GUI.Box(new Rect(padding + offset, padding, screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);
				
				//Continue button
				if (GUI.Button(new Rect(continueButton.left + offset, continueButton.top, continueButton.width, continueButton.height), "Continue", button)) {
					drawBorder(new Rect(continueButton.left + offset, continueButton.top, continueButton.width, continueButton.height));
					
					goToCurrentLevel();
				}

				//Options
				orangeAISelection = GUI.SelectionGrid(new Rect(AIOptionsBox.left + offset, AIOptionsBox.top, AIOptionsBox.width, AIOptionsBox.height), orangeAISelection, AIOptions, 1, checkboxL);
				orangeLivesSelection = greenLivesSelection = GUI.SelectionGrid(new Rect(livesOptionsBox.left + offset, livesOptionsBox.top, livesOptionsBox.width, livesOptionsBox.height), orangeLivesSelection, livesOptions, 1, checkboxL);

				/*
				//Level selection buttons
				GUI.Label(new Rect(screenWidth / 2 - 160 * screenRatio + offset, screenHeight - 240 * screenRatio, width * 2, 60 * screenRatio), "Choose next level:", label);
				
				if (GUI.Button(new Rect(level1Box.left + offset, level1Box.top, level1Box.width, level1Box.height), level1, blank)) {
					drawBorder(new Rect(level1Box.left + offset, level1Box.top, level1Box.width, level1Box.height));

					currentLevel = 2;
					goToCurrentLevel();
				}
				
				if (GUI.Button(new Rect(level2Box.left + offset, level2Box.top, level2Box.width, level2Box.height), level2, blank)) {
					drawBorder(new Rect(level2Box.left + offset, level2Box.top, level2Box.width, level2Box.height));
					
					currentLevel = 3;
					goToCurrentLevel();
				}
				
				if (GUI.Button(new Rect(level3Box.left + offset, level3Box.top, level3Box.width, level3Box.height), level3, blank)) {
					drawBorder(new Rect(level3Box.left + offset, level3Box.top, level3Box.width, level3Box.height));
					
					currentLevel = 4;
					goToCurrentLevel();
				}
				*/
			}
			
			//Statistics
			drawStatistics(offset);
					
		//Showing escape menu
		} else if (escShowing) {
			GUI.Box(new Rect(padding + offset, padding, screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);

			//Continue button
			if (GUI.Button(new Rect(continueButton.left + offset, continueButton.top, continueButton.width, continueButton.height), "Continue", button)) {
				drawBorder(new Rect(continueButton.left + offset, continueButton.top, continueButton.width, continueButton.height));
				
				deactivateEscMenu();
			}
			
			//Return to menu button
			if (GUI.Button(new Rect(returnToMenuButton.left + offset, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height), "Main  Menu", button)) {
				drawBorder(new Rect(returnToMenuButton.left + offset, returnToMenuButton.top, returnToMenuButton.width, returnToMenuButton.height));
				
				greenWon = false;
				returnToMenu();
			}
			
			//Statistics
			drawStatistics(offset);

		//In-game
		} else {
			if (showFPS) GUI.Label(new Rect(24 * screenRatio + offset, 24 * screenRatio, 400 * screenRatio, 80 * screenRatio), "FPS: " + (1 / Time.deltaTime).ToString(), label);
			
			//Draw scores and lives
			GUI.Label(new Rect(80 * screenRatio + offset, screenHeight - 120 * screenRatio, 200 * screenRatio, 40 * screenRatio), "S C O R E :     " + greenScore.ToString(), labelG);
			GUI.Label(new Rect(80 * screenRatio + offset, screenHeight - 80 * screenRatio, 200 * screenRatio, 40 * screenRatio), "LI V E S :      " + greenLives.ToString(), labelG);
			
			GUI.Label(new Rect(screenWidth - 300 * screenRatio + offset, screenHeight - 120 * screenRatio, 200 * screenRatio, 40 * screenRatio), "S C O R E :     " + orangeScore.ToString(), labelO);
			GUI.Label(new Rect(screenWidth - 300 * screenRatio + offset, screenHeight - 80 * screenRatio, 200 * screenRatio, 40 * screenRatio), "LI V E S :      " + orangeLives.ToString(), labelO);
		}
	}

	private void drawNormalGUI() {
		//Showing level selection menu
		if (showing) {
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
					Application.LoadLevel(1);
				}
				
			//No winner yet
			} else {
				GUI.Box(new Rect(padding, padding , screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);
				
				//Level selection buttons
				GUI.Label(new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight - 240 * screenRatio, width, 40 * screenRatio), "Choose next level:", label);
				
				if (GUI.Button(new Rect(screenWidth / 2 - (300 + 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level1, blank)) {
					currentLevel = 2;
					goToCurrentLevel();
				}
				
				if (GUI.Button(new Rect(screenWidth / 2 - 112 * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level2, blank)) {
					currentLevel = 3;
					goToCurrentLevel();
				}
				
				if (GUI.Button(new Rect(screenWidth / 2 + (300 - 112) * screenRatio, screenHeight - 200 * screenRatio, 224 * screenRatio, 128 * screenRatio), level3, blank)) {
					currentLevel = 4;
					goToCurrentLevel();
				}

				//Green selections
				GUI.Label(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2 - 140 * screenRatio, 256 * screenRatio, 256 * screenRatio), "Player  1", labelG);
				greenLivesSelection = GUI.SelectionGrid(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2, 128 * screenRatio, 64 * screenRatio), greenLivesSelection, livesOptions, 1, checkboxL);
				greenAISelection = GUI.SelectionGrid(new Rect(screenWidth / 4 - 60 * screenRatio, screenHeight / 2 - 100 * screenRatio, 128 * screenRatio, 128 * screenRatio), greenAISelection, AIOptions, 1, checkboxL);
				
				//Orange selections
				GUI.Label(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 - 140 * screenRatio, 256 * screenRatio, 256 * screenRatio), "Player  2", labelO);
				orangeLivesSelection = GUI.SelectionGrid(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 , 128 * screenRatio, 64 * screenRatio), orangeLivesSelection, livesOptions, 1, checkboxL);
				orangeAISelection = GUI.SelectionGrid(new Rect(screenWidth / 4 * 3 - 60 * screenRatio, screenHeight / 2 - 100 * screenRatio, 128 * screenRatio, 128 * screenRatio), orangeAISelection, AIOptions, 1, checkboxL);
			}
			
			//Statistics
			drawStatistics(0);

		//Showing escape menu
		} else if (escShowing) {
			GUI.Box(new Rect(padding, padding, screenWidth - padding*2, screenHeight - padding*2), "S  t a  t u  s", box);
			
			//Continue button
			if (GUI.Button(continueButton, "Continue", button) || returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
				drawBorder(continueButton);
				
				if (seebrightTimer <= 0) {
					seebrightTimer = waitTime;
					deactivateEscMenu();
				}
			}
			
			//Return to menu button
			if (GUI.Button(returnToMenuButton, "Main  Menu", button) || returnToMenuButton.Contains(new Vector3(cursorX, cursorY, 0))) {
				drawBorder(returnToMenuButton);
				
				if (seebrightTimer <= 0) {
					seebrightTimer = waitTime;
					greenWon = false;
					returnToMenu();
				}
			}
			
			//Statistics
			drawStatistics(0);
			
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
}