using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent (typeof (PlatformerController2D))]
public class PlayerPlatformer : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	[SerializeField] private float _accelerationTimeAirborne = .2f;
	[SerializeField] private float _accelerationTimeGrounded = .1f;
	[SerializeField] private float _moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float _timeToWallUnstick;

	float _gravity;
	float _maxJumpVelocity;
	float _minJumpVelocity;
	Vector3 _velocity;
	float _velocityXSmoothing;

	Controller2D _controller;

	Vector2 _directionalInput;
	bool _wallSliding;
	int _wallDirX;

	private void Start() {
		_controller = GetComponent<Controller2D> ();

		_gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		_maxJumpVelocity = Mathf.Abs(_gravity) * timeToJumpApex;
		_minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (_gravity) * minJumpHeight);
	}

	private void FixedUpdate() 
	{
		CalculateVelocity ();
		HandleWallSliding ();

		_controller.Move (_velocity * Time.fixedDeltaTime, _directionalInput);

		if (_controller.Collisions.Above || _controller.Collisions.Below) 
		{
			if (_controller.Collisions.SlidingDownMaxSlope) 
			{
				_velocity.y += _controller.Collisions.SlopeNormal.y * -_gravity * Time.fixedDeltaTime;
			} 
			else 
			{
				_velocity.y = 0;
			}
		}
	}

	public void SetDirectionalInput (InputAction.CallbackContext ctx)
	{
		var input = ctx.ReadValue<Vector2>();
		_directionalInput = input;
	}

	public void HandleJump(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			OnJumpInputDown();
		}

		if (ctx.canceled)
		{
			OnJumpInputUp();
		}
	}

	private void OnJumpInputDown() {
		if (_wallSliding) {
			if (_wallDirX == _directionalInput.x) {
				_velocity.x = -_wallDirX * wallJumpClimb.x;
				_velocity.y = wallJumpClimb.y;
			}
			else if (_directionalInput.x == 0) {
				_velocity.x = -_wallDirX * wallJumpOff.x;
				_velocity.y = wallJumpOff.y;
			}
			else {
				_velocity.x = -_wallDirX * wallLeap.x;
				_velocity.y = wallLeap.y;
			}

		}
		if (_controller.Collisions.Below) {
			if (_controller.Collisions.SlidingDownMaxSlope) {
				if (_directionalInput.x != -Mathf.Sign (_controller.Collisions.SlopeNormal.x)) { // not jumping against max slope
					_velocity.y = _maxJumpVelocity * _controller.Collisions.SlopeNormal.y;
					_velocity.x = _maxJumpVelocity * _controller.Collisions.SlopeNormal.x;
				}
			} else {
				_velocity.y = _maxJumpVelocity;
			}
		}
	}

	private void OnJumpInputUp() {
		if (_velocity.y > _minJumpVelocity) {
			_velocity.y = _minJumpVelocity;
		}
	}


	private void HandleWallSliding() {
		_wallDirX = (_controller.Collisions.Left) ? -1 : 1;
		_wallSliding = false;
		if ((_controller.Collisions.Left || _controller.Collisions.Right) && !_controller.Collisions.Below && _velocity.y < 0) {
			_wallSliding = true;

			if (_velocity.y < -wallSlideSpeedMax) {
				_velocity.y = -wallSlideSpeedMax;
			}

			if (_timeToWallUnstick > 0) {
				_velocityXSmoothing = 0;
				_velocity.x = 0;

				if (_directionalInput.x != _wallDirX && _directionalInput.x != 0) {
					_timeToWallUnstick -= Time.deltaTime;
				}
				else {
					_timeToWallUnstick = wallStickTime;
				}
			}
			else {
				_timeToWallUnstick = wallStickTime;
			}

		}

	}

	private void CalculateVelocity() {
		var targetVelocityX = _directionalInput.x * _moveSpeed;
		_velocity.x = Mathf.SmoothDamp (
			_velocity.x, 
			targetVelocityX, ref _velocityXSmoothing, 
			(_controller.Collisions.Below) ? _accelerationTimeGrounded : _accelerationTimeAirborne
			);
		_velocity.y += _gravity * Time.deltaTime;
	}
}