using System.Diagnostics;
using System.Net.NetworkInformation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
          await endovpn();
           // Manager mn = new Manager( "us.ovpn", "freeopenvpn", "026928613");
            Thread serialWriteThread = new Thread(() => Manager("us.ovpn", "freeopenvpn", "598763451"));
             serialWriteThread.Start();


            // mn.GetStatus();
        }

        private  string openVpnExePath;
        private string ovpnFileName;
        private string passFileName;

        public void RunOpenVpnProcess(string fileName, string arguments)
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

                    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    int i = 0;
                    foreach (NetworkInterface adapter in networkInterfaces)
                    {
                        string adname = adapter.Description.Trim().ToLower();
                        if (adapter.Description.Trim().ToLower().StartsWith("tap-windows"))
                        {

                            break;
                            
                        }
                        i++;
                        //Console.WriteLine($"Description: {adapter.Description}");
                        //Console.WriteLine($"Type: {adapter.NetworkInterfaceType}");
                        //Console.WriteLine($"Status: {adapter.OperationalStatus}");
                        //Console.WriteLine($"Is Connected: {adapter.OperationalStatus == OperationalStatus.Up}");
                        //Console.WriteLine();
                    }
                    
                    while (networkInterfaces.ElementAt(i).OperationalStatus != OperationalStatus.Up)
                    {
                        networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    }
                    if (networkInterfaces.ElementAt(i).OperationalStatus == OperationalStatus.Up)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            textBox1.Text = "port up";
                        }));
                    }
                   


                    using (StreamReader reader = process.StandardOutput)
                    {
                        string stderr = process.StandardError.ReadToEnd(); // Error(s)!!
                        string result = reader.ReadToEnd(); // What we want.
                        BeginInvoke(new Action(() =>
                        {
                            textBox1.Text=result;
                        }));
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public string Manager(string ovpnFileName, string userName = null, string password = null, string openVpnExeFileName = @"D:\\openvpn.exe")
        {
            openVpnExePath = Path.Combine(Directory.GetCurrentDirectory() + "\\ovpn", "openvpn.exe");
            if (!string.IsNullOrEmpty(ovpnFileName))
            {

                ovpnFileName = Path.Combine(Directory.GetCurrentDirectory(), ovpnFileName);
                passFileName = Path.Combine(Directory.GetCurrentDirectory(), "ovpnpass.txt").Replace(@"\", @"\\");
                File.WriteAllLines(passFileName, new string[] { userName, password });

               // string openvpnCommand = $"\"\"{openVpnExePath}\" --config \"{ovpnFileName}\"";
                string openvpnCommand = $"\"\"{openVpnExePath}\" --config \"{ovpnFileName}\" --auth-user-pass \"{passFileName}\"";

                RunOpenVpnProcess("cmd.exe", $"/c {openvpnCommand} ");
                return "";
            }
            return "";

        }



        private async  Task endovpn()
        {
            try {

                Process process1 = new Process();
                ProcessStartInfo startInfo1 = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/c taskkill.exe /F /IM openvpn.exe",
                    // Verb = "runas",
                    UseShellExecute = true
                };
                process1.StartInfo = startInfo1;
                process1.Start();
                process1.WaitForExit();
            } catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process process1 = new Process();
            ProcessStartInfo startInfo1 = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = $"/k taskkill.exe /F /IM openvpn.exe",
               // Verb = "runas",
                UseShellExecute = true
            };
            process1.StartInfo = startInfo1;
            process1.Start();
        }
    }
}
