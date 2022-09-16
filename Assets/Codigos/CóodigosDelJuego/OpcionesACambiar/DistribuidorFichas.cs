using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using DominoEngine;

public class DistribuidorFichas : MonoBehaviour
{
    public IDistribution<int> distribution;
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
        options.distribution = Distribution.Classic;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }

    public void Primos()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.distribution = Distribution.Primos;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }
}
