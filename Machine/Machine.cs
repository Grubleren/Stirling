using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace JH.Applications
{
    public partial class Machine : Form
    {
        public class BindingSourceObject
        {
            FunctionBlock functionBlock;

            public BindingSourceObject(FunctionBlock functionBlock)
            {
                this.functionBlock = functionBlock;
            }

            public AnalysisType Subscriber
            {
                get { return functionBlock.analysisType; }
            }
            public AnalysisType Publisher
            {
                get { return functionBlock.Publisher; }
                set { functionBlock.Publisher = value; }
            }
            public string DataType
            {
                get { return functionBlock.DataType; }
                set { functionBlock.DataType = value; }
            }
        }

        BindingSource bindingSource = new BindingSource();

        FunctionBlocks functionBlocks;

        public SoundCardSetup soundCardSetup;
        public SoundCardSetup playBackSetup;
        GeneratorSetup generatorSetup;
        AfilterSetup aFilterSetup;
        UpsamplingSetup upsamplingSetup;
        FftAdapterSetup fftAdaptorSetup;
        FftSetup fftSetup;
        CpbSetup cpbSetup1;
        CpbSetup cpbSetup3;
        DetectorBankSetup detectorsSetup1;
        DetectorBankSetup detectorsSetup3;
        BBDetectorSetup bbDetectorSetup;
        DefaultSetup graphAdapterSetup;
        DefaultSetup valueAdapterSetup;
        DisplaySetup graphSetup;
        DisplaySetup valueSetup;

        DisplayComponent displayComponent;

        int M;

        System.Windows.Forms.OpenFileDialog fileDialog;

        void SoundCardCallBack(bool value)
        {
            checkBox1.Checked = value;
        }

        public Machine()
        {
            SoundCardDelegate soundCardCallback =  SoundCardCallBack;

            InitializeComponent();
            textBox1.KeyDown += new KeyEventHandler(textBox1_Enter);
            textBox2.KeyDown += new KeyEventHandler(textBox2_Enter);
            textBox3.KeyDown += new KeyEventHandler(textBox3_Enter);
            textBox4.KeyDown += new KeyEventHandler(textBox4_Enter);
            textBox5.KeyDown += new KeyEventHandler(textBox5_Enter);
            textBox6.KeyDown += new KeyEventHandler(textBox6_Enter);
            textBox7.KeyDown += new KeyEventHandler(textBox7_Enter);
            textBox8.KeyDown += new KeyEventHandler(textBox8_Enter);
            displayComponent = new DisplayComponent();
            displayComponent.PreviewKeyDown += new PreviewKeyDownEventHandler(display_previewKey);
            displayComponent.KeyDown += new KeyEventHandler(display_keyDown);
            Controls.Add(displayComponent);
            KeyPreview = true;

            InitDisplay();

            fileDialog = new OpenFileDialog();
            functionBlocks = new FunctionBlocks();

            functionBlocks.Add(new FunctionBlockHandler(), AnalysisType.FBHandler);
            functionBlocks.Add(new SoundCard(soundCardCallback), AnalysisType.Soundcard);
            functionBlocks.Add(new PlayBack(), AnalysisType.Playback);
            functionBlocks.Add(new Generator(), AnalysisType.Generator);
            functionBlocks.Add(new Afilter(), AnalysisType.AWeighting);
            functionBlocks.Add(new Upsampling(), AnalysisType.Upsampling);
            functionBlocks.Add(new FftAdapter(), AnalysisType.FFTAdapter);
            functionBlocks.Add(new FftAnalysis(), AnalysisType.FFTAnalysis);
            functionBlocks.Add(new CpbAnalysis(), AnalysisType.OctaveAnalysis);
            functionBlocks.Add(new DetectorBank(), AnalysisType.OctaveDetectorBank);
            functionBlocks.Add(new CpbAnalysis(), AnalysisType.ThirdOctaveAnalysis);
            functionBlocks.Add(new DetectorBank(), AnalysisType.ThirdOctaveDetectorBank);
            functionBlocks.Add(new BBDetector(), AnalysisType.BroadbandDetector);
            functionBlocks.Add(new GraphAdapter(), AnalysisType.GraphAdapter);
            functionBlocks.Add(new GraphHandler(), AnalysisType.Graph);
            functionBlocks.Add(new GraphAdapter(), AnalysisType.ValueAdapter);
            functionBlocks.Add(new ValueHandler(), AnalysisType.Value);
            functionBlocks.Add(new CursorHandler(), AnalysisType.ValueCursor);


            soundCardSetup = new SoundCardSetup();
            playBackSetup = new SoundCardSetup();
            generatorSetup = new GeneratorSetup();
            fftAdaptorSetup = new FftAdapterSetup();
            upsamplingSetup = new UpsamplingSetup();
            fftSetup = new FftSetup();
            aFilterSetup = new AfilterSetup();
            cpbSetup1 = new CpbSetup();
            cpbSetup3 = new CpbSetup();
            detectorsSetup1 = new DetectorBankSetup();
            detectorsSetup3 = new DetectorBankSetup();
            bbDetectorSetup = new BBDetectorSetup();
            graphAdapterSetup = new DefaultSetup();
            valueAdapterSetup = new DefaultSetup();
            graphSetup = new DisplaySetup();
            valueSetup = new DisplaySetup();

            UpdateSettings(dataGridView);

            Setup();

            Reset();
        }

        void Reset()
        {
            foreach (DictionaryEntry entry in functionBlocks)
                ((FunctionBlock)entry.Value).Reset();
        }

        private void InitDisplay()
        {

            displayComponent.axisType = DisplayComponent.AxisType.Lin;
            double min_x = 0;
            double max_x = 20000;
            double min_y = 0;
            double max_y = 80;
            double min_tic_x, min_tic_y, tic_intv_x, tic_intv_y;
            double tic_ratio_x, tic_ratio_y;
            int nDecades;

            displayComponent.FindDisplayRange(displayComponent.axisType, min_x, max_x, out min_tic_x, out tic_intv_x, out tic_ratio_x, out nDecades);
            displayComponent.FindDisplayRange(displayComponent.axisType, min_y, max_y, out min_tic_y, out tic_intv_y, out tic_ratio_y, out nDecades);

            displayComponent.min_x = min_x;
            displayComponent.max_x = max_x;
            displayComponent.min_y = min_y;
            displayComponent.max_y = max_y;
            displayComponent.min_tic_x = min_tic_x;
            displayComponent.tic_intv_x = tic_intv_x;
            displayComponent.min_tic_y = min_tic_y;
            displayComponent.tic_intv_y = tic_intv_y;
            displayComponent.graphType = DisplayComponent.GraphType.Bar;

            textBox4.Text = min_x.ToString();
            textBox5.Text = max_x.ToString();
            textBox6.Text = min_y.ToString();
            textBox7.Text = max_y.ToString();

            displayComponent.frame = 40;
            displayComponent.Location = new Point(34, 190);
            displayComponent.Size = new Size(500, 330);

        }

        void Setup()
        {
            int samplingFrequency = 48000;
            M = int.Parse(textBox1.Text);
            int soundBufferLength = 48*100;

            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).Init(button3, button4, button10);

            soundCardSetup.length = soundBufferLength;
            soundCardSetup.samplingFrequency = samplingFrequency;
            soundCardSetup.sesitivity = 6.8;
            functionBlocks[AnalysisType.Soundcard].Settings = soundCardSetup;

            playBackSetup.length = soundBufferLength;
            playBackSetup.path = Environment.CurrentDirectory + @"\Coriolan.wav";
            playBackSetup.delay = 20;
            playBackSetup.sesitivity = 1;
            functionBlocks[AnalysisType.Playback].Settings = playBackSetup;

            generatorSetup.samplingFrequency = samplingFrequency;
            generatorSetup.frequency = 101;
            generatorSetup.level = 1000 * Math.Sqrt(2);
            generatorSetup.generatorType = GeneratorType.WhiteNoise;
            generatorSetup.length = soundBufferLength;
            generatorSetup.delay = 1;
            functionBlocks[AnalysisType.Generator].Settings = generatorSetup;

            aFilterSetup.length = soundCardSetup.length;
            functionBlocks[AnalysisType.AWeighting].Settings = aFilterSetup;

            upsamplingSetup.length = soundBufferLength;
            functionBlocks[AnalysisType.Upsampling].Settings = upsamplingSetup;

            fftAdaptorSetup.length = (1 << M);
            fftAdaptorSetup.overlap = 2;
            functionBlocks[AnalysisType.FFTAdapter].Settings = fftAdaptorSetup;

            fftSetup.M = M;
            fftSetup.averagingType = FftAnalysis.AveragingType.Exponential;
            fftSetup.numberOfAverages = int.Parse(textBox2.Text);
            fftSetup.type = 0;
            functionBlocks[AnalysisType.FFTAnalysis].Settings = fftSetup;

            cpbSetup1.length = soundBufferLength;
            cpbSetup1.samplingFrequency = samplingFrequency;
            cpbSetup1.lowFrequency = 31;
            cpbSetup1.highFrequency = 8000;
            cpbSetup1.filterType = OctaveFilterType.Octave;
            functionBlocks[AnalysisType.OctaveAnalysis].Settings = cpbSetup1;

            cpbSetup3.length = soundBufferLength;
            cpbSetup3.samplingFrequency = samplingFrequency;
            cpbSetup3.lowFrequency = 25;
            cpbSetup3.highFrequency = 10000;
            cpbSetup3.filterType = OctaveFilterType.ThirdOctave;
            functionBlocks[AnalysisType.ThirdOctaveAnalysis].Settings = cpbSetup3;

            detectorsSetup1.N = 2000;
            detectorsSetup1.samplingFrequency = samplingFrequency;
            detectorsSetup1.nDetectors = 9;
            functionBlocks[AnalysisType.OctaveDetectorBank].Settings = detectorsSetup1;

            detectorsSetup3.N = 250;
            detectorsSetup3.samplingFrequency = samplingFrequency;
            detectorsSetup3.nDetectors = 27;
            functionBlocks[AnalysisType.ThirdOctaveDetectorBank].Settings = detectorsSetup3;

            bbDetectorSetup.N = 2000;
            bbDetectorSetup.samplingFrequency = samplingFrequency;
            functionBlocks[AnalysisType.BroadbandDetector].Settings = bbDetectorSetup;

            functionBlocks[AnalysisType.GraphAdapter].Settings = graphAdapterSetup;

            functionBlocks[AnalysisType.ValueAdapter].Settings = valueAdapterSetup;

            graphSetup.pictureBox = displayComponent;
            functionBlocks[AnalysisType.Graph].Settings = graphSetup;
            valueSetup.label = label1;
            functionBlocks[AnalysisType.Value].Settings = valueSetup;
            functionBlocks[AnalysisType.ValueCursor].Settings = graphSetup;

        }

        public void UpdateSettings(DataGridView gridView)
        {
            dataGridView = gridView;
            AnalysisType[] analysisTypes = new AnalysisType[]{ 
                                                AnalysisType.AWeighting,
                                                AnalysisType.Upsampling,
                                                AnalysisType.FFTAdapter,
                                                AnalysisType.FFTAnalysis,
                                                AnalysisType.OctaveAnalysis,
                                                AnalysisType.OctaveDetectorBank,
                                                AnalysisType.ThirdOctaveAnalysis,
                                                AnalysisType.ThirdOctaveDetectorBank,
                                                AnalysisType.BroadbandDetector,
                                                AnalysisType.GraphAdapter,
                                                AnalysisType.Graph,
                                                AnalysisType.ValueAdapter,
                                                AnalysisType.Value};

            for (int i = 0; i < functionBlocks.Count - 5; i++)
                bindingSource.Add(new BindingSourceObject((FunctionBlock)functionBlocks[analysisTypes[i]]));

            // Initialize the DataGridView.
            dataGridView.AutoGenerateColumns = false;
            dataGridView.AutoSize = false;
            dataGridView.AllowUserToResizeColumns = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToOrderColumns = false;
            dataGridView.RowHeadersVisible = false;
            dataGridView.Font = new System.Drawing.Font("Ariel", 10.0f, System.Drawing.FontStyle.Regular);
            dataGridView.DataSource = bindingSource;
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);

            // Initialize and add a text box column.
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Subscriber";
            column.Name = "Subscriber";
            column.Width = 180;
            dataGridView.Columns.Add(column);

            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.DataSource = new AnalysisType[]{ 
                                                AnalysisType.Soundcard,
                                                AnalysisType.Playback,
                                                AnalysisType.Generator,
                                                AnalysisType.AWeighting,
                                                AnalysisType.Upsampling,
                                                AnalysisType.FFTAdapter,
                                                AnalysisType.FFTAnalysis,
                                                AnalysisType.OctaveAnalysis,
                                                AnalysisType.OctaveDetectorBank,
                                                AnalysisType.ThirdOctaveAnalysis,
                                                AnalysisType.ThirdOctaveDetectorBank,
                                                AnalysisType.BroadbandDetector,
                                                AnalysisType.GraphAdapter,
                                                AnalysisType.ValueAdapter,
                                                AnalysisType.None};
            combo.DataPropertyName = "Publisher";
            combo.Name = "Publisher";
            combo.Width = 160;
            dataGridView.Columns.Add(combo);

            combo = new DataGridViewComboBoxColumn();
            string[] dataSource1 = new string[] { "", "All", "Linear", "Exponential", "Time", "Autospectrum", "Count", "200", "1000", };
            string[] dataSource = new string[dataSource1.Length+ DisplayComponent.cpb3Freq.Length];
            for (int i = 0; i < dataSource1.Length; i++)
                dataSource[i] = dataSource1[i];
            for (int i = 0; i < DisplayComponent.cpb3Freq.Length; i++)
                dataSource[i+dataSource1.Length] = DisplayComponent.cpb3Freq[i];
            combo.DataSource = dataSource;
            combo.DataPropertyName = "DataType";
            combo.Name = "DataType";
            combo.Width = 120;
            dataGridView.Columns.Add(combo);

            int h = dataGridView.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
            h += dataGridView.ColumnHeadersHeight;
            int w = dataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
            dataGridView.Height = h;
            dataGridView.Width = w;
            dataGridView.ScrollBars = ScrollBars.None;
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;

            functionBlocks[AnalysisType.AWeighting].Publisher = AnalysisType.Soundcard;
            functionBlocks[AnalysisType.Upsampling].Publisher = AnalysisType.Soundcard;
            functionBlocks[AnalysisType.FFTAdapter].Publisher = AnalysisType.Upsampling;
            functionBlocks[AnalysisType.FFTAnalysis].Publisher = AnalysisType.FFTAdapter;
            functionBlocks[AnalysisType.OctaveAnalysis].Publisher = AnalysisType.Soundcard;
            functionBlocks[AnalysisType.OctaveDetectorBank].Publisher = AnalysisType.OctaveAnalysis;
            functionBlocks[AnalysisType.ThirdOctaveAnalysis].Publisher = AnalysisType.Soundcard;
            functionBlocks[AnalysisType.ThirdOctaveDetectorBank].Publisher = AnalysisType.ThirdOctaveAnalysis;
            functionBlocks[AnalysisType.BroadbandDetector].Publisher = AnalysisType.Soundcard;
            functionBlocks[AnalysisType.GraphAdapter].Publisher = AnalysisType.FFTAnalysis;
            functionBlocks[AnalysisType.Graph].Publisher = AnalysisType.GraphAdapter;
            functionBlocks[AnalysisType.ValueAdapter].Publisher = AnalysisType.BroadbandDetector;
            functionBlocks[AnalysisType.Value].Publisher = AnalysisType.ValueAdapter;
            functionBlocks[AnalysisType.ValueCursor].Publisher = AnalysisType.Graph;

            functionBlocks[AnalysisType.AWeighting].DataType = "";
            functionBlocks[AnalysisType.Upsampling].DataType = "";
            functionBlocks[AnalysisType.FFTAdapter].DataType = "";
            functionBlocks[AnalysisType.FFTAnalysis].DataType = "";
            functionBlocks[AnalysisType.OctaveAnalysis].DataType = "";
            functionBlocks[AnalysisType.OctaveDetectorBank].DataType = "";
            functionBlocks[AnalysisType.ThirdOctaveAnalysis].DataType = "";
            functionBlocks[AnalysisType.ThirdOctaveDetectorBank].DataType = "";
            functionBlocks[AnalysisType.BroadbandDetector].DataType = "";
            functionBlocks[AnalysisType.GraphAdapter].DataType = "Autospectrum";
            functionBlocks[AnalysisType.Graph].DataType = "200";
            functionBlocks[AnalysisType.ValueAdapter].DataType = "Exponential";
            functionBlocks[AnalysisType.Value].DataType = "1000";
            functionBlocks[AnalysisType.ValueCursor].DataType = "1000";

            ((GraphHandler)functionBlocks[AnalysisType.Graph]).axisDescriptorDirty = true;

        }

        protected void display_previewKey(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        protected void display_keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                displayComponent.cursor--;
            if (e.KeyCode == Keys.Right)
                displayComponent.cursor++;
            base.OnKeyDown(e);
        }

        void button1_Click(object sender, EventArgs e)
        {
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StartSound();
        }

        void button2_Click(object sender, EventArgs e)
        {
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StartPlay();
        }

        void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            DataGridView view = sender as DataGridView;
            view.CommitEdit(DataGridViewDataErrorContexts.Commit);
            ((GraphHandler)functionBlocks[AnalysisType.Graph]).axisDescriptorDirty = true;
        }

        void button5_Click(object sender, EventArgs e)
        {
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StopSound();
        }

        void button6_Click(object sender, EventArgs e)
        {
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StopPlay();
        }

        void button7_Click(object sender, EventArgs e)
        {
            DialogResult result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = fileDialog.FileName;

                ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StopPlay();

                SoundCardSetup setup = functionBlocks[AnalysisType.Playback].Settings as SoundCardSetup;
                setup.path = path;
                functionBlocks[AnalysisType.Playback].Settings = setup;

                functionBlocks[AnalysisType.FFTAnalysis].Reset();
                ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StartPlay();
            }
        }

        void button8_Click(object sender, EventArgs e)
        {
            Reset();
        }

        void button9_Click(object sender, EventArgs e)
        {
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StopGenerator();
        }

        void button11_Click(object sender, EventArgs e)
        {
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StopGenerator();

            GeneratorSetup setup = functionBlocks[AnalysisType.Generator].Settings as GeneratorSetup;
            functionBlocks[AnalysisType.Generator].Settings = setup;

            functionBlocks[AnalysisType.FFTAnalysis].Reset();
            ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).StartGenerator();
        }

        void textBox1_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != 13)
                return;

            try
            {
                ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).PauseSound();
                ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).PausePlay();
                Thread.Sleep(100);

                M = int.Parse(textBox1.Text);
                if (M < 2) M = 2;

                FftAdapterSetup setup1 = functionBlocks[AnalysisType.FFTAdapter].Settings as FftAdapterSetup;
                setup1.length = 1 << M;
                functionBlocks[AnalysisType.FFTAdapter].Settings = setup1;

                FftSetup setup2 = functionBlocks[AnalysisType.FFTAnalysis].Settings as FftSetup;
                setup2.M = M;
                functionBlocks[AnalysisType.FFTAnalysis].Settings = setup2;

                ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).ContinueSound();
                ((FunctionBlockHandler)functionBlocks[AnalysisType.FBHandler]).ContinuePlay();

            }
            catch
            {

            }
        }

        void textBox2_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != 13)
                return;

            try
            {
                FftSetup setup = functionBlocks[AnalysisType.FFTAnalysis].Settings as FftSetup;
                setup.numberOfAverages = int.Parse(textBox2.Text);
                functionBlocks[AnalysisType.FFTAnalysis].Settings = setup;
            }
            catch
            {

            }
        }

        void textBox3_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != 13)
                return;

            try
            {
                SoundCardSetup setup = functionBlocks[AnalysisType.Playback].Settings as SoundCardSetup;
                setup.delay = int.Parse(textBox3.Text);
                functionBlocks[AnalysisType.Playback].Settings = setup;

                GeneratorSetup setupG = functionBlocks[AnalysisType.Generator].Settings as GeneratorSetup;
                setupG.delay = int.Parse(textBox3.Text);
                functionBlocks[AnalysisType.Generator].Settings = setupG;
            }
            catch
            {

            }
        }

        void textBox4_Enter(object sender, KeyEventArgs e)
        {
            double min_tic_x;
            double tic_intv_x;
            double tic_ratio_x;
            int nDecades_x;

            if (e.KeyValue != 13)
                return;

            try
            {
                displayComponent.min_x = double.Parse(textBox4.Text);
                displayComponent.min_tic_x = displayComponent.min_x;

                displayComponent.FindDisplayRange(displayComponent.axisType, displayComponent.min_x, displayComponent.max_x, out min_tic_x, out tic_intv_x, out tic_ratio_x, out nDecades_x);

                displayComponent.min_tic_x = min_tic_x;
                displayComponent.tic_intv_x = tic_intv_x;

                ((GraphHandler)functionBlocks[AnalysisType.Graph]).axisDirty = true;

                ((GraphHandler)functionBlocks[AnalysisType.Graph]).UpdateGraph();
                ((ValueHandler)functionBlocks[AnalysisType.Value]).UpdateLabel();

                displayComponent.SizeChanged();
            }
            catch
            {

            }
        }

        void textBox5_Enter(object sender, KeyEventArgs e)
        {
            double min_tic_x;
            double tic_intv_x;
            double tic_ratio_x;
            int nDecades_x;

            if (e.KeyValue != 13)
                return;

            try
            {
                displayComponent.max_x = double.Parse(textBox5.Text);

                displayComponent.FindDisplayRange(displayComponent.axisType, displayComponent.min_x, displayComponent.max_x, out min_tic_x, out tic_intv_x, out tic_ratio_x, out nDecades_x);

                displayComponent.min_tic_x = min_tic_x;
                displayComponent.tic_intv_x = tic_intv_x;

                ((GraphHandler)functionBlocks[AnalysisType.Graph]).axisDirty = true;

                ((GraphHandler)functionBlocks[AnalysisType.Graph]).UpdateGraph();
                ((ValueHandler)functionBlocks[AnalysisType.Value]).UpdateLabel();

                displayComponent.SizeChanged();

            }
            catch
            {

            }
        }

        void textBox6_Enter(object sender, KeyEventArgs e)
        {
            double min_tic_y;
            double tic_intv_y;
            double tic_ratio_y;
            int nDecades_y;

            if (e.KeyValue != 13)
                return;

            try
            {
                displayComponent.min_y = double.Parse(textBox6.Text);
                displayComponent.min_tic_y = displayComponent.min_y;

                displayComponent.FindDisplayRange(DisplayComponent.AxisType.Lin, displayComponent.min_y, displayComponent.max_y, out min_tic_y, out tic_intv_y, out tic_ratio_y, out nDecades_y);

                displayComponent.min_tic_y = min_tic_y;
                displayComponent.tic_intv_y = tic_intv_y;

                displayComponent.SizeChanged();
            }
            catch
            {

            }
        }

        void textBox7_Enter(object sender, KeyEventArgs e)
        {
            double min_tic_y;
            double tic_intv_y;
            double tic_ratio_y;
            int nDecades_y;

            if (e.KeyValue != 13)
                return;

            try
            {
                displayComponent.max_y = double.Parse(textBox7.Text);

                displayComponent.FindDisplayRange(DisplayComponent.AxisType.Lin, displayComponent.min_y, displayComponent.max_y, out min_tic_y, out tic_intv_y, out tic_ratio_y, out nDecades_y);

                displayComponent.min_tic_y = min_tic_y;
                displayComponent.tic_intv_y = tic_intv_y;

                displayComponent.SizeChanged();
            }
            catch
            {

            }
        }

        private void textBox8_Enter(object sender, KeyEventArgs e)
        {

            if (e.KeyValue != 13)
                return;

            try
            {
                ((Upsampling)functionBlocks[AnalysisType.Upsampling]).Settings = upsamplingSetup;
                ((Upsampling)functionBlocks[AnalysisType.Upsampling]).Publisher = ((Upsampling)functionBlocks[AnalysisType.Upsampling]).Publisher;

            }
            catch
            {

            }
        }
    }
}
