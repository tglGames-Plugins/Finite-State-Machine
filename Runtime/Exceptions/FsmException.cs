using System;

namespace TGL.FSM.Exceptions
{
    public class FsmException : Exception
    {
        public FsmException()
        {
        }

        public FsmException(string message) : base(message)
        {
        }

        public FsmException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}