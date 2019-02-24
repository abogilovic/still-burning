using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCamp : Obstacle {
	[Header("FireCamp")]
	public int damagePerTick;
	public float tickEvery;
	float nextTick=0;

	void OnTriggerStay2D(Collider2D col){
		if(Time.time>nextTick && (col.gameObject.layer==8 || col.gameObject.layer==9) && !col.isTrigger){
			_cigar.H_HealOrDamage(-damagePerTick);
			nextTick=Time.time+tickEvery;
		}
	}
}