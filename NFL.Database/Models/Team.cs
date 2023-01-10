using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NFL.Database.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public int DivisionId { get; set; }

        public virtual Division Division { get; set; }

        public virtual List<Game> HomeGames { get; set; }

        public virtual List<Game> AwayGames { get; set; }
    }
}
