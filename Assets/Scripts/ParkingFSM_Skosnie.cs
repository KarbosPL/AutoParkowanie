using UnityEngine;

public class ParkingFSM_Skosnie : MonoBehaviour
{
    public enum State { Driving, BackingUp, SkosRight, SkosLeft, Centering, Done }
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
    public float backupDistance = 8f;
    public float skosDepth = 3f;

    [Header("Debug")]
    public bool parkingRight       = true;
    public bool skanujePrawe       = false;
    public bool skanujeLewe        = false;
    public bool miejsceZajetePrawe = false;
    public bool miejsceZajeteLewe  = false;
    public bool enableDebugLogs = true;

    private bool prevCameraLineR = false;
    private bool prevCameraLineL = false;
    private float startAngle;
    private float targetAngle;
    private Vector3 manewrStartPos;
    private Vector3 backupStartPos;
    private bool backupComplete = false;
    private float manewrDistance = 0f;

    void Start()
    {
        rb     = GetComponent<Rigidbody>();
        sensor = GetComponent<ParkingSensor>();
        
        Debug.Log("=== PARKING SKOŚNY - START ===");
        
        if (sensor == null)
            Debug.LogError("❌ ParkingFSM_Skosnie: Brak komponentu ParkingSensor!");
        else
            Debug.Log("✅ ParkingSensor znaleziony");
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Driving:     Driving();     break;
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

        // PRAWA STRONA
        if (nowaLiniaR)
        {
            if (!skanujePrawe)
            {
                skanujePrawe       = true;
                miejsceZajetePrawe = false;
                if (enableDebugLogs) Debug.Log("🔴 PRAWA: Początek miejsca - start skanowania");
            }
            else
            {
                if (!miejsceZajetePrawe)
                {
                    if (enableDebugLogs) Debug.Log("🟢 PRAWA: WOLNE MIEJSCE! Zaczynam cofanie na PRAWO");
                    parkingRight = true;
                    // OD RAZU PRZEJDŹ DO COFANIA - bez Positioning!
                    backupStartPos = transform.position;
                    backupComplete = false;
                    currentState = State.BackingUp;
                    return;
                }
                else
                {
                    miejsceZajetePrawe = false;
                    if (enableDebugLogs) Debug.Log("🔴 PRAWA: Miejsce ZAJĘTE - szukam dalej");
                }
            }
        }

        if (skanujePrawe && sensor.frontRight <4f)
            miejsceZajetePrawe = true;

        if (sensor.frontRight < 2f)
            skanujePrawe = false;

        // LEWA STRONA
        if (nowaLiniaL)
        {
            if (!skanujeLewe)
            {
                skanujeLewe       = true;
                miejsceZajeteLewe = false;
                if (enableDebugLogs) Debug.Log("🔴 LEWA: Początek miejsca - start skanowania");
            }
            else
            {
                if (!miejsceZajeteLewe)
                {
                    if (enableDebugLogs) Debug.Log("🟢 LEWA: WOLNE MIEJSCE! Zaczynam cofanie na LEWO");
                    parkingRight = false;
                    // OD RAZU PRZEJDŹ DO COFANIA - bez Positioning!
                    backupStartPos = transform.position;
                    backupComplete = false;
                    currentState = State.BackingUp;
                    return;
                }
                else
                {
                    miejsceZajeteLewe = false;
                    if (enableDebugLogs) Debug.Log("🔴 LEWA: Miejsce ZAJĘTE - szukam dalej");
                }
            }
        }

        if (skanujeLewe && sensor.leftMiddle < 3.7f)
            miejsceZajeteLewe = true;

        if (sensor.leftMiddle < 2f)
            skanujeLewe = false;

        prevCameraLineR = cameraLineR;
        prevCameraLineL = cameraLineL;
    }

    void BackingUp()
    {
        rb.linearVelocity = -transform.forward * reverseSpeed;
        
        float distanceBacked = Vector3.Distance(transform.position, backupStartPos);
        
        if (enableDebugLogs && Time.frameCount % 30 == 0)
            Debug.Log($"⬅️ Cofanie: {distanceBacked:F1}/{backupDistance}f");
        
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
                Debug.Log($"✅ Cofnięto {distanceBacked:F1}f. Skręt w PRAWO: {startAngle:F0}° → {targetAngle:F0}°");
            }
            else
            {
                targetAngle = startAngle - 45f;
                Debug.Log($"✅ Cofnięto {distanceBacked:F1}f. Skręt w LEWO: {startAngle:F0}° → {targetAngle:F0}°");
            }
            
            currentState = parkingRight ? State.SkosRight : State.SkosLeft;
        }
        
        if (sensor.back < backStopDist)
        {
            rb.linearVelocity = Vector3.zero;
            Debug.Log("⚠️ Przeszkoda z tyłu - przerywam cofanie!");
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
                Debug.Log($"🎯 Wjechano {manewrDistance:F1}f - centrowanie");
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
                Debug.Log($"🎯 Wjechano {manewrDistance:F1}f - centrowanie");
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
            Debug.Log("🏁🏁🏁 ZAPARKOWANO SKOŚNIE! 🏁🏁🏁");
        }
    }

    void Done()
    {
        rb.linearVelocity = Vector3.zero;
    }

    bool BackObstacleCheck()
    {
        if (sensor.back      < backStopDist ||
            sensor.backLeft  < sideWarnDist ||
            sensor.backRight < sideWarnDist)
        {
            rb.linearVelocity = transform.forward * 0.3f;
            Debug.Log("⚠️ Korekta - za blisko z tyłu");
            return true;
        }
        return false;
    }
}