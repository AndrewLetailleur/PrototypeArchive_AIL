using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for GUI components et all

public class BaseShipScript : MonoBehaviour {

	//physics variables here
	Rigidbody rb;
	//public float Y_Gravity; //DO NOT MESS WITH DEFAULT GRAVITY VALUES!!!

		//move values
	private float front_velo, rotate_velo;
		//turn values
	public float turnSpeed = 0.1f; //turn speed, ratio wise
	private float maxRotate = 1F; //max rotation speed
		//camera turn values
	private float MaxRotar = 3F;
	private float Rotar;
		//velo values
	public float moveSpeed = 0.1f;//velo speed, ratio wise
	private float maxVelo = 1F;//max speed, velocity wise
	private float minVelo = 0F;
	private float Knots = 0F;

	//projectile attack
	public GameObject[] AttackGuns; // lazy hack test version, of the real deal
	//public GameObject[] FrontGuns, LeftGuns, RightGuns; //an array, for a list of objects
	public GameObject Projectile;// for the cannon attack value
		//rate of fire
	public float loadRate = 1F;

	//private variables, slider gui wise
	public bool canFire, centFire, leftFire, rightFire, specFire = true; //more detailed slider bools;

	/*Major GUI Elements*/

		//slider bar variables, when implemented
	public Slider centBar, leftBar, rightBar; //specBar // GUI aspect
	private float lb_v, cb_v, rb_v, sb_v; //values and maxes by bar

		//health variables
	public Image ShipCON; //DamageFX;
	public float mxHP;
	private float HP;
	public Text GUI_HP; //GUI Aspect, to call upon later, HP val

		//camera variables, when properly implemented
	public GameObject POV; //sets POV of camera
	public float max_POV_R = 2F;
	private float POV_R;

	// Use this for initialization
	void Start () {
		HP = mxHP;
		HP_IconCheck (); //gui LOAD

		//set up loading values for all three/four firing triggers
			//just stick to one for now, though

		cb_v = loadRate;
		centBar.maxValue = loadRate;
		centBar.value = calcCentLoad ();

		lb_v = loadRate;
		leftBar.maxValue = loadRate;
		leftBar.value = calcLeftLoad ();

		rb_v = loadRate;
		rightBar.maxValue = loadRate;
		rightBar.value = calcRightLoad ();



	}
	
	// Update is called once per frame
	void Update () {

		VelocityCode ();//call upon front movement firest
		RotationCode();//then check on rotation
		CameraRotate();//then camera rotation
		ShipAttack();//then firing triggers
		UI_Code();//then gui
		//HP_IconCheck();//then HP icon, IF it needs constant update

		/*Lazy HP damage checker*/
		if (Input.GetKeyDown (KeyCode.J)) {
			ShipDMG ();
		}//end hacky lazy code
		/**/
	}

	void VelocityCode() {
		//begin momentum code
		front_velo = Input.GetAxis ("Vertical");//* moveSpeed

		if (front_velo < 0) {
			if ((Knots -= Time.deltaTime) < minVelo) {
				Knots = minVelo;//JNC
			} else
				Knots -= Time.deltaTime * moveSpeed;
		} else if (front_velo > 0) {
			if ((Knots += Time.deltaTime * moveSpeed) > maxVelo)
				Knots = maxVelo;
			else
				Knots += Time.deltaTime * moveSpeed;
		}//end if

			//as it is, to ensue forward moving of the current ship
		transform.Translate(0, 0, Knots);//could be better worded, on add force say?

		/*END VELOCITY CODE*/
	}//doesn't take into account navmesh

	void RotationCode() {
		//begin rotation code
		rotate_velo = Input.GetAxis("Horizontal");// * turnSpeed;//can be used to simplify turn conditions

			//simplify rotary code logic
		if (Rotar > 360)
			Rotar -= 360;
		else if (Rotar < -360)
			Rotar += 360;
		//end Rotar correction tweak

		//if (Rotar < MaxRotar || Rotar > -MaxRotar) //utter nonsense


//			Rotar += Time.deltaTime * rotate_velo; //beats the "up/down" rotate code
		//end if

		//Rotar = 0F; //default setting say?
		if (rotate_velo > 0) {//as left right is possitive/negative addition, math wise
			if ((Rotar += (Time.deltaTime * turnSpeed)) > MaxRotar)
				Rotar = MaxRotar;
			else
				Rotar += Time.deltaTime * turnSpeed;
			//end if
		}
		if (rotate_velo < 0) {
			if ((Rotar -= (Time.deltaTime * turnSpeed)) < -MaxRotar)
				Rotar = -MaxRotar;
			else
				Rotar -= (Time.deltaTime * turnSpeed / 2);
			//end if
		}
		if (rotate_velo == 0) {
			if (Rotar > 0.5F)
				Rotar -= (Time.deltaTime * turnSpeed / 2);
			else if (Rotar < -0.5F)
				Rotar += (Time.deltaTime * turnSpeed / 2);
			else
				Rotar = 0F; //no more rotate
			//end if
		} //end if


		transform.Rotate (0, Rotar, 0); //should do add force or nav instead
		/*END ROTATION CODE*/
		
	}//doesn't take into account navmesh
	void CameraRotate() {
		if (Input.GetKey(KeyCode.Q)) {
			if ((POV_R -= Time.deltaTime * 3) < (-max_POV_R))
				POV_R = -max_POV_R;
			else
				POV_R -= Time.deltaTime * 3;
			//end if
		} else if (Input.GetKey(KeyCode.E)) {
			if ((POV_R += Time.deltaTime * 3) > (max_POV_R))
				POV_R = max_POV_R;
			else
				POV_R += Time.deltaTime * 3;
			//end if
		} else //{
			POV_R = 0F; //safety value
		/*	if (POV_R > .5F)
				POV_R -= Time.deltaTime * 3;
			else if (POV_R > -.5F)
				POV_R += Time.deltaTime * 3;
			else //safety valve
				POV_R = 0F;//lazy short term form
			//end if
		} //end if
		*/
		POV.transform.Rotate (0, POV_R, 0); //cam test
		/*END CAMERA CODE*/
	}//end POV view

