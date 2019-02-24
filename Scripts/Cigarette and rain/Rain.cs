using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour {
	public static Rain s_inst{get; private set;}
	float ind_xpos;

	AccessUI ui;
	Tobacco cig;
	Transform t_embers;
	Rigidbody2D rb;
	bool fastrain=false;
	float nextTick=0f;

	[Header("Rain")]
	public float rainSpeed;

	[Header("BurningHealth")]
	public int rainDamage;
	public float tickEvery;

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(s_inst);
			s_inst=this;
		}
	}
	void Start(){
		cig=Tobacco.s_inst;
		ui=AccessUI.s_inst;
		ind_xpos=8.2f*CameraController.aspect_multiplier;
		ui.rainDistanceIndicator.position=new Vector3(Camera.main.transform.position.x-ind_xpos, ui.rainDistanceIndicator.position.y, ui.rainDistanceIndicator.position.z);
		rb=GetComponent<Rigidbody2D>();
		Move(true);
	}
	void FixedUpdate(){
		if(cig.Alive){
			if(t_embers.position.x<=transform.position.x-5f){
				cig.BH_HealOrDamage(-cig.BurningHealth, Tobacco.T_DamageHeal._BHDMGWET);
				cig.LockOnAxis(false, Vector2.right);
			}

			if(!fastrain && t_embers.position.x>=transform.position.x+25f){
				fastrain=true; Move(true);
			}else if(fastrain && t_embers.position.x<transform.position.x+25f){
				fastrain=false; Move(true);
			}

			if(Time.time>nextTick && t_embers.position.x<=transform.position.x+10f){
				cig.BH_HealOrDamage(-rainDamage, Tobacco.T_DamageHeal._BHDMGWET);
				nextTick=Time.time+tickEvery;
			}
			int a=Mathf.RoundToInt(2.7f*(t_embers.position.x-(transform.position.x+10f)));
			if(a>=0) ui.rainDistanceText.text=(a/100f).ToString();
			else ui.rainDistanceText.text=string.Empty;
		}else if(t_embers.position.x<=transform.position.x-5f) cig.LockOnAxis(false, Vector2.right);
	
		if(transform.position.x+10f>Camera.main.transform.position.x-ind_xpos)
			ui.rainDistanceIndicator.position=new Vector3(transform.position.x+10f, ui.rainDistanceIndicator.position.y, ui.rainDistanceIndicator.position.z);
	}
	public void RestartRainPositionComparedToPlayer(){
		StartCoroutine(StopAndMoveRainIn(5));
		if(cig.LastTimePos.x<transform.position.x+10f)
			transform.position=new Vector3(cig.LastTimePos.x-15f, transform.position.y);
		
		if(transform.position.x+10f>Camera.main.transform.position.x-ind_xpos)
			ui.rainDistanceIndicator.position=new Vector3(transform.position.x+10f, ui.rainDistanceIndicator.position.y, ui.rainDistanceIndicator.position.z);
		else
			ui.rainDistanceIndicator.position=new Vector3(Camera.main.transform.position.x-ind_xpos, ui.rainDistanceIndicator.position.y, ui.rainDistanceIndicator.position.z);
	}
	public IEnumerator StopAndMoveRainIn(int inSec){
		Move(false);
		ui.rainPause.SetActive(true);
		WaitForSeconds secDelay=new WaitForSeconds(1f);
		while(inSec>0){
			ui.rainPauseCount.text=inSec.ToString();
			inSec--;
			yield return secDelay;
		}
		ui.rainPause.SetActive(false);
		ui.rainPauseCount.text=string.Empty;
		Move(true);
	}

	public void Move(bool toMove){
		if(toMove){
			if(fastrain) rb.velocity=Vector2.right*2.5f*rainSpeed;
			else rb.velocity=Vector2.right*rainSpeed;
		}else rb.velocity=Vector2.zero;
	}
	public void RestartRain(){
		Move(true);
		transform.position=new Vector3(-20f, 15f);
		StopAllCoroutines();
		ui.rainPause.SetActive(false);
		ui.rainPauseCount.text=string.Empty;

		if(transform.position.x+10f>Camera.main.transform.position.x-ind_xpos)
			ui.rainDistanceIndicator.position=new Vector3(transform.position.x+10f, ui.rainDistanceIndicator.position.y, ui.rainDistanceIndicator.position.z);
		else
			ui.rainDistanceIndicator.position=new Vector3(Camera.main.transform.position.x-ind_xpos, ui.rainDistanceIndicator.position.y, ui.rainDistanceIndicator.position.z);
	}
	public Transform T_embers{
		set{t_embers=value;}
	}
}