using UnityEngine;

public class ParkingFSM_Skosnie : MonoBehaviour
{
    public enum State { Driving, Positioning, BackingUp, SkosRight, SkosLeft, Centering, Done }
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

    [Header("Manewr")]
    public float backupDistance = 6f;
    public float skosDepth = 8f;

    [Header("Debug")]
    public bool parkingRight       = true;
    public bool skanujePrawe       = false;
    public bool skanujeLewe        = false;
    public bool miejsceZajetePrawe = false;
    public bool miejsceZajeteLewe  = false;
    public bool enableDebugLogs = false; // DODANE - wyłącznik logów

    private bool prevCameraLineR = false;
    private bool prevCameraLineL = false;
    private float startAngle;
    private float targetAngle;
    private Vector3 manewrStartPos;
    private Vector3 backupStartPos;
    private bool backupComplete = false;
    private float manewrDistance = 0f;
    private int logCounter = 0; // Licznik logów

    void Start()
    {
        rb     = GetComponent<Rigidbody>();
        sensor = GetComponent<ParkingSensor>();
        
        if (sensor == null)
            Debug.LogError("ParkingFSM_Skosnie: Brak komponentu ParkingSensor!");
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Driving:     Driving();     break;
            case State.Positioning: Positioning(); break;
            case State.BackingUp:   BackingUp();   break;
            case State.SkosRight:   SkosRight();   break;
            case State.SkosLeft:    SkosLeft();    break;
            case State.Centering:   Centering();   break;
            case State.Done:        Done();        break;
        }
    }

    void Driving()
    {
        if (sensor == null) return;
        
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

        if (nowaLiniaR)
        {
            if (!skanujePrawe)
            {
                skanujePrawe       = true;
                miejsceZajetePrawe = false;
                if (enableDebugLogs) Debug.Log("Skosnie - Prawa: start skanowania");
            }
            else
            {
                if (!miejsceZajetePrawe)
                {
                    if (enableDebugLogs) Debug.Log("Skosnie - Prawa: znaleziono WOLNE miejsce - parkuję!");
                    parkingRight = true;
                    currentState = State.Positioning;
                    backupComplete = false;
                    return;
                }
                else
                {
                    miejsceZajetePrawe = false;
                    if (enableDebugLogs) Debug.Log("Skosnie - Prawa: miejsce ZAJĘTE - szukam dalej");
                }
            }
        }

        if (skanujePrawe && sensor.rightMiddle < 5f)
            miejsceZajetePrawe = true;

        if (sensor.rightMiddle < 3f)
            skanujePrawe = false;

        if (nowaLiniaL)
        {
            if (!skanujeLewe)
            {
                skanujeLewe       = true;
                miejsceZajeteLewe = false;
                if (enableDebugLogs) Debug.Log("Skosnie - Lewa: start skanowania");
            }
            else
            {
                if (!miejsceZajeteLewe)
                {
                    if (enableDebugLogs) Debug.Log("Skosnie - Lewa: znaleziono WOLNE miejsce - parkuję!");
                    parkingRight = false;
                    currentState = State.Positioning;
                    backupComplete = false;
                    return;
                }
                else
                {
                    miejsceZajeteLewe = false;
                    if (enableDebugLogs) Debug.Log("Skosnie - Lewa: miejsce ZAJĘTE - szukam dalej");
                }
            }
        }

        if (skanujeLewe && sensor.leftMiddle < 5f)
            miejsceZajeteLewe = true;

        if (sensor.leftMiddle < 3f)
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

        bool targetLine = parkingRight ? sensor.cameraBR : sensor.cameraBL;
        
        if (targetLine && !backupComplete)
        {
            rb.linearVelocity = Vector3.zero;
            backupStartPos = transform.position;
            currentState = State.BackingUp;
            if (enableDebugLogs) Debug.Log($"Skosnie: Wykryto linię, zaczynam cofanie o {backupDistance} jednostek");
        }
    }

    void BackingUp()
    {
        rb.linearVelocity = -transform.forward * reverseSpeed;
        
        float distanceBacked = Vector3.Distance(transform.position, backupStartPos);
        
        if (distanceBacked >= backupDistance)
        {
            rb.linearVelocity = Vector3.zero;
            backupComplete = true;
            
            manewrStartPos = transform.position;
            manewrDistance = 0f;
            
            startAngle = transform.eulerAngles.y;
            if (parkingRight)
            {
                targetAngle = startAngle + 45f;
            }
            else
            {
                targetAngle = startAngle - 45f;
            }
            
            currentState = parkingRight ? State.SkosRight : State.SkosLeft;
            if (enableDebugLogs) Debug.Log($"Skosnie: Cofnięto {distanceBacked:F1}f, zaczynam manewr!");
        }
        
        if (sensor.back < backStopDist)
        {
            rb.linearVelocity = Vector3.zero;
            if (enableDebugLogs) Debug.Log("Skosnie: Przeszkoda z tyłu - przerywam cofanie");
            currentState = State.Driving;
        }
    }

    void SkosRight()
    {
        if (BackObstacleCheck()) return;
        
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        
        rb.linearVelocity = transform.forward * driveSpeed;

        if (Mathf.Abs(diff) > 2f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
        }
        else
        {
            manewrDistance += driveSpeed * Time.fixedDeltaTime;
            
            if (manewrDistance < skosDepth)
            {
                rb.linearVelocity = transform.forward * driveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                currentState = State.Centering;
                if (enableDebugLogs) Debug.Log("Skosnie: Centrowanie");
            }
        }
    }

    void SkosLeft()
    {
        if (BackObstacleCheck()) return;
        
        float diff = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);
        
        rb.linearVelocity = transform.forward * driveSpeed;

        if (Mathf.Abs(diff) > 2f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mathf.Clamp(diff, -40f, 40f) * Time.fixedDeltaTime, 0));
        }
        else
        {
            manewrDistance += driveSpeed * Time.fixedDeltaTime;
            
            if (manewrDistance < skosDepth)
            {
                rb.linearVelocity = transform.forward * driveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
                currentState = State.Centering;
                if (enableDebugLogs) Debug.Log("Skosnie: Centrowanie");
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
            if (enableDebugLogs) Debug.Log("Skosnie: Zaparkowano skośnie!");
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
            if (enableDebugLogs) Debug.Log("Skosnie: Korekta - za blisko z tyłu");
            return true;
        }
        return false;
    }
}