using UnityEngine;

namespace CardboardVRProto
{
	/// <summary>
	/// Trigger for warning it's listeners that it detected trigger collision with player.
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class TrackTriggerHandler : MonoBehaviour
	{
		public delegate void OnTriggered(Vector3 currentBlockPosition);
		public event OnTriggered TriggeredEvent;

		private const string PlayerTag = GlobalVariables.PlayerTag;

		private void OnTriggerEnter(Collider bumpCollider)
		{
			if (!bumpCollider.gameObject.CompareTag(PlayerTag)) return;

			if (TriggeredEvent != null) TriggeredEvent(transform.parent.localPosition);
		}
	}
}
