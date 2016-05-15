using UnityEngine;
using System.Collections;

/* class used to save and read pose data */
[System.Serializable]
public class Pose {

	public float[] distances;		// Distance to each joint, specified by GetComponents<Transform>()
	public bool directionMatters = false;	// Determines if the orientation of the pose matters, or if the pose can be seen/done in any direction.
	public Vector3 scale;		    // LocalScale of the saved object (used for distance amplification) 

	public Pose() {}

	public Pose(float[] distances, Vector3 scale = new Vector3()) {
		this.distances = distances;
		this.scale = scale;
	}

	public void Save() {
		if (distances.Length == 0) {
			Debug.Log ("Size of distances is 0, error!");
			return;
		}
	}

	public string Print() {
		string s = "";
		foreach (float f in distances) {
			s += f + ",";
		}
		s += directionMatters;
		s += scale;
		return s;
	}
}
