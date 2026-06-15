using UnityEngine;

public class PieszySpawner : MonoBehaviour
{
    public GameObject pieszyPrefab;
    public float spawnDistance = 25f;
    
    private Transform target;
    private int spawnCount = 0;
    private bool firstSpawned = false;
    private bool secondSpawned = false;
    
    void Start()
    {
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent != null)
            target = agent.transform;
        
        // Spawn pierwszego pieszy po 5 sekundach
        Invoke("SpawnFirst", 5f);
    }
    
    void SpawnFirst()
    {
        if (target == null) return;
        
        SpawnPieszy();
        spawnCount = 1;
        firstSpawned = true;
        Debug.Log("Pieszy #1 pojawił się");
        
        // Spawn drugiego pieszy po 20 sekundach
        Invoke("SpawnSecond", 200f);
    }
    
    void SpawnSecond()
    {
        if (target == null) return;
        
        SpawnPieszy();
        spawnCount = 2;
        secondSpawned = true;
        Debug.Log("Pieszy #2 pojawił się");
    }
    
    void SpawnPieszy()
    {
        if (pieszyPrefab == null)
        {
            // Stwórz tymczasowy prefab
            pieszyPrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            pieszyPrefab.name = "PieszyPrefab";
            pieszyPrefab.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            
            Renderer r = pieszyPrefab.GetComponent<Renderer>();
            r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            r.material.color = Color.green;
            
            pieszyPrefab.AddComponent<Pieszy>();
            
            Destroy(pieszyPrefab.GetComponent<Collider>());
            CapsuleCollider col = pieszyPrefab.AddComponent<CapsuleCollider>();
            col.height = 1.5f;
        }
        
        // Losuj stronę (lewo lub prawo)
        float side = Random.Range(-1f, 1f) > 0 ? 1f : -1f;
        
        // Oblicz pozycję przed autem i na boku
        Vector3 spawnPos = target.position + target.forward * spawnDistance + target.right * side * 3f;
        spawnPos.y = 0.5f;
        
        GameObject nowyPieszy = Instantiate(pieszyPrefab, spawnPos, Quaternion.identity);
        nowyPieszy.name = "Pieszy";
    }
}