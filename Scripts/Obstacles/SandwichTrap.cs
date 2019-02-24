using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwichTrap : Obstacle {
	[Header("SandwichTrap")]
	public Transform bottomPiece;
	public float maxBottomYmove;
	public Transform upperPiece;
	public float maxUpperYmove;

	public override void MoveY(float objdistances){
		bottomPiece.localPosition=Vector2.zero;
		upperPiece.localPosition=Vector2.zero;

		bottomPiece.Translate((1-objdistances)*maxBottomYmove*Vector3.up);
		upperPiece.Translate(-(1-objdistances)*maxUpperYmove*Vector3.up);
	}
}