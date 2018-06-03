using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace CardboardVRProto
{
	public class GameHandler : MonoBehaviour
	{
		private const int BlockLength = GlobalVariables.TrackBlockLength;
		private const int OffsetMultiplier = 4;

		[Header("Track Creation Variables")]
		[SerializeField] private Transform _trackRoot = null;
		[SerializeField] private List<GameObject> _trackBlocksEasy = new List<GameObject>();

		private List<GameObject> _currentTrackBlocks = new List<GameObject>();

		void Start()
		{
			InputTracking.Recenter();

			SpawnTrackBlock(_trackBlocksEasy, 0, Vector3.zero);
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, BlockLength));
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, 2 * BlockLength));
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, 3 * BlockLength));
		}

		public void Restart()
		{
			Debug.Log("Restart");
		}

		private void HandleTrackBlockSpawning(Vector3 lastBlockPosition)
		{
			var index = GetBlockIndex(_trackBlocksEasy);

			var spawnPosition = new Vector3(
				_trackRoot.position.x,
				_trackRoot.position.y,
				_trackRoot.position.z + lastBlockPosition.z + OffsetMultiplier * BlockLength);
			SpawnTrackBlock(_trackBlocksEasy, index, spawnPosition);

			CleanUpBlocks();
		}

		private void CleanUpBlocks()
		{
			var block = _currentTrackBlocks.FirstOrDefault();
			if (block == null) return;

			_currentTrackBlocks.Remove(block);
			Destroy(block);
		}

		private int GetBlockIndex(List<GameObject> blocks)
		{
			var index = Random.Range(0, blocks.Count - 1);
			return index;
		}

		private void SpawnTrackBlock(List<GameObject> blocks, int index, Vector3 position)
		{
			var block = Instantiate(blocks[index], _trackRoot);
			block.transform.localPosition = position;

			block.GetComponentInChildren<TrackTriggerHandler>().TriggeredEvent +=
				HandleTrackBlockSpawning;

			_currentTrackBlocks.Add(block);
		}
	}
}
