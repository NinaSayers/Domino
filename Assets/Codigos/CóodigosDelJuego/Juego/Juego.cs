using System;
using System.Collections.Generic;
using UnityEngine;
using DominoEngine;
using System.IO;
using TMPro;

public class Juego : MonoBehaviour
{
    int index;
    int max;
    public TMP_Text label;
    State<int> state;
    public Opciones options;
    public ICouple<int> couples;
    public IReferee<int> referee;
    public IGameMode<int> modalidadJuego;
    public IGenerator<int> generator;
    public IDistribution<int> distribution;
    public IScore<int> score;
    public IRules<int> reglas;
    public IDomino<int> domino_for_point;
    public IDomino<int> domino_for_round;
    public IEnumerator<State<int>> states;
    //public Table<int> table;

    IStrategy<int> fat_player = new Fat_Player<int>();
    IStrategy<int> random_player = new Random_Player<int>();
    IStrategy<int> standart_player = new Standart_Player<int>();
    public IPlayer<int> player1;
    public IPlayer<int> player2;
    public IPlayer<int> player3;
    public IPlayer<int> player4;
    List<IPlayer<int>> players;
    public bool noentro;



    public bool IsDominoForRounds = true;
    public bool IsDominoForPoints = false;


    // Start is called before the first frame update
    void Start()
    {
        states = Jugar().GetEnumerator();
        index = 0;
        noentro = true;
        max = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (states.MoveNext())
        {
            Debug.Log(states.Current.Piece_On_The_Board.Count);
            if (state.Equals(default(State<int>)))
            {
                Debug.Log(players[index].Name);
                label.text += "Comienza el juego!".ToUpper() + "\n" + players[index].Name + " ha jugado el " + "\n[" + states.Current.Piece_On_The_Board[0].Valores[0].ToString() + " - " + states.Current.Piece_On_The_Board[0].Valores[1].ToString() + "]" + ".";
            }
            else if (states.Current.Piece_On_The_Board.Count == state.Piece_On_The_Board.Count)
            {
                label.text += "\n" + players[index].Name + " se pasó!";
            }
            else
            {
                label.text += "\n" + players[index].Name + " ha jugado el " + "\n[" + states.Current.Piece_On_The_Board[states.Current.Piece_On_The_Board.Count - 1].Valores[0].ToString() + " - " + states.Current.Piece_On_The_Board[states.Current.Piece_On_The_Board.Count - 1].Valores[1].ToString() + "]" + ".";
            }

            if (index < 3) index++;
            else index = 0;
            state = states.Current;
        }

        else if (noentro)
        {
            noentro = false;
            //label.text += "\n El ganador de esta ronda es " + reglas.EndGame(players, referee).Name;
            max++;
            if (IsDominoForRounds)
            {
                //if (referee.Winner_this_count_of_rounds(1))
                //{
                    List<IPlayer<int>> team_winner = referee.Winner_for_rounds(reglas.Couple, 1);
                    label.text += "\n ¡el juego finalizó!".ToUpper() + "\n Los ganadores son : " + team_winner[0].Name;
                //}
                // else
                // {
                //     label.text += "\n ¡otra ronda!".ToUpper();
                //     Start();
                //     //states = Jugar().GetEnumerator();
                //     // index = 0;
                //     // noentro = true;
                //     // max = 0;
                //     //Update();
                // }

            }
            else if (IsDominoForPoints)
            {
                //if (referee.Winner_this_count_of_points(5, couples))
                //{
                    List<IPlayer<int>> team_winner = referee.Winner_for_points(reglas.Couple, 5);
                    label.text += "\n ¡el juego fonalizó!".ToUpper() + "\n Los ganadores son : " /*+ team_winner.Count.ToString()*/ + team_winner[0].Name/*.ToString()*/;
                //}
                //  else
                // {
                //     label.text += "\n ¡otra ronda!".ToUpper();
                //     Start();
                //     //states = Jugar().GetEnumerator();
                //     // index = 0;
                //     // noentro = true;
                //     // max = 0;
                //     //Update();
                // }
            }
        }
    }

    public IEnumerable<State<int>> Jugar()
    {
        if (!File.Exists("./opciones.txt")) Utiles.Serialization();
        options = JsonUtility.FromJson<Opciones>(File.ReadAllText("./opciones.txt"));
        IDomino<int> domino = PreparacionJuego();
        return domino.Play();

    }


    //public void Pausar() // IMPLEMENTAR*************
    //{ }
    //public void Reanudar()// IMPLEMENTAR*************
    //{ }
    public Rules<int> Reglas(ICouple<int> couples, IGameMode<int> modalidadJuego, IGenerator<int> generator, IDistribution<int> distribution, IScore<int> score, int fichas) //Crear las reglas el juego
    {
        Rules<int> reglas;
        return reglas = new Rules<int>(modalidadJuego, generator, distribution, score, couples, fichas);
    }

    public Referee<int> Arbitro(List<IPlayer<int>> players, ICouple<int> couples) //Crear el referee del juego
    {
        Referee<int> arbitro;
        return arbitro = new Referee<int>(players, couples.Teams_in_Smash);
    }

    public List<IPlayer<int>> Players() //Crear lista de jugadores con sus estrategias
    {
        players = new List<IPlayer<int>>();
        this.player1 = new Player<int>("Player1", standart_player);
        this.player2 = new Player<int>("Player2", fat_player);
        this.player3 = new Player<int>("Player3", fat_player);
        this.player4 = new Player<int>("Player4", random_player);

        players.Add(player1);
        players.Add(player2);
        players.Add(player3);
        players.Add(player4);
        return players;
    }

