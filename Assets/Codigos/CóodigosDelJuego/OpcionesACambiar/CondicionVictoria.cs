using UnityEngine;
using Newtonsoft.Json;
using System.IO;
public class CondicionVictoria : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Ronda()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.condicionVictoria = Condicion_Victoria.Domino_for_Round;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }

    public void Puntos()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.condicionVictoria = Condicion_Victoria.Domino_for_Points;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }
   }

