using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour {
	public static GameMenu s_inst{get; private set;}
	public GameObject tutorialPanelFixedArrow;
	public GameObject tutorialPanelJoystick;
	GameObject tutorialPanel;
	public GameObject onDeathPanel;
	public GameObject ratePanel;
	public GameObject exitPanel;
	public GameObject blockControlls;
	public GameObject tapToRestartText;
	public GameObject continueAds;
	public GameObject showHighScore;
	public GameObject pauseMenuPanel;
	public GameObject noInternetConnection;
	public Toggle playSounds;

	float waitForAdsOportunity=2f;
	float deathPanelShowTimePlusWait;
	bool adInitialised=false;
	int nRuns=1;

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(s_inst);
			s_inst=this;
		}
	}
	void Start(){
		if(GameControllerV2.s_inst.LiveDataStorage.runTutorial){
			if(GameControllerV2.s_inst.LiveDataStorage.controllType==ControllType.Joystick)
				tutorialPanel=tutorialPanelJoystick;
			else
				tutorialPanel=tutorialPanelFixedArrow;
			tutorialPanel.SetActive(true);
			Time.timeScale=0f;
		}
		playSounds.isOn=GameControllerV2.s_inst.LiveDataStorage.playSounds;
	}
	void Update(){
		if(onDeathPanel.activeSelf && Time.time>deathPanelShowTimePlusWait){
			tapToRestartText.SetActive(true);
		}
	}
	public void ButtonRateUs(int stars){
		if(stars>2){
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.byteblaststudio.stillburning");
			GameControllerV2.s_inst.LiveDataStorage.positiveRating=true;
		}
		ShowRatePanel(false);
	}
	public void ShowRatePanel(bool b){
		ratePanel.SetActive(b);
	}
	public void ShowDeathPanel(bool b){
		onDeathPanel.SetActive(b);
		if(b && GameControllerV2.s_inst.Cig.LenghtTraveled>250f){
			//SHORT ADS
			if((nRuns+1)%3==0) MyAds.s_inst.RequestShortAd();
			//RATE US
			if(!GameControllerV2.s_inst.LiveDataStorage.positiveRating && GameControllerV2.s_inst.LiveDataStorage.nGamesRunned%2==0)
				ShowRatePanel(true);
			//
		}
		tapToRestartText.SetActive(false);
		if(!continueAds.activeSelf)
			deathPanelShowTimePlusWait=Time.time+waitForAdsOportunity/2;
		else
			deathPanelShowTimePlusWait=Time.time+waitForAdsOportunity;
	}
	public void ShowContinueAdsButton(bool b){
		continueAds.SetActive(b);
	}
	public void ShowNoInternetForAds(bool b){
		noInternetConnection.SetActive(b);
	}
	public void ShowHighScore(bool b){
		showHighScore.SetActive(b);
	}
	public void ShowPauseMenu(){
		pauseMenuPanel.SetActive(true);
		Time.timeScale=0f;
	}
	public void Resume(){
		pauseMenuPanel.SetActive(false);
		Time.timeScale=1f;
	}
	public void Restart(){
		if(GameControllerV2.s_inst.Cig.LenghtTraveled>250f) nRuns++;
		GameControllerV2.s_inst.RestartGame();
		pauseMenuPanel.SetActive(false);
		ShowDeathPanel(false);
		ShowContinueAdsButton(true);
		ShowNoInternetForAds(false);
		ShowHighScore(false);
		if(GameControllerV2.s_inst.LiveDataStorage.runTutorial){
			tutorialPanel.SetActive(true);
			Time.timeScale=0f;
		}else Time.timeScale=1f;
	}
	public void SwitchScene(int n){
		Time.timeScale=1f;
		GameControllerV2.s_inst.LiveDataStorage.forceQuit=false;
		SceneManager.LoadScene(n);
	}
	public void TapToRestart(){
		if(Time.time>deathPanelShowTimePlusWait && !adInitialised){
			Restart();
		}
	}
	public void Exit(){
		exitPanel.SetActive(true);
	}
	public void AreUSureExit(bool b){
		if(b){
			GameControllerV2.s_inst.LiveDataStorage.nGamesRunned++;
			GameControllerV2.s_inst.LiveDataStorage.forceQuit=false;
			Application.Quit();
		}else{
			exitPanel.SetActive(false);
		}
	}
	public void CloseTutorialPanel(){
		tutorialPanel.SetActive(false);
		Time.timeScale=1f;
	}

	public bool AdInitialised{
		set{adInitialised=value;}
	}
}