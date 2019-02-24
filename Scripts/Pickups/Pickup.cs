using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour {
	[Header("Pickup")]
	public float _pickupSpeed;

	bool _activePickup=false;
	Transform t_players;
	bool _triggered=false; float timeWhenTrig;
	Rigidbody2D _rb;
	protected Tobacco _cig;
	float deleteAfterX;

	void Start(){
		_cig=Tobacco.s_inst;
		_rb=GetComponent<Rigidbody2D>();
		deleteAfterX=15f*CameraController.aspect_multiplier;
	}

	void FixedUpdate(){
		if(_activePickup && transform.position.x<t_players.position.x-deleteAfterX && !_triggered) ActivePickup=false;

		if(_triggered){
			if(_cig.Alive){
				_rb.velocity=new Vector2(_cig.transform.position.x-transform.position.x, _cig.transform.position.y-transform.position.y)*_pickupSpeed*(1+Time.time-timeWhenTrig);
				if(Vector2.Distance(_cig.transform.position, transform.position)<1f){
					ActivateEffect();
					ActivePickup=false;
				}
			}else{
				_rb.velocity=Vector2.zero;
				_triggered=false;
			}
		}
	}

	public abstract void ActivateEffect();

	public bool ActivePickup{
		get{return _activePickup;}
		set{
			_activePickup=value;
			gameObject.SetActive(_activePickup);
			_triggered=!_activePickup;
		}
	}

	public Transform PlayersTransform{
		set{t_players=value;}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(!_triggered){
			timeWhenTrig=Time.time;
			_triggered=true;
		}
	}
}