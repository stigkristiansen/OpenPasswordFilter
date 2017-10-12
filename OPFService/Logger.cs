using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace OPFService
{
    class Logger
    {

        public void logException(Exception e) {

            Boolean loggingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["OPFLoggingEnabled"]);

            if (loggingEnabled) {
                writeErrorToLog(e);
            }

        }

        private void writeErrorToLog(Exception errorException) {

            String filePath = ConfigurationManager.AppSettings["OPFLogFilePath"];

            try {
                using (StreamWriter writer = new StreamWriter(filePath, true)) {
                    writer.WriteLine(
                        DateTime.Now.ToString() + ": " +
                        errorException.Message + Environment.NewLine +
                        "StackTrace :" + errorException.StackTrace +
                        Environment.NewLine);
                }

            } catch (Exception e) {

            }

        }

    }
}
