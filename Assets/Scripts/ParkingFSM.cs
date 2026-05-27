using UnityEngine;

public class ParkingFSM : MonoBehaviour
{
    public enum State { Driving, Positioning, Reversing1, Reversing2, Centering, Done }
    public State currentState = State.Driving;

    private Rigidbody rb;
    private ParkingSensor sensor;

    [Header("Prędkości")]
    public float driveSpeed   = 3f;
    public float reverseSpeed = 1.5f;
    public float alignSpeed   = 0.8f;

    [Header("Bezpieczeństwo")]
    public float frontStopDist = 1.5f;
    public float backStopDist  = 2.5f;
    public float sideWarnDist  = 2.0f;

    private float startAngle;
    private float targetAngle;
    private Vector3 gapCenter;
    private bool emergencyStop = false;

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
        case State.Reversing1:  Reversing1();  break;
        case State.Reversing2:  Reversing2();  break;
        case State.Centering:   Centering();   break;
        case State.Done:        Done();        break;
    }
}

    void Driving()
    {
        rb.linearVelocity = transform.forward * driveSpeed;

        if (sensor.gapFound)
        {
            sensor.gapFound = false;
            gapCenter = sensor.gapCenterPosition;
            currentState = State.Positioning;
            Debug.Log("Luka znaleziona, pozycjonuję...");
        }
    }

    void Positioning()
    {
        rb.linearVelocity = transform.forward * driveSpeed;

        // Czekaj aż tylny czujnik zobaczy przednie auto luki
        if (sensor.rightBack < 4f)
        {
            rb.linearVelocity = Vector3.zero;
            startAngle  = transform.eulerAngles.y;
            targetAngle = startAngle-45f;
            currentState = State.Reversing1;
            Debug.Log("Zaczynam manewr!");
        }
    }

    void Reversing1()
{
    // STOP jeśli cokolwiek z tyłu
    if (sensor.back < backStopDist || 
        sensor.backLeft < 1.5f || 
        sensor.backRight < 1.5f)
    {
        rb.linearVelocity = transform.forward * 0.3f; // odsuwaj się
        Debug.Log("Korekta Reversing1 - za blisko z tyłu");
        return;
    }

    if (sensor.rightFront < sideWarnDist)
    {
        rb.linearVelocity = Vector3.zero;
        return;
    }

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
        currentState = State.Reversing2;
        Debug.Log("Faza 2 - wyrównanie");
    }
}

void Reversing2()
{
    // STOP jeśli cokolwiek z tyłu
    if (sensor.back < backStopDist || 
        sensor.backLeft < 1.5f || 
        sensor.backRight < 1.5f)
    {
        rb.linearVelocity = transform.forward * 0.3f; // odsuwaj się
        Debug.Log("Korekta Reversing2 - za blisko z tyłu");
        return;
    }

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
        Debug.Log("Centrowanie w luce");
    }
}

    void Centering()
    {
        float frontDist = sensor.front;
        float backDist  = sensor.back;
        float diff      = frontDist - backDist;

        if (Mathf.Abs(diff) > 0.4f)
        {
            // Jedź w stronę gdzie jest więcej miejsca
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
}