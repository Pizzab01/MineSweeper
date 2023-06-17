using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;



namespace MineSweeper
{
    [Activity(Label = "Activity1")]
    public class Game : Activity, View.IOnClickListener, View.IOnLongClickListener
    {
        private cell[,] board; // creates the game board which is an array of cells
        private Random rnd = new Random(); // random
        private TableLayout tl; // table layout for the game board
        private TableLayout tl2; // table layout for the number of bombs indicator
        private int height; // height of game board
        private int width; // width of game board
        private int numofbombs; // number of bombs in game
        private ImageView smile,right,left,middle; // image for smile button. and images for right, left and middle sections of clock
        private bool win = false; // did the player win
        private bool FirstClick = true; // is this the first click in the game
        private ScaleGestureDetector pinchDetector; // for zoom in and out
        private ClockHandler clockHandler; // updates the clock images
        private Timer timer; // clock thread
        private int numofflags = 0; // number of flags user has placed
        private bool gameover = false; // is the game over
        private string PhotoPath; // path to custom mine sprite, selected in main activity
        private Bitmap bitmap; // bitmap for custom mine sprite

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Game);
            // setup elements
            tl = FindViewById<TableLayout>(Resource.Id.tableLayout1);
            tl2 = FindViewById<TableLayout>(Resource.Id.tableLayout2);
            smile = FindViewById<ImageView>(Resource.Id.smileiv);
            right = FindViewById<ImageView>(Resource.Id.rightiv);
            left = FindViewById<ImageView>(Resource.Id.leftiv);
            middle = FindViewById<ImageView>(Resource.Id.middleiv);
            smile.Clickable= true;
            smile.Click += Iv_Click;
            pinchDetector = new ScaleGestureDetector(this, new MyScaleListener(tl));
            clockHandler = new ClockHandler(this, left, middle, right);
            timer = new Timer(clockHandler);
            height = Intent.GetIntExtra("height",1);
            width = Intent.GetIntExtra("width",1);
            numofbombs = Intent.GetIntExtra("numofbombs",1);
            PhotoPath = Intent.GetStringExtra("photopath");

            if (PhotoPath != null) // if the photopath isn't null put photo in bitmap
            {
                Java.IO.File f = new Java.IO.File(PhotoPath);
                bitmap = BitmapFactory.DecodeFile(f.AbsolutePath);
            }

