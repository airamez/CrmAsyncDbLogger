using System;
using NLog.Targets;
using NLog.Config;
using NLog;
using Microsoft.Xrm.Sdk;
using NLog.Targets.Wrappers;
using System.Diagnostics;
using System.Reflection;
using NLog.Layouts;

namespace CrmAsyncDbLogger
{
    /// <summary>
    /// This class offer Aync and Database logging capability.
    /// This is a singleton class and requires the CrmAsyncDbLogger.Init(dbConnectionString) method to be called before any other method
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// NLog logger
        /// </summary>
        private static NLog.Logger _Logger;

        /// <summary>
        /// Database string connection
        /// </summary>
        private static string _DatabaseConnectionString;

        /// <summary>
        /// Raise Exception indicator
        /// </summary>
        private static bool _RaiseException;

        public static string EVENT_VIEWER_SOURCE_NAME = "CrmAsyncDbLogger";

        /// <summary>
        /// Initialize the Logger
        /// </summary>
        /// <param name="service">MSD organization service</param>
        /// <param name="raiseException">The raise Exception should only be use for test/debug NEVER in PROD code</param>
        public static void Init(string dbConnectionString, bool raiseException = false)
        {
            try
            {
                _RaiseException = raiseException;
                if (_Logger == null)
                {
                    _DatabaseConnectionString = dbConnectionString;
                    InitLogger();
                }
                SetCorrelationID(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                if (_RaiseException)
                {
                    string msg = string.Format("Error Initializing Logger: Error = {0} \nStackTrace = {1}",
                                               ex.Message, ex.StackTrace);
                    WriteToEventLog(msg, EventLogEntryType.Error);
                    throw;
                }
            }
        }

        private static string GetLogDatabaseConnectionString(IOrganizationService service)
        {
            return null;
        }

        /// <summary>
        /// Set correlation ID
        /// </summary>
        /// <param name="correlationId"></param>
        public static void SetCorrelationID(Guid correlationId)
        {
            CheckLogger();
            Trace.CorrelationManager.ActivityId = correlationId;
        }

        /// <summary>
        /// Log Info with string formating (interpolation) parameters
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="messageParameters">message interpolation parameters</param>
        public static void InfoFormat(string message, params object[] messageParameters)
        {
            WriteLog(LogLevel.Info, message, null, null, null, null, null, null, messageParameters);
        }

        /// <summary>
        /// Log Info with data
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="data">Log data as JSON</param>
        public static void Info(string message, string data = null)
        {
            WriteLog(LogLevel.Info, message, data, null, null);
        }

        /// <summary>
        /// Log Debug with string formating (interpolation) parameters
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="messageParameters">message interpolation parameters</param>
        public static void DebugFormat(string message, params object[] messageParameters)
        {
            WriteLog(LogLevel.Debug, message, null, null, null, null, null, null, messageParameters);
        }

        /// <summary>
        /// Log Debug with data
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="data">Log data as JSON</param>
        public static void Debug(string message, string data = null)
        {
            WriteLog(LogLevel.Debug, message, data, null, null);
        }

        /// <summary>
        /// Log Waning with string formating (interpolation) parameters
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="messageParameters">message interpolation parameters</param>
        public static void WarnFormat(string message, params object[] messageParameters)
        {
            WriteLog(LogLevel.Warn, message, null, null, null, null, null, null, messageParameters);
        }

        /// <summary>
        /// Log Warning with data
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="data">Log data as JSON</param>
        public static void Warn(string message, string data = null)
        {
            WriteLog(LogLevel.Warn, message, data, null, null);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="message">Log error message</param>
        /// <param name="exception">Exception</param>
        /// <param name="data">Log error data</param>
        public static void Error(string message, Exception exception, string data = null)
        {
            WriteLog(LogLevel.Error, message, data, exception, null);
        }

        #region Private Methods

        /// <summary>
        /// Check if the class is initialized
        /// </summary>
        private static void CheckLogger()
        {
            if (_Logger == null)
            {
                throw new Exception("Logger.Init method has to be called prior to Log method");
            }
        }

        /// <summary>
        /// Write Log
        /// </summary>
        /// <param name="level">Level</param>
        /// <param name="message">Message</param>
        /// <param name="data">Data</param>
        /// <param name="exception">Exception</param>
        /// <param name="messageParameters">Message interpoloation parameters</param>
        private static void WriteLog(LogLevel level, string message, string data,
                                     Exception exception,
                                     params object[] messageParameters)
        {
            try
            {
                CheckLogger();
                LogEventInfo log = new LogEventInfo(level, _Logger.Name, null, message, messageParameters, exception);
                AddData(data, log);
                AddClassMethodNames(log);
                _Logger.Log(log);
            }
            catch (Exception ex)
            {
                if (_RaiseException)
                {
                    string msg = string.Format("Error logging: {0} \nStackTrace = {1}", ex.Message, ex.StackTrace);
                    WriteToEventLog(msg, EventLogEntryType.Error);
                    throw;
                }
            }
        }

        /// <summary>
        /// Add the caller Class and Method names to the log entry
        /// </summary>
        /// <param name="log">Log event info</param>
        private static void AddClassMethodNames(LogEventInfo log)
        {
            MethodBase method = null;
            try
            {
                var stacktrace = new StackTrace();
                method = stacktrace.GetFrame(3).GetMethod();
                log.Properties["Class"] = method.ReflectedType.FullName;
                log.Properties["Method"] = method.Name;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Add data to the log entry
        /// </summary>
        /// <param name="data">Log data</param>
        /// <param name="log">Log event info</param>
        private static void AddData(string data, LogEventInfo log)
        {
            if (!string.IsNullOrWhiteSpace(data))
                log.Properties["LogData"] = data;
        }

        /// <summary>
        /// Initialize the Logger
        /// </summary>
        private static void InitLogger()
        {
            var config = new LoggingConfiguration();
            var databaseTarget = CreateDatabaseTarget();
            SetupRaiseException(config, databaseTarget);
            LogManager.Configuration = config;
            LogManager.ThrowExceptions = _RaiseException;
            LogManager.ThrowConfigExceptions = _RaiseException;
            _Logger = LogManager.GetLogger("Logger");
        }

        /// <summary>
        /// Setup the Target based on the RaiseException attribute.
        /// If Raise Exception is true the logging will be Async so it is possible to catch exception otherwise it will be Sync
        /// The raise Exception should only be use for test/debug NEVER in PROD code
        /// </summary>
        /// <param name="config">Logging Configuration</param>
        /// <param name="databaseTarget">Database target</param>
        private static void SetupRaiseException(LoggingConfiguration config, DatabaseTarget databaseTarget)
        {
            /*
             * If raise exception is set the logging has to be sync
             */
            if (_RaiseException)
            {
                config.AddTarget(databaseTarget);
                var rule = new LoggingRule("*", LogLevel.Debug, databaseTarget);
                config.LoggingRules.Add(rule);
            }
            else
            {
                var AsyncTarget = new AsyncTargetWrapper("AsyncTarget", databaseTarget);
                config.AddTarget(AsyncTarget);
                var rule = new LoggingRule("*", LogLevel.Debug, AsyncTarget);
                config.LoggingRules.Add(rule);
            }
        }

        /// <summary>
        /// Create the NLog Database Target
        /// </summary>
        /// <returns>Database target</returns>
        private static DatabaseTarget CreateDatabaseTarget()
        {
            var target = new DatabaseTarget("Database");
            target.KeepConnection = true;
            target.DBProvider = "System.Data.SqlClient";
            target.ConnectionString = GetDabataseStringConnection();
            target.CommandText =
                "exec dbo.InsertLog @date, @level, @correlation_id, @machine_name, @user, @Class, @method, @message, @data, @exception";
            target.Parameters.Add(new DatabaseParameterInfo("@date", "${date:universalTime=true}"));
            target.Parameters.Add(new DatabaseParameterInfo("@level", "${level}"));
            target.Parameters.Add(new DatabaseParameterInfo("@correlation_id", "${activityid}"));
            target.Parameters.Add(new DatabaseParameterInfo("@machine_name", "${machinename}"));
            target.Parameters.Add(new DatabaseParameterInfo("@user", "${windows-identity:userName=true:domain=true}"));
            target.Parameters.Add(new DatabaseParameterInfo("@class", "${event-properties:Class}"));
            target.Parameters.Add(new DatabaseParameterInfo("@method", "${event-properties:Method}"));
            target.Parameters.Add(new DatabaseParameterInfo("@message", "${message}"));
            target.Parameters.Add(new DatabaseParameterInfo("@data", "${event-properties:LogData}"));
            target.Parameters.Add(new DatabaseParameterInfo("@exception", "${exception:format=toString,Data:maxInnerExceptionLevel=10}"));
            return target;
        }


        /// <summary>
        /// Get the database connection string
        /// </summary>
        /// <returns></returns>
        private static string GetDabataseStringConnection()
        {
            return _DatabaseConnectionString;
        }

        /// <summary>
        /// Write a log entry to Windows Event Viewer
        /// </summary>
        /// <param name="message"></param>
        /// <param name="entryType"></param>
        public static void WriteToEventLog(string message, EventLogEntryType entryType = EventLogEntryType.Information)
        {
            try
            {
                if (!EventLog.SourceExists(EVENT_VIEWER_SOURCE_NAME))
                {
                    EventLog.CreateEventSource(EVENT_VIEWER_SOURCE_NAME, "CrmAsyncDbLogger");
                }
                EventLog.WriteEntry(EVENT_VIEWER_SOURCE_NAME, message, entryType);
            }
            catch
            {
            }
        }
        #endregion
    }
}
