using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : Obstacle {
	public AudioSource shootSound;
	int matchDamage;
	[Header("Match")]
	public ParticleSystem ps;
	bool doDamage=true;
	Rigidbody2D rb;

	void Start(){
		rb=GetComponent<Rigidbody2D>();
	}
	public void ActivateSpear(int sDmg, float sSpeed, Vector2 thisWay){
		shootSound.Play();
		matchDamage=sDmg;
		rb.gravityScale=1f;
		rb.velocity=thisWay*sSpeed;
	}
	public void RespawnMatch(){
		doDamage=true;
		transform.localPosition=Vector3.zero;
		transform.localRotation=new Quaternion();
		rb.gravityScale=0f;
		ps.Play();
	}
	void OnCollisionEnter2D(Collision2D col){
		if(doDamage && col.gameObject.layer==8) //8 Cigarette Layer
			_cigar.H_HealOrDamage(-matchDamage);
		doDamage=false;
		ps.Stop();
	}
}
