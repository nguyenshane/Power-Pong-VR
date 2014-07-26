using UnityEngine;
using System.Collections;

/*
 * Handles ball movement and all ball collision events, including fireballs and neutral balls
 */

public enum eBall
{
	Left,
	Right,
	F_Left,
	F_Right
}

public class Ball : MonoBehaviour {

	private int normalBrickScore = 6;
	private int goalBrickScore = 12;
	private float leftImpulse = -4f;
	private float rightImpulse = 4f;
	private float goalPointPercentage = 0.20f;
	private float speedChangeBuffer = 0.1f;

	public GameObject score;
	public GameObject otherScore;
	public GameObject paddle;

	public Vector3 initialImpulse;
	public Vector3 dropLocation;
	public float dropSpeed;
	public float height;
	public float maxSpeed;
	public float minSpeed;
	public float minXSpeed;
	public float initialDropDelay;
	public eBall ball;

	Vector3 leftImpulse_F;
	Vector3 rightImpulse_F;
	float currentDropDelay;
	bool dontDrop = false;
	bool neutral = false;
	float currentSpeedBuffer;


	// Use this for initialization
	void Start () {
		leftImpulse_F = new Vector3(-24, 0, Random.Range (-3.0f, 3.0f));
		rightImpulse_F = new Vector3(24, 0, Random.Range (-3.0f, 3.0f));

		if (ball == eBall.F_Left || ball == eBall.F_Right) {
			Destroy(gameObject, 3.5f);
			initialDropDelay = -2000;
			if (ball == eBall.F_Right) rigidbody.AddForce(leftImpulse_F, ForceMode.Impulse);
			else rigidbody.AddForce(rightImpulse_F, ForceMode.Impulse);
		}

		currentSpeedBuffer = speedChangeBuffer;
		currentDropDelay = initialDropDelay;
	}


	// Update is called once per frame
	void Update () {
		if (ball != eBall.F_Left && ball != eBall.F_Right) {
			if (currentDropDelay > 0) {
				currentDropDelay -= Time.deltaTime;
			} else if (currentDropDelay == -1000) {
				dropBall (dropLocation);
				currentDropDelay = -2000;
			} else if (currentDropDelay != -2000 && !dontDrop) {
				currentDropDelay = -1000;
			}

			if (currentDropDelay == -2000) {
				if (rigidbody.position.y < height) {
					rigidbody.MovePosition(new Vector3 (rigidbody.position.x, height, rigidbody.position.z));
					rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
					collider.isTrigger = false;
					score.GetComponent<Scores>().increaseMultiplier();
					rigidbody.AddForce (initialImpulse * score.GetComponent<Scores>().getMultiplier(), ForceMode.Impulse);
				} else if (rigidbody.position.y == height) {
					if (currentSpeedBuffer > 0) {
						currentSpeedBuffer -= Time.deltaTime;
					}

					if (rigidbody.velocity.magnitude < minSpeed) {
						if (rigidbody.velocity.magnitude != 0) {
							rigidbody.AddForce (rigidbody.velocity * (minSpeed / rigidbody.velocity.magnitude - 1), ForceMode.Impulse);
						} else {
							rigidbody.AddForce (initialImpulse, ForceMode.Impulse);
						}
					} else if (rigidbody.velocity.magnitude > maxSpeed) {
						rigidbody.AddForce (rigidbody.velocity * -1 * (1 - maxSpeed / rigidbody.velocity.magnitude), ForceMode.Impulse);
					}
				}
			}
		}
	}


