using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RizlaHealPowerup : Pickup {
	[Header("Rizla Heal Powerup")]
	public	int healFor;

	public override void ActivateEffect(){
		_cig.H_HealOrDamage(healFor);
		AudioManager.s_inst.Play2DSound(3);
	}
}