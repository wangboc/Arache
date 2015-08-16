using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using DevExpress.XtraReports.UI;

namespace AracheTest.Reports
{
    public partial class XtraReport3 : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraReport3()
        {
            InitializeComponent();
            SetReportDataSource();
        }

        public void SetReportDataSource(DataTable table0, DataTable table1, DataTable table2, DataTable table3)
        {
            xrLabel4.Text = DateTime.Now.ToShortDateString();
        }
        
        public void SetReportDataSource()
        {
            xrLabel4.Text = DateTime.Now.ToShortDateString();
        }

    }
}
