using UnityEngine;

public class MapDispatcher : MonoBehaviour
{
    void Start()
    {
        // Wyczyść zapisaną mapę przy każdym uruchomieniu menu
        PlayerPrefs.DeleteKey("selectedMap");
        PlayerPrefs.Save();
        
        int map = PlayerPrefs.GetInt("selectedMap", 0);

        ScenaMapa1 m1 = GetComponent<ScenaMapa1>();
        ScenaMapa2 m2 = GetComponent<ScenaMapa2>();

        // Wyłącz wszystkie mapy
        if (m1) m1.enabled = false;
        if (m2) m2.enabled = false;

        // Włącz odpowiednią
        switch (map)
        {
            case 1: if (m1) m1.enabled = true; break;
            case 2: if (m2) m2.enabled = true; break;
            case 3: break; // ScenaMapa3 do zrobienia
        }
    }
}