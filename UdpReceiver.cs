using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace Analog_Bacnet_lift
{
    class UdpReceiver
    {
        public static EndPoint f_ip;
        public void setserver()
        {
            byte[] buf = new byte[1024];
            byte[] data;
            int recv;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 50);//ip地址
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//建立新的socket
            newsock.Bind(ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender); //获得客户机地址
            while(true)
            {    
                data = new byte[1024];
                recv = newsock.ReceiveFrom(data, ref Remote);
                f_ip = Remote;
                buf = data;
                BACNET_ADDRESS src=new BACNET_ADDRESS();
                src.bacnet_ip =(IPEndPoint)(Remote);
                 
                handler(ref src, ref buf, (ushort)recv);
                

            }
            
        }
       public void handler(ref BACNET_ADDRESS src,ref byte[] pdu,UInt16 pdu_len)
        {

            int apdu_offset = 0;//网络层数据的长度 
            BACNET_ADDRESS dest = new BACNET_ADDRESS();
            BACNET_NPDU_DATA npdu_data = new BACNET_NPDU_DATA();
            BvlcProcessor b_pro = new BvlcProcessor();
            NpduProcessor p_pro = new NpduProcessor();
            ApduProcessor a_pro =new ApduProcessor();
            b_pro.Decode(ref src, ref pdu);
            if (pdu[0] == 1) //1是版本号 
            {
                apdu_offset = p_pro.Decode(ref pdu, ref dest, ref src, ref npdu_data);//获得npdu的数据        //获得npdu的数据 

            }
            if (npdu_data.network_layer_message)
            {
                //尚未定义 
            }

            else
            {
                if ((dest.net == 0))



                    a_pro.Decode(ref src, ref  pdu, (UInt16)(pdu_len - apdu_offset), apdu_offset);
            }
        }
        }
    }

