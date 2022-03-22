using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperProgrammingProject
{
    class MinesweeperCell
    {
        public bool hasMine { get; set; }
        public int numAround { get; set; }
        public bool revealed { get; set; }
        public bool flagged { get; set; }
        public Button butt;

        public MinesweeperCell(Button b)
        {
            hasMine = false;
            numAround = 0;
            revealed = false;
            flagged = false;
            butt = b;
            
        }
    }
}
