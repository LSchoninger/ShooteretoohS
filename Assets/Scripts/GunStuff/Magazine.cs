using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine: MonoBehaviour {

	public List<bool> bullets;
	public int ammoCapacity;
	int count;

	void Awake () {
		int randomNumber = Random.Range(1,ammoCapacity);
		while(randomNumber>=count){
			bullets.Add(true);
			count++;
		} 
	}
	

	void Update () {
		if (bullets.Count  -1 > ammoCapacity) {
			bullets.RemoveAt (0);
		}
		
	}
}
