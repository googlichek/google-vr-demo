using DG.Tweening;
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
		[SerializeField] [Range(0, 180)] private int _jumpRotationThresholdAngle = 45;
		[SerializeField] [Range(0.01f, 100)] private float _forwardSpeedMultiplier = 0.01f;
		[SerializeField] [Range(0, 100)] private int _sideSpeedMultiplier = 10;
		[SerializeField] [Range(0, 1000)] private int _rotationSpeedMultiplier = 50;
		[SerializeField] [Range(0, 1000)] private int _maximumSpeed = 500;

		[Header("Tweening variables")]
		[SerializeField] [Range(0, 1)] private float _rotationCorrectionDuration = 0.5f;
		[SerializeField] private Ease _rotationCorrectionEase = Ease.Linear;

		private Camera _mainCamera = null;
		private Rigidbody _rigidbody = null;
		private SceneLoadingHandler _sceneLoadingHandler = null;

		private Quaternion _defaultRotation = Quaternion.identity;

		private bool _isMovementEnabled = false;
		private bool _isRotationEnabled = false;

		private float _speed = 0;

		void Start()
		{
			_mainCamera = Camera.main;
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.useGravity = false;

			_sceneLoadingHandler = FindObjectOfType<SceneLoadingHandler>();
			_sceneLoadingHandler.SceneStartEvent += EnableGravity;

			_defaultRotation = transform.rotation;
		}

		void FixedUpdate()
		{
			if (_isMovementEnabled)
			{
				SpeedUp();

#if UNITY_ANDROID
				var sideVector = GetDirectionVector();
				MoveWithHead(sideVector);
#endif

#if UNITY_EDITOR
				MoveWithKeyboardInput();
#endif
			}

			if (_isRotationEnabled)
			{
#if UNITY_ANDROID
				var sideVector = GetDirectionVector();
				RotateWithHead(sideVector);
#endif

#if UNITY_EDITOR
				RotateWithKeyboardInput();
#endif
			}
		}

		public void EnableMovement()
		{
			_isMovementEnabled = true;
			_isRotationEnabled = false;

			CheckRotation();
		}

		public void EnableRotation()
		{
			_isMovementEnabled = false;
			_isRotationEnabled = true;
		}

		private void EnableGravity()
		{
			_rigidbody.useGravity = true;
		}

		private void CheckRotation()
		{
			var delta = transform.localRotation.eulerAngles.y % LockAngle;

			if (delta >= LockAngle - _jumpRotationThresholdAngle && delta <= LockAngle ||
			    delta >= 0 && delta <= _jumpRotationThresholdAngle)
			{
				Vector3 rotationVector = _defaultRotation.eulerAngles;
				transform
					.DORotate(rotationVector, _rotationCorrectionDuration)
					.SetEase(_rotationCorrectionEase);
				return;
			}

			FindObjectOfType<GameHandler>().Restart();
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

		private void RotateWithHead(Vector3 directionVector)
		{
			if (directionVector == Vector3.zero) return;

			if (directionVector == Vector3.right)
			{
				transform.Rotate(Vector3.up * _rotationSpeedMultiplier * Time.deltaTime);
			}
			else if (directionVector == Vector3.left)
			{
				transform.Rotate(Vector3.down * _rotationSpeedMultiplier * Time.deltaTime);
			}
		}

		private void MoveWithKeyboardInput()
		{
			var delta = Input.GetAxis(Horizontal);
			if (CheckIfMovementIsPossible((delta * transform.right).normalized))
				transform.Translate(delta * transform.right * _sideSpeedMultiplier * Time.deltaTime);
		}

		private void RotateWithKeyboardInput()
		{
			var delta = Input.GetAxis(Horizontal);
			transform.Rotate(delta * Vector3.up * _rotationSpeedMultiplier * Time.deltaTime);
		}

		private void SpeedUp()
		{
			//transform.Translate(transform.forward * Time.deltaTime * _speed);

			//if (!(_speed < _maximumSpeed)) return;
			//_speed += _forwardSpeedMultiplier;

			if (_rigidbody.velocity.z < _maximumSpeed)
				_rigidbody.AddForce(transform.forward * _forwardSpeedMultiplier);
		}

		private void ControlGravity()
		{
			Debug.Log(_rigidbody.velocity);
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
