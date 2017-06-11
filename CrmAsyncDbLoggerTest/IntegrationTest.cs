using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrmAsyncDbLogger;

namespace CrmAsyncDbLoggerTest
{
    [TestClass]
    public class IntegrationTest
    {
        private const string DB_CONNECTION_STRING = "Data Source=localhost;Initial Catalog=CrmAsyncDbLogger;Integrated Security=True";
        /// <summary>
        /// Before run the integration test please create the database and objects using the script CrmAsyncDbLogger\DatabaseObjects.sql
        /// and fix the DB_CONNECTION_STRING member
        /// </summary>
        [TestMethod]
        public void Main_01()
        {
            string json = "{\"name\":\"Jose\"}";
            Logger.Init(DB_CONNECTION_STRING, true);
            Logger.Info("Info 1");
            Logger.InfoFormat("Info {0}", 2);
            Logger.Info("Info with data", json);
            Logger.Debug("Debug 1");
            Logger.InfoFormat("Debug {0}", 2);
            Logger.Debug("Debug with data", json);
            Logger.Warn("Warning 1");
            Logger.WarnFormat("Warning {0}", 2);
            Logger.Warn("Warning with data", json);
            try
            {
                if (true)
                    throw new Exception("Exception Simulation");
            }
            catch (Exception ex)
            {
                Logger.Error("Logging an exception", ex);
                Logger.Error("Logging an exception",  ex, json);
            }
        }
    }
}
