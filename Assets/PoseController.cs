using UnityEngine;
using UnityEngine.UI;

using Unity.Serialization;

using Leap.Unity;

using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

/* A script used to save, load, and create poses */
public class PoseController : MonoBehaviour {

	public float poseBuffer = 0.02f;
	public Toggle matchDisplay;
	public InputField inputField;

	private Transform[] parts;	// transforms of all child parts + this transform
	private Pose target;

	private Chirality Handiness { 
		get {
			return GetComponent<IHandModel> ().Handedness;
		}
	}

	// Use this for initialization
	void Start () {
		parts = GetComponentsInChildren<Transform> ();	// Note: first one is always this 
	}
	
	// Update is called once per frame
	void Update () {
		matchDisplay.isOn = comparePose (target);
	}

	public bool comparePose(Pose p) {
		/*
		for (int i = 0; i < parts.Length; ++i) {
			float currentDistance = Vector3.Distance (parts [0].position, parts [i].position);
			float difference = Mathf.Abs (currentDistance - p.distances [i]);
			Debug.Log (difference);
			if (difference > poseBuffer) {
				// no match 
				return false;
			}
		}
		*/

		// all matched 
		return true;
	}

	public void save() {
		/*
		// Create pose object 
		float[] distances = new float[parts.Length];
		for (int i = 0; i < parts.Length; ++i) {
			distances [i] = Vector3.Distance (parts [0].position, parts [i].position);
		}
		Vector3 displacement = parts [0].position - parts [3].position;
		Debug.Log ("displacement " + displacement);
		*/

		Vector3[] positions = new Vector3[parts.Length];
		for (int i = 0; i < parts.Length; i++) {
			positions [i] = parts [i].position;
		}

		Pose p = new Pose (positions, Handiness, transform.lossyScale.magnitude);

		Debug.Log (transform.lossyScale);			// 10
		Debug.Log (transform.lossyScale.magnitude);	// 17~
		Debug.Log (p.Print());

		/*
		// Serialize object 
		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer (p.GetType ());

		// Write to file system 
		string fileName = inputField.text + ".pose";
		if (inputField.text.Length == 0) {
			Debug.Log ("file name cannot be blank");
			return;
		}
		if (File.Exists (fileName)) {
			Debug.Log (fileName + " already exists, aborting");
			return; 
		}

		using (StreamWriter sw = new StreamWriter (fileName)) { // using keyword closes the stream writer 
			// Writing data 
			serializer.Serialize(sw, p);
			sw.WriteLine();
		}

		Debug.Log ("Saved pose!");
		*/
		///

		// Write to file system 
		string fileName = inputField.text + ".pose";

		// --- file name validation ---
		if (inputField.text.Length == 0) 
		{
			Debug.Log ("file name cannot be blank");
			return;
		}
		if (File.Exists (fileName)) 
		{
			Debug.Log (fileName + " already exists, aborting");
			return; 
		}

		// --- Create serialization formatter ---
		BinaryFormatter bf = new BinaryFormatter ();

		SurrogateSelector ss = new SurrogateSelector ();
		// Make the unserializable Vector3 type serializable 
		ss.AddSurrogate (
			typeof(Vector3),
			new StreamingContext (StreamingContextStates.All),
			new Vector3SerializationSurrogate ()
		);
		// Make the unserializable Quaterion type serializable 
		ss.AddSurrogate (
			typeof(Quaternion),
			new StreamingContext (StreamingContextStates.All),
			new QuaternionSerializationSurrogate()
		);

		bf.SurrogateSelector = ss; 

		// --- Serialize and write to file ---
		FileStream file = File.Open (Application.persistentDataPath + "/" + fileName, FileMode.Create);
		bf.Serialize (file, p);
		file.Close ();

	}

	public void load() {
		// --- Validate and find in file system ---
		string fileName = inputField.text + ".pose";

		if (inputField.text.Length == 0)
		{
			Debug.Log ("file name cannot be blank");
			return;
		}
		if (!File.Exists (fileName)) 
		{
			Debug.Log (fileName + " doesn't exist, aborting");
			return; 
		}
						
		/*
		Pose p = new Pose ();

		XmlSerializer serializer = new XmlSerializer(p.GetType());

		using (StreamReader sr = new StreamReader(fileName)) { // using keyword closes the stream writer 
			p = (Pose)(serializer.Deserialize (sr));
		}	

		Debug.Log (p.Print ());

		target = p;

		Debug.Log ("Loaded pose!");
		*/

		///

		// --- Create deserialization formatter ---
		BinaryFormatter bf = new BinaryFormatter ();

		SurrogateSelector ss = new SurrogateSelector ();
		// Make the unserializable Vector3 type serializable 
		ss.AddSurrogate (
			typeof(Vector3),
			new StreamingContext (StreamingContextStates.All),
			new Vector3SerializationSurrogate ()
		);
		// Make the unserializable Quaterion type serializable 
		ss.AddSurrogate (
			typeof(Quaternion),
			new StreamingContext (StreamingContextStates.All),
			new QuaternionSerializationSurrogate()
		);

		bf.SurrogateSelector = ss; 

		// --- Deserialize file --- 
		FileStream file = File.Open (Application.persistentDataPath + "/" + fileName, FileMode.Open);

		Pose p = (Pose)bf.Deserialize(file);
		target = p;
		ReformatPose (target);

		file.Close ();

		// --- Change display on GUI ---
		matchDisplay.GetComponentInChildren<Text> ().text = "MATCHING \"" + inputField.text + "\"";

	}

	#region Matching logic 

	// Rename function, but does what it should do 
	private void ReformatPose(Pose pose) {

		// Only reformat if necessary 
		if (pose.chiralityMatters) {

			// Check if reformatting is needed 
			if (Handiness != target.chirality) {

				// Flip all coordinates (so left becomes right, right becomes left) 
				for (int i = 0; i < pose.pointPosition.Length; i++) {
					pose.pointPosition[i] = Vector3.Reflect (pose.pointPosition[i], Vector3.up);
				}
			}
		}
			
	}

	private bool poseIsMatching() {

		float maxDist = 1.0f; // maxDist * target.scale.magnitude (something like that) 
		float maxAngle = 5.0f; // 5 degrees 

		if (target.chiralityMatters) 
		{
			if (Handiness != target.chirality)
				return false; 
		}

		if (target.positionMatters) 
		{
			var dist = Vector3.Distance (
				           transform.parent.transform.position,	// LeapHandController (root) 
				           target.position
			           ); 
			if (dist > maxDist)
				return false;
		}

		if (target.directionMatters) 
		{
			if (Quaternion.Angle (transform.rotation, target.direction) > maxAngle)
				return false; 
		}
	
		for (int i = 0; i < parts.Length; i++) 
		{
			if ((parts [i].position.x - target.pointPosition [i].x) > maxDist
				|| (parts [i].position.y - target.pointPosition [i].y) > maxDist
				|| (parts [i].position.z - target.pointPosition [i].z) > maxDist) 
			{
				return false; 
			}
		}

		return true;
	}

	#endregion 
}
