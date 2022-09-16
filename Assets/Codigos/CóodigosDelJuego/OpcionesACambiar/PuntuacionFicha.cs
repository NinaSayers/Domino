using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using DominoEngine;
public class PuntuacionFicha : MonoBehaviour
{
    public IScore<int> score;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Clasica()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.score = Score.Classic;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }

    public void DoubleScore()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.score = Score.Double;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }
}

