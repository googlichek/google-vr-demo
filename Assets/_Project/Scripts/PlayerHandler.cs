using System.Collections;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerHandler : MonoBehaviour
{
	private const int LockAngle = 360;
	private const string Horizontal = "Horizontal";

	[Header("Tweakable Variables")]
	[SerializeField] [Range(0, 180)] private int _minAngle = 20;
	[SerializeField] [Range(0, 180)] private int _maxAngle = 90;
	[SerializeField] [Range(0, 100)] private int _forwardSpeedMultiplier = 10;
	[SerializeField] [Range(0, 100)] private int _sideSpeedMultiplier = 10;
	[SerializeField] [Range(0, 1000)] private int _maximumSpeed = 500;

	private Rigidbody _rigidbody = null;
	private Camera _mainCamera = null;

	void Start()
	{
		InputTracking.Recenter();

		_rigidbody = GetComponent<Rigidbody>();
		_mainCamera = Camera.main;

		StartCoroutine(AccumulateForwardVelocity());
	}

	void FixedUpdate()
	{
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

		var delta = (Camera.main.transform.localRotation.eulerAngles.z) % LockAngle;

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
		var delta = Input.GetAxis(Horizontal);
		transform.Translate(delta * transform.right * _sideSpeedMultiplier * Time.deltaTime);
	}

	private IEnumerator AccumulateForwardVelocity()
	{
		while (_rigidbody.velocity.z < _maximumSpeed)
		{
			yield return new WaitForEndOfFrame();
			_rigidbody.AddForce(transform.forward * _forwardSpeedMultiplier, ForceMode.Force);
		}
	}
}
