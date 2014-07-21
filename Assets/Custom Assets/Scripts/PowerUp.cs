using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	public int type;
	public Player target;
	public Transform fireballG;
	public Transform fireballO;
	private float minSpeed = 20.0f;
	private float accelSpeed = 20.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (rigidbody.velocity.magnitude < minSpeed && target != null) {
			Vector3 vel = target.transform.position - transform.position;
			vel.z += Random.Range(-2.0f, 2.0f);
			vel = vel / vel.magnitude * accelSpeed * Time.deltaTime;
			rigidbody.AddForce (vel, ForceMode.Impulse);
		} else if (/*rigidbody.velocity.magnitude > minSpeed * 1.5f && */target != null) {
			rigidbody.AddForce (-rigidbody.velocity * 0.5f * Time.deltaTime, ForceMode.Impulse);
		}
	}

	void OnCollisionEnter(Collision Collection) {
		if (Collection.gameObject.name == "Player Left") {
			switch (type) {
			case 0:
				Collection.gameObject.GetComponent<Player>().increaseSize(0.15f);
				break;
				
			case 1:
				Collection.gameObject.GetComponent<Player>().increaseSize(-0.10f);
				break;
				
			case 2:
				Ball newFireball = ((Transform)Instantiate(fireballG, transform.localPosition + new Vector3(4, 0, 0), Quaternion.identity)).gameObject.GetComponent<Ball>();
				newFireball.paddle = Collection.gameObject;
				newFireball.score = GameObject.Find("Score_G");
				break;
				
			case 3:
				Ball newFireball2 = ((Transform)Instantiate(fireballG, transform.localPosition + new Vector3(4, 0, 0), Quaternion.identity)).gameObject.GetComponent<Ball>();
				newFireball2.paddle = Collection.gameObject;
				newFireball2.score = GameObject.Find("Score_G");
				break;
			}
			
			Destroy(gameObject);
		} if (Collection.gameObject.name == "Player Right") {
			switch (type) {
			case 0:
				Collection.gameObject.GetComponent<Player>().increaseSize(0.25f);
				break;
				
			case 1:
				Collection.gameObject.GetComponent<Player>().increaseSize(-0.15f);
				break;
				
			case 2:
				Ball newFireball = ((Transform)Instantiate(fireballO, transform.localPosition + new Vector3(-4, 0, 0), Quaternion.identity)).gameObject.GetComponent<Ball>();
				newFireball.paddle = Collection.gameObject;
				newFireball.score = GameObject.Find("Score_O");
				break;
				
			case 3:
				Ball newFireball2 = ((Transform)Instantiate(fireballO, transform.localPosition + new Vector3(-4, 0, 0), Quaternion.identity)).gameObject.GetComponent<Ball>();
				newFireball2.paddle = Collection.gameObject;
				newFireball2.score = GameObject.Find("Score_O");
				break;
			}

			Destroy(gameObject);
		}  else if (Collection.gameObject.name == "Green_Goal" || Collection.gameObject.name == "Orange_Goal") {
			Destroy(gameObject);
		}
	}
}