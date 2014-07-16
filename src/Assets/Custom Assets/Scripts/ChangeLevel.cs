using UnityEngine;
using System.Collections;

public class ChangeLevel : MonoBehaviour {
	private GameObject level;
	private ScoreScreen number;
	// Use this for initialization
	void Start () {
		level = GameObject.Find("ScoreScreen");
		number = level.GetComponent<ScoreScreen>();
	}
	
	// Update is called once per frame
	void Update () {
		if (number.getCurrentLevel() < 1) {
			number.setCurrentLevel(1);
		}
	}
}
