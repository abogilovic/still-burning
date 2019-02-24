using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameControllerV2 : MonoBehaviour {
	public static GameControllerV2 s_inst{get; private set;}
	public float startGameLaunchTime;
	public Obstacle StartPlatform;

	public Vector2 cigspawn;
	public GameObject[] cigs;
	Tobacco cig;

	[Header("Map Generation")]
	public float criticalLenght;
	public float[] stageStartLenghts;
	public AnimationCurve obstacleDistances;
	public AnimationCurve obstacleHeights;
	public float visionLenght=15f;
	public Transform ObstacleHeap;
	public Transform PE1Heap;
	public float pe1resistance;
	public Transform PE2Heap;
	public float pe2resistance;
	public Transform pickupsHeap;
	public LayerMask layerMaskForPickups;

	public int[] stageObsts;
//ground
	public ObstaclePool groundPool;
	float groundHeight=0f;
	float nextGroundLenght=35f;
//realObstacles
	public ObstaclePool[] obstaclesPools;
	float nextObstLenght=35f;
//PE1
	public ObstaclePool[] pe1Pools;
	float nextPE1Lenght=0f;
	float startPE1x;
//PE2
	public ObstaclePool[] pe2Pools;
	float nextPE2Lenght=0f;
	float startPE2x;
//pickups
	public Vector2 rizlaHealOverDist;
	Vector2 nextRizlaValDistance;
	float nextCoinLenght=40f;
	float nextJetPowerupLenght;
	public PickupPool rizlaHealPool;
	public PickupPool silverCoinPool;
	public PickupPool goldCoinPool;
	public PickupPool jetPowerupPool;
	public AnimationCurve rizlaHealProbability; //over Distance
	public AnimationCurve silverCoinProbability; //over Distance
	public AnimationCurve goldCoinProbability; //over Distance
	public AnimationCurve jetPowerupProbability; //over Distance

	[Header("Live Data Stuff")]
	public Text highScore;
	public Text goldCoinCount;
	public Text silverCoinCount;
	DataStorage1 liveDataStorage;

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(s_inst);
			s_inst=this;
		}
		liveDataStorage=Load();
		SettingRightCigarette();
		AudioListener.pause=!liveDataStorage.playSounds;
	}
	void Start(){
		UpdateHighScoreUI();
		UpdateGoldCoinCountUI();
		StartPlatform.ActiveObstacle=true;
		StartCoroutine(StartLaunch(startGameLaunchTime));
		startPE1x=PE1Heap.position.x;
		startPE2x=PE2Heap.position.x;
		visionLenght*=CameraController.aspect_multiplier;
		nextRizlaValDistance.x=UnityEngine.Random.Range(10,36);
		nextRizlaValDistance.y=nextRizlaValDistance.x/rizlaHealOverDist.x*rizlaHealOverDist.y;
		nextJetPowerupLenght=stageStartLenghts[1]/2;
	}
	int nopo=0;
	void Update(){
		if(ObstaclePool.nopo>nopo) {
			nopo=ObstaclePool.nopo;
			//Debug.Log(nopo);
		}

		PE1Heap.position=new Vector3(pe1resistance*Camera.main.transform.position.x, PE1Heap.position.y, PE1Heap.position.z);
		PE2Heap.position=new Vector3(pe2resistance*Camera.main.transform.position.x, PE2Heap.position.y, PE2Heap.position.z);

		if(cig.transform.position.x+visionLenght > nextGroundLenght) BuildGround();
		if(cig.transform.position.x+visionLenght > nextObstLenght) BuildObstacle();
		if(cig.transform.position.x+visionLenght > nextPE1Lenght+PE1Heap.position.x-startPE1x) BuildPE1();
		if(cig.transform.position.x+visionLenght > nextPE2Lenght+PE2Heap.position.x-startPE2x) BuildPE2();

		if(cig.transform.position.x+visionLenght > nextCoinLenght){
			int cc=ChooseCoin();
			if(cc!=0) BuildPickup(cc, nextCoinLenght-1f);
			else nextCoinLenght+=15f;
		}
		if(cig.transform.position.x+visionLenght > nextRizlaValDistance.y){
			if(UnityEngine.Random.value<=rizlaHealProbability.Evaluate(cig.LenghtTraveled/criticalLenght))
				BuildPickup(3, nextRizlaValDistance.y);
			else nextRizlaValDistance.y+=10f;
		}
		if(cig.transform.position.x+visionLenght > nextJetPowerupLenght){
			if(UnityEngine.Random.value<=jetPowerupProbability.Evaluate((cig.LenghtTraveled-stageStartLenghts[1]/2)/criticalLenght))
				BuildPickup(4, nextJetPowerupLenght);
			else nextJetPowerupLenght+=15f;
		}
	}
	void OnDisable(){
		liveDataStorage.playSounds=GameMenu.s_inst.playSounds.isOn;
		Save();
	}
	void OnApplicationPause(){
		liveDataStorage.forceQuit=true;
		Save();
	}
	void SettingRightCigarette(){
		GameObject tob=Instantiate(cigs[liveDataStorage.playingClass]);
		tob.transform.position=new Vector2(cigspawn.x, cigspawn.y);
		cig=tob.GetComponentInChildren<Tobacco>();
		Camera.main.GetComponent<CameraController>().Target=cig.transform;
		Rain.s_inst.T_embers=cig.embers.transform;
	}
	public void UniButtonRotation(float side){cig.buttonRotation(side);}
	public void UniButtonStopRotation(){cig.buttonStopRotation();}
	public void UniButtonPowerFillingLaunch(){cig.buttonPowerFillingLaunch();}
	public void UniButtonLaunch(){cig.buttonLaunch();}

	IEnumerator StartLaunch(float launchIn){
		yield return new WaitForSeconds(launchIn);
		cig.StartGameCigarLaunch();
		GameMenu.s_inst.blockControlls.SetActive(false);
	}
	int ChooseCoin(){
		if(UnityEngine.Random.value <= silverCoinProbability.Evaluate(cig.LenghtTraveled/criticalLenght)) return 1;
		else if(UnityEngine.Random.value <= goldCoinProbability.Evaluate(cig.LenghtTraveled/criticalLenght)) return 2;
		else return 0;
	}
	void BuildPickup(int type, float xToBuild){
		RaycastHit2D rh = Physics2D.Raycast(new Vector2(xToBuild,groundHeight+13f), Vector2.down, Mathf.Infinity, layerMaskForPickups);
		Pickup pickup=null;

		if(type==1) pickup=silverCoinPool.RequestFreePickup(pickupsHeap);
		else if(type==2) pickup=goldCoinPool.RequestFreePickup(pickupsHeap);
		else if(type==3) pickup=rizlaHealPool.RequestFreePickup(pickupsHeap);
		else if(type==4) pickup=jetPowerupPool.RequestFreePickup(pickupsHeap);
			
		if(rh!=null && pickup!=null){
			pickup.transform.position=new Vector3(rh.point.x, rh.point.y+UnityEngine.Random.Range(1f,2.2f));
			pickup.PlayersTransform=cig.transform;
			if(type==1 || type==2) nextCoinLenght+=2f;
			else if(type==3){
				RizlaHealPowerup r=(RizlaHealPowerup)pickup;
				r.healFor=(int)nextRizlaValDistance.x;
				nextRizlaValDistance.x=UnityEngine.Random.Range(10,35);
				nextRizlaValDistance.y+=nextRizlaValDistance.x/rizlaHealOverDist.x*rizlaHealOverDist.y;
			}
			else if(type==4){
				nextJetPowerupLenght+=UnityEngine.Random.Range(300f, 470f);
			}
			pickup.ActivePickup=true;
		}else{
			if(type==1 || type==2) nextCoinLenght+=10f;
			else if(type==3) nextRizlaValDistance.y+=5f;
			else if(type==4) nextJetPowerupLenght+=7f;
		}
	}
	void BuildGround(){
		Obstacle o=groundPool.RequestFreeObstacle(ObstacleHeap);
		if(o==null){nextGroundLenght+=2f; return;}
		o.transform.position=new Vector3(nextGroundLenght, groundHeight+o._yHeightOffGround);
		nextGroundLenght+=o._width;
		o.ActiveObstacle=true;
	}
	void BuildObstacle(){
		int stage=0;
		for(int i=0; i<stageObsts.Length; i++)
			if(cig.LenghtTraveled >= stageStartLenghts[i]) stage=i; else break;
		Obstacle o=obstaclesPools[UnityEngine.Random.Range(0, stageObsts[stage])].RequestFreeObstacle(ObstacleHeap);
		if(o==null){nextObstLenght+=2f; return;}

		o.transform.position=new Vector3(nextObstLenght, groundHeight+o._yHeightOffGround);
		if(o.yMoveAble){
			o.MoveY(obstacleHeights.Evaluate((cig.LenghtTraveled-stageStartLenghts[stage])/criticalLenght));
		}
		nextObstLenght+=o._width+obstacleDistances.Evaluate(cig.LenghtTraveled/criticalLenght)*5f+UnityEngine.Random.value;
		o.ActiveObstacle=true;
	}
	void BuildPE1(){
		Obstacle o=pe1Pools[UnityEngine.Random.Range(0, pe1Pools.Length)].RequestFreeObstacle(PE1Heap);
		if(o==null){nextPE1Lenght+=2f; return;}
		o.transform.position=new Vector3(nextPE1Lenght+PE1Heap.position.x-startPE1x, groundHeight+o._yHeightOffGround);
		nextPE1Lenght+=o._width+UnityEngine.Random.value*6f;
		o.ActiveObstacle=true;
	}
	void BuildPE2(){
		Obstacle o=pe2Pools[UnityEngine.Random.Range(0, pe2Pools.Length)].RequestFreeObstacle(PE2Heap);
		if(o==null){nextPE2Lenght+=2f; return;}
		o.transform.position=new Vector3(nextPE2Lenght+PE2Heap.position.x-startPE2x, groundHeight+o._yHeightOffGround);
		nextPE2Lenght+=o._width+UnityEngine.Random.value*4f;
		o.ActiveObstacle=true;
	}
	public void RestartGame(){
		StartPlatform.ActiveObstacle=true;
		groundPool.DeactivateAllAlive();
		goldCoinPool.DeactivateAllAlive();
		silverCoinPool.DeactivateAllAlive();
		rizlaHealPool.DeactivateAllAlive();
		for(int i=0; i<obstaclesPools.Length; i++) obstaclesPools[i].DeactivateAllAlive();
		for(int i=0; i<pe1Pools.Length; i++) pe1Pools[i].DeactivateAllAlive();
		for(int i=0; i<pe2Pools.Length; i++) pe2Pools[i].DeactivateAllAlive();

		nextPE1Lenght=0f; nextPE2Lenght=0f;
		nextObstLenght=35f; nextGroundLenght=35f;
		nextCoinLenght=40f;
		nextRizlaValDistance.x=UnityEngine.Random.Range(10,35);
		nextRizlaValDistance.y=nextRizlaValDistance.x/rizlaHealOverDist.x*rizlaHealOverDist.y;
		nextJetPowerupLenght=stageStartLenghts[1]/2;
		PE1Heap.position=Vector3.zero;
		PE2Heap.position=Vector3.zero;

		Rain.s_inst.RestartRain();
		cig.ReviveOrRestart(true);
		Camera.main.transform.position=new Vector3(15f, 3.7f, -10f); //HARDCODED

		UpdateHighScoreUI();
		UpdateGoldCoinCountUI();
		GameMenu.s_inst.blockControlls.SetActive(true);
		StopAllCoroutines(); //##
		StartCoroutine(StartLaunch(startGameLaunchTime));
	}
	void Save(){
		BinaryFormatter bf=new BinaryFormatter();
		FileStream file=File.Open(Application.persistentDataPath+"/dataStorage1.dat", FileMode.OpenOrCreate);
		bf.Serialize(file, liveDataStorage);
		file.Close();
	}
	DataStorage1 Load(){
		//if(File.Exists(Application.persistentDataPath+"/dataStorage1.dat")){
			BinaryFormatter bf=new BinaryFormatter();
			FileStream file=File.Open(Application.persistentDataPath+"/dataStorage1.dat", FileMode.Open);
			DataStorage1 dataStorage=(DataStorage1)bf.Deserialize(file);
			file.Close();
			return dataStorage;
		//}else return new DataStorage1(); 
	}
	public void DisableEnableTutorial(){
		liveDataStorage.runTutorial=!liveDataStorage.runTutorial;
	}
	public DataStorage1 LiveDataStorage{
		get{return liveDataStorage;}
	}
	public Tobacco Cig{
		get{return cig;}
	}
	public void UpdateHighScoreUI(){
		highScore.text=(liveDataStorage.highScore/100f).ToString();
	}
	public void UpdateGoldCoinCountUI(){
		goldCoinCount.text=liveDataStorage.goldCoinCount.ToString();
	}
}

