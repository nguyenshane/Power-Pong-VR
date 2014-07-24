#pragma strict
/*
This script controls Camera to rotate up, down, middle. It also checks the audio volumes and hides Cursor.
*/
function Start () {
	audio.ignoreListenerVolume = true;
}
var target : float;
var speed = 200.0; 
var delay:float;
	
function Update () {
	// Audio volumes and cursor
	audio.volume = Options_Menu_Trigger.bgmvolume;
	AudioListener.volume = Options_Menu_Trigger.sfxvolume;
	Screen.showCursor = false;
	
	var angle : float = Mathf.MoveTowardsAngle
			(transform.eulerAngles.x, target, speed * Time.deltaTime);
	transform.rotation = Quaternion.Euler(angle,0,0); 
	
	if(target == 0.0) {		
		if (Time.time > delay){
			GameObject.Find("Main Menu Trigger").GetComponent(Main_Menu_Trigger).activate = true;
			}	 		
		}	
	if(target == 270.0) {
		if (Time.time > delay){
			GameObject.Find("Options Menu Trigger").GetComponent(Options_Menu_Trigger).activate = true;
			}
		}
	if(target == 90.0) {
		if (Time.time > delay){
			GameObject.Find("Credits Menu Light").GetComponent(Credits_Light).activate = true;
			}
		}
}

// public functions to be called by other scripts
function ToOptions(){
	target = 270.0;
	delay = Time.time + 0.5;
}

function ToMain(){
	target = 0.0;
	delay = Time.time + 0.5;
}

function ToCredits(){
	target = 90.0;
	delay = Time.time + 0.5;
}