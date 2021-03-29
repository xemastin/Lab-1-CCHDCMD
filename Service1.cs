using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Lab_1___Cơ_chế_hoạt_động_của_mã_độc
{
    public partial class Service1 : ServiceBase
    {
        Timer time = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            time.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            time.Interval = 5000;
            time.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            int Hours = DateTime.Now.Hour;
            Process[] processes = Process.GetProcessesByName("notepad");
            if (Hours >= 12) //Nếu như từ 12h đến 24h thì sẽ kiểm tra để khởi động process
            {
                //Có process đang chạy thì không mở thêm process
                if (processes.Length > 0)
                {
                    WriteToFile("Process running");
                }
                else //Ngược lại thì sẽ khởi động process
                {
                    WriteToFile("Not found process : " + Hours);
                    ProcessStartInfo processStart = new ProcessStartInfo();
                    processStart.FileName = @"C:\Windows\System32\notepad.exe";
                    Process process = Process.Start(processStart);
                    WriteToFile("Process start : " + Hours);
                }
            }
            else //Thời gian 0h đến 12h sẽ không cho process khởi chạy
            {
                if (processes.Length > 0)
                {
                    WriteToFile("Process running : " + Hours);
                    foreach (var p in processes) p.Kill();
                    WriteToFile("Process killed : ");
                }
                else
                {
                    WriteToFile("Not found process at " + Hours);
                }
            }
        }



        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }

            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }


        }
    }
}

