#pragma strict

var speed : float = 3.0; //how fast the object should rotate

var initialRotation : UnityEngine.Quaternion;
var gyroInitialRotation : UnityEngine.Quaternion;


function Start () {
	Input.gyro.enabled = true;
	initialRotation = transform.rotation; 
	gyroInitialRotation = Input.gyro.attitude;
}

function Update(){      
      //transform.Rotate(Vector3(-Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"), 0)  * speed);
      //Debug.Log("x= " + transform.localRotation.y + " y= "+ transform.localRotation.x);
      var y = transform.position.z;
      //var x = transform.localRotation.y;
      //transform.localRotation.z = 0;
      
      var offsetRotation : UnityEngine.Quaternion = Quaternion.Inverse(gyroInitialRotation) * Input.gyro.attitude;
 	  var correctRotation : UnityEngine.Quaternion = initialRotation * offsetRotation;
 	  
      //transform.position.z += (correctRotation.x)*speed;
      transform.position.z += -Input.GetAxis("Mouse Y");
      
      //Debug.Log(transform.position.z);
      
      if(y<-42.46) transform.position.z = -42.46;
      if(y>-15.16) transform.position.z = -15.16;
      
      /*if(x<-0.2) transform.localRotation.y = -0.2;
      if(x>0.2) transform.localRotation.y = 0.2;
      if(y<0.65) transform.localRotation.x = 0.65;
      if(y>0.748) transform.localRotation.x = 0.748;
      
      if(Input.GetMouseButtonDown(0)){
      	if(x>0.11 && y>0.716){
      		// Audio Low
      		audiovolume = 0.2;
      		GameObject.Find("Main Menu Light").audio.Play();      		
			//Debug.Log("Pressed Audio Low");
	 	}else if(x<0.045 && x>-0.065 && y>0.716){
			// Audio Medium
			audiovolume = 0.5;
      		GameObject.Find("Main Menu Light").audio.Play();   
			//Debug.Log("Pressed Audio Medium");
		}else if(x<-0.013 && y>0.716){
      		// Audio High
      		audiovolume = 1;
      		GameObject.Find("Main Menu Light").audio.Play();   
			//Debug.Log("Pressed Audio High");
		}else if(x>0.103 && y>0.67 && y<0.7){
      		// Brightness Low
      		brightness = 0.5;
      		GameObject.Find("Main Menu Light").audio.Play();    
			//Debug.Log("Pressed Brightness Dark");
		}else if(x<0.05 && x>-0.055 && y<0.716 && y>0.698){
      		// Brightness Medium
      		brightness = 1.0;
      		GameObject.Find("Main Menu Light").audio.Play();    
			//Debug.Log("Pressed Brightness Medium");
		}else if(x<-0.11 && y>0.675 && y<0.7){
      		// Brightness High
      		brightness = 1.5;
      		GameObject.Find("Main Menu Light").audio.Play();    
			//Debug.Log("Pressed Brightness Bright");
		}else if(x<0.08 && x>-0.09 && y<0.675){
			// Back
			//Debug.Log("Pressed Back");
			GameObject.Find("Main Menu Light").audio.Play(); 
	 		var linkToScript = GameObject.Find("SBCamera").GetComponent(Menu_Camera);
	 		linkToScript.ToMain();
	 		var linkToScript2 = GameObject.Find("Main Menu Trigger").GetComponent(Main_Menu_Trigger);
	 		linkToScript2.activate = true;
	 		activate = false;
			
		} //else Debug.Log("Outside options");
	}*/
}