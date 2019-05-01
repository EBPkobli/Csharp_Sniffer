using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace PacketAlma_Snif_
{
    class Program
    {
        private static Socket Socketm;
        private static byte[] receiveBuffer = new byte[256];
        private static EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static bool _datak;
        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Console.WriteLine("Datayı okuyayım mı? (Evet/Hayır) ");
            string cevap = Console.ReadLine();
            if (cevap.ToUpper() == "EVET")
                _datak = true;
            else
                _datak = false;
            
           
            CreateIcmpSocket();
            while (true) { Thread.Sleep(10); }
        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            
        }
        static byte[] _bBuffer = new byte[8192];
        static decimal _decPackagesReceived;
        static byte[] _bIn = new byte[4] { 1, 0, 0, 0 };
        static byte[] _bOut = new byte[4];
        private static void CreateIcmpSocket()
        {
            xxx:
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Ip adresini giriniz...");
            Console.ForegroundColor = ConsoleColor.Blue;
            string _ipcevap = Console.ReadLine();
            Socketm = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            try
            {
                Socketm.Bind(new IPEndPoint(IPAddress.Parse(_ipcevap), 0));
            }
            catch {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Ip adresini yanlış girdiniz veya okunması engelli bir ip adresi tekrar deneyin!");
                goto xxx;
            }
            Socketm.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
            Socketm.IOControl(IOControlCode.ReceiveAll, _bIn, _bOut);
            while (true)
            {
                byte[] _okunanhersey = new byte[10024];

                
                int size = Socketm.ReceiveBufferSize;
                int bytesReceived = Socketm.Receive(_bBuffer, 0, _bBuffer.Length, SocketFlags.None);

                if (bytesReceived > 0)
                {
                    _decPackagesReceived++;
                    ConvertReceivedData(_bBuffer, bytesReceived);
                }
            }
           
        }
       static StreamWriter _wtdata = new StreamWriter("c:\\tcplog.txt",true);
       static StreamWriter _wudata = new StreamWriter("c:\\udplog.txt",true);
       static StreamWriter _wadata = new StreamWriter("c:\\adresler.txt",true);
       public static string ByteArrayToString(byte[] ba)
       {
           string hex = BitConverter.ToString(ba);
           hex = hex.Replace("0", "");
           return hex.Replace("-", "");
       }
        static public void ConvertReceivedData(byte[] buffer, int iReceived)
        {
            
            if (buffer.Length > 0 && iReceived != 0)
            {
                //getting IP header and data information
                PacketIP ipPacket = new PacketIP(buffer, iReceived);

                // this string used as a key in the buffer
                string strKey = _decPackagesReceived.ToString();   // Guid.NewGuid().ToString();

                //searching which uperlevel protocol contain IP packet
                System.Threading.Thread.Sleep(75);

                string _ipsource = ipPacket.SourceAddress.ToString();
                string _ipdest = ipPacket.DestinationAddress.ToString();

               
                        switch (ipPacket.Protocol)
                        {
                            case "TCP":
                                {
                                    

                                    // Console.WriteLine("------------------------------");
                                    Console.WriteLine(" ");
                                    //if IP contains TCP creating new TCPData object
                                    //and assigning all TCP fields
                                    PacketTCP tcpPacket = new PacketTCP(ipPacket.Data, ipPacket.MessageLength);
                                    if (tcpPacket.DestinationPort != "80" & tcpPacket.SourcePort !="80")
                                    {
                                        //creating new PacketInfo object to fill the buffer
                                        PacketInfo pkgInfo = new PacketInfo(ipPacket, tcpPacket);

                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.Write(ipPacket.SourceAddress.ToString() + ":");
                                        Console.Write(tcpPacket.SourcePort.ToString());
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        Console.Write(" -> ");
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write(ipPacket.DestinationAddress.ToString() + ":");
                                        Console.Write(tcpPacket.DestinationPort + " Protocol: ");
                                        Console.Write(ipPacket.Protocol + " Boyut: ");

                                        Console.Write(ipPacket.TotalLength + " ");
                                        if (_datak)
                                        {
                                            string yazi = BitConverter.ToString(tcpPacket.Data);

                                            yazi = yazi.Replace("-", "");
                                            yazi = yazi.Replace("0", "");
                                            Console.WriteLine(" ");
                                            Console.WriteLine(yazi);
                                            _wtdata.WriteLine("\r\n" + yazi);
                                            _wtdata.Flush();

                                        }

                                        Console.WriteLine(" ");
                                        Console.WriteLine("------------------------------");
                                    }
                                }
                                break;
                            case "UDP":
                                {
                                    Console.WriteLine("------------------------------");
                                    Console.WriteLine(" ");
                                    PacketUdp udpPacket = new PacketUdp(ipPacket.Data, ipPacket.MessageLength);
                                    PacketInfo pkgInfo = new PacketInfo(ipPacket, udpPacket);
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(ipPacket.SourceAddress.ToString() + ":");
                                    Console.Write(udpPacket.SourcePort.ToString());
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.Write(" -> ");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(ipPacket.DestinationAddress.ToString() + ":");
                                    Console.Write(udpPacket.DestinationPort + " Protocol: ");
                                    Console.Write(ipPacket.Protocol + " Boyut: ");
                                    Console.Write(ipPacket.TotalLength + " ");
                                    Console.WriteLine(" ");
                                    if (_datak)
                                    {

                                        string yazi = Encoding.UTF8.GetString(udpPacket.Data);
                                        yazi = yazi.Replace("\0", "");
                                        Console.WriteLine(" ");
                                        Console.WriteLine(yazi);
                                        _wudata.WriteLine(DateTime.Now.ToString() + ":" + "\r\n" + yazi);
                                        _wudata.Flush();

                                    }
                                    Console.WriteLine("------------------------------");


                                } break;
                            case "ICMP":
                                {
                                    PacketIcmp icmpPacket = new PacketIcmp(ipPacket.Data, ipPacket.MessageLength);
                                    PacketInfo pkgInfo = new PacketInfo(ipPacket, icmpPacket);
                                    Console.WriteLine("------------------------------");
                                    Console.WriteLine(" ");

                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(ipPacket.SourceAddress.ToString() + ":");

                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.Write(" -> ");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(ipPacket.DestinationAddress.ToString() + ":");

                                    Console.Write(ipPacket.Protocol + " Boyut: ");
                                    Console.Write(ipPacket.TotalLength + " ");
                                    Console.WriteLine(" ");
                                    Console.WriteLine("------------------------------");

                                } break;

                            case "IGMP":
                                {
                                    PacketIgmp igmpPacket = new PacketIgmp(ipPacket.Data, ipPacket.MessageLength);
                                    PacketInfo pkgInfo = new PacketInfo(ipPacket, igmpPacket);
                                    Console.WriteLine("------------------------------");
                                    Console.WriteLine(" ");

                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(ipPacket.SourceAddress.ToString() + ":");

                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    Console.Write(" -> ");
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write(ipPacket.DestinationAddress.ToString() + ":");

                                    Console.Write(ipPacket.Protocol + " Boyut: ");
                                    Console.Write(ipPacket.TotalLength + " ");
                                    Console.WriteLine(" ");
                                    Console.WriteLine("------------------------------");
                                } break;
                        }
                    }
            }
         
        }
}
