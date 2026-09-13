using System;

namespace TGL.FSM.Exceptions
{
    public class FsmException : Exception
    {
        /*private string _exceptionMessage;
        private Exception  _exception;
        
        public string ExceptionMessage => _exceptionMessage;
        public Exception InnerException => _exception;
        */

        public FsmException(string message) : base(message)
        {
            /*base(message);
            _exceptionMessage = message;*/
        }

        public FsmException(string message, Exception inner) : base(message, inner)
        {
            /*base(message, inner);
            _exceptionMessage = message;
            _exception = inner;*/
        }
    }
}