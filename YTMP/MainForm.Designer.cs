namespace YTMP
{
    partial class MainForm
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
            this.player = new System.Windows.Forms.WebBrowser();
            this.playlist = new System.Windows.Forms.DataGridView();
            this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.videoName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uploader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.playlistOptionsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.importFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.importURLButton = new System.Windows.Forms.ToolStripMenuItem();
            this.playlistOptionsSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exportFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.autoPlayToggleButton = new System.Windows.Forms.ToolStripMenuItem();
            this.shuffleToggleButton = new System.Windows.Forms.ToolStripMenuItem();
            this.helpButton = new System.Windows.Forms.ToolStripMenuItem();
            this.GuideButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.AboutButton = new System.Windows.Forms.ToolStripMenuItem();
            this.minMaxToggleButton = new System.Windows.Forms.ToolStripMenuItem();
            this.onInOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.thumbnail = new System.Windows.Forms.PictureBox();
            this.videoNameLabel = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.loadFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.playlist)).BeginInit();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // player
            // 
            this.player.AllowWebBrowserDrop = false;
            this.player.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.player.IsWebBrowserContextMenuEnabled = false;
            this.player.Location = new System.Drawing.Point(770, 31);
            this.player.MinimumSize = new System.Drawing.Size(480, 270);
            this.player.Name = "player";
            this.player.ScriptErrorsSuppressed = true;
            this.player.ScrollBarsEnabled = false;
            this.player.Size = new System.Drawing.Size(480, 270);
            this.player.TabIndex = 0;
            // 
            // playlist
            // 
            this.playlist.AllowUserToAddRows = false;
            this.playlist.AllowUserToResizeColumns = false;
            this.playlist.AllowUserToResizeRows = false;
            this.playlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playlist.ColumnHeadersHeight = 25;
            this.playlist.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.playlist.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.index,
            this.ID,
            this.videoName,
            this.uploader,
            this.length});
            this.playlist.Location = new System.Drawing.Point(14, 67);
            this.playlist.MultiSelect = false;
            this.playlist.Name = "playlist";
            this.playlist.ReadOnly = true;
            this.playlist.RowHeadersVisible = false;
            this.playlist.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.playlist.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.playlist.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.playlist.Size = new System.Drawing.Size(742, 600);
            this.playlist.TabIndex = 4;
            this.playlist.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Playlist_CellDoubleClick);
            this.playlist.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.playlist_CellMouseClick);
            // 
            // index
            // 
            this.index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.index.Frozen = true;
            this.index.HeaderText = "#";
            this.index.Name = "index";
            this.index.ReadOnly = true;
            this.index.Width = 50;
            // 
            // ID
            // 
            this.ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ID.HeaderText = "ID";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Visible = false;
            // 
            // videoName
            // 
            this.videoName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.videoName.HeaderText = "Name";
            this.videoName.Name = "videoName";
            this.videoName.ReadOnly = true;
            // 
            // uploader
            // 
            this.uploader.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.uploader.HeaderText = "Uploader";
            this.uploader.Name = "uploader";
            this.uploader.ReadOnly = true;
            this.uploader.Width = 200;
            // 
            // length
            // 
            this.length.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.length.HeaderText = "Length";
            this.length.Name = "length";
            this.length.ReadOnly = true;
            this.length.Width = 75;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playlistOptionsButton,
            this.autoPlayToggleButton,
            this.shuffleToggleButton,
            this.helpButton,
            this.minMaxToggleButton});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "menuStrip1";
            // 
            // playlistOptionsButton
            // 
            this.playlistOptionsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importFileButton,
            this.importURLButton,
            this.playlistOptionsSeparator,
            this.exportFileButton});
            this.playlistOptionsButton.Name = "playlistOptionsButton";
            this.playlistOptionsButton.ShortcutKeyDisplayString = "";
            this.playlistOptionsButton.Size = new System.Drawing.Size(101, 20);
            this.playlistOptionsButton.Text = "Playlist Options";
            // 
            // importFileButton
            // 
            this.importFileButton.Name = "importFileButton";
            this.importFileButton.Size = new System.Drawing.Size(172, 22);
            this.importFileButton.Text = "Import from File...";
            this.importFileButton.Click += new System.EventHandler(this.importFileButton_Click);
            // 
            // importURLButton
            // 
            this.importURLButton.Name = "importURLButton";
            this.importURLButton.Size = new System.Drawing.Size(172, 22);
            this.importURLButton.Text = "Import from URL...";
            // 
            // playlistOptionsSeparator
            // 
            this.playlistOptionsSeparator.Name = "playlistOptionsSeparator";
            this.playlistOptionsSeparator.Size = new System.Drawing.Size(169, 6);
            // 
            // exportFileButton
            // 
            this.exportFileButton.Name = "exportFileButton";
            this.exportFileButton.Size = new System.Drawing.Size(172, 22);
            this.exportFileButton.Text = "Export to File...";
            this.exportFileButton.Click += new System.EventHandler(this.exportFileButton_Click);
            // 
            // autoPlayToggleButton
            // 
            this.autoPlayToggleButton.Name = "autoPlayToggleButton";
            this.autoPlayToggleButton.Size = new System.Drawing.Size(96, 20);
            this.autoPlayToggleButton.Text = "Auto-Play: ON";
            // 
            // shuffleToggleButton
            // 
            this.shuffleToggleButton.Name = "shuffleToggleButton";
            this.shuffleToggleButton.Size = new System.Drawing.Size(80, 20);
            this.shuffleToggleButton.Text = "Shuffle: ON";
            // 
            // helpButton
            // 
            this.helpButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GuideButton,
            this.toolStripSeparator2,
            this.AboutButton});
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(44, 20);
            this.helpButton.Text = "Help";
            // 
            // GuideButton
            // 
            this.GuideButton.Name = "GuideButton";
            this.GuideButton.Size = new System.Drawing.Size(107, 22);
            this.GuideButton.Text = "Guide";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(104, 6);
            // 
            // AboutButton
            // 
            this.AboutButton.Name = "AboutButton";
            this.AboutButton.Size = new System.Drawing.Size(107, 22);
            this.AboutButton.Text = "About";
            // 
            // minMaxToggleButton
            // 
            this.minMaxToggleButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.minMaxToggleButton.Name = "minMaxToggleButton";
            this.minMaxToggleButton.Size = new System.Drawing.Size(104, 20);
            this.minMaxToggleButton.Text = "Maximise Player";
            this.minMaxToggleButton.Click += new System.EventHandler(this.MinMaxToggleButton_Click);
            // 
            // onInOrderToolStripMenuItem
            // 
            this.onInOrderToolStripMenuItem.Name = "onInOrderToolStripMenuItem";
            this.onInOrderToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.searchBox.Location = new System.Drawing.Point(14, 31);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(742, 31);
            this.searchBox.TabIndex = 6;
            this.searchBox.Click += new System.EventHandler(this.SearchBox_Click);
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.Leave += new System.EventHandler(this.SearchBox_Leave);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.addButton.Location = new System.Drawing.Point(657, 31);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(99, 31);
            this.addButton.TabIndex = 7;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Visible = false;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // thumbnail
            // 
            this.thumbnail.BackColor = System.Drawing.Color.LightGray;
            this.thumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.thumbnail.Location = new System.Drawing.Point(770, 396);
            this.thumbnail.Name = "thumbnail";
            this.thumbnail.Size = new System.Drawing.Size(240, 135);
            this.thumbnail.TabIndex = 8;
            this.thumbnail.TabStop = false;
            // 
            // videoNameLabel
            // 
            this.videoNameLabel.AutoSize = true;
            this.videoNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.videoNameLabel.Location = new System.Drawing.Point(763, 308);
            this.videoNameLabel.Name = "videoNameLabel";
            this.videoNameLabel.Size = new System.Drawing.Size(179, 37);
            this.videoNameLabel.TabIndex = 9;
            this.videoNameLabel.Text = "Video Title";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            this.saveFileDialog.Title = "Export Playlist";
            // 
            // loadFileDialog
            // 
            this.loadFileDialog.DefaultExt = "txt";
            this.loadFileDialog.Filter = "Text Files (*.txt)|*.txt";
            this.loadFileDialog.Title = "Import Playlist";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.videoNameLabel);
            this.Controls.Add(this.thumbnail);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.playlist);
            this.Controls.Add(this.player);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1280, 720);
            this.Name = "MainForm";
            this.Text = "YTMP";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.playlist)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser player;
        private System.Windows.Forms.DataGridView playlist;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem playlistOptionsButton;
        private System.Windows.Forms.ToolStripMenuItem importFileButton;
        private System.Windows.Forms.ToolStripMenuItem importURLButton;
        private System.Windows.Forms.ToolStripSeparator playlistOptionsSeparator;
        private System.Windows.Forms.ToolStripMenuItem exportFileButton;
        private System.Windows.Forms.ToolStripMenuItem onInOrderToolStripMenuItem;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ToolStripMenuItem autoPlayToggleButton;
        private System.Windows.Forms.ToolStripMenuItem shuffleToggleButton;
        private System.Windows.Forms.ToolStripMenuItem helpButton;
        private System.Windows.Forms.ToolStripMenuItem GuideButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem AboutButton;
        private System.Windows.Forms.ToolStripMenuItem minMaxToggleButton;
        private System.Windows.Forms.PictureBox thumbnail;
        private System.Windows.Forms.DataGridViewTextBoxColumn index;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn videoName;
        private System.Windows.Forms.DataGridViewTextBoxColumn uploader;
        private System.Windows.Forms.DataGridViewTextBoxColumn length;
        private System.Windows.Forms.Label videoNameLabel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog loadFileDialog;
    }
}

