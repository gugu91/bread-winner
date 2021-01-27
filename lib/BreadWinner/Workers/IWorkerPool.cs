namespace BreadWinner
{
    internal interface IWorkerPool : IWorker
    {
        void Add(params IWorker[] workers);
    }
}