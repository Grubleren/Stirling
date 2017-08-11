using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace JH.Applications
{
    public class CursorHandler : FunctionBlock, IObserver<DataObject>
    {
        double[,] input;
        DisplaySetup setup;

        public CursorHandler()
        {
            setup = new DisplaySetup();
        }

        public void OnNext(DataObject data)
        {

            if (!error)
            {
                try
                {
                    input = data.dataElements[dataSelector].data as double[,];
                    if (input == null)
                        return;

                    UpdateLabel();

                }
                catch
                {
                    error = true;
                    TraverseError();
                }
            }
        }

        public void OnCompleted()
        {
            Console.WriteLine("Done");
        }

        public void OnError(Exception e)
        {
            error = true;
            Console.WriteLine(e.Message);
        }
        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                DisplaySetup s = value as DisplaySetup;

                if (setup != null)
                {
                    s.pictureBox.MouseDown -= new MouseEventHandler(OnMouseDown);
                    s.pictureBox.MouseDown += new MouseEventHandler(OnMouseDown);
                }

                setup.Copy(s);
            }
        }
    
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            UpdateLabel();
        }

        public void UpdateLabel()
        {
            if (setup.pictureBox.graphData != null)
            {
                if (setup.pictureBox.cursor < 0)
                    setup.pictureBox.cursor = 0;
                else if (setup.pictureBox.cursor >= setup.pictureBox.graphData.GetLength(0))
                    setup.pictureBox.cursor = setup.pictureBox.graphData.GetLength(0) -1;
                else
                {
                    switch (setup.pictureBox.axisType)
                    {
                        case DisplayComponent.AxisType.Cpb1:
                            setup.pictureBox.cursorText = DisplayComponent.cpb1Freq[int.Parse(setup.pictureBox.graphData[setup.pictureBox.cursor, 0].ToString("0"))] + "     " + setup.pictureBox.graphData[setup.pictureBox.cursor, 1].ToString("0.00");
                            break;
                        case DisplayComponent.AxisType.Cpb3:
                            setup.pictureBox.cursorText = DisplayComponent.cpb3Freq[int.Parse(setup.pictureBox.graphData[setup.pictureBox.cursor, 0].ToString("0"))] + "     " + setup.pictureBox.graphData[setup.pictureBox.cursor, 1].ToString("0.00");
                            break;
                        case DisplayComponent.AxisType.Lin:
                            setup.pictureBox.cursorText = setup.pictureBox.graphData[setup.pictureBox.cursor, 0].ToString("0") + "     " + setup.pictureBox.graphData[setup.pictureBox.cursor, 1].ToString("0.00");
                            break;
                    }
                }
            }

            setup.pictureBox.DrawGraph();
        }

    }
    public class GraphHandler : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        public bool axisDescriptorDirty;
        public bool axisDirty;
        double[] input;
        double[,] graphData;
        AxisDescriptor axisDescriptor;
        DisplaySetup setup;

        public GraphHandler()
        {
            setup = new DisplaySetup();
            output = new DataObject();
            output.dataElements = new DataObjectElement[1];
            output.dataElements[0] = new DataObjectElement("", 0);
        }

        public override IIterator CreateIterator()
        {
            return new IteratorTime(this);
        }

        public void OnNext(DataObject data)
        {

            if (!error)
            {
                try
                {
                    input = data.dataElements[0].data as double[];
                    if (input == null)
                        return;
                    axisDescriptor = (AxisDescriptor)data.dataElements[0].descriptors[0];

                    UpdateGraph();

                    output.dataElements[0].data = setup.pictureBox.graphData;

                    TraverseSubscribers();

                }
                catch
                {
                    error = true;
                    TraverseError();
                }
            }
        }

        public void OnCompleted()
        {
            Console.WriteLine("Done");
        }

        public void OnError(Exception e)
        {
            error = true;
            Console.WriteLine(e.Message);
        }
        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                DisplaySetup s = value as DisplaySetup;

                if (setup != null)
                {
                }

                setup.Copy(s);
            }
        }

        public void UpdateGraph()
        {
            double min_tic_x;
            double tic_intv_x;
            double tic_ratio_x;
            int nDecades_x;

            if (axisDescriptorDirty)
            {
                axisDescriptorDirty = false;
                setup.pictureBox.FindDisplayRange(axisDescriptor.axisType, axisDescriptor.min, axisDescriptor.max, out min_tic_x, out tic_intv_x, out tic_ratio_x, out nDecades_x);

                setup.pictureBox.min_x = axisDescriptor.min;
                setup.pictureBox.max_x = axisDescriptor.max;
                setup.pictureBox.axisType = axisDescriptor.axisType;

                setup.pictureBox.min_tic_x = min_tic_x;
                setup.pictureBox.tic_intv_x = tic_intv_x;

                axisDirty = true;
            }

            if (axisDirty)
            {
                axisDirty = false;
                graphData = new double[(int)((setup.pictureBox.max_x - setup.pictureBox.min_x) / axisDescriptor.step + 1), 2];
                setup.pictureBox.cursor = graphData.GetLength(0) / 2;
                setup.pictureBox.SizeChanged();
            }

            for (int i = 0; i < graphData.GetLength(0); i++)
            {
                graphData[i, 0] = i * axisDescriptor.step + setup.pictureBox.min_x;
                int index = (int)((setup.pictureBox.min_x - axisDescriptor.min) / axisDescriptor.step + i);
                double v = 0;
                if (index >= 0 && index < input.Length)
                    v = input[index];
                if (v < 1e-15)
                    v = 1e-15;
                graphData[i, 1] = 10 * Math.Log10(v);

            }
            setup.pictureBox.graphData = graphData;
            setup.pictureBox.DrawGraph();
        }

    }

    public class ValueHandler : FunctionBlock, IObserver<DataObject>
    {
        double input;
        DisplaySetup setup;

        public ValueHandler()
        {
            setup = new DisplaySetup();
            output = new DataObject();
        }

        public void OnNext(DataObject data)
        {
            if (!error)
            {
                try
                {
                    input = (double)data.dataElements[0].data;

                    UpdateLabel();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    error = true;
                    TraverseError();
                }
            }
        }

        public void OnCompleted()
        {
            Console.WriteLine("Done");
        }

        public void OnError(Exception e)
        {
            error = true;
            Console.WriteLine(e.Message);
        }

        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                DisplaySetup s = value as DisplaySetup;

                if (s == null ||
                    s.label != setup.label)
                {
                    
                }

                setup.Copy(s);
            }
        }

        public void UpdateLabel()
        {
            setup.label.Invoke(new Action(() => setup.label.Text = (10 * Math.Log10(input)).ToString("0.0")));
        }
    }

    public class GraphAdapter : FunctionBlock, IObservable<DataObject>, IObserver<DataObject>
    {
        DefaultSetup setup;

        public GraphAdapter()
        {
            setup = new DefaultSetup();
            output = new DataObject();
            output.dataElements = new DataObjectElement[2];
            output.dataElements[0] = new DataObjectElement("200", 200);
            output.dataElements[1] = new DataObjectElement("1000", 1000);
        }

        public override IIterator CreateIterator()
        {
            return new IteratorTime(this);
        }

        public void OnNext(DataObject data)
        {
            if (!error)
            {
                try
                {
                    output.dataElements[0].data = data.dataElements[dataSelector].data;

                    output.dataElements[0].descriptors.Clear();
                    for (int i = 0; i < data.dataElements[dataSelector].descriptors.Count; i++)
                        output.dataElements[0].descriptors.Add(data.dataElements[dataSelector].descriptors[i]);

                    TraverseSubscribers();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    error = true;
                    TraverseError();
                }
            }
        }

        public void OnCompleted()
        {
            Console.WriteLine("Done");
            //for (int i = 0; i < output.dataSingle.Length && Thread.CurrentThread !=null; i++)
            //    Console.WriteLine(10 * Math.Log10(output.dataSingle[i]));
        }

        public void OnError(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                DefaultSetup s = value as DefaultSetup;

                if (setup == null)
                {
                
                }

                setup.Copy(s);
            }
        }
    }


    public class DisplaySetup : ISetup
    {
        public DisplayComponent pictureBox;
        public Label label;
        

        public void Copy(DisplaySetup setup)
        {
            pictureBox = setup.pictureBox;
            label = setup.label;
        }

        public object Clone()
        {
            DisplaySetup s = new DisplaySetup();
            s.Copy(this);
            return s;
        }
    }
}

