using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Tobacco : MonoBehaviour{
	public static Tobacco s_inst{get; private set;}
	public Embers embers;
	public Transform tfParent;
	AccessUI ui;

	float e0;
	bool alive=true;
	int trenLenght=0, lastFrameLenght=0;
	float health=100f;
	float yStartScale;
	int burninghealth=100;
	float regenBurnHealth=0f;
	float pocetnaPozicijaX;
	float cantGoBackX, cntBackAfter;
	float cigDecrement;
	Rigidbody2D rb; float nextTick=0f;
	float velMag=0f;
	int thisGamesilverCoinCount=0, thisGamegoldCoinCount=0;
	Vector3 lastTimePos;
	Quaternion lastTimeRot;
	Vector3 pocPos;
	Quaternion pocRot;
	public bool shrinkable=true;
	public Dictionary<string, Coroutine> coroutines= new Dictionary<string, Coroutine>();

	[Header("Health")]
	public float burnTime;
	public float tickEvery;
	public float dmgReduction;

	[Header("BurningHealth")]
	public float burnRegenEvery;
	public int regenFor;

	[Header("Launch system")]
	public float launchPower;
	public float launchTime;
	float nextPowerfill=0;
	public BoxCollider2D groundDetectCol;
	public LayerMask launchLayers; 
	public Transform forceLaunch;
	public Transform upLaunch;
	bool powerfilling=false;
	float livelaunchPower, startLaunch;
	bool checkForGround=false;

	[Header("Rotation system")]
	ControllType controllType;
	public float rotationPower;
	public Transform forceRot;
	public Transform rightRot;
	bool rotation=false; float siderot;

	[Header("Jet Flying")]
	public GameObject jetPrefab;
	float jetMaxSpeed;
	bool jetFlying=false, breakingJetFly=false;
	float boostFlyStEnd, onBreak_boostingFor=0f, onStart_boostingFor=0f;
	GameObject jetClone=null;

	[Header("UISpecific")]
	public Slider launchSlider;

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(s_inst);
			s_inst=this;
		}
	}
	void Start(){
		controllType = GameControllerV2.s_inst.LiveDataStorage.controllType;
		ui=AccessUI.s_inst;
		if(shrinkable) e0=embers.transform.localScale.y;
		pocPos=transform.position;
		pocRot=transform.rotation;
		if(shrinkable) yStartScale=transform.localScale.y;
		rb=GetComponentInParent<Rigidbody2D>();
		ui.healthSlider.maxValue=Health;
		launchSlider.maxValue=launchTime;
		cigDecrement=1/(burnTime*60/tickEvery);
		pocetnaPozicijaX=embers.transform.position.x;
		cntBackAfter=10f*CameraController.aspect_multiplier;
		cantGoBackX=transform.position.x-cntBackAfter;
		nextTick=Time.time+tickEvery;
	}
	void FixedUpdate(){
		velMag=rb.velocity.magnitude;
		if(alive){
			if(controllType==ControllType.Joystick){
				siderot=CrossPlatformInputManager.GetAxis("Horizontal");
				rotation=siderot!=0f;
			}
			if(rotation)
				rb.AddForceAtPosition(siderot*rotationPower*(jetFlying?0.75f:1f)*rb.mass*(Vector2)(rightRot.position-forceRot.position), (Vector2)forceRot.position);
			if(checkForGround)
				powerFillingLaunch();
			if(jetFlying){
				if(breakingJetFly){
					onBreak_boostingFor=Mathf.Max(onStart_boostingFor-12*(Time.time-boostFlyStEnd),0f);
					rb.velocity=onBreak_boostingFor*(Vector2)(upLaunch.position-forceLaunch.position);
				}else{
					onStart_boostingFor=Mathf.Min(onBreak_boostingFor+4*(Time.time-boostFlyStEnd),jetMaxSpeed);
					rb.velocity=onStart_boostingFor*(Vector2)(upLaunch.position-forceLaunch.position);
				}
			}
			if(powerfilling){
				float t = Time.time-startLaunch;
				if(t<=launchTime) launchSlider.value=t;
			}
		}
	}
	void Update(){
		if(alive){
			//cig burning
			if(Time.time>nextTick){
				nextTick=Time.time+tickEvery;
				if(shrinkable){
					transform.localScale = new Vector3(1f, transform.localScale.y-(0.9f*cigDecrement), 1f);
					embers.transform.localScale = new Vector3(1f, e0/transform.localScale.y, 1f);
				}
				Health-=100f*cigDecrement;
			}
			//cig burning health regen
			if(Time.time>regenBurnHealth){
				regenBurnHealth=Time.time+burnRegenEvery;
				if(BurningHealth<100) BurningHealth+=regenFor;
			}
			//cig score update
			trenLenght=Mathf.RoundToInt(2.7f*(embers.transform.position.x-pocetnaPozicijaX));
			if(trenLenght>lastFrameLenght){
				ui.lenght.text=(trenLenght/100f).ToString();
				lastFrameLenght=trenLenght;
				cantGoBackX=transform.position.x-cntBackAfter;
			}
		}
		if(transform.position.x<cantGoBackX) LockOnAxis(true, Vector2.right);
		if(transform.position.y>13f) LockOnAxis(true, Vector2.up);
	}
	public void StartGameCigarLaunch(){
		rb.AddForceAtPosition(launchPower*1.5f*rb.mass*(Vector2)(upLaunch.position-forceLaunch.position), (Vector2)forceLaunch.position, ForceMode2D.Impulse);
  	}
	public void LockOnAxis(bool hardlock, Vector2 axis_n){
		rb.velocity=new Vector2(axis_n.y*rb.velocity.x, axis_n.x*rb.velocity.y);
		if(hardlock)
			rb.transform.position=new Vector3(cantGoBackX*axis_n.x + transform.position.x*axis_n.y,
				13f*axis_n.y + transform.position.y*axis_n.x);
	}
	public void buttonStopRotation(){
		rotation=false;
		if(siderot>0) ui.rightRotIcon.color=Color.white;
		else ui.leftRotIcon.color=Color.white;
	}
	public void buttonRotation(float side){
		rotation=true;
		siderot=side;
		if(siderot>0) ui.rightRotIcon.color=ui.onButtonColor;
		else ui.leftRotIcon.color=ui.onButtonColor;
	}
	public void buttonPowerFillingLaunch(){
		ui.launchIcon.color=ui.onButtonColor;
		if(!jetFlying)
			checkForGround=true;
		else{
			breakingJetFly=true;
			boostFlyStEnd=Time.time;
		}
	}
	void powerFillingLaunch(){
		if(Time.time > nextPowerfill && groundDetectCol.IsTouchingLayers(launchLayers)){
			nextPowerfill=Time.time+0.5f;
			startLaunch=Time.time;
			powerfilling=true;
			checkForGround=false;
		}
	}
	public void buttonLaunch(){
		if(!jetFlying){
			if(powerfilling && groundDetectCol.IsTouchingLayers(launchLayers) && alive){
				livelaunchPower=Time.time-startLaunch;
				if(livelaunchPower>=launchTime) livelaunchPower=1f;
				else livelaunchPower+=1-launchTime;
				livelaunchPower*=launchPower;
				rb.AddForceAtPosition(livelaunchPower*rb.mass*(Vector2)(upLaunch.position-forceLaunch.position), (Vector2)forceLaunch.position, ForceMode2D.Impulse);
			}
			checkForGround=false;
		}
		ui.launchIcon.color=Color.white;
		powerfilling=false;
		launchSlider.value=0f;
		breakingJetFly=false;
		boostFlyStEnd=Time.time;
	}
	public void ReviveOrRestart(bool restart){
		alive=true;
		H_HealOrDamage(100);
		BurningHealth=100;
		rb.velocity=Vector2.zero;

		if(!restart){
			rb.transform.position=lastTimePos;
			rb.transform.localRotation=lastTimeRot;
		}else{
			rb.transform.position=pocPos;
			rb.transform.localRotation=pocRot;
			SilverCoinCount=0; GoldCoinCount=0;
			trenLenght=0; lastFrameLenght=0; ui.lenght.text="0";
			cantGoBackX=transform.position.x-cntBackAfter;
		}
		embers.Relight();
		nextTick=Time.time+tickEvery;
	}
	IEnumerator DisplayAnyDamageOrHeal(int value, T_DamageHeal dmgHealType){
		if(dmgHealType==T_DamageHeal._BHDMGWET || dmgHealType==T_DamageHeal._BHDMGSQUASH) ui.healDmgHeader.text="BHP";
		else ui.healDmgHeader.text="HP";

		if(dmgHealType==T_DamageHeal._BHDMGSQUASH || dmgHealType==T_DamageHeal._HDMGSQUASH){
			ui.wetDmgIcon.gameObject.SetActive(false);
			ui.healIcon.gameObject.SetActive(false);
			ui.heal_DmgIndicator.color=ui.squashDmgColor;
			ui.squashDmgIcon.gameObject.SetActive(true);
			ui.healDmgValue.color=ui.squashDmgColor;
			ui.healDmgHeader.color=ui.squashDmgColor;
		}else if(dmgHealType==T_DamageHeal._BHDMGWET){
			ui.squashDmgIcon.gameObject.SetActive(false);
			ui.healIcon.gameObject.SetActive(false);
			ui.heal_DmgIndicator.color=ui.wetDmgColor;
			ui.wetDmgIcon.gameObject.SetActive(true);
			ui.healDmgValue.color=ui.wetDmgColor;
			ui.healDmgHeader.color=ui.wetDmgColor;
		}else if(dmgHealType==T_DamageHeal._HHEAL){
			ui.wetDmgIcon.gameObject.SetActive(false);
			ui.squashDmgIcon.gameObject.SetActive(false);
			ui.heal_DmgIndicator.color=ui.healColor;
			ui.healIcon.gameObject.SetActive(true);
			ui.healDmgValue.color=ui.healColor;
			ui.healDmgHeader.color=ui.healColor;
		}

		ui.heal_DmgIndicator.gameObject.SetActive(true);
		ui.healDmgValue.text=value.ToString();
		ui.healDmgHeader.gameObject.SetActive(true);

		float t_start=Time.time;
		Color c=ui.heal_DmgIndicator.color;
		Color white=Color.white;
		while(Time.time-t_start<1.5f){
			c.a=ui.animCurveHealDmg.Evaluate((Time.time-t_start)/1.5f);
			ui.heal_DmgIndicator.color=c;
			yield return null;
		}

		if(dmgHealType==T_DamageHeal._BHDMGSQUASH || dmgHealType==T_DamageHeal._HDMGSQUASH) ui.squashDmgIcon.gameObject.SetActive(false);
		else if(dmgHealType==T_DamageHeal._BHDMGWET) ui.wetDmgIcon.gameObject.SetActive(false);
		else if(dmgHealType==T_DamageHeal._HHEAL) ui.healIcon.gameObject.SetActive(false);

		ui.heal_DmgIndicator.gameObject.SetActive(false);
		ui.healDmgHeader.gameObject.SetActive(false);
	}
	public enum T_DamageHeal{
		_BHDMGWET, _BHDMGSQUASH, _HDMGSQUASH, _HHEAL
	}
	public void H_HealOrDamage(int bonusHealth){
		if(alive){
			if(bonusHealth<0){
				bonusHealth=Mathf.RoundToInt(bonusHealth*(1f-dmgReduction));
				if(bonusHealth==0) bonusHealth=-1;
			}
			Health+=bonusHealth;
			if(bonusHealth!=100){
				if(coroutines.ContainsKey("ui_display"))
					StopCoroutine(coroutines["ui_display"]);
				if(bonusHealth>0) 
					coroutines["ui_display"]=StartCoroutine(DisplayAnyDamageOrHeal(bonusHealth, T_DamageHeal._HHEAL));
				else coroutines["ui_display"]=StartCoroutine(DisplayAnyDamageOrHeal(bonusHealth, T_DamageHeal._HDMGSQUASH));
			}
			if(shrinkable){
				if(health+bonusHealth>=100f)
					transform.localScale = new Vector3(1f, yStartScale, 1f);
				else if(health+bonusHealth<0)
					transform.localScale = new Vector3(1f, yStartScale*0.1f, 1f);
				else 
					transform.localScale = new Vector3(1f, transform.localScale.y + (bonusHealth/100f-bonusHealth/1000f)*yStartScale, 1f);
				embers.transform.localScale = new Vector3(1f, e0/transform.localScale.y, 1f);
			}
		}
	}
	public void BH_HealOrDamage(int bhDmgValue, T_DamageHeal dmgType){
		if(alive){
			if(bhDmgValue<0){
				if(bhDmgValue!=-100) bhDmgValue=Mathf.RoundToInt(bhDmgValue*(1f-dmgReduction));
				else bhDmgValue=-burninghealth;
				if(bhDmgValue==0) bhDmgValue=-1;
			}
			BurningHealth+=bhDmgValue;
			if(coroutines.ContainsKey("ui_display"))
				StopCoroutine(coroutines["ui_display"]);
			coroutines["ui_display"]=StartCoroutine(DisplayAnyDamageOrHeal(bhDmgValue, dmgType));
		}
	}
	public IEnumerator ActivateJet(float flyFor, float maxSpeed){
		AudioManager.s_inst.Play2DSound(6);
		ui.jetIndicator.SetActive(true);
		jetFlying=true;
		jetMaxSpeed=maxSpeed;
		if(jetClone==null) jetClone=Instantiate(jetPrefab, tfParent);
		else jetClone.SetActive(true);
		boostFlyStEnd=Time.time;

		float t_start=Time.time;
		while(Time.time-t_start<flyFor){
			ui.jetSlider.value=1f-(Time.time-t_start)/flyFor;
			yield return null;
		}

		AudioManager.s_inst.Stop2DSound(6);
		if(breakingJetFly) checkForGround=true;
		ui.jetIndicator.SetActive(false);
		jetFlying=false;
		jetClone.SetActive(false);
		onBreak_boostingFor=0f;
		onStart_boostingFor=0f;
	}
	public float VelMagnitude{
		get{
			//Debug.Log("Hitting Velocity = "+velMag);
			return velMag;
		}
	}
	public float Health{
		get{return health;}
		set{
			if(alive){
				if(value>100f) health=100f;
				else if(value<0) health=0f;
				else health=value;
				ui.healthSlider.value=health;
				ui.healthText.text=ui.healthSlider.value.ToString();
				if(health<=0) embers.KillCig();
			}
		}
	}
	public int BurningHealth{
		get{return burninghealth;}
		set{
			if(alive){
				burninghealth=value;
				ui.burningHealthSlider.value=burninghealth;
				ui.burningHealthText.text=ui.burningHealthSlider.value.ToString();
				if(burninghealth<=0) embers.KillCig();
			}
		}
	}
	public bool Alive{
		get{return alive;}
		set{alive=value;
			if(!alive){
				float x=lastFrameLenght+thisGamesilverCoinCount*0.1f*100+thisGamegoldCoinCount*100;
				if(x>GameControllerV2.s_inst.LiveDataStorage.highScore){
					GameControllerV2.s_inst.LiveDataStorage.highScore=x;
					GameMenu.s_inst.ShowHighScore(true);
					GameControllerV2.s_inst.UpdateHighScoreUI();
				}
				if(jetFlying){
					StopCoroutine(coroutines["jet"]);
					AudioManager.s_inst.Stop2DSound(6);
					ui.jetIndicator.SetActive(false);
					jetFlying=false;
					jetClone.SetActive(false);
					onBreak_boostingFor=0f;
					onStart_boostingFor=0f;
				}
				Rain.s_inst.Move(false);
				lastTimePos=transform.position;
				lastTimeRot=transform.rotation;
				ui.onDeathLenght.text=ui.lenght.text;
				ui.onDeathGoldCoin.text=thisGamegoldCoinCount.ToString();
				ui.onDeathSilverCoin.text=thisGamesilverCoinCount.ToString();
				ui.onDeathScore.text=(x/100f).ToString();
			}
		}
	}
	public Vector3 LastTimePos{
		get{return lastTimePos;}
  	}
	public float LenghtTraveled{
		get{return lastFrameLenght;}
	}
	public int SilverCoinCount{
		get{return thisGamesilverCoinCount;}
		set{
			thisGamesilverCoinCount=value;
			if(thisGamesilverCoinCount==10){
				thisGamegoldCoinCount++;
				GameControllerV2.s_inst.LiveDataStorage.goldCoinCount++;
				GameControllerV2.s_inst.UpdateGoldCoinCountUI();
				thisGamesilverCoinCount=0;
			}
			GameControllerV2.s_inst.silverCoinCount.text=thisGamesilverCoinCount.ToString();
		}
	}
	public int GoldCoinCount{
		get{return thisGamegoldCoinCount;}
		set{
			thisGamegoldCoinCount=value;
		}
	}
}