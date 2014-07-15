using UnityEngine;
using System.Collections;

public class Brick_Explode : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision Collection) {
		if (Collection.gameObject.name == "BallO" || Collection.gameObject.name == "BallG") {
			Debug.Log("Collision");
			audio.Play();
			Destroy(gameObject);
		}
	}
}
