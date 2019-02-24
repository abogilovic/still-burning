using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : Obstacle {
	[Header("Pendulum")]
	public int hitDamage;
	public float hitEvery; float nextHit=0f;
	public float maxAngle;
	public float minAngle;
	public float angularVelocity;
	public float velCheckEvery;
	public float minAV;

	HingeJoint2D hj;
	Rigidbody2D rb;

	float nextCheck=0f;

	protected override void Awake(){
		base.Awake();
		hj=GetComponent<HingeJoint2D>();
		rb=GetComponent<Rigidbody2D>();

		JointAngleLimits2D ja=new JointAngleLimits2D();
		ja.max=maxAngle; ja.min=minAngle;
		hj.limits=ja;
	}
	void OnEnable(){
		rb.angularVelocity=angularVelocity;
		transform.rotation=new Quaternion();
		if(Random.value<0.5f) transform.Rotate(Vector3.forward, minAngle);
		else transform.Rotate(Vector3.forward, maxAngle);
	}
	protected override void Update(){
		base.Update();
		if(Time.time>nextCheck && Mathf.Abs(rb.angularVelocity)<minAV){
			rb.angularVelocity=-Mathf.Sign(transform.rotation.z)*angularVelocity;
			nextCheck=Time.time+velCheckEvery;
		}
	}
	void OnCollisionEnter2D(Collision2D col){
		if(Time.time>nextHit && col.gameObject.layer==8 || col.gameObject.layer==9){
			nextHit=Time.time+hitEvery;
			_cigar.H_HealOrDamage(-hitDamage);
		}
	}
}