using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager s_inst{get; private set;}
	AudioSource[] a2Dsrc;

	void Awake(){
		if(s_inst==null) s_inst=this;
		else{
			Destroy(s_inst);
			s_inst=this;
		}
		a2Dsrc=GetComponents<AudioSource>();
	}
	public void Play2DSound(int i){
		if(!AudioListener.pause) a2Dsrc[i].Play();
	}
	public void Stop2DSound(int i){
		a2Dsrc[i].Stop();
	}
	public bool PlaySounds{
		set{AudioListener.pause=!value;}
	}
}