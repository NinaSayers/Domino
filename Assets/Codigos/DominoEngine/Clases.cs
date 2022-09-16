using System.Runtime.CompilerServices;
using System.Globalization;
using System;
using System.ComponentModel;
using System.Security;
using System.Security.Authentication;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
namespace DominoEngine{
    
    public static class Extensores
    {
        public static List<T> Clone<T>(this List<T> lista){
            List<T> newlist = new List<T>();
            foreach (var item in lista)
            {
                newlist.Add(item);
            }
            return newlist;
        }
    }
    public class Domino_for_Rounds <T> : IDomino<T> where T: IComparable{ //Esta clase controla el juego por rondas
        public ITable<T> table; 
        private IRules<T> rules;  
        private List<IPlayer<T>> players; 
        private IReferee<T> referee;
        public int max_rounds;

        public Domino_for_Rounds (IRules<T> rules, IReferee<T> referee, List<IPlayer<T>> players,int Max_Rounds){
            this.table = new Table<T>(rules,referee,players);
            this.rules = rules;
            this.players = players;
            this.referee = referee;
            this.max_rounds = Max_Rounds;
        }
        public IEnumerable<State<T>> Play (){
            while(!this.referee.Winner_this_count_of_rounds(this.max_rounds)){
                foreach(var state in this.table.Play())
                    yield return state;
                //this.table.Resert(); 
            }
            
            List<IPlayer<T>> team_winner= referee.Winner_for_rounds(rules.Couple,max_rounds);
            System.Console.WriteLine("Los ganadores son : {0} {1}",team_winner.Count,team_winner[0].Name);   

            for (var i = 0; i < team_winner.Count; i++){
                Console.Write(team_winner[i].Name);
            }
        }
    }
    public class Domino_for_Point <T> : IDomino<T> where T: IComparable{ //Esta clase controla el juego por puntos
        
        public ITable<T> table;
        private IRules<T> rules;
        private List<IPlayer<T>> players;
        private IReferee<T> referee;
        public long max_point;
        public Domino_for_Point (IRules<T> rules, IReferee<T> referee, List<IPlayer<T>> players,long max_point ){
            this.table = new Table<T>(rules,referee,players);
            this.rules = rules;
            this.players = players;
            this.referee = referee;
            this.max_point = max_point;
        }
        public IEnumerable<State<T>> Play(){
            while(!this.referee.Winner_this_count_of_points(max_point,rules.Couple)){
                foreach(var state in this.table.Play())
                    yield return state;
                this.table.Resert(); 
                //throw new Exception("exploto");
            }
            
            /*List<IPlayer<T>> team_winner = referee.Winner_for_points(rules.Couple,max_point);
            System.Console.WriteLine("Los ganadores son : {0} {1}",team_winner.Count,team_winner[0].Name);   
            for (var i = 0; i < team_winner.Count; i++){
                Console.Write(team_winner[i].Name);
            }*/
        }
    }
    #region Fundamentales
    public struct State <T> where T : IComparable{ //Estado del juego 
        List<IFicha<T>> piece_on_the_game;
        List<IFicha<T>> piece_on_the_board;
        List<IFicha<T>> piece_not_played;
        Dictionary<int,T> places_available;
        //List<StatePlayer> players;
        public State(Dictionary<int,T> places_available, List<IFicha<T>> piece_on_the_game,List<IFicha<T>> piece_on_the_board, List<IFicha<T>> piece_not_played){ // recibe las piezas jugadas , las posibles piezas que tiene la modalidad de juego y las piezas no jugadas 
            this.piece_on_the_game = piece_on_the_game.Clone<IFicha<T>>();
            this.piece_on_the_board = piece_on_the_board.Clone<IFicha<T>>();
            this.piece_not_played = piece_not_played.Clone<IFicha<T>>();
            this.places_available = places_available;
        }
        public List<IFicha<T>> Game_Pieces {get {return this.piece_on_the_game;}} // Todas las fichas del juego
        public List<IFicha<T>> Piece_Not_Played { get{ return this.piece_not_played;}} // Turno del juego
        public List<IFicha<T>> Piece_On_The_Board { get{ return this.piece_on_the_board ;}} // Las fichas jugadas (estan en la mesa)
        public Dictionary<int,T> Places_Available{ get { return this.places_available;}}
     }
    public class Table<T> : ITable<T> where T : IComparable // Aqui se desarrolla el juego
    {   // Se descarta el tipo board
        private List<IFicha<T>> pieces_of_game;
        private List<IFicha<T>> pieces_on_the_board;
        private List<IFicha<T>> pieces_not_played; 
        private List<IPlayer<T>> players;
        private Dictionary<int,T> available_place; // Lugares disponibles para jugar 
        private IReferee<T> referee; // Arbitro
        private IRules<T> rules; // Reglas Del Juego Diferente a las Estrategias de los jugadores
        public List<IFicha<T>> Game_Pieces {get {return this.pieces_of_game; }} // Todas las fichas del juego
        public List<IFicha<T>> Pieces_On_The_Board { get{ return this.pieces_on_the_board;}} // Las fichas jugadas (estan en la mesa)
        public List<IFicha<T>> Pieces_Not_Played { get{ return this.pieces_not_played;}} // Turno del juego
        public IRules<T> Rules { get { return this.rules;}}
        public int Number_Of_Players {get {return this.players.Count;}}
        public Table(IRules<T> rules,IReferee<T> referee, List<IPlayer<T>> players){
            this.pieces_of_game = new List<IFicha<T>>();
            this.pieces_on_the_board = new List<IFicha<T>>();
            pieces_not_played = new List<IFicha<T>>();
            this.rules = rules; 
            this.referee = referee;
            this.players = players;
            this.available_place = new Dictionary<int, T>();
            this.pieces_of_game = this.rules.Pack(); // Genero las fichas

            /*foreach(var x in this.pieces_of_game){
                System.Console.WriteLine($" ficha  dnetro [{String.Join(",", x.Valores)}]");
            }*/
            Time_Of_Distribution(); // Despacho a los jugadores 
        }
        public void Time_Of_Distribution(){   
            for(int i = 0; i < this.players.Count; i++){
                this.players[i].AddHand(this.rules.Dispatcher(this.Game_Pieces), this.Rules); // Cada jugador recibe una mano
                //System.Console.WriteLine("Le toca a otro cantidad de fichas {0}",Game_Pieces.Count);
            }
            
        }
        public IEnumerable<State<T>> Play() // Flujo del juego
        {
            return this.rules.Game(this.rules,this.referee,this.available_place,this.players,this.pieces_of_game,this.pieces_on_the_board,this.pieces_not_played);
        }
        public void Resert(){
            this.pieces_on_the_board = new List<IFicha<T>>();
            this.pieces_not_played = new List<IFicha<T>>();
            this.available_place = new Dictionary<int, T>();
            this.pieces_of_game = this.rules.Pack(); // Genero las fichas 
            Time_Of_Distribution(); // Despacho la mano de cada jugador 
        }
        
    }
    public class Ficha<T> : IFicha<T> where T : IComparable{ // Esta clase solo informa sobre su valor
        private List<T> value;
        public Ficha(List<T> value){
            this.value = value;
        }
        public List<T> Valores { get {return this.value;}} //Devuelve una lista con los valores es cada cara de la ficha
        public int Value { get{

            int aux = 0;
            for(int i = 0; i < this.value.Count; i++){
                aux += value[i].GetHashCode();
            } 
            return aux;
        }} // Pendiente a implementar. Este criterio otorga valor a una ficha como pieza
    }
    public class Rules<T> : IRules<T> where T : IComparable {//Es objeto contiene la mayoria de los criterios a utilizar en el juego
        
