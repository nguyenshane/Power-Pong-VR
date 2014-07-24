#pragma strict

function Start () {
	GameObject.Find("Point light").light.intensity *= Options_Menu_Trigger.brightness;
	GameObject.Find("Player point light").light.intensity *= Options_Menu_Trigger.brightness;
	//Debug.Log("Point Light=" + GameObject.Find("Point light").light.intensity);

	// Audio volumes
	audio.volume = Options_Menu_Trigger.bgmvolume;
	AudioListener.volume = Options_Menu_Trigger.sfxvolume;
}

function Update () {

}

function First_Drop(){
	//GameObject.Find("BallG").GetComponents("Ball");
	//GameObject.Find("BallO").GetComponents("Ball");
}