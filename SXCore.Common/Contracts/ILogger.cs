using System;

namespace SXCore.Common.Contracts
{
    public interface ILogger
    {
        void Log(string message);
        void Log(Exception exception);

        void Error(string message);
        void Error(Exception exception);
    }
}