        private IGameMode<T> game_mode; // tipo de juego,aqui es donde va el flujo 
        private IDistribution<T> distribution; // Manera de repartir las fichas
        private IGenerator<T> generator; // generador de fichas
        private IScore<T> score; // 
        private ICouple<T> couples; // La forma de recoger las parejas del juego
        private int number_of_piece_for_player; // cantidad de fichas por jugador 

        public ICouple<T> Couple {get {return this.couples;}}

        public Rules(IGameMode<T> modality_of_game,IGenerator<T> generator,IDistribution<T> distribution, IScore<T> score,ICouple<T> couples,int number_of_piece_for_player){
            this.game_mode = modality_of_game;
            this.distribution = distribution;
            this.number_of_piece_for_player = number_of_piece_for_player;
            this.generator = generator;
            this.couples = couples;
            this.score = score;
        }
        public List<IFicha<T>> Dispatcher(List<IFicha<T>> piece_on_the_board){
            return this.distribution.DistributionOfPics(piece_on_the_board,this.number_of_piece_for_player); //Despacha las fichas del juego segun la cantidad suministrada en la construccion de la regla  
        }

        public IEnumerable<State<T>> Game(IRules<T> rules,IReferee<T> referee , Dictionary<int,T> places_available, List<IPlayer<T>> players, List<IFicha<T>> piece_on_the_game, List<IFicha<T>> piece_on_the_board, List<IFicha<T>> piece_not_played)
        {
            return this.game_mode.Game(rules,referee,places_available,players,piece_on_the_game,piece_on_the_board,piece_not_played);
        }

