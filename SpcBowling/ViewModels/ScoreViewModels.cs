using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpcBowling.Models;
using System.ComponentModel.DataAnnotations;

namespace SpcBowling.ViewModels
{
    public class ScoreViewModels
    {
        public IEnumerable<Player> Players { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        //public IEnumerable<DateScoresModel> DateScores { get; set; }
    }

    /// <summary>
    /// Not In Use
    /// </summary>
    public class DateScoresModel
    {
        public DateTime Date { get; set; }
        public IEnumerable<Score> Scores { get; set; }
    }

    /// <summary>
    /// Used to calculate customer average in CustomerAverage ActionResult method in Score Controller
    /// </summary>
    public class CustomAverage
    {
        public int PlayerID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name="From")]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name="To")]
        public DateTime EndDate { get; set; }

        public int Average { get; set; }
    }
}