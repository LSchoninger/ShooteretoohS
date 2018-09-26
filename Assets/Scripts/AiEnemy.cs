using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiEnemy : MonoBehaviour {

	private GameObject player;
	private NavMeshAgent _navAgent;

	//How long should the enemy wait in a movespot
	public float waitTime = 3f;
	private float stayInPlaceTime;
	public float startWaitForPatrol;
	public float startWaitForShoot;
	private float waitForPatrol;
	private float waitForShoot;
	public float startTimeBetweenShots;
	private float timeBetweenShots;

	public AudioClip gunShotSound;
	public AudioClip beepSound;

	//Where the enemy should go next
	private int nextMoveSpot;
	public Transform[] moveSpots;
	public int health;
	public bool spotted;
	void Start () {
		player = GameObject.Find ("Player");
		_navAgent = GetComponent<NavMeshAgent> ();
		nextMoveSpot = Random.Range (0, moveSpots.Length);
		waitForPatrol = startWaitForPatrol;
		waitForShoot = startWaitForShoot;
		timeBetweenShots = startTimeBetweenShots;
	}

	void Update () {
		Patrol ();
		Shoot ();
		if (health <= 0) {
			Destroy (this.gameObject);
		}
	}

	void Patrol () {
		if (!spotted) {
			_navAgent.SetDestination (moveSpots[nextMoveSpot].position);

			//If the enemy is nearby the movespot
			if (Vector3.Distance (transform.position, moveSpots[nextMoveSpot].position) < 3f) {
				//If it alreadey waited enough, then select another spot
				if (stayInPlaceTime <= 0) {
					nextMoveSpot = Random.Range (0, moveSpots.Length);
					stayInPlaceTime = waitTime;

					//else wait for the time
				} else {
					stayInPlaceTime -= Time.deltaTime;
				}

			}

		} else {
			_navAgent.isStopped = true;
		}
	}

	void OnTriggerStay (Collider other) {

		if (other.gameObject.tag == "Player") {
			RaycastHit hit;
			Vector3 distanceToPlayer = player.transform.position - transform.GetChild (0).position;

			if (Physics.Raycast (transform.GetChild (0).position, distanceToPlayer, out hit, 1000)) {
				Debug.DrawRay (transform.GetChild (0).position, distanceToPlayer * 1000, Color.red);
				if (hit.transform.gameObject.tag == ("Player")) {
					spotted = true;
				} else if (waitForPatrol <= 0) {
					spotted = false;
					_navAgent.isStopped = false;
					waitForPatrol = startWaitForPatrol;
					waitForShoot = startWaitForShoot;
				} else {
					waitForPatrol -= Time.deltaTime;
					spotted=false;
				}
			}
		}
	}
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			Vector3 distToPlayer = other.transform.position - transform.position;
			RaycastHit hit;

			if (Physics.Raycast (transform.position, distToPlayer, out hit)) {
				if (hit.collider.gameObject.tag == "Player") {
					AudioSource.PlayClipAtPoint (beepSound, transform.position);
				}
			}
		}

	}

	void Shoot () {
		if (spotted) {
			this.transform.LookAt (player.transform);
			if (waitForShoot <= 0) {

				if (timeBetweenShots <= 0) {
					AudioSource.PlayClipAtPoint (gunShotSound, transform.position);
					timeBetweenShots = startTimeBetweenShots;
					RaycastHit hit;
					Vector3 distanceToPlayer = player.transform.position - transform.GetChild (0).position;
					if (Physics.Raycast (transform.GetChild (0).position, distanceToPlayer, out hit, 1000)) {
						Debug.DrawRay (transform.GetChild (0).position, distanceToPlayer * 1000, Color.blue);
						if (hit.transform.gameObject.tag == "Player") {
							spotted = true;
							hit.transform.gameObject.GetComponent<Player> ().health -= 50;
						}
					}
				} else {
					timeBetweenShots -= Time.deltaTime;
				}

			} else {
				waitForShoot -= Time.deltaTime;
			}
		}
	}
}