using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Updater
{
    public class wndSettings : components.UI.Windows.wndAppSettings.wndAppSettings
    {
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_AddNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private Label label1;
        private NumericUpDown general_main_fetchTimeout;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabControl tabControl1;
        private Label label2;
        private TextBox general_main_localPath;
        private System.Windows.Forms.ToolStripButton toolStripButton_Remove;

        public wndSettings()
            : base()
        {
            InitializeComponent();

            bool _hasProfiles = false;
            Hashtable dataSyncProfiles = (Hashtable)ApplicationSettingsContext.Configuration["datasync"];
            if (dataSyncProfiles != null)
                foreach (DictionaryEntry de in dataSyncProfiles)
                {
                    // restore profiles
                    string _displayName = "";
                    try
                    {
                        _displayName = ApplicationSettingsContext.GetValueByPath<string>("datasync." + de.Key + ".sync.profileDisplayText");
                    }
                    catch { }
                    _addSettingsProfile(de.Key.ToString(), _displayName, de.Key.ToString().ToLower().Equals("profile-default"));
                    _hasProfiles = true;
                }

            // add default profile
            if (!_hasProfiles)
                _addSettingsProfile("profile-default", "", true);

            LoadControlSettings();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wndSettings));
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.general_main_fetchTimeout = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_AddNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.label2 = new System.Windows.Forms.Label();
            this.general_main_localPath = new System.Windows.Forms.TextBox();
            this.tabControl2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.general_main_fetchTimeout)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(258, 249);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Зберегти";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Location = new System.Drawing.Point(12, 12);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(340, 231);
            this.tabControl2.TabIndex = 10;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.general_main_localPath);
            this.tabPage2.Controls.Add(this.general_main_fetchTimeout);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(332, 235);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Загальні";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // general_main_fetchTimeout
            // 
            this.general_main_fetchTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.general_main_fetchTimeout.Location = new System.Drawing.Point(9, 19);
            this.general_main_fetchTimeout.Maximum = new decimal(new int[] {
            120000,
            0,
            0,
            0});
            this.general_main_fetchTimeout.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.general_main_fetchTimeout.Name = "general_main_fetchTimeout";
            this.general_main_fetchTimeout.Size = new System.Drawing.Size(317, 20);
            this.general_main_fetchTimeout.TabIndex = 0;
            this.general_main_fetchTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(215, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Час опитування віддаленої папки (мсек.)";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tabControl1);
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(332, 205);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Профілі синхронізації";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(326, 174);
            this.tabControl1.TabIndex = 12;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_AddNew,
            this.toolStripSeparator1,
            this.toolStripButton_Remove,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(326, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_AddNew
            // 
            this.toolStripButton_AddNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_AddNew.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.toolStripButton_AddNew.ForeColor = System.Drawing.Color.DarkGreen;
            this.toolStripButton_AddNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_AddNew.Image")));
            this.toolStripButton_AddNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_AddNew.Name = "toolStripButton_AddNew";
            this.toolStripButton_AddNew.Size = new System.Drawing.Size(85, 22);
            this.toolStripButton_AddNew.Text = "Новий профіль";
            this.toolStripButton_AddNew.ToolTipText = "Add New";
            this.toolStripButton_AddNew.Click += new System.EventHandler(this.toolStripButton_AddNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton_Remove
            // 
            this.toolStripButton_Remove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_Remove.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.toolStripButton_Remove.ForeColor = System.Drawing.Color.DarkRed;
            this.toolStripButton_Remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Remove.Image")));
            this.toolStripButton_Remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Remove.Name = "toolStripButton_Remove";
            this.toolStripButton_Remove.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton_Remove.Text = "Видалити";
            this.toolStripButton_Remove.Click += new System.EventHandler(this.toolStripButton_Remove_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Локальна папка";
            // 
            // general_main_localPath
            // 
            this.general_main_localPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.general_main_localPath.Location = new System.Drawing.Point(9, 58);
            this.general_main_localPath.Name = "general_main_localPath";
            this.general_main_localPath.Size = new System.Drawing.Size(317, 20);
            this.general_main_localPath.TabIndex = 6;
            // 
            // wndSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(364, 284);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "wndSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Параметри";
            this.tabControl2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.general_main_fetchTimeout)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.SaveControlSettings();
            this.Close();
        }

        private void toolStripButton_AddNew_Click(object sender, EventArgs e)
        {
            _addSettingsProfile("profile-" + tabControl2.TabCount, "", false);
        }

        private void toolStripButton_Remove_Click(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Tag.ToString().Equals("profile-default"))
            {
                try
                {
                    if (MessageBox.Show("Видалити профіль синхронізації?", Application.ProductName, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        ((Hashtable)ApplicationSettingsContext.Configuration["datasync"]).Remove(tabControl1.SelectedTab.Tag);
                        tabControl1.SelectedTab.Controls[0].Dispose();
                        tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nTry to remove this profile manually in the datasync.xml config"); }
            }
        }

        private void _addSettingsProfile(string name, string displayName, bool isDefault)
        {
            // create data sync profile
            DataSyncProfile _newSyncProfile = new DataSyncProfile(name, isDefault);
            _newSyncProfile.Dock = DockStyle.Fill;
            if (displayName == null || displayName == "")
                displayName = _newSyncProfile.ProfileDisplayName;
            // update control fields names
            ApplicationSettingsContext.CollectAndSetProfileName(_newSyncProfile, "datasync_" + _newSyncProfile.ProfileName);
            // react on profile name changes
            //_newSyncProfile.OnProfileNameChange += new DataSyncProfile.ProfileNameChanged(_newSyncProfile_OnProfileNameChange);
            // create new tab
            TabPage _newSyncPage = new TabPage(displayName);
            _newSyncPage.Tag = name;
            _newSyncPage.Text = displayName;
            // add data sync control
            _newSyncPage.Controls.Add(_newSyncProfile);
            // add tab into tabcontrol
            tabControl1.TabPages.Add(_newSyncPage);
        }

        //private void _newSyncProfile_OnProfileNameChange(string profileName, DataSyncProfile sender, EventArgs e)
        //{
            //if (tabControl1.SelectedTab.Text.Equals(profileName))
            //    return;

            // tabControl1.SelectedTab.Text = profileName;
            //ApplicationSettingsContext.CollectAndSetProfileName(sender, "datasync_" + sender.PropfileName);
        //}
    }
}
