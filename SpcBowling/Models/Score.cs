using System;
using System.ComponentModel.DataAnnotations;

namespace SpcBowling.Models
{
    public class Score : IComparable
    {
        public int ScoreID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        
        [Required]
        [Range(0, 300)]
        [Display(Name="Bowling Score")]
        public int? BowlingScore { get; set; }

        [Required]
        [Range(0, 500)]
        [Display(Name="Handicap Score")]
        public int HandiScore { get; set; }

        public int PlayerID { get; set; }

        // navigational property for code-first entity framework
        public virtual Player Player { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            Score otherScore = (Score)obj;
            if (this.BowlingScore < otherScore.BowlingScore)
                return -1;
            else if (this.BowlingScore == otherScore.BowlingScore)
                return 0;
            else
                return 1;
        }
    }
}