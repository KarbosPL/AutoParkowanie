using UnityEngine;

public class WheelVisuals : MonoBehaviour
{
    private Rigidbody rb;
    
    // Referencje do różnych FSM
    private ParkingFSM_Rownolegle fsmRownolegle;
    private ParkingFSM_Prostopadle fsmProstopadle;
    private ParkingFSM_Skosnie fsmSkosnie;

    private Transform wheelFL, wheelFR, wheelRL, wheelRR;
    private Transform wheelFL_visual, wheelFR_visual, wheelRL_visual, wheelRR_visual;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Pobierz wszystkie FSM (którekolwiek jest aktywne)
        fsmRownolegle = GetComponent<ParkingFSM_Rownolegle>();
        fsmProstopadle = GetComponent<ParkingFSM_Prostopadle>();
        fsmSkosnie = GetComponent<ParkingFSM_Skosnie>();

        if (fsmRownolegle == null && fsmProstopadle == null && fsmSkosnie == null)
            Debug.LogError("Żaden FSM nie znaleziony na obiekcie!");

        // Tworzenie kół
        CreateWheel("Wheel_FL", new Vector3(-0.5f, -0.15f, 0.4f), out wheelFL, out wheelFL_visual);
        CreateWheel("Wheel_FR", new Vector3(0.5f, -0.15f, 0.4f), out wheelFR, out wheelFR_visual);
        CreateWheel("Wheel_RL", new Vector3(-0.5f, -0.15f, -0.4f), out wheelRL, out wheelRL_visual);
        CreateWheel("Wheel_RR", new Vector3(0.5f, -0.15f, -0.4f), out wheelRR, out wheelRR_visual);
    }

    void CreateWheel(string name, Vector3 localPos, out Transform pivot, out Transform visual)
    {
        // Pivot do skrętu (tylko dla przednich kół)
        GameObject pivotObj = new GameObject(name + "_pivot");
        pivotObj.transform.SetParent(transform);
        pivotObj.transform.localPosition = localPos;
        pivotObj.transform.localRotation = Quaternion.identity;
        pivot = pivotObj.transform;

        // Wizualne koło (cylinder)
        GameObject wheelObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelObj.name = name;
        wheelObj.transform.SetParent(pivotObj.transform);
        wheelObj.transform.localPosition = Vector3.zero;
        wheelObj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        wheelObj.transform.localScale = new Vector3(0.9f, 0.18f, 0.9f);
        visual = wheelObj.transform;

        // Usuwamy kolider koła
        Destroy(wheelObj.GetComponent<Collider>());

        // Materiał
        Renderer r = wheelObj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            r.material.color = new Color(0.1f, 0.1f, 0.1f);
        }
    }

    void Update()
    {
        // Obliczenie prędkości obrotu koła
        float speed = rb.linearVelocity.magnitude;
        float direction = Vector3.Dot(rb.linearVelocity, transform.forward) >= 0 ? 1f : -1f;
        float rotAngle = speed * Time.deltaTime * 180f / (Mathf.PI * 0.35f);

        // Tylne koła - tylko toczenie (bez skrętu)
        if (wheelRL_visual != null)
            wheelRL_visual.Rotate(0, rotAngle * direction, 0, Space.Self);
        if (wheelRR_visual != null)
            wheelRR_visual.Rotate(0, rotAngle * direction, 0, Space.Self);

        // Przednie koła - skręt + toczenie
        float steer = GetSteerAngle();

        if (wheelFL != null)
            wheelFL.localEulerAngles = new Vector3(0, steer, 0);
        if (wheelFR != null)
            wheelFR.localEulerAngles = new Vector3(0, steer, 0);

        if (wheelFL_visual != null)
            wheelFL_visual.Rotate(0, rotAngle * direction, 0, Space.Self);
        if (wheelFR_visual != null)
            wheelFR_visual.Rotate(0, rotAngle * direction, 0, Space.Self);
    }

    float GetSteerAngle()
    {
        // === PARKOWANIE RÓWNOLEGŁE ===
        if (fsmRownolegle != null && fsmRownolegle.enabled)
        {
            switch (fsmRownolegle.currentState)
            {
                case ParkingFSM_Rownolegle.State.ParaRight1:
                    return 35f;   // Skręt w prawo
                case ParkingFSM_Rownolegle.State.ParaRight2:
                    return -25f;  // Wyrównanie
                case ParkingFSM_Rownolegle.State.ParaLeft1:
                    return -35f;  // Skręt w lewo
                case ParkingFSM_Rownolegle.State.ParaLeft2:
                    return 25f;   // Wyrównanie
                default:
                    return 0f;
            }
        }
        
        // === PARKOWANIE PROSTOPADŁE ===
        if (fsmProstopadle != null && fsmProstopadle.enabled)
        {
            switch (fsmProstopadle.currentState)
            {
                case ParkingFSM_Prostopadle.State.PerpRight:
                    return 35f;   // Skręt w prawo
                case ParkingFSM_Prostopadle.State.PerpLeft:
                    return -35f;  // Skręt w lewo
                case ParkingFSM_Prostopadle.State.Centering:
                    // Podczas centrowania delikatne korekty
                    float diff = 0f;
                    if (fsmProstopadle != null)
                    {
                        // Możesz dodać logikę centrowania z FSM
                    }
                    return 0f;
                default:
                    return 0f;
            }
        }
        
        // === PARKOWANIE SKOŚNE ===
        if (fsmSkosnie != null && fsmSkosnie.enabled)
        {
            switch (fsmSkosnie.currentState)
            {
                case ParkingFSM_Skosnie.State.SkosRight:
                    return 35f;   // Skręt w prawo
                case ParkingFSM_Skosnie.State.SkosLeft:
                    return -35f;  // Skręt w lewo
                case ParkingFSM_Skosnie.State.Centering:
                    return 0f;
                default:
                    return 0f;
            }
        }

        return 0f;
    }
}