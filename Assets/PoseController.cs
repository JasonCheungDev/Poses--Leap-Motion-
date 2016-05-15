using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

/* A script used to save, load, and create poses */
public class PoseController : MonoBehaviour {

	public float poseBuffer = 0.02f;
	public Toggle matchDisplay;

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
		Pose p = new Pose (distances, transform.lossyScale.magnitude);
		Debug.Log (transform.lossyScale);
		Debug.Log (transform.lossyScale.magnitude);
		Debug.Log (p.Print());
		// Serialize object 
		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer (p.GetType ());

		// Write to file system 
		string fileName = "rock.pose";
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
	}

	public void load() {
		// Write to file system 
		string fileName = "rock.pose";
		if (!File.Exists (fileName)) {
			Debug.Log (fileName + " doesn't exist, aborting");
			return; 
		}
			
		Pose p = new Pose ();
		XmlSerializer serializer = new XmlSerializer(p.GetType());

		using (StreamReader sr = new StreamReader(fileName)) { // using keyword closes the stream writer 
			p = (Pose)(serializer.Deserialize (sr));
		}	

		Debug.Log (p.Print ());

		target = p;

		Debug.Log ("Loaded pose!");
	}

	public void create() {

	}
}
