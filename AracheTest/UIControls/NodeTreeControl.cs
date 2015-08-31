using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AracheTest.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;

namespace AracheTest.UIControls
{
    internal class NodeTreeControl
    {
        public RepositoryItemTreeListLookUpEdit NodeTreeEdit { get; set; }
        public int CurrentNodeMid { get; set; }

        public NodeTreeControl()
        {
            CurrentNodeMid = 1;
        }

        public void UpdateNodesData(List<AmmeterInfo> nodesList)
        {
            NodeTreeEdit.DataSource = nodesList;
            CurrentNodeMid = nodesList[0].MID;
            NodeTreeEdit.TreeList.CollapseAll();
        }

        public void Init()
        {
            NodeTreeEdit.TreeList.GetStateImage += TreeList_GetStateImage;
            NodeTreeEdit.TreeList.NodeCellStyle += TreeList_NodeCellStyle;
            NodeTreeEdit.TreeList.CustomDrawNodeCell += TreeList_CustomDrawNodeCell;

            var nodeTree = NodeTreeEdit.TreeList;
           
            NodeTreeEdit.TreeList.MoveFirst();
          
            NodeTreeEdit.ValueMember = "MID";
            NodeTreeEdit.DisplayMember = "Name";
 

            var columnName = nodeTree.Columns.Add();
            columnName.Caption = "节点名称";
            columnName.Name = "Name";
            columnName.Visible = true;
            columnName.FieldName = "Name";

            var columnParentID = nodeTree.Columns.Add();
            columnParentID.Caption = "ParentID";
            columnParentID.Name = "ParentID";
            columnParentID.FieldName = "ParentID";
            columnParentID.Visible = false;

            var columnPID = nodeTree.Columns.Add();
            columnPID.Caption = "PID";
            columnPID.Name = "PID";
            columnPID.FieldName = "PID";
            columnPID.Visible = false;

            var columnMID = nodeTree.Columns.Add();
            columnMID.Caption = "MID";
            columnMID.Name = "MID";
            columnMID.FieldName = "MID";
            columnMID.Visible = false;

            nodeTree.OptionsBehavior.PopulateServiceColumns = true;
            nodeTree.KeyFieldName = "MID";
            nodeTree.ParentFieldName = "ParentID";
        

            NodeTreeEdit.AutoExpandAllNodes = false;
        }

        private void TreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            Brush backBrush, foreBrush;
            if (e.Node != (sender as TreeList).FocusedNode)
            {
                backBrush = new LinearGradientBrush(e.Bounds, Color.PapayaWhip, Color.PeachPuff,
                    LinearGradientMode.ForwardDiagonal);
                foreBrush = Brushes.Black;
            }
            else
            {
                backBrush = Brushes.PeachPuff;
                foreBrush = new SolidBrush(Color.Black);
            }
            // Fill the background.
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            // Paint the node value.
            e.Graphics.DrawString(e.CellText, e.Appearance.Font, foreBrush, e.Bounds,
                e.Appearance.GetStringFormat());
            // Prohibit default painting.
            e.Handled = true;
        }

        private void TreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
//            bool isNode = Convert.ToBoolean(e.Node.GetValue("IsNode"));
//            if (isNode == false)
//                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
        }

        private void TreeList_GetStateImage(object sender, GetStateImageEventArgs e)
        {
//            bool isNode = Convert.ToBoolean(e.Node.GetValue("IsNode"));

            e.NodeImageIndex = 2;
        }

//        private void nodeTreeEdit_QueryCloseUp(object sender, CancelEventArgs e)
//        {
//            TreeListLookUpEdit treeEdit = sender as TreeListLookUpEdit;
//            var node = treeEdit.Properties.TreeList.FocusedNode;
//            if (node == null) e.Cancel = false;
//            bool isNode = Convert.ToBoolean(node.GetValue("IsNode"));
//            e.Cancel = isNode;
//        }
    }
}

