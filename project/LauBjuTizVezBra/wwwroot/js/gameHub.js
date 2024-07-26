import '../lib/bootstrap/dist/js/bootstrap.bundle.min.js';

export var isConnected = false;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(()=> {
    console.log("SignalR Connected");
    isConnected = true;
}).catch(err => console.error(err.toString()));

export function createSession(sessionId) {
    connection.invoke("CreateGroup", sessionId).catch(err => console.error(err.toString()));
}

export function joinSession(sessionId) {
    connection.invoke("JoinGroup", sessionId).catch(err => console.error(err.toString()));
}

export function leaveSession(userId, sessionId) {
    connection.invoke("LeaveGroup", userId, sessionId).catch(err => console.error(err.toString()));
}

export function closeSession(sessionId) {
    console.log("SessionId: " + sessionId + " is closing");
    connection.invoke("RemovePlayersFromGroup", sessionId).catch(err => console.error(err.toString()));
}

export function createANewGame(sessionId) {
    console.log("SessionId: " + sessionId + " is starting a new game");
    connection.invoke("StartNextRound", sessionId).catch(err => console.error(err.toString()));
}

// Temp function to test if the connection works
export function sendMessage(message, sessionId) {
    connection.invoke("SendMessage", message, sessionId).catch(err => console.error(err.toString()));
}

export function sendGuess(message, userId, sessionId, gameId, guesserId, imageIdentifier) {
    console.log("Sending guess: " + message + " to game: " + gameId + " from guesser: " + guesserId);
    connection.invoke("SendGuess", message, userId, sessionId, gameId, guesserId, imageIdentifier).catch(err => console.error(err.toString()));
}

export function oracleRevealedATile(oracleId) {
    console.log("Oracle with Id: " + oracleId + " revealed a tile");
    connection.invoke("OracleRevealedATile", oracleId).catch(err => console.error(err.toString()));
}

export function showThisPiece(pieceId, sessionId) {
    console.log("We are trying to show this piece")
    connection.invoke("ShowThisPiece", pieceId, sessionId).catch(err => console.error(err.toString()));
}

export function showPieceForAllPlayers(sessionId) {
    console.log("We are trying to show a piece for all players")
    connection.invoke("ShowNextPieceForAllPlayers", sessionId).catch(err => console.error(err.toString()));
}

// Temp function to test if the connection works
connection.on("ReceiveMessage", (message) => {
    var chatDiv = document.getElementById("chatDiv");
    var messageDiv = document.createElement("p");
    messageDiv.textContent = message;
    chatDiv.appendChild(messageDiv);
});

// These functions are used to send a user from to the link or force a reload of the current page
// and serve as a way to reload the page for everyone in the SignalR groups
connection.on("RedirectToLink", (link) => {
    console.log("Redirecting to: " + link);
    
    window.location.href = link
});

// Pretty much serves the same purpose as above just by reloading the current page instead of redirecting
connection.on("ReloadPage", () => {
    window.location.reload();
});

// Handles functions for receiving and displaying guesses
connection.on("ReceiveGuess", (guess, playerName) => {
    var guessingDiv = document.getElementById("guessingDiv");
    var guessP = document.createElement("p");
    guess = playerName + ": " + guess;
    guessP.textContent = guess;
    guessingDiv.appendChild(guessP);
})

connection.on("CorrectGuess", (winnerText, answer) => {
    document.getElementById("winningPlayer").textContent = winnerText;
    document.getElementById("modalAnswer").textContent = answer;

    var victoryModal = new bootstrap.Modal(document.getElementById("myModal"));
    victoryModal.show();
});

export { connection };