    public Table<int> Mesaa(IReferee<int> arbitro, List<IPlayer<int>> players, Rules<int> reglas) //Crear Mesa-tablero
    {
        Table<int> mesa = new Table<int>(reglas, arbitro, players);
        return mesa;
    }

    public IGameMode<int> ModalidadJuego()
    {
        IGameMode<int> game_mode;
        switch (options.gameMode)
        {
            case GameMode.Classic:
                game_mode = new ClassicGame<int>();
                break;
            case GameMode.Longniza:
                game_mode = new LonganizaGame<int>();
                break;
            default:
                throw new NotImplementedException();
        }
        return game_mode;
    }
    public IGenerator<int> GeneradorFichas()
    {
        int[] fichas = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        IGenerator<int> generator;
        switch (options.generator)
        {
            case Generator.Classic:
                generator = new ClassicGenerator<int>(fichas);
                break;
            case Generator.Crazy:
                generator = new CrazyGenerator<int>(fichas);
                break;
            default:
                throw new NotImplementedException();
        }
        return generator;
    }
    public IDistribution<int> DistribucionFichas()
    {
        IDistribution<int> distribution;
        switch (options.distribution)
        {
            case Distribution.Classic:
                distribution = new ClassicDistribution<int>();
                break;
            case Distribution.Primos:
                distribution = new CousinDistribution<int>(DateTime.Now.Millisecond);
                break;
            default:
                throw new NotImplementedException();
        }
        return distribution;
    }
    public IScore<int> PuntuacionFichas()
    {
        IScore<int> score;
        switch (options.score)
        {
            case Score.Classic:
                score = new ClassicScore<int>();
                break;
            case Score.Double:
                score = new DoubleScore<int>();
                break;
            default:
                throw new NotImplementedException();
        }
        return score;
    }
    public IDomino<int> CondicionVictoria(IRules<int> reglas, IReferee<int> referee, List<IPlayer<int>> players)
    {
        IDomino<int> domino;
        switch (options.condicionVictoria)
        {
            case Condicion_Victoria.Domino_for_Round:
                this.IsDominoForRounds = true;
                this.IsDominoForPoints = false;
                domino = new Domino_for_Rounds<int>(reglas, referee, players, 1);
                this.domino_for_round = domino;
                break;
            case Condicion_Victoria.Domino_for_Points:
                this.IsDominoForRounds = false;
                this.IsDominoForPoints = true;
                domino = new Domino_for_Point<int>(reglas, referee, players, 5);
                this.domino_for_point = domino;
                break;
            default:
                throw new NotImplementedException();
        }
        return domino;
    }
    public ICouple<int> Couples(List<IPlayer<int>> players, int integrantes)
    {
        ICouple<int> couples;
        couples = new Couples<int>(integrantes);
        couples.Team(players);
        return couples;
    }

    public IDomino<int> PreparacionJuego()
    {
        // //doble nueve
        // int[] Cuantas9 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        // //doble sies 
        // int[] Cuantas6 = { 0, 1, 2, 3, 4, 5, 6 };
        // //numeros primos
        // int[] NumerosPrimos_doble6 = { 2, 3, 5, 7, 11, 13, 17 };
        // int a = 1;
        // System.Console.WriteLine(Equals(1, a));
        // IScore<int> classic_score = new ClassicScore<int>();
        // IDistribution<int> classic_distribution = new ClassicDistribution<int>();
        // IGenerator<int> classic_generator = new ClassicGenerator<int>(Cuantas9);
        // IGameMode<int> classic_game = new ClassicGame<int>();
        // IGameMode<int> longaniza_game = new LonganizaGame<int>();
        // IStrategy<int> fact_player = new Fat_Player<int>();
        // IStrategy<int> random_player = new Random_Player<int>();
        // IStrategy<int> standart_player = new Standart_Player<int>();


        // player1 = new Player<int>("Adriana", standart_player);
        // player2 = new Player<int>("Gilbert", fact_player);
        // player3 = new Player<int>("Cusco", fact_player);
        // player4 = new Player<int>("Nubia", random_player);
        // players = new List<IPlayer<int>>();


        // players.Add(player1);
        // players.Add(player2);
        // players.Add(player3);
        // players.Add(player4);

        // ICouple<int> couple = new Couples<int>(1);
        // couple.Team(players);
        // IReferee<int> referee = new Referee<int>(players, couple.Teams_in_Smash);
        // IRules<int> rules = new Rules<int>(classic_game, classic_generator, classic_distribution, classic_score, couple, 10);
        // //Table<int> table = new Table<int>(rules,referee,players);
        // Domino_for_Rounds<int> domino_rounds = new Domino_for_Rounds<int>(rules, referee, players, 1);
        // Domino_for_Point<int> domino_score = new Domino_for_Point<int>(rules, referee, players, 5);
        // //table.Start();
        IDomino<int> domino;
        List<IPlayer<int>> players = Players();
        couples = Couples(players, 1);
        referee = Arbitro(players, couples);
        modalidadJuego = ModalidadJuego();
        generator = GeneradorFichas();
        distribution = DistribucionFichas();
        score = PuntuacionFichas();
        reglas = Reglas(couples, modalidadJuego, generator, distribution, score, 10);
        domino = CondicionVictoria(reglas, referee, players);

        /*if(IsDominoForRounds) return domino_rounds;
        else*/
        return domino;
    }
}


