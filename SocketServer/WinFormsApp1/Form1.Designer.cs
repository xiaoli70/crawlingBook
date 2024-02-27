namespace DeBugWGSD
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            textBox1 = new TextBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            uPToolStripMenuItem = new ToolStripMenuItem();
            downToolStripMenuItem = new ToolStripMenuItem();
            cataLogToolStripMenuItem = new ToolStripMenuItem();
            okToolStripMenuItem = new ToolStripMenuItem();
            toolStripComboBox1 = new ToolStripComboBox();
            最小化ToolStripMenuItem = new ToolStripMenuItem();
            notifyIcon1 = new NotifyIcon(components);
            pictureBox1 = new PictureBox();
            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = SystemColors.ButtonHighlight;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.ContextMenuStrip = contextMenuStrip1;
            textBox1.Cursor = Cursors.IBeam;
            textBox1.Font = new Font("微软雅黑", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.ForeColor = Color.DimGray;
            textBox1.Location = new Point(-1, 0);
            textBox1.Margin = new Padding(5);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.RightToLeft = RightToLeft.No;
            textBox1.Size = new Size(670, 74);
            textBox1.TabIndex = 0;
            textBox1.Tag = "222222";
            textBox1.MouseDown += Form_MouseDown;
            textBox1.MouseMove += Form_MouseMove;
            textBox1.MouseUp += Form_MouseUp;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { uPToolStripMenuItem, downToolStripMenuItem, cataLogToolStripMenuItem, okToolStripMenuItem, toolStripComboBox1, 最小化ToolStripMenuItem });
            contextMenuStrip1.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(241, 143);
            // 
            // uPToolStripMenuItem
            // 
            uPToolStripMenuItem.Name = "uPToolStripMenuItem";
            uPToolStripMenuItem.Size = new Size(240, 22);
            uPToolStripMenuItem.Text = "UP";
            uPToolStripMenuItem.Click += uPToolStripMenuItem_Click;
            // 
            // downToolStripMenuItem
            // 
            downToolStripMenuItem.Name = "downToolStripMenuItem";
            downToolStripMenuItem.Size = new Size(240, 22);
            downToolStripMenuItem.Text = "Down";
            downToolStripMenuItem.Click += downToolStripMenuItem_Click;
            // 
            // cataLogToolStripMenuItem
            // 
            cataLogToolStripMenuItem.Name = "cataLogToolStripMenuItem";
            cataLogToolStripMenuItem.Size = new Size(240, 22);
            cataLogToolStripMenuItem.Text = "CataLog";
            cataLogToolStripMenuItem.Click += cataLogToolStripMenuItem_Click;
            // 
            // okToolStripMenuItem
            // 
            okToolStripMenuItem.Name = "okToolStripMenuItem";
            okToolStripMenuItem.Size = new Size(240, 22);
            okToolStripMenuItem.Text = "Ok";
            okToolStripMenuItem.Click += okToolStripMenuItem_Click;
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.DropDownWidth = 280;
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(180, 25);
            // 
            // 最小化ToolStripMenuItem
            // 
            最小化ToolStripMenuItem.Name = "最小化ToolStripMenuItem";
            最小化ToolStripMenuItem.Size = new Size(240, 22);
            最小化ToolStripMenuItem.Text = "最小化";
            最小化ToolStripMenuItem.Click += 最小化ToolStripMenuItem_Click;
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "搜狗输入法2";
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBox1.BackColor = SystemColors.ControlLightLight;
            pictureBox1.Location = new Point(644, -6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(26, 55);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = SystemColors.ActiveCaptionText;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(669, 49);
            Controls.Add(pictureBox1);
            Controls.Add(textBox1);
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private ToolStripMenuItem uPToolStripMenuItem;
        private ToolStripMenuItem downToolStripMenuItem;
        private ToolStripMenuItem cataLogToolStripMenuItem;
        private ToolStripMenuItem okToolStripMenuItem;
        public ContextMenuStrip contextMenuStrip1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripMenuItem 最小化ToolStripMenuItem;
        private NotifyIcon notifyIcon1;
        private PictureBox pictureBox1;
    }
}