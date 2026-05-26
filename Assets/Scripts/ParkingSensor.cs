using UnityEngine;

public class ParkingSensor : MonoBehaviour
{
    public float sideSensorLength = 10f;
    public float frontSensorLength = 5f;

    public float rightFront, rightMiddle, rightBack;
    public float front, back;

    public bool gapFound = false;
    public float gapSize = 0f;
    public Vector3 gapCenterPosition;

    private enum SideState { Obstacle, Clear }
    private SideState previousState = SideState.Obstacle;
    private Vector3 gapStartPos;

    void FixedUpdate()
    {
        Measure();
        Detect();
        Draw();
    }

    void Measure()
    {
        RaycastHit hit;
        front = Physics.Raycast(transform.position + transform.forward * 2.5f, transform.forward, out hit, frontSensorLength) ? hit.distance : frontSensorLength;
        back = Physics.Raycast(transform.position - transform.forward * 2.5f, -transform.forward, out hit, frontSensorLength) ? hit.distance : frontSensorLength;
        rightFront = Physics.Raycast(transform.position + transform.forward * 1.5f, transform.right, out hit, sideSensorLength) ? hit.distance : sideSensorLength;
        rightMiddle = Physics.Raycast(transform.position, transform.right, out hit, sideSensorLength) ? hit.distance : sideSensorLength;
        rightBack = Physics.Raycast(transform.position - transform.forward * 1.5f, transform.right, out hit, sideSensorLength) ? hit.distance : sideSensorLength;
    }

    void Detect()
    {
        bool clear = rightFront > 5f && rightMiddle > 5f && rightBack > 5f;
        SideState currentState = clear ? SideState.Clear : SideState.Obstacle;

        if (previousState == SideState.Obstacle && currentState == SideState.Clear)
        {
            gapStartPos = transform.position;
            gapFound = false;
        }

        if (previousState == SideState.Clear && currentState == SideState.Obstacle)
        {
            gapSize = Vector3.Distance(gapStartPos, transform.position);
            if (gapSize >= 4.9f)
            {
                gapFound = true;
                gapCenterPosition = (gapStartPos + transform.position) / 2f;
                Debug.Log($"Luka: {gapSize:F1}m");
            }
        }

        previousState = currentState;
    }

    void Draw()
    {
        Debug.DrawRay(transform.position + transform.forward * 2.5f, transform.forward * front, Color.red);
        Debug.DrawRay(transform.position - transform.forward * 2.5f, -transform.forward * back, Color.red);
        Debug.DrawRay(transform.position + transform.forward * 1.5f, transform.right * rightFront, Color.blue);
        Debug.DrawRay(transform.position, transform.right * rightMiddle, Color.cyan);
        Debug.DrawRay(transform.position - transform.forward * 1.5f, transform.right * rightBack, Color.green);
    }
}