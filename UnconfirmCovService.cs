using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analog_Bacnet_lift
{
    public class UnconFirmCovService
    {
        public int Unconfirm_Cov_Service_Pack(ref byte[] Handler_Transmit_Buffer ,ref BACNET_COV_DATA cov_data)
        {   //无证实服务 广播
            int len = 0;
            int pdu_len = 0;
            int bytes_len = 0;
            BACNET_ADDRESS dest=new  BACNET_ADDRESS() ;
            BACNET_NPDU_DATA npdu_data=new BACNET_NPDU_DATA();
            BACNET_ADDRESS my_address=new BACNET_ADDRESS();
            BacnetAddresssProcessor.Get_My_Address(ref my_address);
            BacnetAddresssProcessor.Get_Broadcast_Address(ref dest,true ,60);
            NpduProcessor n_pro = new NpduProcessor();
            BvlcProcessor b_pro = new BvlcProcessor();

           
           // datalink_get_my_address(&my_address);
          //  datalink_get_broadcast_address(dest);
            n_pro.Encode_NpduData(ref npdu_data, true, BACNET_MESSAGE_PRIORITY.MESSAGE_PRIORITY_NORMAL);
            pdu_len = n_pro.Encode(ref Handler_Transmit_Buffer, ref dest, ref my_address, ref npdu_data);
            len = Unconfirm_Cov(ref Handler_Transmit_Buffer, cov_data, pdu_len);
            pdu_len += len;
            bytes_len = b_pro.Encode(ref Handler_Transmit_Buffer, ref dest, ref npdu_data, pdu_len);
            return bytes_len;
        }
        private int Unconfirm_Cov(ref Byte[] apdu,BACNET_COV_DATA cov_data,int pos)
        {
            int len = 0;        /* length of each encoding */
            int apdu_len = 0;   /* total length of the apdu, return value */


                apdu[pos+0] = (Byte)BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST;
                apdu[pos+1] =(Byte) BACNET_UNCONFIRMED_SERVICE.SERVICE_UNCONFIRMED_COV_NOTIFICATION; /* service choice */
                apdu_len = 2;
                len = Notify_Encode_Data(ref apdu, cov_data, pos + apdu_len);
                apdu_len += len;
           

            return apdu_len;
        }
        private int Notify_Encode_Data(ref Byte[] apdu,BACNET_COV_DATA cov_data,int pos)
        {
            int len = 0;        /* length of each encoding */
            int apdu_len = 0;
            BACNET_PROPERTY_VALUE value = null;        /* value in list */
            /* tag 0 - subscriberProcessIdentifier */
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 0, cov_data.subscriberProcessIdentifier, pos + apdu_len);
            apdu_len += len;
            /* tag 1 - initiatingDeviceIdentifier */
            len = BasicalProcessor.Encode_Context_ObjectId(ref apdu, 1, (int)BACNET_OBJECT_TYPE.OBJECT_DEVICE, cov_data.initiatingDeviceIdentifier, pos + apdu_len);
            apdu_len += len;
            /* tag 2 - monitoredObjectIdentifier */
            len = BasicalProcessor.Encode_Context_ObjectId(ref apdu, 2, (int)cov_data.monitoredObjectIdentifier.type, cov_data.monitoredObjectIdentifier.instance, pos + apdu_len);
            apdu_len += len;
            /* tag 3 - timeRemaining */
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 3, cov_data.timeRemaining, pos+apdu_len);
            apdu_len += len;
            /* tag 4 - listOfValues */
            len = BasicalProcessor.Encode_Opening_Tag(ref apdu, 4, pos + apdu_len);
            apdu_len += len;
            /* the first value includes a pointer to the next value, etc */



            for (int count = 0; count < cov_data.listOfValues.Count;count++ )
            {
                value = cov_data.listOfValues[count];
                /* tag 0 - propertyIdentifier */
                len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 0, (uint)value.propertyIdentifier, pos + apdu_len);
                apdu_len += len;
                /* tag 1 - propertyArrayIndex OPTIONAL */
                if (value.propertyArrayIndex != BacnetConst.BACNET_ARRAY_ALL)
                {
                    len =
                        BasicalProcessor.Encode_Context_Unsigned(ref apdu, 1,
                        (uint)value.propertyArrayIndex, apdu_len);
                    apdu_len += len;
                }
                /* tag 2 - value */
                len = BasicalProcessor.Encode_Opening_Tag(ref apdu, 2, pos + apdu_len);
                apdu_len += len;
                len = BasicalProcessor.Encode_Application_Data(ref apdu, ref value.value, apdu_len + pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Closing_Tag(ref apdu, 2, apdu_len + pos);
                apdu_len += len;
                /* tag 3 - priority OPTIONAL */
                if (value.priority != 0)
                {
                    len =
                        BasicalProcessor.Encode_Context_Unsigned(ref apdu, 3, value.priority, apdu_len);
                    apdu_len += len;
                }




            }

            len = BasicalProcessor.Encode_Closing_Tag(ref apdu, 4, pos + apdu_len);
            apdu_len += len;

            return apdu_len;
         }
        public void Unconfirm_Cov_Handler(ref Byte[] service_request, UInt16 service_len,ref BACNET_ADDRESS src )
        {
            int len = 0;
            BACNET_COV_DATA cov_data=new BACNET_COV_DATA();
              /* create linked list to store data if more
       than one property value is expected */
            len = Unconfirm_Cov_Decode(ref service_request, service_len, ref cov_data);
            MessageBox.Show(cov_data.initiatingDeviceIdentifier.ToString());            
          //可以对得到的数据进行操作

        }
        private int Unconfirm_Cov_Decode(ref Byte[] request,uint service_len,ref BACNET_COV_DATA data)
        {

            int len = 0;        /* return value */
            Byte tag_number = 0;
            UInt32 len_value = 0;
            UInt32 decoded_value = 0; /* for decoding */
            UInt16 decoded_type = 0;  /* for decoding */
            UInt32 property = 0;      /* for decoding */
               /* value in list */
            /* tag 0 - subscriberProcessIdentifier */
          //  if (decode_is_context_tag(&apdu[len], 0))
            len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref len_value, len);
            len += BasicalProcessor.Decode_Unsigned(ref request, len_value, ref decoded_value, len);
            data.subscriberProcessIdentifier = decoded_value;
            /* tag 1 - initiatingDeviceIdentifier */
            len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref  len_value, len);
            len += BasicalProcessor.Decode_Object_Id(ref request, ref decoded_type, ref data.initiatingDeviceIdentifier, len);
            /* tag 2 - monitoredObjectIdentifier */
            len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref len_value, len);
            len += BasicalProcessor.Decode_Object_Id(ref request, ref decoded_type, ref data.monitoredObjectIdentifier.instance, len);
            /* tag 3 - timeRemaining */
            len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref len_value, len);
            len += BasicalProcessor.Decode_Unsigned(ref request, len_value, ref decoded_value, len);
            data.timeRemaining = decoded_value;
            
               /* tag 4: opening context tag - listOfValues */
           //   if (!decode_is_opening_tag_number(&apdu[len], 4)) {
            len++;
          
            while (true) //应该设置遇到时间戳就BREAK? 下面CLOSING TAG有break
            {
                BACNET_PROPERTY_VALUE value = new BACNET_PROPERTY_VALUE(); 
                /* tag 0 - propertyIdentifier */

                if (BasicalProcessor.Decode_Is_Context_Tag(ref request, 0, len))
                {
                    len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref  len_value, len);
                    len += BasicalProcessor.Decode_Enumerated(ref request, len_value, ref  property, len);
                    value.propertyIdentifier = (BACNET_PROPERTY_ID)property;
                }
                else
                {
                    return -1;
                }
                /* tag 1 - propertyArrayIndex OPTIONAL */
                if (BasicalProcessor.Decode_Is_Context_Tag(ref request, 1, len))
                {
                    len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref  len_value, len);
                    len += BasicalProcessor.Decode_Unsigned(ref request, len_value, ref decoded_value, len);
                    value.propertyArrayIndex = (UInt32)decoded_value;
                }
                else
                {
                    value.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;//#define BACNET_ARRAY_ALL (~(unsigned int)0)

                }
                /* tag 2: opening context tag - value */
                if (!BasicalProcessor.Decode_Is_Opening_Tag_Number(ref request, 2,len))
                {
                    return -1;
                }
                len++;
                len += BasicalProcessor.Decode_Application_Data(ref request, (uint)(service_len - len), ref value.value, len);

                    if (!BasicalProcessor.Decode_Is_Closing_Tag_Number(ref request, 2,len)) {
                return -1;
                    }
                    len++;
                  
                    /* tag 3 - priority OPTIONAL */
                    if (BasicalProcessor.Decode_Is_Context_Tag(ref request, 3, len))
                    {
                        len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref  len_value, len);
                        len += BasicalProcessor.Decode_Unsigned(ref request, len_value, ref decoded_value, len);
                        value.priority = (Byte)decoded_value;

                    }
                    else
                    {
                        value.priority = 0;

                    }
                    data.listOfValues.Add(value);
                if (BasicalProcessor.Decode_Is_Closing_Tag_Number(ref request, 4, len))
                {
                    break;
                }
            }
          
            return len;
        }
    }
}
