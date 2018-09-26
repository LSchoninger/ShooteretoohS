using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {


	void Update(){
		if (SceneManager.GetActiveScene().name=="MenuScene") {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}else if(SceneManager.GetActiveScene().name=="Game"){
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("MenuScene");
		}
	}

	public void LoadLevel(int scene){
		SceneManager.LoadScene (scene);
	}

	public void Quit(){
		Debug.Log ("exit");
		Application.Quit ();
	}
}
