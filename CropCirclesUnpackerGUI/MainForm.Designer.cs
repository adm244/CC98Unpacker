namespace CropCirclesUnpackerGUI
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
      this.ctrlMainMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.clearArchivesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extractToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ctrlStatusBar = new System.Windows.Forms.StatusStrip();
      this.ctrlStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.ctrlMainContainer = new System.Windows.Forms.SplitContainer();
      this.ctrlSideContainer = new System.Windows.Forms.SplitContainer();
      this.ctrlTreeView = new System.Windows.Forms.TreeView();
      this.ctrlContentPanel = new System.Windows.Forms.Panel();
      this.ctrlOpenFile = new System.Windows.Forms.OpenFileDialog();
      this.ctrlContentContainer = new System.Windows.Forms.SplitContainer();
      this.btnImport = new System.Windows.Forms.Button();
      this.btnExport = new System.Windows.Forms.Button();
      this.ctrlPaletteSelector = new System.Windows.Forms.ComboBox();
      this.ctrlContentBarPanel = new System.Windows.Forms.Panel();
      this.ctrlContentBarPaletteLabel = new System.Windows.Forms.Label();
      this.ctrlMainMenu.SuspendLayout();
      this.ctrlStatusBar.SuspendLayout();
      this.ctrlMainContainer.Panel1.SuspendLayout();
      this.ctrlMainContainer.Panel2.SuspendLayout();
      this.ctrlMainContainer.SuspendLayout();
      this.ctrlSideContainer.Panel1.SuspendLayout();
      this.ctrlSideContainer.SuspendLayout();
      this.ctrlContentContainer.Panel1.SuspendLayout();
      this.ctrlContentContainer.Panel2.SuspendLayout();
      this.ctrlContentContainer.SuspendLayout();
      this.ctrlContentBarPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // ctrlMainMenu
      // 
      this.ctrlMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.ctrlMainMenu.Location = new System.Drawing.Point(0, 0);
      this.ctrlMainMenu.Name = "ctrlMainMenu";
      this.ctrlMainMenu.Size = new System.Drawing.Size(623, 24);
      this.ctrlMainMenu.TabIndex = 0;
      this.ctrlMainMenu.Text = "MainMenu";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addArchiveToolStripMenuItem,
            this.clearArchivesToolStripMenuItem,
            this.extractToToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // addArchiveToolStripMenuItem
      // 
      this.addArchiveToolStripMenuItem.Name = "addArchiveToolStripMenuItem";
      this.addArchiveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.addArchiveToolStripMenuItem.Text = "&Add archive";
      this.addArchiveToolStripMenuItem.Click += new System.EventHandler(this.addArchiveToolStripMenuItem_Click);
      // 
      // clearArchivesToolStripMenuItem
      // 
      this.clearArchivesToolStripMenuItem.Enabled = false;
      this.clearArchivesToolStripMenuItem.Name = "clearArchivesToolStripMenuItem";
      this.clearArchivesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.clearArchivesToolStripMenuItem.Text = "C&lear archives";
      this.clearArchivesToolStripMenuItem.Click += new System.EventHandler(this.clearArchivesToolStripMenuItem_Click);
      // 
      // extractToToolStripMenuItem
      // 
      this.extractToToolStripMenuItem.Enabled = false;
      this.extractToToolStripMenuItem.Name = "extractToToolStripMenuItem";
      this.extractToToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.extractToToolStripMenuItem.Text = "E&xtract to...";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
      // 
      // closeToolStripMenuItem
      // 
      this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
      this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.closeToolStripMenuItem.Text = "&Close";
      this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
      // 
      // ctrlStatusBar
      // 
      this.ctrlStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctrlStatusLabel});
      this.ctrlStatusBar.Location = new System.Drawing.Point(0, 364);
      this.ctrlStatusBar.Name = "ctrlStatusBar";
      this.ctrlStatusBar.Size = new System.Drawing.Size(623, 22);
      this.ctrlStatusBar.TabIndex = 1;
      this.ctrlStatusBar.Text = "StatusBar";
      // 
      // ctrlStatusLabel
      // 
      this.ctrlStatusLabel.Name = "ctrlStatusLabel";
      this.ctrlStatusLabel.Size = new System.Drawing.Size(42, 17);
      this.ctrlStatusLabel.Text = "Ready.";
      // 
      // ctrlMainContainer
      // 
      this.ctrlMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlMainContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.ctrlMainContainer.Location = new System.Drawing.Point(0, 24);
      this.ctrlMainContainer.Name = "ctrlMainContainer";
      // 
      // ctrlMainContainer.Panel1
      // 
      this.ctrlMainContainer.Panel1.Controls.Add(this.ctrlSideContainer);
      // 
      // ctrlMainContainer.Panel2
      // 
      this.ctrlMainContainer.Panel2.Controls.Add(this.ctrlContentContainer);
      this.ctrlMainContainer.Size = new System.Drawing.Size(623, 340);
      this.ctrlMainContainer.SplitterDistance = 207;
      this.ctrlMainContainer.TabIndex = 2;
      // 
      // ctrlSideContainer
      // 
      this.ctrlSideContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlSideContainer.Location = new System.Drawing.Point(0, 0);
      this.ctrlSideContainer.Name = "ctrlSideContainer";
      this.ctrlSideContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // ctrlSideContainer.Panel1
      // 
      this.ctrlSideContainer.Panel1.Controls.Add(this.ctrlTreeView);
      this.ctrlSideContainer.Size = new System.Drawing.Size(207, 340);
      this.ctrlSideContainer.SplitterDistance = 169;
      this.ctrlSideContainer.TabIndex = 0;
      // 
      // ctrlTreeView
      // 
      this.ctrlTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlTreeView.Location = new System.Drawing.Point(0, 0);
      this.ctrlTreeView.Name = "ctrlTreeView";
      this.ctrlTreeView.Size = new System.Drawing.Size(207, 169);
      this.ctrlTreeView.TabIndex = 0;
      this.ctrlTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ctrlTreeView_NodeMouseDoubleClick);
      // 
      // ctrlContentPanel
      // 
      this.ctrlContentPanel.AutoScroll = true;
      this.ctrlContentPanel.BackColor = System.Drawing.SystemColors.ControlDark;
      this.ctrlContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlContentPanel.Location = new System.Drawing.Point(0, 0);
      this.ctrlContentPanel.Name = "ctrlContentPanel";
      this.ctrlContentPanel.Size = new System.Drawing.Size(412, 308);
      this.ctrlContentPanel.TabIndex = 0;
      // 
      // ctrlContentContainer
      // 
      this.ctrlContentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlContentContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.ctrlContentContainer.IsSplitterFixed = true;
      this.ctrlContentContainer.Location = new System.Drawing.Point(0, 0);
      this.ctrlContentContainer.Name = "ctrlContentContainer";
      this.ctrlContentContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // ctrlContentContainer.Panel1
      // 
      this.ctrlContentContainer.Panel1.Controls.Add(this.ctrlContentBarPanel);
      this.ctrlContentContainer.Panel1.Controls.Add(this.btnExport);
      this.ctrlContentContainer.Panel1.Controls.Add(this.btnImport);
      this.ctrlContentContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
      // 
      // ctrlContentContainer.Panel2
      // 
      this.ctrlContentContainer.Panel2.Controls.Add(this.ctrlContentPanel);
      this.ctrlContentContainer.Size = new System.Drawing.Size(412, 340);
      this.ctrlContentContainer.SplitterDistance = 28;
      this.ctrlContentContainer.TabIndex = 1;
      // 
      // btnImport
      // 
      this.btnImport.Dock = System.Windows.Forms.DockStyle.Left;
      this.btnImport.Location = new System.Drawing.Point(3, 3);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(75, 22);
      this.btnImport.TabIndex = 0;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // btnExport
      // 
      this.btnExport.Dock = System.Windows.Forms.DockStyle.Left;
      this.btnExport.Location = new System.Drawing.Point(78, 3);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(75, 22);
      this.btnExport.TabIndex = 1;
      this.btnExport.Text = "Export";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // ctrlPaletteSelector
      // 
      this.ctrlPaletteSelector.Dock = System.Windows.Forms.DockStyle.Left;
      this.ctrlPaletteSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ctrlPaletteSelector.FormattingEnabled = true;
      this.ctrlPaletteSelector.Location = new System.Drawing.Point(53, 0);
      this.ctrlPaletteSelector.Margin = new System.Windows.Forms.Padding(0);
      this.ctrlPaletteSelector.Name = "ctrlPaletteSelector";
      this.ctrlPaletteSelector.Size = new System.Drawing.Size(185, 21);
      this.ctrlPaletteSelector.Sorted = true;
      this.ctrlPaletteSelector.TabIndex = 2;
      // 
      // ctrlContentBarPanel
      // 
      this.ctrlContentBarPanel.Controls.Add(this.ctrlPaletteSelector);
      this.ctrlContentBarPanel.Controls.Add(this.ctrlContentBarPaletteLabel);
      this.ctrlContentBarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlContentBarPanel.Location = new System.Drawing.Point(153, 3);
      this.ctrlContentBarPanel.Margin = new System.Windows.Forms.Padding(0);
      this.ctrlContentBarPanel.Name = "ctrlContentBarPanel";
      this.ctrlContentBarPanel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
      this.ctrlContentBarPanel.Size = new System.Drawing.Size(256, 22);
      this.ctrlContentBarPanel.TabIndex = 2;
      // 
      // ctrlContentBarPaletteLabel
      // 
      this.ctrlContentBarPaletteLabel.AutoSize = true;
      this.ctrlContentBarPaletteLabel.Dock = System.Windows.Forms.DockStyle.Left;
      this.ctrlContentBarPaletteLabel.Location = new System.Drawing.Point(10, 0);
      this.ctrlContentBarPaletteLabel.Name = "ctrlContentBarPaletteLabel";
      this.ctrlContentBarPaletteLabel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.ctrlContentBarPaletteLabel.Size = new System.Drawing.Size(43, 16);
      this.ctrlContentBarPaletteLabel.TabIndex = 3;
      this.ctrlContentBarPaletteLabel.Text = "Palette:";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(623, 386);
      this.Controls.Add(this.ctrlMainContainer);
      this.Controls.Add(this.ctrlStatusBar);
      this.Controls.Add(this.ctrlMainMenu);
      this.MainMenuStrip = this.ctrlMainMenu;
      this.Name = "MainForm";
      this.Text = "CropCircles Unpacker";
      this.ctrlMainMenu.ResumeLayout(false);
      this.ctrlMainMenu.PerformLayout();
      this.ctrlStatusBar.ResumeLayout(false);
      this.ctrlStatusBar.PerformLayout();
      this.ctrlMainContainer.Panel1.ResumeLayout(false);
      this.ctrlMainContainer.Panel2.ResumeLayout(false);
      this.ctrlMainContainer.ResumeLayout(false);
      this.ctrlSideContainer.Panel1.ResumeLayout(false);
      this.ctrlSideContainer.ResumeLayout(false);
      this.ctrlContentContainer.Panel1.ResumeLayout(false);
      this.ctrlContentContainer.Panel2.ResumeLayout(false);
      this.ctrlContentContainer.ResumeLayout(false);
      this.ctrlContentBarPanel.ResumeLayout(false);
      this.ctrlContentBarPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip ctrlMainMenu;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem addArchiveToolStripMenuItem;
    private System.Windows.Forms.StatusStrip ctrlStatusBar;
    private System.Windows.Forms.ToolStripStatusLabel ctrlStatusLabel;
    private System.Windows.Forms.SplitContainer ctrlMainContainer;
    private System.Windows.Forms.SplitContainer ctrlSideContainer;
    private System.Windows.Forms.TreeView ctrlTreeView;
    private System.Windows.Forms.Panel ctrlContentPanel;
    private System.Windows.Forms.OpenFileDialog ctrlOpenFile;
    private System.Windows.Forms.ToolStripMenuItem extractToToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem clearArchivesToolStripMenuItem;
    private System.Windows.Forms.SplitContainer ctrlContentContainer;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Button btnExport;
    private System.Windows.Forms.ComboBox ctrlPaletteSelector;
    private System.Windows.Forms.Panel ctrlContentBarPanel;
    private System.Windows.Forms.Label ctrlContentBarPaletteLabel;
  }
}

