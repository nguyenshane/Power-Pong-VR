#pragma strict
/*
This script controls the animations and functions of buttons in Main Menu
*/

function Start() {

}

var activate = true;

function Update() {
	// turn on and off box collider based on screen
	if (activate){
		collider.enabled = true;
 	}
 	else collider.enabled = false;
}

// forward animations and calls to functions
function OnTriggerEnter(other: Collider) {
	if (other.gameObject.name == "Play Cube") {
		iTween.MoveTo(other.gameObject,{"z":-4,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":-6,"time":1,"delay":1.5,"oncomplete":"Play","oncompletetarget":gameObject});
		Debug.Log("Play");
	}
	if (other.gameObject.name == "Options Cube") {
		iTween.MoveTo(other.gameObject,{"z":-4,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":-6,"time":1,"delay":1.5,"oncomplete":"Options","oncompletetarget":gameObject});
		Debug.Log("Options");
	}
	if (other.gameObject.name == "Credits Cube") {
		iTween.MoveTo(other.gameObject,{"z":-4,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":-6,"time":1,"delay":1.5,"oncomplete":"Credits","oncompletetarget":gameObject});
		Debug.Log("Credits");
	}
}

// return animations
function OnTriggerExit(other: Collider) {
	iTween.Stop(other.gameObject, "punch");
	iTween.MoveTo(other.gameObject,{"z":0,"time":2});
}

// functions of the buttons
function Play(){
	audio.Play();
	Application.LoadLevel("Level1");
}

function Options(){
	audio.Play();
	GameObject.Find("SBCamera").GetComponent(Menu_Camera).ToOptions();
	var optionslight = GameObject.Find("Options Menu Trigger").GetComponent(Options_Menu_Trigger);
	optionslight.activate = true;
	activate = false;
}

function Credits(){
	audio.Play();
	GameObject.Find("SBCamera").GetComponent(Menu_Camera).ToCredits();
	var creditslight = GameObject.Find("Credit Menu Light").GetComponent(Credits_Light);
	creditslight.activate = true;
	activate = false;
}