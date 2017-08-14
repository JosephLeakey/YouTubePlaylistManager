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
            this.addButton = new System.Windows.Forms.Button();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.exportCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.addButton.Location = new System.Drawing.Point(657, 14);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(99, 31);
            this.addButton.TabIndex = 9;
            this.addButton.Text = "Import";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Visible = false;
            // 
            // searchBox
            // 
            this.searchBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.searchBox.Location = new System.Drawing.Point(14, 14);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(742, 31);
            this.searchBox.TabIndex = 8;
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
            this.ClientSize = new System.Drawing.Size(770, 79);
            this.Controls.Add(this.exportCheckBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.searchBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportForm";
            this.Text = "Import Playlist from URL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.CheckBox exportCheckBox;
    }
}