using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepeljaraCoin : Pickup {
	[Header("PepeljaraCoin")]
	public bool goldCoin=false;
	public int coinWorth;

	public override void ActivateEffect(){
		if(!goldCoin){
			_cig.SilverCoinCount+=coinWorth;
			AudioManager.s_inst.Play2DSound(1);
		}
		else {
			GameControllerV2.s_inst.LiveDataStorage.goldCoinCount+=coinWorth;
			_cig.GoldCoinCount+=coinWorth;
			AudioManager.s_inst.Play2DSound(2);
			GameControllerV2.s_inst.UpdateGoldCoinCountUI();
		}
	}
}