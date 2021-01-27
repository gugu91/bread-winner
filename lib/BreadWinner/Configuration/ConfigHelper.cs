using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace BreadWinner
{
    // TODO: refactor class for better testability
    [ExcludeFromCodeCoverage]
    internal class ConfigHelper
    {
        private static ThreadPriority? _threadPriority;
        private static bool? _throwsExceptions;
        private static int? _threadWaitTime;

        internal static ThreadPriority ThreadPriority
        {
            get
            {
                if (_threadPriority != null)
                {
                    return _threadPriority.Value;
                }

                _threadPriority = ThreadPriority.Normal;
                var valueString = ConfigurationManager.AppSettings[ConfigOptions.ThreadPriority];
                if (valueString == null)
                {
                    return _threadPriority.Value;
                }

                _threadPriority = Enum.TryParse(valueString, out ThreadPriority threadPriority)
                    ? threadPriority
                    : ThreadPriority.Normal;

                return _threadPriority.Value;
            }
        }

        internal static bool ThrowExceptions
        {
            get
            {
                if (_throwsExceptions.HasValue)
                {
                    return _throwsExceptions.Value;
                }

                _throwsExceptions = false;
                var valueString = ConfigurationManager.AppSettings[ConfigOptions.ThrowExceptions];
                if (valueString == null)
                {
                    return _throwsExceptions.Value;
                }

                if (bool.TryParse(valueString, out var throwExceptions))
                {
                    _throwsExceptions = throwExceptions;
                }

                return _throwsExceptions.Value;
            }
        }

        public static int ThreadWaitTime
        {
            get
            {
                if (_threadWaitTime.HasValue) return _threadWaitTime.Value;
                _threadWaitTime = 0;

                var tp = ConfigurationManager.AppSettings[ConfigOptions.WaitXForthreads];
                if (tp == null)
                {
                    return _threadWaitTime.Value;
                }

                if (int.TryParse(tp, out var tpInt))
                {
                    _threadWaitTime = tpInt;
                }

                return _threadWaitTime.Value;
            }
        }
    }
}