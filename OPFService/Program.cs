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
using System.ServiceProcess;
using System.IO;
using System.Configuration;

namespace OPFService {
    class OPFService : ServiceBase {
        Thread worker;

        public OPFService() {
        }

        static void Main(string[] args) {
#if (!DEBUG)

            try {
            ServiceBase.Run(new OPFService());
            } catch (Exception e) {
                Logger logger = new Logger();
                logger.logException(e);
            }            
#else
            OPFService opfService = new OPFService();
            opfService.OnStart(args);
#endif
        }

        protected override void OnStart(string[] args) {
            base.OnStart(args);
            //Boolean useDatabaseCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["OPFDatabaseCheck"]);

            OPFDictionary d;
            NetworkService svc;

            //if (useDatabaseCheck) {
            //    svc = new NetworkService();
            //} else {
                d = new OPFDictionary(AppDomain.CurrentDomain.BaseDirectory + "\\opfmatch.txt", AppDomain.CurrentDomain.BaseDirectory + "opfcont.txt");
                svc = new NetworkService(d);
            //}

            worker = new Thread(() => svc.main());
            worker.Start();
        }

        protected override void OnShutdown() {
            base.OnShutdown();
            worker.Abort();
        }

        private void InitializeComponent()
        {
            // 
            // OPFService
            // 
            this.ServiceName = "OPF";

        }
    }
}
