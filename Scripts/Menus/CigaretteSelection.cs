using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class CigaretteSelection : MonoBehaviour {
	public MainMenu mm;
	public int[] nClassesSkins;
	public CigaretteClass[] cigClasses;
	public Text[] properties;
	public Text liveGoldc;
	public Text className, skinName;
	public GameObject lockIndicator1;
	public GameObject lockIndicator2;
	public GameObject notEnoughText;
	public GameObject unlockPanel;
	public Text startText;
	public Text Price;
	public GameObject leftArrow, rightArrow, upArrow, downArrow;

	public Transform main_t;
	public float comfortDistance;
	public float distanceBetweenModels;


	Vector2 startPoint, endPoint;
	Vector3 moveTo; bool moving=false;

	int currentClass=0, currentSkin=0;

	void Start(){
		currentClass=mm.LiveDataStorage.playingClass;
		currentSkin=mm.LiveDataStorage.playingSkin;
		liveGoldc.text=mm.LiveDataStorage.goldCoinCount.ToString();
		main_t.localPosition=Vector3.left*currentClass*distanceBetweenModels+Vector3.down*currentSkin*distanceBetweenModels;
		UpdateClassInfo();
	}

	void Update(){
		if(moving){
			if(Vector3.Distance(main_t.localPosition, moveTo)<0.1f) moving=false;
			main_t.localPosition=Vector3.Lerp(main_t.localPosition, moveTo, 0.2f);
		}
	}
	public void StartGame(){
		if(mm.LiveDataStorage.ContentUnlockState[currentClass][currentSkin]){
			mm.LiveDataStorage.playingClass=currentClass;
			mm.LiveDataStorage.playingSkin=currentSkin;
			SceneManager.LoadScene(1);
		}
	}
	public void UnlockPanelState(bool state){
		unlockPanel.SetActive(state && mm.LiveDataStorage.goldCoinCount>=cigClasses[currentClass].skins[currentSkin].price);
	}
	public void UnlockContent(){
		if(mm.LiveDataStorage.goldCoinCount>=cigClasses[currentClass].skins[currentSkin].price){
			mm.LiveDataStorage.ContentUnlockState[currentClass][currentSkin]=true;
			mm.LiveDataStorage.goldCoinCount-=cigClasses[currentClass].skins[currentSkin].price;
			liveGoldc.text=mm.LiveDataStorage.goldCoinCount.ToString();
			UpdateSkinInfo();
			unlockPanel.SetActive(false);
		}
	}
	public void StartDrag(){
		#if UNITY_ANDROID
		if(Input.touchCount>0) startPoint=Input.GetTouch(0).position;
		#endif
	}
	public void EndDrag(){
		#if UNITY_ANDROID
			endPoint=Input.GetTouch(0).position;
			bool horizontalSwipe=false;
			float deltaX=endPoint.x-startPoint.x;
			float deltaY=endPoint.y-startPoint.y;
		 
			if(Mathf.Abs(deltaX)>=Mathf.Abs(deltaY)) horizontalSwipe=true;
			if(horizontalSwipe && Mathf.Abs(deltaX)>comfortDistance)
			if(deltaX>=0) Swipe(1); else Swipe(2);
			else if(Mathf.Abs(deltaY)>comfortDistance)
			if(deltaY>=0) Swipe(3); else Swipe(4);
		#endif
	}
	public void Swipe(int side){
		if(side==1 && currentClass>0){
			AudioManager.s_inst.Play2DSound(0);
			currentClass--;
			if(currentSkin>0){
				currentSkin=0;
				main_t.localPosition=Vector3.left*currentClass*distanceBetweenModels;
			}
			UpdateClassInfo();
		}else if(side==2 && currentClass<nClassesSkins.Length-1){
			AudioManager.s_inst.Play2DSound(0);
			currentClass++;
			if(currentSkin>0){
				currentSkin=0;
				main_t.localPosition=Vector3.left*currentClass*distanceBetweenModels;
			}
			UpdateClassInfo();
		}else if(side==3 && currentSkin>0){
			AudioManager.s_inst.Play2DSound(0);
			currentSkin--; UpdateSkinInfo();
		}
		else if(side==4 && currentSkin<nClassesSkins[currentClass]-1){
			AudioManager.s_inst.Play2DSound(0);
			currentSkin++; UpdateSkinInfo();
		}
		
		moveTo=Vector3.left*currentClass*distanceBetweenModels+Vector3.down*currentSkin*distanceBetweenModels;
		moving=true;
	}

	void UpdateClassInfo(){
		CigaretteClass cctemp=cigClasses[currentClass];
		className.text=cctemp.name;

		leftArrow.SetActive(currentClass!=0);
		rightArrow.SetActive(currentClass!=nClassesSkins.Length-1);

		for(int i=0; i<properties.Length; i++){
			if(i<cctemp.properties.Length) properties[i].text=cctemp.properties[i];
			else properties[i].text=string.Empty;
		}
		UpdateSkinInfo();
	}
	void UpdateSkinInfo(){
		skinName.text=cigClasses[currentClass].skins[currentSkin].name;
		downArrow.SetActive(currentSkin!=0);
		upArrow.SetActive(currentSkin!=nClassesSkins[currentClass]-1);

		bool locked = !mm.LiveDataStorage.ContentUnlockState[currentClass][currentSkin];
		lockIndicator1.SetActive(locked); lockIndicator2.SetActive(locked);
		if(locked){
			Price.text=cigClasses[currentClass].skins[currentSkin].price.ToString();
			notEnoughText.SetActive(mm.LiveDataStorage.goldCoinCount<cigClasses[currentClass].skins[currentSkin].price);
			startText.text=". . .";
		}else startText.text="Start";
	}
}

[Serializable]
public class CigaretteClass {
	public string name;
	public string[] properties;
	public CigaretteSkin[] skins;
}

[Serializable]
public class CigaretteSkin {
	public string name="SkinName";
	public int price;
}