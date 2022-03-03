using System;
using System.Runtime.CompilerServices;

namespace Log
{
    public class Logger
    {
        public readonly ILogSink LogSink;

        public Logger(ILogSink logSink)
        {
            LogSink = logSink;
            _trace = new Level(this, LogLevel.Trace);
            _info = new Level(this, LogLevel.Info);
            _checkpoint = new Level(this, LogLevel.Checkpoint);
            _warning = new Level(this, LogLevel.Warning);
            _error = new Level(this, LogLevel.Error);
            _exception = new LevelException(this);
        }

        private readonly LevelException _exception;
        private readonly Level          _error;
        private readonly Level          _warning;
        private readonly Level          _checkpoint;
        private readonly Level          _info;
        private readonly Level          _trace;

        public LevelException Exception  => LogSink.IsLevelAvailable(LogLevel.Exception) ? _exception : null;
        public Level          Error      => LogSink.IsLevelAvailable(LogLevel.Error) ? _error : null;
        public Level          Warning    => LogSink.IsLevelAvailable(LogLevel.Warning) ? _warning : null;
        public Level          Checkpoint => LogSink.IsLevelAvailable(LogLevel.Checkpoint) ? _checkpoint : null;
        public Level          Info       => LogSink.IsLevelAvailable(LogLevel.Info) ? _info : null;
        public Level          Trace      => LogSink.IsLevelAvailable(LogLevel.Trace) ? _trace : null;

        // public TaggedLogger? this[object tag]
        // {
        //     get
        //     {
        //         if(LogSink.IsTagAvailable(tag)) return new TaggedLogger(this, tag, LogSink);
        //         return null;
        //     }
        // }

        public class Level
        {
            private readonly Logger   _logger;
            private readonly LogLevel _level;

            public Level(Logger logger, LogLevel level)
            {
                _logger = logger;
                _level = level;
            }

            public void Msg(string msg)
            {
                _logger.LogSink.Msg(null, _level, msg);
            }

            public TaggedLevel? this[object tag] => _logger.LogSink.IsTagAvailable(tag)
                ? new TaggedLevel(_logger, _level, tag)
                : TaggedLevel.Null;
        }

        public class LevelException
        {
            private readonly Logger _logger;

            public LevelException(Logger logger)
            {
                _logger = logger;
            }

            public void Exc(Exception exception)
            {
                _logger.LogSink.Exception(null, exception);
            }

            public TaggedLevelException? this[object tag]
            {
                get
                {
                    if(_logger.LogSink.IsTagAvailable(tag)) return new TaggedLevelException(_logger, tag);
                    return null;
                }
            }
        }

        public readonly struct TaggedLevel
        {
            private readonly Logger   _logger;
            private readonly LogLevel _level;
            private readonly object   _tag;

            internal static readonly TaggedLevel? Null = null;

            public TaggedLevel(Logger logger, LogLevel level, object tag)
            {
                _logger = logger;
                _level = level;
                _tag = tag;
            }

            public void Msg(string msg)
            {
                _logger.LogSink.Msg(_tag, _level, msg);
            }
        }

        public readonly struct TaggedLevelException
        {
            private readonly Logger _logger;
            private readonly object _tag;

            public TaggedLevelException(Logger logger, object tag)
            {
                _logger = logger;
                _tag = tag;
            }

            public void Exc(Exception exception)
            {
                _logger.LogSink.Exception(_tag, exception);
            }
        }

        public readonly struct TaggedLogger
        {
            private readonly ILogSink _logSink;
            private readonly object   _tag;
            private readonly Logger   _logger;

            public TaggedLogger(Logger logger, object tag, ILogSink logSink)
            {
                _logSink = logSink;
                _tag = tag;
                _logger = logger;
            }

            public TaggedLevelException? Exception => _logSink.IsLevelAvailable(LogLevel.Exception)
                ? new TaggedLevelException(_logger, _tag)
                : (TaggedLevelException?)null;

            public TaggedLevel? Error      => GetLevel(LogLevel.Error);
            public TaggedLevel? Warning    => GetLevel(LogLevel.Warning);
            public TaggedLevel? Checkpoint => GetLevel(LogLevel.Checkpoint);
            public TaggedLevel? Info       => GetLevel(LogLevel.Info);
            public TaggedLevel? Trace      => GetLevel(LogLevel.Trace);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private TaggedLevel? GetLevel(LogLevel level)
            {
                return _logSink.IsLevelAvailable(level) ? new TaggedLevel(_logger, level, _tag) : TaggedLevel.Null;
            }
        }
    }

    public enum LogLevel : byte
    {
        Exception,
        Error,
        Warning,
        Checkpoint,
        Info,
        Trace
    }
}