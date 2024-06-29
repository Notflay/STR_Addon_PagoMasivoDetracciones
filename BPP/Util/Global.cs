using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public static class Global
    {
        public static void WriteToFile(string Message)
        {
			try
			{
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logs\\Service_Creation_Log_{DateTime.Now.Date.ToShortDateString().Replace('/', '_')}.txt";
                if (!File.Exists(filepath))
                {
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + " - " + Message);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(DateTime.Now.ToString() + " - " + Message);
                    }
                }
            }
			catch (Exception)
			{

			}
        }
    }
}
