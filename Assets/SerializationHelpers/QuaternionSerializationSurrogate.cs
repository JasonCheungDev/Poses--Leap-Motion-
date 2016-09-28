using System.Runtime.Serialization;
using UnityEngine;

namespace Unity.Serialization {
	
	/// <summary>
	/// When using a BinaryFormatter (to serialize an object into a binary string),
	/// add this surrogate to the SurrogateSelector to allow custom serialization for
	/// Quaternion types (as it has no default serialization capabilities). 
	/// 
	/// This is an alternative from using serializable objects.
	/// 
	/// http://answers.unity3d.com/questions/956047/serialize-quaternion-or-vector3.html
	/// </summary>
	sealed class QuaternionSerializationSurrogate : ISerializationSurrogate {

		// Method called to serialize a Vector3 object
		public void GetObjectData(System.Object obj,
			SerializationInfo info, StreamingContext context) {

			Quaternion q = (Quaternion) obj;
			info.AddValue("x", q.x);
			info.AddValue("y", q.y);
			info.AddValue("z", q.z);
			info.AddValue("w", q.w);
			Debug.Log(q);
		}

		// Method called to deserialize a Vector3 object
		public System.Object SetObjectData(System.Object obj,
			SerializationInfo info, StreamingContext context,
			ISurrogateSelector selector) {

			Quaternion q = (Quaternion) obj;
			q.x = (float)info.GetValue("x", typeof(float));
			q.y = (float)info.GetValue("y", typeof(float));
			q.z = (float)info.GetValue("z", typeof(float));
			q.w = (float)info.GetValue ("w", typeof(float));
			obj = q;
			return obj;   // Formatters ignore this return value //Seems to have been fixed!
		}
	}

}