        public List<IFicha<T>> Pack(){ // Genera las Fichas del juego 
            return this.generator.PieceGenerator();
        }
        public void Update_The_Board((int position, IFicha<T> piece) move, Dictionary<int, T> places_avialable){
            if (places_avialable.Count != 0){ // El tablero no esta vacio
                T old_piece = places_avialable[move.position];
                
                List<T> newPieces = new List<T>();//paso una copia de la Ficha
                for(int i = 0; i < move.piece.Valores.Count;i++)
                    newPieces.Add(move.piece.Valores[i]);
                //Se hace una copia 

                newPieces.Remove(old_piece);//remuevo de la copia el valor x el q voy a jugar
                places_avialable[move.position] = newPieces[newPieces.Count - 1];
                newPieces.Remove(newPieces[newPieces.Count - 1]);//elimino la Posicionn q ya agrege para despues agregar las restantes al tablero
                for (int i = 0; i < newPieces.Count; i++){
                    places_avialable.Add(places_avialable.Count, newPieces[i]);
                }
            }
            else{
                for (int i = 0; i < move.piece.Valores.Count; i++){
                    places_avialable.Add(places_avialable.Count, move.piece.Valores[i]);
                }
            }
        }
        public bool IsValid((int position, IFicha<T> piece) move, State<T> state){
            if (state.Places_Available.Count == 0){                
                if (move.Item2 == null){
                    return false;
                }
                return true;
            }
            if(move.position < 0){
                return false;
            }
            if (state.Places_Available.ContainsKey(move.position)){
                return (move.Item2.Valores.Contains(state.Places_Available[move.position]));//verifico si la Ficha que tiro es verdad que tiene un valor que esta en esa Posicionn
            }
            return false;
        }
        public void EndGame(List<IPlayer<T>> players, IReferee<T> referee){ // Este metodo decide el ganador, este metodo se utiliza dentro del metodo Game que esta en rules
            int winner = -1;
            int Min = int.MaxValue;
            long score_of_player = 0;
            for (var i = 0; i < players.Count; i++){ ///Esta primer ciclo elege un ganador por el valor de sus fichas 
                int aux = score.Score(players[i].Hand); // El valor de la mano depende del criterio score
                score_of_player += aux; //Estos puntos van para el score del jugador(equipo)
                if(Min > aux){
                    winner = i;
                    Min = aux;
                }
                System.Console.WriteLine("El jugador {0} tiene {1} puntos, su mano es",players[i].Name,aux);

                for(int j = 0; j < players[i].Hand.Count; j++)
                    System.Console.WriteLine($" [{String.Join(",", players[i].Hand[j].Valores)}]");

            }
            for (int i = 0; i < players.Count; i++){ // Este ciclo elige como ganador al que no tiene fichas, este criterio es determinate 
                
                if(players[i].Hand.Count == 0){
                    winner = i;
                }
            }
            int winner_team = couples.ThisTeam(players[winner]); 
            for (var j = 0; j < couples.Teams_in_Smash[winner_team].Count; j++){ // Elimino los puntos del equipo ganador de la variable score_of_player
                score_of_player-=(long)score.Score(couples.Teams_in_Smash[winner_team][j].Hand); /// couples.Teams_int_Smash[winner_team] = List<IPlayer<T>>
            }                                                                                    /// [j].Hand Mano del jugador
            referee.EndSmash(players[winner],this.couples ,score_of_player); // El resultado se almacena 
            if(couples.Teams_in_Smash[winner_team].Count > 1)
            System.Console.WriteLine("El ganador es {0} con {1} puntos ", players[winner].Name, Min);
            else
            System.Console.WriteLine("El ganador de la ronda es {0} con {1} puntos . El equipo equipo {2} gana esta ronda",players[winner].Name, Min,winner_team);

           
            
        }
    }
    #endregion
    #region administracion del juego y control de equipos 
    public class Referee <T> : IReferee<T> where T : IComparable{ //Este objeto administra los puntajes y las victorias de cada equipo y jugador
        Dictionary<IPlayer<T>, int> rounds_for_player;
        Dictionary<IPlayer<T>, long> score_of_player;
        Dictionary<List<IPlayer<T>>,int> rounds_for_teams;
        Dictionary<List<IPlayer<T>>,long> score_of_teams;
        List<IPlayer<T>> winner_for_round;
        public List<IPlayer<T>> Winner_for_rounds (ICouple<T> couple, int max_rounds){
            for (var i = 0; i < couple.Teams_in_Smash.Count; i++){
                if(rounds_for_teams[couple.Teams_in_Smash[i]] == max_rounds)
                    return couple.Teams_in_Smash[i];
            }
            throw new Exception ("Ningun equipo gano la cantidad de rondas indicadas ");
        }
        
        public List<IPlayer<T>> Winner_for_points(ICouple<T> couple, long max_points){
            int team_winner = -1;
            for (var i = 0; i < couple.Teams_in_Smash.Count; i++){
                if(score_of_teams[couple.Teams_in_Smash[i]] >= max_points){
                    max_points = score_of_teams[couple.Teams_in_Smash[i]];
                    team_winner = i;
                }
            }
            if(team_winner >=0) return couple.Teams_in_Smash[team_winner];
            throw new Exception ("Ningun equipo gano la cantidad de puntos indicadas");
        }
        
