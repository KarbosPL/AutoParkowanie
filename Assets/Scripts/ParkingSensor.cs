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

    [Header("Kamery (dół)")]
    public bool cameraFL = false;
    public bool cameraFR = false;
    public bool cameraBL = false;
    public bool cameraBR = false;
    public float lineThreshold = 0.6f;

    void FixedUpdate()
    {
        Measure();
        ScanParkingLines();
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

    void ScanParkingLines()
    {
        if (ParkingTexture1.parkingTexture == null) return;

        Vector3 pos = transform.position;
        Vector3 fwd = transform.forward;
        Vector3 rgt = transform.right;

        // Kamery tylko przy krawędziach auta (±1m od środka)
        // Przód auta (+2.3f wzdłuż forward), tył (-2.3f)
        Vector3 frontCenter = pos + fwd * 2.3f;
        Vector3 backCenter  = pos - fwd * 2.3f;

        // Skanuj wąski pas przy krawędzi lewej i prawej
        cameraFL = ScanStrip(frontCenter, -4f, -0.8f);
        cameraFR = ScanStrip(frontCenter,  0.8f,  4f);
        cameraBL = ScanStrip(backCenter,  -4f, -0.8f);
        cameraBR = ScanStrip(backCenter,   0.8f,  4f);
    }

    bool ScanStrip(Vector3 basePos, float xFrom, float xTo)
    {
        int steps = 30;

        for (int i = 0; i <= steps; i++)
        {
            float t = (float)i / steps;
            float localX = Mathf.Lerp(xFrom, xTo, t);
            // Używamy transform.right żeby działało niezależnie od rotacji auta
            Vector3 worldPos = basePos + transform.right * localX;
            if (ScanPoint(worldPos)) return true;
        }
        return false;
    }

    bool ScanPoint(Vector3 worldPos)
    {
        RaycastHit hit;
        Vector3 origin = worldPos + Vector3.up * 2f;

        if (Physics.Raycast(origin, Vector3.down, out hit, 5f,
            LayerMask.GetMask("Default")))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Texture2D tex = ParkingTexture1.parkingTexture;
                int px = (int)(hit.textureCoord.x * tex.width);
                int py = (int)(hit.textureCoord.y * tex.height);
                Color c = tex.GetPixel(px, py);
                return c.r > lineThreshold && c.g > lineThreshold && c.b > lineThreshold;
            }
        }
        return false;
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


        // Wizualizacja kamer (linie w dół)
        Vector3 fwdPos  = transform.position + transform.forward * 2.3f;
        Vector3 backPos = transform.position - transform.forward * 2.3f;

        Color colFL = cameraFL ? Color.white : Color.yellow;
        Color colFR = cameraFR ? Color.white : Color.yellow;
        Color colBL = cameraBL ? Color.white : Color.yellow;
        Color colBR = cameraBR ? Color.white : Color.yellow;

        // Prawy prostokąt przedni
        DrawCameraRect(fwdPos,  transform.right,  0.8f, 4f, transform.forward, 0.3f, colFR);
        // Lewy prostokąt przedni
        DrawCameraRect(fwdPos, -transform.right,  0.8f, 4f, transform.forward, 0.3f, colFL);
        // Prawy prostokąt tylny
        DrawCameraRect(backPos,  transform.right, 0.8f, 4f, transform.forward, 0.3f, colBR);
        // Lewy prostokąt tylny
        DrawCameraRect(backPos, -transform.right, 0.8f, 4f, transform.forward, 0.3f, colBL);
    }
    void DrawCameraRect(Vector3 origin, Vector3 sideDir, float near, float far,Vector3 fwd, float halfHeight, Color c)
    {
        float y = 0.05f; // tuż nad podłożem

        Vector3 p1 = origin + sideDir * near + fwd *  halfHeight + Vector3.up * y;
        Vector3 p2 = origin + sideDir * far  + fwd *  halfHeight + Vector3.up * y;
        Vector3 p3 = origin + sideDir * far  + fwd * -halfHeight + Vector3.up * y;
        Vector3 p4 = origin + sideDir * near + fwd * -halfHeight + Vector3.up * y;

        Debug.DrawLine(p1, p2, c);
        Debug.DrawLine(p2, p3, c);
        Debug.DrawLine(p3, p4, c);
        Debug.DrawLine(p4, p1, c);
    }
}