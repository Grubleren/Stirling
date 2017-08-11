using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace JH.Applications
{
    public class FunctionBlockHandler : FunctionBlock, IObserver<DataObject>
    {
        Button soundIndicator;
        Button playIndicator;
        Button generatorIndicator;
        Thread threadSoundCard;
        Thread threadPlayBack;
        Thread threadGenerator;
        SoundCard soundCard;
        PlayBack playBack;
        Generator generator;
        IIterator iteratorSound;
        IIterator iteratorPlay;
        IIterator iteratorGenerator;

        public FunctionBlockHandler()
        {
        }

        public void Init(Button soundIndicator, Button playIndicator, Button generatorIndicator)
        {
            this.soundIndicator = soundIndicator;
            this.playIndicator = playIndicator;
            this.generatorIndicator = generatorIndicator;

            this.soundCard = (SoundCard)functionBlocks[AnalysisType.Soundcard];
            this.playBack = (PlayBack)functionBlocks[AnalysisType.Playback];
            this.generator = (Generator)functionBlocks[AnalysisType.Generator];
        }

        public void StartSound()
        {
            if (threadSoundCard != null)
                return;

            soundIndicator.BackColor = Color.FromArgb(192, 0, 0);
            SetIterator(soundCard.CreateIterator());
            iteratorSound = iterator;
            soundCard.Subscribe(this);

            SoundCardSetup setup = soundCard.Settings as SoundCardSetup;
            setup.running = true;
            soundCard.Settings = setup;

            threadSoundCard = new Thread(new ThreadStart(soundCard.Compute));
            threadSoundCard.Name = "SoundCard";
            threadSoundCard.Start();
        }

        public void StartPlay()
        {
            if (threadPlayBack != null)
                return;

            playIndicator.BackColor = Color.FromArgb(192, 0, 0);
            SetIterator(playBack.CreateIterator());
            iteratorPlay = iterator;
            playBack.Subscribe(this);

            SoundCardSetup setup = playBack.Settings as SoundCardSetup;
            setup.running = true;
            playBack.Settings = setup;

            threadPlayBack = new Thread(new ThreadStart(playBack.Compute));
            threadPlayBack.Name = "PlayBack";
            threadPlayBack.Start();
        }
        
        public void StartGenerator()
        {
            if (threadGenerator != null)
                return;

            generatorIndicator.BackColor = Color.FromArgb(192, 0, 0);
            SetIterator(generator.CreateIterator());
            iteratorGenerator = iterator;
            generator.Subscribe(this);

            GeneratorSetup setup = generator.Settings as GeneratorSetup;
            setup.running = true;
            generator.Settings = setup;

            threadGenerator = new Thread(new ThreadStart(generator.Compute));
            threadGenerator.Name = "Generator";
            threadGenerator.Start();
        }

        public void StopSound()
        {
            SoundCardSetup setup = soundCard.Settings as SoundCardSetup;
            setup.running = false;
            soundCard.Settings = setup;
            threadSoundCard = null;
        }

        public void StopPlay()
        {
            SoundCardSetup setup = playBack.Settings as SoundCardSetup;
            setup.running = false;
            playBack.Settings = setup;
            threadPlayBack = null;
        }
        public void StopGenerator()
        {
            GeneratorSetup setup = generator.Settings as GeneratorSetup;
            setup.running = false;
            generator.Settings = setup;
            threadGenerator = null;
        }
        public void PauseSound()
        {
            SoundCardSetup setup = soundCard.Settings as SoundCardSetup;
            setup.paused = true;
            soundCard.Settings = setup;
        }
        public void PausePlay()
        {
            SoundCardSetup setup = playBack.Settings as SoundCardSetup;
            setup.paused = true;
            playBack.Settings = setup;
        }
        public void PauseGenerator()
        {
            GeneratorSetup setup = generator.Settings as GeneratorSetup;
            setup.paused = true;
            generator.Settings = setup;
        }
        public void ContinueSound()
        {
            SoundCardSetup setup = soundCard.Settings as SoundCardSetup;
            setup.paused = false;
            soundCard.Settings = setup;
        }
        public void ContinuePlay()
        {
            SoundCardSetup setup = playBack.Settings as SoundCardSetup;
            setup.paused = false;
            playBack.Settings = setup;
        }
        public void ContinueGenerator()
        {
            GeneratorSetup setup = generator.Settings as GeneratorSetup;
            setup.paused = false;
            generator.Settings = setup;
        }
        public void OnNext(DataObject data)
        {
        }

        public void OnCompleted()
        {
            SoundCardSetup setupCard = soundCard.Settings as SoundCardSetup;
            if (!setupCard.running)
            {
                soundIndicator.BackColor = Color.FromArgb(0, 192, 0);
                threadSoundCard = null;
            }

            SoundCardSetup setupPlay = playBack.Settings as SoundCardSetup;
            if (!setupPlay.running)
            {
                playIndicator.BackColor = Color.FromArgb(0, 192, 0);
                threadPlayBack = null;
            }

            GeneratorSetup setupGenerator = generator.Settings as GeneratorSetup;
            if (!setupGenerator.running)
            {
                generatorIndicator.BackColor = Color.FromArgb(0, 192, 0);
                threadGenerator = null;
            }

            //foreach (DictionaryEntry functionBlock in functionBlocks)
            //    ((FunctionBlock)functionBlock.Value).Reset();

        }

        public void OnError(Exception e)
        {
            if (e.Message.Contains("SoundCard"))
            {
                soundIndicator.BackColor = Color.Black;
                threadSoundCard = null;
            }
            else if (e.Message.Contains("PlayBack"))
            {
                playIndicator.BackColor = Color.Black;
                threadPlayBack = null;
            }
            else if (e.Message.Contains("Generator"))
            {
                generatorIndicator.BackColor = Color.Black;
                threadGenerator = null;
            }

            foreach (DictionaryEntry functionBlock in functionBlocks)
                ((FunctionBlock)functionBlock.Value).Reset();
        }


    }
}