        public Referee(List<IPlayer<T>> players,Dictionary<int,List<IPlayer<T>>> team){
            this.rounds_for_player = new Dictionary<IPlayer<T>, int>();
            this.score_of_player = new Dictionary<IPlayer<T>, long>();
            this.rounds_for_teams = new Dictionary<List<IPlayer<T>>, int>();
            this.score_of_teams = new Dictionary<List<IPlayer<T>>,long>();
            this.winner_for_round = new List<IPlayer<T>>();
            List<IPlayer<T>> winner_for_round = new List<IPlayer<T>>();
            for (var i = 0; i < players.Count; i++){
                this.rounds_for_player.Add(players[i],0);
                this.score_of_player.Add(players[i],0);
            }
            for (var i = 0; i < team.Count; i++){
                this.rounds_for_teams.Add(team[i],0);
                this.score_of_teams.Add(team[i],0);
            }
        }
        public void EndSmash (IPlayer<T> player,ICouple<T> couple,long score){
            if(!rounds_for_player.ContainsKey(player)){
                throw new Exception("Eeeh, este no participa en el juego");
            }
            else{
                System.Console.WriteLine("El score es igual a {0}", score);
                this.score_of_player[player]+=score; /// Lista que almacena el puntaje de cada jugador
                this.rounds_for_player[player]+=1;   /// Lista que almacena la cantidad de rondas ganadas por cada jugador
                this.winner_for_round.Add(player); ///  Lista que almacena el jugador ganador de cada ronda
                int team = couple.ThisTeam(player); /// equipo al que pertenece el ganador
                this.rounds_for_teams[couple.Teams_in_Smash[team]]+=1; // Lista que almacena la cantidad de rondas ganadas por equipo
                this.score_of_teams[couple.Teams_in_Smash[team]]+=score; // Lista que almacena el puntaje por equipos;
                System.Console.WriteLine(this.score_of_teams[couple.Teams_in_Smash[team]]);
                System.Console.WriteLine(this.rounds_for_teams[couple.Teams_in_Smash[team]]);
                //throw new Exception("Exploto");
            }
        }
        
       
        public bool Winner_this_count_of_rounds(int Max_Rounds){
            if(rounds_for_teams.ContainsValue(Max_Rounds))
                return true;
            else 
                return false;
        }
        public bool Winner_this_count_of_points(long max_point, ICouple<T> couple){
            //throw new Exception("Exploto");
            Dictionary<int,List<IPlayer<T>>> teams_in_smash = couple.Teams_in_Smash; // Una lista con los equipos que participan en el juego 
            for (var i = 0; i < teams_in_smash.Count; i++){
                if(max_point <= score_of_teams[teams_in_smash[i]]){ 
                    return true;
                }
            }
            return false;
        }
    }
    public class Couples<T> : ICouple<T> where T: IComparable{ //Este  objeto controla lo referente a los equipos
        Dictionary<int,List<IPlayer<T>>> team;
        private int number_in_team;
        public Dictionary<int,List<IPlayer<T>>> Teams_in_Smash { get { return this.team;}}
        public Couples(int number_in_team){
            this.number_in_team = number_in_team;
            this.team = new Dictionary<int, List<IPlayer<T>>>();
        }
        public void Team(List<IPlayer<T>> players){
            
            if( this.number_in_team == 0 || players.Count % this.number_in_team != 0){
                throw new Exception("El numero de intengrantes por equipos no es correcto");
            }
            Dictionary<int,List<IPlayer<T>>> team = new Dictionary<int, List<IPlayer<T>>>();
            int selectteam = players.Count /this.number_in_team;
            System.Console.WriteLine("selectteam = {0}",selectteam);
            for (var i = 0; i < selectteam; i++){
                team.Add(i,new List<IPlayer<T>>());
                int flag = 0;
                while(flag<number_in_team){
                    team[i].Add(players[flag + i * number_in_team]);
                    flag++;
                }
            }
            
            this.team = team;
        }
        public int ThisTeam (IPlayer<T> player){ // Devuelve el equipo del jugador
            for(int i = 0; i < this.team.Count; i++){
                if(team[i].Contains(player)){
                    return i;
                }
            }
            throw new Exception ("Error. Este jugador debe estar en algun equipo");
        }
    }
    #endregion
    #region Jugadores

    public class Player <T> : IPlayer <T> where T : IComparable { //Clase jugador recibe un objeto tipo estrategia,este ultimo contiene el algoritmo para jugar
        protected List<IFicha<T>> hand;
        protected string name;
        protected IStrategy<T> strategy;
        protected int score;
        public Player (string name, IStrategy<T> strategy){
            this.name = name;
            this.strategy = strategy;
            this.hand = new List<IFicha<T>>();
            this.score = 0;
        }
        public void AddHand (List<IFicha<T>> piece_of_the_game, IRules<T> rules ){
            this.hand = rules.Dispatcher(piece_of_the_game);// Despacha segun la regla
        }
        public string Name { get {return this.name;}}
        public int Puntuacion {get ;}
        public IStrategy<T> WayToPlay{ get {return this.strategy;} }
        public List<IFicha<T>> Hand { get{ return this.hand ;}} // Devuelve una copia de la mano 

        public int Numb_Of_Piece {get{return hand.Count;}}

        public int Score { get {return score;}}

