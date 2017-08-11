using System;

namespace JH.Applications
{
    public interface IGenerator
    {
        GeneratorAbstraction BaseClass
        {
            set;
        }
        void GenerateNextBuffer();
        void Init();
    }
}
