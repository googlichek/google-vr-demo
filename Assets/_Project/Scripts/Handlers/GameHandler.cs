using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace CardboardVRProto
{
	/// <summary>
	/// Handles game flow.
	/// Listens to all events that obstacles send.
	/// </summary>
	public class GameHandler : MonoBehaviour
	{
		private const int BlockLength = GlobalVariables.TrackBlockLength;

		/// <summary>
		/// I'm creating 4 track blocks on start, so when new block is created
		/// it should be pushed forward by 4 length units.
		/// </summary>
		private const int OffsetMultiplier = 4;
		private const int Minute = 60;

		private const float BlockDestructionTimeout = 0.5f;

		private readonly List<GameObject> _currentTrackBlocks = new List<GameObject>();

		[Header("Track Creation Variables")]
		[SerializeField] private Transform _trackRoot = null;
		[SerializeField] private List<GameObject> _trackBlocksEasy = new List<GameObject>();
		[SerializeField] private List<GameObject> _trackBlocksMedium = new List<GameObject>();

		private int _startTime = 0;
		private int _timeDelta = 0;
		private SceneLoadingHandler _sceneLoadingHandler = null;

		void Start()
		{
			InputTracking.Recenter();

			_sceneLoadingHandler = FindObjectOfType<SceneLoadingHandler>();
			_sceneLoadingHandler.SceneStartEvent += StartGame;

			SpawnTrackBlock(_trackBlocksEasy, 0, Vector3.zero);
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, BlockLength));
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, 2 * BlockLength));
			SpawnTrackBlock(_trackBlocksEasy, 0, new Vector3(0, 0, 3 * BlockLength));
		}

		void Update()
		{
			// Restart game, if time limit of 2 minutes was reached.
			var currentTime = Mathf.RoundToInt(Time.time);
			_timeDelta = currentTime - _startTime;
			if (_timeDelta > 2 * Minute) Restart();
		}

		/// <summary>
		/// Restarts demo scene.
		/// </summary>
		public void Restart()
		{
			_sceneLoadingHandler.LoadScene();
		}

		private void StartGame()
		{
			_startTime = Mathf.RoundToInt(Time.time);
		}

		/// <summary>
		/// Initializes new track block creation, when player enters trigger.
		/// </summary>
		/// <param name="lastBlockPosition">Position of the block from which event was triggered.</param>
		private void InitializeBlockSpawning(Vector3 lastBlockPosition)
		{
			var spawnPosition = new Vector3(
				_trackRoot.position.x,
				_trackRoot.position.y,
				_trackRoot.position.z + lastBlockPosition.z + OffsetMultiplier * BlockLength);

			HandleProgression(spawnPosition);
		}

		/// <summary>
		/// Spawn blocks in accordance with the amount of passed time.
		/// </summary>
		private void HandleProgression(Vector3 spawnPosition)
		{
			if (_timeDelta <= Minute)
			{
				HandleBlockSpawnProcess(_trackBlocksEasy, spawnPosition);
			}
			else if (_timeDelta > Minute && _timeDelta <= 2 * Minute)
			{
				HandleBlockSpawnProcess(_trackBlocksMedium, spawnPosition);
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
			StartCoroutine(DestroyBlock(block));
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

		private IEnumerator DestroyBlock(GameObject block)
		{
			yield return new WaitForSeconds(BlockDestructionTimeout);
			Destroy(block);
		}
	}
}
