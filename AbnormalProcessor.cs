using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analog_Bacnet_lift
{
   public class AbnormalProcessor
    {
       public int error_encode_apdu(ref Byte[] apdu,Byte invoke_id,BACNET_CONFIRMED_SERVICE service, BACNET_ERROR_CLASS error_class,BACNET_ERROR_CODE error_code,int pos)
       {
           int apdu_len = 0;
           apdu[pos+0] = (Byte)BACNET_PDU_TYPE.PDU_TYPE_ERROR;
           apdu[pos+1] = invoke_id;
           apdu[pos+2] = (Byte)service;
           apdu_len = 3;
           apdu_len += BasicalProcessor.Encode_Application_Enumerated(ref apdu, (uint)error_class,pos+apdu_len);
           apdu_len += BasicalProcessor.Encode_Application_Enumerated(ref apdu, (uint)error_code, pos + apdu_len);
           return apdu_len;


       }
       public int reject_encode_apdu(ref Byte[]apdu,Byte invoke_id,Byte reason,int pos)
       {
           int apdu_len = 0;   /* total length of the apdu, return value */


           apdu[pos + 0] = (Byte)BACNET_PDU_TYPE.PDU_TYPE_REJECT;
               apdu[pos+1] = invoke_id;
               apdu[pos+2] = reason;
               apdu_len = 3;
           

           return apdu_len;
       }
       public int abort_encode_apdu(ref Byte[] apdu, Byte invoke_id, Byte reason,Boolean server, int pos)
       {
           int apdu_len = 0;   /* total length of the apdu, return value */

           if (server)
               apdu[pos + 0] = (Byte)BACNET_PDU_TYPE.PDU_TYPE_ABORT | 1;
           else
               apdu[pos + 0] = (Byte)BACNET_PDU_TYPE.PDU_TYPE_ABORT;
           apdu[pos + 1] = invoke_id;
           apdu[pos + 2] = reason;
           apdu_len = 3;


           return apdu_len;
       }

    }
}
