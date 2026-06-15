using UnityEngine;

public class PieszySpawner : MonoBehaviour
{
    [Header("Ustawienia spawnera")]
    public GameObject pieszyPrefab; // Prefab pieszy
    public float spawnDistance = 30f; // W jakiej odległości przed autem spawnuje
    public float spawnOffset = 5f; // Przesunięcie w bok
    public float minSpawnTime = 10f;
    public float maxSpawnTime = 20f;
    
    private Transform target;
    private float spawnTimer;
    
    void Start()
    {
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent != null)
            target = agent.transform;
        
        // Losowy czas pierwszego spawnu
        spawnTimer = Random.Range(5f, minSpawnTime);
    }
    
    void Update()
    {
        if (target == null) return;
        
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer <= 0f)
        {
            SpawnPieszy();
            spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
    
    void SpawnPieszy()
    {
        if (pieszyPrefab == null)
        {
            // Stwórz pieszy prefab proceduralnie jeśli nie ma
            CreatePieszyPrefab();
        }
        
        // Oblicz pozycję przed autem
        Vector3 spawnPos = target.position + target.forward * spawnDistance;
        
        // Dodaj losowe przesunięcie w bok
        float sideOffset = Random.Range(-spawnOffset, spawnOffset);
        spawnPos += target.right * sideOffset;
        
        // Upewnij się że pieszy stoi na ziemi
        spawnPos.y = 0.5f;
        
        // Spawnuj pieszy
        GameObject nowyPieszy = Instantiate(pieszyPrefab, spawnPos, Quaternion.identity);
        nowyPieszy.name = "Pieszy";
        
        Debug.Log("Pieszy pojawił się na pozycji: " + spawnPos);
    }
    
    void CreatePieszyPrefab()
    {
        // Stwórz prosty model pieszy (kapsuła)
        pieszyPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        pieszyPrefab.name = "PieszyPrefab";
        pieszyPrefab.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        
        // Ustaw kolor
        Renderer r = pieszyPrefab.GetComponent<Renderer>();
        r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        r.material.color = new Color(0.2f, 0.5f, 0.2f); // Zielony
        
        // Dodaj skrypt pieszy
        pieszyPrefab.AddComponent<Pieszy>();
        
        // Usuń kolider (użyjemy własnego)
        Destroy(pieszyPrefab.GetComponent<Collider>());
        
        // Dodaj mały kolider
        CapsuleCollider col = pieszyPrefab.AddComponent<CapsuleCollider>();
        col.height = 1.5f;
        col.radius = 0.3f;
        
        Debug.Log("Stworzono prefab pieszy");
    }
}