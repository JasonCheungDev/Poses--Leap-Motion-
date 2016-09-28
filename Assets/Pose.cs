using UnityEngine;
using System.Collections;

using Leap.Unity;

/* class used to save and read pose data */
[System.Serializable]
public class Pose {

	public float[] distances;				// Distance to each joint, specified by GetComponents<Transform>()
	public float scale;		    			// LocalScale of the saved object (used for distance amplification) 
	public bool directionMatters = false;	// If the orientation (rotation) of the hand matters. 
	public bool distanceMatters  = false; 	// If the location of the hand (distance to the camera) matters. 
	public bool chiralityMatters = false; 	// If the chirality of the hand matters
	public Chirality chirality; 			// Which hand (l/r) this pose was made for 
	// isTwoHandedPose 

	public Pose() {}

	public Pose(float[] distances, Chirality whichHand, float scale = 1) {
		this.distances = distances;
		this.chirality = whichHand;
		this.scale = scale;
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