	//end movement scripts

	void ShipAttack() {//begin attack code
		//X cent (Test vers)
		if (Input.GetKeyDown (KeyCode.X) && AttackGuns.Length > 0 && canFire) //front
		{
			for (int i = 0; i < AttackGuns.Length; i++) {
				//spawn an internal GameObject, to further manipulate with force addition, depending on current knot speed.
					//this can also be additionally useful, if/when rotation becomes an issue, later on.
				GameObject Bullet = Instantiate (Projectile, AttackGuns[i].transform.position, AttackGuns[i].transform.rotation) as GameObject;
				Rigidbody BulletRB = Bullet.GetComponent<Rigidbody> ();
				BulletRB.AddForce(transform.forward * (Knots * 2500));
				BulletRB.AddForce(transform.up * (Knots / 3));
			 //	Debug.Log ("Open Fire!");
			}//end for    //C_Load();//reload, disabled for test code
		}//end if

		//X cent
			//if (Input.GetKeyDown (KeyCode.X) && FrontGuns.Length > 0 && centFire) //front
		//Z left
			//if (Input.GetKeyDown (KeyCode.Z) && LeftGuns.Length > 0 && leftFire) //front
		//C right
			//if (Input.GetKeyDown (KeyCode.C) && RightGuns.Length > 0 && rightFire) //front
			
		//? Special
			//if (Input.GetKeyDown (KeyCode.Space) && SpecialGuns.Length > 0 && specFire)

		//end reload if
	}//end of ship attack



	void HP_IconCheck() {
			//not needed/used well for now, due to technical issues with gradient effects
		Color HP_Col = ShipCON.color;//gets the HP color, for later transition effect.

		//basic HP test condition, lazy wise

		if (HP < 0)
			ShipCON.color = Color.black;
		else if (HP <= (mxHP / 4) )
			ShipCON.color = Color.red;
		else if (HP <= (mxHP / 3) )
			ShipCON.color = new Color (1f, 0.5f, 0f, 1f);//orange
		else if (HP <= (mxHP / 2) )
			ShipCON.color = Color.yellow;
		else if (HP <= ( (mxHP / 4) * 3) )
			ShipCON.color = new Color (0.5f, 1f, 0f, 1f);//lime
		else
			ShipCON.color = Color.green;
		//end if

		//update HP val, GUI wise

		if (HP < 0)
			GUI_HP.text = "CRASH!";
		else
			GUI_HP.text = "%" + HP.ToString ();
		//endif
	}

	//begin attack variables

	void ShotDMG () {
		HP -= 10;
		HP_IconCheck ();
	} //"standard"
	void ShipDMG () {
		HP -= 15;
		HP_IconCheck ();
	}//maybe reword as "Ram DMG"?

	void OnCollisionEnter (Collision col){
		if (col.gameObject.tag == "Enemy_Shot") {
			ShotDMG ();
			Debug.Log ("We're hit!");
		}
	}

	void UI_Code() {
		if (!centFire) {
			cb_v = cb_v + Time.deltaTime;
			if (cb_v >= loadRate) {
				cb_v = loadRate; //debug proof it
				centFire = true;
			}//end if
			centBar.value = calcCentLoad();
		} if (!leftFire) {
			lb_v = lb_v + Time.deltaTime;
			if (lb_v >= loadRate) {
				lb_v = loadRate; //debug proof it
				leftFire = true;
			}//end if
			leftBar.value = calcLeftLoad();
		} if (!rightFire) {
			rb_v = rb_v + Time.deltaTime;
			if (rb_v >= loadRate) {
				rb_v = loadRate; //debug proof it
				rightFire = true;
			}//end if
			rightBar.value = calcRightLoad();
		}//end if
	} //end UI Code

	//sets firing conditions to false, and empties GUI sliders of each bar
	void C_Load() {
		centFire = false;
		cb_v = 0;
		centBar.value = calcCentLoad ();
	} void L_Load() {
		leftFire = false;
		lb_v = 0;
		leftBar.value = calcLeftLoad ();
	} void R_Load() {
		rightFire = false;
		rb_v = 0;
		rightBar.value = calcRightLoad ();
	}	//void S_Load(){}
		//begin calc return values for loading values
	float calcCentLoad () {return cb_v;}
	float calcLeftLoad () {return lb_v;}
	float calcRightLoad () {return rb_v;}
	//float calcSpecLoad () {return sb_v;}
		//end firing trig code
}