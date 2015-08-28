using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using DevExpress.XtraReports.UI;

namespace AracheTest.Reports
{
    public partial class XtraReport2 : XtraReport
    {
        private ReportContent Content;
        public ReportContent GetReportContent()
        {
            return Content;
        }

        public void SetReportContent(ReportContent _content)
        {
            Content = _content;
            xrLabel1.Text = Content.Title;
            this.CreateDocument();
        }

        public XtraReport2()
        {
            InitializeComponent();
            Content = new ReportContent();
            Content.Title = "国网浙江慈溪市供电公司非居民用户电费复核单据 （二次抄表）";
        }

        public void SetReportDataSource(DataTable dt0, DataTable dt1, DataTable dt2, DataTable dt3)
        {
            xrLabel4.Text = DateTime.Now.ToShortDateString();
            xrTableCell18.Text = dt0.Rows[0]["MID"].ToString();
            xrTableCell25.Text = dt0.Rows[1]["MID"].ToString();
            xrTableCell67.Text = dt0.Rows[2]["MID"].ToString();
            xrTableCell60.Text = dt0.Rows[3]["MID"].ToString();
            xrTableCell53.Text = dt0.Rows[4]["MID"].ToString();

            xrTableCell19.Text = dt0.Rows[0]["MName"].ToString();
            xrTableCell26.Text = dt0.Rows[1]["MName"].ToString();
            xrTableCell68.Text = dt0.Rows[2]["MName"].ToString();
            xrTableCell61.Text = dt0.Rows[3]["MName"].ToString();
            xrTableCell54.Text = dt0.Rows[4]["MName"].ToString();

            xrTableCell20.Text = dt0.Rows[0]["PresentShown"].ToString();
            xrTableCell27.Text = dt0.Rows[1]["PresentShown"].ToString();
            xrTableCell69.Text = dt0.Rows[2]["PresentShown"].ToString();
            xrTableCell62.Text = dt0.Rows[3]["PresentShown"].ToString();
            xrTableCell55.Text = dt0.Rows[4]["PresentShown"].ToString();

            xrTableCell22.Text = dt0.Rows[0]["PreviousShown"].ToString();
            xrTableCell29.Text = dt0.Rows[1]["PreviousShown"].ToString();
            xrTableCell71.Text = dt0.Rows[2]["PreviousShown"].ToString();
            xrTableCell64.Text = dt0.Rows[3]["PreviousShown"].ToString();
            xrTableCell57.Text = dt0.Rows[4]["PreviousShown"].ToString();

            xrTableCell23.Text = dt0.Rows[0]["Rate"].ToString();
            xrTableCell30.Text = dt0.Rows[1]["Rate"].ToString();
            xrTableCell72.Text = dt0.Rows[2]["Rate"].ToString();
            xrTableCell65.Text = dt0.Rows[3]["Rate"].ToString();
            xrTableCell58.Text = dt0.Rows[4]["Rate"].ToString();

            xrTableCell24.Text = dt0.Rows[0]["WPP"].ToString();
            xrTableCell31.Text = dt0.Rows[1]["WPP"].ToString();
            xrTableCell73.Text = dt0.Rows[2]["WPP"].ToString();
            xrTableCell66.Text = dt0.Rows[3]["WPP"].ToString();
            xrTableCell59.Text = dt0.Rows[4]["WPP"].ToString();

            xrTableCell83.Text = dt1.Rows[0][0].ToString();
            xrTableCell84.Text = dt1.Rows[0][1].ToString();
            xrTableCell85.Text = dt1.Rows[0][2].ToString();
            xrTableCell86.Text = dt1.Rows[0][3].ToString();
            xrTableCell87.Text = dt1.Rows[0][4].ToString();
            xrTableCell88.Text = dt1.Rows[0][5].ToString();
            xrTableCell89.Text = dt1.Rows[0][6].ToString();
            xrTableCell90.Text = dt1.Rows[0][7].ToString();
            xrTableCell91.Text = dt1.Rows[0][8].ToString();

            xrTableCell101.Text = dt2.Rows[0][0].ToString();
            xrTableCell102.Text = dt2.Rows[0][1].ToString();
            xrTableCell103.Text = dt2.Rows[0][2].ToString();
            xrTableCell104.Text = dt2.Rows[0][3].ToString();
            xrTableCell105.Text = dt2.Rows[0][4].ToString();
            xrTableCell106.Text = dt2.Rows[0][5].ToString();
            xrTableCell107.Text = dt2.Rows[0][6].ToString();
            xrTableCell108.Text = dt2.Rows[0][7].ToString();
            xrTableCell109.Text = dt2.Rows[0][8].ToString();

            xrTableCell117.Text = dt3.Rows[0][0].ToString();
            xrTableCell118.Text = dt3.Rows[0][1].ToString();
            xrTableCell119.Text = dt3.Rows[0][2].ToString();
            xrTableCell120.Text = dt3.Rows[0][3].ToString();
            xrTableCell121.Text = dt3.Rows[0][4].ToString();
            xrTableCell122.Text = dt3.Rows[0][5].ToString();
            xrTableCell123.Text = dt3.Rows[0][6].ToString();
            xrTableCell124.Text = dt3.Rows[1][0].ToString();
            xrTableCell125.Text = dt3.Rows[1][1].ToString();
            xrTableCell126.Text = dt3.Rows[1][2].ToString();
            xrTableCell127.Text = dt3.Rows[1][3].ToString();
            xrTableCell128.Text = dt3.Rows[1][4].ToString();
            xrTableCell129.Text = dt3.Rows[1][5].ToString();
            xrTableCell130.Text = dt3.Rows[1][6].ToString();
            xrTableCell131.Text = dt3.Rows[2][0].ToString();
            xrTableCell132.Text = dt3.Rows[2][1].ToString();
            xrTableCell133.Text = dt3.Rows[2][2].ToString();
            xrTableCell134.Text = dt3.Rows[2][3].ToString();
            xrTableCell135.Text = dt3.Rows[2][4].ToString();
            xrTableCell136.Text = dt3.Rows[2][5].ToString();
            xrTableCell137.Text = dt3.Rows[2][6].ToString();
            xrTableCell150.Text = dt3.Rows[0]["Total"].ToString();

            this.CreateDocument();
        }
    }
}