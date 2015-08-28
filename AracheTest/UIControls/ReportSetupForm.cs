using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AracheTest.Reports;

namespace AracheTest.UIControls
{
    public partial class ReportSetupForm : Form
    {
        private ReportContent Content;

        public ReportSetupForm()
        {
            InitializeComponent();
            Content = new ReportContent();
        }

        public ReportContent GetReportContent()
        {
            return Content;
        }
        public void SetReportContent(ReportContent content)
        {
            Content = content;
            textBox1.Text = Content.Title;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            
            Content.Title = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
