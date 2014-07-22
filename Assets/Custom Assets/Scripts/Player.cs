using UnityEngine;
using System.Collections;

/*
 * Handles paddle movement, AI and player input, and powerup effects that change the paddle
 */

public enum ePlayer 
{
	Left,
	Right
}

public class Player : MonoBehaviour {

	public float speed = 32f;
	public float inputSpeed = 0f;
	public float maxSize = 16f;
	public float minSize = 4f;
	public float friction = 0.8f;
	public GameObject ball1;
	public GameObject ball2;
	public ePlayer player;

	public bool AIEnabled = false;
	public float reactionTime = 0f;
	public float accuracy = 0f;
	public float interceptRange = 15;

	private Ball b1, b2;
	private float reactionTimer;
	private float centerDistance = 4.0f;
	
	
	void Start() {
		b1 = ball1.GetComponent<Ball>();
		b2 = ball2.GetComponent<Ball>();

		updateOptions();

		reactionTimer = reactionTime;
	}


	// Update is called once per frame
	void Update () {
		if (AIEnabled) {
			if (reactionTimer <= 0f) {
				reactionTimer = reactionTime;
				
				Vector3 b1pos = b1.transform.position;
				Vector3 b2pos = b2.transform.position;
				
				Vector3 b1vel = new Vector3(b1.rigidbody.velocity.x * (1 + Random.Range(-accuracy, accuracy)), b1.rigidbody.velocity.y * (1 + Random.Range(-accuracy, accuracy)), b1.rigidbody.velocity.z * (1 + Random.Range(-accuracy, accuracy)));
				Vector3 b2vel = new Vector3(b2.rigidbody.velocity.x * (1 + Random.Range(-accuracy, accuracy)), b2.rigidbody.velocity.y * (1 + Random.Range(-accuracy, accuracy)), b2.rigidbody.velocity.z * (1 + Random.Range(-accuracy, accuracy)));

				float z = transform.position.z;
				float x = transform.position.x;

				float b1time = (Mathf.Abs(b1pos.x - x)) / Mathf.Abs(b1vel.x);
				float b2time = (Mathf.Abs(b2pos.x - x)) / Mathf.Abs(b2vel.x);
				
				float b1z = b1time * b1vel.z;
				float b2z = b2time * b2vel.z;

				
				if (player == ePlayer.Left) {
					z += transform.localScale.z / 2;

					if (b1vel.x >= 0f || b1pos.x < x) {
						if (b2vel.x >= 0f || b2pos.x < x) {
							//Go back to center, both balls moving away
							if (z > centerDistance) inputSpeed = -1;
							else if (z < -centerDistance) inputSpeed = 1;
							else inputSpeed = 0;
						} else {
							//Intercept ball 2, only ball 1 moving away
							if (Mathf.Abs(b2z) > interceptRange) {
								//Go back to center
								if (z > centerDistance) inputSpeed = -1;
								else if (z < -centerDistance) inputSpeed = 1;
								else inputSpeed = 0;
							} else {
								if (b2z < z) inputSpeed = -1;
								else inputSpeed = 1;
							}
						}
					} else if (b2vel.x >= 0f || b2pos.x < x) {
						//Intercept ball 1, only ball 2 moving away
						if (Mathf.Abs(b1z) > interceptRange) {
							//Go back to center
							if (z > centerDistance) inputSpeed = -1;
							else if (z < -centerDistance) inputSpeed = 1;
							else inputSpeed = 0;
						} else {
							if (b1z < z) inputSpeed = -1;
							else inputSpeed = 1;
						}
					} else {
						//Figure out which ball will reach the goal first
						if (b1time < b2time) {
							//Intercept ball 1
							if (Mathf.Abs(b1z) > interceptRange) {
								//Intercept ball 2
								if (Mathf.Abs(b2z) > interceptRange) {
									//Go back to center
									if (z > centerDistance) inputSpeed = -1;
									else if (z < -centerDistance) inputSpeed = 1;
									else inputSpeed = 0;
								} else {
									if (b2z < z) inputSpeed = -1;
									else inputSpeed = 1;
								}
							} else {
								if (b1z < z) inputSpeed = -1;
								else inputSpeed = 1;
							}
						} else {
							//Intercept ball 2
							if (Mathf.Abs(b2z) > interceptRange) {
								//Intercept ball 1
								if (Mathf.Abs(b1z) > interceptRange) {
									//Go back to center
									if (z > centerDistance) inputSpeed = -1;
									else if (z < -centerDistance) inputSpeed = 1;
									else inputSpeed = 0;
								} else {
									if (b1z < z) inputSpeed = -1;
									else inputSpeed = 1;
								}
							} else {
								if (b2z < z) inputSpeed = -1;
								else inputSpeed = 1;
							}
						}
					}
				} else if (player == ePlayer.Right) {
					z -= transform.localScale.z / 2;

					if (b1vel.x <= 0f || b1pos.x > x) {
						if (b2vel.x <= 0f || b2pos.x < x) {
							//Go back to center, both balls moving away
							if (z > centerDistance) inputSpeed = -1;
							else if (z < -centerDistance) inputSpeed = 1;
							else inputSpeed = 0;
						} else {
							//Intercept ball 2, only ball 1 moving away
							if (Mathf.Abs(b2z) > interceptRange) {
								//Go back to center
								if (z > centerDistance) inputSpeed = -1;
								else if (z < -centerDistance) inputSpeed = 1;
								else inputSpeed = 0;
							} else {
								if (b2z < z) inputSpeed = -1;
								else inputSpeed = 1;
							}
						}
					} else if (b2vel.x <= 0f || b2pos.x < x) {
						//Intercept ball 1, only ball 2 moving away
						if (Mathf.Abs(b1z) > interceptRange) {
							//Go back to center
							if (z > centerDistance) inputSpeed = -1;
							else if (z < -centerDistance) inputSpeed = 1;
							else inputSpeed = 0;
						} else {
							if (b1z < z) inputSpeed = -1;
							else inputSpeed = 1;
						}
					} else {
						//Figure out which ball will reach the goal first
						if (b1time < b2time) {
							//Intercept ball 1
							if (Mathf.Abs(b1z) > interceptRange) {
								//Intercept ball 2
								if (Mathf.Abs(b2z) > interceptRange) {
									//Go back to center
									if (z > centerDistance) inputSpeed = -1;
									else if (z < -centerDistance) inputSpeed = 1;
									else inputSpeed = 0;
								} else {
									if (b2z < z) inputSpeed = -1;
									else inputSpeed = 1;
								}
							} else {
								if (b1z < z) inputSpeed = -1;
								else inputSpeed = 1;
							}
						} else {
							//Intercept ball 2
							if (Mathf.Abs(b2z) > interceptRange) {
								//Intercept ball 1
								if (Mathf.Abs(b1z) > interceptRange) {
									//Go back to center
									if (z > centerDistance) inputSpeed = -1;
									else if (z < -centerDistance) inputSpeed = 1;
									else inputSpeed = 0;
								} else {
									if (b1z < z) inputSpeed = -1;
									else inputSpeed = 1;
								}
							} else {
								if (b2z < z) inputSpeed = -1;
								else inputSpeed = 1;
							}
						}
					}
				}
			} else {
				reactionTimer -= Time.deltaTime;
			}
		} else {
			if (player == ePlayer.Left) {
				inputSpeed = -Input.acceleration.x*(float)2;//Input.GetAxisRaw ("PlayerLeft");
			} else if (player == ePlayer.Right) {
				inputSpeed = Input.GetAxisRaw ("PlayerRight");
			}
		}

		rigidbody.velocity = Vector3.zero; //stop it from drifting around from collisions
		transform.position += new Vector3(0f, 0f, inputSpeed * speed * Time.deltaTime);
	}

