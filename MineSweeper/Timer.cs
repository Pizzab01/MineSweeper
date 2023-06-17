using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MineSweeper
{
    public class Timer
    {
        int time; // game time
        Handler handler; // updates time display

        public bool stop { get; set; } // stop thread

        public Timer(Handler handler)
        {
            // clock handler
            this.handler = handler;
        }

        public void start() // strart thread
        {
            // create thread
            ThreadStart threadStart = new ThreadStart(Run);
            Thread t = new Thread(threadStart);

            // reset variables
            this.stop = false;
            time = 0;

            // start thread
            t.Start();
        }

        private void Run() // run thread
        {
            while(!stop)
            {
                // update time every second
                time++;
                Thread.Sleep(1000);

                // send current time to handler
                Message msg= new Message();
                msg.Arg1 = time;
                handler.SendMessage(msg);
            }
        }

        public int gettime()
        {
            // return how much time has passed
            return time;
        }
    }
}