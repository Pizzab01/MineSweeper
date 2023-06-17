using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Fragment;
using AndroidX.Fragment.App;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using Fragment = AndroidX.Fragment.App.Fragment;


namespace MineSweeper
{
    public class ExpertFragment : Fragment
    {
        ListView lv;
        ScoreAdapter ScoreAdapter;

        public static List<Score> ScoreList { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) // runs when fragment is created
        {
            // create view
            View view = inflater.Inflate(Resource.Layout.HighScore, container, false);

            // get list of expert scores
            ScoreList = Helper.getAllScores("Expert");

            // send list to adapter
            ScoreAdapter = new ScoreAdapter(view.Context, ScoreList);

            // create list view
            lv = view.FindViewById<ListView>(Resource.Id.listView1);

            // send list view to adapter
            lv.Adapter = ScoreAdapter;

            return view;
        }
    }
}