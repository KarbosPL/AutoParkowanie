using UnityEngine;

public class Kamera : MonoBehaviour
{
    [Header("Pozycja kamery")]
    public float height = 8f;        // Wysokość
    public float distance = 12f;     // Odległość
    public float sideOffset = 0f;    // Przesunięcie w bok (-5 = lewo, 5 = prawo)
    
    [Header("Kąt patrzenia")]
    public float lookHeight = 2f;    // Na którą wysokość auta patrzy
    
    private Transform autoTransform;
    
    void LateUpdate()
    {
        GameObject auto = GameObject.Find("AutoAgent");
        if (auto == null) return;
        
        autoTransform = auto.transform;
        
        // Pozycja kamery względem auta
        Vector3 desiredPosition = autoTransform.position 
            + (-autoTransform.forward * distance)
            + (autoTransform.right * sideOffset)
            + (Vector3.up * height);
        
        transform.position = desiredPosition;
        
        // Kamera patrzy na auto (na wysokość lookHeight)
        Vector3 lookPoint = autoTransform.position + Vector3.up * lookHeight;
        transform.LookAt(lookPoint);
    }
}