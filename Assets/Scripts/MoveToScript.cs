using System.Collections.Generic;
using UnityEngine;

public class SawTrapSimple : MonoBehaviour {
    [Header("Path")]
    [Tooltip("If true, path loops from last point back to first. If false, path ping-pongs (YoYo).")]
    [SerializeField] private bool closedLoop = false;

    [Tooltip("World-space points for the path. Needs at least 2.")]
    [SerializeField] private Vector2[] sawPoints;

    [Tooltip("Movement speed in world units per second.")]
    [SerializeField] private float unitsPerSecond = 3f;

    [Tooltip("Pause time (seconds) at each end when NOT closedLoop.")]
    [SerializeField] private float pauseAtEnds = 0f;

    [Header("Visuals (optional)")]
    [SerializeField] private GameObject sawPrefab;
    [SerializeField] private GameObject chainPoint;   // little sprite to show chain segments
    [SerializeField] private bool chainVisible = false;
    [SerializeField] private float chainSpacing = 0.5f;
    [SerializeField] private bool spriteToTheFront = false;

    [Header("Runtime")]
    [SerializeField] private float snapEpsilon = 0.001f;

    // spawned blade + cached rb
    private GameObject sawBlade;
    private Rigidbody2D sawRB;

    // path/segment state
    private List<Vector2> path = new List<Vector2>();
    private int segStartIndex = 0; // index into path for start of current segment
    private int dir = +1;          // +1 forward, -1 backward (for YoYo)
    private float pauseTimer = 0f; // >0 means currently paused

    private void Start() {
        if(sawPoints == null || sawPoints.Length < 2) {
            Debug.LogWarning("[SawTrapSimple] Provide at least 2 sawPoints.");
            enabled = false;
            return;
        }

        BuildPath();
        if(chainVisible && chainPoint != null) BuildChain();

        CreateSawBlade();

        // place exactly on first point
        SetPosition(path[0],snap: true);

        // if closed loop, ensure direction always forward (we'll wrap)
        if(closedLoop) dir = +1;
    }

    private void FixedUpdate() {
        if(sawBlade == null) return;
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

    // --- helpers ---

    private void BuildPath() {
        path.Clear();
        path.AddRange(sawPoints);

        // For closed loop, we conceptually connect last -> first; we won't duplicate the first point here.
        // For open path, we’ll YoYo by flipping 'dir' at ends.
    }

    private void BuildChain() {
        // draw chain along each consecutive pair
        for(int i = 0;i < sawPoints.Length - 1;i++) {
            SpawnChainBetween(sawPoints[i],sawPoints[i + 1]);
        }
        if(closedLoop && sawPoints.Length >= 2) {
            SpawnChainBetween(sawPoints[sawPoints.Length - 1],sawPoints[0]);
        }
    }

    private void SpawnChainBetween(Vector2 a,Vector2 b) {
        float segLen = Vector2.Distance(a,b);
        if(segLen <= 0f) return;

        float spacing = Mathf.Max(0.01f,chainSpacing);
        int count = Mathf.Max(1,Mathf.FloorToInt(segLen / spacing));
        Vector2 dir = (b - a).normalized;
        float step = segLen / count;

        for(int c = 0;c <= count;c++) {
            Vector2 p = a + dir * (c * step);
            Instantiate(chainPoint,p,Quaternion.identity,this.transform);
        }
    }

    private void CreateSawBlade() {
        Vector2 startPos = path[0];
        sawBlade = (sawPrefab != null)
            ? Instantiate(sawPrefab,startPos,Quaternion.identity,this.transform)
            : new GameObject("SawBlade");

        sawRB = sawBlade.GetComponent<Rigidbody2D>();
        if(sawRB != null) {
            // For smooth scripted motion, use Kinematic
            sawRB.bodyType = RigidbodyType2D.Kinematic;
            sawRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if(spriteToTheFront) {
            var sr = sawBlade.GetComponent<SpriteRenderer>();
            if(sr != null) sr.sortingOrder = 999;
        }
    }

    private bool AdvanceSegment() {
        // Decide next segment based on closed/open
        if(closedLoop) {
            segStartIndex = (segStartIndex + 1) % path.Count;
            return true;
        }
        else {
            // open path, ping-pong
            if(dir > 0) {
                if(NextIndex(segStartIndex) == path.Count - 1) {
                    // reached last point; flip direction
                    if(pauseAtEnds > 0f) pauseTimer = pauseAtEnds;
                    dir = -1;
                }
                else {
                    segStartIndex += 1;
                }
            }
            else // dir < 0
            {
                if(segStartIndex == 0) {
                    // reached first point; flip direction
                    if(pauseAtEnds > 0f) pauseTimer = pauseAtEnds;
                    dir = +1;
                }
                else {
                    segStartIndex -= 1;
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
        if(sawRB != null) return sawRB.position;
        return sawBlade.transform.position;
    }

    private void SetPosition(Vector2 pos,bool snap) {
        if(sawRB != null) {
            if(snap) sawRB.position = pos;
            else sawRB.MovePosition(pos);
        }
        else {
            sawBlade.transform.position = pos;
        }
    }

    private void OnDrawGizmos() {
        if(sawPoints == null || sawPoints.Length == 0) return;

        Gizmos.color = Color.yellow;
        for(int i = 0;i < sawPoints.Length;i++) {
            Gizmos.DrawSphere(sawPoints[i],0.12f);
            if(i < sawPoints.Length - 1)
                Gizmos.DrawLine(sawPoints[i],sawPoints[i + 1]);
        }
        if(closedLoop && sawPoints.Length >= 2) {
            Gizmos.DrawLine(sawPoints[sawPoints.Length - 1],sawPoints[0]);
        }
    }
}
