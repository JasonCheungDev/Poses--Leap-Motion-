using UnityEngine;
using System.Collections;

using Leap.Unity;

/* class used to save and read pose data */
[System.Serializable]
public class Pose {

	public Vector3[] pointPosition;			// Position of joints relative to the root (hand)
	public float scale;		    			// LocalScale of the saved object (used for distance amplification) 
	public bool directionMatters = false;	// If the orientation (rotation) of the hand matters. 
	public Quaternion direction; 			// Direction (rotation) of the hand. 
	public bool positionMatters  = false; 	// If the location of the hand (distance to the root) matters. 
	public Vector3 position;				// Location of the hand 
	public bool chiralityMatters = false; 	// If the chirality of the hand matters
	public Chirality chirality; 			// Which hand (l/r) this pose was made for 
	// isTwoHandedPose 

	public Pose() {}

	public Pose(Vector3[] positions, Chirality whichHand, float scale = 1) {
		this.pointPosition = positions;
		this.chirality = whichHand;
		this.scale = scale;
	}

	public string Print() {
		string s = "";
		foreach (Vector3 f in pointPosition) {
			s += f + ",";
		}
		s += directionMatters;
		s += scale;
		return s;
	}

}
