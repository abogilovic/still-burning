using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matchthrower : Obstacle {
	[Header("Matchthrower")]
	public int matchDamages;
	public float matchSpeeds;
	public Match[] matches;
	bool throwMatches=false;

	IEnumerator ThrowMatches(float matchEvery){
		for(int i=0; i<matches.Length; i++){
			matches[i].ActivateSpear(matchDamages, matchSpeeds, Vector2.down);
			yield return new WaitForSeconds(matchEvery);
		}
	}

	public override bool ActiveObstacle{
		get{return _activeObstacle;}
		set{
			_activeObstacle=value;
			gameObject.SetActive(_activeObstacle);
			if(!_activeObstacle){
				for(int i=0; i<matches.Length; i++) matches[i].RespawnMatch();
				throwMatches=false;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(!throwMatches && col.gameObject.layer==8){ //8 Cigarette Layer
			StartCoroutine(ThrowMatches(0.2f));
			throwMatches=true;
		}
	}
}