        public virtual (int, IFicha<T>) Play(State<T> state,Func<State<T>,IFicha<T>,bool> Filter_Piece){

            //System.Console.WriteLine("soy {0}",Name);
            return this.strategy.Play(state,Hand,Filter_Piece);
        }
        public bool Have(State<T> state,Func<State<T>,IFicha<T>,bool> Filter_Piece){
            if(state.Places_Available.Count == 0){
                foreach(var piece in Hand)
                    if(Filter_Piece(state,piece)){
                        //System.Console.WriteLine($"[{String.Join(",", piece.Valores)}]");
                        return true;
                    }
                return false;
            }
            foreach (var place in state.Places_Available){
                foreach (var piece in this.Hand){
                    if (piece.Valores.Contains(place.Value) && Filter_Piece(state,piece)){ // se puede jugar al menos una ficha 
                        return true;
                    }
                }
            }
            return false;
            }
        }
    public class Random_Player<T> : IStrategy<T> where T : IComparable{
    public (int, IFicha<T>) Play(State<T> state, List<IFicha<T>> Hand,Func<State<T>,IFicha<T>,bool> Filter_Piece){
        Random random = new Random();
        List<IFicha<T>> move_available = new List<IFicha<T>>(); // posibles fichas a jugar 
        List<int> places_available = new List<int>();// posibles lugares posiciones a jugar 
        //System.Console.WriteLine("Cantidad de fichas en la mano {0}",Hand.Count);
        if (state.Piece_On_The_Board.Count == 0){
            //System.Console.WriteLine("Es el primero en colocar");
            List<IFicha<T>> first_pieces = new List<IFicha<T>>();
            foreach(var posible_piece  in Hand) //Averigua las primeras fichas que se pueden jugar 
                if(Filter_Piece(state,posible_piece))
                    first_pieces.Add(posible_piece);
            IFicha<T> piece = first_pieces[random.Next(first_pieces.Count - 1)]; // De esas fichas saca una randon
            Hand.Remove(piece); //retirala de la mano
            return (1, piece);
        }
        //System.Console.WriteLine("Piezas disponibles");
        foreach (var place in state.Places_Available){
            foreach (var piece in Hand){
                if (piece.Valores.Contains(place.Value) && !move_available.Contains(piece) && Filter_Piece(state,piece)){ // Si alguno de los lugares disponibles a jugar tiene el valor de esa ficha y la ficha no esta seleccionada para jugar
                    move_available.Add(piece);
                    //System.Console.WriteLine($"[{String.Join(",", piece.Valores)}]");
                }
            }
        }
        //System.Console.WriteLine(move_available.Count);
        IFicha<T> move = move_available[random.Next(move_available.Count)]; // tomo una ficha al azar de las que puedo jugar
        Hand.Remove(move);
        for (int i = 0; i < state.Places_Available.Count; i++){
            if (move.Valores.Contains(state.Places_Available[i])){
                places_available.Add(i); // Lugares disponibles a jugar 
            }
        }
        //System.Console.WriteLine("Hola estoy dentro de la estrategia randow");
        return (places_available[random.Next(places_available.Count - 1)], move); // devuelve la ficha a alguna de las posiciones disponibles
    }

}
    public class Fat_Player<T> : IStrategy<T> where T : IComparable{
    private bool sort = false;
    public (int, IFicha<T>) Play(State<T> state, List<IFicha<T>> Hand,Func<State<T>,IFicha<T>,bool> Filter_Piece){   // Las fichas se van a ordenar de mayor a menor 
        if (!sort){ // 
            for (int i = 0; i < Hand.Count - 1; i++){
                for (int j = i + 1; j < Hand.Count; j++){
                    if (Hand[i].Value < Hand[j].Value){
                        IFicha<T> temp = Hand[i];
                        Hand[i] = Hand[j];
                        Hand[j] = temp;
                    }
                }
            }
            sort = true;
        }

        if (state.Piece_On_The_Board.Count == 0){ // El tablero esta vacio
            for(int i = 0; i < Hand.Count;i++){
                if(Filter_Piece(state,Hand[i])){
                    IFicha<T> first_move = Hand[i];
                    Hand.Remove(first_move);
                    return (1, first_move);
                }    
            }
        }
        List<IFicha<T>> move_available = new List<IFicha<T>> ();
        //System.Console.WriteLine("Piezas diponibles para un jugador gordo");
        foreach (var place in state.Places_Available){
            foreach (var piece in Hand){
                if (piece.Valores.Contains(place.Value) && !move_available.Contains(piece) && Filter_Piece(state,piece)){
                    move_available.Add(piece);
                    //System.Console.WriteLine($"[{String.Join(",", piece.Valores)}]");
                }
            }
        }
        for (int i = 0; i < move_available.Count - 1; i++){
            for (int j = i + 1; j < move_available.Count; j++){
                if (move_available[i].Value < move_available[j].Value){
                    IFicha<T> temp = move_available[i];
                    move_available[i] = move_available[j];
                    move_available[j] = temp;
                }
            }
        }
        IFicha<T> move = move_available[0];
        int position = 0;
        foreach (var x in state.Places_Available){
            if (move.Valores.Contains(x.Value)){
                position = x.Key;
                break;
            }
        }
        Hand.Remove(move);
        System.Console.WriteLine("Hola estoy dentro de la estrategia gorda");
        return (position, move);

    }

    }
    public class Standart_Player<T> : IStrategy<T> where T : IComparable{   
        Fat_Player<T> fat_Player;
        Random_Player<T> random_Player;
        public Standart_Player(){
            this.fat_Player = new Fat_Player<T>();
            this.random_Player = new Random_Player<T>();
        }
         
