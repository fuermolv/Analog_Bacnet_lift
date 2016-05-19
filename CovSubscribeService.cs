using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Analog_Bacnet_lift
{
    class CovSubscribeService
    { 
        public Byte Cov_Subscribe_pack(ref Byte[] Handler_Transmit_Buffer,UInt32 device_id,ref BACNET_SUBSCRIBE_COV_DATA cov_data)
        {   Byte invoke_id=0;
            BACNET_ADDRESS dest=new BACNET_ADDRESS();
            BACNET_ADDRESS my_address=new BACNET_ADDRESS();
            BacnetAddresssProcessor.Get_My_Address(ref my_address);
            BacnetAddresssProcessor.Get_Device_Address(ref dest, device_id);
         //   uint max_apdu = 0;      
         //   bool status = false;
            int len = 0;
            int pdu_len = 0;
            int  byte_len = 0;
            BACNET_NPDU_DATA npdu_data=new BACNET_NPDU_DATA();
            NpduProcessor n_pro=new NpduProcessor();
            BvlcProcessor b_pro = new BvlcProcessor();
            //status = address_get_by_device(device_id, &max_apdu, &dest);
             //invoke_id = tsm_next_free_invokeID();
             invoke_id=1;
            n_pro.Encode_NpduData(ref npdu_data, true, BACNET_MESSAGE_PRIORITY.MESSAGE_PRIORITY_NORMAL);
          
            pdu_len = n_pro.Encode(ref Handler_Transmit_Buffer, ref dest, ref my_address, ref npdu_data);
              len = Cov_Subscribe_Encode(ref Handler_Transmit_Buffer,invoke_id, ref cov_data,pdu_len);
             pdu_len += len;
             // if ((unsigned) pdu_len < max_apdu) {
            // tsm_set_confirmed_unsegmented_transaction(invoke_id, &dest,
           //    &npdu_data, &Handler_Transmit_Buffer[0], (uint16_t) pdu_len);
          
            byte_len=b_pro.Encode(ref Handler_Transmit_Buffer, ref dest, ref npdu_data, pdu_len);
           
            return invoke_id;
           
        }
        int Cov_Subscribe_Encode(ref Byte[] apdu, int invoke_id, ref BACNET_SUBSCRIBE_COV_DATA cov_data, int pos)
        {
            int len = 0;        /* length of each encoding */
            int apdu_len = 0;
            apdu[pos + 0] = (byte)BACNET_PDU_TYPE.PDU_TYPE_CONFIRMED_SERVICE_REQUEST;
            apdu[pos + 1] = BasicalProcessor.Encode_MaxSegsandApdu(0, 1476);
            apdu[pos + 2] = (Byte)invoke_id;
            apdu[pos + 3] = (byte)BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_SUBSCRIBE_COV;
            apdu_len = 4;
            len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 0, cov_data.subscriberProcessIdentifier, pos + apdu_len);
            apdu_len += len;

            len = BasicalProcessor.Encode_Context_ObjectId(ref apdu, 1, (int)cov_data.monitoredObjectIdentifier.type, cov_data.monitoredObjectIdentifier.instance, pos + apdu_len);
            apdu_len += len;

            /*
             If both the 'Issue Confirmed Notifications' and
             'Lifetime' parameters are absent, then this shall
             indicate a cancellation request.
           */
            if (!cov_data.cancellationRequest)
            {
                /* tag 2 - issueConfirmedNotifications */
                len =
                    BasicalProcessor.Encode_Context_Boolean(ref apdu, 2,
                    cov_data.issueConfirmedNotifications, pos + apdu_len);
                apdu_len += len;
                /* tag 3 - lifetime */
                len = BasicalProcessor.Encode_Context_Unsigned(ref apdu, 3, cov_data.lifetime, pos + apdu_len);
                apdu_len += len;
            }

            return apdu_len;
        }
        /********************************************************************************************************************************************************************************/
        public void Cov_Subscribe_Handler(ref Byte[] request,UInt16 request_len,ref BACNET_ADDRESS src,ref BACNET_CONFIRMED_SERVICE_DATA request_data)
    {   int pdu_len=0;
        int byte_len = 0;
        Byte[] buffer = new Byte[1024];
        BACNET_SUBSCRIBE_COV_DATA cov_data = new BACNET_SUBSCRIBE_COV_DATA();
        BACNET_ADDRESS my_address=new BACNET_ADDRESS();
        NpduProcessor n_pro = new NpduProcessor();
        BACNET_NPDU_DATA npdu_data = new BACNET_NPDU_DATA();
        ApduProcessor a_pro = new ApduProcessor();
        BvlcProcessor b_pro = new BvlcProcessor();
        int apdu_len = -1;
        int npdu_len = -1;
        int len = 0;
        //cov_data.error_code = ABORT_REASON_SEGMENTATION_NOT_SUPPORTED;
        //datalink_get_my_address(&my_address);
        n_pro.Encode_NpduData(ref npdu_data, false, BACNET_MESSAGE_PRIORITY.MESSAGE_PRIORITY_NORMAL);
        npdu_len = n_pro.Encode(ref buffer, ref src, ref my_address, ref npdu_data);
        //  if (service_data.segmented_message) 
        /* we don't support segmentation - send an abort */
        len = Decode_Service_Request(ref request, request_len, ref cov_data);
      
        //
        for (int i = 0; i<Form_main.lift_count;i++)
        {   
            if(cov_data.monitoredObjectIdentifier.instance==Form_main.lift_list[i].Object_Identifier.instance)
            {
                Form_main.lift_list[i].cov_subscribe(src, cov_data);
              
            }
        }





            //
            apdu_len = a_pro.Encode_Simple_Ack(ref buffer, request_data.invoke_id, (Byte)BACNET_CONFIRMED_SERVICE.SERVICE_CONFIRMED_SUBSCRIBE_COV, npdu_len);
        pdu_len = npdu_len + apdu_len;
        byte_len = b_pro.Encode(ref buffer, ref src, ref npdu_data, pdu_len);
        


    }
    private int Decode_Service_Request(ref Byte[] request,uint request_len,ref  BACNET_SUBSCRIBE_COV_DATA data )
        {
            int len = 0;
            Byte tag_number = 0;
            UInt32 len_value = 0;
            UInt32 decoded_value = 0; /* for decoding */
            UInt16 decoded_type = 0;  /* for decoding */
           // if (decode_is_context_tag(&apdu[len], 0))
            len +=BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number,ref len_value,len);
            len += BasicalProcessor.Decode_Unsigned(ref request, len_value, ref decoded_value,len);
            data.subscriberProcessIdentifier = decoded_value;
            /* tag 1 - monitoredObjectIdentifier */
            len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref len_value, len);
            len += BasicalProcessor.Decode_Object_Id(ref request, ref decoded_type, ref data.monitoredObjectIdentifier.instance, len);
            data.monitoredObjectIdentifier.type = decoded_type;
            /* optional parameters - if missing, means cancellation */
            if ((uint)len < request_len)
            {
                if (BasicalProcessor.Decode_Is_Context_Tag(ref request, 2, len))
                {
                    data.cancellationRequest = false;
                    len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref len_value, len);
                    data.issueConfirmedNotifications = BasicalProcessor.Decode_Context_Boolean(ref request, len);
                    len += (int)len_value;
                    //  if (decode_is_context_tag(&apdu[len], 3))
                    len += BasicalProcessor.Decode_Tag_number_and_Value(ref request, ref tag_number, ref len_value, len);

                    len += BasicalProcessor.Decode_Unsigned(ref request, len_value, ref decoded_value, len);
                    data.lifetime = decoded_value;
                }
                else
                    data.cancellationRequest = true;

                
            }
            else
                data.cancellationRequest = true;

            return len;
        }
    }
  
} 

