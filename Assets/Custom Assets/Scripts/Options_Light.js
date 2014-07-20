#pragma strict

static var audiovolume:float = 0.5;
static var brightness:float = 1.0;

var activate = false;
var speed : float = 1.0; //how fast the object should rotate
var mainlight: float;
var optionslight: float;
var creditslight:float;
function Start () {
	mainlight = GameObject.Find("Main Light").light.intensity;
	optionslight = GameObject.Find("Options Light").light.intensity;
	creditslight = GameObject.Find("Credits Light").light.intensity;
	Screen.showCursor = false;
}

function Update(){
      AudioListener.volume = audiovolume;
      GameObject.Find("Main Light").light.intensity = brightness * mainlight;
      GameObject.Find("Options Light").light.intensity = brightness * optionslight;
      GameObject.Find("Credits Light").light.intensity = brightness * creditslight;
      
 if (activate){
 	  collider.isTrigger = true;
      transform.Rotate(Vector3(-Input.GetAxis("Mouse Y"),Input.GetAxis("Mouse X"), 0)  * speed);
      //Debug.Log("x= " + transform.localRotation.y + " y= "+ transform.localRotation.x);
      var y = transform.localRotation.x;
      var x = transform.localRotation.y;
      transform.localRotation.z = 0;
      if(x<-0.2) transform.localRotation.y = -0.2;
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
	}
  }
  else collider.isTrigger = false;
}