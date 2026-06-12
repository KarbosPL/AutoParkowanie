using UnityEngine;

public class ScenaMapa3 : MonoBehaviour
{
    void Start()
    {
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent)
        {
            agent.transform.position = new Vector3(0, 0.5f, -45f);
            agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetColor(agent, new Color(0.9f, 0.9f, 0.1f));

            // Wyłącz inne FSM
            ParkingFSM_Rownolegle rownolegle = agent.GetComponent<ParkingFSM_Rownolegle>();
            if (rownolegle != null) rownolegle.enabled = false;
            
            ParkingFSM_Prostopadle prostopadle = agent.GetComponent<ParkingFSM_Prostopadle>();
            if (prostopadle != null) prostopadle.enabled = false;
            
            // Dodaj i włącz FSM skośne jeśli nie istnieje
            ParkingFSM_Skosnie skosnie = agent.GetComponent<ParkingFSM_Skosnie>();
            if (skosnie == null)
            {
                skosnie = agent.AddComponent<ParkingFSM_Skosnie>();
                Debug.Log("Dodano komponent ParkingFSM_Skosnie do AutoAgent");
            }
            skosnie.enabled = true;
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu AutoAgent!");
            return;
        }

        // Miejsca parkingowe co 5 jednostek od -45 do 45
        float[] miejscaZ = { -45f, -40f, -35f, -30f, -25f, -20f, -15f, -10f, -5f, 0f, 5f, 10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f };
        
        Color[] kolory = new Color[]
        {
            new Color(0.8f, 0.1f, 0.1f),
            new Color(0.1f, 0.3f, 0.8f),
            new Color(0.1f, 0.7f, 0.2f),
            new Color(0.3f, 0.1f, 0.5f),
            new Color(0.9f, 0.4f, 0.1f),
            new Color(0.1f, 0.5f, 0.5f),
            new Color(0.6f, 0.2f, 0.2f),
            new Color(0.2f, 0.6f, 0.3f),
            new Color(0.5f, 0.5f, 0.1f),
            new Color(0.8f, 0.3f, 0.6f)
        };
        
        int licznikPrawo = 0;
        int licznikLewo = 0;
        
        for (int i = 0; i < miejscaZ.Length; i++)
        {
            // Przesunięcie o połowę szerokości miejsca (2.5f)
            float z = miejscaZ[i] - 2.5f;
            
            // Prawa strona - ZWIĘKSZONE X o 0,5 (z 6f na 6.5f)
            Color kolorPrawo = kolory[licznikPrawo % kolory.Length];
            CreateCar($"AutoZaparkowane_Prawa_{i}", new Vector3(6.5f, 0.5f, z), kolorPrawo, 45f);
            licznikPrawo++;
            
            // Lewa strona - ZWIĘKSZONE X o 0,5 (z -6f na -6.5f)
            Color kolorLewo = kolory[licznikLewo % kolory.Length];
            CreateCar($"AutoZaparkowane_Lewa_{i}", new Vector3(-6.5f, 0.5f, z), kolorLewo, -45f);
            licznikLewo++;
        }
        
        GameObject planeObj = GameObject.Find("Plane");
        if (planeObj != null)
        {
            // Usuń starą teksturę jeśli istnieje
            ParkingTexture3 existingTexture = planeObj.GetComponent<ParkingTexture3>();
            if (existingTexture != null) Destroy(existingTexture);
            
            // Dodaj nową
            planeObj.AddComponent<ParkingTexture3>();
        }
        else
        {
            Debug.LogError("Nie znaleziono obiektu Plane!");
        }
    }

    void CreateCar(string name, Vector3 pos, Color color, float rotationY)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            existing.transform.position = pos;
            existing.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            SetColor(existing, color);
            return;
        }
        GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
        car.name = name;
        car.transform.position = pos;
        car.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        car.transform.localScale = new Vector3(2f, 1f, 4.5f);
        SetColor(car, color);
    }

    void SetColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            r.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            r.material.color = color;
        }
    }
}