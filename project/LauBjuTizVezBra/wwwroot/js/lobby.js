import {isConnected, createSession, sendMessage, leaveSession, closeSession} from "./gameHub.js";

// Managing the lobby options and the live website updates
// These parameters are used to disable the start and enable the update button whenever an option is changed
// this is to make sure that you cant start the game without applying the latest changes and sending them to the server
var updateButton = document.getElementById("updateButton");
var startGameButton = document.getElementById("startGameButton");
var sessionId = document.getElementById("sessionId").value;

document.querySelectorAll('.image-selector-button').forEach(button => {
    button.addEventListener('click', function() {
        selectThisPicture(button.id);
    });
});

// Managing the image selection in the Lobby
let selectedPicture;
function selectThisPicture(id) {
    selectedPicture = document.getElementById("selectedPicture");
    var imageInput = document.getElementById("imageInput");
    var selectedSpan = document.createElement("span");
    var button = document.getElementById(id);
    
    if (!button.querySelector("span")) {
        if (selectedPicture) selectedPicture.remove();
        selectedSpan.classList.add("position-absolute");
        selectedSpan.classList.add("top-0");
        selectedSpan.classList.add("start-100");
        selectedSpan.classList.add("translate-middle");
        selectedSpan.classList.add("badge");
        selectedSpan.classList.add("rounded-pill");
        selectedSpan.classList.add("bg-success");
        selectedSpan.id = "selectedPicture";
        
        // Creates the checkmark
        var checkMark = document.createElement("i");
        checkMark.classList.add("bi");
        checkMark.classList.add("bi-check-lg");
        selectedSpan.appendChild(checkMark);
        
        button.appendChild(selectedSpan);
        
        imageInput.value = id;
        
        updateButton.disabled = false;
        startGameButton.disabled = true;
    }
}

// This manages the Number of rounds
const roundRange = document.getElementById("numRounds");
const roundValue = document.getElementById("numRoundsValue");

// Here we only check if roundRange exists because if it exists the other option settings will
// also exist and if its not rendered the others will not be rendered either.
// As options is only rendered for the host not checking will cause an error for the other users
// and stop the execution of the script.


const randomPictureMode = document.getElementById("randomPictureMode");
const pictureSelector = document.getElementById("pictureSelector");


if (roundRange) {
    
    roundRange.oninput = function () {
        roundValue.textContent = this.value;
        updateButton.disabled = false;
        startGameButton.disabled = true;
    }

    // This manages the Lobby size
    const lobbyRange = document.getElementById("lobbySize");
    const lobbyValue = document.getElementById("numLobbySizeValue");
    
    lobbyRange.oninput = function () {
        lobbyValue.textContent = this.value;
        updateButton.disabled = false;
        startGameButton.disabled = true;
    }
    
    // This Manages the Gamemode
    const gameModeSelect = document.getElementById("gameModeSelect");
    const lobbySizeRange = document.getElementById("lobbySize");
    const randomOracle = document.getElementById("randomOracle");
    const oracleCheckBoxes = document.querySelectorAll("input.oracleCheckBox");
    const useAiCheckbox = document.getElementById("useAi");

    gameModeSelect.onchange = function () {

        if (this.value == "SinglePlayer") {
            pictureSelector.hidden = true;
            randomPictureMode.disabled = true;
            randomPictureMode.checked = true;
            roundRange.disabled = false;
            useAiCheckbox.checked = true;

            var imageInput = document.getElementById("imageInput");
            imageInput.value = null;

        } else {
            pictureSelector.hidden = false;
            pictureSelector.style.display = "block";
            randomPictureMode.disabled = false;
            randomPictureMode.checked = false;
            roundRange.disabled = true;
        }

        if (this.value == "Duo") {
            useAiCheckbox.checked = false;
            useAiCheckbox.disabled = true; 
        }

        if (this.value != "FreeForAll") {
            // Setting number of players based on the selected gameMode
            lobbyValue.textContent = (this.value == "Singleplayer") ? 1 : 2;
            lobbyRange.value = (this.value == "Singleplayer") ? 1 : 2;

            lobbySizeRange.disabled = true;
            randomOracle.disabled = true;
            useAiCheckbox.disabled = true; 

            oracleCheckBoxes.forEach( function (input) {
                input.disabled = true;
            });

            randomOracle.disabled = true;
        } else {
            lobbyValue.textContent = 2;
            lobbySizeRange.disabled = false;

            useAiCheckbox.disabled = false;
            useAiCheckbox.checked = false;

            randomOracle.disabled = false;

            oracleCheckBoxes.forEach( function (input) {
                input.disabled = false;
            });

            randomOracle.disabled = false;
        }

        updateButton.disabled = false;
        startGameButton.disabled = true;
    }

    // This Manages the Random picture Mode

    randomPictureMode.onchange = function () {
        // This disables the picture selector and enables round selector when random picture mode is selected
        if (this.checked) {
            roundRange.disabled = false;
            pictureSelector.hidden = true;
        } else {
            if (gameModeSelect.value != "Singleplayer") {
                console.log("test");    
                roundRange.disabled = true;
             
                pictureSelector.hidden = false;
                pictureSelector.style.display = "block";
            }
        }
        updateButton.disabled = false;
        startGameButton.disabled = true;
    }

    // This Manages the Random oracle

    randomOracle.onchange = function () {

        // This disables the Oracle checkboxes on Players when random Oracle is selected
        if (this.checked) {
            oracleCheckBoxes.forEach( function (input) {
                input.disabled = true;
            });
        } else {
            oracleCheckBoxes.forEach( function (input) {
                input.disabled = false;
            });
        }
        updateButton.disabled = false;
        startGameButton.disabled = true;
    }

    // This Manages the Oracle checkboxes
    const newOracleInput = document.getElementById("newOracleInput");

    oracleCheckBoxes.forEach( function (checkbox) {
        checkbox.addEventListener("click", function (){
            oracleCheckBoxes.forEach( function (othercheckbox) {
                if (othercheckbox !== checkbox) {
                    othercheckbox.checked = false;
                } else {
                    newOracleInput.value = checkbox.id;
                }
                
                updateButton.disabled = false;
                startGameButton.disabled = true;
            });
        });
    });

    
    // This Manages the UseAI checkbox
    useAiCheckbox.onchange = function () {

        if (this.checked) 
        {
            oracleCheckBoxes.forEach( function (input) 
            {
                input.disabled = true;
            });
            randomOracle.disabled = true;
            randomOracle.checked = false;
        } else if(gameModeSelect.value != "Duo") 
        {
            oracleCheckBoxes.forEach( function (input) 
            {
                input.disabled = false;
            });
            randomOracle.disabled = false;
        }

        updateButton.disabled = false;
        startGameButton.disabled = true;
    }
    
    var closeLobbyButton = document.getElementById("closeLobbyButton");

    closeLobbyButton.addEventListener("click", function () {
        closeSession(sessionId);
    });
}

// Leaves the session and SignalR Group
const leaveButton = document.getElementById("leaveButton");

if (leaveButton) {

    leaveButton.addEventListener("click", function () {
        const userId = this.getAttribute("player-Id");
        leaveSession(userId, sessionId);
    });
}

document.querySelectorAll('.kickButton').forEach(button => {
    button.addEventListener('click', function () {

        const userId = button.getAttribute("player-Id");
        leaveSession(userId, sessionId);
    });
});

window.onload = function() {
    console.log("Creating session, onload");
    function waitForConnection() {
        if (isConnected) {
            createSession(sessionId);
        } else {
            setTimeout(waitForConnection, 100);
        }
    }

    waitForConnection();
}

