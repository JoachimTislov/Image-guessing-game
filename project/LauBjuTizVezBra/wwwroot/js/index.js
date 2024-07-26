var recentGamesTable = document.getElementById('recentGames');
document.querySelectorAll(".leaderBoardButton").forEach(input => {
    input.addEventListener("click", () => {
        var leaderboard = input.id;
        showLeaderboard(leaderboard);
    });
});

function showLeaderboard(selectedLeaderboard) {
    document.querySelectorAll(".leaderBoard").forEach(leaderboard => {
        console.log(leaderboard.id);
        console.log(selectedLeaderboard);
        if (leaderboard.id == selectedLeaderboard) {
            leaderboard.hidden = false;
        } else {
            leaderboard.hidden = true;
        }
        recentGamesTable.hidden = false;
    });
}