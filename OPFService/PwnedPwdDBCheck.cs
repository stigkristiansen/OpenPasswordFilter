using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace OPFService
{
    class PwnedPwdDBCheck
    {

        private SqlConnection connection;
        private Logger logger = new Logger();

        public Boolean containsHash(string hash) {

            // if there is no sql server return false (no password hash found) so users can change password at all
            if (!InitConnection()) return false;

            try {

                String tableName = ConfigurationManager.AppSettings["OPFDatabaseTableName"];
                SqlCommand command = new SqlCommand("Select hash from " + tableName + " where hash='" + Hash(hash) + "'", connection);

                using (SqlDataReader reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        CloseConnection();
                        return true;
                    } else {
                        CloseConnection();
                        return false;
                    }
                }

            } catch (Exception e) {
                logger.logException(e);
                CloseConnection();
                return true;
            }

        }

        private Boolean InitConnection() {

            connection = new SqlConnection();
            Boolean useTrustedConnection = Convert.ToBoolean(ConfigurationManager.AppSettings["OPFUseTrustedConnection"]);
            String serverName = ConfigurationManager.AppSettings["OPFDatabaseServer"];
            String databaseName = ConfigurationManager.AppSettings["OPFDatabaseName"];

            if (useTrustedConnection) {

                connection.ConnectionString = "Server=" + serverName + "; Database=" + databaseName + "; Integrated Security=SSPI;";

            } else {

                String username = ConfigurationManager.AppSettings["OPFDatabaseUserName"];
                String password = ConfigurationManager.AppSettings["OPFDatabasePassword"];

                connection.ConnectionString = "Server=" + serverName + "; Database=" + databaseName + "; User id=" + username + "; Password=" + password + " ;";

            }

            try {
                connection.Open();
                return true;
            } catch (Exception e) {
                logger.logException(e);
                return false;
            }

        }

        private void CloseConnection() {

            try {
                connection.Close();
            } catch (Exception e) {
                logger.logException(e);
            }
            connection.Dispose();

        }

        static string Hash(string input) {
            using (SHA1Managed sha1 = new SHA1Managed()) {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash) {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
