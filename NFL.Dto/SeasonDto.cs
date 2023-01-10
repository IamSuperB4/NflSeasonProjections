using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Dto
{
    public class SeasonDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Year { get; set; }

        public int RegularSeasonWeekCount { get; set; }

        public int PlayoffTeams { get; set; }
    }
}
