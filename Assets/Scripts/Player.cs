using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour {

	public float walkSpeed;
	public float runSpeed;
	public float jumpSpeed;
	public float verticalLimit =60f;
	public bool horizontalMoveLimit;
	private float forwardBackwardsMovement;
	private float sidewaysMovement;
	private float verticalMovement;
	public Text objectiveText;
	public Text helperText;
	private float horizontalRotation;
	private float verticalRotation;
	public Text CountText;
	public AudioClip unloadGunSound;
	public AudioClip reloadSound;
	public GameObject inventoryWeapon;
	public GameObject inventoryPos1;
	public GameObject inventoryPos2;
	public GameObject  inventoryPos3;
	public GameObject  handPosition;
	public int objectsColleted;
	public float speedLerp;
	private float startTime;
	public GameObject hand;
	public int bullets;
	public bool isGunDown;
	public List<GameObject> inventory;
	int count;
	public int health;
	
	public GameObject gun;

	//Character controller is a component [REQUIRED] used to control the character
	CharacterController charCtrl;

	void Start () {
		
		charCtrl = GetComponent<CharacterController> ();
		Cursor.visible = false;
		startTime = Time.time;
		bullets =Random.Range(1,15);
		int luck = Random.Range(0,2);
		while(luck>=count){
			GameObject magazinesInstantiated = Instantiate(this.GetComponentInChildren<Gun>().magazine);
			magazinesInstantiated.transform.parent = Camera.main.transform;
			inventory.Insert(count,magazinesInstantiated);
			count++;
		}
		inventory.Add(null);
		inventory.Add(null);
		
	}
	
	
	void Update () {
		
		Movement ();
		PlayerInputs();
		endGame ();
		objectiveText.text = "Colectables: " + objectsColleted;
	}

	void lerp(Transform startMarker,Transform endMarker,GameObject lerpingObject){
		float journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
		float distCovered = (Time.time - startTime) * speedLerp;
		float fracJourney = distCovered / journeyLength;
		lerpingObject.transform.position	 = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
	
	
	}

	void endGame(){
		if (this.transform.position.y <= -30) {
			SceneManager.LoadScene("Game");
		}
		if(objectsColleted>=3){
			SceneManager.LoadScene("EndGame");
		}
		if (health <= 0) {
			SceneManager.LoadScene("Game");
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.transform.tag == "Colectable") {
			objectsColleted += 1;
			Destroy (other.gameObject);
		}
		if (other.transform.tag == "Munnition") {
			int luck = Random.Range (1, 7);
			bullets += luck;
			Destroy (other.gameObject);
		}
	}

		void Movement(){

		//ROTATION//
		//horizontal rotation= The whole character rotates horizontaly through mouse input X
		horizontalRotation = Input.GetAxis ("Mouse X");
		if (horizontalMoveLimit == true) {
			transform.Rotate (0f, horizontalRotation, 0f);
		}
		//Vertical rotation = We move just the camera, else the whole character would pivot up and down.
		//Using verticalLimmit we clamp how much the player can look vertically with mouse input Y
		verticalRotation -= Input.GetAxis ("Mouse Y");
		verticalRotation = Mathf.Clamp (verticalRotation, -verticalLimit, verticalLimit);
		Camera.main.transform.localRotation = Quaternion.Euler (verticalRotation, 0f, 0f);

		//MOVEMENT//
		//Movement is controlled through vertical and horizontal axis using the movement floats we created
		forwardBackwardsMovement = Input.GetAxis ("Vertical") * walkSpeed;
		sidewaysMovement = Input.GetAxis ("Horizontal") * walkSpeed;

		//Char will only run when going forward
		if (Input.GetKey (KeyCode.LeftShift) && forwardBackwardsMovement > 0) {
			forwardBackwardsMovement = Input.GetAxis ("Vertical") * runSpeed;
		}

		//isGrounded automatically checks if its grounded, else everytime you press jump it would jump to oblivion and beyond
		if (charCtrl.isGrounded) {
			if (Input.GetButtonDown ("Jump")) {
				verticalMovement = jumpSpeed;
			} 
		}else{
			//CharacterController is NOT affect by gravity, so we must manually add vertical(y) gravity times time.deltaTime
			verticalMovement += Physics.gravity.y * Time.deltaTime;
		}
		//We pass the movement floats to a V3, multiply it with the rotation so the character follow the direction he's 
		//looking at, and use the .Move function from the CharacterController to move it
		Vector3 playerMovement = new Vector3(sidewaysMovement,verticalMovement,forwardBackwardsMovement);
		playerMovement = transform.rotation * playerMovement;
		charCtrl.Move (playerMovement * Time.deltaTime);
	}
	void PlayerInputs(){
		if(Input.GetKey(KeyCode.H)){
			helperText.text = "'E' Retira o pente da arma, quando a arma está levantada" + "\n" + "'Q' abaixa arma e levanta a arma" + "\n"
			+ "'R' puxa a culatra da arma" + "\n" + "'T' mostra se a culatra está puxada" + "\n" + "'Z'Coloca o carregador(Arma Levantada) / Carrega o pente"
			+ "\n" + "'1,2,3' posições no inventário" + "\n" + "'Botão do mouse' Atira" + "\n" + "COLETE 3 OBJETOS VERMELHOS NO MAPA";
				
		}else{
			helperText.text="Press 'h' for help";

		}

		if (hand != null && isGunDown == true) {
			CountText.text = "Bullets: " + bullets + "\n" + "Bullets in mag: " + hand.GetComponent<Magazine>().bullets.Count;
		} else {
			CountText.text = "";
		}
		if (isGunDown==false&&Input.GetKey (KeyCode.T)) {
			CountText.text="Chamber = " + gun.GetComponent<Gun> ().chamber;
		}

		if(inventory[0]!=null){
			inventory[0].transform.position = inventoryPos1.transform.position;
		}
		if(inventory[1]!=null){
			inventory[1].transform.position = inventoryPos2.transform.position;
		}
		if(inventory[2]!=null){
			inventory[2].transform.position = inventoryPos3.transform.position;
		}
		if(hand!=null){
			hand.transform.position = handPosition.transform.position;
		}

		if(Input.GetKeyDown(KeyCode.Alpha1)){
			if(inventory[0]!=null&&inventory[0].GetComponent<Magazine>()==true){
				if(hand==null){
				//	inventory[0].transform.position = Vector3.Lerp (inventoryPos1.transform.position, handPosition.transform.position, Time.deltaTime);
					hand=inventory[0];
					inventory[0]=null;
				}else if(hand!=null){
					GameObject auxiliary = hand;
					hand= inventory[0];
					inventory[0]=auxiliary;
				}
			}else if(inventory[0]==null&&hand!=null){
				inventory[0]=hand;
				hand=null;
			}
		}
		if(Input.GetKeyDown(KeyCode.Alpha2)){
			if(inventory[1]!=null&&inventory[1].GetComponent<Magazine>()==true){
				if(hand==null){
					hand=inventory[1];
					inventory[1]=null;
				}else if(hand!=null){
					GameObject auxiliary = hand;
					hand= inventory[1];
					inventory[1]=auxiliary;
				}
			}else if(inventory[1]==null&&hand!=null){
				inventory[1]=hand;
				hand=null;
			}
		}
		if(Input.GetKeyDown(KeyCode.Alpha3)){
			if(inventory[2]!=null&&inventory[2].GetComponent<Magazine>()==true){
				if(hand==null){
					hand=inventory[2];
					inventory[2]=null;
				}else if(hand!=null){
					GameObject auxiliary = hand;
					hand= inventory[2];
					inventory[2]=auxiliary;
				}
			}else if(inventory[2]==null&&hand!=null){
				inventory[2]=hand;
				hand=null;
			}
		}

		if(Input.GetKeyDown(KeyCode.E)){
			if(isGunDown==false&&hand==null){
				AudioSource.PlayClipAtPoint (unloadGunSound, this.transform.position);
				gun.GetComponent<Gun>().isMagInWeapon=false;
				hand = 	gun.GetComponent<Gun>().magazine;
				gun.GetComponent<Gun> ().magazine = null;
			}
		}
		if(Input.GetKeyDown(KeyCode.Q)){
			if(isGunDown==false){
				inventory.Insert(3,gun);
				gun.gameObject.transform.position= inventoryWeapon.gameObject.transform.position;
				gun.gameObject.transform.rotation= inventoryWeapon.gameObject.transform.rotation;
				isGunDown=true;
			}else{
				inventory.Remove(gun);
				isGunDown=false;
			}
		}
		if(Input.GetKeyDown(KeyCode.Z)){
			if(isGunDown==true&&hand!=null){
				if(bullets>=1 &&hand.GetComponent<Magazine>().bullets.Count<=hand.GetComponent<Magazine>().ammoCapacity){
					AudioSource.PlayClipAtPoint (reloadSound, this.transform.position);
					hand.GetComponent<Magazine>().bullets.Insert(0,true);
					bullets--;
				}
			}
			if(isGunDown==false&&gun.GetComponent<Gun>().isMagInWeapon==false){
					if(hand!=null){
					AudioSource.PlayClipAtPoint (unloadGunSound, this.transform.position);
					gun.GetComponent<Gun>().isMagInWeapon=true;
					gun.GetComponent<Gun>().magazine=hand;
					hand=null;
					gun.GetComponent<Gun> ().magazine.transform.position = new Vector3 (-1, -1, -1);
					}
				}
		}

	}
}
