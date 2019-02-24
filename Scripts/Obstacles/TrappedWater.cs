using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedWater : Obstacle {
	[Header("TrappedWater")]
	public int damageToTobacco;

	void OnTriggerEnter2D(Collider2D col){
		if(!col.isTrigger){
			AudioManager.s_inst.Play2DSound(4);
			if(col.gameObject.layer==9){
				_cigar.BH_HealOrDamage(-100, Tobacco.T_DamageHeal._BHDMGWET);
			}else if(col.gameObject.layer==8){
				_cigar.BH_HealOrDamage(-damageToTobacco, Tobacco.T_DamageHeal._BHDMGWET);
			}
		}
	}
}