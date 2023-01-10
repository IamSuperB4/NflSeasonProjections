using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Dto
{
    public class GameDto
    {
        public int Id { get; set; }

        public int Week { get; set; }

        public DateTime StartTime { get; set; }

        public float? Spread { get; set; } // based on home team

        public int? AwaySpreadOdds { get; set; }

        public int? HomeSpreadOdds { get; set; }

        public int? AwayMoneyLine { get; set; }

        public int? HomeMoneyLine { get; set; }

        public float? OverUnder { get; set; }

        public int? OverOdds { get; set; }

        public int? UnderOdds { get; set; }

        public bool IsPlayoffs { get; set; }

        public bool WasOvertime { get; set; }

        public int? Bet { get; set; }

        public string AwayTeamName { get; set; }

        public int AwayTeamScore { get; set; }

        public string HomeTeamName { get; set; }

        public int HomeTeamScore { get; set; }
    }
}
