using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Database.Models
{
    public class Division
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ConferenceId { get; set; }

        public virtual Conference Conference { get; set; }

        public int SeasonId { get; set; }

        public virtual Season Season { get; set; }

        public virtual List<Team> Teams { get; set; }
    }
}
