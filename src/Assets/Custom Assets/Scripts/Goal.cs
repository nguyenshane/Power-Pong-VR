using UnityEngine;
using System.Collections;

public enum eGoal
{
	//Left = Green
	//Right = Orange
	Left,
	Right
}

public class Goal : MonoBehaviour {
	public eGoal goal;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision Collection) {

	}

}
