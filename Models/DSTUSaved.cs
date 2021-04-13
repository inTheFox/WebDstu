using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebDstu.Models
{
    public class DSTUSaved
    {
        public int Id { get; set; }
        public int SortId { get; set; }
        public string Action { get; set; }
        public string OptionsJson { get; set; }
        public string SubActions { get; set; }
    }
}
