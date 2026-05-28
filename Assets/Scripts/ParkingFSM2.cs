using UnityEngine;

public class ParkingFSM2 : MonoBehaviour
{
    public enum ParkingType { None, Perpendicular, Parallel }
    public enum State 
    { 
        Driving, Positioning, 
        PerpRight, PerpLeft, 
        ParaRight1, ParaRight2, 
        ParaLeft1, ParaLeft2, 
        Centering, Done 
    }

    public State currentState = State.Driving;
    public ParkingType parkingType = ParkingType.None;

    private Rigidbody rb;
    private ParkingSensor sensor;

    [Header("Prędkości")]
    public float driveSpeed    = 3f;
    public float reverseSpeed  = 1.5f;
    public float alignSpeed    = 0.8f;

    [Header("Progi parkowania")]
    public float minDepthPerp      = 4.5f;
    public float minLengthPerp     = 2.8f;
    public float minLengthParallel = 6.5f;

    [Header("Parkowanie prostopadłe")]
    public float perpDepth = 5.5f;

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
    private bool inGap        = false;
    private bool prevRightClear = false;
    private bool prevLeftClear  = false;
    private bool perpParking  = false;

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
            case State.ParaRight1:  ParaRight1();  break;
            case State.ParaRight2:  ParaRight2();  break;
            case State.ParaLeft1:   ParaLeft1();   break;
            case State.ParaLeft2:   ParaLeft2();   break;
            case State.Centering:   Centering();   break;
            case State.Done:        Done();        break;
        }
    }

    // ─── STAN 1: Jedź i wykrywaj luki ───────────────────────────────────
    void Driving()
    {
        if (sensor.front < frontStopDist)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        rb.linearVelocity = transform.forward * driveSpeed;

        // Używamy tylko środkowego czujnika do wykrywania luki
        bool rightClear = sensor.rightMiddle > 3.5f;
        bool leftClear  = sensor.leftMiddle  > 3.5f;

        // Wejście w lukę po prawej
        if (!prevRightClear && rightClear)
        {
            gapStartPos  = transform.position;
            inGap        = true;
            parkingRight = true;
        }

        // Wejście w lukę po lewej - tylko gdy prawa zajęta
        if (!prevLeftClear && leftClear && !inGap)
        {
            gapStartPos  = transform.position;
            inGap        = true;
            parkingRight = false;
        }

        // Mierz lukę na bieżąco
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
        if (Physics.Raycast(midPoint, sideDir, out hit, 15f))
            measuredDepth = hit.distance;
        else
            measuredDepth = 15f;

        Debug.Log($"Luka: długość={measuredLength:F1}m, głębokość={measuredDepth:F1}m, strona={(parkingRight ? "prawa" : "lewa")}");

        if (measuredDepth >= minDepthPerp && measuredLength >= minLengthPerp)
        {
            parkingType  = ParkingType.Perpendicular;
            perpParking  = true;
            currentState = State.Positioning;
            Debug.Log("Decyzja: PROSTOPADŁE");
        }
        else if (measuredLength >= minLengthParallel)
        {
            parkingType  = ParkingType.Parallel;
            perpParking  = false;
            currentState = State.Positioning;
            Debug.Log("Decyzja: RÓWNOLEGŁE");
        }
        else
        {
            Debug.Log("Luka za mała, szukam dalej...");
        }
    }

    // ─── POZYCJONOWANIE ──────────────────────────────────────────────────
    void Positioning()
    {
        if (sensor.front < frontStopDist)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        rb.linearVelocity = transform.forward * driveSpeed;

        float middleSensor = parkingRight ? sensor.rightMiddle : sensor.leftMiddle;

        if (middleSensor < 3.5f)
        {
            rb.linearVelocity = Vector3.zero;
            startAngle   = transform.eulerAngles.y;
            perpStartPos = transform.position;

            if (perpParking)
            {
                targetAngle  = parkingRight ? startAngle + 90f : startAngle - 90f;
                currentState = parkingRight ? State.PerpRight : State.PerpLeft;
                Debug.Log("Zaczynam manewr prostopadły tyłem!");
            }
            else
            {
                targetAngle  = parkingRight ? startAngle - 40f : startAngle + 40f;
                currentState = parkingRight ? State.ParaRight1 : State.ParaLeft1;
                Debug.Log("Zaczynam manewr równoległy!");
            }
        }
    }

    // ─── PARKOWANIE PROSTOPADŁE ──────────────────────────────────────────
    void PerpRight()
    {
        float cur  = transform.eulerAngles.y;
        float diff = Mathf.DeltaAngle(cur, targetAngle);

        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 2f)
        {
            float rotStep = Mathf.Clamp(diff, -50f, 50f) * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotStep, 0));
        }
        else
        {
            float distanceMoved = Vector3.Distance(transform.position, perpStartPos);
            if (distanceMoved < perpDepth)
            {
                rb.linearVelocity = -transform.forward * reverseSpeed;
            }
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
        float cur  = transform.eulerAngles.y;
        float diff = Mathf.DeltaAngle(cur, targetAngle);

        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 2f)
        {
            float rotStep = Mathf.Clamp(diff, -50f, 50f) * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotStep, 0));
        }
        else
        {
            float distanceMoved = Vector3.Distance(transform.position, perpStartPos);
            if (distanceMoved < perpDepth)
            {
                rb.linearVelocity = -transform.forward * reverseSpeed;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                currentState = State.Done;
                Debug.Log("Zaparkowano prostopadle!");
            }
        }
    }

    // ─── PARKOWANIE RÓWNOLEGŁE PRAWE ─────────────────────────────────────
    void ParaRight1()
    {
        if (BackObstacleCheck()) return;

        float cur  = transform.eulerAngles.y;
        float diff = Mathf.DeltaAngle(cur, targetAngle);

        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 1f)
        {
            float rotStep = Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotStep, 0));
        }
        else
        {
            targetAngle  = startAngle;
            currentState = State.ParaRight2;
            Debug.Log("Para prawo faza 2");
        }
    }

    void ParaRight2()
    {
        if (BackObstacleCheck()) return;

        float cur  = transform.eulerAngles.y;
        float diff = Mathf.DeltaAngle(cur, targetAngle);

        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 1f)
        {
            float rotStep = Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, rotStep, 0));
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Centering;
            Debug.Log("Centrowanie");
        }
    }

    // ─── PARKOWANIE RÓWNOLEGŁE LEWE ──────────────────────────────────────
    void ParaLeft1()
    {
        if (BackObstacleCheck()) return;

        float cur  = transform.eulerAngles.y;
        float diff = Mathf.DeltaAngle(cur, targetAngle);

        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 1f)
        {
            float rotStep = Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -rotStep, 0));
        }
        else
        {
            targetAngle  = startAngle;
            currentState = State.ParaLeft2;
            Debug.Log("Para lewo faza 2");
        }
    }

    void ParaLeft2()
    {
        if (BackObstacleCheck()) return;

        float cur  = transform.eulerAngles.y;
        float diff = Mathf.DeltaAngle(cur, targetAngle);

        rb.linearVelocity = -transform.forward * reverseSpeed;

        if (Mathf.Abs(diff) > 1f)
        {
            float rotStep = Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -rotStep, 0));
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Centering;
            Debug.Log("Centrowanie");
        }
    }

    // ─── CENTROWANIE I KONIEC ────────────────────────────────────────────
    void Centering()
    {
        float diff = sensor.front - sensor.back;

        if (Mathf.Abs(diff) > 0.4f)
        {
            float dir = diff > 0 ? 1f : -1f;
            rb.linearVelocity = transform.forward * dir * alignSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Done;
            Debug.Log("Zaparkowano!");
        }
    }

    void Done()
    {
        rb.linearVelocity = Vector3.zero;
    }

    // ─── POMOCNICZE ──────────────────────────────────────────────────────
    bool BackObstacleCheck()
    {
        if (sensor.back      < backStopDist  ||
            sensor.backLeft  < sideWarnDist  ||
            sensor.backRight < sideWarnDist)
        {
            rb.linearVelocity = transform.forward * 0.3f;
            Debug.Log("Korekta - za blisko z tyłu");
            return true;
        }
        return false;
    }
}