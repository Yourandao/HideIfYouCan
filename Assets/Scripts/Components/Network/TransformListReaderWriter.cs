using System.Collections.Generic;

using Mirror;

using UnityEngine;

namespace Assets.Scripts.Components.Network
{
	public static class TransformListReaderWriter
	{
		public static IEnumerable<Transform> TransformListRead(this NetworkReader networkReader)
		{
			yield return networkReader.ReadTransform();
		}

		public static void TransformListWrite(this NetworkWriter networkWriter, IEnumerable<Transform> objects)
		{
			foreach (var gameObject in objects)
			{
				networkWriter.WriteTransform(gameObject);
			}
		}
	}
}
