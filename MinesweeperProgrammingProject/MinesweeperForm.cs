using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperProgrammingProject
{
    public partial class MinesweeperForm : Form
    {
        //////////////////////////////////////////
        // class constants
        private const int ROWS = 20;
        private const int COLS = 20;
        private const int BUTTON_SIZE = 25;
        private const string BOMB = "\uD83D\uDCA3";
        private const string FLAG = "\uD83D\uDEA9";
        private MinesweeperCell[,] cells = new MinesweeperCell[ROWS, COLS];
        private bool firstClick = true;
        private bool gameOver = false;
        private int timerTick = 0;
        private bool win = false;
        private int chanceMine = 85;
        Random rand = new Random();

        //////////////////////////////////////////
        // fields and properties
        private int Rows { get; set; }
        private int Cols { get; set; }

        //////////////////////////////////////////
        // constructor
        public MinesweeperForm()
        {
            InitializeComponent();
            this.Rows = ROWS;
            this.Cols = COLS;
        }

        //////////////////////////////////////////
        // event handlers
        private void MinesweeperForm_Load(object sender, EventArgs e)
        {
            // resize the form
            this.Width = BUTTON_SIZE * this.Cols + this.Cols;
            int titleHeight = this.Height - this.ClientRectangle.Height;
            this.Height = BUTTON_SIZE * this.Rows + this.Rows + titleHeight;

            // create the buttons on the form
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    // create a new button control
                    Button b = new Button();
                    // set the button width and height
                    b.Width = BUTTON_SIZE;
                    b.Height = BUTTON_SIZE;
                    // position the button on the form
                    b.Top = i * BUTTON_SIZE;
                    b.Left = j * BUTTON_SIZE;
                    // no text
                    b.Text = String.Empty;
                    // set the button style
                    b.FlatStyle = FlatStyle.Popup;
                    // add a MouseDown event handler
                    b.MouseDown += new MouseEventHandler(MinesweeperForm_MouseDown);
                    // give the button a name in "row_col" format 
                    b.Name = i + "_" + j;
                    // add the button control to the form
                    b.BackgroundImageLayout = ImageLayout.Stretch;
                    this.Controls.Add(b);
                    cells[i, j] = new MinesweeperCell(b);
                    // do other stuff here?

                }
            }


            // set up the board
            SetUpBoard();

            // set up the board
            //this.revealBoard();
            label1.Visible = false;
        }

        private void SetUpBoard()
        {

            // set up the board
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    cells[i, j].numAround = 0;
                }
            }
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    if (rand.Next(0, 100) > chanceMine)
                    {
                        cells[i, j].hasMine = true;
                        for (int l = -1; l < 2; l++)
                        {
                            for (int n = -1; n < 2; n++)
                            {
                                if (i + l >= 0 && j + n >= 0 && i + l < this.Rows && j + n < this.Cols)
                                {
                                    cells[i + l, j + n].numAround += 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        cells[i, j].hasMine = false;
                    }
                }
            }
        }

        private void MinesweeperForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Button)
            {
                Button b = (Button)sender;
                // extract the row and column from the button name
                int index = b.Name.IndexOf("_");
                int i = int.Parse(b.Name.Substring(0, index));
                int j = int.Parse(b.Name.Substring(index + 1));

                // handle mousebuttons left and right differently
                if (e.Button == MouseButtons.Left)
                {
                    // dig the position to reveal the contents 
                    revealCell(cells[i, j], i, j);
                }
                else
                {
                    // flag the position as a possible mine
                    flagCell(cells[i, j]);
                }
            }
        }

        //////////////////////////////////////////
        // instance methods
        
        private void flagCell(MinesweeperCell cell)
        {
            if (cell.flagged && !cell.revealed)
            {
                cell.flagged = false;
                cell.butt.BackColor = Color.White;
                cell.butt.BackgroundImage = null;
            }
            else if (!cell.revealed || cell.hasMine)
            {
                cell.butt.BackColor = Color.Pink;
                cell.butt.BackgroundImage = Properties.Resources.flag;
                cell.flagged = true;
            }
        }

        private void revealCell(MinesweeperCell cell, int i, int j)
        {
            if (cell.numAround > 0 && !cell.flagged && !firstClick)
            {
                // if cell is not blank
                if (cell.hasMine)
                {
                    cell.butt.BackColor = Color.Red;
                    timer1.Start();
                    gameOver = true;
                }
                else
                {
                    cell.butt.BackColor = Color.LightGreen;
                    cell.revealed = true;
                    cell.butt.Text = cell.numAround.ToString();
                }
            }
            else if (!cell.flagged && !firstClick)
            {
                for (int l = -1; l < 2; l++)
                {
                    for (int n = -1; n < 2; n++)
                    {
                        if (i + l >= 0 && j + n >= 0 && i + l < this.Rows && j + n < this.Cols)
                        {
                            if (cells[i + l, j + n].numAround < 1 && !cells[i + l, j + n].revealed)
                            {

                                cells[i + l, j + n].butt.BackColor = Color.LightGreen;
                                cells[i + l, j + n].revealed = true;
                                revealCell(cells[i + l, j + n], i + l, j + n);
                            }
                            else if (!cells[i + l, j + n].revealed)
                            {
                                cells[i + l, j + n].butt.BackColor = Color.LightGreen;
                                cells[i + l, j + n].revealed = true;
                                cells[i + l, j + n].butt.Text = cells[i + l, j + n].numAround > 0 ? cells[i + l, j + n].numAround.ToString() : "";
                            }
                        }
                    }
                }
            }
            else if (firstClick)
            {
                if (cell.numAround > 0)
                {
                    SetUpBoard();
                    revealCell(cell, i, j);
                }
                else
                {
                    firstClick = false;
                    revealCell(cell, i, j);
                }
            }

            if (isGameOver())
            {
                gameOver = true;
                win = true;
                label1.Visible = true;
                label1.Left = this.Cols * 8;
                timer1.Start();
            }
            
        }

        private bool isGameOver()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    if (!cells[i, j].revealed && !cells[i, j].hasMine)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        private void revealBoard()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Cols; j++)
                {
                    //MessageBox.Show(String.Format("{0} ==> {1}", buttons[i, j].Name.ToString(), mines[i, j].ToString()));
                    if (cells[i, j].hasMine)
                    {
                        cells[i, j].butt.Text = "x";
                    }
                    else
                    {
                        cells[i, j].butt.Text = cells[i, j].numAround.ToString();
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerTick++;
            if (gameOver && !win)
            {
                for (int i = 0; i < this.Rows; i++)
                {
                    for (int j = 0; j < this.Cols; j++)
                    {
                        cells[i, j].butt.BackColor = (timerTick % 2) > 0 ? Color.White : Color.Red;
                    }
                }
                
                if (timerTick > 10)
                {
                    this.Close();
                }
            }
            else
            {
                for (int i = 0; i < this.Rows; i++)
                {
                    for (int j = 0; j < this.Cols; j++)
                    {
                        cells[i, j].butt.BackColor = (timerTick % 2) > 0 ? Color.Yellow : Color.Green;
                    }
                }

            }
        }

    }
}
