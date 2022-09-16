using System.Globalization;
using System;
using System.ComponentModel;
using System.Security;
using System.Security.Authentication;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
namespace DominoEngine{
    public interface IGenerator <T> where T : IComparable { // tiene como parametro un numero fijo  
        public List<IFicha<T>> PieceGenerator();
    } 
    public interface IStrategy <T> where T : IComparable{ // Forma de Jugar de cada jugador 
        public (int, IFicha<T>) Play (State<T> state, List<IFicha<T>> Hand, Func<State<T>,IFicha<T>,bool> Filter_Piece);
    }
    public interface IDistribution <T> where T : IComparable { // Criterio de Distribucion ( se usa en la regla del juego ) 
        public List<IFicha<T>> DistributionOfPics(List<IFicha<T>> piece_of_the_game, int number_of_piece_for_player); // las reglas informaran el numero de fichas a reparartir por jugador 
    }
    public interface IGameMode <T> where T : IComparable { // Aqui ira el flujo del juego  
        public IEnumerable<State<T>> Game(IRules<T> rules,IReferee<T> referee ,Dictionary<int,T> places_available ,List<IPlayer<T>> players,List<IFicha<T>> piece_on_the_game,List<IFicha<T>> piece_on_the_board, List<IFicha<T>> piece_not_played);
    }
    public interface IScore <T> where T : IComparable { // Este es el criterio para indicar la puntuacion de cada jugador
        public int Score (List<IFicha<T>> hand);
    }
    public interface IRules<T> where T : IComparable{ // Objecto que contiene la mayoria de los criterios que conforman el juego
          
        public IEnumerable<State<T>> Game(IRules<T> rules,IReferee<T> referee ,Dictionary<int,T> places_available ,List<IPlayer<T>> players,List<IFicha<T>> piece_on_the_game,List<IFicha<T>> piece_on_the_board, List<IFicha<T>> piece_not_played);
        public List<IFicha<T>> Pack(); // Genera las fichas del juego 
        public List<IFicha<T>> Dispatcher(List<IFicha<T>> piece_on_the_board); // Despacha las fichas segun la cantidad 
        public void Update_The_Board ((int position,IFicha<T> piece) move ,Dictionary<int,T> places_avialable); // actualiza el tablero
        public bool IsValid((int position,IFicha<T> piece) move, State<T> state);
        public void EndGame(List<IPlayer<T>> players, IReferee<T> referee);
        public ICouple<T> Couple {get;}
        
    }
    
    public interface IPlayer<T> where T : IComparable{
        
        public string Name { get ;}
        public int Score {get ;}
        public IStrategy<T> WayToPlay{ get; }
        public List<IFicha<T>> Hand { get;} // Devuelve una copia de la mano 
        public (int, IFicha<T>) Play(State<T> state,Func<State<T>,IFicha<T>,bool> Filter_Piece);
        public int Numb_Of_Piece {get;}
        public void AddHand (List<IFicha<T>> piece_on_the_board, IRules<T> rules ); // Agrega una mano
        public bool Have(State<T> state,Func<State<T>,IFicha<T>,bool> Filter_Piece); 
    }
    public interface ICouple<T> where T : IComparable{ // Objecto que se encarga de abministrar los equipos
        public Dictionary<int,List<IPlayer<T>>> Teams_in_Smash { get;}
        public void Team(List<IPlayer<T>> players);
        public int ThisTeam(IPlayer<T> player);
    }
    public interface IFicha<T> where T : IComparable{
        public List<T> Valores{ get;}
        public int Value{get;}
        
    }
    public interface ITable<T> where T : IComparable{
        public void Time_Of_Distribution ();
        public IEnumerable<State<T>> Play(); 
        public void Resert();
        public int Number_Of_Players {get ;}
        public List<IFicha<T>> Game_Pieces {get;} // Todas las piezas de esa modalidad de juego
        public List<IFicha<T>> Pieces_On_The_Board { get ;}
        public List<IFicha<T>> Pieces_Not_Played {get; }


    }
    public interface IDomino<T> where T : IComparable{ // SuperClase
        public IEnumerable<State<T>> Play();
    }
    public interface IReferee<T> where T : IComparable{ // Su funcion es la de almacenar la informacion generada por cada ronda(puntucion, ganadores por ronda)
        public void EndSmash (IPlayer<T> player,ICouple<T> couple,long score);
        public bool Winner_this_count_of_rounds(int Max_Rounds);
        public bool Winner_this_count_of_points(long max_point, ICouple<T> couple);
        public List<IPlayer<T>> Winner_for_rounds (ICouple<T> couple, int max_rounds);
        public List<IPlayer<T>> Winner_for_points (ICouple<T> couple, long max_points);
        
    }
    public interface IWinner<T> where T : IComparable{ // Criterio para elegir el equipo ganador 
        public IPlayer<T> Winner(State<T> state, List<IPlayer<T>> player);
    }
}