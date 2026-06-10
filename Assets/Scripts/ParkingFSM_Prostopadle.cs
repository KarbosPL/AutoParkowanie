using UnityEngine;

public class ParkingFSM_Prostopadle : MonoBehaviour
{
    public enum State { Driving, Positioning, PerpRight, PerpLeft, Done }
    public State currentState = State.Driving;

    private Rigidbody rb;
    private ParkingSensor sensor;

    [Header("Prędkości")]
    public float driveSpeed   = 3f;
    public float reverseSpeed = 1.5f;

    [Header("Progi")]
    public float minDepthPerp  = 4.5f;
    public float minLengthPerp = 2.8f;
    public float perpDepth     = 5.5f;

    [Header("Bezpieczeństwo")]
    public float frontStopDist = 1.5f;
    public float backStopDist  = 2.0f;
    public float sideWarnDist  = 1.5f;

    [Header("Debug")]
    public float measuredLength = 0f;
    public float measuredDepth  = 0f;
    public bool  parkingRight   = true;

    private float startAngle;
    private float targetAngle;
    private Vector3 gapStartPos;
    private Vector3 perpStartPos;
    private bool inGap          = false;
    private bool prevRightClear = false;
    private bool prevLeftClear  = false;

    void Start()
    {
        rb     = GetComponent<Rigidbody>();
        sensor = GetComponent<ParkingSensor>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Driving:     Driving();     break;
            case State.Positioning: Positioning(); break;
            case State.PerpRight:   PerpRight();   break;
            case State.PerpLeft:    PerpLeft();    break;
            case State.Done:        Done();        break;
        }
    }

    void Driving()
    {
        if (sensor.front < frontStopDist) { rb.linearVelocity = Vector3.zero; return; }
        rb.linearVelocity = transform.forward * driveSpeed;

        bool rightClear = sensor.rightMiddle > 3.5f;
        bool leftClear  = sensor.leftMiddle  > 3.5f;

        if (!prevRightClear && rightClear)
        {
            gapStartPos = transform.position;
            inGap = true;
            parkingRight = true;
        }
        if (!prevLeftClear && leftClear && !inGap)
        {
            gapStartPos = transform.position;
            inGap = true;
            parkingRight = false;
        }

        if (inGap)
        {
            bool stillClear = parkingRight ? rightClear : leftClear;
            if (!stillClear)
            {
                inGap = false;
                MeasureAndDecide();
            }
        }

        prevRightClear = rightClear;
        prevLeftClear  = leftClear;
    }

    void MeasureAndDecide()
    {
        Vector3 toHere = transform.position - gapStartPos;
        measuredLength = Vector3.Dot(toHere, transform.forward);

        Vector3 midPoint = gapStartPos + transform.forward * (measuredLength / 2f);
        Vector3 sideDir  = parkingRight ? transform.right : -transform.right;

        RaycastHit hit;
        measuredDepth = Physics.Raycast(midPoint, sideDir, out hit, 15f) ? hit.distance : 15f;

        Debug.Log($"Luka prostopadła: dł={measuredLength:F1}m głęb={measuredDepth:F1}m strona={(parkingRight ? "prawa" : "lewa")}");

        if (measuredDepth >= minDepthPerp && measuredLength >= minLengthPerp)
        {
            currentState = State.Positioning;
            Debug.Log("Decyzja: PROSTOPADŁE");
        }
        else
        {
            Debug.Log("Luka za mała, szukam dalej...");
        }
    }

    void Positioning()
    {
        if (sensor.front < frontStopDist) { rb.linearVelocity = Vector3.zero; return; }
        rb.linearVelocity = transform.forward * driveSpeed;

        float middleSensor = parkingRight ? sensor.rightMiddle : sensor.leftMiddle;
        if (middleSensor < 3.5f)
        {
            rb.linearVelocity = Vector3.zero;
            startAngle   = transform.eulerAngles.y;
            perpStartPos = transform.position;
            targetAngle  = parkingRight ? startAngle + 90f : startAngle - 90f;
            currentState = parkingRight ? State.PerpRight : State.PerpLeft;
            Debug.Log("Zaczynam manewr prostopadły!");
        }
    }

    void PerpRight()
    {
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 2f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -50f, 50f) * Time.fixedDeltaTime, 0));
        else
        {
            if (Vector3.Distance(transform.position, perpStartPos) < perpDepth)
                rb.linearVelocity = -transform.forward * reverseSpeed;
            else
            {
                rb.linearVelocity = Vector3.zero;
                currentState = State.Done;
                Debug.Log("Zaparkowano prostopadle!");
            }
        }
    }

    void PerpLeft()
    {
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 2f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -50f, 50f) * Time.fixedDeltaTime, 0));
        else
        {
            if (Vector3.Distance(transform.position, perpStartPos) < perpDepth)
                rb.linearVelocity = -transform.forward * reverseSpeed;
            else
            {
                rb.linearVelocity = Vector3.zero;
                currentState = State.Done;
                Debug.Log("Zaparkowano prostopadle!");
            }
        }
    }

    void Done() => rb.linearVelocity = Vector3.zero;
}