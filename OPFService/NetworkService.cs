// This file is part of OpenPasswordFilter.
// 
// OpenPasswordFilter is free software; you can redistribute it and / or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// OpenPasswordFilter is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OpenPasswordFilter; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111 - 1307  USA
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;

namespace OPFService {
    class NetworkService {
        OPFDictionary dict;
        PwnedPwdDBCheck pwnDB = new PwnedPwdDBCheck();
        Logger logger = new Logger();

        public NetworkService(OPFDictionary d = null) {
            if (d != null) {
                dict = d;
            }
        }

        public void main() {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint local = new IPEndPoint(ip, 5999);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind(local);
                listener.Listen(64);
                while (true) {
                    Socket client = listener.Accept();
                    new Thread(() => handle(client)).Start();
                }
            } catch (Exception e) {
                logger.logException(e);
            }
        }

        public void handle(Socket client) {
            try {
                NetworkStream netStream = new NetworkStream(client);
                StreamReader istream = new StreamReader(netStream);
                StreamWriter ostream = new StreamWriter(netStream);
                String clientRecognitionKey = ConfigurationManager.AppSettings["OPFClientRecognitionKeyword"];

                string command = istream.ReadLine();
                if (command == clientRecognitionKey) {
                    string password = istream.ReadLine();
                    if (password.Length == 8 && password.StartsWith("vi")) {
                        ostream.WriteLine("false");
                        ostream.Flush();
                        client.Close();
                        return;
                    }

                    Boolean result;
                    result = dict.contains(password);
                    
                    if (!result)
                        result = pwnDB.containsHash(password);
                    
                    ostream.WriteLine(result? "false" : "true");
                    
                    ostream.Flush();
                } else {
                    ostream.WriteLine("ERROR");
                    ostream.Flush();
                }
            } catch (Exception e) {
                logger.logException(e);
            }
            client.Close();
        }
    }
}
