using UnityEngine;

namespace CardboardVRProto
{
	/// <summary>
	/// Class for handling track behaviour
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class TrackHandler : MonoBehaviour
	{
		private void OnTriggerEnter(Collider bumpCollider)
		{
			var player = bumpCollider.gameObject.GetComponent<PlayerHandler>();
			if (player == null) return;

			player.EnableMovement();
		}
	}
}
