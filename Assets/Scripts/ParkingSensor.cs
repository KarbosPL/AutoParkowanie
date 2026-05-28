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

    void FixedUpdate()
    {
        Measure();
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

        front = Cast(pos + fwd * 2.3f,   fwd,  frontSensorLength);
        back  = Cast(pos - fwd * 2.3f,  -fwd,  frontSensorLength);

        frontRight = Cast(pos + fwd * 2.3f + rgt, (fwd + rgt).normalized,  diagSensorLength);
        frontLeft  = Cast(pos + fwd * 2.3f - rgt, (fwd - rgt).normalized,  diagSensorLength);
        backRight  = Cast(pos - fwd * 2.3f + rgt, (-fwd + rgt).normalized, diagSensorLength);
        backLeft   = Cast(pos - fwd * 2.3f - rgt, (-fwd - rgt).normalized, diagSensorLength);

        rightFront  = Cast(pos + fwd * 1.5f,  rgt, sideSensorLength);
        rightMiddle = Cast(pos,                rgt, sideSensorLength);
        rightBack   = Cast(pos - fwd * 1.5f,  rgt, sideSensorLength);

        leftFront   = Cast(pos + fwd * 1.5f, -rgt, sideSensorLength);
        leftMiddle  = Cast(pos,              -rgt,  sideSensorLength);
        leftBack    = Cast(pos - fwd * 1.5f, -rgt,  sideSensorLength);
    }

    void Draw()
    {
        Vector3 pos = transform.position;
        Vector3 fwd = transform.forward;
        Vector3 rgt = transform.right;

        Debug.DrawRay(pos + fwd * 2.3f,   fwd  * front,  Color.red);
        Debug.DrawRay(pos - fwd * 2.3f,  -fwd  * back,   Color.red);

        Debug.DrawRay(pos + fwd * 2.3f + rgt, (fwd + rgt).normalized  * frontRight, new Color(1f, 0.5f, 0f));
        Debug.DrawRay(pos + fwd * 2.3f - rgt, (fwd - rgt).normalized  * frontLeft,  new Color(1f, 0.5f, 0f));
        Debug.DrawRay(pos - fwd * 2.3f + rgt, (-fwd + rgt).normalized * backRight,  new Color(1f, 0.5f, 0f));
        Debug.DrawRay(pos - fwd * 2.3f - rgt, (-fwd - rgt).normalized * backLeft,   new Color(1f, 0.5f, 0f));

        Debug.DrawRay(pos + fwd * 1.5f,  rgt * rightFront,  Color.blue);
        Debug.DrawRay(pos,               rgt * rightMiddle, Color.cyan);
        Debug.DrawRay(pos - fwd * 1.5f,  rgt * rightBack,   Color.green);

        Debug.DrawRay(pos + fwd * 1.5f, -rgt * leftFront,  new Color(0.5f, 0f, 1f));
        Debug.DrawRay(pos,              -rgt * leftMiddle,  new Color(0.5f, 0f, 1f));
        Debug.DrawRay(pos - fwd * 1.5f, -rgt * leftBack,   new Color(0.5f, 0f, 1f));
    }
}