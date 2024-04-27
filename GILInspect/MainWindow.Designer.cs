namespace GILInspect
{
    partial class MainWindow
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
            menuStrip1 = new MenuStrip();
            ファイルfToolStripMenuItem = new ToolStripMenuItem();
            MenuOpenLibrary = new ToolStripMenuItem();
            MenuOpenRuntime = new ToolStripMenuItem();
            DisplayBox = new TextBox();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { ファイルfToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(884, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // ファイルfToolStripMenuItem
            // 
            ファイルfToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuOpenLibrary, MenuOpenRuntime });
            ファイルfToolStripMenuItem.Name = "ファイルfToolStripMenuItem";
            ファイルfToolStripMenuItem.Size = new Size(73, 20);
            ファイルfToolStripMenuItem.Text = " ファイル(&F) ";
            // 
            // MenuOpenLibrary
            // 
            MenuOpenLibrary.Name = "MenuOpenLibrary";
            MenuOpenLibrary.Size = new Size(195, 22);
            MenuOpenLibrary.Text = "ライブラリファイルを開く(&O)";
            MenuOpenLibrary.Click += MenuOpenLibrary_Click;
            // 
            // MenuOpenRuntime
            // 
            MenuOpenRuntime.Name = "MenuOpenRuntime";
            MenuOpenRuntime.Size = new Size(195, 22);
            MenuOpenRuntime.Text = "ランタイムファイルを開く(&P)";
            MenuOpenRuntime.Click += MenuOpenRuntime_Click;
            // 
            // DisplayBox
            // 
            DisplayBox.BorderStyle = BorderStyle.FixedSingle;
            DisplayBox.Dock = DockStyle.Fill;
            DisplayBox.Font = new Font("HackGen35Nerd Console", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            DisplayBox.Location = new Point(0, 24);
            DisplayBox.Multiline = true;
            DisplayBox.Name = "DisplayBox";
            DisplayBox.ScrollBars = ScrollBars.Vertical;
            DisplayBox.Size = new Size(884, 537);
            DisplayBox.TabIndex = 1;
            DisplayBox.WordWrap = false;
            // 
            // MainWindow
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(884, 561);
            Controls.Add(DisplayBox);
            Controls.Add(menuStrip1);
            Font = new Font("HackGen35Nerd Console", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            MainMenuStrip = menuStrip1;
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GILInspect Tool";
            Load += MainWindow_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem ファイルfToolStripMenuItem;
        private TextBox DisplayBox;
        private ToolStripMenuItem MenuOpenLibrary;
        private ToolStripMenuItem MenuOpenRuntime;
    }
}