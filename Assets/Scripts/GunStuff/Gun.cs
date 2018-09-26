using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun: MonoBehaviour {

	public GameObject magazine;

	public GameObject shell;

	public bool isMagInWeapon;
	public bool chamber;
	public float rotateGun;
	public GameObject muzzleFlash;

	public float xMin,xMax,yMin,yMax;
	public float gunDepth;
	public AudioClip fireSound;
	public AudioClip GunChamberSound;

	public GameObject lookAtObj;


	void Awake () {
		isMagInWeapon =true;
		int luck = Random.Range(0,2);
		if(luck==1){
			chamber=true;
		}
	}
	

	void Update () {
		GunMechanics();
		GunInputs();
	}

	void GunMechanics(){

		if(this.gameObject.transform.root.GetComponent<Player>().isGunDown==false){
			Vector3 mousePos = Input.mousePosition;
			Vector3 objPos =Input.mousePosition;

			mousePos.x = Mathf.Clamp(mousePos.x,xMin,Screen.width-xMax);
			mousePos.y = Mathf.Clamp(mousePos.y,yMin,Screen.height-yMax);
			objPos.x = Mathf.Clamp(mousePos.x,xMin,Screen.width-xMax);
			objPos.y = Mathf.Clamp(mousePos.y,yMin,Screen.height-yMax);
			mousePos.z =gunDepth;
			objPos.z=30f;
			if (mousePos.x == xMin || mousePos.x == Screen.width- xMax) {
				transform.root.gameObject.GetComponent<Player> ().horizontalMoveLimit = true;
			} else if(mousePos.x > xMin || mousePos.x < Screen.width-xMax ){
				transform.root.gameObject.GetComponent<Player> ().horizontalMoveLimit = false;
			}

			this.transform.rotation = Camera.main.transform.rotation;

			this.transform.position = Camera.main.ScreenToWorldPoint(mousePos)+new Vector3(0,rotateGun,0);
			lookAtObj.transform.position = Camera.main.ScreenToWorldPoint(objPos);
			this.transform.LookAt(lookAtObj.transform);
		}
	}

	void GunInputs(){
		if(this.gameObject.transform.root.GetComponent<Player>().isGunDown == false && chamber ==true && Input.GetMouseButtonDown(0)){
			Shoot();
			if(magazine.GetComponent<Magazine>().bullets.Contains(true)==true&& isMagInWeapon==true){
				magazine.GetComponent<Magazine>().bullets.RemoveAt(0);
				chamber=true;
			}else{
				//Bullets are over
				chamber=false;
			}
		}

		if(Input.GetKeyDown(KeyCode.R)){
			if(isMagInWeapon==true&&magazine.GetComponent<Magazine>().bullets.Contains(true)==true){
				AudioSource.PlayClipAtPoint (GunChamberSound, this.transform.position);
				magazine.GetComponent<Magazine>().bullets.RemoveAt(0);
				if (chamber == true) {
					GameObject shellBoi = Instantiate (shell, this.transform.GetChild (1).transform.position, Quaternion.identity);
					Vector3 ejectionVelocity= new Vector3 (10f, 3f, 0f);
					ejectionVelocity = this.transform.rotation * ejectionVelocity;
					shellBoi.GetComponent<Rigidbody> ().velocity = ejectionVelocity;
					shellBoi.GetComponent<Rigidbody> ().rotation = this.transform.rotation;
				}
				chamber=true;

			}else if(chamber==true&&magazine.GetComponent<Magazine>().bullets.Contains(true)==false){
				AudioSource.PlayClipAtPoint (GunChamberSound, this.transform.position);
				chamber=false;
				GameObject shellBoi = Instantiate (shell, this.transform.GetChild (1).transform.position, Quaternion.identity);
				Vector3 ejectionVelocity= new Vector3 (10f, 3f, 0f);
				ejectionVelocity = this.transform.rotation * ejectionVelocity;
				shellBoi.GetComponent<Rigidbody> ().velocity = ejectionVelocity;
				shellBoi.GetComponent<Rigidbody> ().rotation = this.transform.rotation;
			}
		}
	}
	void Shoot(){
		StartCoroutine("TurnOnMuzzle");
		AudioSource.PlayClipAtPoint (fireSound, this.transform.position);
		Ray ray = new Ray(this.transform.GetChild(0).transform.position,this.transform.GetChild(0).transform.forward);
		RaycastHit hit;
		Debug.DrawRay (this.transform.GetChild(0).transform.position, this.transform.GetChild(0).transform.forward*1000,Color.blue,3f);

		if(Physics.Raycast(ray,out hit,1000)){
			GameObject shellBoi = Instantiate (shell, this.transform.GetChild (1).transform.position, Quaternion.identity);
			Vector3 ejectionVelocity= new Vector3 (10f, 3f, 0f);
			ejectionVelocity = this.transform.rotation * ejectionVelocity;
			shellBoi.GetComponent<Rigidbody> ().velocity = ejectionVelocity;
			shellBoi.GetComponent<Rigidbody> ().rotation = this.transform.rotation;
			Vector3 hitPoint = hit.point;
			GameObject objectHit = hit.collider.gameObject;
			if (objectHit.transform.tag == "Enemy") {
				objectHit.transform.GetComponent<AiEnemy> ().health -= 45;
			} else if (objectHit.transform.tag == "EnemyTurret") {
				objectHit.transform.GetComponent<Turret> ().health -= 45;
			} else if (objectHit.transform.tag == "Bebis") {
				objectHit.transform.forward = this.transform.forward;
				objectHit.GetComponent<Rigidbody> ().AddForce (objectHit.transform.forward * 500);
			} else if (objectHit.transform.tag == "Barrel") {
				objectHit.transform.forward = this.transform.forward;
				objectHit.GetComponent<Rigidbody> ().AddForce (objectHit.transform.forward * 15,ForceMode.Impulse);
			}
		}
	}
	IEnumerator TurnOnMuzzle(){
		muzzleFlash.SetActive (true);
		yield return new WaitForSeconds (0.05f);
		muzzleFlash.SetActive (false);
	}
}
