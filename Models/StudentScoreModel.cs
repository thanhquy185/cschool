using System;
using System.Collections.Generic;
using System.Linq;
namespace cschool.Models;

public class StudentScoreModel
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = "";
    public List<double?> OralScores {get; set;} = new();
    public List<double?> Quizzes {get; set;} = new();
    public double? MidtermScore {get; set;} = null;
    public double? FinalScore {get; set;} = null;
    public string OralScoresDisplay
    {
        get
        {
            return string.Join("\t", OralScores.Select(score => score.HasValue ? score.Value.ToString("N2") : " "));
        }
    }
    public string QuizzesDisplay
    {
        get
        {
            return string.Join("\t", Quizzes.Select(score => score.HasValue ? score.Value.ToString("N2") : " "));
        }
    }
}