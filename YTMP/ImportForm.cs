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

        private Dictionary<String, object> DJSON;

        private const string addressBoxText = "Paste a YouTube playlist's URL in here to import it...";

        public ImportForm(Framework framework)
        {
            InitializeComponent();

            this.framework = framework;
        }

        private string GetAddress()
        {
            string address = addressBox.Text;

            while (address.Length > 34 && address[address.Length - 1] == char.Parse("/"))
            {
                address = address.Substring(0, address.Length - 1);
            }

            return address;
        }

        private void addressBox_TextChanged(object sender, EventArgs e)
        {
            string address = GetAddress();

            if (address.Length > 33)
            {
                DJSON = framework.GetDJSON(address.Substring(address.Length - 34), true);
            }
            else
            {
                DJSON = null;
            }

            if (DJSON != null && !importButton.Visible)
            {
                addressBox.Size = new Size(addressBox.Size.Width - 104, addressBox.Size.Height);
                importButton.Visible = true;
            }
            else if (DJSON == null && importButton.Visible)
            {
                importButton.Visible = false;
                addressBox.Size = new Size(addressBox.Size.Width + 104, addressBox.Size.Height);
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addressBox_Enter(object sender, EventArgs e)
        {
            if (addressBox.ForeColor == SystemColors.ControlDark)
            {
                addressBox.Clear();
                addressBox.Font = new Font(addressBox.Font, FontStyle.Regular);
                addressBox.ForeColor = SystemColors.WindowText;
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
