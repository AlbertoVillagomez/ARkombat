﻿using UnityEngine;
using System.Collections;

public class movieSon : MonoBehaviour {

	//private MovieTexture son_fatality;
	private AudioClip son;

	private AudioSource source;

	// Use this for initialization
	void Start () {
		#if UNITY_ANDROID
		Handheld.PlayFullScreenMovie("");
		#else
		MovieTexture stuff;
		#endif

	}

	//public static void makeFatality(MovieTexture m, bool mute){
	//	if (mute)
	//		m.Stop ();
	//	m.Play ();
	//}
}