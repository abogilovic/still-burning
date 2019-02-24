using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Embers : MonoBehaviour {
	[Header("Particle effects")]
	public bool smokeable=true;
	public ParticleSystem smoke;
	public ParticleSystem burnout;
	SpriteRenderer sr;

	void Awake () {
		if(smokeable) burnout.Stop();
		sr=GetComponent<SpriteRenderer>();
	}
	public void Relight(){
		sr.color=Color.white;
		if(smokeable) smoke.Play();
	}
	public void KillCig(){
		Tobacco.s_inst.Alive=false;
		GameMenu.s_inst.ShowDeathPanel(true);
		if(smokeable){
			burnout.Play();
			smoke.Stop();
			AudioManager.s_inst.Play2DSound(5);
		}
		sr.color=Color.gray;
	}
	void OnCollisionEnter2D(Collision2D col){
		if(Tobacco.s_inst.Alive){
			if(Tobacco.s_inst.VelMagnitude>10f){
				Tobacco.s_inst.BH_HealOrDamage(-100, Tobacco.T_DamageHeal._BHDMGSQUASH);
			}else{
				if(smokeable) burnout.Emit(Mathf.FloorToInt(1f+Tobacco.s_inst.VelMagnitude)*3);
				Tobacco.s_inst.BH_HealOrDamage(-Mathf.FloorToInt(1f+Tobacco.s_inst.VelMagnitude)*5, Tobacco.T_DamageHeal._BHDMGSQUASH);
			}
		}
	}
}