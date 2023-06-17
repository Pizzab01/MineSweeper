using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Sql;
using SQLite;


namespace MineSweeper
{
    [Table("Scores")]
    public class Score
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Time { get; set; } // score
        public string Date { get; set; } // date when score was created
        public String Type { get; set; } // what game mode: beginner, intermediate, expert

        public void setScore(int id, int time, string date, string type)
        {
            this.Id = id;
            this.Time = time;
            this.Type = type;
            this.Date = date;   
        }
        public Score()
        {

        }
        public Score(int time, string date, String type)
        {
            this.Time = time;
            this.Type = type;
            this.Date= date;    
        }

    }
}