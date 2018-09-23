using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//for GUI components et all

public class ShipScript : MonoBehaviour {

	//misc variables here
	Rigidbody rb;
	//public float Y_Gravity = 2f; //sets the default gravity value, DO NOT TOUCH!


	//movement values
	private float frontal_Velo, rotation_Velo;
		//turning values
	public float turnSpeed = 0.1F;//turn speed, ratio wise
	private float max_Rot = 2F; //max rotation speed
	private float L_Rot, R_Rot, Rotar;
		//velocity values
	public float moveSpeed = 0.1f;//velo speed, ratio wise
	private float max_Velo = 2F; //max speed, velocity wise
	private float min_Velo, Knots;

		//projectile/attack variables, object reference wise
	public GameObject[] FrontGuns, LeftGuns, RightGuns;	//an array, for a list of objects
	public GameObject Projectile;

	//public variables
		//public float p_Rotar, p_Knots, p_Load, p_POV_R;//for visual editing ref

	public GameObject POV;
	public float max_POV_R = 3F;
	private float POV_R;

	//fire code here
	public float loadRate = 1F;//max load speed
		//gui elements //private bool canFire; //made private, replaced by three firing triggers for each side
	public bool centFire, leftFire, rightFire, specFire = true; //more detailed reload bools, for 'slider' reference. :P

	/* TO BREAK UP LATER */
		// major GUI element, HP wise? See on breaking up later
	//AMMO gui elements
	public Slider leftBar, centBar, rightBar; // GUI aspect
	private float lb_v, cb_v, rb_v; //values and maxes
	//HEALTH gui elements
	public Image ShipCON, DamageFX; //for Ship Icon color, and 'flashing' damage indicator.
	public float mHP = 100f; //set up HP lazily
	private float HP;
	public Text GUI_HP; //GUI aspect, should ideally be updated if called upon
	/* TO BREAK UP LATER */

	// Use this for initialization
	void Start () {
		HP = mHP;
		HP_IconCheck();//gui LOAD

		//set up loading values for all three/four firing triggers
		lb_v = loadRate;
		leftBar.maxValue = loadRate;
		leftBar.value = calcLeftLoad ();

		rb_v = loadRate;
		rightBar.maxValue = loadRate;
		rightBar.value = calcRightLoad ();

		cb_v = loadRate;
		centBar.maxValue = loadRate;
		centBar.value = calcCentLoad ();

		//tbd later. "specBar" = loadRate * X value, special delay wise?

		rb = GetComponent<Rigidbody> ();//there, jnc rigidbody is needed

		//set min/max rot values
		L_Rot = max_Rot;
		R_Rot = -max_Rot;
		Rotar = 0F; //start at no rotation here

		//set min/max velocity values
		min_Velo = 0F;
		Knots = 0F; //start at no velocity here

		//set pov rotation default values
		POV = GameObject.FindGameObjectWithTag ("Head");
		POV_R = 0F;


		//set load value
		//Load = 0F;//start all primed, and ready to fire
		FrontGuns = GameObject.FindGameObjectsWithTag ("Front_Attack");
		LeftGuns = GameObject.FindGameObjectsWithTag ("Left_Attack");
		RightGuns = GameObject.FindGameObjectsWithTag ("Right_Attack");
	}

	//checks for current health, and sets/edits gui and later, to trigger a 'death con'.
	void HP_IconCheck () {
		//gets the HP color, for later transition effect. Not needed for now due to technical issues in getting gradiants to work.
//		Color HP_Col = ShipCON.color;

		/* shoddy test code example, that doesn't work
		if (HP > (mHP / 2)) {
				//shoddy code example, should be fixed with a smoother transition in future
			ShipCON.color = Color.Lerp (HP_Col, Color.black, Mathf.Clamp((HP - mHP) ) );
			ShipCON.color = Color.Lerp (HP_Col, Color.yellow, Mathf.PingPong(Time.time, 1));
		}
		*/

		//better color code, that is verbose/rough, but works to a shoddy degree.

		//HP Test condition, from bottom to high end.
		if (HP <= 0) {//DEAD Black Icon test
			ShipCON.color = Color.black;
		} else if (HP <= (mHP / 4) ) {//RED icon test
			ShipCON.color = Color.red;
		} else if (HP <= (mHP / 3) ) {//ORANGE icon test
			ShipCON.color = new Color(1f, 0.5f, 0f, 1f);
		} else if (HP <= (mHP / 2)) {//YELLOW icon test
			ShipCON.color = Color.yellow;
		} else if (HP <= ( (mHP / 4) * 3) ) {//LIME icon test
			ShipCON.color = new Color(0.5f, 1f, 0f, 1f);
		} else {//GREEN icon test
			ShipCON.color = Color.green;
		} //end color test/checker

		//update HP val, GUI wise
		GUI_HP.text = "%"+ HP.ToString ();
	}

	// Update is called once per frame
	void Update () {
		
		//call upon velocity and rotation values first
		VelocityCode ();
		//p_Knots = Knots; //to get a read, test speed
		RotationCode ();
		//p_Rotar = Rotar; //to get a read, test rotation
		HeadRotate (); //rotate the head, test wise

		ShipAttack ();//check for firing trig code

		UICode (); //updates the UI, loading wise

		HP_IconCheck (); //just to make sure the health updates after damage per frame. For a future gradiant fade in. Might be more efficient to fold in HP Damage code.

		/*lazy HP damage checker
		if (Input.GetKeyDown (KeyCode.J) )//front
		{
			ShipDMG ();
		}//end hacky lazy code
		*/
	}

