using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using JH.Calculations;
using System.Runtime.InteropServices;

namespace JH.Applications
{
    public delegate void SoundCardDelegate(bool value);

    public interface ISetup
    {
    }

    public interface ISoundCardBase
    {
        void Compute();
    }

    public class DefaultSetup : ISetup
    {
        

        public void Copy(DefaultSetup setup)
        {
            
        }

        public object Clone()
        {
            DefaultSetup s = new DefaultSetup();
            s.Copy(this);
            return s;
        }
    }

    public class SoundCardSetup : ISetup
    {
        public int length;
        public String path;
        public int delay;
        public bool running;
        public bool paused;
        public int samplingFrequency;
        public double sesitivity;
        

        public void Copy(SoundCardSetup setup)
        {
            length = setup.length;
            path = setup.path;
            delay = setup.delay;
            running = setup.running;
            paused = setup.paused;
            samplingFrequency = setup.samplingFrequency;
            sesitivity = setup.sesitivity;
        }

        public object Clone()
        {
            SoundCardSetup s = new SoundCardSetup();
            s.Copy(this);
            return s;
        }
    }

    public class DescriptorBase
    {
        public enum DescriptorType
        {
            Axis,
            Type
        }

        public DescriptorType type;
    }

    public class AxisDescriptor : DescriptorBase
    {
        public AxisDescriptor()
        {
            type = DescriptorType.Axis;
        }

        public double min;
        public double max;
        public double step;
        public int minIndex;
        public int maxIndex;
        public DisplayComponent.AxisType axisType;

    }

    public class TypeDescriptor : DescriptorBase
    {
        public string[] names;

        public TypeDescriptor(int ndata)
        {
            type = DescriptorType.Type;
            names = new string[ndata];
        }
    }

    public class DataObjectElement
    {
        public object data;
        public string name;
        public int val;
        public List<DescriptorBase> descriptors = new List<DescriptorBase>();

        public DataObjectElement(string name, int val)
        {
            this.name = name;
            this.val = val;
        }

        public void AddDescriptor(DescriptorBase descriptor)
        {
            descriptors.Add(descriptor);
        }

        public DescriptorBase FindDescriptor(DescriptorBase.DescriptorType type)
        {
            foreach (DescriptorBase descriptor in descriptors)
            {
                if (descriptor.type == type)
                    return descriptor;
            }
            return null;
        }

    }

    public class DataObject
    {
        public DataObjectElement[] dataElements;

        public DataObject()
        {

        }

        public int FindName(string name)
        {
            if (dataElements == null)
                return 0;

            for (int i=0;i< dataElements.Length;i++)
            {
                if (dataElements[i].name == name)
                    return dataElements[i].val;
            }

            return 0;
        }

        public int DataSize()
        {
            return dataElements.Length;
        }

        public DataObjectElement GetData(int index)
        {
            return dataElements[index] as DataObjectElement;
        }
    }

    public class FunctionBlocks : Hashtable
    {
        public void Add(FunctionBlock functionBlock, AnalysisType analysisType)
        {
            functionBlock.analysisType = analysisType;
            functionBlock.FunctionBlocks = this;
            base.Add(functionBlock.analysisType, functionBlock);
        }

        public FunctionBlock this[AnalysisType analysisType]
        {
            get { return (FunctionBlock)base[analysisType]; }
        }
    }

    public abstract class FunctionBlock
    {
        protected FunctionBlocks functionBlocks;
        protected ArrayList subscribers = new ArrayList();
        protected IIterator iterator;
        public DataObject output;
        protected bool error;
        protected IDisposable disposable;
        public AnalysisType analysisType;
        protected AnalysisType publisher;
        public int dataSelector;
        string dataType;

        public FunctionBlocks FunctionBlocks
        {
            set { functionBlocks = value; }
        }

