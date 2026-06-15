using UnityEngine;

public class MapDispatcher : MonoBehaviour
{
    void Start()
    {
        int map = PlayerPrefs.GetInt("selectedMap", 0);

        ScenaMapa1 m1 = GetComponent<ScenaMapa1>();
        ScenaMapa2 m2 = GetComponent<ScenaMapa2>();
        ScenaMapa3 m3 = GetComponent<ScenaMapa3>();
        
        if (m1) m1.enabled = false;
        if (m2) m2.enabled = false;
        if (m3) m3.enabled = false;
        
        switch (map)
        {
            case 1: if (m1) m1.enabled = true; break;
            case 2: if (m2) m2.enabled = true; break;
            case 3: if (m3) m3.enabled = true; break;
        }
    }
}