namespace TGL.FSM.Exceptions
{
    public class FSMResult
    {
        public bool IsCancelled { get; private set; }
        public bool HasException { get; private set; }
        // public bool IsCompleted { get; private set; }
        public bool IsCompletedSuccessfully { get; private set; }
        
        public FsmException Exception { get; private set; }
        
        private FSMResult()
        {
            IsCancelled = false;
            HasException = false;
            // IsCompleted = true;
            IsCompletedSuccessfully = false;
            Exception = null;
        }

        public FSMResult(bool isCancelled, bool isCompletedSuccessfully, FsmException exception)
        {
            IsCancelled = isCancelled;
            HasException = exception != null;
            // IsCompleted = true;
            IsCompletedSuccessfully = isCompletedSuccessfully;
            Exception = exception;
        }
        
        public static FSMResult GetSuccess()
        {
            return new FSMResult()
            {
                IsCancelled =  false,
                HasException = false,
                // IsCompleted = true,
                IsCompletedSuccessfully = true
            };
        }

        public static FSMResult GetCancelled()
        {
            return new FSMResult()
            {
                IsCancelled = true,
                HasException = false,
                // IsCompleted = true,
                IsCompletedSuccessfully = false,
            };
        }
    }
}