[Serializable]
public class ObstaclePool {
	public static int nopo=0; //numberofpooledobstacles
	public Obstacle obstaclePrefab;
	List<Obstacle> pooledObstacles=new List<Obstacle>();
	public int MAX=3;

	public Obstacle RequestFreeObstacle(Transform t){
		if(pooledObstacles.Count!=0){
			foreach(Obstacle o in pooledObstacles)
				if(!o.ActiveObstacle) return o;
		}else{
			pooledObstacles.Add(MonoBehaviour.Instantiate(obstaclePrefab, t)); nopo++;
			return pooledObstacles[0];
		}
		if(pooledObstacles.Count==MAX) return null;
		pooledObstacles.Add(MonoBehaviour.Instantiate(obstaclePrefab, t)); nopo++;
		return pooledObstacles[pooledObstacles.Count-1];
	}

	public void DeactivateAllAlive(){
		for(int i=0; i<pooledObstacles.Count; i++)
			pooledObstacles[i].ActiveObstacle=false;
	}
}

[Serializable]
public class PickupPool {
	public Pickup pickupPrefab;
	List<Pickup> pooledPickups=new List<Pickup>();
	public int MAX=4;

	public Pickup RequestFreePickup(Transform t){
		if(pooledPickups.Count!=0){
			foreach(Pickup o in pooledPickups)
				if(!o.ActivePickup) return o;
		}else{
			pooledPickups.Add(MonoBehaviour.Instantiate(pickupPrefab, t));
			return pooledPickups[0];
		}
		if(pooledPickups.Count==MAX) return null;
		pooledPickups.Add(MonoBehaviour.Instantiate(pickupPrefab, t));
		return pooledPickups[pooledPickups.Count-1];
	}

