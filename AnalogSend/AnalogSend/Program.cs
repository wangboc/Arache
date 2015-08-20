using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AnalogSend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ElectricityOriginalData data = new ElectricityOriginalData();
            int i = 1;
            data.UA = 224;
            data.UB = 224;
            data.UC = 224;
            data.UAB = 385;
            data.UBC = 386;
            data.UCA = 387;
            data.IA = 2;
            data.IB = 3;
            data.IC = 4;
            data.PA = 400;
            data.PB = 402;
            data.PC = 401;
            data.PS = 1200;
            data.QA = 400;
            data.QB = 400;
            data.QC = 400;
            data.QS = 1200;
            data.SA = 300;
            data.SB = 300;
            data.SC = 300;
            data.SS = 900;
            data.PFA = 500;
            data.PFB = 500;
            data.PFC = 500;
            data.PFS = 1500;
            data.FR = 50;
            data.WPP = 500;
            data.WPN = 500;
            data.WQN = 500;
            data.WQP = 500;

            while (true)
            {
                TcpClient tcpClient = new TcpClient();
                try
                {
                    tcpClient.Connect(new IPEndPoint(new IPAddress(new byte[] {121, 41, 109, 137}), 50000));
                    while (tcpClient.Connected)
                    {
                        while (true)
                        {
                            Console.WriteLine("已连接主机");
                            i++;
                            NetworkStream stream = tcpClient.GetStream();
                            string message = "[,T,1,12345,1," +
                                             data.UA + "," +
                                             data.UB + "," +
                                             data.UC + "," +
                                             data.UAB + "," +
                                             data.UBC + "," +
                                             data.UCA + "," +
                                             data.IA + "," +
                                             data.IB + "," +
                                             data.IC + "," +
                                             data.PA + "," +
                                             data.PB + "," +
                                             data.PC + "," +
                                             data.PS + "," +
                                             data.QA + "," +
                                             data.QB + "," +
                                             data.QC + "," +
                                             data.SA + "," +
                                             data.SB + "," +
                                             data.SC + "," +
                                             data.SS + "," +
                                             data.PFA + "," +
                                             data.PFB + "," +
                                             data.PFC + "," +
                                             data.PFS + "," +
                                             data.FR + "," +
                                             (data.WPP + i) + "," +
                                             (data.WPN + i) + "," +
                                             (data.WQP + i) + "," +
                                             (data.WQN + i) + "," +
                                             "FF,]";

                            byte[] dataBytes = Encoding.ASCII.GetBytes(message);
                            IAsyncResult result = stream.BeginWrite(dataBytes, 0, dataBytes.Length,
                                new AsyncCallback(SendCallback), stream); //异步发送数据
                            while (result.IsCompleted)
                                Console.WriteLine("成功发送：" + message);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("连接失败");
                }
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
        }
    }
}