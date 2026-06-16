using UnityEngine;

public class ParkingFSM_Prostopadle : MonoBehaviour
{
    public enum State { Driving, Positioning, PerpRight, PerpLeft, Centering, Done }
    public State currentState = State.Driving;

    private Rigidbody rb;
    private ParkingSensor sensor;

    [Header("Prędkości")]
    public float driveSpeed   = 3f;
    public float reverseSpeed = 1.5f;
    public float alignSpeed   = 0.8f;

    [Header("Bezpieczeństwo")]
    public float frontStopDist = 2f;
    public float backStopDist  = 2.0f;
    public float sideWarnDist  = 1.5f;

    [Header("Manewr")]
    public float perpDepth = 7.5f;
    public float earlyTurnOffset = 2f; // O ile wcześniej zacząć manewr (w jednostkach)

    [Header("Debug")]
    public bool parkingRight       = true;
    public bool skanujePrawe       = false;
    public bool skanujeLewe        = false;
    public bool miejsceZajetePrawe = false;
    public bool miejsceZajeteLewe  = false;

    private bool prevCameraLineR = false;
    private bool prevCameraLineL = false;
    private float startAngle;
    private float targetAngle;
    private Vector3 perpStartPos;
    private bool positionReached = false;
    private Vector3 earlyTurnPos;

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
            case State.Centering:   Centering();   break;
            case State.Done:        Done();        break;
        }
    }

    void Driving()
    {
        if (sensor.front < frontStopDist || sensor.frontLeft < frontStopDist || sensor.frontRight < frontStopDist)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        rb.linearVelocity = transform.forward * driveSpeed;

        bool cameraLineR = sensor.cameraFR;
        bool cameraLineL = sensor.cameraFL;
        bool nowaLiniaR  = !prevCameraLineR && cameraLineR;
        bool nowaLiniaL  = !prevCameraLineL && cameraLineL;

        // ── PRAWA STRONA ──────────────────────────────────
        if (nowaLiniaR)
        {
            if (!skanujePrawe)
            {
                skanujePrawe       = true;
                miejsceZajetePrawe = false;
                Debug.Log("Prawa: start skanowania");
            }
            else
            {
                Debug.Log("Prawa: miejsce - parkuję!");
                parkingRight = false;
                currentState = State.Positioning;
                positionReached = false;
                return;
            }
        }

        if (skanujePrawe && sensor.rightMiddle < 7f)
            miejsceZajetePrawe = true;

        if (sensor.rightMiddle < 5f)
            skanujePrawe = false;

        // ── LEWA STRONA ───────────────────────────────────
        if (nowaLiniaL)
        {
            if (!skanujeLewe)
            {
                skanujeLewe       = true;
                miejsceZajeteLewe = false;
                Debug.Log("Lewa: start skanowania");
            }
            else
            {
                Debug.Log("Lewa: miejsce - parkuję!");
                parkingRight = true;
                currentState = State.Positioning;
                positionReached = false;
                return;
            }
        }

        if (skanujeLewe && sensor.leftMiddle < 7f)
            miejsceZajeteLewe = true;

        if (sensor.leftMiddle < 5f)
            skanujeLewe = false;

        prevCameraLineR = cameraLineR;
        prevCameraLineL = cameraLineL;
    }

    void Positioning()
    {
        if (sensor.front < frontStopDist) 
        { 
            rb.linearVelocity = Vector3.zero; 
            return; 
        }
        
        rb.linearVelocity = transform.forward * driveSpeed;

        // Używamy przedniej kamery do wykrycia linii i robimy wcześniejszy skręt
        bool frontLine = parkingRight ? sensor.cameraFR : sensor.cameraFL;
        
        if (frontLine && !positionReached)
        {
            // Wykryto linię - zapisz pozycję do wcześniejszego skrętu
            earlyTurnPos = transform.position + transform.forward * earlyTurnOffset;
            positionReached = true;
            Debug.Log($"Wykryto linię, skręt nastąpi za {earlyTurnOffset} jednostki");
        }
        
        // Sprawdź czy dojechaliśmy do pozycji wczesnego skrętu
        if (positionReached && Vector3.Distance(transform.position, earlyTurnPos) < 0.1f)
        {
            rb.linearVelocity = Vector3.zero;
            startAngle   = transform.eulerAngles.y;
            targetAngle  = parkingRight ? startAngle + 90f : startAngle - 90f;
            perpStartPos = transform.position;
            currentState = parkingRight ? State.PerpRight : State.PerpLeft;
            Debug.Log($"Zaczynam manewr prostopadły WCZEŚNIEJ o {earlyTurnOffset}!");
        }
    }

    void PerpRight()
    {
        if (BackObstacleCheck()) return;
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
                currentState = State.Centering;
                Debug.Log("Centrowanie");
            }
        }
    }

    void PerpLeft()
    {
        if (BackObstacleCheck()) return;
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
                currentState = State.Centering;
                Debug.Log("Centrowanie");
            }
        }
    }

    void Centering()
    {
        float diff = sensor.front - sensor.back;
        if (Mathf.Abs(diff) > 0.2f)
            rb.linearVelocity = transform.forward * (diff > 0 ? 1f : -1f) * alignSpeed;
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Done;
            Debug.Log("Zaparkowano prostopadle!");
        }
    }

    void Done() => rb.linearVelocity = Vector3.zero;

    bool BackObstacleCheck()
    {
        if (sensor.back      < backStopDist ||
            sensor.backLeft  < sideWarnDist ||
            sensor.backRight < sideWarnDist)
        {
            rb.linearVelocity = transform.forward * 0.3f;
            Debug.Log("Korekta - za blisko z tyłu");
            return true;
        }
        return false;
    }
}