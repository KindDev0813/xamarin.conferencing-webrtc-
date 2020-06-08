namespace WebRTC.H113.Schedulers
{
    public interface IExecutorService : IExecutor
    {
        void Release();
    }
}