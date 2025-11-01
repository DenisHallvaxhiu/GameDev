using System.Collections.Generic;
using UnityEngine;

public class MoveToScript : MonoBehaviour {
    [Header("Path")]
    //if true it loops, if false it goes backwards
    [SerializeField] private bool closedLoop = false;

    //Path of the moving object (needs at least 2)
    [SerializeField] private Vector2[] trajectoryPoints;

    //Allows the Prefab to go back and forth
    [SerializeField] private bool backAndForth = false;

    //Speed of movement
    [SerializeField] private float unitsPerSecond = 3f;

    //Pause at edges (last and first) 
    [SerializeField] private float pauseAtEnds = 0f;


    [SerializeField] private GameObject movingObjectPrefab;
    // little sprite to show trajectory segments
    [SerializeField] private GameObject trajectoryPoint;
    [SerializeField] private bool trajectoryVisible = false;
    [SerializeField] private float trajectorySpacing = 0.5f;

    private float snapEpsilon = 0.001f;

    // spawned blade + cached rb
    private GameObject movingObject;
    private Rigidbody2D rb;

    // trajectory state
    private List<Vector2> path = new List<Vector2>();
    private int segStartIndex = 0; // index into path for start of current segment
    private int dir = +1;          // +1 forward, -1 backward 
    private float pauseTimer = 0f; // >0 means currently paused

    private void Start() {
        if(trajectoryPoints == null || trajectoryPoints.Length < 2) {
            Debug.LogWarning("[SawTrapSimple] Provide at least 2 trajectoryPoints.");
            return;
        }

        BuildPath();
        if(trajectoryVisible && trajectoryPoint != null) BuildTrajectory();

        CreateMainObject();

        // place exactly on first point
        SetPosition(path[0],snap: true);

        // if closed loop, ensure direction always forward (we'll wrap)
        if(closedLoop) dir = +1;
    }

    private void FixedUpdate() {

        if(movingObject == null) return;
        if(unitsPerSecond <= 0f) return;
        // handle optional pause at ends (only for open YoYo)
        if(!closedLoop && pauseTimer > 0f) {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }


        float distToTravel = unitsPerSecond * Time.fixedDeltaTime;

        // Consume distance possibly across multiple short segments
        while(distToTravel > 0f) {
            Vector2 currentPos = GetPosition();
            Vector2 segStart = path[segStartIndex];
            Vector2 segEnd = path[NextIndex(segStartIndex)];

            // When we’re exactly on a point, ensure we move toward segEnd
            Vector2 toEnd = segEnd - currentPos;
            float remain = toEnd.magnitude;

            if(remain <= snapEpsilon) {
                // Snap and advance segment
                SetPosition(segEnd,snap: true);
                if(!AdvanceSegment()) return; // might pause/stop this frame
                continue;
            }

            if(distToTravel >= remain) {
                // Reach the end of this segment this frame
                SetPosition(segEnd,snap: true);
                distToTravel -= remain;

                if(!AdvanceSegment()) return; // might pause/stop this frame
            }
            else {
                // Move partway along the segment
                Vector2 step = toEnd.normalized * distToTravel;
                SetPosition(currentPos + step,snap: false);
                distToTravel = 0f;
            }
        }

    }

    private void BuildPath() {
        path.Clear();
        path.AddRange(trajectoryPoints);
    }

    private void BuildTrajectory() {
        // draw trajectory along each consecutive pair
        for(int i = 0;i < trajectoryPoints.Length - 1;i++) {
            SpawnTrajectoryBetween(trajectoryPoints[i],trajectoryPoints[i + 1]);
        }
        if(closedLoop && trajectoryPoints.Length >= 2) {
            SpawnTrajectoryBetween(trajectoryPoints[trajectoryPoints.Length - 1],trajectoryPoints[0]);
        }
    }

    private void SpawnTrajectoryBetween(Vector2 a,Vector2 b) {
        float segLen = Vector2.Distance(a,b);
        if(segLen <= 0f) return;

        float spacing = Mathf.Max(0.01f,trajectorySpacing);
        int count = Mathf.Max(1,Mathf.FloorToInt(segLen / spacing));
        Vector2 dir = (b - a).normalized;
        float step = segLen / count;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up,dir);
        for(int c = 0;c <= count;c++) {
            Vector2 position = a + dir * (c * step);
            Instantiate(trajectoryPoint,position,rotation,this.transform);
        }
    }

    private void CreateMainObject() {
        Vector2 startPos = path[0];
        movingObject = (movingObjectPrefab != null)
            ? Instantiate(movingObjectPrefab,startPos,Quaternion.identity,this.transform)
            : new GameObject("GameObject");

        rb = movingObject.GetComponent<Rigidbody2D>();
        if(rb != null) {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

    }

    private bool AdvanceSegment() {
        if(closedLoop) {
            segStartIndex = (segStartIndex + 1) % path.Count;
            return true;
        }
        else {
            if(dir > 0) {
                // moving forward
                if(NextIndex(segStartIndex) == path.Count - 1) {
                    // we just snapped to LAST point — make it the new start, then flip
                    segStartIndex = path.Count - 1;               // ★ crucial
                    if(pauseAtEnds > 0f) pauseTimer = pauseAtEnds;
                    dir = -1;
                }
                else {
                    segStartIndex += 1;                            // normal advance
                }
            }
            else {
                // moving backward
                if(segStartIndex == 0) {
                    // we just snapped to FIRST point — make it the new start, then flip
                    segStartIndex = 0;                             // ★ crucial
                    if(pauseAtEnds > 0f) pauseTimer = pauseAtEnds;
                    dir = +1;
                }
                else {
                    segStartIndex -= 1;                            // normal advance
                }
            }
            return true;
        }
    }

    private int NextIndex(int startIdx) {
        if(closedLoop) {
            return (startIdx + 1) % path.Count;
        }
        else {
            // For open path, “next” depends on dir
            int next = startIdx + dir;
            next = Mathf.Clamp(next,0,path.Count - 1);
            return next;
        }
    }

    private Vector2 GetPosition() {
        if(rb != null) return rb.position;
        return movingObject.transform.position;
    }

    private void SetPosition(Vector2 pos,bool snap) {
        if(rb != null) {
            if(snap) rb.position = pos;
            else rb.MovePosition(pos);
        }
        else {
            movingObject.transform.position = pos;
        }
    }

    private void OnDrawGizmos() {
        if(trajectoryPoints == null || trajectoryPoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for(int i = 0;i < trajectoryPoints.Length;i++) {
            Gizmos.DrawSphere(trajectoryPoints[i],0.12f);
            if(i < trajectoryPoints.Length - 1)
                Gizmos.DrawLine(trajectoryPoints[i],trajectoryPoints[i + 1]);
        }
        if(closedLoop && trajectoryPoints.Length >= 2) {
            Gizmos.DrawLine(trajectoryPoints[trajectoryPoints.Length - 1],trajectoryPoints[0]);
        }
    }
}
