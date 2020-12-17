using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController 
{
	public CollisionInfo Collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start() {
		base.Start ();
		Collisions.FaceDir = 1;

	}

	public void Move(Vector2 moveAmount) {
		Move (moveAmount, Vector2.zero);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();

		Collisions.Reset ();
		Collisions.MoveAmountOld = moveAmount;
		playerInput = input;

		OnMove(ref moveAmount, input);
		
		transform.Translate (moveAmount);

		if (standingOnPlatform) {
			Collisions.Below = true;
		}
	}

	protected virtual void OnMove(ref Vector2 moveAmount, Vector2 input)
	{
		if (moveAmount.x != 0) {
			Collisions.FaceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions (ref moveAmount);
		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}
	}

	protected virtual void HorizontalCollisions(ref Vector2 moveAmount) {
		float directionX = Collisions.FaceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}
				
				
				moveAmount.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				Collisions.Left = directionX == -1;
				Collisions.Right = directionX == 1;
			}
		}
	}

	protected virtual void VerticalCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			if (hit)
			{
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				Collisions.Below = directionY == -1;
				Collisions.Above = directionY == 1;
			}
		}
	}

	

	

	

}