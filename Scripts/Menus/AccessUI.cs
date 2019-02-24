using UnityEngine;
using UnityEngine.UI;

public class AccessUI : MonoBehaviour{
	public static AccessUI s_inst;
	public GameObject joystick;
	public GameObject rightArrow, leftArrow;
	public Slider healthSlider;
	public Text healthText;
	public Slider burningHealthSlider;
	public Text burningHealthText;
	public Text lenght;
	public GameObject jetIndicator;
	public Slider jetSlider;

	public Image heal_DmgIndicator;
	public Text healDmgValue;
	public Text healDmgHeader;
	public AnimationCurve animCurveHealDmg;
	public Image healIcon;
	public Color healColor;
	public Image squashDmgIcon;
	public Color squashDmgColor;
	public Image wetDmgIcon;
	public Color wetDmgColor;

	public Color onButtonColor;
	public Image leftRotIcon;
	public Image rightRotIcon;
	public Image launchIcon;

	public Text onDeathLenght;
	public Text onDeathSilverCoin;
	public Text onDeathGoldCoin;
	public Text onDeathScore;

	public Transform rainDistanceIndicator; 
	public Text rainDistanceText;
	public GameObject rainPause;
	public Text rainPauseCount;

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(this);
			s_inst=this;
		}
	}
	void Start(){
		if(GameControllerV2.s_inst.LiveDataStorage.controllType==ControllType.Joystick) joystick.SetActive(true);
		else{
			rightArrow.SetActive(true); leftArrow.SetActive(true);
		}
	}
}