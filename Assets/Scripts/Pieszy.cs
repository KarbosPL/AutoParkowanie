using UnityEngine;

public class Pieszy : MonoBehaviour
{
    [Header("Ruch")]
    public float walkSpeed = 2f;
    public float stopDistance = 3f; // Zatrzymaj się 3m przed autem
    public float waitTime = 3f;     // Czas zatrzymania
    
    [Header("Po zniknięciu")]
    public float hideDistance = 10f; // Jak daleko w bok się przesunie
    
    private Transform target; // AutoAgent
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool isHiding = false;
    private Vector3 hideDirection;
    
    void Start()
    {
        // Znajdź AutoAgent
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent != null)
            target = agent.transform;
        else
            Destroy(gameObject); // Usuń pieszy jeśli nie ma auta
    }
    
    void Update()
    {
        if (target == null) return;
        
        // Oblicz odległość do auta
        float distance = Vector3.Distance(transform.position, target.position);
        
        // Jeśli jesteśmy w fazie chowania się
        if (isHiding)
        {
            transform.Translate(hideDirection * walkSpeed * Time.deltaTime, Space.World);
            
            // Sprawdź czy jesteśmy wystarczająco daleko od auta
            if (distance > hideDistance)
            {
                Destroy(gameObject); // Zniknij
            }
            return;
        }
        
        // Jeśli czekamy
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                // Wybierz kierunek ucieczki (w bok)
                HideAway();
            }
            return;
        }
        
        // Sprawdź czy jesteśmy za blisko auta
        if (distance < stopDistance)
        {
            // Zatrzymaj się
            isWaiting = true;
            waitTimer = waitTime;
            return;
        }
        
        // Idź w stronę auta
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Trzymaj się ziemi
        transform.Translate(direction * walkSpeed * Time.deltaTime, Space.World);
        
        // Obróć w stronę auta
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
    
    void HideAway()
    {
        isHiding = true;
        
        // Wybierz losowy kierunek w bok (lewo lub prawo)
        float sideDirection = Random.Range(-1f, 1f) > 0 ? 1f : -1f;
        
        // Użyj prawej/lewej osi auta (nie pieszy)
        Vector3 autoForward = target.forward;
        Vector3 autoRight = target.right;
        
        // Kierunek ucieczki: lekko do przodu i w bok
        hideDirection = (autoForward * 0.5f + autoRight * sideDirection).normalized;
        
        Debug.Log("Pieszy ucieka w stronę: " + hideDirection);
    }
}