namespace YTMP
{
    partial class ImportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.importButton = new System.Windows.Forms.Button();
            this.addressBox = new System.Windows.Forms.TextBox();
            this.exportCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // importButton
            // 
            this.importButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.importButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.importButton.Location = new System.Drawing.Point(657, 14);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(99, 31);
            this.importButton.TabIndex = 9;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Visible = false;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // addressBox
            // 
            this.addressBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.addressBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressBox.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.addressBox.Location = new System.Drawing.Point(14, 14);
            this.addressBox.Name = "addressBox";
            this.addressBox.Size = new System.Drawing.Size(742, 31);
            this.addressBox.TabIndex = 8;
            this.addressBox.TextChanged += new System.EventHandler(this.addressBox_TextChanged);
            this.addressBox.Enter += new System.EventHandler(this.addressBox_Enter);
            this.addressBox.Leave += new System.EventHandler(this.addressBox_Leave);
            // 
            // exportCheckBox
            // 
            this.exportCheckBox.AutoSize = true;
            this.exportCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.exportCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportCheckBox.Location = new System.Drawing.Point(620, 48);
            this.exportCheckBox.Name = "exportCheckBox";
            this.exportCheckBox.Size = new System.Drawing.Size(136, 17);
            this.exportCheckBox.TabIndex = 10;
            this.exportCheckBox.Text = "Export to file after import";
            this.exportCheckBox.UseVisualStyleBackColor = true;
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 76);
            this.Controls.Add(this.exportCheckBox);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.addressBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportForm";
            this.Text = "Import Playlist from URL";
            this.Shown += new System.EventHandler(this.ImportForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.TextBox addressBox;
        private System.Windows.Forms.CheckBox exportCheckBox;
    }
}