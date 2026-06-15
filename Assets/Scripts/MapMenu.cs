using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMenu : MonoBehaviour
{
    void Start()
    {
        // Reset tylko przy pierwszym uruchomieniu gry
        if (!PlayerPrefs.HasKey("gameStarted"))
        {
            PlayerPrefs.SetInt("selectedMap", 0);
            PlayerPrefs.SetInt("gameStarted", 1);
        }
        
        // WYŁĄCZ KAMERĘ NA STARCIE (w menu)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Kamera kam = mainCam.GetComponent<Kamera>();
            if (kam != null)
                kam.enabled = false;
        }
    }

    void OnGUI()
    {
        int map = PlayerPrefs.GetInt("selectedMap", 0);

        if (map == 0)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height),
                Texture2D.blackTexture);
            DrawMenu();
        }
        else
        {
            DrawHUD();
        }
    }

    void DrawMenu()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fontSize = 28;
        title.normal.textColor = Color.white;
        title.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(w/2 - 150, h/2 - 160, 300, 50), "Auto Parkowanie", title);

        GUIStyle btn = new GUIStyle(GUI.skin.button);
        btn.fontSize = 18;

        if (GUI.Button(new Rect(w/2 - 120, h/2 - 80, 240, 50), "Mapa 1 - Równoległe", btn))
            LoadMap(1);
        if (GUI.Button(new Rect(w/2 - 120, h/2 - 10, 240, 50), "Mapa 2 - Prostopadłe", btn))
            LoadMap(2);
        if (GUI.Button(new Rect(w/2 - 120, h/2 + 60, 240, 50), "Mapa 3 - Skośne", btn))
            LoadMap(3);
    }

    void DrawHUD()
    {
        GUIStyle btn = new GUIStyle(GUI.skin.button);
        btn.fontSize = 14;

        if (GUI.Button(new Rect(10, 10, 160, 36), "← Wróć do menu", btn))
        {
            PlayerPrefs.SetInt("selectedMap", 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        GUIStyle lbl = new GUIStyle(GUI.skin.label);
        lbl.fontSize = 14;
        lbl.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 52, 200, 24),
            $"Mapa {PlayerPrefs.GetInt("selectedMap", 0)}", lbl);
    }

    void LoadMap(int map)
    {
        PlayerPrefs.SetInt("selectedMap", map);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnApplicationQuit()
    {
        // Reset przy zamknięciu gry
        PlayerPrefs.DeleteKey("gameStarted");
        PlayerPrefs.SetInt("selectedMap", 0);
    }
}