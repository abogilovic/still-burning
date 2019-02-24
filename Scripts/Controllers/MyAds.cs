using UnityEngine;
using UnityEngine.Advertisements;

public class MyAds : MonoBehaviour {
	public static MyAds s_inst{get; private set;}

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(s_inst);
			s_inst=this;
		}
	}
	public void RequestReviveAd(){
		GameMenu.s_inst.AdInitialised=true;
		if(Advertisement.IsReady("rewardedVideo")){
			ShowOptions sOpt=new ShowOptions();
			sOpt.resultCallback=RunReviveAD;
			Advertisement.Show("rewardedVideo", sOpt);
		}else{
			GameMenu.s_inst.AdInitialised=false;
			GameMenu.s_inst.ShowNoInternetForAds(true);
		}
	}
	public void RequestShortAd(){
		GameMenu.s_inst.AdInitialised=true;
		if(Advertisement.IsReady()){
			ShowOptions sOpt=new ShowOptions();
			sOpt.resultCallback=RunShortAd;
			Advertisement.Show(sOpt);
		}else{
			GameMenu.s_inst.AdInitialised=false;
		}
	}
	void RunShortAd(ShowResult sr){
		GameMenu.s_inst.AdInitialised=false;
	}
	void RunReviveAD(ShowResult sr){
		if(sr==ShowResult.Finished){
			GameMenu.s_inst.ShowDeathPanel(false);
			GameMenu.s_inst.ShowContinueAdsButton(false);
			GameControllerV2.s_inst.Cig.ReviveOrRestart(false);
			Rain.s_inst.RestartRainPositionComparedToPlayer();
		}
		GameMenu.s_inst.AdInitialised=false;
	}
}