using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

	public bool lookAt;
	public GameObject player;
	private float timeBetweenShots;
	public float startTimeBetweenShots=1f;
	public AudioClip gunShotSound;

	//[HideInInspector]
	public float waitForShoot;
	public float startWaitForShoot =4f;

	public int health;

	public Animator anim;
	void Start () {
		anim = GetComponentInChildren<Animator> ();
		anim.SetBool ("Playanim", true);
		timeBetweenShots = startTimeBetweenShots;
		waitForShoot = startWaitForShoot;

	}
	
	// Update is called once per frame
	void Update () {
		HandleLook ();
		Shoot ();
		if (health <= 0) {
			Destroy (this.gameObject);
		}
	}

	void HandleLook(){
		if (lookAt) {
			anim.SetBool ("Playanim", false);
			GetComponentInChildren<Animator>().enabled =false;
			transform.GetChild (0).transform.LookAt (player.transform);
		} else {
			GetComponentInChildren<Animator>().enabled =true;
			anim.SetBool("Playanim", true);
		}
	}

	void Shoot(){

		if (lookAt) {
			
			if (waitForShoot <= 0) {
				
				if (timeBetweenShots <= 0) {
					AudioSource.PlayClipAtPoint (gunShotSound, transform.position);
					timeBetweenShots = startTimeBetweenShots;
					RaycastHit hit;
					Vector3 distanceToPlayer = player.transform.position - transform.GetChild (0).position;
					if (Physics.Raycast (transform.GetChild (0).position, distanceToPlayer, out hit, 1000)) {
						Debug.DrawRay (transform.GetChild (0).position, distanceToPlayer * 1000, Color.blue);
						if (hit.transform.gameObject.tag == "Player") {
							lookAt= true;
							hit.transform.gameObject.GetComponent<Player> ().health -= 50;
						}
					}
				} else {
					timeBetweenShots -= Time.deltaTime;
				}
			} else {
				waitForShoot -=Time.deltaTime;
			}
		}
	}
}