	public void DeactivateAllAlive(){
		for(int i=0; i<pooledPickups.Count; i++)
			pooledPickups[i].ActivePickup=false;
	}
}

[Serializable]
public class ProjectilePool {
	public Projectile projectilePrefab;
	List<Projectile> pooledProjectiles=new List<Projectile>();
	public int MAX=3;

	public Projectile RequestFreeProjectile(Transform t){
		if(pooledProjectiles.Count!=0){
			foreach(Projectile p in pooledProjectiles)
				if(!p.ActiveProjectile) return p;
		}else{
			pooledProjectiles.Add(MonoBehaviour.Instantiate(projectilePrefab, t));
			return pooledProjectiles[0];
		}
		if(pooledProjectiles.Count==MAX) return null;
		pooledProjectiles.Add(MonoBehaviour.Instantiate(projectilePrefab, t));
		return pooledProjectiles[pooledProjectiles.Count-1];
	}

	public void DeactivateAllAlive(){
		for(int i=0; i<pooledProjectiles.Count; i++)
			pooledProjectiles[i].ActiveProjectile=false;
	}
}

public enum ControllType
{
	FixedArrows, Joystick
}

[Serializable]
public class DataStorage1{
	public string gameVersion;
	public bool runTutorial=true;
	public bool playSounds=true;
	public ControllType controllType=ControllType.Joystick;

