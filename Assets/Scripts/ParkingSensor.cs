using UnityEngine;
using UnityEngine.InputSystem;

public class ParkingSensor : MonoBehaviour
{
    public float sensorLength = 10f;

    // Wyniki pomiarów - odległości po prawej stronie
    public float rightFront = 0f;
    public float rightBack = 0f;

    // Szerokość wykrytej luki
    public float gapSize = 0f;
    public bool gapFound = false;

    private bool inGap = false;
    private float gapStart = 0f;

    void FixedUpdate()
    {
        // Czujnik prawy przedni
        Vector3 frontSensorPos = transform.position 
            + transform.forward * 2f 
            + transform.right * 1f;
        
        RaycastHit hit;
        if (Physics.Raycast(frontSensorPos, transform.right, out hit, sensorLength))
            rightFront = hit.distance;
        else
            rightFront = sensorLength;

        // Czujnik prawy tylny
        Vector3 backSensorPos = transform.position 
            - transform.forward * 2f 
            + transform.right * 1f;
        
        if (Physics.Raycast(backSensorPos, transform.right, out hit, sensorLength))
            rightBack = hit.distance;
        else
            rightBack = sensorLength;

        // Wykrywanie luki
        DetectGap();

        // Wizualizacja w edytorze
        Debug.DrawRay(frontSensorPos, transform.right * rightFront, Color.red);
        Debug.DrawRay(backSensorPos, transform.right * rightBack, Color.blue);
    }

    void DetectGap()
    {
        bool clearRight = rightFront > 3f && rightBack > 3f;

        if (clearRight && !inGap)
        {
            inGap = true;
            gapStart = transform.position.z;
            gapFound = false;
        }
        else if (!clearRight && inGap)
        {
            inGap = false;
            gapSize = Mathf.Abs(transform.position.z - gapStart);
            if (gapSize >= 6f)
                gapFound = true;
        }
    }
}