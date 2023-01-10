using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Database.Models
{
    public class Game
    {
        public int Id { get; set; }

        public int Week { get; set; }

        public DateTime StartTime { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal? Spread { get; set; } // based on home team

        public int? AwaySpreadOdds { get; set; }

        public int? HomeSpreadOdds { get; set; }

        public int? AwayMoneyLine { get; set; }

        public int? HomeMoneyLine { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public decimal? OverUnder { get; set; }

        public int? OverOdds { get; set; }

        public int? UnderOdds { get; set; }

        public bool IsPlayoffs { get; set; }

        public bool WasOvertime { get; set; }

        public int? Bet { get; set; }

        public int AwayTeamId { get; set; }

        public int AwayTeamScore { get; set; }

        public int HomeTeamId { get; set; }

        public int HomeTeamScore { get; set; }

        public int SeasonId { get; set; }

        public int PersonId { get; set; }

        public virtual Team AwayTeam { get; set; }

        public virtual Team HomeTeam { get; set; }

        public virtual Season Season { get; set; }

        public virtual Person Person { get; set; }
    }
}
