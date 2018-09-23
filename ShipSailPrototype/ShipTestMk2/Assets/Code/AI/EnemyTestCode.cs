using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTestCode : MonoBehaviour {

	//distance variables
	public float dist;
	private float min_dist = 45f;

	//gun load variables
	public float Mx_FireRate = 3f;
	private float FireRate = 0f;
	private bool Loaded = true;

	//enemy ship HP variables
	public float Mx_HP = 5f;
	private float HP;

	//enemy ship attack variables
	public GameObject[] AttackGuns;	//an array, for a list of objects. Front only bias
	public GameObject Projectile; //the enemy projectile


	Transform target; // Reference to the player's position
	public NavMeshAgent nav; // Reference to the nav mesh agent, requires UnityEngine.AI
	//ignore HP for now, this is a test hack ;P

	private Rigidbody rb;


	// Use this for initialization, a notch earlier than start
	void Awake () {
		target = GameObject.FindGameObjectWithTag ("Player").transform; //position wise
		nav = GetComponent <NavMeshAgent> ();
		rb = GetComponent<Rigidbody> ();

		//set up variables
		HP = Mx_HP;

		//don't set tags automatically, in case of 'technical issues'.
	}
	
	// Update is called once per frame
	void Update () {
		//insert tick here, to bar instant rapid fire
		Reload();

		MoveTowards (target);
		if (AtGunRange (target)) {//RotateTowards (target); //buggy, thus commented out. Sail straight instead
			FireGuns();	//insert gun script call
		}// else {MoveTowards (target)}
		//end if
	}

	private void MoveTowards (Transform target) {
		nav.SetDestination (target.position);
	}

	private void RotateTowards (Transform target) {//buggy/not ideal 'realist' wis code, REDACTED out of main loop for now
		//nav.updateRotation (); //something that should work, but documentation was shite at describing an example case, thus commented out
		transform.RotateAround (target.transform.position, target.transform.up, Time.deltaTime * nav.speed); //very not good/natural, but better than none
	}

	/*END MOVEMENT CODE*/

	private bool AtGunRange (Transform target) {
		dist = Vector3.Distance (target.position, transform.position);
		return dist < min_dist; // if dist is below min range
	}//end gunRange bool

	void Reload () {
		if (!Loaded) {
			if (FireRate <= 0) {
				Loaded = true;
				FireRate = 0f;
			} else {
				FireRate -= Time.deltaTime;
			}
		}//endif
	}//end AI reload code

	void FireGuns () {//firing code
		if (Loaded) {
			//reload AI gun
			Loaded = false;
			FireRate = Mx_FireRate;

			//fire AI gun, front gun bias as of this prototype vers;
			//Debug.Log ("BANG!");
			for (int i = 0; i < AttackGuns.Length; i++) {
				Instantiate (Projectile, AttackGuns[i].transform.position, AttackGuns[i].transform.rotation);
			} //endfor


		}
	}//end AI firing code

	void OnTriggerEnter (Collider col) {
		if(col.gameObject.tag == "Player_Shot") {
			ShipDMG ();
			//Debug.Log ("Mayday!");
		}
	}


	void ShipDMG () {
		HP -= 5F;

		if (HP <= 0) {
			Debug.Log ("BOOM!");
			Destroy(this.gameObject); //must destroy this with gameObject, otherwise you'd only destroy the script instead.
		}
	}


}
