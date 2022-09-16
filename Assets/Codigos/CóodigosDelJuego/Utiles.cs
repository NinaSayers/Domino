using System.Collections;
using System.Globalization;
using System;
using System.ComponentModel;
using System.Security;
using System.Security.Authentication;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DominoEngine;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.IO;

public static class Utiles
{
    public static void Serialization()
    {
        int[] fichas = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        Score score = Score.Classic;
        Distribution distribution = Distribution.Classic;
        Generator generator = Generator.Classic;
        GameMode gameMode = GameMode.Classic;
        Opciones options = new Opciones(score, distribution, generator, gameMode, Condicion_Victoria.Domino_for_Round);
        //string opciones = JsonConvert.SerializeObject(options);
        string opciones = JsonUtility.ToJson(options);
        File.WriteAllText("./opciones.txt", opciones);
    }
}
