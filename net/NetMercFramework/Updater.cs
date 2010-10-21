using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace it.mintlab.desktopnet.mercframework
{
    public class Updater
    {
        public const string UPDATE_MODE_FILELIST = "file-list";
        public const string UPDATE_MODE_RESTJSON = "rest-json";

        private bool centerFound = false;
        private Dictionary<string, string> mercAssemblyMap = new Dictionary<string, string>();

        public void setTextList(Uri address)
        {
            if (pingTest(address.Host))
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(address);
                wr.Timeout = 10000;
                WebResponse resp = wr.GetResponse();
                Stream stream = resp.GetResponseStream();
                mercAssemblyMap = new Dictionary<string, string>();

                using (StreamReader sr = new StreamReader(stream))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] data = line.Split(' ');
                        if (!mercAssemblyMap.ContainsKey(data[0]))
                            mercAssemblyMap.Add(data[0], data[1] + " " + data[2]);
                    }
                }
                centerFound = true;
            }
        }

        public void setRestJson(Uri address)
        { 
        
        }

        public bool updateMerc(String mercClassName, Version version)
        { 
            if(centerFound)
                if (mercAssemblyMap.ContainsKey(mercClassName))
                {
                    WebClient updaterClient = new WebClient();
                    String[] dllAndVersion = mercAssemblyMap[mercClassName].Split(' ');
                    Version newVersion = new Version(dllAndVersion[1]);
                    if (version.CompareTo(newVersion) < 0)
                    {
                        Uri remoteFile = new Uri(dllAndVersion[0]);
                        updaterClient.DownloadFile(remoteFile, remoteFile.Segments[remoteFile.Segments.Length-1]);
                        return true;
                    }
                }
            return false;
        }

        private bool pingTest(String ip)
        {
            // ping test
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;
            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120; //ms
            PingReply reply = pingSender.Send(ip, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
