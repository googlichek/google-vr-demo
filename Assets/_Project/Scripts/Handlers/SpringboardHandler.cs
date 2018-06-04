using UnityEngine;

namespace CardboardVRProto
{
	/// <summary>
	/// Class for handling springboard behaviour
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class SpringboardHandler : MonoBehaviour
	{
		private void OnTriggerEnter(Collider bumpCollider)
		{
			var player = bumpCollider.gameObject.GetComponent<PlayerHandler>();
			if (player == null) return;

			player.EnableRotation();
		}
	}
}
