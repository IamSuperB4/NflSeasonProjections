using NFL.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Database
{
    public class Conference
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual List<Division> Divisions { get; set; }
    }
}
