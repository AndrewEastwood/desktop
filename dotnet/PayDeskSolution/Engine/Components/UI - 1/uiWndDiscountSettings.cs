﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using driver.Config;
//0using mdcore;
//0using mdcore.Config;

namespace PayDesk.Components.UI
{
    public partial class uiWndDiscountSettings : Form
    {
        public uiWndDiscountSettings()
        {
            InitializeComponent();

            //staic discount
            checkBox1.Checked = ConfigManager.Instance.CommonConfiguration.APP_UseStaticDiscount;
            maskedTextBox1.Text = ConfigManager.Instance.CommonConfiguration.APP_StaticDiscountValue.ToString();
            //comboBox1.SelectedIndex = (byte)AppConfig.APP_StaticDiscountType;
            //type
            checkBox3.Checked = ConfigManager.Instance.CommonConfiguration.APP_UsePercentTypeDisc;
            checkBox4.Checked = ConfigManager.Instance.CommonConfiguration.APP_UseAbsoluteTypeDisc;
            comboBox1.SelectedIndex = ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc;
            //variant
            radioButton4.Checked = ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount;
            radioButton3.Checked = !ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount;
            //rules
            checkBox2.Checked = ConfigManager.Instance.CommonConfiguration.APP_UseStaticRules;
            if (ConfigManager.Instance.CommonConfiguration.APP_DiscountRules != null)
                listBox1.Items.AddRange(ConfigManager.Instance.CommonConfiguration.APP_DiscountRules);
        }

        //static discount
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                checkBox2.Checked = false;
        }
        //static rules
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                checkBox1.Checked = false;
        }
        //context menu for ListBox1
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Tag.ToString())
            {
                case "add":
                    {
                        uiWndDiscountRule bl = new uiWndDiscountRule("");
                        bl.ShowDialog();
                        if (bl.NewRule == null)
                            return;
                        if (bl.NewRule != "")
                            listBox1.Items.Add(bl.NewRule);
                        bl.Dispose();
                        видалитиToolStripMenuItem.Enabled = listBox1.Items.Count != 0;
                        редагуватиToolStripMenuItem.Enabled = listBox1.Items.Count != 0;
                        break;
                    }
                case "edit":
                    try
                    {

                        uiWndDiscountRule bl = new uiWndDiscountRule(listBox1.SelectedItem.ToString());
                        bl.ShowDialog();
                        if (bl.NewRule == null)
                            return;
                        if (bl.NewRule != "")
                            listBox1.Items[listBox1.SelectedIndex] = bl.NewRule;
                        bl.Dispose();
                    }
                    catch { }
                    break;
                case "delete":
                    try
                    {
                        listBox1.Items.Remove(listBox1.SelectedItem);
                    }
                    catch { }
                    видалитиToolStripMenuItem.Enabled = listBox1.Items.Count != 0;
                    редагуватиToolStripMenuItem.Enabled = listBox1.Items.Count != 0;
                    break;
            }
        }
        //save button
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //static discount
                ConfigManager.Instance.CommonConfiguration.APP_UseStaticDiscount = checkBox1.Checked;
                ConfigManager.Instance.CommonConfiguration.APP_StaticDiscountValue = double.Parse(maskedTextBox1.Text);
                //type
                ConfigManager.Instance.CommonConfiguration.APP_UsePercentTypeDisc = checkBox3.Checked;
                ConfigManager.Instance.CommonConfiguration.APP_UseAbsoluteTypeDisc = checkBox4.Checked;
                ConfigManager.Instance.CommonConfiguration.APP_DefaultTypeDisc = comboBox1.SelectedIndex;
                //variant
                ConfigManager.Instance.CommonConfiguration.APP_OnlyDiscount = radioButton4.Checked;
                ConfigManager.Instance.CommonConfiguration.APP_UseStaticRules = checkBox2.Checked;
                //rules
                using (StreamWriter sw = File.CreateText(ConfigManager.Instance.CommonConfiguration.Path_Rules))
                {
                    ConfigManager.Instance.CommonConfiguration.APP_DiscountRules = new string[listBox1.Items.Count];
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        sw.WriteLine(listBox1.Items[i].ToString());
                        ConfigManager.Instance.CommonConfiguration.APP_DiscountRules[i] = listBox1.Items[i].ToString();
                    }

                    sw.Close();
                    sw.Dispose();
                }
            }
            catch { return; }

            DialogResult = DialogResult.OK;
            Close();
        }
        //Keydown event
        private void BillRulesList_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventArgs esc = new KeyEventArgs(Keys.Escape);

            if (e.KeyValue == esc.KeyValue)
            {
                Close();
                return;
            }
        }

        private void checkBox3_4_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = comboBox1.Enabled = (checkBox3.Checked && checkBox4.Checked);
        }

    }
}