	//Size should be > -1.0
	public void increaseSize(float size) {
		float newZScale = transform.localScale.z * (1.0f + size);
		if (newZScale > maxSize) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, maxSize);
		else if (newZScale < minSize) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, minSize);
		else transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZScale);
	}

	public void updateOptions() {
		if (player == ePlayer.Left) {
			switch (ScoreScreen.greenAISelection) {
			case 0:
				AIEnabled = true;
				reactionTime = 0.4f;
				accuracy = 0.1f;
				break;
				
			case 1:
				AIEnabled = true;
				reactionTime = 0.25f;
				accuracy = 0.04f;
				break;
				
			case 2:
				AIEnabled = true;
				reactionTime = 0.1f;
				accuracy = 0.02f;
				break;
				
			case 3:
				AIEnabled = false;
				break;
			}
		} else {
			switch (ScoreScreen.orangeAISelection) {
			case 0:
				AIEnabled = true;
				reactionTime = 0.4f;
				accuracy = 0.1f;
				break;
				
			case 1:
				AIEnabled = true;
				reactionTime = 0.25f;
				accuracy = 0.04f;
				break;
				
			case 2:
				AIEnabled = true;
				reactionTime = 0.1f;
				accuracy = 0.02f;
				break;
				
			case 3:
				AIEnabled = false;
				break;
			}
		}
	}
}