	void OnCollisionEnter(Collision Collection) {
		//SOUNDS
		if(Collection.gameObject.name != "Brick" 
		   && Collection.gameObject.name != "Orange_Goal"
		   && Collection.gameObject.name != "Green_Goal"
		   && Collection.gameObject.name != "Ground")
		GameObject.Find("PongSound").audio.Play();

		//GREEN BALL
		if (ball == eBall.Left) {
			//WITH ANY BRICK
			if(Collection.gameObject.name == "Brick") {
				audio.Play();
				Destroy(Collection.gameObject);
				if (!neutral) score.GetComponent<Scores>().AddScore(normalBrickScore);
			} 
			//WITH ORANGE GOAL BRICK
			else if (Collection.gameObject.name == "Brick_O") {
				GameObject.Find("KickSound").audio.Play();
				Destroy(Collection.gameObject);
				if (!neutral) score.GetComponent<Scores>().AddScore(goalBrickScore);
			}
			//WITH GREEN GOAL BRICK
			else if (Collection.gameObject.name == "Brick_G") {
				GameObject.Find("KickSound").audio.Play();
				Destroy(Collection.gameObject);
			} 
			//WITH OWN PADDLE
			else if (Collection.gameObject.name == "Player Left") {
				if (currentSpeedBuffer <= 0) {
					Player player = Collection.gameObject.GetComponent<Player>();
					Vector3 playerpos = player.transform.position;
					Vector3 ballpos = transform.position;

					if (!neutral) score.GetComponent<Scores>().AddScore(normalBrickScore/2);
					if (playerpos.z + player.transform.localScale.z / 2 > ballpos.z &&
					    playerpos.z - player.transform.localScale.z / 2 < ballpos.z &&
					    playerpos.x < ballpos.x) {
						transform.position = new Vector3(playerpos.x + player.transform.localScale.x / 2 + transform.localScale.x / 2, ballpos.y, ballpos.z);
						rigidbody.AddForce(new Vector3(rightImpulse, 0, player.friction * (Mathf.Abs(ballpos.z - playerpos.z) / (player.transform.localScale.z / 2)) *
						                               player.inputSpeed * player.speed), ForceMode.Impulse);
					}
				}
			}
			//WITH OTHER PADDLE
			else if (Collection.gameObject.name == "Player Right") {
				if (currentSpeedBuffer <= 0) {
				Player player = Collection.gameObject.GetComponent<Player>();
				Vector3 playerpos = player.transform.position;
				Vector3 ballpos = transform.position;

				if (playerpos.z + player.transform.localScale.z / 2 > ballpos.z &&
				    playerpos.z - player.transform.localScale.z / 2 < ballpos.z &&
				    playerpos.x > ballpos.x) {
					transform.position = new Vector3(playerpos.x - player.transform.localScale.x / 2 - transform.localScale.x / 2, ballpos.y, ballpos.z);
					rigidbody.AddForce(new Vector3(0, 0, player.friction * (Mathf.Abs(ballpos.z - playerpos.z) / (player.transform.localScale.z / 2)) *
					                               player.inputSpeed * player.speed), ForceMode.Impulse);
				}
			}
			//Other goal
			} else if (Collection.gameObject.name == "Orange_Goal" && ball != eBall.F_Left && ball != eBall.F_Right) {
				GameObject.Find("ExplosionSound").audio.Play();

				int points = (int)(otherScore.GetComponent<Scores>().getScore () * goalPointPercentage);
				if (!neutral) score.GetComponent<Scores>().AddScore(points);
				otherScore.GetComponent<Scores>().RemoveLife();
				int lives = (int)(score.GetComponent<Scores>().getLives ());
				if (lives > 0) {
					dropBall(dropLocation);
				} else {
					if (!neutral) convertToNeutral(dropLocation);
					else dropBall(dropLocation);
					int pointDeduction = (int)(score.GetComponent<Scores>().getScore() * goalPointPercentage);
					score.GetComponent<Scores>().AddScore(-pointDeduction);
				}
			//Own goal
			} else if (Collection.gameObject.name == "Green_Goal" && ball != eBall.F_Left && ball != eBall.F_Right) {
				Handheld.Vibrate();
				GameObject.Find("ExplosionSound").audio.Play();

				score.GetComponent<Scores>().RemoveLife();
				int lives = (int)(score.GetComponent<Scores>().getLives ());
				if (lives > 0) {
					dropBall(dropLocation);
				} else {
					if (!neutral) convertToNeutral(dropLocation);
					else dropBall(dropLocation);
					int pointDeduction = (int)(score.GetComponent<Scores>().getScore() * goalPointPercentage);
					score.GetComponent<Scores>().AddScore(-pointDeduction);
				}
			//Side walls
			} else if (Collection.gameObject.name == "Big Wall") {
				if (Mathf.Abs(rigidbody.velocity.x) < minXSpeed) {
					if (rigidbody.velocity.x != 0) rigidbody.AddForce(new Vector3(rigidbody.velocity.x * (minXSpeed / Mathf.Abs(rigidbody.velocity.x) - 1), 0, 0), ForceMode.Impulse);
					else rigidbody.AddForce(new Vector3(-minXSpeed, 0, 0), ForceMode.Impulse);
				}
			}

		//Orange ball
		} else if (ball == eBall.Right) {
			if(Collection.gameObject.name == "Brick") {
				audio.Play();
				Destroy(Collection.gameObject);
				if (!neutral) score.GetComponent<Scores>().AddScore(normalBrickScore);
			//Other goal bricks
			} else if (Collection.gameObject.name == "Brick_G") {
				GameObject.Find("KickSound").audio.Play();
				Destroy(Collection.gameObject);
				if (!neutral) score.GetComponent<Scores>().AddScore(goalBrickScore);
			//Own goal bricks
			} else if (Collection.gameObject.name == "Brick_O") {
				GameObject.Find("KickSound").audio.Play();
				Destroy(Collection.gameObject);
			//Own paddle
			} else if (Collection.gameObject.name == "Player Right") {
				if (currentSpeedBuffer <= 0) {
					Player player = Collection.gameObject.GetComponent<Player>();
					Vector3 playerpos = player.transform.position;
					Vector3 ballpos = transform.position;
					
					if (!neutral) score.GetComponent<Scores>().AddScore(normalBrickScore/2);
					if (playerpos.z + player.transform.localScale.z / 2 > ballpos.z &&
					    playerpos.z - player.transform.localScale.z / 2 < ballpos.z &&
					    playerpos.x > ballpos.x) {
						transform.position = new Vector3(playerpos.x - player.transform.localScale.x / 2 - transform.localScale.x / 2, ballpos.y, ballpos.z);
						rigidbody.AddForce(new Vector3(leftImpulse, 0, player.friction * (Mathf.Abs(ballpos.z - playerpos.z) / (player.transform.localScale.z / 2)) *
						                               player.inputSpeed * player.speed), ForceMode.Impulse);
					}
				}
			//Other paddle
			} else if (Collection.gameObject.name == "Player Left") {
				if (currentSpeedBuffer <= 0) {
					Player player = Collection.gameObject.GetComponent<Player>();
					Vector3 playerpos = player.transform.position;
					Vector3 ballpos = transform.position;
					
					if (playerpos.z + player.transform.localScale.z / 2 > ballpos.z &&
					    playerpos.z - player.transform.localScale.z / 2 < ballpos.z &&
					    playerpos.x < ballpos.x) {
						transform.position = new Vector3(playerpos.x + player.transform.localScale.x / 2 + transform.localScale.x / 2, ballpos.y, ballpos.z);
						rigidbody.AddForce(new Vector3(0, 0, player.friction * (Mathf.Abs(ballpos.z - playerpos.z) / (player.transform.localScale.z / 2)) *
						                               player.inputSpeed * player.speed), ForceMode.Impulse);
					}
				}
			//Other goal
			} else if (Collection.gameObject.name == "Green_Goal" && ball != eBall.F_Left && ball != eBall.F_Right) {
				GameObject.Find("ExplosionSound").audio.Play();

				int points = (int)(otherScore.GetComponent<Scores>().getScore() * goalPointPercentage);
				if (!neutral) score.GetComponent<Scores>().AddScore(points);
				otherScore.GetComponent<Scores>().RemoveLife();
				int lives = (int)(score.GetComponent<Scores>().getLives());
				if (lives > 0) {
					dropBall(dropLocation);
				} else {
					if (!neutral) convertToNeutral(dropLocation);
					else dropBall(dropLocation);
					int pointDeduction = (int)(score.GetComponent<Scores>().getScore() * goalPointPercentage);
					score.GetComponent<Scores>().AddScore(-pointDeduction);
				}
			//Own goal
			} else if (Collection.gameObject.name == "Orange_Goal" && ball != eBall.F_Left && ball != eBall.F_Right) {
				GameObject.Find("ExplosionSound").audio.Play();

				score.GetComponent<Scores>().RemoveLife();
				int lives = (int)(score.GetComponent<Scores>().getLives ());
				if (lives > 0) {
					dropBall(dropLocation);
				} else {
					if (!neutral) convertToNeutral(dropLocation);
					else dropBall(dropLocation);
					int pointDeduction = (int)(score.GetComponent<Scores>().getScore() * goalPointPercentage);
					score.GetComponent<Scores>().AddScore(-pointDeduction);
				}
			//Side walls
			} else if (Collection.gameObject.name == "Big Wall") {
				if (Mathf.Abs(rigidbody.velocity.x) < minXSpeed) {
					if (rigidbody.velocity.x != 0) rigidbody.AddForce(new Vector3(rigidbody.velocity.x * (minXSpeed / Mathf.Abs(rigidbody.velocity.x) - 1), 0, 0), ForceMode.Impulse);
					else rigidbody.AddForce(new Vector3(minXSpeed, 0, 0), ForceMode.Impulse);
				}
			}
		} 
		//FIREBALLS
		else if (ball == eBall.F_Left){
			if (Collection.gameObject.name == "Brick" || Collection.gameObject.name == "Brick_G" || Collection.gameObject.name == "Brick_O") {
				audio.Play();
				Destroy(Collection.gameObject);
				rigidbody.AddForce(-rigidbody.velocity + rightImpulse_F, ForceMode.Impulse);
				score.GetComponent<Scores>().AddScore(normalBrickScore);
			} else if (Collection.gameObject.name != "Ground") {
				Destroy(gameObject);
			}
		}
		//FIREBALLS
		else if (ball == eBall.F_Right){
			if (Collection.gameObject.name == "Brick" || Collection.gameObject.name == "Brick_G" || Collection.gameObject.name == "Brick_O") {
				audio.Play();
				Destroy(Collection.gameObject);
				rigidbody.AddForce(-rigidbody.velocity + leftImpulse_F, ForceMode.Impulse);
				score.GetComponent<Scores>().AddScore(normalBrickScore);
			} else if (Collection.gameObject.name != "Ground") {
				Destroy(gameObject);
			}
		}

		currentSpeedBuffer = speedChangeBuffer; //Prevents the speed restrictions from causing wierd things when colliding
	}

	void dropBall(Vector3 location) {
		rigidbody.transform.position = location;
		rigidbody.constraints = RigidbodyConstraints.None;
		collider.isTrigger = true;
		rigidbody.AddForce(-rigidbody.velocity + new Vector3(0, dropSpeed, 0), ForceMode.Impulse);
	}

	void disableDrop(Vector3 location) {
		rigidbody.transform.position = location;
		rigidbody.AddForce(-rigidbody.velocity, ForceMode.Impulse);
		dontDrop = true;
	}

	void convertToNeutral(Vector3 location) {
		neutral = true;
		renderer.material.SetColor("_Color", Color.white);
		dropBall(location);
	}
}
