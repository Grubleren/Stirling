using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace JH.Applications
{
    public unsafe class SoundCard : FunctionBlock, ISoundCardBase, IObservable<DataObject>
    {
        short[] buffer;
        SoundCardSetup setup;
        double[] outputData;
        SoundCardDelegate callBack;

        public SoundCard(SoundCardDelegate callBack)
        {
            output = new DataObject();
            output.dataElements = new DataObjectElement[1];
            output.dataElements[0] = new DataObjectElement("",0);
            setup = new SoundCardSetup();
            this.callBack = callBack;
        }

        public void Compute()
        {
            int status = 0;

            try
            {
                while (setup.running)
                {
                    lock (this)
                    {
                        fixed (short* pBuffer = buffer)
                            status = Wait(pBuffer);
                    }

                   // Console.WriteLine(status);

                    for (int i = 0; i < setup.length; i++)
                        outputData[i] = buffer[2 * i] * setup.sesitivity;

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
            lock (this)
            {
                buffer = new short[setup.length * 2]; // stereo

                Stop();
                callBack(Start(setup.length, setup.samplingFrequency));

            }
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

                if (setup == null ||
                    s.length != setup.length)
                {
                    outputData = new double[s.length];
                    output.dataElements[0].data = outputData;
                }

                setup.Copy(s);
            }
        }

        [DllImport("RecorderWin32.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool Start(int bufferlength, int fs);
        [DllImport("RecorderWin32.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Stop();
        [DllImport("RecorderWin32.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int Wait(short* pBuffer);
    }


}
