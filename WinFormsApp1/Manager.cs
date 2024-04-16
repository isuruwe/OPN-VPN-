using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class Manager : IDisposable
    {

        public enum Signal
        {
            Hup,
            Term,
            Usr1,
            Usr2
        }

        private Socket socket;
        private const int bufferSize = 1024;
        private string ovpnFileName;
        private string passFileName;
        private const string eventName = "MyOpenVpnEvent";
        private readonly Process prc = new Process();
        private readonly string openVpnExePath;

       
        static void RunOpenVpnProcess(string fileName, string arguments)
        {
           

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                //WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            FileName = fileName,
                Arguments = arguments,
               // Verb = "runas", // Run as administrator
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
        };

            try
            {
                
                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string stderr = process.StandardError.ReadToEnd(); // Error(s)!!
                        string result = reader.ReadToEnd(); // What we want.
                       
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public  Manager( string ovpnFileName, string userName = null, string password = null, string openVpnExeFileName = @"D:\\openvpn.exe")
        {
            this.openVpnExePath = Path.Combine(Directory.GetCurrentDirectory()+ "\\ovpn", "openvpn.exe");
            if (!string.IsNullOrEmpty(ovpnFileName))
            {
               
                this.ovpnFileName = Path.Combine(Directory.GetCurrentDirectory(), ovpnFileName);
                this.passFileName = Path.Combine(Directory.GetCurrentDirectory(), "ovpnpass.txt").Replace(@"\", @"\\");
                File.WriteAllLines(passFileName, new string[] { userName, password });
              
              
                string openvpnCommand = $"\"\"{openVpnExePath}\" --config \"{ovpnFileName}\" --auth-user-pass \"{passFileName}\"";

                RunOpenVpnProcess("cmd.exe", $"/c {openvpnCommand} ");
                
            }

            
        }

       

        public void Dispose()
        {
           
        }
    }
}
