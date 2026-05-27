using UnityEngine;

public class ParkingSensor : MonoBehaviour
{
    public float sideSensorLength = 10f;
    public float frontSensorLength = 5f;
    public float diagSensorLength = 6f;

    [Header("Przód i tył")]
    public float front, back;
    public float frontLeft, frontRight;
    public float backLeft, backRight;

    [Header("Boki prawe")]
    public float rightFront, rightMiddle, rightBack;

    [Header("Boki lewe")]
    public float leftFront, leftMiddle, leftBack;

    [Header("Luka prawa")]
    public bool gapFound = false;
    public float gapSize = 0f;
    public Vector3 gapCenterPosition;

    private enum SideState { Obstacle, Clear }
    private SideState prevRight = SideState.Obstacle;
    private Vector3 gapStartPos;

    void FixedUpdate()
    {
        Measure();
        DetectGap();
        Draw();
    }

    float Cast(Vector3 origin, Vector3 dir, float len)
    {
        RaycastHit hit;
        return Physics.Raycast(origin, dir, out hit, len) ? hit.distance : len;
    }

    void Measure()
    {
        Vector3 pos = transform.position;
        Vector3 fwd = transform.forward;
        Vector3 rgt = transform.right;

        // Przód i tył - prosto
        front = Cast(pos + fwd * 2.3f,   fwd,  frontSensorLength);
        back  = Cast(pos - fwd * 2.3f,  -fwd,  frontSensorLength);

        // Przód - skosy (45 stopni)
        Vector3 diagFR = (fwd + rgt).normalized;
        Vector3 diagFL = (fwd - rgt).normalized;
        frontRight = Cast(pos + fwd * 2.3f + rgt * 1f,  diagFR, diagSensorLength);
        frontLeft  = Cast(pos + fwd * 2.3f - rgt * 1f,  diagFL, diagSensorLength);

        // Tył - skosy (45 stopni)
        Vector3 diagBR = (-fwd + rgt).normalized;
        Vector3 diagBL = (-fwd - rgt).normalized;
        backRight = Cast(pos - fwd * 2.3f + rgt * 1f,  diagBR, diagSensorLength);
        backLeft  = Cast(pos - fwd * 2.3f - rgt * 1f,  diagBL, diagSensorLength);

        // Boki prawe (3 czujniki)
        rightFront  = Cast(pos + fwd * 1.5f,  rgt, sideSensorLength);
        rightMiddle = Cast(pos,                rgt, sideSensorLength);
        rightBack   = Cast(pos - fwd * 1.5f,  rgt, sideSensorLength);

        // Boki lewe (3 czujniki)
        leftFront   = Cast(pos + fwd * 1.5f, -rgt, sideSensorLength);
        leftMiddle  = Cast(pos,              -rgt, sideSensorLength);
        leftBack    = Cast(pos - fwd * 1.5f, -rgt, sideSensorLength);
    }

    void DetectGap()
    {
        float threshold = 5f;
        bool clear = rightFront > threshold && rightMiddle > threshold && rightBack > threshold;
        SideState cur = clear ? SideState.Clear : SideState.Obstacle;

        if (prevRight == SideState.Obstacle && cur == SideState.Clear)
        {
            gapStartPos = transform.position;
            gapFound = false;
            gapSize = 0f;
        }

        if (cur == SideState.Clear)
            gapSize = Vector3.Distance(gapStartPos, transform.position);

        if (prevRight == SideState.Clear && cur == SideState.Obstacle)
        {
            if (gapSize >= 5f)
            {
                gapFound = true;
                gapCenterPosition = (gapStartPos + transform.position) / 2f;
                Debug.Log($"Luka: {gapSize:F1}m");
            }
            else
            {
                gapSize = 0f;
            }
        }

        prevRight = cur;
    }

    void Draw()
    {
        Vector3 pos = transform.position;
        Vector3 fwd = transform.forward;
        Vector3 rgt = transform.right;

        // Przód/tył czerwone
        Debug.DrawRay(pos + fwd * 2.3f,   fwd  * front,  Color.red);
        Debug.DrawRay(pos - fwd * 2.3f,  -fwd  * back,   Color.red);

        // Skosy pomarańczowe
        Debug.DrawRay(pos + fwd * 2.3f + rgt,  (fwd + rgt).normalized  * frontRight, new Color(1f, 0.5f, 0f));
        Debug.DrawRay(pos + fwd * 2.3f - rgt,  (fwd - rgt).normalized  * frontLeft,  new Color(1f, 0.5f, 0f));
        Debug.DrawRay(pos - fwd * 2.3f + rgt,  (-fwd + rgt).normalized * backRight,  new Color(1f, 0.5f, 0f));
        Debug.DrawRay(pos - fwd * 2.3f - rgt,  (-fwd - rgt).normalized * backLeft,   new Color(1f, 0.5f, 0f));

        // Boki prawe: niebieski/cyjan/zielony
        Debug.DrawRay(pos + fwd * 1.5f,  rgt * rightFront,  Color.blue);
        Debug.DrawRay(pos,               rgt * rightMiddle, Color.cyan);
        Debug.DrawRay(pos - fwd * 1.5f,  rgt * rightBack,   Color.green);

        // Boki lewe: fioletowy
        Debug.DrawRay(pos + fwd * 1.5f, -rgt * leftFront,  new Color(0.5f, 0f, 1f));
        Debug.DrawRay(pos,              -rgt * leftMiddle,  new Color(0.5f, 0f, 1f));
        Debug.DrawRay(pos - fwd * 1.5f, -rgt * leftBack,   new Color(0.5f, 0f, 1f));
    }
}