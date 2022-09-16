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

public enum Condicion_Victoria
{
    Domino_for_Round,
    Domino_for_Points
}
public enum Score
{
    Classic,
    Double
}public enum Generator
{
    Classic,
    Crazy
}
public enum GameMode
{
    Classic,
    Longniza
}
public enum Distribution
{
    Classic,
    Primos
}


[DataContract]
public struct Opciones
{
    [DataMember]
    public Score score;
    [DataMember]
    public Generator generator;
    [DataMember]
    public GameMode gameMode;
    [DataMember]
    public Distribution distribution;
    [DataMember]
    public Condicion_Victoria condicionVictoria;

    public Opciones(Score score, Distribution distribution, Generator generator, GameMode gameMode, Condicion_Victoria condicionVictoria)
    {
        this.score = score;
        this.generator = generator;
        this.gameMode = gameMode;
        this.distribution = distribution;
        this.condicionVictoria = condicionVictoria;
    }
}


 