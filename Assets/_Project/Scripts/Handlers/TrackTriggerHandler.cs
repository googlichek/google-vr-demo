using UnityEngine;

namespace CardboardVRProto
{
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
