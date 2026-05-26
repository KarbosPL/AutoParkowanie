using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        // Znajdź i ustaw AutoAgent
        GameObject agent = GameObject.Find("AutoAgent");
        if (agent) agent.transform.position = new Vector3(-1, 0.5f, 0);
        

        // Ustaw zaparkowane auta
        GameObject car1 = GameObject.Find("AutoZaparkowane_1");
        GameObject car2 = GameObject.Find("AutoZaparkowane_2");
        GameObject car3 = GameObject.Find("AutoZaparkowane_3");

        if (car1) car1.transform.position = new Vector3(4, 0.5f, 5);
        if (car2) car2.transform.position = new Vector3(4, 0.5f, 15);
        if (car3) car3.transform.position = new Vector3(4, 0.5f, 35);
    }
}