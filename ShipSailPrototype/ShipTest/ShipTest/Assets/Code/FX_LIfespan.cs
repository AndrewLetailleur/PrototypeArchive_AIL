using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_LIfespan : MonoBehaviour {

	public float lifespan = 2f;
	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, lifespan);
	}
}
