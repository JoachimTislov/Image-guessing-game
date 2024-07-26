namespace Core.Domain.SignalRContext.Hubs;
public interface IGameClient
{
   //------------ Server to Client ------------
   Task RedirectToLink(string Url);
   Task ReloadPage();
   Task ReceiveMessage(string Message);
   Task ReceiveGuess(string Guess, string UserName);
   Task CorrectGuess(string WinnerText, string CorrectGuess);
   Task ShowPiece(string Piece);
   Task ShowNextPieceForAll();
}