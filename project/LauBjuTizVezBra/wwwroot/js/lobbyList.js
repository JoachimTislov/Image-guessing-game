import {joinSession} from "./gameHub.js";

document.querySelectorAll(".joinButton").forEach(button => {
    button.addEventListener("click", () => {
        var sessionId = button.id;
        console.log("hello");
        joinSession(sessionId);
    })
})

