using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YTMP
{
    public partial class ImportForm : Form
    {
        private Framework framework;

        private const string addressBoxText = "Paste a YouTube playlist's URL in here to import it...";

        public ImportForm(Framework framework)
        {
            InitializeComponent();

            this.framework = framework;
        }

        public string GetID()
        {
            if (addressBox.ForeColor == SystemColors.ControlDark || addressBox.TextLength < 34) { return null; }

            string ID = addressBox.Text;

            while (ID.Length > 34 && ID[ID.Length - 1] == char.Parse("/"))
            {
                ID = ID.Substring(0, ID.Length - 1);
            }

            ID = ID.Substring(ID.Length - 34);

            return ID;
        }

        public bool GetCheckState()
        {
            return exportCheckBox.Checked;
        }

        private void addressBox_TextChanged(object sender, EventArgs e)
        {
            if (GetID() != null && !importButton.Visible)
            {
                addressBox.Size = new Size(addressBox.Size.Width - 104, addressBox.Size.Height);
                importButton.Visible = true;
            }
            else if (importButton.Visible)
            {
                importButton.Visible = false;
                addressBox.Size = new Size(addressBox.Size.Width + 104, addressBox.Size.Height);
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void addressBox_Enter(object sender, EventArgs e)
        {
            if (addressBox.ForeColor == SystemColors.ControlDark)
            {
                addressBox.Clear();
                addressBox.Font = new Font(addressBox.Font, FontStyle.Regular);
                addressBox.ForeColor = SystemColors.ControlLightLight;
            }
        }

        private void addressBox_Leave(object sender, EventArgs e)
        {
            if (addressBox.TextLength == 0)
            {
                addressBox.ForeColor = SystemColors.ControlDark;
                addressBox.Font = new Font(addressBox.Font, FontStyle.Italic);
                addressBox.Text = addressBoxText;
            }
        }

        private void ImportForm_Shown(object sender, EventArgs e)
        {
            exportCheckBox.Focus();
        }
    }
}
