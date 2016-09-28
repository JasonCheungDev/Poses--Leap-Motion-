using UnityEngine;
using UnityEngine.UI;

using Unity.Serialization;

using Leap.Unity;

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

	// Use this for initialization
	void Start () {
		parts = GetComponentsInChildren<Transform> ();	// Note: first one is always this 
	}
	
	// Update is called once per frame
	void Update () {
		matchDisplay.isOn = comparePose (target);
	}

	public bool comparePose(Pose p) {

		for (int i = 0; i < parts.Length; ++i) {
			float currentDistance = Vector3.Distance (parts [0].position, parts [i].position);
			float difference = Mathf.Abs (currentDistance - p.distances [i]);
			Debug.Log (difference);
			if (difference > poseBuffer) {
				// no match 
				return false;
			}
		}	

		// all matched 
		return true;
	}

	public void save() {
		// Create pose object 
		float[] distances = new float[parts.Length];
		for (int i = 0; i < parts.Length; ++i) {
			distances [i] = Vector3.Distance (parts [0].position, parts [i].position);
		}
		Vector3 displacement = parts [0].position - parts [3].position;
		Debug.Log ("displacement " + displacement);

		Pose p = new Pose (distances, GetComponent<IHandModel>().Handedness, transform.lossyScale.magnitude);

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

		file.Close ();

		// --- Change display on GUI ---
		matchDisplay.GetComponentInChildren<Text> ().text = "MATCHING \"" + inputField.text + "\"";

	}

	public void create() {

	}
}
