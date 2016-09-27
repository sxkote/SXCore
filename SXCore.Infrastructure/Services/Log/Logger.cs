using System;
using SXCore.Common.Contracts;
using System.Diagnostics;

namespace SXCore.Infrastructure.Services.Log
{
    public class Logger : ILogger
    {
        protected readonly Action<string> _action = null;

        public Logger(Action<string> action)
        {
            _action = action;
        }

        protected virtual void Write(string text)
        {
            if (_action != null)
                _action(text);
        }

        protected virtual string CreateExceptionMessage(Exception exception)
        {
            if (exception == null)
                return "";

            string message = exception.Message;

            if (exception.InnerException != null)
                message += Environment.NewLine + "  - Inner Exception: " + this.CreateExceptionMessage(exception.InnerException);

            return message;
        }

        public void Error(Exception exception)
        {
            this.Write("Error: " + this.CreateExceptionMessage(exception));
        }

        public void Error(string message)
        {
            this.Write("Error:" + message ?? "");
        }

        public void Log(Exception exception)
        {
            this.Write(this.CreateExceptionMessage(exception));
        }

        public void Log(string message)
        {
            this.Write(message ?? "");
        }
    }

    public class DebugLogger : Logger
    {
        public DebugLogger()
            : base(text => Debug.WriteLine(text))
        { }
    }

    public class ConsoleLogger: Logger
    {
        public ConsoleLogger()
            : base(text => Console.WriteLine(text))
        { }
    }
}
