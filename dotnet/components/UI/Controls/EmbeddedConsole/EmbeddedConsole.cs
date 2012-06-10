using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace components.UI.Controls.EmbeddedConsole
{
    public partial class EmbeddedConsole : UserControl
    {
        private Process proc;
        public bool HideOnFinish { get; set; }
        public bool ShowScroll { get; set; }
        public int TextBuffer { get; set; }

        public EmbeddedConsole()
        {
            InitializeComponent();

            ShowScroll = true;
            TextBuffer = 1300;
        }

        public RichTextBox ConsloeObject { get { return this.richTextBox1; } set { this.richTextBox1 = value; } }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public string Output { get { return this.richTextBox1.Text; } }

        public Process ProcessExec { set { proc = value; } get { return proc; } }

        public void Execute(Process p)
        {
            proc = p;
            Execute();
        }

        public void Execute()
        {
            if (HideOnFinish)
                this.Visible = true;
            using (components.Shared.Objects.TextBoxStreamWriter tb = new components.Shared.Objects.TextBoxStreamWriter(richTextBox1))
            {
                if (ShowScroll)
                    richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
                else
                    richTextBox1.ScrollBars = RichTextBoxScrollBars.None;

                tb.TextBuffer = this.TextBuffer;

                Console.SetOut(tb);
                this.backgroundWorker1.RunWorkerAsync();
                //Console.Clear();
                while (this.backgroundWorker1.IsBusy)
                {
                    Application.DoEvents();
                }
                tb.Close();
            }
            if (HideOnFinish)
                this.Visible = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
                //proc.StartInfo.FileName = Application.StartupPath + @"\tools\source\up.bat";
                //proc.StartInfo.Arguments = @"att trunk";
                //proc.StartInfo.WorkingDirectory = Application.StartupPath;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.ErrorDataReceived += new DataReceivedEventHandler(ProcessErrorHandler);
                proc.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                proc.WaitForExit();
                proc.Close();
        }

        public void ClearOutput()
        {
            this.richTextBox1.Clear();
        }

        private void ProcessErrorHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }
        private void ProcessOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            proc.Kill();
        }


    }
}
