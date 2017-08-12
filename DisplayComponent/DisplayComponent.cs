using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;

namespace JH.Applications
{
    public class DisplayComponent : Button
    {
        public enum AxisType
        {
            Lin,
            Log,
            Cpb1,
            Cpb3
        }

        public enum GraphType
        {
            Line,
            Bar,
            Contour
        }

        public static readonly string[] cpb1Freq = new string[] { "1 Hz", "2 Hz", "4 Hz", "8 Hz", "16 Hz", "31.5 Hz", "63 Hz", "125 Hz", "250 Hz", "500 Hz", "1 kHz", "2 kHz", "4 kHz", "8 kHz", "16 kHz", "31.5 kHz", "63 kHz" };
        public static readonly string[] cpb3Freq = new string[] { "1 Hz", "1.25 Hz", "1.6 Hz", "2 Hz", "2.5 Hz", "3.15 Hz", "4 Hz", "5 Hz", "6.3 Hz", "8 Hz", "10 Hz", "12.5 Hz", "16 Hz", "20 Hz", "25 Hz", "31.5 Hz", "40 Hz", "50 Hz", "63 Hz", "80 Hz", "100 Hz", "125 Hz", "160 Hz", "200 Hz", "250 Hz", "315 Hz", "400 Hz", "500 Hz", "630 Hz", "800 Hz", "1 kHz", "1.25 kHz", "1.6 kHz", "2 kHz", "2.5 kHz", "3.15 kHz", "4 kHz", "5 kHz", "6.3 kHz", "8 kHz", "10 kHz", "12.5 kHz", "16 kHz", "20 kHz", "25 kHz", "31.5 kHz", "40 kHz", "50 kHz", "63 kHz", "80 kHz" };

        Bitmap graphArea;
        Bitmap displayArea;
        public Graphics graphGraphics;
        Graphics displayGraphics;
        Label cursorLabel;
        public int frame;
        public double min_x, min_y, max_x, max_y;
        public double min_tic_x, min_tic_y, tic_intv_x, tic_intv_y;
        int[] ticPos_x;
        int[] ticPos_y;
        string[] annotations_x;
        string[] annotations_y;
        public int cursor;
        public string cursorText;
        public int nDecades;
        public double tic_ratio_x, tic_ratio_y;
        public AxisType axisType;
        public GraphType graphType = GraphType.Bar;
        public double[,] graphData;
        double alpha_x;
        double beta_x;
        double alpha_y;
        double beta_y;
        public Color backColor;
        public Color frameColor;
        public Color ticColor;
        public Color cursorColor;
        public Color annotationColor;
        public Color graphColor;
        public Pen framePen;
        public Pen graphPen;
        int contourIndex;
        
        public DisplayComponent()
        {
            backColor = Color.White;
            frameColor = Color.White;
            ticColor = Color.Black;
            cursorColor = Color.Black;
            framePen = new Pen(frameColor);
            annotationColor = Color.Red;
            graphColor = Color.Blue;
            graphPen = new Pen(graphColor);
            BackColor = backColor;
            ResizeRedraw = true;
            cursorLabel = new Label();
            Controls.Add(cursorLabel);
        }
        public static int Frequency2Index1(AxisType axisType, double f)
        {
            if (axisType == AxisType.Cpb1)
                return (int)Math.Round((Math.Log(f) / Math.Log(2)));
            else if (axisType == AxisType.Cpb3)
                return (int)Math.Round((Math.Log(f) / Math.Log(2)) * 3);

            return -1;
        }

        public new void SizeChanged()
        {
            OnSizeChanged(new EventArgs());
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            lock (this)
            {
                int graphWidth = Width - 2 * (frame + 1);
                int graphHeight = Height - 2 * (frame + 1);

                displayArea = new Bitmap(Width, Height);
                graphArea = new Bitmap(graphWidth, graphHeight);

                graphGraphics = Graphics.FromImage(graphArea);
                graphGraphics.Clear(backColor);

                displayGraphics = Graphics.FromImage(displayArea);
                displayGraphics.Clear(backColor);

                cursorLabel.Location = new Point(5, 5);
                cursorLabel.ForeColor = graphColor;
                DrawFrame(displayGraphics, new Point(0, 0), new Size(Width - 1, Height - 1));
                DrawFrame(displayGraphics, new Point(frame, frame), new Size(graphWidth + 1, graphHeight + 1));

                double xt;
                double yt;
                ticPos_x = TicPositions(graphWidth + 1, min_x, max_x, min_tic_x, tic_intv_x, out xt);
                ticPos_y = TicPositions(graphHeight + 1, min_y, max_y, min_tic_y, tic_intv_y, out yt);

                DrawTicks(displayGraphics, frame, Height - frame - 1, 10);

                string decimals = Decimals(tic_intv_x);
                annotations_x = GetAnnotations(axisType, xt, tic_intv_x, ticPos_x, decimals);
                decimals = Decimals(tic_intv_y);
                annotations_y = GetAnnotations(AxisType.Lin, yt, tic_intv_y, ticPos_y, decimals);

                StringFormat sf_x = new StringFormat();
                sf_x.LineAlignment = StringAlignment.Near;
                sf_x.Alignment = StringAlignment.Center;
                StringFormat sf_y = new StringFormat();
                sf_y.LineAlignment = StringAlignment.Center;
                sf_y.Alignment = StringAlignment.Far;
                DrawAnnotations(displayGraphics, frame, Height - frame - 1, 10, sf_x, sf_y);
                DrawGraph();
            }
        }

