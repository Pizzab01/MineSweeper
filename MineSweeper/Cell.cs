public class cell
{
    private bool bomb; // is there a bomb in this cell
    private bool flag; // is there a flag on this cell
    private bool click; // has this cell been revealed
    private int AdjacentBombNum; // how many bombs are touching this cell/what is this cells number
    private bool EndGame; // is this cell an unrevealed bomb when the game is over

    public cell()
    {
        bomb = false;
        flag = false;
        click = false;
        AdjacentBombNum = 0;
        EndGame = false;
    }   
    public void setbomb(bool bomb)
        { this.bomb = bomb; }
    public void setflag(bool flag)
        { this.flag = flag; }
    public void setnum(int num)
        { this.AdjacentBombNum = num; }
    public void setclick(bool click)
        { this.click = click; }
    public bool getbomb()
        { return this.bomb; }
    public bool getflag()
        { return this.flag; }
    public int getnum()
        { return this.AdjacentBombNum; }
    public bool getclick()
    { return this.click; }
    public void setendgame(bool EndGame)
        { this.EndGame = EndGame; }
    public bool getendgame()
    { return this.EndGame; }
}