using UnityEngine;

public class Kamera : MonoBehaviour
{
    [Header("Ustawienia podstawowe")]
    public float height = 8f;           // Wysokość kamery
    public float distance = 12f;        // Odległość od auta
    public float angle = 30f;           // Kąt patrzenia
    
    [Header("Sterowanie")]
    public float zoomSpeed = 2f;        // Szybkość przybliżania (kółko myszy)
    public float minDistance = 5f;      // Minimalna odległość
    public float maxDistance = 20f;     // Maksymalna odległość
    
    public float rotationSpeed = 100f;  // Szybkość obracania (WSAD)
    public float heightSpeed = 50f;     // Szybkość zmiany wysokości (RF)
    public float minHeight = 3f;        // Minimalna wysokość
    public float maxHeight = 15f;       // Maksymalna wysokość
    
    [Header("Płynność")]
    public float smoothSpeed = 5f;      // Płynność podążania
    
    // Prywatne zmienne
    private float currentDistance;
    private float currentHeight;
    private float currentAngle = 0f;
    private Vector3 velocityRef = Vector3.zero;
    private Transform autoTransform;
    
    void Start()
    {
        currentDistance = distance;
        currentHeight = height;
    }
    
    void LateUpdate()
    {
        // Znajdź AutoAgent
        GameObject auto = GameObject.Find("AutoAgent");
        if (auto == null) return;
        
        autoTransform = auto.transform;
        
        // === STEROWANIE ===
        
        // 1. Przyblizanie/oddalanie (kółko myszy)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        
        // 2. Zmiana wysokości (R - wyżej, F - niżej)
        if (Input.GetKey(KeyCode.R))
            currentHeight += heightSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.F))
            currentHeight -= heightSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        
        // 3. Obracanie kamerą wokół auta (A - lewo, D - prawo)
        if (Input.GetKey(KeyCode.A))
            currentAngle -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            currentAngle += rotationSpeed * Time.deltaTime;
        
        // 4. Reset kamery (spacja)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentDistance = distance;
            currentHeight = height;
            currentAngle = 0f;
        }
        
        // === OBLICZANIE POZYCJI KAMERY ===
        
        // Oblicz kąt w radianach
        float rad = currentAngle * Mathf.Deg2Rad;
        
        // Oblicz pozycję na okręgu wokół auta
        float x = Mathf.Sin(rad) * currentDistance;
        float z = Mathf.Cos(rad) * currentDistance;
        
        // Pozycja docelowa
        Vector3 desiredPosition = autoTransform.position + new Vector3(x, currentHeight, z);
        
        // Płynne przesunięcie kamery
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocityRef, 1f / smoothSpeed);
        
        // Kamera patrzy na auto
        Vector3 lookPoint = autoTransform.position + Vector3.up * (currentHeight * 0.3f);
        transform.LookAt(lookPoint);
    }
}