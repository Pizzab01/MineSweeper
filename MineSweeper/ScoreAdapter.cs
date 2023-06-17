using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace MineSweeper
{
    class ScoreAdapter : BaseAdapter<Score>
    {
        Android.Content.Context context;
        List<Score> objects;

        public ScoreAdapter(Android.Content.Context context, System.Collections.Generic.List<Score> objects)
        {
            this.context = context;
            this.objects = objects;
        }

        public List<Score> GetList()
        {
            return this.objects;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {

            get { return this.objects.Count; }

        }

        public override Score this[int position]

        {

            get { return this.objects[position]; }

        }

        public override View GetView(int position, View convertView, ViewGroup parent) // create list display

        {
            // setup custom layout
            Android.Views.LayoutInflater layoutInflater = ((Activity)context).LayoutInflater;
            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.custom_layout, parent, false);
            TextView tvScore = view.FindViewById<TextView>(Resource.Id.tvScore);
            TextView tvDate = view.FindViewById<TextView>(Resource.Id.tvDate);
            TextView tvNum = view.FindViewById<TextView>(Resource.Id.num);

            // put values into custom layout
            Score temp = objects[position];
            if (temp != null)
            {
                tvScore.Text = "" + temp.Time;
                tvDate.Text = temp.Date;
                int pos = position + 1;
                tvNum.Text = pos + ".";            
            }
            return view;
        }
    }
}
