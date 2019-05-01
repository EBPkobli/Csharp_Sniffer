using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAlma_Snif_
{
    class PacketInfo
    {
        PacketIP _ip;     // IP packet information
        PacketTCP _tcp;    // TCP header information
        PacketUdp _udp;    // UDP header information
        PacketIcmp _icmp;   // ICMP header information
        PacketIgmp _igmp;   // IGMP header informatiom

        /// <summary>
        /// 
        /// Some overloaded constructors 
        /// they may teke any of packets combination 
        /// ECSAMPLE: if IP packet contains TCP protocol and data using ->PacketInfo(IPData ip,TCPData tcp)
        /// 
        /// </summary>
        /// 
        public PacketInfo()
        {
        }
        public PacketInfo(PacketIP ip)
        {
            _ip = ip;
        }
        public PacketInfo(PacketIP ip, PacketTCP tcp)
        {
            _ip = ip;
            _tcp = tcp;
        }
        public PacketInfo(PacketIP ip, PacketUdp udp)
        {
            _ip = ip;
            _udp = udp;
        }
        public PacketInfo(PacketIP ip, PacketIcmp icmp)
        {
            _ip = ip;
            _icmp = icmp;
        }
        public PacketInfo(PacketIP ip, PacketIgmp igmp)
        {
            _ip = ip;
            _igmp = igmp;
        }

        public PacketIP IP
        {
            get { return _ip; }
        }
        public PacketTCP TCP
        {
            get { return _tcp; }
        }
        public PacketUdp UDP
        {
            get { return _udp; }
        }
        public PacketIcmp ICMP
        {
            get { return _icmp; }
        }
        public PacketIgmp IGMP
        {
            get { return _igmp; }
        }

    }
}
