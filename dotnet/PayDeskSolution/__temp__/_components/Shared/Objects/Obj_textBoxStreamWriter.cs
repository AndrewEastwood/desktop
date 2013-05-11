using System;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace components.Shared.Objects
{
    public class TextBoxStreamWriter : TextWriter
    {
        RichTextBox _output = null;
        public int TextBuffer { get; set; }

        public TextBoxStreamWriter()
        {

        }

        public TextBoxStreamWriter(RichTextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            updateLabelText(value.ToString());

        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        delegate void updateLabelTextDelegate(string newText);

        private void updateLabelText(string newText)
        {
            if (_output.InvokeRequired)
            {
                // this is worker thread
                updateLabelTextDelegate del = new updateLabelTextDelegate(updateLabelText);
                _output.Invoke(del, new object[] { newText });
            }
            else
            {

                if (_output.Text.Length > TextBuffer + 700)
                    _output.Text = _output.Text.Substring(_output.Text.IndexOf("\n", 700));


                // this is UI thread"
                bool scroll = newText.Contains("\n");
                _output.Text += newText.Replace("\n", "");

                //_output.Text = _output.Text.Replace("\n\n", "\r\n");
                
                _output.SelectionStart = _output.Text.Length;
                //_output.ScrollToCaret();
                if (scroll)
                {
                    _output.SelectionStart = _output.Text.Length;
                    _output.ScrollToCaret();
                    //_output.Select(_output.Text.Length, 1);
                    //_output.ScrollToCaret();
                }
            }
        }
    }
}