	//begin movement code
	void VelocityCode () {//controls velocity/knots of the ship
		//begin momentum code
		frontal_Velo = Input.GetAxis ("Vertical") * moveSpeed;

		if (frontal_Velo < 0) {
			if ((Knots -= Time.deltaTime * moveSpeed) < min_Velo)
				Knots = min_Velo;//JNC barring backtracking done, unless intentional
			else
				Knots -= Time.deltaTime * moveSpeed;
			//end if
		}
		if (frontal_Velo > 0) {
			if ((Knots += Time.deltaTime * moveSpeed) > max_Velo)
				Knots = max_Velo;
			else
				Knots += Time.deltaTime * moveSpeed;
			//end if
		}

		transform.Translate(Knots, 0, 0);//if going frontal say?

		/*END VELOCITY CODE*/
	}

	void RotationCode () {//controls turning speed of the ship
		//begin rotation code
		rotation_Velo = Input.GetAxis ("Horizontal") * turnSpeed;//could alt be rotSpeed

		if (rotation_Velo > 0) {
			if ((Rotar += Time.deltaTime * turnSpeed) > L_Rot)
				Rotar = L_Rot;
			else
				Rotar += Time.deltaTime * turnSpeed;
			//end if
		}
		if (rotation_Velo < 0) {
			if ((Rotar -= Time.deltaTime * turnSpeed) < R_Rot)
				Rotar = R_Rot;
			else
				Rotar -= Time.deltaTime * turnSpeed;
			//end if
		}
		if (rotation_Velo == 0) {
			if (Rotar > 0.5F)
				Rotar -= Time.deltaTime * turnSpeed * 2;
			else if (Rotar < -0.5F)
				Rotar += Time.deltaTime * turnSpeed * 2;
			else
				Rotar = 0F;//lazy, short term
			//endif
		}
		//endif
		transform.Rotate (0, Rotar, 0);

		/*END ROTATION CODE*/
	}
		
	void HeadRotate() {//controls character POV viewpoint.
		if (Input.GetKey (KeyCode.Q)) {
			if ((POV_R -= Time.deltaTime * 3) < (max_POV_R * -1))
				POV_R = -max_POV_R;
			else
				POV_R -= Time.deltaTime * 3;
			//end if
		}
		else if (Input.GetKey (KeyCode.E)) {
			if ((POV_R += Time.deltaTime * 3) > max_POV_R)
				POV_R = max_POV_R;
			else
				POV_R += Time.deltaTime * 3;
			//end if
		} else {
			if (POV_R > 0.5F)
				POV_R -= Time.deltaTime * 3;
			else if (POV_R < -0.5F)
				POV_R += Time.deltaTime * 3;
			else
				POV_R = 0F;//lazy, short term
			//endif
		}
		//endif

		POV.transform.Rotate (0, POV_R, 0); //ltest
		/*END CAMERA CODE*/
	}
	//end movement scripts



	void ShipAttack () {
		//begin attack code
		if (Input.GetKeyDown (KeyCode.X) && FrontGuns.Length > 0 && centFire)//front
		{
			for (int i = 0; i < FrontGuns.Length; i++) {
				Instantiate (Projectile, FrontGuns[i].transform.position, FrontGuns[i].transform.rotation);
			} //endfor
			//Reload ();
			C_Load();
		}
		if (Input.GetKeyDown (KeyCode.Z) && LeftGuns.Length > 0 && leftFire)//left
		{
			for (int i = 0; i < LeftGuns.Length; i++) {
				Instantiate (Projectile, LeftGuns[i].transform.position, LeftGuns[i].transform.rotation);
			} //endfor
			//Reload ();
			L_Load();
		}
		if (Input.GetKeyDown (KeyCode.C) && RightGuns.Length > 0 && rightFire)//right
		{
			for (int i = 0; i < RightGuns.Length; i++) {
				Instantiate (Projectile, RightGuns[i].transform.position, RightGuns[i].transform.rotation);
			} //endfor
			//Reload ();
			R_Load();
		}//reload firing code
		//endif
	}
		
	//begin DMG variables
	void ShotDMG () { HP -= 10;	} //tbi, "RamDMG", "ReefDMG?", est.

	//test code only, for future ramming
	void ShipDMG () { HP -= 15; }

	//when object enters a collision field
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "Projectile") {
			ShotDMG ();
			Debug.Log ("We're hit!");
		}
	}

	//sets up UI, to increment the slider while empty, and reset firing triggers when filled
	void UICode () {
		if (!leftFire) {
			lb_v = lb_v + Time.deltaTime;
			if (lb_v >= loadRate)
				leftFire = true;
			leftBar.value = calcLeftLoad ();
		}

		if (!rightFire) {
			rb_v = rb_v + Time.deltaTime;
			if (rb_v >= loadRate)
				rightFire = true;
			rightBar.value = calcRightLoad ();
		}

		if (!centFire) {
			cb_v = cb_v + Time.deltaTime;
			if (cb_v >= loadRate)
				centFire = true;
			centBar.value = calcCentLoad ();
		}
	}//end gui code

	//sets firing conditions to false, and empties gui sliders of each bar.
	void L_Load () {	
		leftFire = false;
		lb_v = 0;
		leftBar.value = calcLeftLoad ();
	}
	void R_Load () {	
		rightFire = false;
		rb_v = 0;
		rightBar.value = calcRightLoad ();
	}
	void C_Load () {
		centFire = false;
		cb_v = 0;
		centBar.value = calcCentLoad ();
	}

	//begin calc return valves for loading variables
	float calcLeftLoad () {
		return lb_v;
	} float calcRightLoad () {
		return rb_v;
	} float calcCentLoad () {
		return cb_v;
	}
	//end firing trig code
}