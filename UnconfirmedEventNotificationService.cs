using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analog_Bacnet_lift
{
    class UnconfirmedEventNotificationService
    {

        public int Unconfirmed_Event_Notification_Pack(ref Byte[] buffer,ref BACNET_EVENT_NOTIFICATION_DATA data)
        {
            int len = 0;
            int pdu_len = 0;
            int bytes_sent = 0;
            BACNET_NPDU_DATA npdu_data=new BACNET_NPDU_DATA();
            BACNET_ADDRESS dest = new BACNET_ADDRESS();
            BACNET_ADDRESS my_address = new BACNET_ADDRESS();
            BacnetAddresssProcessor.Get_My_Address(ref my_address);
            BacnetAddresssProcessor.Get_My_Address(ref dest);


            NpduProcessor n_pro = new NpduProcessor();
            BvlcProcessor b_pro = new BvlcProcessor();
            n_pro.Encode_NpduData(ref npdu_data, true, BACNET_MESSAGE_PRIORITY.MESSAGE_PRIORITY_NORMAL);

            pdu_len = n_pro.Encode(ref buffer, ref dest, ref my_address, ref npdu_data);

            len = Uevent_Notify_Encode(ref buffer,ref data, pdu_len);
            pdu_len += len;


            bytes_sent = b_pro.Encode(ref buffer, ref dest, ref npdu_data, pdu_len);
            return bytes_sent;
        }
        private int Uevent_Notify_Encode(ref Byte[] apdu,ref BACNET_EVENT_NOTIFICATION_DATA data,int pos )
        {
            int len = 0;        /* length of each encoding */
            int apdu_len = 0;   /* total length of the apdu, return value */
            apdu[pos+0] = (Byte)BACNET_PDU_TYPE.PDU_TYPE_UNCONFIRMED_SERVICE_REQUEST;
            apdu[pos + 1] = (Byte)BACNET_UNCONFIRMED_SERVICE.SERVICE_UNCONFIRMED_EVENT_NOTIFICATION;     /* service choice */
            apdu_len = 2;

             len += Encode_Service_Request(ref apdu, ref data,apdu_len +pos);

            apdu_len += len;
            return apdu_len;
        }
        private int Encode_Service_Request(ref Byte[] apdu,ref BACNET_EVENT_NOTIFICATION_DATA data,int pos )
        {
            int len = 0;        /* length of each encoding */
            int apdu_len = 0;
            /* tag 0 - processIdentifier */
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 0, data.processIdentifier, pos + apdu_len);
            apdu_len += len;
            /* tag 1 - initiatingObjectIdentifier */
            len = BasicalProcessor.Encode_Context_ObjectId(ref apdu, 1, (int)data.initiatingObjectIdentifier.type, data.initiatingObjectIdentifier.instance, pos + apdu_len);
            apdu_len += len;
            /* tag 2 - eventObjectIdentifier */
            len = BasicalProcessor.Encode_Context_ObjectId(ref apdu, 2, (int)data.eventObjectIdentifier.type, data.eventObjectIdentifier.instance, pos + apdu_len);
            apdu_len += len;
            /* tag 3 - timeStamp */
            len = BasicalProcessor.Encode_Context_Timestamp(ref apdu, 3, ref data.timeStamp, pos + apdu_len);
            apdu_len += len;
            /* tag 4 - noticicationClass */
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 4, data.notificationClass, apdu_len + pos);
            apdu_len += len;
            /* tag 5 - priority */
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 5, data.priority, apdu_len + pos);
            apdu_len += len;
            /* tag 6 - eventType */
            len = BasicalProcessor.Encode_Context_Enumerate(ref apdu,6,(uint)data.eventType,apdu_len+pos);
            apdu_len += len;
            /* tag 7 - messageText */
           // if (data->messageText) 可选参数
            /* tag 8 - notifyType */
            len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 8, (uint)data.notifyType, apdu_len + pos);
            apdu_len += len;
            /* tag 9 - ackRequired */
            switch(data.notifyType)
            {
                case BACNET_NOTIFY_TYPE.NOTIFY_ALARM:
                case BACNET_NOTIFY_TYPE.NOTIFY_EVENT:
                    /* tag 9 - ackRequired */
                    len = BasicalProcessor.Encode_Context_Boolean(ref apdu, 9, data.ackRequired, apdu_len + pos);
                    apdu_len += len;
                    /* tag 10 - fromState */
                     len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 10, (uint)data.fromState, apdu_len + pos);
                    
                     apdu_len += len;
                 break;
                
                default:
                 break;
            }
            /* tag 11 - toState */
            len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 11, (uint)data.toState, apdu_len + pos);
            apdu_len += len;
            
            /* tag 12 - event values */
            if (data.notifyType == BACNET_NOTIFY_TYPE.NOTIFY_ALARM || data.notifyType==BACNET_NOTIFY_TYPE.NOTIFY_EVENT)
            {
                switch(data.eventType)
                { 
                //EVENT_CHANGE_OF_STATE:
             /*   case BACNET_EVENT_TYPE.EVENT_CHANGE_OF_STATE:
                {
                len = BasicalProcessor.Encode_Opening_Tag(ref apdu, 12,apdu_len+pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Opening_Tag(ref apdu, 1, apdu_len + pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Opening_Tag(ref apdu, 0, apdu_len + pos);
                apdu_len += len;
                len=BasicalProcessor.Encode_Property_State(ref apdu,ref data.notificationParams.newState,apdu_len+pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Closing_Tag(ref apdu, 0, apdu_len + pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Context_Bitstring(ref apdu, 1, ref data.notificationParams.statusFlags, apdu_len + pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Opening_Tag(ref apdu,1, apdu_len + pos);
                apdu_len += len;
                len = BasicalProcessor.Encode_Closing_Tag(ref apdu, 12,apdu_len+pos);
                apdu_len += len;
                break;
                 }*/
                case BACNET_EVENT_TYPE.EVENT_CHANGE_OF_LIFE_SAFETY:
                {
                    len = BasicalProcessor.Encode_Opening_Tag(ref apdu, 8, apdu_len + pos);
                    apdu_len += len;

                    len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 0, (UInt16)data.notificationParams.change_of_lifesafety.newState, apdu_len + pos);
            
                    apdu_len += len;
                    len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 1, (UInt16)data.notificationParams.change_of_lifesafety.newMode, apdu_len + pos);

                    apdu_len += len;
                    len = BasicalProcessor.Encode_Context_Bitstring(ref apdu, 2, ref data.notificationParams.change_of_lifesafety.statusFlags, apdu_len + pos);
                    apdu_len += len;

                    len = BasicalProcessor.Encode_Context_Enumerate(ref apdu, 3, (UInt16)data.notificationParams.change_of_lifesafety.operationExpected, apdu_len + pos);

                    apdu_len += len;


                    len = BasicalProcessor.Encode_Closing_Tag(ref apdu, 8, apdu_len + pos);
                    apdu_len += len;
                    break;
                }
            }

            }
            return apdu_len;
        }
        public int Uevent_Notify_Decode(ref Byte[] apdu,uint apdu_len,ref BACNET_EVENT_NOTIFICATION_DATA data)
        {
              int len = 0;        /* return value */
              int section_length = 0;
              UInt32 value = 0;
              Byte tag_number = 0;
              uint len_value = 0;
              /* tag 0 - processIdentifier */
             section_length =BasicalProcessor.Decode_Context_Unsigned(ref apdu,0,ref data.processIdentifier,len);
             len += section_length;

             /* tag 1 - initiatingObjectIdentifier */
             len += BasicalProcessor.Decode_Tag_number_and_Value(ref apdu, ref tag_number, ref  len_value, len);
             section_length = BasicalProcessor.Decode_Object_Id(ref apdu, ref data.initiatingObjectIdentifier.type,ref data.initiatingObjectIdentifier.instance, len);
             len += section_length;
             /* tag 2 - eventObjectIdentifier */
             len += BasicalProcessor.Decode_Tag_number_and_Value(ref apdu, ref tag_number, ref  len_value, len);
             section_length = BasicalProcessor.Decode_Object_Id(ref apdu, ref data.eventObjectIdentifier.type, ref data.eventObjectIdentifier.instance, len);
             len += section_length;
             /* tag 3 - timeStamp */
             section_length = BasicalProcessor.Decode_Context_Timestamp(ref apdu, 3,ref data.timeStamp, len);
             len += section_length;
             /* tag 4 - noticicationClass */
             section_length = BasicalProcessor.Decode_Context_Unsigned(ref apdu, 4, ref data.notificationClass, len);
             len += section_length;
             /* tag 5 - priority */
             section_length = BasicalProcessor.Decode_Context_Unsigned(ref apdu, 5, ref value, len);
             data.priority = (Byte)value;
             len += section_length;
             /* tag 6 - eventType */
             section_length = BasicalProcessor.Decode_Context_Enumerated(ref apdu, 6, ref value, len);
             data.eventType = (BACNET_EVENT_TYPE)value;
             len += section_length;
             /* tag 7 - messageText */ //option
            //此处没写

             /* tag 8 - notifyType */
             section_length = BasicalProcessor.Decode_Context_Enumerated(ref apdu, 8, ref value, len);
             data.notifyType = (BACNET_NOTIFY_TYPE)value;  
             len += section_length;
             
            switch(data.notifyType)
            {

                case BACNET_NOTIFY_TYPE.NOTIFY_ALARM:
                case BACNET_NOTIFY_TYPE.NOTIFY_EVENT:
                    /* tag 9 - ackRequired */
                    data.ackRequired=BasicalProcessor.Decode_Context_Boolean(ref apdu, len);
                    len++;
                    len++;
                    /* tag 10 - fromState */
                    section_length = BasicalProcessor.Decode_Context_Enumerated(ref apdu, 10, ref value, len);
                    data.fromState=(BACNET_EVENT_STATE) value;
                    
                    len += section_length;
                    break;
                default:
                    break;
            }
            /* tag 11 - toState */
            section_length = BasicalProcessor.Decode_Context_Enumerated(ref apdu, 11, ref value, len);
            data.toState = (BACNET_EVENT_STATE)value;
            len += section_length;
         
            /* tag 12 - eventValues */
            if (BasicalProcessor.Decode_Is_Opening_Tag_Number(ref apdu, 12,len))
                len++;
             if (BasicalProcessor.Decode_Is_Opening_Tag_Number(ref apdu,(Byte )data.eventType,len))
              len++;

              if (data.notifyType == BACNET_NOTIFY_TYPE.NOTIFY_ALARM || data.notifyType == BACNET_NOTIFY_TYPE.NOTIFY_EVENT)
              {
                  switch (data.eventType)
                  {
                      case BACNET_EVENT_TYPE.EVENT_CHANGE_OF_STATE:
                      //    section_length = BasicalProcessor.Decode_Context_Poroperty_State(ref apdu, 0, ref data.notificationParams.newState, len);
                          len += section_length;
                         
                     //     section_length = BasicalProcessor.Decode_Context_Bitstring(ref apdu, 1, ref data.notificationParams.statusFlags, len);
                          len += section_length;

                          break;
                      default:
                          break;

                  }
                  if (BasicalProcessor.Decode_Is_Closing_Tag_Number(ref apdu, (Byte)data.eventType, len))
                      len++;
                  if (BasicalProcessor.Decode_Is_Closing_Tag_Number(ref apdu, 12, len))
                      len++;
              }
               

               return len;
        }
        public void Uevent_Notify_Handler(ref Byte[] service_request,UInt16 service_len,ref BACNET_ADDRESS src)
        {
            int len = 0;
            BACNET_EVENT_NOTIFICATION_DATA data=new BACNET_EVENT_NOTIFICATION_DATA();
            len=Uevent_Notify_Decode(ref service_request,service_len,ref data );
            
           
            //此处添加对事件的处理

        }
    }
}