        public (int, IFicha<T>) Play(State<T> state, List<IFicha<T>> Hand,Func<State<T>,IFicha<T>,bool> Filter_Piece){
            Random random_seed = new Random();
            if(random_seed.Next(1) == 0){
                return fat_Player.Play(state,Hand,Filter_Piece);
            }
            else{
                return random_Player.Play(state,Hand,Filter_Piece);
            }
        }
    }
    public class Smart_Fat_Player<T> : IStrategy<T> where T : IComparable{ ///// Pendiente a implementar ,este ugador debe valorar la ficha que mas "pesa" siguiendo cualquier criterio de comparacion
        public IComparable comparable;
        public Smart_Fat_Player(IComparable comparable){
            this.comparable = comparable;
        }

        public (int, IFicha<T>) Play(State<T> state, List<IFicha<T>> Hand,Func<State<T>,IFicha<T>,bool> Filter_Piece){
            throw new NotImplementedException();
        }
        
    }
    #endregion
    #region Juego Clasico
    public class ClassicGenerator<T> : IGenerator<T> where T : IComparable{
        private T[] value;
        public ClassicGenerator(T[] value){
            this.value = value;
        }
        public List<IFicha<T>> PieceGenerator(){
            bool[] mask = new bool [this.value.Length];
            List<IFicha<T>> aux = new List<IFicha<T>>();
            for (int i = 0; i < this.value.Length; i++){
                for (int j = 0; j < this.value.Length; j++){
                    if(!mask[j]){
                        List<T> list = new List<T>();
                        list.Add(value[i]);
                        list.Add(value[j]);
                        IFicha<T> piece = new Ficha<T>(list);
                        aux.Add(piece);
                        
                    }
                }
                mask[i] = true;
            }
            //Esto es para comprobar las fichas generadas 
            //System.Console.WriteLine("El estucho de este juego es el siguiente");
            /*foreach(var x in aux){
                System.Console.WriteLine($" ficha   [{String.Join(",", x.Valores)}]");
            }*/
            return aux;
        }

        
    }
    public class ClassicDistribution<T> : IDistribution<T> where T : IComparable{
        public List<IFicha<T>> DistributionOfPics(List<IFicha<T>> piece_of_the_game, int number_of_piece_for_player){
            Random seed = new Random();
            
            List<IFicha<T>> aux = new List<IFicha<T>> ();
            for (int i = 0; i < number_of_piece_for_player; i++){
                int position =  seed.Next(piece_of_the_game.Count);
                //System.Console.WriteLine(position);
                //System.Console.WriteLine("cantidad de piezas {0}", piece_of_the_game.Count);
                aux.Add(piece_of_the_game[position]);
                piece_of_the_game.Remove(piece_of_the_game.ElementAt(position));
            }
            //System.Console.WriteLine("Hi");
            return aux;
        }
    }
    public class ClassicGame<T> : IGameMode<T> where T : IComparable{ 

        public static bool Standart_Filter(State<T> state,IFicha<T> piece) => true;
        public static bool Double_Filter(State<T> state,IFicha<T> piece){ 
            T comp = piece.Valores.First();
            foreach (var value in piece.Valores) 
                if(!Equals(comp,value)) 
                    return false; 
            return true;
        }
        
