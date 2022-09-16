using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using DominoEngine;

public class ModalidadJuego : MonoBehaviour
{
    public IGameMode<int> classicGame;
    public IGameMode<int> longanizaGame;

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
        options.gameMode = GameMode.Classic;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }

    public void Longaniz()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        Opciones options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        options.gameMode = GameMode.Longniza;
        File.WriteAllText("opciones.txt", JsonConvert.SerializeObject(options));
    }
}

