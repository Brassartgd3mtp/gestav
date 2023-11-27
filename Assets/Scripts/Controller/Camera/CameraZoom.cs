using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraControl
{
	public class CameraZoom : MonoBehaviour
	{
		[SerializeField] private float _speed = 25f;
		[SerializeField] private float _smoothing = 5f;
		[SerializeField] private Vector2 _range = new(30f, 70f);
		public Vector2 Range => _range;
		[SerializeField] private Transform _cameraHolder;
		public Transform CameraHolder => _cameraHolder;

		public Vector3 CameraDirection => transform.InverseTransformDirection(_cameraHolder.forward);

		public Vector3 TargetPosition;

		private float _input;


		private void Awake()
		{
			TargetPosition = _cameraHolder.localPosition;
		}

		private void HandleInput()
		{
			_input = Input.GetAxisRaw("Mouse ScrollWheel");
		}

		private void Zoom()
		{
			Vector3 nextTargetPosition = TargetPosition + CameraDirection * (_input * _speed);
			if (IsInBounds(nextTargetPosition)) TargetPosition = nextTargetPosition;
			_cameraHolder.localPosition = Vector3.Lerp(_cameraHolder.localPosition, TargetPosition, Time.deltaTime * _smoothing);
		}

		private bool IsInBounds(Vector3 position)
		{
			return position.magnitude > _range.x && position.magnitude < _range.y;
		}

		private void Update()
		{
			HandleInput();
			Zoom();
		}
	}
}
