using System.Collections.Generic;

namespace Network
{
    internal sealed class Node
    {
        internal bool isOpen { get; private set; }
        internal bool isSource { get; private set; }
        internal bool isSink { get; private set; }

        private List<Pipe> _pipes = new List<Pipe>();
        private float _sourceRate;
        private float _sinkPressure;

        internal void Open()
        { 
            isOpen = true;
        }

        internal void Close()
        { 
            isOpen = false;
        }

        internal void AddPipe(Pipe pipe)
        { 
            _pipes.Add(pipe);
        }

        internal void RemovePipe(Pipe pipe)
        {
            _pipes.Remove(pipe);
        }

        internal void SetAsSource(float sourceRate)
        {
            if (sourceRate <= 0) return;
            _sourceRate = sourceRate;
            isSource= true;
            isSink= false;
        }

        internal void SetAsSink(float sinkPressure)
        { 
            if (sinkPressure <= 1) return;
            _sinkPressure = sinkPressure;
            isSink= true;
            isSource= false;
        }
    }
}
