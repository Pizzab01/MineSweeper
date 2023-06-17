using System;
using System.Collections.Generic;
using System.IO;
using Android.Graphics;
using Android.Util;
using SQLite;

namespace MineSweeper
{
    public class Helper
    {
            public static string dbname = "dbTest6";
            public Helper()
            {
            }
            public static string Path() // get path for database
            {
                string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Helper.dbname);
                return path;
            }
        public static List<Score> getAllScores(string Type) // return a list of scores in acending order based on game mode
        {
            // create database connection
            var db = new SQLiteConnection(Path());

            // get the scores from database
            string strsql = string.Format("SELECT * FROM Scores WHERE Type = '" + Type + "' ORDER BY Time");
            var Scores = db.Query<Score>(strsql);

            // create list of scores
            List<Score> ScoreList = new List<Score>();
            if (Scores.Count > 0)
            {
                foreach (var item in Scores)
                {
                    ScoreList.Add(item);

                }
            }
            return ScoreList;
        }

    }    
}
