using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShooter : Obstacle {
	[Header("TowerShooter")]
	public ProjectilePool projPool;
	public float shootEvery; float t=0;
	public int projDamage;
	public float projSpeed;
	public Transform space;
	Projectile proj=null;

	protected override void Update(){
		base.Update();
		if(Time.time>t){
			if(proj!=null){
				Shoot();
				t=Time.time+shootEvery;
			}else proj=projPool.RequestFreeProjectile(space);
		}
	}
	void Shoot(){
		proj.ActiveProjectile=true;
		proj.ShootProjectile(projDamage, projSpeed, -transform.right);
		//novi odmah u stocku
		proj=projPool.RequestFreeProjectile(space);
		if(proj!=null) proj.ActiveProjectile=true;
	}
	public override bool ActiveObstacle{
		get{return _activeObstacle;}
		set{
			_activeObstacle=value;
			if(_activeObstacle) transform.Translate(new Vector3(_width, 0, 0));
			else projPool.DeactivateAllAlive();
			gameObject.SetActive(_activeObstacle);
		}
	}
}