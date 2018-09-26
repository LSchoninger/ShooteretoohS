using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour {

	public AudioClip beepSound;
	private float waitForPatrol;
	public float startWaitForPatrol;
	void Start () {
		waitForPatrol = startWaitForPatrol;
	}
	
	// Update is called once per frame
	void Update () {
		
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

	void OnTriggerStay(Collider other){
		if (other.gameObject.tag == "Player")  {
			Vector3 distToPlayer = other.transform.position - transform.position;
			RaycastHit hit;
			if (Physics.Raycast (transform.position, distToPlayer, out hit)) {
				Debug.DrawRay (transform.position, distToPlayer * 1000, Color.red, 1.5f);
				if (hit.collider.gameObject.tag == "Player") {
				this.transform.root.GetComponent<Turret> ().lookAt = true;
				}else if(waitForPatrol <= 0) {
					transform.root.GetComponent<Turret> ().lookAt = false;
					waitForPatrol = startWaitForPatrol;
					transform.root.GetComponent<Turret> ().waitForShoot = transform.root.GetComponent<Turret> ().startWaitForShoot;
				} else {
					waitForPatrol -= Time.deltaTime;
				}
			}
		}
	
	}
	
}
