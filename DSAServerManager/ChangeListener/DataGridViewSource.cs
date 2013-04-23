using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DSAServerManager
{
    /// <summary>
    /// 
    /// </summary>
    public class DataGridViewSource : ChangeSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public DataGridViewSource(DataGridView control)
        {
            Grid = control;
            OriginValues = new Dictionary<Point, string>();
            SubscribeControlEvents();
        }

        private void SubscribeControlEvents()
        {
            Grid.RowsAdded += new DataGridViewRowsAddedEventHandler(Grid_RowsAdded);
            Grid.RowsRemoved += new DataGridViewRowsRemovedEventHandler(Grid_RowsRemoved);
            Grid.CurrentCellDirtyStateChanged += new EventHandler(Grid_CurrentCellDirtyStateChanged);
        }

        private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            CompareValues();
        }

        private void Grid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CompareValues();
        }

        private void Grid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CompareValues();
        }

        private void CompareValues()
        {
            if (Suspend) return;

            bool changed = false;
            foreach (DataGridViewRow row in Grid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    Point location = new Point(cell.ColumnIndex, cell.RowIndex);
                    string originValue = string.Empty, newValue = string.Empty;

                    if (OriginValues.ContainsKey(location))
                        originValue = OriginValues[location];

                    newValue = cell.Value + "";

                    if (originValue != newValue)
                    {
                        changed = true;
                        break;
                    }
                }

                if (changed) break;
            }

            if (changed)
                RaiseStatusChanged(ValueStatus.Dirty);
            else
                RaiseStatusChanged(ValueStatus.Clean);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            OriginValues = new Dictionary<Point, string>();

            foreach (DataGridViewRow row in Grid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    OriginValues.Add(new Point(cell.ColumnIndex, cell.RowIndex), cell.Value + "");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<Point, string> OriginValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected DataGridView Grid { get; set; }

    }
}
