using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Telemetry
{
    public class TrackEventConsoleListener
    {
        private static readonly ConsoleColor? DefaultConsoleColor = null;
        private static readonly int _leftPad;
        private static readonly string _leftPadString;
        private static StringBuilder _logBuilder;
        private readonly object _lock = new object();

        static TrackEventConsoleListener()
        {
            _leftPad = GetLogLevelString(TelemetryLevel.Verbose).Length;
            _leftPadString = new string(' ', _leftPad);
        }

        public TrackEventConsoleListener(bool dissableColor)
        {
            DisableColors = dissableColor;
        }

        public bool DisableColors { get; set; }

        public void Post(EventData eventData)
        {
            lock (_lock)
            {
                _logBuilder = _logBuilder ?? new StringBuilder();
                var logLevelColors = default(ConsoleColors);
                var logLevelString = string.Empty;

                logLevelColors = GetLogLevelConsoleColors(eventData.TelemetryLevel);
                logLevelString = GetLogLevelString(eventData.TelemetryLevel);

                // category and event id
                _logBuilder.Append(logLevelString);
                _logBuilder.Append(eventData.EventSourceName);
                _logBuilder.Append("[");
                _logBuilder.Append(eventData.EventName);
                _logBuilder.AppendLine("]");

                if (!string.IsNullOrEmpty(eventData.Message))
                {
                    // message
                    _logBuilder.Append(_leftPadString);
                    _logBuilder.Append(eventData.Message);
                }

                ConsoleColor saveForegroundColor = Console.ForegroundColor;
                ConsoleColor saveBackgroundColor = Console.BackgroundColor;

                if( logLevelColors.Foreground != null)
                {
                    Console.ForegroundColor = (ConsoleColor)logLevelColors.Foreground;
                }

                if (logLevelColors.Background != null)
                {
                    Console.BackgroundColor = (ConsoleColor)logLevelColors.Background;
                }

                Console.WriteLine(_logBuilder.ToString());

                Console.ForegroundColor = saveForegroundColor;
                Console.BackgroundColor = saveBackgroundColor;

                _logBuilder.Clear();
                if (_logBuilder.Capacity > 1024)
                {
                    _logBuilder.Capacity = 1024;
                }
            }
        }

        private static string GetLogLevelString(TelemetryLevel logLevel)
        {
            switch (logLevel)
            {
                case TelemetryLevel.Metric:
                    return "metric:   ";
                case TelemetryLevel.Verbose:
                    return "verbose:  ";
                case TelemetryLevel.Informational:
                    return "info:     ";
                case TelemetryLevel.Warning:
                    return "warning:  ";
                case TelemetryLevel.Error:
                    return "error:    ";
                case TelemetryLevel.Critical:
                    return "critical: ";

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private ConsoleColors GetLogLevelConsoleColors(TelemetryLevel logLevel)
        {
            if (DisableColors)
            {
                return new ConsoleColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case TelemetryLevel.Critical:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);
                case TelemetryLevel.Error:
                    return new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red);
                case TelemetryLevel.Warning:
                    return new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black);
                case TelemetryLevel.Informational:
                    return new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
                case TelemetryLevel.Verbose:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                case TelemetryLevel.Metric:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);

                default:
                    return new ConsoleColors(DefaultConsoleColor, DefaultConsoleColor);
            }
        }

        private readonly struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }

    }
}
