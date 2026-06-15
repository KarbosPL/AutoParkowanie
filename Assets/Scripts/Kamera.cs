using UnityEngine;

public class Kamera : MonoBehaviour
{
    public Transform target; // AutoAgent
    public Vector3 offset = new Vector3(0f, 8f, -12f); // Wysoko i z tyłu

    void LateUpdate()
    {
        // Znajdź AutoAgent jeśli nie przypisany
        if (target == null)
        {
            GameObject agent = GameObject.Find("AutoAgent");
            if (agent != null)
                target = agent.transform;
            else
                return;
        }

        // Ustaw pozycję kamery
        transform.position = target.position + offset;

        // Kamera patrzy na auto
        transform.LookAt(target);
    }
}