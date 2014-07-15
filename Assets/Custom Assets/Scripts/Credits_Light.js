#pragma strict

var activate = false;
var speed : float = 4.0; //how fast the object should rotate
function Start () {
}

function Update(){
      
 if (activate){
 	  collider.isTrigger = true;
      transform.Rotate(Vector3(0, Input.GetAxis("Mouse X"),0 )  * speed);
      //Debug.Log("x= " + transform.localRotation.x);
      var x = transform.localRotation.x;
      transform.localRotation.z = 0;
      transform.localRotation.y = 0;
      
      if(x<0.7018) transform.localRotation.x = 0.7018;
      if(x>0.7041) transform.localRotation.x = 0.7041;
      
      if(Input.GetMouseButtonDown(0)){
      	//Debug.Log("Pressed Back");
		GameObject.Find("Main Menu Light").audio.Play(); 
	 	GameObject.Find("Camera").GetComponent(Menu_Camera).ToMain();
	 	activate = false;
	}
  }
  else collider.isTrigger = false;
}