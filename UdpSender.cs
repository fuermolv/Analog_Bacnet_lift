using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Analog_Bacnet_lift
{
    class UdpSender
    {
        private Byte[] data;
        private IPEndPoint ipep;

        public UdpSender(ref Byte[] byte_send, IPEndPoint ip)
        {

            data = byte_send;
            ipep = ip;
        }
        public UdpSender(int length)
        {
           data = new Byte[length];
        }
        public void Send(int length)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            Byte[] data_send = new Byte[length];
            Array.Copy(data, 0, data_send, 0, length);
            //lient.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            client.SendTo(data_send, length, SocketFlags.None, ipep);
            client.Close();
        }
        public void SendBrocast(int length)
        {

            Byte[] data_send = new Byte[length];
            Array.Copy(data, 0, data_send, 0, length);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            client.SendTo(data, data.Length, SocketFlags.None, ipep);
            client.Close(); 
        }
        public void Send_ReadPropertyService(ref IPEndPoint dest)
        {
          /*  ReadPropertyService rp_service = new ReadPropertyService();
             rp_service.Pack_Read_Property_Request(ref data, 11);
            ipep = dest;
            Send();
           * */

        }
        public void Send_Unconfirmed_Cov_Notification(ref IPEndPoint dest,ref BACNET_COV_DATA cov_data)
        {
            UnconFirmCovService service = new UnconFirmCovService();
            service.Unconfirm_Cov_Service_Pack(ref data, ref cov_data);
            ipep = dest;
           // Send();

        }
        public void show()
        {
            Console.WriteLine(data);
        }

    }
}
