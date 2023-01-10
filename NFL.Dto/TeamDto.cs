using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NFL.Dto
{
    public class TeamDto
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string Division { get; set; }

        public string Conference { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Ties { get; set; }

        public float WinPercentage { get; set; }

        public float StrengthOfVictory { get; set; }

        public float StrengthOfSchedule { get; set; }

        public int DivisionWins { get; set; }

        public int DivisionLosses { get; set; }

        public int DivisionTies { get; set; }

        public float DivisionWinPercentage { get; set; }

        public int ConferenceWins { get; set; }

        public int ConferenceLosses { get; set; }

        public int ConferenceTies { get; set; }

        public float ConferenceWinPercentage { get; set; }

        public int PointsScored { get; set; }

        public int PointsAllowed { get; set; }

        public int DivisionRank { get; set; }

        public int ConferenceRank { get; set; }

        public List<GameDto> Games { get; set; }
    }
}
