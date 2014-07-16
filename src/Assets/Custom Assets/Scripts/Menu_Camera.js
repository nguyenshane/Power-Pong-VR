#pragma strict

function Start () {
}
var target : float;
var speed = 200.0; 
var delay:float;
	
function Update () {
	
	var angle : float = Mathf.MoveTowardsAngle
			(transform.eulerAngles.x, target, speed * Time.deltaTime);
	transform.rotation = Quaternion.Euler(angle,0,0); 
	/*var angle : float = Mathf.MoveTowardsAngle
			(transform.eulerAngles.x, target, speed * Time.deltaTime);
		transform.eulerAngles = Vector3(angle, 0, 0);
		*/
	//Debug.Log("target=" + target);
	
	if(target == 0.0) {
		//Debug.Log("target=" + target);
		
		if (Time.time > delay){
			GameObject.Find("Main Menu Light").GetComponent(Menu_Light).activate = true;
			}	 		
		}	
	if(target == 270.0) {
		if (Time.time > delay){
			GameObject.Find("Options Menu Light").GetComponent(Options_Light).activate = true;
			}
		}
		
	if(target == 90.0) {
		if (Time.time > delay){
			GameObject.Find("Credits Menu Light").GetComponent(Credits_Light).activate = true;
			}
		}
}

function ToOptions(){
	target = 270.0;
	delay = Time.time + 0.5;
}

function ToMain(){
	target = 0.0;
	delay = Time.time + 0.5;
}

function ToCredits(){
	target = 90.0;
	delay = Time.time + 0.5;
}