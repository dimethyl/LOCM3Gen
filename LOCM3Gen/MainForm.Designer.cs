namespace LOCM3Gen
{
  partial class MainForm
  {
    /// <summary>
    /// Designer variable used to keep track of non-visual components.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TextBox locm3DirectoryInput;
    private System.Windows.Forms.Button locm3DirectoryButton;
    private System.Windows.Forms.Label locm3DirectoryLabel;
    private System.Windows.Forms.FolderBrowserDialog directorySelector;
    private System.Windows.Forms.Label projectDirectoryLabel;
    private System.Windows.Forms.Button projectDirectoryButton;
    private System.Windows.Forms.TextBox projectDirectoryInput;
    private System.Windows.Forms.Label projectNameLabel;
    private System.Windows.Forms.TextBox projectNameInput;
    private System.Windows.Forms.CheckBox projectSubdirectoryCheckbox;
    private System.Windows.Forms.Label familyLabel;
    private System.Windows.Forms.ComboBox familiesList;
    private System.Windows.Forms.Button generateButton;
    
    /// <summary>
    /// Disposes resources used by the form.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing) {
        if (components != null) {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }
    
    /// <summary>
    /// This method is required for Windows Forms designer support.
    /// Do not change the method contents inside the source code editor. The Forms designer might
    /// not be able to load this method if it was changed manually.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.locm3DirectoryInput = new System.Windows.Forms.TextBox();
      this.locm3DirectoryButton = new System.Windows.Forms.Button();
      this.locm3DirectoryLabel = new System.Windows.Forms.Label();
      this.directorySelector = new System.Windows.Forms.FolderBrowserDialog();
      this.projectDirectoryLabel = new System.Windows.Forms.Label();
      this.projectDirectoryButton = new System.Windows.Forms.Button();
      this.projectDirectoryInput = new System.Windows.Forms.TextBox();
      this.projectNameLabel = new System.Windows.Forms.Label();
      this.projectNameInput = new System.Windows.Forms.TextBox();
      this.projectSubdirectoryCheckbox = new System.Windows.Forms.CheckBox();
      this.familyLabel = new System.Windows.Forms.Label();
      this.familiesList = new System.Windows.Forms.ComboBox();
      this.generateButton = new System.Windows.Forms.Button();
      this.environmentsList = new System.Windows.Forms.ComboBox();
      this.environmentLabel = new System.Windows.Forms.Label();
      this.AboutLabel = new System.Windows.Forms.Label();
      this.devicesList = new System.Windows.Forms.ComboBox();
      this.deviceLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // locm3DirectoryInput
      // 
      this.locm3DirectoryInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.locm3DirectoryInput.BackColor = System.Drawing.SystemColors.Window;
      this.locm3DirectoryInput.Location = new System.Drawing.Point(16, 31);
      this.locm3DirectoryInput.Margin = new System.Windows.Forms.Padding(4);
      this.locm3DirectoryInput.Name = "locm3DirectoryInput";
      this.locm3DirectoryInput.Size = new System.Drawing.Size(387, 22);
      this.locm3DirectoryInput.TabIndex = 1;
      // 
      // locm3DirectoryButton
      // 
      this.locm3DirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.locm3DirectoryButton.Location = new System.Drawing.Point(405, 30);
      this.locm3DirectoryButton.Margin = new System.Windows.Forms.Padding(4);
      this.locm3DirectoryButton.Name = "locm3DirectoryButton";
      this.locm3DirectoryButton.Size = new System.Drawing.Size(32, 27);
      this.locm3DirectoryButton.TabIndex = 2;
      this.locm3DirectoryButton.Text = "...";
      this.locm3DirectoryButton.UseVisualStyleBackColor = true;
      this.locm3DirectoryButton.Click += new System.EventHandler(this.Locm3DirectoryButton_Click);
      // 
      // locm3DirectoryLabel
      // 
      this.locm3DirectoryLabel.AutoSize = true;
      this.locm3DirectoryLabel.Location = new System.Drawing.Point(16, 11);
      this.locm3DirectoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.locm3DirectoryLabel.Name = "locm3DirectoryLabel";
      this.locm3DirectoryLabel.Size = new System.Drawing.Size(143, 17);
      this.locm3DirectoryLabel.TabIndex = 0;
      this.locm3DirectoryLabel.Text = "libopencm3 directory:";
      // 
      // projectDirectoryLabel
      // 
      this.projectDirectoryLabel.AutoSize = true;
      this.projectDirectoryLabel.Location = new System.Drawing.Point(16, 59);
      this.projectDirectoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.projectDirectoryLabel.Name = "projectDirectoryLabel";
      this.projectDirectoryLabel.Size = new System.Drawing.Size(115, 17);
      this.projectDirectoryLabel.TabIndex = 3;
      this.projectDirectoryLabel.Text = "Project directory:";
      // 
      // projectDirectoryButton
      // 
      this.projectDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.projectDirectoryButton.Location = new System.Drawing.Point(405, 78);
      this.projectDirectoryButton.Margin = new System.Windows.Forms.Padding(4);
      this.projectDirectoryButton.Name = "projectDirectoryButton";
      this.projectDirectoryButton.Size = new System.Drawing.Size(32, 27);
      this.projectDirectoryButton.TabIndex = 5;
      this.projectDirectoryButton.Text = "...";
      this.projectDirectoryButton.UseVisualStyleBackColor = true;
      this.projectDirectoryButton.Click += new System.EventHandler(this.ProjectDirectoryButton_Click);
      // 
      // projectDirectoryInput
      // 
      this.projectDirectoryInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectDirectoryInput.Location = new System.Drawing.Point(16, 79);
      this.projectDirectoryInput.Margin = new System.Windows.Forms.Padding(4);
      this.projectDirectoryInput.Name = "projectDirectoryInput";
      this.projectDirectoryInput.Size = new System.Drawing.Size(387, 22);
      this.projectDirectoryInput.TabIndex = 4;
      // 
      // projectNameLabel
      // 
      this.projectNameLabel.AutoSize = true;
      this.projectNameLabel.Location = new System.Drawing.Point(16, 107);
      this.projectNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.projectNameLabel.Name = "projectNameLabel";
      this.projectNameLabel.Size = new System.Drawing.Size(95, 17);
      this.projectNameLabel.TabIndex = 6;
      this.projectNameLabel.Text = "Project name:";
      // 
      // projectNameInput
      // 
      this.projectNameInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectNameInput.Location = new System.Drawing.Point(16, 127);
      this.projectNameInput.Margin = new System.Windows.Forms.Padding(4);
      this.projectNameInput.MaxLength = 64;
      this.projectNameInput.Name = "projectNameInput";
      this.projectNameInput.Size = new System.Drawing.Size(418, 22);
      this.projectNameInput.TabIndex = 7;
      this.projectNameInput.TextChanged += new System.EventHandler(this.ProjectNameInput_TextChanged);
      // 
      // projectSubdirectoryCheckbox
      // 
      this.projectSubdirectoryCheckbox.AutoSize = true;
      this.projectSubdirectoryCheckbox.Checked = true;
      this.projectSubdirectoryCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.projectSubdirectoryCheckbox.Location = new System.Drawing.Point(16, 159);
      this.projectSubdirectoryCheckbox.Margin = new System.Windows.Forms.Padding(4);
      this.projectSubdirectoryCheckbox.Name = "projectSubdirectoryCheckbox";
      this.projectSubdirectoryCheckbox.Size = new System.Drawing.Size(201, 21);
      this.projectSubdirectoryCheckbox.TabIndex = 6;
      this.projectSubdirectoryCheckbox.Text = "Create project subdirectory";
      this.projectSubdirectoryCheckbox.UseVisualStyleBackColor = true;
      // 
      // familyLabel
      // 
      this.familyLabel.AutoSize = true;
      this.familyLabel.Location = new System.Drawing.Point(16, 235);
      this.familyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.familyLabel.Name = "familyLabel";
      this.familyLabel.Size = new System.Drawing.Size(145, 17);
      this.familyLabel.TabIndex = 9;
      this.familyLabel.Text = "Microcontroller family:";
      // 
      // familiesList
      // 
      this.familiesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.familiesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.familiesList.FormattingEnabled = true;
      this.familiesList.Location = new System.Drawing.Point(16, 255);
      this.familiesList.Margin = new System.Windows.Forms.Padding(4);
      this.familiesList.Name = "familiesList";
      this.familiesList.Size = new System.Drawing.Size(418, 24);
      this.familiesList.TabIndex = 10;
      this.familiesList.SelectedIndexChanged += new System.EventHandler(this.FamiliesList_SelectedIndexChanged);
      // 
      // generateButton
      // 
      this.generateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.generateButton.Location = new System.Drawing.Point(14, 343);
      this.generateButton.Margin = new System.Windows.Forms.Padding(4);
      this.generateButton.Name = "generateButton";
      this.generateButton.Size = new System.Drawing.Size(422, 28);
      this.generateButton.TabIndex = 15;
      this.generateButton.Text = "Generate or update project";
      this.generateButton.UseVisualStyleBackColor = true;
      this.generateButton.Click += new System.EventHandler(this.GenerateButton_Click);
      // 
      // environmentsList
      // 
      this.environmentsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.environmentsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.environmentsList.FormattingEnabled = true;
      this.environmentsList.Location = new System.Drawing.Point(16, 203);
      this.environmentsList.Margin = new System.Windows.Forms.Padding(4);
      this.environmentsList.Name = "environmentsList";
      this.environmentsList.Size = new System.Drawing.Size(418, 24);
      this.environmentsList.TabIndex = 8;
      // 
      // environmentLabel
      // 
      this.environmentLabel.AutoSize = true;
      this.environmentLabel.Location = new System.Drawing.Point(16, 183);
      this.environmentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.environmentLabel.Name = "environmentLabel";
      this.environmentLabel.Size = new System.Drawing.Size(138, 17);
      this.environmentLabel.TabIndex = 7;
      this.environmentLabel.Text = "Project environment:";
      // 
      // AboutLabel
      // 
      this.AboutLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.AboutLabel.AutoSize = true;
      this.AboutLabel.Cursor = System.Windows.Forms.Cursors.Help;
      this.AboutLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.AboutLabel.ForeColor = System.Drawing.SystemColors.Highlight;
      this.AboutLabel.Location = new System.Drawing.Point(395, 9);
      this.AboutLabel.Name = "AboutLabel";
      this.AboutLabel.Size = new System.Drawing.Size(45, 17);
      this.AboutLabel.TabIndex = 16;
      this.AboutLabel.Text = "About";
      this.AboutLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
      this.AboutLabel.Click += new System.EventHandler(this.AboutLabel_Click);
      // 
      // devicesList
      // 
      this.devicesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.devicesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.devicesList.FormattingEnabled = true;
      this.devicesList.Location = new System.Drawing.Point(16, 307);
      this.devicesList.Margin = new System.Windows.Forms.Padding(4);
      this.devicesList.Name = "devicesList";
      this.devicesList.Size = new System.Drawing.Size(418, 24);
      this.devicesList.TabIndex = 18;
      // 
      // deviceLabel
      // 
      this.deviceLabel.AutoSize = true;
      this.deviceLabel.Location = new System.Drawing.Point(16, 287);
      this.deviceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.deviceLabel.Name = "deviceLabel";
      this.deviceLabel.Size = new System.Drawing.Size(55, 17);
      this.deviceLabel.TabIndex = 17;
      this.deviceLabel.Text = "Device:";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(452, 379);
      this.Controls.Add(this.devicesList);
      this.Controls.Add(this.deviceLabel);
      this.Controls.Add(this.AboutLabel);
      this.Controls.Add(this.environmentsList);
      this.Controls.Add(this.environmentLabel);
      this.Controls.Add(this.generateButton);
      this.Controls.Add(this.familiesList);
      this.Controls.Add(this.familyLabel);
      this.Controls.Add(this.projectSubdirectoryCheckbox);
      this.Controls.Add(this.projectNameLabel);
      this.Controls.Add(this.projectNameInput);
      this.Controls.Add(this.projectDirectoryLabel);
      this.Controls.Add(this.projectDirectoryButton);
      this.Controls.Add(this.projectDirectoryInput);
      this.Controls.Add(this.locm3DirectoryLabel);
      this.Controls.Add(this.locm3DirectoryButton);
      this.Controls.Add(this.locm3DirectoryInput);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MaximumSize = new System.Drawing.Size(10000, 426);
      this.MinimumSize = new System.Drawing.Size(470, 426);
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "libopencm3 Project Generator";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    private System.Windows.Forms.ComboBox environmentsList;
    private System.Windows.Forms.Label environmentLabel;
    private System.Windows.Forms.Label AboutLabel;
    private System.Windows.Forms.ComboBox devicesList;
    private System.Windows.Forms.Label deviceLabel;
  }
}
