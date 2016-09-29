using UnityEngine;
using System.Collections;

// TODO: Supposed to be a concrete implementation of iPoseComparator, but not sure if this class is really needed. 
// Main reason I wanted a seperate object was for the DebugPoseComparator. 
public class PoseComparator {

	public static bool MatchPose(Transform hand, Pose pose) {
		return true;
	}

}
