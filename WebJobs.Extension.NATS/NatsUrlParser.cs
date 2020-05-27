using System;
using System.Text.RegularExpressions;

namespace WebJobs.Extension.Nats
{
    /// <summary>
    /// NatsUrlParser, a utility class to parse the Nats Connection String.
    /// The connection stirng is in the form of:
    /// nats://<username>:<password>@<host>:<port>
    /// </summary>
    /// <example>
    /// <code>
    /// nats://user:pwd@localhost:4222
    /// </code>
    /// </example>
    public class NatsUrlParser
    {
        /// <summary>
        /// Connection paramter class
        /// </summary>
        public class ConnectionParams
        {
            /// <summary>
            /// Host string
            /// </summary>
            public readonly string host;
            /// <summary>
            /// Port
            /// </summary>
            public readonly int port;
            /// <summary>
            /// Username
            /// </summary>
            public readonly string username;
            /// <summary>
            /// Password
            /// </summary>
            public readonly string password;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="host">Host</param>
            /// <param name="port">Port</param>
            /// <param name="username">Username</param>
            /// <param name="password">Password</param>
            public ConnectionParams(string host, int port, string username, string password)
            {
                this.host = host;
                this.port = port;
                this.username = username;
                this.password = password;
            }

            /// <summary>
            /// HasCredentials
            /// </summary>
            /// <returns>True if the connection string has credentials,
            /// otherwise calse
            /// </returns>
            public bool HasCredentials()
            {
                return (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password));
            }
        }

        /// <summary>
        /// Server regex pattern
        /// </summary>
        private const string serverPattern = @"(?<host>[^:]*):?(?<port>\d*)";
        /// <summary>
        /// Rest of the connection string regex
        /// </summary>
        private const string pattern = @"^(?<scheme>nats)://" + @"((?<username>[^:@/]+)(:(?<password>[^:@/]*))?@)?" + serverPattern + "$";
        /// <summary>
        /// Default port
        /// </summary>
        public const int DefaultPort = 4222;

        /// <summary>
        /// Parse connection string and return ConnectionParams instance
        /// </summary>
        /// <param name="connstring">Connection string</param>
        /// <returns>Retruns the ConnectionParams instance with parsed values</returns>
        public static ConnectionParams Parse(string connstring)
        {
            var matches = Regex.Match(connstring, pattern);
            var hoststring = matches.Groups["host"].ToString();
            var port = matches.Groups["port"].ToString();
            var username = matches.Groups["username"].ToString();
            var password = matches.Groups["password"].ToString();

            if (String.IsNullOrEmpty(port))
            {
                port = DefaultPort.ToString();
            }

            return new ConnectionParams(hoststring, int.Parse(port), username, password);
        }
    }
}
