using System;

namespace Log
{
    public interface ILogSink
    {
        bool IsLevelAvailable(LogLevel level);
        bool IsTagAvailable(object     tag);
        void Msg(object                tag, LogLevel  lvl, string msg);
        void Exception(object          tag, Exception exception);
    }
}