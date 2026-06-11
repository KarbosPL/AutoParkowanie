using UnityEngine;

public class ParkingFSM_Rownolegle : MonoBehaviour
{
    public enum State { Driving, Positioning, ParaRight1, ParaRight2, ParaLeft1, ParaLeft2, Centering, Done }
    public State currentState = State.Driving;

    private Rigidbody rb;
    private ParkingSensor sensor;

    [Header("Prędkości")]
    public float driveSpeed   = 3f;
    public float reverseSpeed = 1.5f;
    public float alignSpeed   = 0.8f;

    [Header("Bezpieczeństwo")]
    public float frontStopDist = 1.5f;
    public float backStopDist  = 2.0f;
    public float sideWarnDist  = 1.5f;

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
            case State.ParaRight1:  ParaRight1();  break;
            case State.ParaRight2:  ParaRight2();  break;
            case State.ParaLeft1:   ParaLeft1();   break;
            case State.ParaLeft2:   ParaLeft2();   break;
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
                if (!miejsceZajetePrawe)
                {
                    Debug.Log("Prawa: miejsce WOLNE - parkuję!");
                    parkingRight = true;
                    currentState = State.Positioning;
                    return;
                }
                else
                {
                    miejsceZajetePrawe = false;
                    Debug.Log("Prawa: zajęte - szukam dalej");
                }
            }
        }

        if (skanujePrawe && sensor.rightMiddle < 5f)
        {
            miejsceZajetePrawe = true;
        }

        if (sensor.rightMiddle < 3f)
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
                if (!miejsceZajeteLewe)
                {
                    Debug.Log("Lewa: miejsce WOLNE - parkuję!");
                    parkingRight = false;
                    currentState = State.Positioning;
                    return;
                }
                else
                {
                    miejsceZajeteLewe = false;
                    Debug.Log("Lewa: zajęte - szukam dalej");
                }
            }
        }

        if (skanujeLewe && sensor.leftMiddle < 5f)
        {
            miejsceZajeteLewe = true;
        }

        if (sensor.leftMiddle < 3f)
            skanujeLewe = false;

        prevCameraLineR = cameraLineR;
        prevCameraLineL = cameraLineL;
    }

    void Positioning()
    {
        if (sensor.front < frontStopDist) { rb.linearVelocity = Vector3.zero; return; }
        rb.linearVelocity = transform.forward * driveSpeed;

        bool cameraLine = parkingRight ? sensor.cameraBR : sensor.cameraBL;
        if (cameraLine)
        {
            rb.linearVelocity = Vector3.zero;
            startAngle   = transform.eulerAngles.y;
            targetAngle  = parkingRight ? startAngle - 40f : startAngle + 40f;
            currentState = parkingRight ? State.ParaRight1 : State.ParaLeft1;
            Debug.Log("Zaczynam manewr równoległy!");
        }
    }

    void ParaRight1()
    {
        if (BackObstacleCheck()) return;
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        rb.linearVelocity = -transform.forward * reverseSpeed;
        if (Mathf.Abs(diff) > 0.2f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
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
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        rb.linearVelocity = -transform.forward * reverseSpeed;
        if (Mathf.Abs(diff) > 3f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Centering;
            Debug.Log("Centrowanie");
        }
    }

    // void ParaLeft1()
    // {
    //     if (BackObstacleCheck()) return;
    //     float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
    //     rb.linearVelocity = -transform.forward * reverseSpeed;
    //     if (Mathf.Abs(diff) > 0.2f)
    //         rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
    //     else
    //     {
    //         targetAngle  = startAngle;
    //         currentState = State.ParaLeft2;
    //         Debug.Log("Para lewo faza 2");
    //     }
    // }

    // void ParaLeft2()
    // {
    //     if (BackObstacleCheck()) return;
    //     float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
    //     rb.linearVelocity = -transform.forward * reverseSpeed;
    //     if (Mathf.Abs(diff) > 3f)
    //         rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
    //     else
    //     {
    //         rb.linearVelocity = Vector3.zero;
    //         currentState = State.Centering;
    //         Debug.Log("Centrowanie");
    //     }
    // }
    void ParaLeft1()
    {
        if (BackObstacleCheck()) return;
        
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        rb.linearVelocity = -transform.forward * reverseSpeed;
        
        if (Mathf.Abs(diff) > 0.2f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
        else
        {
            targetAngle = startAngle;
            currentState = State.ParaLeft2;
            Debug.Log("Para lewo faza 2");
        }
    }

    void ParaLeft2()
    {
        if (BackObstacleCheck()) return;
        
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        rb.linearVelocity = -transform.forward * reverseSpeed;
        
        if (Mathf.Abs(diff) > 3f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Centering;
            Debug.Log("Centrowanie");
        }
    }
    void Centering()
    {
        float diff = sensor.front - sensor.back;
        if (Mathf.Abs(diff) >0.2f)
            rb.linearVelocity = transform.forward * (diff > 0 ? 1f : -1f) * alignSpeed;
        else
        {
            rb.linearVelocity = Vector3.zero;
            currentState = State.Done;
            Debug.Log("Zaparkowano równolegle!");
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