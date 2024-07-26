import {sendGuess, closeSession, createANewGame, leaveSession, 
        oracleRevealedATile, showThisPiece, connection, isConnected, showPieceForAllPlayers} from "./gameHub.js";

var guessButton = document.getElementById("guessButton");
var sessionId = document.getElementById("sessionId").value;
var gameId = document.getElementById("gameId").value;
var imageIdentifier = document.getElementById("imageIdentifier").value;
var userId = document.getElementById("userId").value;
var gameMode = document.getElementById("gameMode").value;

var oracleIsAI = document.getElementById("oracleIsAI").value;

if(!oracleIsAI) {
  var chosenOracle = document.getElementById("userOracleId").value;
}

var oracleId = document.getElementById("oracleId").value;

// script from html file
let availablePiecesOfImage = availablePiecesOfImagePreLoad;

let displayedImages = [];


// Guess button is only visible if the user is a guesser
if(chosenOracle != null && chosenOracle != userId || oracleIsAI) {
  if(guessButton) {
      guessButton.addEventListener("click", () => {
          var message = document.getElementById("answerInput").value;
          var guesserId = document.getElementById("guesserId").value;
  
          sendGuess(message, userId, sessionId, gameId, guesserId, imageIdentifier);
  })}
};


// Renders guessers image
function renderImages() {//Denne koden manipulerere og forandrer på individuelle bilder
  const imageContainer = document.getElementById('imageList');
  imageContainer.innerHTML = '';
  displayedImages.forEach((image, index) => {

    const imageElement = document.createElement('img');
              
    const relativeImagePiecePath = image.replace('wwwroot', '');//Fjerner wwwroot bugggen

    imageElement.src = relativeImagePiecePath;
    imageElement.style.position = 'absolute';

    imageContainer.appendChild(imageElement);// legger til iagecontaienr som viser alle bildene. 
  
  });
}

connection.on("ShowPiece", (piece) => {

  const chosenImage = availablePiecesOfImage.find(aI => aI == piece);
  const imgIndex = availablePiecesOfImage.indexOf(chosenImage);

  if (imgIndex == -1) return;
  availablePiecesOfImage.splice(imgIndex, 1);

  displayedImages.push(chosenImage);

  if (chosenOracle != null && chosenOracle != userId || oracleIsAI) {
    renderImages();
  } else {
    renderUserOracleImage();
  }
});

function renderUserOracleImage() {
  const imageContainer = document.getElementById('oracleImageView');
  imageContainer.innerHTML = '';

  displayedImages.forEach((image, index) => {
    console.log("Image" + index + ":" + image);
    const imageElement = document.createElement('img');

    const relativeImagePiecePath = image.replace('wwwroot', '');

    imageElement.src = relativeImagePiecePath;

    imageElement.style.position = 'absolute';
    imageElement.id = image

    imageContainer.appendChild(imageElement);
  });

  availablePiecesOfImage.forEach((image, index) => {
    const imageElement = document.createElement('img');
  
    const relativeImagePiecePath = image.replace('wwwroot', '');

    imageElement.src = relativeImagePiecePath;

    imageElement.style.position = 'absolute';
    imageElement.style.opacity = '0.2';
    imageElement.id = image

    imageContainer.appendChild(imageElement);
  });
}

function imageInteractionInitializer() {
  const imageContainer = document.getElementById('oracleImageView');
  const gameId = document.getElementById('gameId').value;

  imageContainer.addEventListener('click', function(event) {
    let x = event.offsetX;
    let y = event.offsetY;

    for (let piece of imagePiecesData) {
      if (isPointInNonTransparentArea(x, y, piece.Item2)) {

        showThisPiece(piece.Item1, sessionId);
        oracleRevealedATile(oracleId);

      }
    }
  })
}

var showNextPieceForAllButton = document.getElementById("ShowNextPieceForAll");
if (oracleIsAI && showNextPieceForAllButton && gameMode == "FreeForAll") {
  showNextPieceForAllButton.addEventListener("click", () => {
      showPieceForAllPlayers(sessionId);
  });
}


connection.on("ShowNextPieceForAll", () => {    
  
    console.log("ShowNextPieceForAll");
    const newImagePiece = getRandomImage();// henter det randome bildet 

    if (newImagePiece != null) {
      displayedImages.push(newImagePiece);// legger til hvis bildet ikke er i arrayen.
      
      oracleRevealedATile(oracleId);
      renderImages();
    }
});


function showOneMore() {
  const newImagePiece = getRandomImage();// henter det randome bildet 

  if (newImagePiece != null) {
    displayedImages.push(newImagePiece);// legger til hvis bildet ikke er i arrayen.

    oracleRevealedATile(oracleId);
    renderImages();
  }
}

function getRandomImage() {
  if (availablePiecesOfImage.length == 0) {
      console.warn("No more images to show");
      return null;
  }

  const randomIndex = oracleAI_Array_NumbersForImagePieces[0];
  oracleAI_Array_NumbersForImagePieces.splice(0, 1);
  
  const chosenImage = availablePiecesOfImage[randomIndex];

  return chosenImage;// Lager et random bilde.
}


function isPointInNonTransparentArea(x, y, nonTransparentPixels) {
  return nonTransparentPixels.some(pixel => pixel.Item1 == x && pixel.Item2 == y);
}

var showOneMoreButton = document.getElementById("showOneMore"); 
if (oracleIsAI && showOneMoreButton) {
  showOneMoreButton.addEventListener("click", () => {
      showOneMore();
  });
}

document.querySelectorAll(".EndGame").forEach(button => {
    button.addEventListener("click", () => {
        closeSession(sessionId);
    })
})

var nextRoundButton = document.getElementById("NextRound");
if (nextRoundButton) {
nextRoundButton.addEventListener("click", () => {
    createANewGame(sessionId, gameId);
})};

var requestNewAI = document.getElementById("RequestNewAI");
if (requestNewAI) {
    requestNewAI.addEventListener("click", () => {
    createANewGame(sessionId);
})};

var playerQuit = document.getElementById("Quit");
if (playerQuit) {
    playerQuit.addEventListener("click", () => {
    leaveSession(userId, sessionId);
})};

window.onload = function() {
  function waitForConnection() {
      if (isConnected) {
        if(chosenOracle != userId || oracleIsAI) {
          
          showOneMore();// starter koden( initial load)
        
        } else {
          imageInteractionInitializer();
        
          renderUserOracleImage();
        }
      } else {
          setTimeout(waitForConnection, 100);
      }
  }
  waitForConnection();
}


connection.on("CorrectGuess", (winnerText, answer) => {
  document.getElementById("winningPlayer").textContent = winnerText;
  document.getElementById("modalAnswer").textContent = answer;

  var myModal = document.getElementById('victoryModal')

  var victoryModal = new bootstrap.Modal(myModal);
  victoryModal.show();
});