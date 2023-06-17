using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Content;
using Android.Views;
using System;
using SQLite;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;

namespace MineSweeper
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        int width; // width of game board
        int height; // height of game board
        int numofbombs; // number of bombs in game
        EditText editText, editText2,editText3; // width, height, nomber of bombs
        Button btn1,btn2,btn3,btn4,btn5; // beginner, intermediate, expert, high score, custom
        Android.Content.ISharedPreferences sharedPreferences;
        string PhotoPath; // saves path of selected photo

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Android.Content.Intent intent = new Intent(this, typeof(MediaService)); // creates service for background music
            StartService(intent); // starts the background music

            var db = new SQLiteConnection(Helper.Path());
            db.CreateTable<Score>();  // create table for database

            // setup elements
            editText = FindViewById<EditText>(Resource.Id.editText1);
            editText2 = FindViewById<EditText>(Resource.Id.editText2);
            editText3 = FindViewById<EditText>(Resource.Id.editText3);
            btn1 = FindViewById<Button>(Resource.Id.button1);
            btn1.Click += Btn1_Click;
            btn2 = FindViewById<Button>(Resource.Id.button2);
            btn2.Click += Btn2_Click;
            btn3 = FindViewById<Button>(Resource.Id.button3);
            btn3.Click += Btn3_Click;
            btn4 = FindViewById<Button>(Resource.Id.button4);
            btn4.Click += Btn4_Click;
            btn5 = FindViewById<Button>(Resource.Id.button5);
            btn5.Click += Btn5_Click;
            RegisterForContextMenu(btn5);

            // retrive saved settings from shared preferences
            sharedPreferences = this.GetSharedPreferences("custom", Android.Content.FileCreationMode.Private);
            string spheight = sharedPreferences.GetString("height", null);
            string spwidth = sharedPreferences.GetString("width", null);
            string spnumofbombs = sharedPreferences.GetString("numofbombs", null);

            // put saved settings in edit texts
            editText.Text = spwidth;
            editText2.Text = spheight;
            editText3.Text = spnumofbombs;


        }


        private void Btn1_Click(object sender, EventArgs e) // create beginner game
        {
            Game(9, 9, 10);
        }
        private void Btn2_Click(object sender, EventArgs e) // create intermediate game
        {
            Game(16, 16, 40);
        }
        private void Btn3_Click(object sender, EventArgs e) // create expert game
        {
            Game(16, 30, 99);
        }
        private void Btn4_Click(object sender, EventArgs e) // opens highscore page
        {
            Intent intent = new Intent(this, typeof(HighScoreFragmentActivity));
            StartActivity(intent);
        }
        private void Btn5_Click(object sender, System.EventArgs e) // create custom game
        {
            width = int.Parse(editText.Text); // get game width
            height = int.Parse(editText2.Text); // get game height
            numofbombs = int.Parse(editText3.Text); // get game number of bombs

            var editor = sharedPreferences.Edit(); // setup editor to save settings in shared preferences

            //save settings
            editor.PutString("width", editText.Text);
            editor.PutString("height", editText2.Text);
            editor.PutString("numofbombs", editText3.Text);
            editor.Commit();

            Game(width, height, numofbombs); // create game with custom settings
        }
        private void Game(int width,int height,int numofbombs) // create new game and send to game page
        {
            if (numofbombs < 1 || numofbombs > width * height) // makes sure user hasn't chosen an impossible number of bombs
            {
                Toast.MakeText(this, "bombs should be between 1 and " + width * height, ToastLength.Long).Show(); // shows error message
            }
            else
            {
                // sends variables to game page and starts game
                Intent intent = new Intent(this, typeof(Game));
                intent.PutExtra("width", width);
                intent.PutExtra("height", height);
                intent.PutExtra("numofbombs", numofbombs);
                intent.PutExtra("photopath", PhotoPath);
                StartActivity(intent);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) // asks user for permissions
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo) // create context menu to choose between camera or gallery
        {
            base.OnCreateContextMenu(menu, v, menuInfo);
            MenuInflater.Inflate(Resource.Layout.menu1, menu);
        }

        public override bool OnMenuItemSelected(int featureId, IMenuItem item) // send to camera or gallery based on user choice
        {
            if (item.ItemId == Resource.Id.Cam) // send to camera
            {
                TakePhotoAsync(true);
                return true;
            }

            if (item.ItemId == Resource.Id.Gal) // send to gallery
            {
                TakePhotoAsync(false);
                return true;
            }

            return false;

        }

        private async Task TakePhotoAsync(bool iscam) // takes user to canera or gallery
        {
            try
            {
                
                if (iscam == true) // camera
                {
                    var photo = await MediaPicker.CapturePhotoAsync();
                    await LoadPhotoAsync(photo); // save selected photo
                }
                else // gallery
                {
                    var photo = await MediaPicker.PickPhotoAsync();
                    await LoadPhotoAsync(photo); // save selected photo
                }
                   
                  
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device  
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        private async Task LoadPhotoAsync(FileResult photo) // save photo and save path
        {
            // canceled  
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage  
            var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
            await stream.CopyToAsync(newStream);

            PhotoPath = newFile; // save photo path
        }
        
    }
}