using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPowerup : Pickup {
	[Header("Jet Powerup")]
	public float flyFor; //seconds
	public float maxSpeed;

	public override void ActivateEffect(){
		if(_cig.coroutines.ContainsKey("jet"))
			_cig.StopCoroutine(_cig.coroutines["jet"]);
		_cig.coroutines["jet"]=_cig.StartCoroutine(_cig.ActivateJet(flyFor, maxSpeed));
	}
}