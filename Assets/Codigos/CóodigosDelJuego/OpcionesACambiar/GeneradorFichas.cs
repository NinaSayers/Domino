using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using DominoEngine;

public class GeneradorFichas : MonoBehaviour
{
    public IGenerator<int> generador;
    private int[] fichas = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Clasico()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.generator = Generator.Classic;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }

    public void Crazy()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.generator = Generator.Crazy;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }
}
