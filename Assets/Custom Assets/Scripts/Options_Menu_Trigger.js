#pragma strict
/*
This script controls the animations and functions of buttons in Option Menu
*/
function Start() {
	mainlight = GameObject.Find("Main Light").light.intensity;
	optionslight = GameObject.Find("Options Light").light.intensity;
	creditslight = GameObject.Find("Credits Light").light.intensity;
}

var activate = false;
var mainlight: float;
var optionslight: float;
var creditslight:float;

// global variables for game settings
static var bgmvolume:float = 0.5;
static var sfxvolume:float = 0.5;
static var brightness:float = 1.0;

function Update() {
	// control lights in menu
    GameObject.Find("Main Light").light.intensity = brightness * mainlight;
    GameObject.Find("Options Light").light.intensity = brightness * optionslight;
    GameObject.Find("Credits Light").light.intensity = brightness * creditslight;
    
    // turn on and off box collider based on scree
	if (activate){
		collider.enabled = true;
 	}
 	else collider.enabled = false;
}

// forward animations and calls to functions
function OnTriggerEnter(other: Collider) {
	// BGM
	if (other.gameObject.name == "BGM Mute Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"BGM","oncompletetarget":gameObject,"oncompleteparams":"mute"});
	}
	if (other.gameObject.name == "BGM Low Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"BGM","oncompletetarget":gameObject,"oncompleteparams":"minus"});
	}
	if (other.gameObject.name == "BGM High Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"BGM","oncompletetarget":gameObject,"oncompleteparams":"plus"});
	}
	
	// SFX
	if (other.gameObject.name == "SFX Mute Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"SFX","oncompletetarget":gameObject,"oncompleteparams":"mute"});
	}
	if (other.gameObject.name == "SFX Low Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"SFX","oncompletetarget":gameObject,"oncompleteparams":"minus"});
	}
	if (other.gameObject.name == "SFX High Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"SFX","oncompletetarget":gameObject,"oncompleteparams":"plus"});
	}
	
	// Brightness
	if (other.gameObject.name == "Brightness Low Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"Brightness","oncompletetarget":gameObject,"oncompleteparams":"minus"});
	}
	
	if (other.gameObject.name == "Brightness High Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"Brightness","oncompletetarget":gameObject,"oncompleteparams":"plus"});
	}
	
	if (other.gameObject.name == "Options Back Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"Back","oncompletetarget":gameObject});
	}
}

/*
function OnTriggerStay(other: Collider) {
	// BGM
	if (other.gameObject.name == "BGM Mute Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"BGM","oncompletetarget":gameObject,"oncompleteparams":"mute"});
	}
	if (other.gameObject.name == "BGM Low Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"BGM","oncompletetarget":gameObject,"oncompleteparams":"minus"});
	}
	if (other.gameObject.name == "BGM High Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"BGM","oncompletetarget":gameObject,"oncompleteparams":"plus"});
	}
	
	// SFX
	if (other.gameObject.name == "SFX Mute Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"SFX","oncompletetarget":gameObject,"oncompleteparams":"mute"});
	}
	if (other.gameObject.name == "SFX Low Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"SFX","oncompletetarget":gameObject,"oncompleteparams":"minus"});
	}
	if (other.gameObject.name == "SFX High Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"SFX","oncompletetarget":gameObject,"oncompleteparams":"plus"});
	}
	
	// Brightness
	if (other.gameObject.name == "Brightness Low Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"Brightness","oncompletetarget":gameObject,"oncompleteparams":"minus"});
	}
	
	if (other.gameObject.name == "Brightness High Cube") {
		iTween.MoveTo(other.gameObject,{"y":54.3559,"time":2});
		iTween.PunchPosition(other.gameObject,{"z":10,"time":1,"delay":1,
					"oncomplete":"Brightness","oncompletetarget":gameObject,"oncompleteparams":"plus"});
	}
}*/


// return animations
function OnTriggerExit(other: Collider) {
	iTween.Stop(other.gameObject, "punch");
	iTween.MoveTo(other.gameObject,{"y":57.3559,"time":2});
}

// functions of buttons
function BGM(x){
	var mid = GameObject.Find("BGM Line").transform.position.x;
	if(x == "mute") {bgmvolume = 0.0; iTween.MoveTo(GameObject.Find("BGM Slider"),{"x":mid-5,"time":1});}
	if(x == "minus" && bgmvolume >= 0.1) {bgmvolume -= 0.1; iTween.MoveTo(GameObject.Find("BGM Slider"),{"x":mid-5+bgmvolume*10,"time":1});}
	if(x == "plus" && bgmvolume <= 0.9) {bgmvolume += 0.1; iTween.MoveTo(GameObject.Find("BGM Slider"),{"x":mid-5+bgmvolume*10,"time":1});}
	audio.Play();
	Debug.Log("BMG");
}

function SFX(x){
	var mid = GameObject.Find("SFX Line").transform.position.x;
	if(x == "mute") {sfxvolume = 0.0; iTween.MoveTo(GameObject.Find("SFX Slider"),{"x":mid-5,"time":1});}
	if(x == "minus" && sfxvolume >= 0.1) {sfxvolume -= 0.1; iTween.MoveTo(GameObject.Find("SFX Slider"),{"x":mid-5+sfxvolume*10,"time":1});}
	if(x == "plus" && sfxvolume <= 0.9) {sfxvolume += 0.1; iTween.MoveTo(GameObject.Find("SFX Slider"),{"x":mid-5+sfxvolume*10,"time":1});}
	audio.Play();
	Debug.Log("SFX");
}

function Brightness(x){
	if(x == "minus" && brightness >= 0.5) brightness -= 0.5;
	if(x == "plus" && brightness <= 0.95) brightness += 0.5;
	audio.Play();
	Debug.Log("Brightness");
}

function Back(){
	audio.Play();
	GameObject.Find("SBCamera").GetComponent(Menu_Camera).ToMain();
	var mainlight = GameObject.Find("Main Menu Trigger").GetComponent(Main_Menu_Trigger);
	mainlight.activate = true;
	activate = false;
}