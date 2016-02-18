using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpcBowling.Models
{
    public class Player
    {
        public int PlayerID { get; set; }

        [Required]
        [Display(Name="First Name")]
        [StringLength(30, ErrorMessage="First Name can't be longer than 30")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name="Last Name")]
        [StringLength(30, ErrorMessage="Last Namecan't be longer than 30")]
        public string LastName { get; set; }

        [Required]
        [Display(Name="Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:###-###-####}")]
        public string PhoneNumber { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Display(Name = "Profile Picture")]
        public byte[] ImageData { get; set; }

        [StringLength(50)]
        [Display(Name = "File Type")]
        public string ImageMimeType { get; set; }

        [Display(Name="Cumulative Average")]
        public int Average 
        {
            get
            {
                if (Scores == null || Scores.Count() == 0)
                    return 0;
                else
                    return Convert.ToInt32(Scores.Where(id => id.PlayerID == PlayerID).Average(s => s.BowlingScore));
            }
        }

        [Display(Name="Handicap")]
        public int Handicap
        {
            get { return CalculateHandicap(null); }
        }

        [Display(Name="Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public int CalculcateAverage(DateTime? from, DateTime? to)
        {
            int average = 0;

            try
            {
                if (Scores.Count() > 0)
                {
                    int scoreSum = 0;

                    // extract all of a player's Scores
                    var scoresRange = from s in Scores
                                      where s.PlayerID == PlayerID
                                      select s;

                    // if from & to values are specified.. then filter the score by the date ranges
                    if (from != null && to != null)
                        scoresRange = Scores.Where(s => (s.Date.Date >= from.Value.Date) && (s.Date.Date <= to.Value.Date));

                    // if no score exists in the specified range.. then average is 0
                    if (scoresRange == null)
                        average = 0;
                    else
                    {
                        // calculate the sum of all the players' selected scores
                        foreach (var s in scoresRange)
                            scoreSum += (int)s.BowlingScore;

                        // calculate the average score
                        average = (int)Math.Floor((decimal)scoreSum / scoresRange.Count());
                    }
                }
            }
            catch (Exception ex)
            {
                // log the exception in the log
            }

            return average;
        }
        public int CalculateHandicap(int? avg)
        {
            int handi = 0;

            if (avg == null)
                avg = Average;

            // if a player has not entered any scores yet or avg is higher than handi index 220
            if (avg == 0 || avg >= 220)       
                return handi;

            if (Gender == Gender.Male)  // for male, handicap is calculated as (220 - average) * 80%
                handi = Convert.ToInt32((220 - avg) * 0.80);
            else if (Gender == Gender.Female)   // for female, handicap is calculated as (220 - average) * 90%
                handi = Convert.ToInt32((220 - avg) * 0.90);

            return handi;
        }

        // navigational property for code-first entity framework
        virtual public ICollection<Score> Scores { get; set; }
    }

    public enum Gender { Male, Female }
}