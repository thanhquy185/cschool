using System;

namespace cschool.Models;

public class Score
{
    public int Student_id{ get; set; }
    public int Subject_id { get; set; }
    
    public float ScoreSubject { get; set; }
    
    public Score(int maHS, int maMH, float diem)
    {
        Student_id = maHS;
        Subject_id = maMH;
        ScoreSubject = diem;
    }
}