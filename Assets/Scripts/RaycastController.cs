using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

    public LayerMask collisionMask;
	
    public const float skinWidth = .015f;
    const float dstBetweenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D BoxCollider2D;
    public RaycastOrigins raycastOrigins;

    public virtual void Awake() {
        BoxCollider2D = GetComponent<BoxCollider2D> ();
    }

    public virtual void Start() {
        CalculateRaySpacing ();
    }

    protected void UpdateRaycastOrigins() {
        Bounds bounds = BoxCollider2D.bounds;
        bounds.Expand (skinWidth * -2);
		
        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing() {
        Bounds bounds = BoxCollider2D.bounds;
        bounds.Expand (skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;
		
        horizontalRayCount = Mathf.RoundToInt (boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt (boundsWidth / dstBetweenRays);
		
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
	
    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}

public struct CollisionInfo {
    public bool Above, Below;
    public bool Left, Right;

    public bool ClimbingSlope;
    public bool DescendingSlope;
    public bool SlidingDownMaxSlope;

    public float SlopeAngle, SlopeAngleOld;
    public Vector2 SlopeNormal;
    public Vector2 MoveAmountOld;
    public int FaceDir;
    public bool FallingThroughPlatform;

    public void Reset() {
        Above = Below = false;
        Left = Right = false;
        ClimbingSlope = false;
        DescendingSlope = false;
        SlidingDownMaxSlope = false;
        SlopeNormal = Vector2.zero;

        SlopeAngleOld = SlopeAngle;
        SlopeAngle = 0;
    }
}