        public virtual ISetup Settings
        {
            get { return null; }
            set { }
        }
        public virtual void Reset()
        {
            error = false;
        }
        public virtual IIterator CreateIterator()
        {
            return new IteratorSimple(this);
        }
        public IIterator GetIterator()
        {
            return iterator;
        }
        public void SetIterator(IIterator iterator)
        {
            this.iterator = iterator;
        }
        public IDisposable Subscribe(IObserver<DataObject> observer)
        {
            lock (this)
                subscribers.Add(observer);
            return new Cancel(observer, this);
        }
        public void Unsubscribe(IObserver<DataObject> observer)
        {
            lock (this)
                subscribers.Remove(observer);
        }
        public IDisposable Disposable
        {
            get { return disposable; }
            set { disposable = value; }
        }
        protected void TraverseSubscribers()
        {
            lock (this)
            {
                foreach (IObserver<DataObject> subscriber in subscribers)
                {
                    IIterator iterator = ((FunctionBlock)subscriber).GetIterator();
                    if (!iterator.IsDone())
                        subscriber.OnNext(iterator.Next(subscriber as FunctionBlock));
                }
            }
        }
        protected void TraverseError()
        {
            lock (this)
            {
                foreach (IObserver<DataObject> subscriber in subscribers)
                    subscriber.OnError(new Exception(ToString()));
            }
            throw new Exception(ToString());

        }

        public AnalysisType Publisher
        {
            get
            {
                return publisher;
            }
            set
            {
                if (value == analysisType)
                {
                    Publisher = AnalysisType.None;
                    return;
                }
                publisher = value;

                FunctionBlock observer;
                FunctionBlock observable;

                observable = functionBlocks[publisher] as FunctionBlock;
                observer = this;
                if (observer.Disposable != null)
                {
                    observer.Disposable.Dispose();
                    observer.Disposable = null;
                }
                if (observable == null)
                    return;
                observer.SetIterator(observable.CreateIterator());
                observer.Disposable = observable.Subscribe(observer as IObserver<DataObject>);
            }
        }

        public string DataType
        {
            get { return dataType; }
            set
            {
                dataType = value;
                dataSelector = functionBlocks[publisher].output.FindName( value); }
            }
    }

    public interface IIterator
    {
        DataObject Next(FunctionBlock subscriber);
        bool IsDone();
    }

    public class IteratorSimple : IIterator
    {
        FunctionBlock functionBlock;
        public IteratorSimple(FunctionBlock functionBlock)
        {
            this.functionBlock = functionBlock;
        }
        public string Type()
        {
            return "all";
        }
        public DataObject Next(FunctionBlock subscriber)
        {
            return functionBlock.output;
        }
        public bool IsDone()
        {
            return false;
        }

    }

    public class IteratorTime : IIterator
    {
        FunctionBlock functionBlock;
        System.Threading.Timer timer;
        bool done;
        int time;

        public IteratorTime(FunctionBlock functionBlock)
        {
            this.functionBlock = functionBlock;
            time = functionBlock.dataSelector;
            timer = new System.Threading.Timer(timer_Tick, null, 0, time);
        }
        void timer_Tick(object sender)
        {
            lock (this)
            {
                done = false;
            }
        }
        public string Type()
        {
            return "200 ms";
        }
        public DataObject Next(FunctionBlock subscriber)
        {
            lock (this)
            {
                done = true;
                if (time != subscriber.dataSelector)
                {
                    time = subscriber.dataSelector;
                    timer.Change(0, time);
                }
                return functionBlock.output;
            }
        }
        public bool IsDone()
        {
            lock (this)
            {
                return done;
            }
        }

    }

    public enum GeneratorType
    {
        Sine,
        WhiteNoise,
        PinkNoise,
        LinearSweep,
        ExponentialSweep,
        Launchpad,
    }

    public enum AnalysisType
    {
        FBHandler,
        Soundcard,
        Playback,
        Generator,
        AWeighting,
        Upsampling,
        FFTAdapter,
        FFTAnalysis,
        OctaveAnalysis,
        OctaveDetectorBank,
        ThirdOctaveAnalysis,
        ThirdOctaveDetectorBank,
        BroadbandDetector,
        GraphAdapter,
        Graph,
        ValueAdapter,
        Value,
        ValueCursor,
        None,
    };

    public class Cancel : IDisposable
    {
        IObserver<DataObject> observer;
        FunctionBlock observable;
        bool disposed;
        public Cancel(IObserver<DataObject> observer, FunctionBlock observable)
        {
            this.observer = observer;
            this.observable = observable;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                observable.Unsubscribe(observer);
                disposed = true;
            }
        }
    }
}
