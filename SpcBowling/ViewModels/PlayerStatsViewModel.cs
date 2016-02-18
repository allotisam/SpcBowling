using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpcBowling.Models;

namespace SpcBowling.ViewModels
{
    /// <summary>
    /// Used by Player Controller -> Details() functions
    /// </summary>
    public class RaiseAvgBy1ViewModel
    {
        public Player Player { get; set; }

        // calculate below two properties only if Scores.count() > 0
        public int ToRaiseAvgBy1Total       // scores needed in next 3 games to raise player's avg by 1
        {
            get
            {
                if (Player.Scores.Count() > 0)
                {
                    int AverageBy1 = Player.Average + 1;
                    int TotalScoreSoFar = Player.Scores.Sum(s => s.BowlingScore).Value;
                    int NumOfScoreSoFar = Player.Scores.Count();
                    return (AverageBy1 * (NumOfScoreSoFar + 3)) - TotalScoreSoFar;
                }
                else
                    return 0;
            }
            set { }
        }
        public int ToRaiseAvgBy1PerGame     // scores needed per game to raise player's avg by 1..(in next 3 games)
        {
            get
            {
                if (Player.Scores.Count() > 0 && (ToRaiseAvgBy1Total > 0))
                {
                    return (int)Math.Ceiling((double)ToRaiseAvgBy1Total / 3);
                }
                else
                    return 0;
            }
            set { }
        }
        public int StandardDeviation
        {
            get
            {
                if (Player.Scores.Count() > 0)
                {
                    double sum = Player.Scores.Sum(s => Math.Pow(s.BowlingScore.Value - Player.Average, 2));
                    return (int)(Math.Sqrt((sum) / (Player.Scores.Count())));
                }
                return 0;
            }
            set { }
        }
        public double StandardDeviationPercentage
        {
            get
            {
                if (Player.Scores.Count() > 0)
                {
                    int stdd = StandardDeviation;
                    int avg = Player.Average;
                    double val = (double)stdd / avg;
                    return Math.Round((val * 100), 2);
                }
                return 0.0;
            }
            set { }
        }
    }

    public class PlayerStatsViewModel
    {
        public DailyStats DailyStats { get; set; }
        public SeasonStats SeasonStats { get; set; }
    }

    public class DailyStats : Stats
    {
        public DateTime Today { get; set; }

        public DailyStats(DateTime today) { Today = today; }
        public DailyStats() { Today = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time"); }

    }
    public class SeasonStats : Stats
    {
        public string SeasonName { get; set; }

        public SeasonStats(string seasonName) { SeasonName = seasonName; }
        public SeasonStats() { }
    }
    public class Stats
    {
        public int BestScratchScore { get; set; }
        public string BestScratchScorer { get; set; }

        public int BestHandiScore { get; set; }
        public string BestHandiScorer { get; set; }

        public int BestTotal { get; set; }
        public string BestTotalPlayer { get; set; }

        public int BestTotalHandi { get; set; }
        public string BestTotalHandiPlayer { get; set; }

        public int WorstScratchScore { get; set; }
        public string WorstScratchScorer { get; set; }

        public int WorstHandiScore { get; set; }
        public string WorstHandiScorer { get; set; }
    }
}
