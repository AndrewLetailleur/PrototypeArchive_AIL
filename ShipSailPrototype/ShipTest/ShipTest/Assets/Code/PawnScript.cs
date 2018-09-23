using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnScript : MonoBehaviour {

	public GameObject[] ShipPositions;
	// Use this for initialization
	void Start () {
		ShipPositions = GameObject.FindGameObjectsWithTag ("Pawn_Position");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
			transform.position = new Vector3(ShipPositions[0].transform.position.x, ShipPositions[0].transform.position.y, ShipPositions[0].transform.position.z);
		//endif
		if (Input.GetKeyDown(KeyCode.Alpha2))
			transform.position = new Vector3(ShipPositions[1].transform.position.x, ShipPositions[1].transform.position.y, ShipPositions[1].transform.position.z);
		//endif
		if (Input.GetKeyDown(KeyCode.Alpha3))
			transform.position = new Vector3(ShipPositions[2].transform.position.x, ShipPositions[2].transform.position.y, ShipPositions[2].transform.position.z);
		//endif
	}
}
