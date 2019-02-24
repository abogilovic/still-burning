using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public AudioSource shootSound;
	public float flyingFor; float startFly=0; bool flying=false;
	public ParticleSystem ps;

	bool _activeProjectile=false;
	bool doDamage=true;
	int projDamage;
	Rigidbody2D _rb;
	protected Tobacco _cigar;

	void Awake(){
		_cigar=Tobacco.s_inst;
		_rb=GetComponent<Rigidbody2D>();
	}
	void Update(){
		if(flying && Time.time>startFly+flyingFor) ActiveProjectile=false; 
	}
	public void ShootProjectile(int pDamage, float pSpeed, Vector3 thisWay){
		projDamage=pDamage;
		doDamage=true;
		_rb.gravityScale=1f;
		_rb.velocity=thisWay*pSpeed;
		startFly=Time.time;
		shootSound.Play();
		flying=true;
	}
	public bool ActiveProjectile{
		get{return _activeProjectile;}
		set{
			_activeProjectile=value;
			if(_activeProjectile){
				ps.Play();
				flying=false;
				_rb.gravityScale=0f;
				_rb.velocity=Vector2.zero;
				transform.localPosition=Vector3.zero;
				transform.localRotation=new Quaternion();
			}
			gameObject.SetActive(_activeProjectile);
		}
	}
	void OnCollisionEnter2D(Collision2D col){
		if(doDamage && col.gameObject.layer==8){ //8 Cigarette Layer
			_cigar.H_HealOrDamage(-projDamage);
		}
		doDamage=false;
		ps.Stop();
	}
}