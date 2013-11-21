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
        private Panel panel1;
        private Label label1;
        private NumericUpDown general_fetchTimeout;
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
                    _addSettingsProfile(de.Key.ToString(), de.Key.ToString().ToLower().Equals("profile-default"));
                    _hasProfiles = true;
                }

            // add default profile
            if (!_hasProfiles)
                _addSettingsProfile("profile-default", true);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wndSettings));
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_AddNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1 = new System.Windows.Forms.Panel();
            this.general_fetchTimeout = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.general_fetchTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(278, 413);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Location = new System.Drawing.Point(0, 28);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(384, 317);
            this.tabControl2.TabIndex = 10;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_AddNew,
            this.toolStripSeparator1,
            this.toolStripButton_Remove,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(384, 25);
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
            this.toolStripButton_AddNew.Size = new System.Drawing.Size(54, 22);
            this.toolStripButton_AddNew.Text = "Add New";
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
            this.toolStripButton_Remove.Size = new System.Drawing.Size(88, 22);
            this.toolStripButton_Remove.Text = "Remove current";
            this.toolStripButton_Remove.Click += new System.EventHandler(this.toolStripButton_Remove_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.general_fetchTimeout);
            this.panel1.Location = new System.Drawing.Point(0, 351);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(384, 56);
            this.panel1.TabIndex = 12;
            // 
            // general_fetchTimeout
            // 
            this.general_fetchTimeout.Location = new System.Drawing.Point(89, 3);
            this.general_fetchTimeout.Maximum = new decimal(new int[] {
            120000,
            0,
            0,
            0});
            this.general_fetchTimeout.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.general_fetchTimeout.Name = "general_fetchTimeout";
            this.general_fetchTimeout.Size = new System.Drawing.Size(120, 20);
            this.general_fetchTimeout.TabIndex = 0;
            this.general_fetchTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Fetch timeout";
            // 
            // wndSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(384, 448);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "wndSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.general_fetchTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.SaveControlSettings();
            this.Close();
        }

        private void toolStripButton_AddNew_Click(object sender, EventArgs e)
        {
            _addSettingsProfile("profile-" + tabControl2.TabCount, false);
        }

        private void toolStripButton_Remove_Click(object sender, EventArgs e)
        {
            if (!tabControl2.SelectedTab.Tag.ToString().Equals("profile-default"))
            {
                try
                {
                    ((Hashtable)ApplicationSettingsContext.Configuration["datasync"]).Remove(tabControl2.SelectedTab.Tag);
                    tabControl2.SelectedTab.Controls[0].Dispose();
                    tabControl2.TabPages.Remove(tabControl2.SelectedTab);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + "\r\nTry to remove this profile manually in the datasync.xml config"); }
            }
        }

        private void _addSettingsProfile(string name, bool isDefault)
        {
            // create data sync profile
            DataSyncProfile _newSyncProfile = new DataSyncProfile(name, isDefault);
            _newSyncProfile.Dock = DockStyle.Fill;
            // update control fields names
            ApplicationSettingsContext.CollectAndSetProfileName(_newSyncProfile, "datasync_" + _newSyncProfile.PropfileName);
            // react on profile name changes
            _newSyncProfile.OnProfileNameChange += new DataSyncProfile.ProfileNameChanged(_newSyncProfile_OnProfileNameChange);
            // create new tab
            TabPage _newSyncPage = new TabPage(_newSyncProfile.PropfileName);
            _newSyncPage.Tag = name;
            _newSyncPage.Text = name;
            // add data sync control
            _newSyncPage.Controls.Add(_newSyncProfile);
            // add tab into tabcontrol
            tabControl2.TabPages.Add(_newSyncPage);
        }

        private void _newSyncProfile_OnProfileNameChange(string profileName, DataSyncProfile sender, EventArgs e)
        {
            if (tabControl2.SelectedTab.Text.Equals(profileName))
                return;

            tabControl2.SelectedTab.Text = profileName;
            ApplicationSettingsContext.CollectAndSetProfileName(sender, "datasync_" + sender.PropfileName);
        }
    }
}
