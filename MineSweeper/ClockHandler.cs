using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineSweeper
{
    public class ClockHandler : Handler
    {
        Context context; // game context
        ImageView Left,Middle,Right; // images for time display

        public ClockHandler(Context context, ImageView Left, ImageView Middle, ImageView Right)
        {
            this.context = context;
            this.Left = Left;
            this.Middle = Middle;
            this.Right = Right;
        }

        public override void HandleMessage(Message msg) // update time display every time clock updates
        {
            int num = Resource.Drawable.Timer0;
            if (msg.Arg1.ToString().Length == 1) // if time is in single digits
            {
                // get time from tread
                num += msg.Arg1;

                // update time display
                Right.SetImageResource(num);
                Left.SetImageResource(Resource.Drawable.Timer0);
                Middle.SetImageResource(Resource.Drawable.Timer0);
            }
            else if (msg.Arg1.ToString().Length == 2) // if time is in double digits
            {
                // get time from tread
                int t1 = num + int.Parse(msg.Arg1.ToString()[0].ToString());
                int t2 = num + int.Parse(msg.Arg1.ToString()[1].ToString());

                // update time display
                Middle.SetImageResource(t1);
                Left.SetImageResource(Resource.Drawable.Timer0);
                Right.SetImageResource(t2);
            }
            else if (msg.Arg1.ToString().Length == 3) // if time is in triple digits
            {
                // get time from tread
                int t1 = num + int.Parse(msg.Arg1.ToString()[0].ToString());
                int t2 = num + int.Parse(msg.Arg1.ToString()[1].ToString());
                int t3 = num + int.Parse(msg.Arg1.ToString()[2].ToString());

                // update time display
                Left.SetImageResource(t1);
                Middle.SetImageResource(t2);
                Right.SetImageResource(t3);
            }
            else if (msg.Arg1.ToString().Length >= 4) // if time is over triple digits
            {
                // cannot update time display over triple digits
                Left.SetImageResource(Resource.Drawable.Timer9);
                Middle.SetImageResource(Resource.Drawable.Timer9);
                Right.SetImageResource(Resource.Drawable.Timer9);
            }
        }
    }
}
