using NFL.Common;
using NFL.Database.Models;
using NFL.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Business.Calculations
{
    /// <summary>
    /// Functions to attach a rank to all teams in a division or conference
    ///     - Runs all necessary tiebreakers to create rankings
    /// 
    /// Tiebreaking procedures come from the official NFL website:
    ///     - https://www.nfl.com/standings/tie-breaking-procedures
    /// </summary>
    public class Tiebreakers
    {
        static List<TeamDto> allTeams = new();
        static List<TeamDto> allConferenceTeams = new();

        public Tiebreakers(List<TeamDto> _allTeams)
        {
            allTeams = _allTeams;
        }

        /// <summary>
        /// Attach a ConferenceRank to each team in the conference
        /// </summary>
        /// <param name="conferenceTeams">List of teams in the conference</param>
        /// <returns>Returns List of conferenceTeams with a ConferenceRank attached</returns>
        public static List<TeamDto> OrderConferenceRanks(List<TeamDto> conferenceTeams)
        {
            allConferenceTeams = conferenceTeams;

            // First, determine division ranks
            List<string> divisions = conferenceTeams.Select(t => t.Division).Distinct().ToList();

            foreach (string division in divisions)
            {
                List<TeamDto> divisionTeams = conferenceTeams.Where(t => t.Division == division).ToList();

                divisionTeams = OrderDivisionRanks(divisionTeams);

                foreach (TeamDto team in divisionTeams)
                {
                    int index = conferenceTeams.FindIndex(t => t.FullName == team.FullName);

                    if (index != -1)
                        conferenceTeams[index] = team;
                }
            }


            // Next, do conference tiebreakers
            conferenceTeams = conferenceTeams.OrderByDescending(t => t.ConferenceWinPercentage).ToList();

            List<TeamDto> tiedTeams = new();
            int rank = 1;

            foreach (TeamDto team in conferenceTeams)
            {
                // if tiedTeams is empty
                if (tiedTeams?.Count < 1)
                {
                    tiedTeams.Add(team);
                    continue;
                }
                // if this team is tied with a team in the tiedTeams
                else if (team.ConferenceWinPercentage == tiedTeams?.FirstOrDefault()?.ConferenceWinPercentage)
                {
                    tiedTeams.Add(team);
                }
                // if there is no tie
                else
                {
                    // if there was a tie between previous teams
                    if (tiedTeams?.Count > 1)
                    {
                        tiedTeams = BreakTies(tiedTeams, rank, "conference");

                        foreach (TeamDto tiedTeam in tiedTeams)
                        {
                            TeamDto? rankedTeam = conferenceTeams?.FirstOrDefault(t => t.FullName == tiedTeam.FullName);

                            rankedTeam = tiedTeam;
                        }
                    }
                    // set rank
                    else
                    {
                        TeamDto? rankedTeam = conferenceTeams?.FirstOrDefault(t => t.FullName == tiedTeams?.FirstOrDefault()?.FullName);

                        if (rankedTeam != null)
                            rankedTeam.ConferenceRank = rank;
                    }

                    // there either was no tie or the tiebreakers have been ran, so clear tiedTeams and add this team to the empty list
                    tiedTeams?.Clear();
                    tiedTeams?.Add(team);
                }
            }

            return conferenceTeams;
        }

        /// <summary>
        /// Attach a DivisionRank to each team in the division
        /// </summary>
        /// <param name="conferenceTeams">List of teams in the division</param>
        /// <returns>Returns List of divisionTeams with a DivisionRank attached</returns>
        public static List<TeamDto> OrderDivisionRanks(List<TeamDto> divisionTeams)
        {
            divisionTeams = divisionTeams.OrderByDescending(t => t.DivisionWinPercentage).ToList();

            List<TeamDto> tiedTeams = new();
            int rank = 1;

            foreach (TeamDto team in divisionTeams)
            {
                // if tiedTeams is empty
                if (tiedTeams?.Count < 1)
                {
                    tiedTeams.Add(team);
                    continue;
                }
                // if this team is tied with a team in the tiedTeams
                else if (team.DivisionWinPercentage == tiedTeams?.FirstOrDefault()?.DivisionWinPercentage)
                {
                    tiedTeams.Add(team);
                }
                // if there is no tie
                else
                {
                    // if there was a tie between previous teams
                    if (tiedTeams?.Count > 1)
                    {
                        tiedTeams = BreakTies(tiedTeams, rank, "division");

                        foreach (TeamDto tiedTeam in tiedTeams)
                        {
                            TeamDto? rankedTeam = divisionTeams?.FirstOrDefault(t => t.FullName == tiedTeam.FullName);

                            rankedTeam = tiedTeam;
                        }
                    }
                    // set rank
                    else
                    {
                        TeamDto? rankedTeam = divisionTeams?.FirstOrDefault(t => t.FullName == tiedTeams?.FirstOrDefault()?.FullName);

                        if (rankedTeam != null)
                            rankedTeam.DivisionRank = rank;
                    }

                    // there either was no tie or the tiebreakers have been ran, so clear tiedTeams and add this team to the empty list
                    tiedTeams?.Clear();
                    tiedTeams?.Add(team);
                }
            }

            return divisionTeams;
        }

        #region Run All Tiebreakers

        /// <summary>
        /// Runs tiebreakers for 2 teams and resulting ranks placed in the TeamDto
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <param name="rank">Highest rank available</param>
        /// <param name="type">"conference" or "division"</param>
        /// <returns>Teams with rank placed in the TeamDto</returns>
        private static List<TeamDto> BreakTies(List<TeamDto> teams, int rank, string type)
        {
            List<TeamDto> teamsLeft = teams;
            List<TeamDto> tiebreakerWinners = new();

            int tiebreakerNumber = 1;
            int teamsBeforeTiebreaker;

            // run tiebreakers until all rankings have been determined
            while (tiebreakerWinners.Count < teams.Count)
            {
                // run through all tiebreakers
                while (teamsLeft.Count > 1)
                {
                    teamsBeforeTiebreaker = teamsLeft.Count;

                    // teams left after tiebreaker is ran
                    teamsLeft = RunTiebreaker(teamsLeft, tiebreakerNumber, type);

                    // if any teams have been removed from this round of tiebreakers, start back at first tiebreaker
                    if (teamsLeft.Count < teamsBeforeTiebreaker)
                        tiebreakerNumber = 1;
                    else
                        tiebreakerNumber++;
                }

                TeamDto? tiebreakerWinner = teamsLeft.FirstOrDefault();

                if (tiebreakerWinner != null)
                {
                    // add rank to TeamDto and 
                    if (type == "division")
                        tiebreakerWinner.DivisionRank = rank + tiebreakerWinners.Count;
                    else
                        tiebreakerWinner.ConferenceRank = rank + tiebreakerWinners.Count;

                    // add to list of tiebreakerWinners
                    tiebreakerWinners.Add(tiebreakerWinner);
                }
            }

            return tiebreakerWinners;
        }

        #endregion


        #region Run Tiebreakers

        /// <summary>
        /// Run designated tiebreaker for teams based on tiebreaker number, tiebreaker type (division or conference), and number of teams involved in tiebreaker
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <param name="tiebreaker">Tiebreaker number</param>
        /// <param name="type">"conference" or "division"</param>
        /// <returns>Highest teams remaining team(s)</returns>
        /// <exception cref="ArgumentOutOfRangeException">Incorrect tiebreaker type or tiebreaker number</exception>
        private static List<TeamDto> RunTiebreaker(List<TeamDto> teams, int tiebreakerNumber, string type) => (type, tiebreakerNumber, teams.Count) switch
        {
            // Division tiebreakers (2-team and multi-team are the same)
            ("division", 1, 2) => HeadToHeadTieBreaker(teams, type),
            ("division", 2, 2) => DivisionWinPercentageTieBreaker(teams),
            ("division", 3, 2) => CommonGamesTieBreaker(teams, type),
            ("division", 4, 2) => ConferenceWinPercentageTieBreaker(teams),
            ("division", 5, 2) => StrengthOfVictoryTieBreaker(teams),
            ("division", 6, 2) => StrengthOfScheduleTieBreaker(teams),
            ("division", 7, 2) => PointsRankingTieBreaker(teams, true),
            ("division", 8, 2) => PointsRankingTieBreaker(teams, false),
            ("division", 9, 2) => NetPointsTiebreaker(teams, "common"),
            ("division", 10, 2) => NetPointsTiebreaker(teams, "conference"),
            ("division", > 11, 2) => CoinFlipTieBreaker(teams),

            // 2-team conference tiebreakers
            ("conference", 1, 2) => HeadToHeadTieBreaker(teams, type),
            ("conference", 2, 2) => ConferenceWinPercentageTieBreaker(teams),
            ("conference", 3, 2) => CommonGamesTieBreaker(teams, "conference"),
            ("conference", 4, 2) => StrengthOfVictoryTieBreaker(teams),
            ("conference", 5, 2) => StrengthOfScheduleTieBreaker(teams),
            ("conference", 6, 2) => PointsRankingTieBreaker(teams, true),
            ("conference", 7, 2) => PointsRankingTieBreaker(teams, false),
            ("conference", 8, 2) => NetPointsTiebreaker(teams, "common"),
            ("conference", 9, 2) => NetPointsTiebreaker(teams, "conference"),
            ("conference", > 9, 2) => CoinFlipTieBreaker(teams),

            // Multi-team conference tiebreakers
            ("conference", 1, > 2) => HighestDivisionRanksTieBreaker(teams),
            ("conference", 2, > 2) => HeadToHeadTieBreaker(teams, type),
            ("conference", 3, > 2) => ConferenceWinPercentageTieBreaker(teams),
            ("conference", 4, > 2) => CommonGamesTieBreaker(teams, "conference"),
            ("conference", 5, > 2) => StrengthOfVictoryTieBreaker(teams),
            ("conference", 6, > 2) => StrengthOfScheduleTieBreaker(teams),
            ("conference", 7, > 2) => PointsRankingTieBreaker(teams, true),
            ("conference", 8, > 2) => PointsRankingTieBreaker(teams, false),
            ("conference", 9, > 2) => NetPointsTiebreaker(teams, "common"),
            ("conference", 10, > 2) => NetPointsTiebreaker(teams, "conference"),
            ("conference", > 10, > 2) => CoinFlipTieBreaker(teams),

            _ => throw new ArgumentOutOfRangeException(nameof(tiebreakerNumber), $"Not expected value: type = {type} or tiebreakerNumber = {tiebreakerNumber}")
        };

        #endregion


        #region Universal Tiebreakers

        /// <summary>
        /// Runs a Head-To-Head tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wordings:
        ///     - Head-to-head (best won-lost-tied percentage in games between the clubs) OR
        ///     - Head-to-head (best won-lost-tied percentage in games among the clubs) OR
        ///     - Head-to-head sweep. (Applicable only if one club has defeated each of the
        ///         others or if one club has lost to each of the others.)
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <param name="type">"conference" or "division"</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> HeadToHeadTieBreaker(List<TeamDto> teams, string type)
        {
            IDictionary<TeamDto, float> teamHeadToHeadWinPercentages = new Dictionary<TeamDto, float>();

            // calculate win percentage for each team in head to head games
            foreach (TeamDto team in teams)
            {
                Tuple<float, int> headToHeadResults = CalculateWinPercentageAgainstTeams(team, teams.Select(t => t.FullName).Where(t => t != team.FullName).ToHashSet());

                float winPercentage = headToHeadResults.Item1;
                int numGames = headToHeadResults.Item2;

                // in a conference tiebreaker, only applies if team has won or lost against all teams in tiebreaker
                if (type == "conference"
                    && numGames < teams.Count - 1
                    && (winPercentage >= 1.0 || winPercentage <= 0.0))
                    continue;

                teamHeadToHeadWinPercentages.Add(team, winPercentage);
            }


            // if conference tiebreaker
            //  - win sweep = that team wins tiebreaker
            //  - loss sweep = that team is eliminated from tiebreaker
            if (type == "conference")
            {
                // if there are no teams in Dictionary, there is no team who had a head to head win/loss sweep
                if (teamHeadToHeadWinPercentages.Count > 0)
                {
                    foreach (TeamDto team in teamHeadToHeadWinPercentages.Keys)
                    {
                        if (teamHeadToHeadWinPercentages[team] >= 1.0)
                            return new List<TeamDto>() { team };

                        else if (teamHeadToHeadWinPercentages[team] <= 0.0)
                           teams.Remove(team);
                    }
                }

                return teams;
            }


            // division tiebreakers eliminate teams without highest head-to-head win percentage
            List<TeamDto> tieBreakerWinners = new();

            float highestWinPercentage = teamHeadToHeadWinPercentages.Values.Max();

            // teams with the highest head to head win percentage stay in contention
            foreach(TeamDto team in teams)
            {
                if (teamHeadToHeadWinPercentages[team] >= highestWinPercentage)
                    tieBreakerWinners.Add(team);
            }

            return tieBreakerWinners;
        }

        /// <summary>
        /// Runs a Common Games tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wording:
        ///     - Best won-lost-tied percentage in common games
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <param name="type">"conference" or "division"</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> CommonGamesTieBreaker(List<TeamDto> teams, string type)
        {
            IDictionary<TeamDto, float> teamCommonGamesWinPercentages = new Dictionary<TeamDto, float>();

            HashSet<string> commonOpponents = CommonOpponents(teams);

            // calculate win percentage for each team in head to head games
            foreach (TeamDto team in teams)
            {
                Tuple<float, int> commonGamesResults = CalculateWinPercentageAgainstTeams(team, commonOpponents);

                float winPercentage = commonGamesResults.Item1;
                int numGames = commonGamesResults.Item2;

                // in a conference tiebreaker, if there are not 4 common games, skip tiebreaker
                if (type == "conference"
                    && numGames < 4)
                    return teams;

                teamCommonGamesWinPercentages.Add(team, winPercentage);
            }


            // eliminate teams without highest head-to-head win percentage
            List<TeamDto> tieBreakerWinners = new();

            float highestWinPercentage = teamCommonGamesWinPercentages.Values.Max();

            // teams with the highest head to head win percentage stay in contention
            foreach (TeamDto team in teams)
            {
                if (teamCommonGamesWinPercentages[team] >= highestWinPercentage)
                    tieBreakerWinners.Add(team);
            }

            return tieBreakerWinners;
        }


        /// <summary>
        /// Runs a Strength of Victory tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wording:
        ///     - Strength of victory in all games
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> StrengthOfVictoryTieBreaker(List<TeamDto> teams)
        {
            List<TeamDto> tieBreakerWinners = new();

            float highestWinPercentage = teams.Select(t => t.StrengthOfVictory).Max();

            // teams with the highest head to head win percentage stay in contention
            foreach (TeamDto team in teams)
            {
                if (team.StrengthOfVictory >= highestWinPercentage)
                    tieBreakerWinners.Add(team);
            }

            return tieBreakerWinners;
        }


        /// <summary>
        /// Runs a Strength of Schedule tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wording:
        ///     - Strength of schedule in all games
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> StrengthOfScheduleTieBreaker(List<TeamDto> teams)
        {
            List<TeamDto> tieBreakerWinners = new();

            float highestWinPercentage = teams.Select(t => t.StrengthOfVictory).Max();

            // teams with the highest head to head win percentage stay in contention
            foreach (TeamDto team in teams)
            {
                if (team.StrengthOfVictory >= highestWinPercentage)
                    tieBreakerWinners.Add(team);
            }

            return tieBreakerWinners;
        }


        /// <summary>
        /// Runs a Strength of Schedule tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wordings:
        ///     - Best combined ranking among conference teams in points scored and points allowed in all games OR
        ///     - Best combined ranking among all teams in points scored and points allowed in all games
        /// 
        /// *Official wording of how this tiebreaker is calculated
        ///     - To determine the best combined ranking among conference team's in points scored and points allowed,
        ///         add a team's position in the two categories, and the lowest score wins. For example, if Team A
        ///         is first in points scored and second in points allowed, its combined ranking is "3." If Team B
        ///         is third in points scored and first in points allowed, its combined ranking is "4." Team A then
        ///         wins the tiebreaker. If two teams are tied for a position, both teams are awarded the ranking
        ///         as if they held it solely. For example, if Team A and Team B are tied for first in points scored,
        ///         each team is assigned a ranking of "1" in that category, and if Team C is third, its ranking will
        ///         still be "3."
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> PointsRankingTieBreaker(List<TeamDto> teams, bool isConferencePoints)
        {
            List<TeamDto> tieBreakerWinners = new();

            // first tiebreaker is rankings against conference, second tiebreaker is rankings against entire league
            List<TeamDto> rankingTeams = isConferencePoints ? allConferenceTeams : allTeams;

            Dictionary<TeamDto, int> pointsScoredRank = new();
            Dictionary<TeamDto, int> pointsAllowedRank = new();
            List<KeyValuePair<TeamDto, int>> combinedRank = new();

            int previousValue = 0;
            int rank = 1;

            // Points scored
            rankingTeams = rankingTeams.OrderByDescending(t => t.PointsScored).ToList();

            for (int i = 1; i <= rankingTeams.Count; i++)
            {
                TeamDto team = rankingTeams[i];

                // only change rank when lower points scored value is found
                if (team.PointsScored < previousValue)
                    rank = i;

                previousValue = team.PointsScored;

                pointsScoredRank.Add(team, rank);
            }

            previousValue = 10000;
            rank = 1;

            // Points allowed
            rankingTeams = rankingTeams.OrderBy(t => t.PointsAllowed).ToList();

            for (int i = 1; i <= rankingTeams.Count; i++)
            {
                TeamDto team = rankingTeams[i];

                // only change rank when higher points allowed value is found
                if (team.PointsAllowed > previousValue)
                    rank = i;

                previousValue = team.PointsAllowed;

                pointsScoredRank.Add(team, rank);
            }

            // Combined
            foreach (TeamDto team in rankingTeams)
            {
                if (teams.Select(t => t.FullName).Contains(team.FullName))
                    combinedRank.Add(new KeyValuePair<TeamDto, int>(team, pointsScoredRank[team] + pointsAllowedRank[team]));
            }

            // order ranks of tiebreaker teams
            combinedRank.Sort((x1, x2) => x1.Value.CompareTo(x2.Value));

            int bestRank = combinedRank.FirstOrDefault().Value;

            // only add tiebreaker teams with best ranks
            tieBreakerWinners.AddRange(combinedRank.Where(r => r.Value <= bestRank).Select(r => r.Key));

            return tieBreakerWinners;
        }

        /// <summary>
        /// Runs a net points tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wordings:
        ///     - Best net points in common games OR
        ///     - Best net points in conference game OR
        ///     - Best net points in all games
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <param name="tiebreakerType">"commom", "conference", "all"</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> NetPointsTiebreaker(List<TeamDto> teams, string tiebreakerType)
        {
            List<TeamDto> tieBreakerWinners = new();

            HashSet<string>? opponents = null;

            if (tiebreakerType == "common")
                opponents = CommonOpponents(teams);
            else if (tiebreakerType == "conference")
                opponents = allConferenceTeams.Select(t => t.FullName).ToHashSet();

            List<KeyValuePair<TeamDto, int>> netPoints = new();

            foreach (TeamDto team in teams)
            {
                netPoints.Add(new KeyValuePair<TeamDto, int>(team, NetPointsAgainstTeams(team, opponents)));
            }

            // order net points of tiebreaker teams in reverse (descending) order
            netPoints.Sort((x1, x2) => -x1.Value.CompareTo(x2.Value));

            int mostNetPoints = netPoints.FirstOrDefault().Value;

            // only add tiebreaker teams with best ranks
            tieBreakerWinners.AddRange(netPoints.Where(r => r.Value <= mostNetPoints).Select(r => r.Key));

            return tieBreakerWinners;
        }

        /// <summary>
        /// Runs a coin flip tiebreaker and returns highest team remaining
        /// 
        /// *Official wordings:
        ///     - Coin toss
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Highest teams remaining team as a List</returns>
        private static List<TeamDto> CoinFlipTieBreaker(List<TeamDto> teams)
        {
            return new List<TeamDto>() { CoinFlip(teams) };
        }

        #endregion


        #region Division Tiebreakers

        /// <summary>
        /// Runs a Division Win Percentage tiebreaker and returns highest team(s) remaining
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> DivisionWinPercentageTieBreaker(List<TeamDto> teams)
        {
            List<TeamDto> tieBreakerWinners = new();

            float highestWinPercentage = teams.Select(t => t.DivisionWinPercentage).Max();

            // teams with the highest head to head win percentage stay in contention
            foreach (TeamDto team in teams)
            {
                if (team.DivisionWinPercentage >= highestWinPercentage)
                    tieBreakerWinners.Add(team);
            }

            return tieBreakerWinners;
        }

        /// <summary>
        /// Only highest ranked teams in their division involved in tiebreaker
        ///     - Only one team per division moves on to conference tiebreaker
        ///     
        /// *Official wording:
        ///     - Apply division tiebreaker to eliminate all but the highest ranked club in each
        ///         division prior to proceeding to step 2 (head-to-head). The original seeding
        ///         within a division upon application of the division tiebreaker remains the
        ///         same for all subsequent applications of the procedure that are necessary to
        ///         identify the two Wild-Card participants
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Remaining team(s)</returns>
        private static List<TeamDto> HighestDivisionRanksTieBreaker(List<TeamDto> teams)
        {
            List<TeamDto> highestTeamInDivisions = new();

            List<string> divisions = teams.Select(t => t.Division).Distinct().ToList();

            foreach (string division in divisions)
            {
                TeamDto? highestRankedTeamInDivision = teams.Where(t => t.Division == division).OrderBy(t => t.DivisionRank).FirstOrDefault();

                if(highestRankedTeamInDivision != null)
                    highestTeamInDivisions.Add(highestRankedTeamInDivision);
            }

            return highestTeamInDivisions;
        }

        #endregion


        #region Conference Tiebreakers

        /// <summary>
        /// Runs a Conference Win Percentage tiebreaker and returns highest team(s) remaining
        /// 
        /// *Official wording:
        ///     - Best won-lost-tied percentage in games played within the conference
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>Highest teams remaining team(s)</returns>
        private static List<TeamDto> ConferenceWinPercentageTieBreaker(List<TeamDto> teams)
        {
            List<TeamDto> tieBreakerWinners = new();

            float highestWinPercentage = teams.Select(t => t.ConferenceWinPercentage).Max();

            // teams with the highest head to head win percentage stay in contention
            foreach (TeamDto team in teams)
            {
                if (team.ConferenceWinPercentage >= highestWinPercentage)
                    tieBreakerWinners.Add(team);
            }

            return tieBreakerWinners;
        }

        #endregion


        #region Tiebreaker Helpers

        /// <summary>
        /// Calculates win percentage against a list of teams, as well as number of games compared
        ///     - Used in Head-to-head and common games tiebreakers
        /// </summary>
        /// <param name="team">Team to calculate win percentage for</param>
        /// <param name="againstTeams">Teams to calculate win percentage against</param>
        /// <returns>Tuple with win percentage and number of games</returns>
        private static Tuple<float, int> CalculateWinPercentageAgainstTeams(TeamDto team, HashSet<string> againstTeams)
        {
            int headToHeadWins = 0;
            int headToHeadLosses = 0;
            int headToHeadTies = 0;
            int countCommonGames = 0;

            foreach (GameDto game in team.Games)
            {
                if (againstTeams.Contains(game.AwayTeamName)
                    || againstTeams.Contains(game.HomeTeamName))
                {
                    string gameWinner = Helpers.CalculateGameWinner(game);

                    if (gameWinner == team.FullName)
                        headToHeadWins++;
                    else if (gameWinner == "tie")
                        headToHeadTies++;
                    else
                        headToHeadLosses++;

                    countCommonGames++;
                }
            }

            return Tuple.Create(Helpers.CalculateWinPercentage(headToHeadWins, headToHeadLosses, headToHeadTies), countCommonGames);
        }

        /// <summary>
        /// Calculates the net points against a list of teams
        ///     - Used in Best net points tiebreakers
        /// </summary>
        /// <param name="team">Team to determine net points for</param>
        /// <param name="againstTeams">List of opponents to calculate net points for. Null = all games</param>
        /// <returns>Net points against teams for team</returns>
        private static int NetPointsAgainstTeams(TeamDto team, HashSet<string>? againstTeams)
        {
            int netPoints = 0;

            foreach (GameDto game in team.Games)
            {
                if(againstTeams == null
                    || againstTeams.Contains(game.AwayTeamName)
                    || againstTeams.Contains(game.HomeTeamName))
                {
                    // this team is away team
                    if (game.AwayTeamName == team.FullName)
                        netPoints += game.AwayTeamScore - game.HomeTeamScore;
                    // this team is home team
                    else
                        netPoints += game.HomeTeamScore - game.AwayTeamScore;
                }
            }

            return netPoints;
        }

        /// <summary>
        /// Returns list of common opponents between list of teams
        /// </summary>
        /// <param name="teams">Teams involved in tiebreaker</param>
        /// <returns>List of common opponents</returns>
        private static HashSet<string> CommonOpponents(List<TeamDto> teams)
        {
            HashSet<string> commonOpponents = new();
            Dictionary<string, int> opponents = new();

            foreach (TeamDto team in teams)
            {
                HashSet<string> alreadyUsedForTeam = new();

                foreach (GameDto game in team.Games)
                {
                    // determine which team is the opponent
                    string opponent = game.AwayTeamName == team.FullName ? game.AwayTeamName : game.HomeTeamName;

                    // only count each opponent once per team (so divisional opponents aren't counted twice)
                    if (!alreadyUsedForTeam.Contains(opponent))
                    {
                        // if opponent isn't in Dictionary, add it
                        if (!opponents.ContainsKey(opponent))
                            opponents.Add(opponent, 1);
                        // if opponent is in Dictionary, add one to count
                        else
                            opponents[opponent]++;

                        alreadyUsedForTeam.Add(opponent);
                    }
                }
            }

            foreach (string opponent in opponents.Keys)
            {
                // if every team has played this opponent, add to lost of commonOpponents 
                if (opponents[opponent] >= teams.Count)
                    commonOpponents.Add(opponent);
            }

            return commonOpponents;
        }

        private static readonly Random randomGenerator = new();

        private static TeamDto CoinFlip(List<TeamDto> teams)
        {
            int randomNumber = randomGenerator.Next(teams.Count);

            return teams[randomNumber];
        }

        #endregion

    }
}
