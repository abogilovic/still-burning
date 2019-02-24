using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public CigaretteSelection csel;
	public GameObject selectcigPanel, exitPanel, infoPanel, optionsPanel, chooseModePanel;
	public Toggle enableTutorial;
	public Toggle enableSounds;
	public Toggle enableJoystick;
	public Text highScore;

	DataStorage1 liveDataStorage;

	void OnEnable(){
		//File.Delete(Application.persistentDataPath+"/dataStorage1.dat");
		liveDataStorage=Load();
		if(liveDataStorage.forceQuit) liveDataStorage.nGamesRunned++;
		enableTutorial.isOn=liveDataStorage.runTutorial;
		enableSounds.isOn=liveDataStorage.playSounds;
		enableJoystick.isOn=liveDataStorage.controllType==ControllType.Joystick;
		highScore.text=(liveDataStorage.highScore/100f).ToString();
	}
	void OnDisable(){
		liveDataStorage.runTutorial=enableTutorial.isOn;
		liveDataStorage.playSounds=enableSounds.isOn;
		if(enableJoystick.isOn) liveDataStorage.controllType=ControllType.Joystick;
		else liveDataStorage.controllType=ControllType.FixedArrows;
		Save();
	}
	void OnApplicationPause(bool p){
		liveDataStorage.forceQuit=true;
		if(Time.time>1f && p){
			liveDataStorage.runTutorial=enableTutorial.isOn;
			liveDataStorage.playSounds=enableSounds.isOn;
			if(enableJoystick.isOn) liveDataStorage.controllType=ControllType.Joystick;
			else liveDataStorage.controllType=ControllType.FixedArrows;
			Save();
		}
	}
	public void ChooseMode(bool b){
		chooseModePanel.SetActive(b);
	}
	public void SelectCigarette(bool b){
		selectcigPanel.SetActive(b);
	}
	public void Info(bool active){
		infoPanel.SetActive(active);
	}
	public void Options(bool active){
		optionsPanel.SetActive(active);
	}
	public void Exit(){
		exitPanel.SetActive(true);
	}
	public void AreUSureExit(bool b){
		if(b){
			liveDataStorage.nGamesRunned++;
			liveDataStorage.forceQuit=false;
			Application.Quit();
		}
		else exitPanel.SetActive(false);
	}

	void Save(){
		BinaryFormatter bf=new BinaryFormatter();
		FileStream file=File.Open(Application.persistentDataPath+"/dataStorage1.dat", FileMode.OpenOrCreate);
		bf.Serialize(file, liveDataStorage);
		file.Close();
	}

	DataStorage1 Load(){
		if(File.Exists(Application.persistentDataPath+"/dataStorage1.dat")){
			Debug.Log("FILE EXIST");
			BinaryFormatter bf=new BinaryFormatter();
			FileStream file=File.Open(Application.persistentDataPath+"/dataStorage1.dat", FileMode.Open);
			DataStorage1 dataStorage=(DataStorage1)bf.Deserialize(file);
			if(Application.version!=dataStorage.gameVersion)
				dataStorage.NewGameUpdate(Application.version, csel.nClassesSkins);
			file.Close();
			Debug.Log("DOBRO ODRADJENO");
			return dataStorage;
		}else{
			Debug.Log("VRACAM OVO");
			Debug.Log(Application.version);
			return new DataStorage1(Application.version, csel.nClassesSkins);
		}
	}
	public DataStorage1 LiveDataStorage{
		get{return liveDataStorage;}
	}
}