        public IEnumerable<State<T>> Game(IRules<T> rules,IReferee<T> referee, Dictionary<int, T> places_available, List<IPlayer<T>> players, List<IFicha<T>> piece_on_the_game, List<IFicha<T>> piece_on_the_board, List<IFicha<T>> piece_not_played){
            //System.Console.WriteLine("Empieza el juego");
            bool end = false;
            int passed = 0;
            //List<IFicha<T>> fichas_para_graficar = new List<IFicha<T>>();
            while (!end){
                //System.Console.WriteLine("el juego sigue");
                bool[] mask = new bool[players.Count]; // 
                for (int i = 0; i < players.Count; i++){
                    //System.Console.WriteLine("El juego continua");
                    State<T> actual_state = new State<T>(places_available,piece_on_the_game,piece_on_the_board,piece_not_played);
                    if (players[i].Have(actual_state,Standart_Filter)){ //Si el jugador puede jugar
                        (int position, IFicha<T> piece) = players[i].Play(actual_state,Standart_Filter);
                        //System.Console.WriteLine("La jugada del jugador {0} es valida ? respueta {1}", players[i].Name,rules.IsValid((position,piece), actual_state));
                        if (rules.IsValid((position,piece), actual_state)){
                            //System.Console.WriteLine("La jugada es valida");
                            ///System.Console.WriteLine($"El jugador {players[i].Name} ha jugado [{String.Join(",", piece.Valores)}]");
                            piece_on_the_board.Add(piece); // se agrega la ficha al monton de las jugadas 
                            passed = 0; // Se actualiza la situacion de los pases
                            //Grafico<T>.ImprimirLasFichasDeTodosLosJugadores(players);
                            //Grafico<T>.ImprimirFichasEnElTablero(piece_on_the_board)
                            //Grafico<T>.OrganizarFichasEnELTablero(fichas_para_graficar,piece);
                            //Grafico<T>.ImprimirFichasEnElTablero(fichas_para_graficar);
                            rules.Update_The_Board((position,piece), places_available);//actualizo el tablero Actualizar_Board(Posicionn, Ficha)
                            ///System.Console.WriteLine($"Tab: [{String.Join(",", places_available.Values)}]");
                            if (players[i].Hand.Count == 0){//El jugador se pego 
                                ///System.Console.WriteLine($"El jugador {players[i].Name} se ha pegado");
                                end = true;
                                rules.EndGame(players,referee);
                                yield return new State<T>(places_available,piece_on_the_game,piece_on_the_board,piece_not_played);//Turno del siguiente jugador 
                                yield break;
                                
                            }
                            yield return new State<T>(places_available,piece_on_the_game,piece_on_the_board,piece_not_played);//Turno del siguiente jugador 
                        }
                        else{ // Jugada invalida se penaliza con la expulsion 
                            System.Console.WriteLine($"El jugador {players[i].Name} ha hecho una jugada invalida y sera expulsado");
                        mask[i] = true;
                        }
                    }
                    else { //El jugador se paso Se Usa EndGame
                        passed++;
                        if (passed == players.Count){ //Se tranco el juego 
                            end = true;
                            System.Console.WriteLine("Se tranco el juego");
                            rules.EndGame(players,referee);
                            yield return new State<T>(places_available,piece_on_the_game,piece_on_the_board,piece_not_played);//Turno del siguiente jugador 
                            yield break;
                        }
                        continue;
                    }
                }
                for (int i = 0; i < players.Count; i++){
                    if (mask[i]){
                        players.Remove(players[i]);
                    }
                }

            }
        }
        /*private int ScoreForPlayer(IPlayer<T> winner, List<IPlayer<T>> players, IRules<T> rules){
            int score = 0;
            foreach (var x in rules.Couples.)){
                if (!x.Value.Contains(winner)){
                    for (int i = 0; i < x.Value.Count; i++){
                        score += x.Value[i].Valor(rules.Score);
                    }
                }
            }
            return score;
        }*/
    }
    public class ClassicScore<T> : IScore<T> where T : IComparable{
        public int Score(List<IFicha<T>> hand){
            int aux = 0;
            foreach (var x in hand){
            aux += x.Value;
            }
            return aux;
        }
    }
    #endregion
    #region Variantes 
    
