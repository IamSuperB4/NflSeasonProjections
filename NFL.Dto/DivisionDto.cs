using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFL.Dto
{
    public class DivisionDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Season { get; set; }

        public List<TeamDto> Teams { get; set; }
    }
}
