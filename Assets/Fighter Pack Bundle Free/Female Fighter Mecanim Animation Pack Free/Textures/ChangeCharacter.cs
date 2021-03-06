﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ChangeCharacter : MonoBehaviour {
	
	private List<AudioClip> list = new List<AudioClip> ();
	private AudioSource source;

	private int numberOfFights;
	private int fightsWon;
	private float timeLeft;
	private string effects, music;
	private string fatMovie;

	private bool fightEnd;
	private bool stopBackground;
	private bool executingFatality;
	private bool photoTaken;
	private bool achievementsTable;
	public bool showTextPhotoTakenFeedback;
	public static bool isGameStarted;
	public static bool stopEffects;
	public static bool fatal;

	public GameObject liuKang, scorpion, sonya, subzero;
	public GameObject fatLiu, fatSub, faScorp, fatSon;
	public GameObject p1, p2;

	public AudioClip liu_start;
	public AudioClip sonya_start;
	public AudioClip scorpion_start;
	public AudioClip subzero_start;
	public AudioClip liu_wins;
	public AudioClip sonya_wins;
	public AudioClip scorpion_wins;
	public AudioClip subzero_wins;
	public AudioClip fight;
	public AudioClip fatality;
	public AudioClip finish_him;
	public AudioClip finish_her;
	public AudioClip perfect;
	public AudioClip background;
	public AudioClip arena;
	public AudioClip liu_fat;
	public AudioClip sonya_fat;
	public AudioClip scorpion_fat;
	public AudioClip subzero_fat;

	public Canvas canvas;
	public Canvas canvas2;

	public static float hp1;
	public static float hp2;

	public static string name1, name2;
	public string photoName;

	void Awake () {
		source = GetComponent<AudioSource>();
		timeLeft = 90.0f;
		hp1 = 100.0f;
		hp2 = 100.0f;
		fatLiu.active = false;
		fatSub.active = false;
		faScorp.active = false;
		fatSon.active = false;
	}


	// Use this for initialization
	void Start () {
		liuKang = GameObject.Find ("LiuKang");
		scorpion = GameObject.Find ("Scorpion");
		sonya = GameObject.Find ("Sonya");
		subzero = GameObject.Find ("SubZero");
		scorpion.active = false;
		subzero.active = false;
		canvas.enabled = false;
		canvas2.enabled = false;
		list.Clear ();
		LoadStats ();
	}

	// Update is called once per frame
	void Update () {

		if(isGameStarted){
			timeLeft -= Time.deltaTime;
			if(timeLeft <= 0 || hp1 <= -50 || hp2 <= -50){
				fatal = true;
				isGameStarted = false;
				fightEnd = true;
				numberOfFights++;
				if (hp1 > -50) {
					fightsWon++;
				}
				SaveStats ();
			}
		}
		if (fightEnd) {
			AudioClip winner = liu_wins;
			bool man = true;
			float len = 0.0f;
			if (hp1 == hp2) {
				Random.seed = (int)System.DateTime.Now.Ticks;
				hp1 = Random.Range (0.0f, 100.0f);
				Random.seed = (int)System.DateTime.Now.Ticks;
				hp2 = Random.Range (0.0f, 100.0f);
			}
			if (hp1 > hp2) {
				if (p1.name == "LiuKang") {
					fatMovie = "liu";
					winner = liu_wins;
				} else {
					fatMovie = "scorpion";
					winner = scorpion_wins;
				}
			} else if (hp2 > hp1) {
				if (p2.name == "Sonya") {
					fatMovie = "sonya";
					winner = sonya_wins;
					man = false;
				} else {
					fatMovie = "subzero";
					winner = subzero_wins;
				}
			}
			source.clip = background;
			if (man) {
				if(!stopEffects) source.PlayOneShot(finish_him,10);
			} else {
				if(!stopEffects) source.PlayOneShot(finish_her,10);
			}
			list.Add(winner);
			if(hp1 == 100.0f || hp2 == 100.0f){
				list.Add (perfect);
			}
			fightEnd = false;
		}
	}

	IEnumerator Wait(AudioClip a){
		if(!stopEffects) source.PlayOneShot(a,10);
		yield return new WaitForSeconds (a.length);
	}
	IEnumerator Wait(AudioClip a, AudioClip b){
		if(!stopEffects) source.PlayOneShot(a,10);
		yield return new WaitForSeconds (a.length);
		if(!stopEffects) source.PlayOneShot(b,10);
		yield return new WaitForSeconds (b.length);
	}
	IEnumerator Wait(AudioClip a, AudioClip b, AudioClip c){
		if(!stopEffects) source.PlayOneShot(a,10);
		yield return new WaitForSeconds (a.length);
		if(!stopEffects) source.PlayOneShot(b,10);
		yield return new WaitForSeconds (b.length);
		if(!stopEffects) source.PlayOneShot(c,10);
		yield return new WaitForSeconds (c.length);
	}
	IEnumerator WaitFatality(AudioClip a, GameObject go){
		if(!stopBackground) source.PlayDelayed(a.length);
		yield return new WaitForSeconds (a.length);
		go.active = false;
		executingFatality = false;
	}
	private IEnumerator PlayStreamingVideo(string url){
        Handheld.PlayFullScreenMovie(url, Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
        yield return new WaitForEndOfFrame();
		executingFatality = false;
		if(!stopBackground) source.Play ();
     }
	private IEnumerator photoTakenFeedback(){
		yield return new WaitForSeconds (0.5f);
		showTextPhotoTakenFeedback = false;
	}

	float playSongs(AudioClip[] a){
		float len = 0.0f;
		if (a.Length == 1) {
			len += a [0].length;
			StartCoroutine (Wait(a[0]));
		} else if (a.Length == 2) {
			len += a [0].length;
			len += a [1].length;
			StartCoroutine (Wait(a[0], a[1]));
		} else if (a.Length == 3){
			len += a [0].length;
			len += a [1].length;
			len += a [2].length;
			StartCoroutine (Wait(a[0], a[1], a[2]));
		}
		return len;
	}

	void OnGUI () {
		int w = Screen.width;
		int h = Screen.height;

		if (fatal) {	//Fatality
			canvas2.enabled = true;
			GUIStyle custom = new GUIStyle ("box");
			custom.fontSize = 20;
			GUIStyle customB = new GUIStyle ("button");
			customB.fontSize = 30;
			GUI.Box (new Rect (w/2 - w/6, h/2 - h/6 +50, w / 3 + 10, h / 3), "", custom);
			if (GUI.Button (new Rect (w/2 - w/6 + 10, h/2 + 30, w / 6 -10, h / 9), "Yes", customB)) {
				if (fatMovie == "liu") {
					StartCoroutine (PlayStreamingVideo("liu_fatality.mp4"));
				}else if (fatMovie == "subzero") {
					StartCoroutine (PlayStreamingVideo("subzero_fatality.mp4"));
				}else if (fatMovie == "sonya") {
					StartCoroutine (PlayStreamingVideo("sonya_fatality.mp4"));
				}else if (fatMovie == "scorpion") {
					StartCoroutine (PlayStreamingVideo("scorpion_fatality.mp4"));
				}
				fatal = false;
				executingFatality = true;
				list.Clear ();
			}
			if (GUI.Button (new Rect (w/2 + 10, h/2 + 30, w / 6 -10, h / 9), "No", customB)) {
				AudioClip[] songs = list.ToArray();
				if(!stopBackground) source.PlayDelayed(playSongs (songs));
				fatal = false;
				list.Clear ();
			}

		}
		else if(executingFatality){}
		else if (!isGameStarted && !fatal) {
			canvas2.enabled = false;
			timeLeft = 90.0f;
			hp1 = 100.0f;
			hp2 = 100.0f;
			canvas.enabled = false;
			GUIStyle customBox = new GUIStyle ("box");
			customBox.fontSize = 20;
			GUIStyle customButton = new GUIStyle ("button");
			customButton.fontSize = 30;

			GUI.Box (new Rect (10, 10, w / 4, h / 4 + 70), "Player 1", customBox);
			GUI.Box (new Rect (w - w / 4 - 10, 10, w / 4, h / 4 + 70), "Player 2", customBox);
			GUI.Box (new Rect (10, w/3 - 50, w / 4, h / 4 + 70), "Settings", customBox);

			if (!stopBackground) {
				music = "Stop music";
			} else {
				music = "Play music";
			}

			if (!stopEffects) {
				effects = "Stop effects";
			} else {
				effects = "Play effects";
			}

			if (GUI.Button (new Rect (20, w/3 -20, w / 4 - 20, h / 8), music, customButton)) {
				stopBackground = !stopBackground;
				if (stopBackground) {
					source.Stop ();
				} else {
					source.Play ();
				}
			}
			if (GUI.Button (new Rect (20, w/3 + h / 8, w / 4 - 20, h / 8), effects, customButton)) {
				stopEffects = !stopEffects;
			}

			if (GUI.Button (new Rect (20, 40, w / 4 - 20, h / 8), "Liu Kang", customButton)) {
				if(!stopEffects) source.PlayOneShot(liu_start,10);
				liuKang.active = true;
				scorpion.active = false;
			}
			if (GUI.Button (new Rect (20, h / 8 + 50, w / 4 - 20, h / 8), "Scorpion", customButton)) {
				if(!stopEffects) source.PlayOneShot(scorpion_start,10);
				liuKang.active = false;
				scorpion.active = true;
			}
				
			if (GUI.Button (new Rect (w - w / 4, 40, w / 4 - 20, h / 8), "Sonya", customButton)) {
				if(!stopEffects) source.PlayOneShot(sonya_start,10);
				sonya.active = true;
				subzero.active = false;
			}
			if (GUI.Button (new Rect (w - w / 4, h / 8 + 50, w / 4 - 20, h / 8), "Sub-Zero", customButton)) {
				if(!stopEffects) source.PlayOneShot(subzero_start,10);
				sonya.active = false;
				subzero.active = true;
			}

			if (GUI.Button (new Rect (w - w/4 - 10, h - h / 8 - 20, w / 4, h / 8), "Statistics", customButton)) {
				achievementsTable = !achievementsTable;
			}
			if (achievementsTable) {
				GUIStyle customText = new GUIStyle ("button");
				customText.fontSize = 20;
				string text = "";
				text += "Number of fights: " + numberOfFights + "\n";
				text += "Number of wins: " + fightsWon + "\n";
				text += "Number of losts: "+ (numberOfFights - fightsWon) + "\n";
				GUI.Box (new Rect (w / 2 - w / 8, 10, w / 4, h / 4), text, customText);
			}
				
			if (GUI.Button (new Rect (w / 2 - w / 8, h / 2 - 20, w / 4, h / 8), "Fight", customButton)) {
				if (liuKang.activeSelf) {
					p1 = liuKang;
					name1 = "Liu Kang";
				} else {
					p1 = scorpion;
					name1 = "Scorpion";
				}
				if (sonya.activeSelf) {
					p2 = sonya;
					name2 = "Sonya";
				} else {
					p2 = subzero;
					name2 = "Sub-Zero";
				}

				isGameStarted = true;
				canvas.enabled = true;
				source.clip = arena;
				if(!stopEffects) source.PlayOneShot(fight,10);
				if(!stopBackground) source.PlayDelayed(fight.length);
			}
		}else {
			canvas2.enabled = false;
			GUIStyle customLabel = new GUIStyle ("label");
			customLabel.fontSize = 30;
			GUI.Label (new Rect (w / 2, 10+40, 200, 100), (int)timeLeft+"", customLabel);
			GUIStyle customButton = new GUIStyle ("button");
			customButton.fontSize = 30;
			if (GUI.Button (new Rect (w - w / 8 - 10, h / 4, w / 8, h / 8), "Photo", customButton)) {
				photoName = "ARKombat__"+System.DateTime.Now.ToFileTime ().ToString () + ".png";
				Application.CaptureScreenshot (photoName);
				photoTaken = true;
				showTextPhotoTakenFeedback = true;
			}

			if(showTextPhotoTakenFeedback){
				StartCoroutine (photoTakenFeedback ());
				GUIStyle c = new GUIStyle ("label");
				c.fontSize = 15;
				GUI.Label (new Rect (w/2, h/2, 500, 500), "Photo taken", c);
			}
			if (photoTaken) {
				string Origin_Path = System.IO.Path.Combine (Application.persistentDataPath, photoName);

				string Path = "/mnt/sdcard/DCIM/" + photoName;
				if (System.IO.File.Exists (Origin_Path)) {
					System.IO.File.Move (Origin_Path, Path);
				}
			}
		}
	}
		
	public void SaveStats(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath+"/playerInfo.dat");
		PlayerData data = new PlayerData ();
		data.fights = this.numberOfFights;
		data.wins = this.fightsWon;
		bf.Serialize (file, data);
		file.Close ();
	}

	public void LoadStats(){
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();
			numberOfFights = data.fights;
			fightsWon = data.wins;
		}
	}
}

[System.Serializable]
class PlayerData{
	public int fights;
	public int wins;
}
