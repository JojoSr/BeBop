using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fruixion.bebop.Gui
{
    public partial class Form1 : Form
    {
        DroneManager mgr;

        public Form1()
        {
            InitializeComponent();
            mgr = new DroneManager();
     
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (mgr.Discover())
            {
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mgr.TakeOff();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mgr.Land();
        }
    }
}
