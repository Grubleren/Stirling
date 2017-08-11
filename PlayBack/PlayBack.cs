using System;
using System.IO;
using System.Threading;

namespace JH.Applications
{
    public unsafe class PlayBack : FunctionBlock, ISoundCardBase, IObservable<DataObject>
    {
        FileStream stream;
        BinaryReader reader;
        WaveIO waveIO;
        SoundCardSetup setup;
        double[] outputData;

        public PlayBack()
        {
            output = new DataObject();
            output.dataElements = new DataObjectElement[1];
            output.dataElements[0] = new DataObjectElement("",0);
            waveIO = new WaveIO();
            setup = new SoundCardSetup();
        }

        public void Compute()
        {
            try
            {
                while (setup.running)
                {
                    Thread.Sleep(setup.delay);

                    waveIO.ReadSamples(reader, outputData);

                    for (int i = 0; i < outputData.Length; i++)
                    {
                        outputData[i] *= setup.sesitivity / (1 << 16);
                    }
                    if (!setup.paused)
                        TraverseSubscribers();

                    if (reader.BaseStream.Position >= reader.BaseStream.Length - waveIO.BlockAlign * setup.length)
                        setup.running = false;
                }
                reader.Close();
                foreach (IObserver<DataObject> subscriber in subscribers)
                    subscriber.OnCompleted();
            }
            catch
            {
                lock (this)
                {
                    foreach (IObserver<DataObject> subscriber in subscribers)
                        subscriber.OnError(new Exception(ToString()));
                }
            }
        }

        public override void Reset()
        {

        }

        public override ISetup Settings
        {
            get
            {
                return (ISetup)setup.Clone();
            }
            set
            {
                SoundCardSetup s = value as SoundCardSetup;

                if (stream != null)
                    stream.Close();
                stream = new FileStream(s.path, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(stream);
                waveIO.ReadWaveFileInfo(reader);
                reader.BaseStream.Seek(waveIO.DataStart, SeekOrigin.Begin);

                outputData = new double[s.length];
                output.dataElements[0].data = outputData;
                setup.Copy(s);
            }
        }
    }
}