    public class CousinDistribution<T> : IDistribution<T> where T : IComparable{ // Despacha de numeros 
        private int cant;
        public CousinDistribution(int cant){
            if(cant <= 0) throw new Exception("La cantidad de fichas debe ser mayor que cero (generador de fichas primas)");
            this.cant = cant;
        }
        private static bool Cousins (int n){
            int k = 2;
            int x = (int)Math.Sqrt(n);
            while(k <= x){
                if(n % k == 0) return false;
                else k++;
            }
            return true;
        }
        public List<IFicha<T>> DistributionOfPics(List<IFicha<T>> piece_of_the_game, int number_of_piece_for_player){
            Random seed = new Random();
            for (var i = 0; i < piece_of_the_game.Count; i++){ // Desordena el paqueta de fichas que recibe
                int position =  seed.Next(piece_of_the_game.Count);
                IFicha<T> temp = piece_of_the_game[position];
                piece_of_the_game[position] = piece_of_the_game[i];
                piece_of_the_game[i] = temp;
            }
            List<IFicha<T>> aux = new List<IFicha<T>> ();
            for (int i = 0; i < number_of_piece_for_player; i++){ // Selecciona las fichas de acuerdo a la posicion (numero primo) en la lista desordenada
                int position =  seed.Next(piece_of_the_game.Count);
                while(!Cousins(position)){
                    position = seed.Next(piece_of_the_game.Count);
                }
                //System.Console.WriteLine(position);
                //System.Console.WriteLine("cantidad de piezas {0}", piece_of_the_game.Count);
                aux.Add(piece_of_the_game[position]);
                piece_of_the_game.Remove(piece_of_the_game.ElementAt(position));
            }
            //System.Console.WriteLine("Hi");
            return aux;
        }

    }
    public class LonganizaGame<T> : IGameMode<T> where T : IComparable 
    {   
        public static bool Longaniza_Filter(State<T> state, IFicha<T> piece){
            if(state.Places_Available.Count == 0){ //No hay ficha en la mesa, jugar el doble
                T comp = piece.Valores.First();
                foreach (var value in piece.Valores) 
                    if(!Equals(comp,value)) 
                        return false; 
                return true;
            }
            else 
                return true;
        }
        public IEnumerable<State<T>> Game(IRules<T> rules, IReferee<T> referee, Dictionary<int, T> places_available, List<IPlayer<T>> players, List<IFicha<T>> piece_on_the_game, List<IFicha<T>> piece_on_the_board, List<IFicha<T>> piece_not_played){
            System.Console.WriteLine($"Se esta jugando{0}");
            bool end = false;
            int passed = 0;
            while (!end){
                System.Console.WriteLine("el juego sigue ");
                bool[] mask = new bool[players.Count]; // 
                for (int i = 0; i < players.Count; i++){
                    System.Console.WriteLine("El juego continua");
                    State<T> actual_state = new State<T>(places_available,piece_on_the_game,piece_on_the_board,piece_not_played);
                    if (players[i].Have(actual_state,Longaniza_Filter)){ //Si el jugador puede jugar
                        (int position, IFicha<T> piece) = players[i].Play(actual_state,Longaniza_Filter);
                        System.Console.WriteLine("La jugada del jugador {0} es valida ? respueta {1}", players[i].Name,rules.IsValid((position,piece), actual_state));
                        if(places_available.Count == 0){// Si el tablero esta vacio
                               for (int j = 0; j < 3; j++){ //El inicializa la cruz 
                                    places_available.Add(j,piece.Valores[0]); //Longaiza Filter me garantiza que esta ficha es doble
                                    System.Console.WriteLine($"Posicion {j} tiene como valor {places_available[j]}");
                                }
                        }
                        if (rules.IsValid((position,piece), actual_state)){
                            System.Console.WriteLine("La jugada es valida");
                            System.Console.WriteLine($"El jugador {players[i].Name} ha jugado [{String.Join(",", piece.Valores)}]");
                            piece_on_the_board.Add(piece); // se agrega la ficha al monton de las jugadas 
                            passed = 0; // Se actualiza la situacion de los pases
                            if(places_available.Count == 0){
                                
                            }
                            rules.Update_The_Board((position,piece), places_available);//actualizo el tablero Actualizar_Board(Posicionn, Ficha)
                            System.Console.WriteLine($"Tab: [{String.Join(",", places_available.Values)}]");
                            if (players[i].Hand.Count == 0){//El jugador se pego 
                                System.Console.WriteLine($"El jugador {players[i].Name} se ha pegado");
                                end = true;
                                rules.EndGame(players,referee);
                                yield break;
                                
                            }
                            yield return new State<T>(places_available,piece_on_the_game,piece_on_the_board,piece_not_played);//Turno del siguiente jugador 
                        }
                        else{ // Jugada invalida se penaliza con la expulsion 
                            System.Console.WriteLine($"El jugador {players[i].Name} ha hecho una jugada invalida y sera expulsado");
                        mask[i] = true;
                        }
                    }
                    else { //El jugador se paso Se Usa EndGame
                        passed++;
                        if (passed == players.Count){ //Se tranco el juego 
                            end = true;
                            System.Console.WriteLine("Se tranco el juego");
                            rules.EndGame(players,referee);
                            yield break;
                        }
                        continue;
                    }
                }
                for (int i = 0; i < players.Count; i++){
                    if (mask[i]){
                        players.Remove(players[i]);
                    }
                }

            }
        }
    }
    public class DoubleScore<T> : IScore<T> where T : IComparable{ //El score es igual a los puntos de las fichas dobles
        public static bool Is_Double(IFicha<T> piece){ 
            T comp = piece.Valores.First();
            foreach (var value in piece.Valores) 
                if(!Equals(comp,value)) 
                    return false; 
            return true;
        }
        public int Score(List<IFicha<T>> hand){
            int aux = 0;
            foreach (var piece in hand){
                if(Is_Double(piece))
                    aux += piece.Value;
            }
            return aux;
        }
    } 
    public class CrazyGenerator<T> : IGenerator<T> where T : IComparable{
        private T[] value;
        public CrazyGenerator(T[] value){
            this.value = value;
        }
        public List<IFicha<T>> PieceGenerator(){
             
            List<IFicha<T>> aux = new List<IFicha<T>>();
            for (int i = 0; i < this.value.Length; i++){
                for (int j = 0; j < this.value.Length; j++){
                    List<T> list = new List<T>();
                    list.Add(value[i]);
                    list.Add(value[j]);
                    IFicha<T> piece = new Ficha<T>(list);
                    aux.Add(piece);
                }
            }    //Esto es para comprobar las fichas generadas 
            //System.Console.WriteLine("El estucho de este juego es el siguiente");
            /*foreach(var x in aux){
                System.Console.WriteLine($" ficha   [{String.Join(",", x.Valores)}]");
            }*/
            return aux;
            }
            
    
    }
    #endregion
}     