        void DrawFrame(Graphics graphics, Point ll, Size ur)
        {
            Rectangle rectangle = new Rectangle(ll, ur);
            graphics.FillRectangle(new SolidBrush(frameColor), rectangle);
            graphics.DrawRectangle(new Pen(ticColor), rectangle);
        }

        void DrawTicks(Graphics graphics, int origo_x, int origo_y, int ticLen)
        {
            for (int i = 0; i < ticPos_x.Length; i++)
            {
                graphics.DrawLine(new Pen(ticColor), ticPos_x[i] + origo_x, origo_y + ticLen, ticPos_x[i] + origo_x, origo_y);
            }
            for (int i = 0; i < ticPos_y.Length; i++)
            {
                graphics.DrawLine(new Pen(ticColor), origo_x - ticLen, origo_y - ticPos_y[i], origo_x, origo_y - ticPos_y[i]);
            }
        }

        string[] GetAnnotations(AxisType axisType, double t, double tic_intv, int[] ticPos, string decimals)
        {
            string[] annotations = new string[ticPos.Length];

            if (axisType == AxisType.Lin)
            {
                for (int i = 0; i < ticPos.Length; i++)
                {
                    annotations[i] = (t + i * tic_intv).ToString(decimals, CultureInfo.InvariantCulture);
                }
            }
            else if (axisType == AxisType.Cpb1)
            {
                for (int i = 0; i < ticPos.Length; i++)
                {
                    annotations[i] = cpb1Freq[(int)(t + i * tic_intv)];
                }
            }
            else if (axisType == AxisType.Cpb3)
            {
                for (int i = 0; i < ticPos.Length; i++)
                {
                    annotations[i] = cpb3Freq[(int)(t + i * tic_intv)];
                }
            }

            return annotations;

        }

        void DrawAnnotations(Graphics graphics, int origo_x, int origo_y, int ticLen, StringFormat sf_x, StringFormat sf_y)
        {
            for (int i = 0; i < ticPos_x.Length; i++)
            {
                graphics.DrawString(annotations_x[i], new Font("Arial", 10, FontStyle.Italic), new SolidBrush(annotationColor), new PointF(ticPos_x[i] + origo_x, origo_y + ticLen), sf_x);
            }
            for (int i = 0; i < ticPos_y.Length; i++)
            {
                graphics.DrawString(annotations_y[i], new Font("Arial", 10, FontStyle.Italic), new SolidBrush(annotationColor), new PointF(origo_x - ticLen, origo_y - ticPos_y[i]), sf_y);
            }


        }

        int[] TicPositions(float size, double min, double max, double min_tic, double tic_intv, out double xt)
        {
            List<double> list = new List<double>();
            double x = min_tic;
            xt = min_tic - tic_intv;
            while ((max - x) / max >= -1e-6)
            {
                if (x >= min)
                {
                    list.Add(x - min);
                }
                else
                    xt = x;
                x += tic_intv;
            }
            xt += tic_intv;
            int[] ticPos = new int[list.Count];
            double ratio = size / (max - min);
            for (int i = 0; i < list.Count; i++)
            {
                ticPos[i] = (int)Math.Round(list[i] * ratio);
            }
            return ticPos;
        }

        string Decimals(double value)
        {
            double frac = value - (int)value;
            string count = "";
            while (frac != 0)
            {
                frac = 10 * frac - (int)(10 * frac);
                count += "0";
            }
            return "0." + count;
        }

