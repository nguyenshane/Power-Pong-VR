#pragma strict

function Start () {
	GameObject.Find("Point light").light.intensity *= Options_Light.brightness;
	//Debug.Log("Point Light=" + GameObject.Find("Point light").light.intensity);
	AudioListener.volume = Options_Light.audiovolume;
}

function Update () {

}

function First_Drop(){
	//GameObject.Find("BallG").GetComponents("Ball");
	//GameObject.Find("BallO").GetComponents("Ball");
}