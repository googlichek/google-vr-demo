using UnityEngine;

namespace CardboardVRProto
{
	/// <summary>
	/// Class for handling obstacle behaviour
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class ObstacleHandler : MonoBehaviour
	{
		private const string PlayerTag = GlobalVariables.PlayerTag;

		private void OnTriggerEnter(Collider bumpCollider)
		{
			if (!bumpCollider.gameObject.CompareTag(PlayerTag)) return;

			var gameHandler = FindObjectOfType<GameHandler>();
			if (gameHandler != null) gameHandler.Restart();
		}
	}
}