        public void FindDisplayRange(AxisType axisType, double min, double max, out double min_tic, out double tic_intv, out double tic_ratio, out int nDecades)
        {
            min_tic = -1;
            tic_intv = 1;
            tic_ratio = 1;
            nDecades = 3;

            switch (axisType)
            {
                case AxisType.Lin:
                    double d = (max - min) / 4;
                    double dlog = Math.Log10(d);
                    int dlogint = (int)Math.Floor(dlog);
                    double dfrac = dlog - dlogint;
                    if (dlog > -6 && dlog < 6)
                    {
                        if (dfrac < 0.5 * Math.Log10(2))
                            tic_intv = Math.Pow(10, dlogint);
                        else if (dfrac < 0.5 * Math.Log10(10))
                            tic_intv = 2 * Math.Pow(10, dlogint);
                        else
                            tic_intv = 5 * Math.Pow(10, dlogint);
                    }
                    min_tic = Math.Floor(min / tic_intv) * tic_intv;
                    break;
                case AxisType.Cpb1:
                    min_tic = min;
                    tic_intv = 1;
                    break;
                case AxisType.Cpb3:
                    min_tic = (int)(min / 3) * 3;
                    tic_intv = 3;
                    break;

            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            cursor = (int)Math.Round((e.X - frame) / alpha_x);
            base.OnMouseDown(e);
        }

        public void DrawCursor(Graphics graphics)
        {
            if (graphType == GraphType.Contour)
                return;
            float x1 = GraphScaling_X(cursor);
            graphics.DrawLine(new Pen(cursorColor), x1, 0, x1, graphics.VisibleClipBounds.Height - 1);
        }

        void DrawGraph(Graphics graphics)
        {
            if (graphData == null || graphType == GraphType.Contour)
                return;

            alpha_x = (graphics.VisibleClipBounds.Width - 1) / (graphData.GetLength(0) - 1);
            beta_x = 0;
            alpha_y = -(graphics.VisibleClipBounds.Height - 1) / (max_y - min_y);
            beta_y = -alpha_y * max_y;
            float x1, x2, y1, y2;

            if (graphType == GraphType.Line)
            {
                for (int i = 0; i < graphData.GetLength(0) - 1; i++)
                {
                    x1 = GraphScaling_X(i);
                    x2 = x1 + 1;
                    y1 = GraphScaling_Y(graphData[i, 1]);
                    y2 = GraphScaling_Y(0);

                    graphics.DrawRectangle(graphPen, x1, y1, x2 - x1, y2 - y1);
                }
            }
            else
            {
                x1 = GraphScaling_X(0);
                x2 = GraphScaling_X(0.5);
                y1 = GraphScaling_Y(graphData[0, 1]);
                y2 = y1;
                graphics.DrawLine(graphPen, x1, y1, x2, y2);
                x1 = x2;
                y2 = GraphScaling_Y(graphData[1, 1]);
                graphics.DrawLine(graphPen, x1, y1, x2, y2);

                for (int i = 1; i < graphData.GetLength(0) - 1; i++)
                {
                    x1 = GraphScaling_X(i - 0.5);
                    x2 = GraphScaling_X(i + 0.5);
                    y1 = GraphScaling_Y(graphData[i, 1]);
                    y2 = y1;
                    graphics.DrawLine(graphPen, x1, y1, x2, y2);
                    x1 = x2;
                    y2 = GraphScaling_Y(graphData[i + 1, 1]);
                    graphics.DrawLine(graphPen, x1, y1, x2, y2);
                }

                int last = graphData.GetLength(0) - 1;

                x2 = GraphScaling_X(last + 0.5);
                x2 = GraphScaling_X(last);
                y1 = GraphScaling_Y(graphData[last, 1]);
                y2 = y1;
                graphics.DrawLine(graphPen, x1, y1, x2, y2);
            }
        }

        public void ClearContour()
        {
            graphGraphics.Clear(backColor);
            contourIndex = 0;
        }
        public void DrawContour(double[] graph)
        {
            for (int i = 0; i < graphArea.Width; i++)
            {
                byte r,g,b;
                byte c = (byte)(graph[i]*255);
                r = c;
                g = c;
                b = c;
                int x = i;
                graphArea.SetPixel(x, graphArea.Height-1-contourIndex, Color.FromArgb(r,g,b));
            }
            contourIndex++;
            displayGraphics.DrawImage(graphArea, frame + 1, frame + 1);
            Invalidate();
        }

        public void DrawGraphNoClear()
        {
            lock (this)
            {
                DrawGraph(graphGraphics);
                DrawCursor(graphGraphics);
                displayGraphics.DrawImage(graphArea, frame + 1, frame + 1);
                Invalidate();
            }
        }

        public void DrawGraph()
        {
            lock (this)
            {
                graphGraphics.Clear(backColor);
                DrawGraph(graphGraphics);
                DrawCursor(graphGraphics);
                displayGraphics.DrawImage(graphArea, frame + 1, frame + 1);
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(displayArea, 0, 0);
            cursorLabel.Text = cursorText;

        }

        float GraphScaling_X(double data)
        {
            return (float)(beta_x + alpha_x * data);
        }

        float GraphScaling_Y(double data)
        {
            return (float)(beta_y + alpha_y * data);
        }

        protected override void WndProc(ref Message m)
        {
            bool handled = false;
            int RESIZE_HANDLE_SIZE = 10;
            int WM_NCHITTEST = 0x84;
            if (m.Msg == WM_NCHITTEST)
            {
                Size formSize = this.Size;
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point clientPoint = this.PointToClient(screenPoint);
                Rectangle hitBox = new Rectangle(formSize.Width - RESIZE_HANDLE_SIZE, formSize.Height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE);
                if (hitBox.Contains(clientPoint))
                {
                    m.Result = (IntPtr)17;
                    handled = true;
                }
            }

            if (!handled)
                base.WndProc(ref m);
        }

    }
}