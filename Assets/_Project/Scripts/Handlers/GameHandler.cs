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
		private const int Minute = 60;

		[Header("Track Creation Variables")]
		[SerializeField] private Transform _trackRoot = null;
		[SerializeField] private List<GameObject> _trackBlocksEasy = new List<GameObject>();
		[SerializeField] private List<GameObject> _trackBlocksMedium = new List<GameObject>();

		private readonly List<GameObject> _currentTrackBlocks = new List<GameObject>();

		private int _startTime = 0;

		void Start()
		{
			InputTracking.Recenter();

			SpawnTrackBlock(_trackBlocksEasy, 0, Vector3.zero);
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, BlockLength));
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, 2 * BlockLength));
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, 3 * BlockLength));

			_startTime = Mathf.RoundToInt(Time.time);
		}

		public void Restart()
		{
		}

		private void InitializeBlockSpawning(Vector3 lastBlockPosition)
		{
			var currentTime = Mathf.RoundToInt(Time.time);

			var spawnPosition = new Vector3(
				_trackRoot.position.x,
				_trackRoot.position.y,
				_trackRoot.position.z + lastBlockPosition.z + OffsetMultiplier * BlockLength);

			var timeDelta = currentTime - _startTime;
			HandleProgression(timeDelta, spawnPosition);
		}

		private void HandleProgression(int timeDelta, Vector3 spawnPosition)
		{
			if (timeDelta <= Minute)
			{
				HandleBlockSpawnProcess(_trackBlocksEasy, spawnPosition);
			}
			else if (timeDelta > Minute && timeDelta <= 2 * Minute)
			{
				HandleBlockSpawnProcess(_trackBlocksMedium, spawnPosition);
			}
			else
			{
				Restart();
			}
		}

		private void HandleBlockSpawnProcess(List<GameObject> blocks,Vector3 spawnPosition)
		{
			var index = GetBlockIndex(blocks);
			SpawnTrackBlock(blocks, index, spawnPosition);
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
				InitializeBlockSpawning;

			_currentTrackBlocks.Add(block);
		}
	}
}
