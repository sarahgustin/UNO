// See https://aka.ms/new-console-template for more information
using UNOGame.Logic;
using UNOGame.Models;
using UNOGame.UI;


    ConsoleDisplay.ShowMenu();
    List<IPlayer> players = ConsoleDisplay.InitPlayer();
    
    
    
    //IBoard board = new Board();
    //ICard card = new Card();
    //IDeck deck = new Deck();
    //GameController gc =  new GameController(List<IPlayer> player , IDeck deck, IBoard board);
    
    

    /*subscribe event
    gc.OnDeckEmpty += UI.ShowDeckEmpty;
    gc.OnPlayerRunOutCard += UI.ShowWinner;
     */