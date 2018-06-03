using UnityEngine;

namespace CardboardVRProto
{
	/// <summary>
	/// Handles player movement & input.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class PlayerHandler : MonoBehaviour
	{
		private const string Horizontal = GlobalVariables.Horizontal;

		private const int LockAngle = 360;
		private const int LayerMask = 1 << (int) Layers.Wall;

		private const float RayDistance = 4;

		[Header("Tweakable Variables")]
		[SerializeField] [Range(0, 180)] private int _minAngle = 20;
		[SerializeField] [Range(0, 180)] private int _maxAngle = 90;
		[SerializeField] [Range(0.01f, 1)] private float _forwardSpeedMultiplier = 0.01f;
		[SerializeField] [Range(0, 100)] private int _sideSpeedMultiplier = 10;
		[SerializeField] [Range(0, 1000)] private int _maximumSpeed = 500;

		private Camera _mainCamera = null;
		private SceneLoadingHandler _sceneLoadingHandler = null;

		private float _speed = 0;

		private bool _isMovementEnabled = false;

		void Start()
		{
			_mainCamera = Camera.main;
			_sceneLoadingHandler = FindObjectOfType<SceneLoadingHandler>();
			_sceneLoadingHandler.SceneStartEvent += EnableMovement;
		}

		void FixedUpdate()
		{
			if (!_isMovementEnabled) return;

			SpeedUp();

#if UNITY_ANDROID
			var sideVector = GetDirectionVector();
			MoveWithHead(sideVector);
#endif

#if UNITY_EDITOR
			MoveWithKeyboardInput();
#endif
		}

		private void EnableMovement()
		{
			_isMovementEnabled = true;
		}

		private Vector3 GetDirectionVector()
		{
			var directionVector = Vector3.zero;

			var delta = (_mainCamera.transform.localRotation.eulerAngles.z) % LockAngle;

			if (delta > _minAngle && delta < _maxAngle)
			{
				directionVector = Vector3.left;
			}

			if (delta > LockAngle - _maxAngle && delta < LockAngle - _minAngle)
			{
				directionVector = Vector3.right;
			}

			return directionVector;
		}

		private void MoveWithHead(Vector3 directionVector)
		{
			if (directionVector == Vector3.zero) return;

			if (CheckIfMovementIsPossible(directionVector))
				transform.Translate(directionVector * _sideSpeedMultiplier * Time.deltaTime);
		}

		private void MoveWithKeyboardInput()
		{
			var delta = Input.GetAxis(Horizontal);
			if (CheckIfMovementIsPossible((delta * transform.right).normalized))
				transform.Translate(delta * transform.right * _sideSpeedMultiplier * Time.deltaTime);
		}

		private void SpeedUp()
		{
			transform.Translate(transform.forward * Time.deltaTime * _speed);

			if (!(_speed < _maximumSpeed)) return;
			_speed += _forwardSpeedMultiplier;
		}

		private bool CheckIfMovementIsPossible(Vector3 directionVector)
		{
			Vector3 rayOrigin = transform.position;

			RaycastHit hitTarget;
			Ray ray = new Ray(rayOrigin, directionVector);

			bool isHit =
				Physics.Raycast(ray, out hitTarget, RayDistance, LayerMask);
			if (isHit && hitTarget.distance <= RayDistance) return false;

			return true;
		}
	}
}
