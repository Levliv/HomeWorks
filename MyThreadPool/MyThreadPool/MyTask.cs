namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        public bool IsCompleted { get; set; } = false;
        public AggregateException? AggregateException { get; set; } = null;
        private TResult result;
        private Func<TResult> func;
        private readonly ManualResetEvent manualResetEvent = new(false);
        private readonly Queue<Action> continueWithTasks = new();
        private MyThreadPool myThreadPool;
        private object locker = new();
        public TResult Result
        {
            get
            {
                manualResetEvent.WaitOne();
                if (AggregateException == null)
                {
                    return result;
                }
                throw AggregateException;
            }
        }
        public MyTask(Func<TResult> func, MyThreadPool myThreadPool)
        {
            this.myThreadPool = myThreadPool;
            this.func = func;

        }
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            TNewResult Func() => func(Result);
            var task = new MyTask<TNewResult>(Func, myThreadPool);
            lock (locker)
            {
                if (IsCompleted)
                {
                    return myThreadPool.Add(Func);
                }
                continueWithTasks.Enqueue(task.Run);
                return task;
            }

        }
        public void Run()
        {
            try
            {

            }
            catch (Exception ex)
            {
                AggregateException = new(ex);
            }
            finally
            {
                lock (locker)
                {
                    while (continueWithTasks.Count > 0)
                    {
                        continueWithTasks.Dequeue()();
                    }
                    IsCompleted = true;
                    func = null;
                    manualResetEvent.Set();

                }
            }
        }
    }
}
