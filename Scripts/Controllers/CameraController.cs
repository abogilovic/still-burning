using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	Transform target;
	Vector3 newPosition;
	public float speed;
	public float yMin, yMax;
	public float xMin;
	public static float aspect_multiplier;

	void Awake(){
		aspect_multiplier=GetComponent<Camera>().aspect/(16f/9);
	}
	void Start(){
		if(target)
			transform.position = new Vector3(Mathf.Max(xMin, target.transform.position.x+5f), Mathf.Clamp(target.transform.position.y ,yMin ,yMax), transform.position.z);
	}
	void FixedUpdate(){
		if(target){
			newPosition = new Vector3(Mathf.Max(xMin, target.transform.position.x+5f), Mathf.Clamp(target.transform.position.y ,yMin ,yMax), transform.position.z);
			transform.position = Vector3.Lerp(transform.position, newPosition, speed*Time.deltaTime);
		}
	}
	public Transform Target{
		set{
			target=value;
		}
	}
}