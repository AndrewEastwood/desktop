using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace components.UI.Controls.Spinner
{
    public partial class Spinner : Control
    {
        Timer frameTimer;
        bool switchBack = false;
        int segmentsSwitchedBack = 0;
        int segmentsGreen = 0;

        Brush GreenBrush { get; set; }
        Brush GreenShadowBrush { get; set; }
        Brush LightGreenBrush { get; set; }
        Brush LightGreenShadowBrush { get; set; }
        Brush GreyBrush { get; set; }
        Brush GreyShadowBrush { get; set; }

        public Spinner()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            Segments = 12;
            this.frameTimer = new Timer();
            frameTimer.Interval = 100;
            frameTimer.Tick += new EventHandler(frameTimer_Tick);

             GreenBrush = Brushes.Green;
             GreenShadowBrush = Brushes.DarkGreen;
             LightGreenBrush = Brushes.LimeGreen;
             LightGreenShadowBrush = Brushes.ForestGreen;
             GreyBrush = Brushes.Gainsboro;
             GreyShadowBrush = Brushes.LightGray;
        }

        public Spinner(bool start)
            : this()
        {
            Start();
        }

        public void Start()
        {
            frameTimer.Start();
        }

        public void Stop()
        {
            frameTimer.Stop();
        }

        int segments;

        public int Segments
        {
            get
            {
                return this.segments;
            }
            set
            {
                this.segments = value;
                Invalidate();
            }
        }
        
        void frameTimer_Tick(object sender, EventArgs e)
        {
            if (switchBack)
                segmentsSwitchedBack++;
            else
                segmentsGreen++;

            if (segmentsGreen >= Segments)
                switchBack = true;

            if ((segmentsGreen >= Segments && segmentsSwitchedBack >= segmentsGreen) || (switchBack && segmentsSwitchedBack > Segments))
            {
                switchBack = false;
                segmentsGreen = 0;
                segmentsSwitchedBack = 0;
            }

            frameTimer.Interval = 1200 / Segments;

            Invalidate();
        }
        
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                /*
                Brush greenBrush = Brushes.Green;
                Brush greenShadowBrush = Brushes.DarkGreen;
                Brush lightGreenBrush = Brushes.LimeGreen;
                Brush lightGreenShadowBrush = Brushes.ForestGreen;
                Brush greyBrush = Brushes.Gainsboro;
                Brush greyShadowBrush = Brushes.LightGray;
                */
                Brush backBrush = new SolidBrush(this.BackColor);

                Graphics g = pe.Graphics;
                int size = ClientRectangle.Width > ClientRectangle.Height ? ClientRectangle.Height - 10 : ClientRectangle.Width - 10;
                Rectangle progressArea = new Rectangle((ClientRectangle.Width - size) / 2, (ClientRectangle.Height - size) / 2, size, size);
                g.ResetClip();

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(this.BackColor);

                Pen backPen = new Pen(backBrush, size / 30);

                float segs = Segments < 3 ? 3 : Segments > 90 ? 90 : Segments;
                float angle = 360 / segs;

                int innerCircleWidth = (int)(progressArea.Width * 0.65);
                int shadowCircleWidth = (int)(progressArea.Width * 0.68);
                int innerCircleOffset = ((progressArea.Width - innerCircleWidth) / 2);
                int shadowCircleOffset = ((progressArea.Width - shadowCircleWidth) / 2);

                Rectangle innerCircle = new Rectangle(progressArea.Left + innerCircleOffset,
                                                       progressArea.Top + innerCircleOffset,
                                                       innerCircleWidth, innerCircleWidth);

                Rectangle shadowCircle = new Rectangle(progressArea.Left + shadowCircleOffset,
                                                       progressArea.Top + shadowCircleOffset,
                                                       shadowCircleWidth, shadowCircleWidth);

                for (int i = 0; i < segs; i++)
                {
                    float startAngle = (angle * i) + 270;
                    Brush b;
                    Brush shadow;

                    if (i <= segmentsGreen && i >= segmentsSwitchedBack)
                        if (((i == segmentsGreen) && !switchBack) || ((i == segmentsSwitchedBack) && switchBack))
                        {
                            b = LightGreenBrush;
                            shadow = LightGreenShadowBrush;
                        }
                        else
                        {
                            b = GreenBrush;
                            shadow = GreenShadowBrush;
                        }
                    else
                    {
                        b = GreyBrush;
                        shadow = GreyShadowBrush;
                    }

                    g.FillPie(b, progressArea, startAngle, angle);
                    g.FillPie(shadow, shadowCircle, startAngle, angle);
                    g.DrawPie(backPen, progressArea, startAngle, angle);
                }

                g.FillEllipse(backBrush, innerCircle);
                backPen.Dispose();
                backBrush.Dispose();
            }
            catch (Exception) { }
        }
    }
}
