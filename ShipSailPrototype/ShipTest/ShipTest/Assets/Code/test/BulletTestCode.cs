using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTestCode : MonoBehaviour {

	//copy paste lazy from projectile code, as this is for testing purposes only

	public GameObject this_parent, splashFX, crashFX; //for the parent itself, and some 'prefabs' per say
	//Rigidbody rb;
	public float lifeSpan = 10f;
	public float bulletSpeed = 450f;
	private bool w_flag, h_hit; //a flag to see if the object has 'hit' the water, debug wise



	// Use this for initialization
	void Start () {
		this_parent = this.gameObject; //jnc, debug wise check
		w_flag = false;
		h_hit = false;
		//Destroy (this, lifeSpan);//destroy after ten seconds
		/* edited out, since it's no needed for testing purposes. Let gravity do it's job
		rb = GetComponent<Rigidbody>();
		/rb.AddForce(transform.forward * bulletSpeed);
		rb.AddForce(transform.up * (-bulletSpeed / 2)); */
	}
	
	// Update is called once per frame
	void Update () {

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
			Splash ();
		//end death cons.
	}

	void OnTriggerEnter (Collider col) //collider, no collision with trigger.
	{
		if (col.gameObject.tag == "Water") {
			w_flag = true;
			Debug.Log ("Hit the ocean");
		} else {
			Debug.Log ("it's a wipe!");
			h_hit = true;
		}
			
	}

	void Splash () {//replaces with a 'splash' effect. But for now, erase auto.
		Destroy (this_parent);
	}

}
