using UnityEngine;

namespace CardboardVRProto
{
	[RequireComponent(typeof(Collider))]
	public class ObstacleHandler : MonoBehaviour
	{
		private void OnTriggerEnter(Collider bumpCollider)
		{
			if (!bumpCollider.gameObject.CompareTag(GlobalVariables.PlayerTag)) return;

			var gameHandler = FindObjectOfType<GameHandler>();
			if (gameHandler != null) gameHandler.Restart();
		}
	}
}
