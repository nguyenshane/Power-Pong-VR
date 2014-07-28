#pragma strict

function Start () {
	//initAccY = Input.acceleration.y;
	Input.gyro.enabled = true;

	
	// control the title and menu animations
	iTween.MoveTo(GameObject.Find("title"),{"y":6,"time":3});
	iTween.MoveTo(GameObject.Find("Menu Buttons"),{"y":2.382,"time":2.5,"delay":1,"oncomplete":"activatefn","oncompletetarget":gameObject});
	
	
	initialRotation = transform.rotation; 
	gyroInitialRotation = Input.gyro.attitude;
	activate = true;
}

function activatefn(){
	initialRotation = transform.rotation; 
	gyroInitialRotation = Input.gyro.attitude;
	activate = true;
	
}
var speed : float = 3; //how fast the object should rotate
var activate = false;
var initialRotation : UnityEngine.Quaternion;
var gyroInitialRotation : UnityEngine.Quaternion;

function Update(){
Input.gyro.enabled = false;
Input.gyro.enabled = true;
 if (activate){
 	 collider.isTrigger = true;
 	  
 	  var offsetRotation : UnityEngine.Quaternion = Quaternion.Inverse(gyroInitialRotation) * Input.gyro.attitude;
 	  var correctRotation : UnityEngine.Quaternion = initialRotation * offsetRotation;
 	  
      transform.position.y += (offsetRotation.x)*speed;
      //transform.position.y += Input.GetAxis("Mouse Y");
      
      var y = transform.position.y;
      if(y<-6.486) transform.position.y = -6.486;
      if(y>2.964) transform.position.y = 2.964;
  

    //Debug.Log(correctRotation);
      
      
      // Hovers
      /*
      if(y>-0.3 && y<-0.18){
      		iTween.RotateTo(GameObject.Find("Play Cube"),Vector3(0,-16,0),2);
      		iTween.MoveTo(GameObject.Find("Play Cube"),{"z":-2,"time":2});
	 	}else if(y>-0.09 && y<0.09){
	 		iTween.MoveTo(GameObject.Find("Options Cube"),{"z":-2,"time":2});

		} else if(y>0.19 && y<0.3){
			iTween.RotateTo(GameObject.Find("Credits Cube"),Vector3(0,16,0),2);
			iTween.MoveTo(GameObject.Find("Credits Cube"),{"z":-2,"time":2});
		} else {
			iTween.RotateTo(GameObject.Find("Play Cube"),Vector3(0,0,0),2);
			iTween.MoveTo(GameObject.Find("Play Cube"),{"z":0,"time":2});
			iTween.RotateTo(GameObject.Find("Credits Cube"),Vector3(0,0,0),2);
			iTween.MoveTo(GameObject.Find("Credits Cube"),{"z":0,"time":2});
			iTween.MoveTo(GameObject.Find("Options Cube"),{"z":0,"time":2});
		}
      
      
      // Left Clicks
      if(Input.GetMouseButtonDown(0)){
      	if(y>-0.3 && y<-0.18){
			audio.Play();
			//Debug.Log("Pressed Play");
			Application.LoadLevel("Level1");
	 	}else if(y>-0.09 && y<0.09){
			audio.Play();
	 		var linkToScript = GameObject.Find("Camera").GetComponent(Menu_Camera);
	 		linkToScript.ToOptions();
	 		var linkToScript2 = GameObject.Find("Options Menu Light").GetComponent(Options_Light);
	 		linkToScript2.activate = true;
	 		activate = false;
			//Debug.Log("Pressed Options");
		} else if(y>0.19 && y<0.3){
			audio.Play();
			GameObject.Find("Camera").GetComponent(Menu_Camera).ToCredits();
	 		GameObject.Find("Credits Menu Light").GetComponent(Credits_Light).activate = true;
	 		activate = false;
			//Debug.Log("Pressed Credits");
		} //else Debug.Log("Outside");
	}*/
} else {
		collider.isTrigger = false;
		}
 }