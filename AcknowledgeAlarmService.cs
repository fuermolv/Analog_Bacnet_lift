using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analog_Bacnet_lift
{
    class AcknowledgeAlarmService
    {

        public int AcknowledgeAlarmService_Pack(ref byte[] Handler_Transmit_Buffer,BACNET_ADDRESS dest, ref  ACKNOWLEDGE_ALARM_DATA Ack_data)
        {
            BACNET_ADDRESS my_address=new BACNET_ADDRESS();
            BacnetAddresssProcessor.Get_My_Address(ref my_address);
            int byte_len = 0;
            Byte invoke_id;
            int len = 0;
            int pdu_len = 0;
            NpduProcessor n_pro = new NpduProcessor();
            BACNET_NPDU_DATA npdu_data = new BACNET_NPDU_DATA();
            BvlcProcessor b_pro = new BvlcProcessor();
            //invoke_id=get_invoke_id()  获取invoke_id未写 假设为1
            invoke_id = 1;
            n_pro.Encode_NpduData(ref npdu_data, true, BACNET_MESSAGE_PRIORITY.MESSAGE_PRIORITY_NORMAL);

            pdu_len = n_pro.Encode(ref Handler_Transmit_Buffer, ref dest, ref my_address, ref npdu_data);
            len = Acknowledge_Alarm_Encode(ref Handler_Transmit_Buffer,invoke_id, ref Ack_data, pdu_len);
            pdu_len += len;
            byte_len = b_pro.Encode(ref Handler_Transmit_Buffer, ref dest, ref npdu_data, pdu_len);

            return invoke_id; ;
            

        }
        private int Acknowledge_Alarm_Encode(ref Byte[] apdu,Byte invoke_id,ref ACKNOWLEDGE_ALARM_DATA Ack_data,int pos )
        {
            int len = 0;        /* length of each encoding */
            int apdu_len = 0;
            apdu[pos + 0] = (byte)BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST;
            apdu[pos + 1] = BasicalProcessor.Encode_MaxSegsandApdu(0, 1476);
            apdu[pos + 2] = invoke_id;
            apdu[pos + 3] = (byte)BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_ACKNOWLEDGE_ALARM;
            apdu_len = 4;

            /* tag 0 - processIdentifier */
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 0, Ack_data.ProcessIdentifier, pos + apdu_len);
            apdu_len += len;
            /* tag 1 - eventObjectIdentifier */
            len = BasicalProcessor.Encode_Context_ObjectId(ref apdu, 1, (int)Ack_data.EventIdentifier.type, Ack_data.EventIdentifier.instance, pos + apdu_len);
            apdu_len += len;
            /* tag 2 - toState */
            len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 2, (uint)Ack_data.StateAcknowledged, apdu_len + pos);
            apdu_len += len;
            /* tag 3 - timeStamp */
            len = BasicalProcessor.Encode_Context_Timestamp(ref apdu, 3, ref Ack_data.TimeStamp, pos + apdu_len);
            apdu_len += len;
            /* tag 4 - Acknowledgment Source */

            len = BasicalProcessor.Encode_Context_Character_String(ref apdu, 4, ref Ack_data.Source, pos + apdu_len);
            apdu_len += len;
            /* tag 5- timeStamp */
            len = BasicalProcessor.Encode_Context_Timestamp(ref apdu, 5, ref Ack_data.TimeOfAcknowledgment, pos + apdu_len);
            apdu_len += len;
            return apdu_len;


        }
       public void Acknowledge_Alarm_Handler(ref Byte[] request,UInt16 request_len,ref BACNET_ADDRESS src,ref BACNET_CONFIRMED_SERVICE_DATA request_data)
        {  
            ACKNOWLEDGE_ALARM_DATA Ack_data = new ACKNOWLEDGE_ALARM_DATA();
            Byte[] buffer = new Byte[1024];
            BACNET_ADDRESS my_address = new BACNET_ADDRESS();
            NpduProcessor n_pro = new NpduProcessor();
            BACNET_NPDU_DATA npdu_data = new BACNET_NPDU_DATA();
            ApduProcessor a_pro = new ApduProcessor();
            BvlcProcessor b_pro = new BvlcProcessor();
            int pdu_len = 0;
            int byte_len = 0;
            int apdu_len = -1;
            int npdu_len = -1;
            int len = 0;
            len = Decode_Service_Request(ref request, request_len, ref Ack_data);
         

            n_pro.Encode_NpduData(ref npdu_data, false, BACNET_MESSAGE_PRIORITY.MESSAGE_PRIORITY_NORMAL);
            npdu_len = n_pro.Encode(ref buffer, ref src, ref my_address, ref npdu_data);
            apdu_len = a_pro.Encode_Simple_Ack(ref buffer, request_data.invoke_id, (Byte)BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_SUBSCRIBE_COV, npdu_len);
            pdu_len = npdu_len + apdu_len;
            byte_len = b_pro.Encode(ref buffer, ref src, ref npdu_data, pdu_len);
        }
        private int Decode_Service_Request(ref Byte[] apdu,uint request_len,ref  ACKNOWLEDGE_ALARM_DATA Ack_data)
       {
           int len = 0;
       
           int section_length = 0;
           UInt32 value=0;
         
           /* tag 0 - processIdentifier */
           section_length =BasicalProcessor.Decode_Context_Unsigned(ref apdu, 0, ref Ack_data.ProcessIdentifier, len);
           len += section_length;
           /* tag 1 - eventObjectIdentifier */
           section_length = BasicalProcessor.Decode_Object_Id(ref apdu, ref Ack_data.EventIdentifier.type, ref Ack_data.EventIdentifier.instance, len);
           len += section_length;
           /* tag2  EventStare */
           section_length = BasicalProcessor.Decode_Enumerated(ref apdu, 2, ref value, len);
           Ack_data .StateAcknowledged= (BACNET_EVENT_STATE)value;
           len += section_length;

           /* tag 3 - timeStamp */
           section_length = BasicalProcessor.Decode_Context_Timestamp(ref apdu, 3, ref Ack_data.TimeStamp, len);
           len += section_length;
           /* tag 4 Acknowledgment Source */
           section_length = BasicalProcessor.Decode_Context_Character_String(ref apdu, 4, ref Ack_data.Source, len);
           len += section_length;
            
           /* tag 5 - AcktimeStamp */
           section_length = BasicalProcessor.Decode_Context_Timestamp(ref apdu, 5, ref Ack_data.TimeOfAcknowledgment, len);
           len += section_length;

           return len;


       }
    }
}