	List<List<bool>> contentUnlockState=new List<List<bool>>();
	public int playingClass=0;
	public int playingSkin=0;

	public int goldCoinCount=0;
	public float highScore=0f;
	public bool positiveRating=false;
	public bool forceQuit=false;
	public int nGamesRunned=0;

	public DataStorage1(string ver, int[] nClassesSkins){
		gameVersion=ver;
		for(int i=0; i<nClassesSkins.Length; i++){
			contentUnlockState.Add(new List<bool>());
			for(int j=0; j<nClassesSkins[i]; j++){
				if(i==0 && j==0) contentUnlockState[i].Add(true);
				else contentUnlockState[i].Add(false);
			}
		}
	}
	public void NewGameUpdate(string ver, int[] nClassesSkins){
		gameVersion=ver; //updateVerzije

		int x=contentUnlockState.Count-nClassesSkins.Length;
		while(x!=0){ //updateanje klasa
			if(x<0) contentUnlockState.Add(new List<bool>());
			else if(x>0) contentUnlockState.RemoveAt(contentUnlockState.Count-1);
			x=contentUnlockState.Count-nClassesSkins.Length;
		}

		for(int i=0; i<nClassesSkins.Length; i++){ //updateanje skinova
			x=contentUnlockState[i].Count-nClassesSkins[i];
			while(x!=0){
				if(x<0) contentUnlockState[i].Add(false);
				else if(x>0) contentUnlockState[i].RemoveAt(contentUnlockState[i].Count-1);
				x=contentUnlockState[i].Count-nClassesSkins[i];
			}
		}
	}
	public List<List<bool>>  ContentUnlockState{
		get{return contentUnlockState;}
	}
}