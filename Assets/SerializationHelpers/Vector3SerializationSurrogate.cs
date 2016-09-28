using System.Runtime.Serialization;
using UnityEngine;

namespace Unity.Serialization {

	/// <summary>
	/// When using a BinaryFormatter (to serialize an object into a binary string),
	/// add this surrogate to the SurrogateSelector to allow custom serialization for
	/// Vector3 types (as it has no default serialization capabilities). 
	/// 
	/// This is an alternative from using serializable objects.
	/// 
	/// http://answers.unity3d.com/questions/956047/serialize-quaternion-or-vector3.html
	/// </summary>
	sealed class Vector3SerializationSurrogate : ISerializationSurrogate {

		// Method called to serialize a Vector3 object
		public void GetObjectData(System.Object obj,
			SerializationInfo info, StreamingContext context) {

			Vector3 v3 = (Vector3) obj;
			info.AddValue("x", v3.x);
			info.AddValue("y", v3.y);
			info.AddValue("z", v3.z);
			Debug.Log(v3);
		}

		// Method called to deserialize a Vector3 object
		public System.Object SetObjectData(System.Object obj,
			SerializationInfo info, StreamingContext context,
			ISurrogateSelector selector) {

			Vector3 v3 = (Vector3) obj;
			v3.x = (float)info.GetValue("x", typeof(float));
			v3.y = (float)info.GetValue("y", typeof(float));
			v3.z = (float)info.GetValue("z", typeof(float));
			obj = v3;
			return obj;   // Formatters ignore this return value //Seems to have been fixed!
		}
	}

}