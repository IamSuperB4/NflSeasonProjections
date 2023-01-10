using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Database.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string? Money { get; set; }

        public virtual List<Game> Games { get; set; }
    }
}
