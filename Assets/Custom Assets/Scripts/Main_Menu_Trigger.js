#pragma strict

function Start() {

}

function Update() {
	if (activate){
		collider.isTrigger = true;
 	}
 	else collider.isTrigger = false;
}

var activate = true;

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
		iTween.PunchPosition(other.gameObject,{"z":-6,"time":1,"delay":1.5});
		Debug.Log("Credits");
	}

}

function OnTriggerExit(other: Collider) {
	
	iTween.Stop("punch");
	iTween.MoveTo(other.gameObject,{"z":0,"time":2});

}

function Play(){
	audio.Play();
	Application.LoadLevel("Level1");
}

function Options(){
	audio.Play();
	GameObject.Find("SBCamera").GetComponent(Menu_Camera).ToOptions();
	var optionslight = GameObject.Find("Options Menu Light").GetComponent(Options_Light);
	optionslight.activate = true;
	activate = false;
}

function Credits(){
	audio.Play();
	Application.LoadLevel("Level1");
}