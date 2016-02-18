using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpcBowling.ViewModels
{
    public class MatchupViewModel
    {
        public int PlayerID { get; set; }
        public string FullName { get; set; }
        public int CumulativeAvg { get; set; }
        public int CumulativeHandi { get; set; }
        public int TodayAvg { get; set; }
        public int TodayHandi { get; set; }
    }
}
