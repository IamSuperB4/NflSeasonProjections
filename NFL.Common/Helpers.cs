using NFL.Dto;

namespace NFL.Common
{
    /// <summary>
    /// Helper functions that any project can call
    ///     - Does not use database and does not reference any projects other than NFL.Dto
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Determine the win percentage of a team based on number of wins, losses, and ties
        /// </summary>
        /// <param name="wins">Number of wins</param>
        /// <param name="losses">Number of losses</param>
        /// <param name="ties">Number of ties</param>
        /// <returns>Win percentage</returns>
        public static float CalculateWinPercentage(int wins, int losses, int ties = 0)
        {
            // doesn't allow divide by 0
            if (wins + losses + ties == 0)
                return -1;

            // ties count as half a win and half a loss
            return (float)(wins + ties * 0.5) / (wins + losses + ties);
        }

        /// <summary>
        /// Determine the winner of a game
        /// </summary>
        /// <param name="game">GameDto</param>
        /// <returns>Full name of winning teams or "tie"</returns>
        public static string CalculateGameWinner(GameDto game)
        {
            // home team won
            if (game.HomeTeamScore > game.AwayTeamScore)
                return game.HomeTeamName;

            // away team won
            if (game.AwayTeamScore > game.HomeTeamScore)
                return game.AwayTeamName;

            return "tie";
        }
    }
}