using UnityEngine;

namespace CardboardVRProto
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class PlayerHandler : MonoBehaviour
	{
		private const string Horizontal = GlobalVariables.Horizontal;
		private const int LockAngle = 360;

		[Header("Tweakable Variables")]
		[SerializeField] [Range(0, 180)] private int _minAngle = 20;
		[SerializeField] [Range(0, 180)] private int _maxAngle = 90;
		[SerializeField] [Range(0.01f, 1)] private float _forwardSpeedMultiplier = 0.01f;
		[SerializeField] [Range(0, 100)] private int _sideSpeedMultiplier = 10;
		[SerializeField] [Range(0, 1000)] private int _maximumSpeed = 500;

		private Camera _mainCamera = null;

		private float _speed = 0;

		void Start()
		{
			_mainCamera = Camera.main;
		}

		void FixedUpdate()
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

			transform.Translate(directionVector * _sideSpeedMultiplier * Time.deltaTime);
		}

		private void MoveWithKeyboardInput()
		{
			var delta = Input.GetAxis(GlobalVariables.Horizontal);
			transform.Translate(delta * transform.right * _sideSpeedMultiplier * Time.deltaTime);
		}

		private void SpeedUp()
		{
			transform.Translate(transform.forward * Time.deltaTime * _speed);

			if (!(_speed < _maximumSpeed)) return;
			_speed += _forwardSpeedMultiplier;
		}
	}
}
