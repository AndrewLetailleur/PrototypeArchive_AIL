using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {
	public GameObject this_parent;//rest not needed for now, splashFX, crashFX; //for the parent itself, and some 'prefabs' per say
	private Rigidbody rb;
	public float lifeSpan = 10f;
	public float bulletSpeed = 450f;

//	public float velo_y; //not needed for now, debug wise
	private bool w_flag, h_hit; //a flag to see if the object has 'hit' the water, debug wise

	// Use this for initialization
	void Start () {
		this_parent = this.gameObject; //jnc, debug wise check
		w_flag = false;
		h_hit = false;
		//Destroy (this, lifeSpan);//destroy after ten seconds
		rb = GetComponent<Rigidbody>();
		rb.AddForce(transform.forward * bulletSpeed);
		rb.AddForce(transform.up * (bulletSpeed / 2));
	}



	// Update is called once per frame
	void Update () {
//		velo_y = rb.velocity.y;

		//begin death cons
		if (lifeSpan > 0)
			lifeSpan -= Time.deltaTime;
		else
			Destroy (this_parent);
		//end lifespan check

		//cue collosions
		if (w_flag)
			Splash ();
		else if (h_hit) //default to splash for now
			Smash ();
		//end death cons.
	}

	void OnTriggerEnter (Collider col) //collider, no collision with trigger.
	{//Debug.Log ("Col Entered");
		if(col.gameObject.tag == "Sea") {
			w_flag = true;
			//Debug.Log ("Hit the ocean");
		} else {
			//Debug.Log ("it's a wipe!");
			h_hit = true;
		}
	}

	void Splash () {//replaces with a 'splash' effect. But for now, erase auto.
//		Instantiate(splashFX, transform.position, transform.rotation);
		Destroy (this_parent);
	} void Smash () {//replaces with a 'splash' effect. But for now, erase auto.
//		Instantiate(crashFX, transform.position, transform.rotation);
		Destroy (this_parent);
	}

	/*debug code (NLN, but there for archive purposes)
		velo_y = rb.velocity.y;

		if (rb.velocity.y < -5) {
			float pz, px;
			px = rb.velocity.x;
			pz = rb.velocity.z;
			rb.velocity = new Vector3 (px, -5, pz);//soften blow test c. Force add is 'too weak'.
		}
	*/
}
