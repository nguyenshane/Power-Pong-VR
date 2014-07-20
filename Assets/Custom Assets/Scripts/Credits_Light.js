#pragma strict

var activate = false;
var timer: float = 3.0;
function Start () {
}

function Update(){
 if (activate){
 	  timer -= Time.deltaTime;
  	  if (timer <= 0){
  	    timer = 3.0;
		GameObject.Find("Main Menu Trigger").audio.Play(); 
	 	GameObject.Find("SBCamera").GetComponent(Menu_Camera).ToMain();
	 	activate = false;
	 	
	}
  }
}