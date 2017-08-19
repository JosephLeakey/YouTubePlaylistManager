﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTPM
{
    public class PlaylistGrid : System.Windows.Forms.DataGridView
    {
        private int i = -1;

        private int current = -1;

        private int visible = 0;

        public bool shuffle = false;

        private List<int> queue = new List<int>();

        private ContextMenu menu = new ContextMenu();

        public PlaylistGrid()
        {
            AllowUserToAddRows = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;

            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            RowHeadersVisible = false;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            ScrollBars = ScrollBars.Vertical;

            SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Columns.Add("index", "#");
            Columns.Add("ID", "ID");
            Columns.Add("videoName", "Name");
            Columns.Add("uploader", "Uploader");
            Columns.Add("description", "Description");
            Columns.Add("length", "Length");

            Columns[1].Visible = false;
            Columns[4].Visible = false;

            Columns[0].Width = 50;
            Columns[2].Width = 390;
            Columns[3].Width = 200;
            Columns[5].Width = 100;

            foreach (DataGridViewColumn column in Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

                if (column.Index != 0)
                {
                    column.ReadOnly = true;
                }

                if (column.Index == 2)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }

            CellMouseClick += new DataGridViewCellMouseEventHandler(PlaylistGrid_MouseClick);
            CellDoubleClick += new DataGridViewCellEventHandler(PlaylistGrid_DoubleClick);
            CellEndEdit += new DataGridViewCellEventHandler(PlaylistGrid_EndEdit);
            UserDeletingRow += new DataGridViewRowCancelEventHandler(PlaylistGrid_UserDeletingRow);
            RowsRemoved += new DataGridViewRowsRemovedEventHandler(PlaylistGrid_RowsRemoved);
            EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(PlaylistGrid_EditingControlShowing);
            SortCompare += new DataGridViewSortCompareEventHandler(PlaylistGrid_SortCompare);

            menu.MenuItems.Add(new MenuItem("Delete", new EventHandler(DeleteMenuItemEventHandler)));
        }

        public void AddVideo(string ID, object[] details, bool format)
        {
            if (details.Length < 4) { throw new RankException(); }

            if (format)
            {
                int time = (int)details[3];
                string timeString = string.Empty;

                if (time / 86400 > 0)
                {
                    timeString += string.Format("{0:00}", time / 86400) + ":";

                    time = time % 86400;
                }

                if (time / 3600 > 0)
                {
                    timeString += string.Format("{0:00}", time / 3600) + ":";

                    time = time % 3600;
                }

                timeString += string.Format("{0:00}", time / 60) + ":";

                time = time % 60;

                timeString += string.Format("{0:00}", time);

                Rows.Add(RowCount + 1, ID, details[0], details[1], details[2], timeString);
            }
            else
            {
                Rows.Add(RowCount + 1, ID, details[0], details[1], details[2], details[3]);
            }

            visible += 1;
        }

        public void Advance()
        {
            if (!shuffle)
            {
                queue.Clear();

                if (current >= RowCount - 1) { current = 0; } else { current += 1; }
            }
            else
            {
                if (queue.Count == 0) { queue.Add(current); }

                int index = queue.IndexOf(current);

                if (queue.Count >= RowCount)
                {
                    if (index == queue.Count - 1) { current = queue[0]; } else { current = queue[index + 1]; } return;
                }

                if (index < queue.Count - 1) { current = queue[index + 1]; return; }

                Random RNGesus = new Random(); int random;
                do { random = RNGesus.Next(0, RowCount); } while (random == current || queue.Contains(random));

                current = random;

                queue.Add(random);
            }
        }

        public void Retreat()
        {
            if (!shuffle)
            {
                queue.Clear();

                if (current <= 0) { current = RowCount - 1; } else { current -= 1; }
            }
            else
            {
                if (queue.Count == 0) { queue.Add(current); }

                int index = queue.IndexOf(current);

                if (queue.Count >= RowCount)
                {
                    if (index == 0) { current = queue[queue.Count - 1]; } else { current = queue[index - 1]; } return;
                }

                if (index > 0) { current = queue[index - 1]; return; }

                Random RNGesus = new Random(); int random;
                do { random = RNGesus.Next(0, RowCount); } while (random == current || queue.Contains(random));

                current = random;

                queue.Insert(0, random);
            }
        }

        private void PlaylistGrid_MouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Rows[e.RowIndex].Selected = true;

                menu.Show(this, this.PointToClient(MousePosition));
            }
        }

        public void SelectCurrentRow()
        {
            if (current < 0) { return; }

            ClearSelection(); Rows[current].Selected = true;
        }

        private void PlaylistGrid_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ClearSelection();

            Rows[e.RowIndex].Selected = true;

            if (e.ColumnIndex == 0)
            {
                CurrentCell = Rows[e.RowIndex].Cells[0];

                BeginEdit(true);

                return;
            }

            current = e.RowIndex;
        }

        private void PlaylistGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= CheckKeyPress;
            e.Control.KeyPress += CheckKeyPress;
        }

        private void CheckKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Enter) { e.Handled = true; } else { e.Handled = false; }
        }

        private void PlaylistGrid_EndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Rows[e.RowIndex].Cells[0].Value == null) { Rows[e.RowIndex].Cells[0].Value = e.RowIndex + 1; return; }

            MoveRow(e.RowIndex, int.Parse(Rows[e.RowIndex].Cells[0].Value.ToString()) - 1);
        }

        private void MoveRow(int source, int target)
        {
            if (target < 0) { target = 0; } else if (target >= RowCount) { target = RowCount - 1; }

            if (target == source) { return; }

            if (queue.Contains(source)) { queue[queue.IndexOf(source)] = -1; }

            if (source == current)
            {
                current = target;
            }
            else if (target > source && current > source && current <= target)
            {
                current -= 1;
            }
            else if (target < source && current < source && current >= target)
            {
                current += 1;
            }

            string cache = Rows[source].Cells[1].Value.ToString();

            if (target > source)
            {
                for (int i = source + 1; i <= target; i++)
                {
                    Rows[i].Cells[0].Value = i;

                    if (queue.Contains(i)) { queue[queue.IndexOf(i)] -= 1; }
                }
            }
            else
            {
                for (int i = source - 1; i >= target; i--)
                {
                    Rows[i].Cells[0].Value = i + 2;

                    if (queue.Contains(i)) { queue[queue.IndexOf(i)] += 1; }
                }
            }

            Rows[source].Cells[0].Value = target + 1;

            if (queue.Contains(-1)) { queue[queue.IndexOf(-1)] = target; }

            Sort(Columns[0], ListSortDirection.Ascending);

            /*
            if (searchBox.ForeColor != SystemColors.ControlDark && searchBox.TextLength > 0)
            {
                string search = GetSearchText();

                bool visible = false;

                if (search == playlistGrid.Rows[target].Cells[0].Value.ToString()) { visible = true; }

                if (!visible)
                {
                    if (search.Length > 10 && search.Substring(search.Length - 11) == playlistGrid.Rows[target].Cells[1].Value.ToString()) { visible = true; }

                    if (!visible)
                    {
                        search = search.ToLower();

                        for (int i = 2; i < 4; i++)
                        {
                            if (!visible && i != 1 && playlistGrid.Rows[target].Cells[i].Value.ToString().ToLower().Contains(search))
                            {
                                visible = true;
                            }
                        }

                        if (!visible)
                        {
                            visibleCount -= 1;

                            if (visibleCount == 0)
                            {
                                SetSearchBar(0);
                            }
                            else
                            {
                                playlistGrid.Rows[target].Visible = false;

                                return;
                            }
                        }
                    }
                }
            }
            */

            ClearSelection();

            Rows[target].Selected = true;
        }

        private void PlaylistGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DeletionPreparation();
        }

        private void DeleteMenuItemEventHandler(object sender, EventArgs e)
        {
            DeletionPreparation();

            foreach (DataGridViewRow row in SelectedRows)
            {
                Rows.Remove(row);
            }
        }

        private void DeletionPreparation()
        {
            if (i == -1)
            {
                i = SelectedRows[0].Index;

                int displacement = 0;

                foreach (DataGridViewRow row in SelectedRows)
                {
                    if (row.Index < i) { i = row.Index; }

                    if (row.Index < current) { displacement += 1; }

                    if (queue.Contains(row.Index)) { queue.Remove(row.Index); }
                }

                current -= displacement;

                foreach (int index in queue)
                {
                    if (index > i)
                    {
                        displacement = 0;

                        foreach (DataGridViewRow row in SelectedRows)
                        {
                            if (index > row.Index) { displacement += 1; }
                        }

                        queue[queue.IndexOf(index)] -= displacement;
                    }
                }

                visible -= SelectedRows.Count;
            }
        }

        private void PlaylistGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (SelectedRows.Count == 0)
            {
                for (int c = 0; c < RowCount; c++)
                {
                    if (c >= i)
                    {
                        Rows[c].Cells[0].Value = c + 1;
                    }
                }

                i = -1;

                /*
                if (visible == 0)
                {
                    string search = GetSearchText();

                    if (search.Length < 11 || !framework.VideoExists(search.Substring(search.Length - 11)))
                    {
                        SetSearchBar(0);
                    }
                    else { Search(); }
                }
                */
            }
        }

        private void PlaylistGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString())); e.Handled = true;
        }

        public int GetCurrentIndex() { return current; }

        public int GetVisibleCount() { return visible; }
    }
}