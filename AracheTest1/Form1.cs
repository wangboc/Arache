using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AracheTest1
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();
            Rectangle rect = ClientRectangle;
            try
            {
                Image img = Image.FromFile("logo.png");
                g.DrawImage(img, new Point(100, 100));
            }
            catch { }

        }
    }
}
