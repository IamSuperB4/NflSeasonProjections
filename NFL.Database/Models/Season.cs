using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Database.Models
{
    public class Season
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public int RegularSeasonWeekCount { get; set; }

        public int PlayoffTeams { get; set; }

        public virtual List<Division> Divisions { get; set; }
    }
}
