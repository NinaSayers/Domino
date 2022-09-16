using UnityEngine;

public class BotónSalir : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Salir();
    }

    public void Salir()
    {
        Application.Quit();
    }
}
