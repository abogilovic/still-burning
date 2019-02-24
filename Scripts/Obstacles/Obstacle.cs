using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {
	[Header("Obstacle")]
	public float _width;
	public float _yHeightOffGround;
	public bool yMoveAble;
	public float maxYMove;

	protected bool _activeObstacle=false;
	float deleteAfterX;
	protected Tobacco _cigar;

	protected virtual void Awake(){
		_cigar=Tobacco.s_inst;
	}
	void Start(){
		deleteAfterX=15f*CameraController.aspect_multiplier;
	}

	protected virtual void Update(){
		if(_activeObstacle && _cigar.Alive && _cigar.transform.position.x-deleteAfterX>transform.position.x+_width) ActiveObstacle=false;
	}

	public virtual void MoveY(float objdistances){
		transform.Translate(objdistances*Mathf.Sign(_yHeightOffGround)*maxYMove*Vector3.up);
	}

	public virtual bool ActiveObstacle{
		get{return _activeObstacle;}
		set{
			_activeObstacle=value;
			gameObject.SetActive(_activeObstacle);
		}
	}
}