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
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace OPFService {
    class OPFDictionary {
        List<string> matchlist;
        List<string> contlist;

        public OPFDictionary(string pathmatch, string pathcont) {
            string line;
            matchlist = new List<string>();
            int a = 0;
            StreamReader infilematch = null; ;
            try
            {
                infilematch = new StreamReader(pathmatch);

                while ((line = infilematch.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        a += 1;
                        matchlist.Add(line.ToLower());
                    }
                }

                infilematch.Close();

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Loaded " + a.ToString() + " line(s) from opfmatch.txt.", EventLogEntryType.Information, 101, 1);
                }
            }
            catch
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Failed to load opfmatch.txt.", EventLogEntryType.Error, 101, 1);
                }
            }

            contlist = new List<string>();
            a = 0;
            StreamReader infilecont = null;
            try
            {
                infilecont = new StreamReader(pathcont);

                while ((line = infilecont.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        a += 1;
                        contlist.Add(line.ToLower());
                    }
                }

                infilecont.Close();

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Loaded " + a.ToString() + " line(s) from opfcont.txt.", EventLogEntryType.Information, 101, 1);
                }
            }
            catch
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Failed to load opfcont.txt.", EventLogEntryType.Error, 101, 1);
                }
            }
        }

        public Boolean contains(string word) {
            foreach (string badstr in contlist)
            {
                if (word.ToLower().Contains(badstr))
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("Password contains poison string " + badstr +", case insensitive.", EventLogEntryType.Warning, 101, 1);
                    }
                    return true;
                }
            }
            if (matchlist.Contains(word))
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Password matched a string in the bad password list", EventLogEntryType.Warning, 101, 1);
                }
                return true;
            }
            else
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Password passed local lists.", EventLogEntryType.Information, 101, 1);
                }
                return false;
            }
        }
    }
}
