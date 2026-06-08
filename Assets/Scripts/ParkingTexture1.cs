using UnityEngine;

public class ParkingTexture1 : MonoBehaviour
{
    
    void Start()
    {
        Texture2D tex = Generate();
        Renderer r = GetComponent<Renderer>();
        if (r == null) return;
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.mainTexture = tex;
        r.material = mat;
    }

    public static Texture2D parkingTexture;
    
    Texture2D Generate()
    {
        int W = 2048, H = 2048;
        Texture2D tex = new Texture2D(W, H);

        Color asfaltAlejka  = new Color(0.22f, 0.22f, 0.22f);
        Color asfaltParking = new Color(0.15f, 0.15f, 0.15f);
        Color linia         = Color.white;

        // Plane Scale=10 → Unity units X: -50..50, Z: -50..50
        // Mapowanie: texX = (unityX + 50) / 100 * W
        //            texY = (unityZ + 50) / 100 * H

        // Wypełnij całość asfaltem parkingowym
        Fill(tex, 0, 0, W, H, asfaltParking);

        // Alejka: Unity X = -2.5 do 2.5
        int aleL = ToTexX(W, -2.5f);
        int aleR = ToTexX(W,  2.5f);
        Fill(tex, aleL, 0, aleR - aleL, H, asfaltAlejka);

        // Krawężniki (linie oddzielające alejkę od parkingów)
        //DrawV(tex, aleL, 0, H, linia, 5);
        //DrawV(tex, aleR, 0, H, linia, 5);

        // Linia tylna prawego parkingu: Unity X = 10
        DrawV(tex, ToTexX(W, 6), 0, H, linia, 4);
        // Linia tylna lewego parkingu: Unity X = -10
        DrawV(tex, ToTexX(W, -6f), 0, H, linia, 4);

        // Linie między miejscami parkingowymi (wzdłuż Z)
        // Miejsca co 8 jednostek: Z = -2, 6, 14, 22, 30, 38
        float[] zLines = { -40f, -30f, -20f, -10f, 0f, 10f, 20f, 30f, 40f };
        int parkPL = ToTexX(W, 2.5f);   // lewa krawędź prawego parkingu
        int parkPR = ToTexX(W, 6f);    // prawa krawędź prawego parkingu
        int parkLL = ToTexX(W, -6f);   // lewa krawędź lewego parkingu
        int parkLR = ToTexX(W, -2.5f);  // prawa krawędź lewego parkingu

        foreach (float z in zLines)
        {
            int texY = ToTexY(H, z);
            DrawH(tex, texY, parkPL, parkPR, linia, 4);
            DrawH(tex, texY, parkLL, parkLR, linia, 4);
        }

        tex.Apply();
        parkingTexture = tex;
        return tex;
    }

    int ToTexX(int W, float unityX) => (int)((unityX + 50f) / 100f * W);
    int ToTexY(int H, float unityZ) => (int)((unityZ + 50f) / 100f * H);

    void Fill(Texture2D tex, int x, int y, int w, int h, Color c)
    {
        for (int px = x; px < x + w && px < tex.width;  px++)
        for (int py = y; py < y + h && py < tex.height; py++)
            tex.SetPixel(px, py, c);
    }

    void DrawV(Texture2D tex, int x, int yStart, int yEnd, Color c, int thickness)
    {
        for (int t = -thickness/2; t <= thickness/2; t++)
        {
            int px = Mathf.Clamp(x + t, 0, tex.width - 1);
            for (int py = yStart; py < yEnd; py++)
                tex.SetPixel(px, py, c);
        }
    }

    void DrawH(Texture2D tex, int y, int xStart, int xEnd, Color c, int thickness)
    {
        for (int t = -thickness/2; t <= thickness/2; t++)
        {
            int py = Mathf.Clamp(y + t, 0, tex.height - 1);
            for (int px = xStart; px < xEnd; px++)
                tex.SetPixel(px, py, c);
        }
    }
}