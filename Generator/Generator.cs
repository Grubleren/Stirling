using System;
using System.IO;
using System.Threading;

namespace JH.Applications
{
    public class Generator : FunctionBlock, ISoundCardBase, IObservable<DataObject>
    {
        GeneratorAbstraction abstraction;
        IGenerator sine;
        IGenerator whiteNoise;
        IGenerator launchpad;
        GeneratorSetup setup;

        public Generator()
        {
            output = new DataObject();
            output.dataElements = new DataObjectElement[1];
            output.dataElements[0] = new DataObjectElement("",0);
            abstraction = new GeneratorAbstraction();
            sine = new Sine();
            whiteNoise = new WhiteNoise();
            launchpad = new Launchpad();
            setup = new GeneratorSetup();
        }

        public void Compute()
        {
            try
            {
                abstraction.Init();

                while (setup.running)
                {
                    Thread.Sleep(setup.delay);

                    abstraction.GenerateNextBuffer();

                    if (!setup.paused)
                        TraverseSubscribers();

                }
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
                GeneratorSetup s = value as GeneratorSetup;

                if (setup == null ||
                    s.length != setup.length)
                {
                    abstraction.buffer = new double[s.length];
                    output.dataElements[0].data = abstraction.buffer;
                    s.running = true;
                }
                abstraction.setup = s;
                switch (s.generatorType)
                {
                    case GeneratorType.Sine:
                        abstraction.Attach(sine);
                        break;
                    case GeneratorType.WhiteNoise:
                        abstraction.Attach(whiteNoise);
                        break;
                    case GeneratorType.Launchpad:
                        abstraction.Attach(launchpad);
                        break;
                }

                setup.Copy(s);
            }
        }
    }
    public class GeneratorSetup : ISetup
    {
        public GeneratorType generatorType;
        public int samplingFrequency;
        public double frequency;
        public double level;
        public int length;
        public bool running;
        public bool paused;
        public int delay;
        

        public void Copy(GeneratorSetup setup)
        {
            generatorType = setup.generatorType;
            samplingFrequency = setup.samplingFrequency;
            frequency = setup.frequency;
            level = setup.level;
            length = setup.length;
            running = setup.running;
            paused = setup.paused;
            delay = setup.delay;
        }

        public object Clone()
        {
            GeneratorSetup s = new GeneratorSetup();
            s.Copy(this);
            return s;
        }

    }
}
