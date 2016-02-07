using System;

namespace IsThereAnybodyOutThere.Client.Messages
{
    public class ErrorOccurred
    {
        public string Message { get; }
        public Exception Exception { get; }

        public ErrorOccurred(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}