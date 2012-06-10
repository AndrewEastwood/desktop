using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mpwsDBU
{
    public partial class excelDataView : UserControl, ICloneable
    {

        private DataTable _internalData;
        public delegate void menuEventHandler(ToolStripItem clickedItem, string layoutName);
        public menuEventHandler MenuHandler;
        private DataLayoutCollection layoutCollection;

        public excelDataView()
        {
            InitializeComponent();
            MenuHandler = this.defaultMenuHandler;
            _internalData = new DataTable();
        }

        public excelDataView(DataTable ds)
            : this()
        {
            DataSource(ds);
        }

        public excelDataView(DataTable ds, DataLayoutCollection dlc)
            : this()
        {
            DataSource(ds);
            this.layoutCollection = dlc;
        }

        public excelDataView(DataTable ds, string name)
            : this(ds)
        {
            _internalData.TableName = name;
        }

        public void DataSource(DataTable ds)
        {
            DataTable _ids = ds.Copy();

            // checked column 
            DataColumn _checkColState = new DataColumn("ST", typeof(bool));
            _ids.Columns.Add(_checkColState);

            dataGridView1.DataSource = _ids;
            this._internalData = _ids;

            // hide state column

            dataGridView1.Columns["ST"].Visible = false;
        }

        public DataLayoutCollection LayoutCollection
        {
            get { return this.layoutCollection; }
        }

        public DataTable dataLayout
        {
            get { return this._internalData; }
            set { this.DataSource(value); }
        }

        public DataGridView dgView()
        {
            return this.dataGridView1;
        }

        public DataGridViewRow[] getViewSelected()
        {
            List<DataGridViewRow> drv = new List<DataGridViewRow>();
            int[] idxs = new int[dataGridView1.SelectedRows.Count];

            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                try
                {
                    if ((bool)r.Cells["ST"].Value)
                        drv.Add(r);
                }
                catch { }
            }

            // performing data row sort
            DataGridViewRow[] drvi = new DataGridViewRow[drv.Count];
            DataGridViewRow __tmpRow = null;
            for (int rowWalkerIndex = 0, innerWalkerj = 1; rowWalkerIndex < drv.Count; rowWalkerIndex++)
            {
                for (innerWalkerj = rowWalkerIndex; innerWalkerj < drv.Count; innerWalkerj++)
                    if (drv[rowWalkerIndex].Index > drv[innerWalkerj].Index)
                    {
                        __tmpRow = drv[rowWalkerIndex];
                        drv[rowWalkerIndex] = drv[innerWalkerj];
                        drv[innerWalkerj] = __tmpRow;
                    }
            }

            // sort by row index


            return drv.ToArray();
        }

        public void SetSelectedRow(int idx, string index)
        {
            SetSelectedRows(new int[] { idx }, index);
        }

        public void SetSelectedRow(int idx)
        {

            this.SetSelectedRows(new int[] { idx });

            /*
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                this.dataGridView1.Rows[i].Selected = false;

            try
            {
                this.dataGridView1.Rows[idx].Selected = true;
            }
            catch { }
            */
        }

        public void SetSelectedRows(int[] idxs)
        {
            SetSelectedRows(idxs, _internalData.Columns[0].ColumnName);
        }
        public void SetSelectedRows(int[] idxs, string index)
        {
            string _selectcmd = "";

            // packages
            int ipp = 10;
            List<int[]> pkgs = new List<int[]>();
            List<int> _tmppkg = new List<int>();
            for (int j = 0, pp = 0; j < idxs.Length; j++)
            {
                _tmppkg.Add(idxs[j]);
                pp++;
                if (pp % ipp == 0 || j + 1 == idxs.Length)
                {
                    pkgs.Add(_tmppkg.ToArray());
                    _tmppkg.Clear();
                    pp = 0;
                }
            }


            for (int i = 0; _internalData.Rows.Count > i; i++)
                _internalData.Rows[i]["ST"] = false;

            DataRow[] drwss = null;

            foreach (int[] pkg in pkgs)
            {
                _selectcmd = "";
                for (int i = 0; i < pkg.Length; i++)
                    _selectcmd += string.Format("{0} LIKE '{1}' OR ", index, pkg[i]);
                _selectcmd = _selectcmd.Substring(0, _selectcmd.Length - 4);
                drwss = _internalData.Select(_selectcmd);

                for (int i = 0; drwss.Length > i; i++)
                {
                    _internalData.Rows[_internalData.Rows.IndexOf(drwss[i])]["ST"] = true;
                }

            }

            //DataRow[] drwss = _internalData.Select(_selectcmd);


            /*
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                //DataRowView drvv = (DataRowView)dataGridView1.Rows[i].DataBoundItem;
   
                this.dataGridView1.Rows[].Selected = false;
            }
            try
            {
                for (int i = 0; i < idxs.Length; i++)
                    this.dataGridView1.Rows[idxs[i] - 1].Selected = true;
            }
            catch { }
            */
        }

        private void contextMenuStrip_tableCommands_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            MenuHandler.Invoke(e.ClickedItem, this._internalData.TableName);
        }

        private void defaultMenuHandler(ToolStripItem clickedItem, string layoutName) { }

        public DataTable IndependentDataLayout
        {
            get
            {
                return this.dataLayout.Copy();
            }
        }

        public excelDataView Copy()
        {
            return (excelDataView)this.Clone();
        }

        #region ICloneable Members

        public object Clone()
        {
            excelDataView _clone = new excelDataView(this.dataLayout, this.LayoutCollection);
            _clone.MenuHandler = this.MenuHandler;
            return _clone;
        }

        #endregion
    }
}
