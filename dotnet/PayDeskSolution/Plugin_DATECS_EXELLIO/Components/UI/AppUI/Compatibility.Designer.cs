namespace DATECS_EXELLIO.UI.AppUI
{
    partial class Compatibility
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.msg_comm_attemptsToRead = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.msg_comm_timeoutOnFail = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.msg_comm_attemptsToWait = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.msg_comm_timeoutOnBusy = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_attemptsToRead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_timeoutOnFail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_attemptsToWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_timeoutOnBusy)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(20, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(218, 224);
            this.panel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.msg_comm_attemptsToRead);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.msg_comm_timeoutOnFail);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.msg_comm_attemptsToWait);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.msg_comm_timeoutOnBusy);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(218, 224);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label1, true);
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "К-сть спроб читання";
            // 
            // msg_comm_attemptsToRead
            // 
            this.msg_comm_attemptsToRead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.msg_comm_attemptsToRead, true);
            this.msg_comm_attemptsToRead.Location = new System.Drawing.Point(3, 16);
            this.msg_comm_attemptsToRead.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.msg_comm_attemptsToRead.Name = "msg_comm_attemptsToRead";
            this.msg_comm_attemptsToRead.Size = new System.Drawing.Size(215, 20);
            this.msg_comm_attemptsToRead.TabIndex = 1;
            this.msg_comm_attemptsToRead.ValueChanged += new System.EventHandler(this.msg_comm_ValueChanged);
            this.msg_comm_attemptsToRead.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.msg_comm_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label2, true);
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Таймаут після помилки";
            // 
            // msg_comm_timeoutOnFail
            // 
            this.msg_comm_timeoutOnFail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.msg_comm_timeoutOnFail, true);
            this.msg_comm_timeoutOnFail.Location = new System.Drawing.Point(3, 55);
            this.msg_comm_timeoutOnFail.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.msg_comm_timeoutOnFail.Name = "msg_comm_timeoutOnFail";
            this.msg_comm_timeoutOnFail.Size = new System.Drawing.Size(215, 20);
            this.msg_comm_timeoutOnFail.TabIndex = 3;
            this.msg_comm_timeoutOnFail.ValueChanged += new System.EventHandler(this.msg_comm_ValueChanged);
            this.msg_comm_timeoutOnFail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.msg_comm_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label3, true);
            this.label3.Location = new System.Drawing.Point(3, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(124, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "К-сть спроб очікування";
            // 
            // msg_comm_attemptsToWait
            // 
            this.msg_comm_attemptsToWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.msg_comm_attemptsToWait, true);
            this.msg_comm_attemptsToWait.Location = new System.Drawing.Point(3, 94);
            this.msg_comm_attemptsToWait.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.msg_comm_attemptsToWait.Name = "msg_comm_attemptsToWait";
            this.msg_comm_attemptsToWait.Size = new System.Drawing.Size(215, 20);
            this.msg_comm_attemptsToWait.TabIndex = 5;
            this.msg_comm_attemptsToWait.ValueChanged += new System.EventHandler(this.msg_comm_ValueChanged);
            this.msg_comm_attemptsToWait.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.msg_comm_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.label4, true);
            this.label4.Location = new System.Drawing.Point(3, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Таймаут очікування";
            // 
            // msg_comm_timeoutOnBusy
            // 
            this.msg_comm_timeoutOnBusy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.SetFlowBreak(this.msg_comm_timeoutOnBusy, true);
            this.msg_comm_timeoutOnBusy.Location = new System.Drawing.Point(3, 133);
            this.msg_comm_timeoutOnBusy.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.msg_comm_timeoutOnBusy.Name = "msg_comm_timeoutOnBusy";
            this.msg_comm_timeoutOnBusy.Size = new System.Drawing.Size(215, 20);
            this.msg_comm_timeoutOnBusy.TabIndex = 7;
            this.msg_comm_timeoutOnBusy.ValueChanged += new System.EventHandler(this.msg_comm_ValueChanged);
            this.msg_comm_timeoutOnBusy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.msg_comm_KeyPress);
            // 
            // Compatibility
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Name = "Compatibility";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.Size = new System.Drawing.Size(258, 264);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_attemptsToRead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_timeoutOnFail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_attemptsToWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.msg_comm_timeoutOnBusy)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown msg_comm_attemptsToRead;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown msg_comm_timeoutOnFail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown msg_comm_attemptsToWait;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown msg_comm_timeoutOnBusy;
    }
}
