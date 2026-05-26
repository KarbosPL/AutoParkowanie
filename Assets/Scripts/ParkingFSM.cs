using UnityEngine;

public class ParkingFSM : MonoBehaviour
{
    public enum State { Driving, Positioning, Parking, Done }
    public State currentState = State.Driving;

    private Rigidbody rb;
    private ParkingSensor sensor;

    public float driveSpeed = 3f;
    public float reverseSpeed = 2f;

    private float timer = 0f;
    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sensor = GetComponent<ParkingSensor>();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Driving:     Driving();     break;
            case State.Positioning: Positioning(); break;
            case State.Parking:     Parking();     break;
            case State.Done:        Done();        break;
        }
    }

    void Driving()
    {
        if (sensor.front < 2f) { rb.linearVelocity = Vector3.zero; return; }

        rb.linearVelocity = transform.forward * driveSpeed;

        if (sensor.gapFound)
        {
            sensor.gapFound = false;
            targetPosition = sensor.gapCenterPosition;
            currentState = State.Positioning;
            Debug.Log("Luka znaleziona, pozycjonuję!");
        }
    }

void Positioning()
{
    // Mierz odległość wzdłuż kierunku jazdy
    Vector3 toTarget = targetPosition - transform.position;
    float dist = Vector3.Dot(toTarget, transform.forward);

    if (dist > 0.3f)
    {
        rb.linearVelocity = transform.forward * driveSpeed;
    }
    else
    {
        rb.linearVelocity = Vector3.zero;
        timer = 0f;
        currentState = State.Parking;
        Debug.Log("Zaczynam parkowanie!");
    }
}

    void Parking()
    {
        timer += Time.fixedDeltaTime;

        if (timer < 2f)
        {
            rb.linearVelocity = -transform.forward * reverseSpeed;
            transform.Rotate(0, 25f * Time.fixedDeltaTime, 0);
        }
        else if (timer < 4f)
        {
            rb.linearVelocity = -transform.forward * reverseSpeed;
            transform.Rotate(0, -25f * Time.fixedDeltaTime, 0);
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