            updateflagcount(); // set number of bombs indicator
            createboard(); // create game board
            UpdateBoard(); // show game board
        }


        private void Iv_Click(object sender, EventArgs e) // starts a new game when smile button is pressed
        {
            if(!gameover) // if the user is currently in a game asks if they're sure they want to end it and start a new one
            {
                // create alert dialog
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("New Game");
                builder.SetMessage("Are you sure you want to start a new game?");
                builder.SetCancelable(true);
                builder.SetPositiveButton("yes", OkAction);
                builder.SetNegativeButton("No", CancelAction);
                AlertDialog alertDialog= builder.Create();
                alertDialog.Show();
            }
            else
            {
                newgame(); // starts new game
            }
        }

        private void CancelAction(object sender, DialogClickEventArgs e) // if user selects no in alert dailog
        {
            // does nothing
        }

        private void OkAction(object sender, DialogClickEventArgs e) // if user selects yes in alert dialog
        {
            newgame(); // starts new game
        }
        public void newgame() // start new game
        {
            FirstClick = true; // reset first click
            numofflags = 0; // reset number of flags
            timer.stop = true; // stop timer
            win = false; // reset if user won
            gameover= false; // reset if game is over
            smile.SetImageResource(Resource.Drawable.Smile); // reset smile button image

            updateflagcount(); // set number of bombs indicator
            createboard(); // create game board
            UpdateBoard(); // show game board
        }

        public void createboard() // create game board
        {
            board = new cell[height + 2, width + 2]; // create board array with buffer lines and rows(these wont be visible)

            // put cells in every slot in the board array
            for (int i = 0; i < height + 2; i++)
            {
                for (int j = 0; j < width + 2; j++)
                {
                    board[i, j] = new cell();
                }
            }

            //set number of cells in buffer rows to -1
            for (int i = 0; i < width + 2; i++)
            {
                board[height + 1, i].setnum(-1);
            }
            for (int i = 0; i < width + 2; i++)
            {
                board[0, i].setnum(-1);
            }
            for (int i = 0; i < height + 2; i++)
            {
                board[i, width + 1].setnum(-1);
            }
            for (int i = 0; i < height + 2; i++)
            {
                board[i, 0].setnum(-1);
            }
        }
        public void placebombs(int row , int col) // randomises bomb locations and generates cell numbers after first click
        {

            // place bombs
            for (int i = 0; i < numofbombs; i++)
            {
                int w = rnd.Next(1, width + 1); // randomises x coordanate exluding buffer collums
                int h = rnd.Next(1, height + 1); // randomises y coordanate exluding buffer rows
                while (board[h, w].getbomb() || isbomb(row,col,h,w)) // if coordanate already has a mine or is one of the nine cells around the first click reroll x and y coords
                {
                    w = rnd.Next(1, width + 1);
                    h = rnd.Next(1, height + 1);
                }
                board[h, w].setbomb(true); // place bomb in randomised coords
            }

            // set cell numbers
            for (int i = 1; i < height + 1; i++)
            {
                for (int j = 1; j < width + 1; j++) 
                {
                    // check all cells without bombs and set cell number based on how many bombs are touching the cell 
                    if (!board[i, j].getbomb())
                    {
                        int adjacentbombnum = 0;
                        for (int s = i - 1; s <= i + 1; s++)
                        {
                            for (int d = j - 1; d <= j + 1; d++)
                            {
                                if (board[s, d].getbomb())
                                {
                                    adjacentbombnum++;
                                }
                            }
                        }
                        board[i, j].setnum(adjacentbombnum);
                    }
                }
            }
            UpdateBoard();
        }

        public bool isbomb(int row, int col, int h, int w)
        {
            // check if there is a bomb in a certain cell or the 8 cells around it
            for (int i = row -1; i <= row +1; i++)
            {
                for(int j = col - 1; j <= col + 1; j++)
                {
                    if(i == h && j == w)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateBoard() // update the images in the board based on board information
        {
            // set cell size based on screen and board size
            var metrics = Resources.DisplayMetrics;
            float ScreenHeight = metrics.HeightPixels;
            float ScreenWidth = metrics.WidthPixels;
            ScreenHeight -= 66 * this.Resources.DisplayMetrics.Density;
            float CellSize;
            if((ScreenHeight / height) > (ScreenWidth / width))
            {
                CellSize = ScreenWidth / width;
            }
            else
            {
                CellSize = ScreenHeight / height;
            }
            CellSize /= this.Resources.DisplayMetrics.Density;

            // Clear the game board layout
            tl.RemoveAllViews();

            // Populate the game board layout with cells based off current board information
            for (int i = 1; i < height + 1; i++)
            {
                TableRow tr = new TableRow(this);

                for (int j = 1; j < width + 1; j++)
                {
                    // create cell sprite
                    ImageView iv = new ImageView(this);
                    TableRow.LayoutParams layoutParams = new TableRow.LayoutParams((int)CellSize * 2 , (int)CellSize * 2 );
                    iv.LayoutParameters = layoutParams;


                    if (board[i, j].getclick()) // if current cell has been revealed
                    {
                        if (board[i, j].getflag()) // if there is already a flag on the cell do not reveal it
                        {
                            board[i, j].setclick(false);
                        }
                        else
                        {
                            if (board[i, j].getbomb() && !win) // if current cell has been revealed and has a bomb
                            {
                                if (bitmap != null) // if there is a custom bomb sprite
                                {
                                    // set current cell sprite to custom bomb sprite
                                    iv.SetImageBitmap(bitmap);
                                    iv.SetBackgroundColor(Color.Red);
                                }
                                else
                                {
                                    // set current cell sprite to default bomb sprite
                                    iv.SetImageResource(Resource.Drawable.PressedMine);
                                }        
                            }
                            if (board[i, j].getnum() > 0) // if current cell isnt a bomb and there are bombs touching it
                            {
                                // set current cell sprite to number based on how many cells are touching it
                                string st = Resources.GetResourceName(Resource.Drawable.Cell);
                                st += board[i, j].getnum();
                                iv.SetImageResource(Resources.GetIdentifier(st, "", ""));
                            }
                            if (board[i, j].getnum() == 0 && !board[i, j].getbomb()) // if current cell isnt a bomb and there are no bombs touching it
                            {
                                // set current cell sprite to revealed cell sprite
                                iv.SetImageResource(Resource.Drawable.Cell0);
                            }
                        }
                    }
                    else if (board[i, j].getendgame()) // if game is over and current cell is an unrevealed bomb or a cell with a flag
                    {
                        if (board[i,j].getflag() && !board[i, j].getbomb()) // if current cell is not a bomb and is a cell with a flag
                        {
                            // set current cell sprite to sprite indicating incorrect flag placement
                            iv.SetImageResource(Resource.Drawable.NotMine);
                        }
                        else // if current cell is an unrevealed bomb
                        {
                            if (bitmap != null) // if there is a custom bomb sprite
                            {
                                // set current cell sprite to custom bomb sprite
                                iv.SetImageBitmap(bitmap);
                            }
                            else
                            {
                                // set current cell sprite to default bomb sprite
                                iv.SetImageResource(Resource.Drawable.Mine);
                            }
                        }
                    }
                    else if (board[i, j].getflag()) // if current cell is unrevealed and has a flag placed on it
                    {
                        // set current cell sprite to cell with flag sprite
                        iv.SetImageResource(Resource.Drawable.CellFlag);
                    }
                    else // if current cell is an unrevealed cell
                    {
                        //set current cell sprite to unrevealed cell sprite
                        iv.SetImageResource(Resource.Drawable.Cell);
                    }
                    
                    //give cell a tag indicating it's location
                    iv.Tag = i + "-" + j;

                    // make cell clickable
                    iv.SetOnClickListener(this);
                    iv.SetOnLongClickListener(this);

                    // add cell to table row
                    tr.AddView(iv);
                }

                // add table row to table layout
                tl.AddView(tr);

            }
        }

        public void ClickOnCell(int row, int col) // reveal empty cell and all adjacent cells
        {
            if(!board[row, col].getclick() && !board[row, col].getflag()) // makes sure cell hasn't already been revealed and doesn't have a flag on it
            {

                //reveal cell
                board[row, col].setclick(true);

                if ((board[row, col].getnum() == 0)) // if cell doesn't have bombs around it
                {

                    // repeat function on all adjacent cells
                    for (int s = row - 1; s <= row + 1; s++)
                    {
                        for (int d = col - 1; d <= col + 1; d++)
                        {
                            if (!(s == row && d == col))
                            {
                                ClickOnCell(s, d);
                            }
                        }
                    }
                }
            }
        }
            

        public void EndGame() // end the game
        {
            gameover = true;
            timer.stop = true; // stop timer

            // vibrate phone
            var duration = TimeSpan.FromSeconds(0.3);
            Vibration.Vibrate(duration);

            // update all cells
            for (int i = 1; i < height + 1; i++)
            {
                for (int j = 1; j < width + 1; j++)
                {
                    if ((board[i, j].getbomb() && !board[i,j].getclick()) || board[i,j].getflag()) // if current cell is an unrevealed bomb or a cell with a flag
                    {
                        board[i, j].setendgame(true);
                    }
                    else // reveal all other cells
                    {
                        board[i,j].setclick(true);
                    }
                }
            }

            if (!win) // if player has lost
            {
                // set smile button sprite to defeat sprite
                smile.SetImageResource(Resource.Drawable.DefeatSad);
            }
            else // if player has won
            {
                // set smile button sprite to victory sprite
                smile.SetImageResource(Resource.Drawable.VictorySmile);

                // check which game mode was played
                string type = "";
                if (width == 9 && height == 9)
                {
                    type = "Beginner";
                }
                else if (width == 16 && height == 16)
                {
                    type = "Intermediate";
                }
                else if (width == 16 && height == 30)
                {
                    type = "Expert";
                }

                // create database connection
                var db = new SQLiteConnection(Helper.Path());

                // get current game mode high scores from database
                string strsql = string.Format("SELECT * FROM Scores WHERE Type = '" + type + "' ORDER BY Time");
                var Scores = db.Query<Score>(strsql);

                // add current score to database if it's a new high score
                if (Scores.Count == 0 || Scores[0].Time > timer.gettime())
                {
                        Score sc = new Score(timer.gettime(), DateTime.Now.ToShortDateString(),type);
                        db.Insert(sc);
                }
            }
            UpdateBoard();
        }
        public void OnClick(View v) // when a cell is clicked
        {
            // get current cell location
            string tag = v.Tag.ToString();
            string[] arr = tag.Split(new char[] { '-' });
            int row = int.Parse(arr[0]);
            int col = int.Parse(arr[1]);

            if (FirstClick) // if this is the first cell clicked in the game
            {
                FirstClick= false;
                placebombs(row, col); // generate random bomb locations exept current cell and 8 cells around it
                ClickOnCell(row, col); // reveal current cell and all adjacent cells
                UpdateBoard();
                timer.start(); // start timer
            }
            else if (!board[row, col].getflag()) // if cell isn't a flag
            {
                if ((board[row, col].getnum() > 0)) // if cell isn't a bomb and has bombs next to it
                {
                    // reveal cell
                    board[row, col].setclick(true);
                    UpdateBoard();
                }
                else if (board[row, col].getbomb()) // if cell is a bomb
                {
                    // reveal cell
                    board[row, col].setclick(true);
                    EndGame(); // end the game
                }
                else // if cell it empty
                {
                    // reveal cell and all adjacent cells
                    ClickOnCell(row, col);
                    UpdateBoard();
                }
                if (iswin() && !board[row,col].getbomb()) // if cell was the last left to reveal and wasn't a bomb
                {
                    // end the game as a win
                    win = true;
                    EndGame();
                }
            }
        }

        public bool OnLongClick(View v) // when player wants to place or remove a flag
        {
            // vibrate device
            var duration = TimeSpan.FromSeconds(0.05);
            Vibration.Vibrate(duration);

            // get current cell location
            string tag = v.Tag.ToString();
            string[] arr = tag.Split(new char[] { '-' });
            int row = int.Parse(arr[0]);
            int col = int.Parse(arr[1]);

            if (!board[row, col].getflag() && !board[row,col].getclick()) // if current cell hasn't been revealed and doesn't have a flag
            {
                // place flag on cell
                board[row, col].setflag(true);
                numofflags++; // increase number of flags
            }
            else if (board[row, col].getflag() && !board[row,col].getclick()) // if current cell already has a flag
            {
                // remove flag from cell
                board[row, col].setflag(false);
                numofflags--; // decrease number of flags
            }
            if (iswin()) // if all bombs are covered by flags
            {
                // end game as a win
                win = true;
                EndGame();
            }
            updateflagcount();
            UpdateBoard();
            return true;
        }

        public override bool OnTouchEvent(MotionEvent e) // zoom game board in and out based on pinch
        {
            tl.OnTouchEvent(e);
            pinchDetector.OnTouchEvent(e);
            return true;
        }

        public bool iswin() // check if win conditions have been cleared
        {
            if (numofflags == numofbombs) // if all flags have been placed
            {
                for (int i = 1; i < height + 1; i++)
                {
                    for (int j = 1; j < width + 1; j++)
                    {
                        // check all cells
                        if ((!board[i,j].getbomb() && board[i, j].getflag()) || (board[i, j].getbomb() && !board[i, j].getflag())) // if a flag was placed incorrectly
                        {
                            // win conditions not cleared
                            return false;
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i < height + 1; i++)
                {
                    for (int j = 1; j < width + 1; j++)
                    {
                        // check all cells
                        if (!board[i, j].getbomb() && !board[i, j].getclick()) // if cell that is not a bomb has not been revealed
                        {
                            // win conditions not cleared
                            return false;
                        }
                    }
                }
            }
            // win conditions cleared
            return true;
        }
        public void updateflagcount() // update flag count indicator
        {
            // clear table layout
            tl2.RemoveAllViews();

            // create new table row
            TableRow tr2 = new TableRow(this);

            for (int i = 0; i < (numofbombs - numofflags).ToString().Length; i++) // update each digit in flag count
            {
                // create digit image
                ImageView iv1 = new ImageView(this);
                TableRow.LayoutParams layoutParams = new TableRow.LayoutParams((int)(26 * Resources.DisplayMetrics.Density), (int)(46 * Resources.DisplayMetrics.Density));
                iv1.LayoutParameters = layoutParams;

                // set digit number
                int t = Resource.Drawable.Timer0 + int.Parse((numofbombs - numofflags).ToString()[i].ToString());
                iv1.SetImageResource(t);

                // add digit image to table row
                tr2.AddView(iv1);
            }

            // add table row to table layout
            tl2.AddView(tr2);
        }
    }
}