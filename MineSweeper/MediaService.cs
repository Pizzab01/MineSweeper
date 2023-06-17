using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    [Service]
    public class MediaService : Service
    {
        private MediaPlayer player;

        public override IBinder OnBind(Intent intent)
        {
            return null;	// must return null 
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);

            Toast.MakeText(this, "Loading", ToastLength.Short).Show();

            // start thread
            Task.Run(() =>
            {
                // load file
                player = MediaPlayer.Create(this, Resource.Raw.Music );

                // start music agian when finished
                player.Looping = true;

                // start music
                player.Start();
            });
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy() // stop music
        {
            player.Stop();
        }
    }
}