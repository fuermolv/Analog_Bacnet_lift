using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analog_Bacnet_lift
{
    class TsmProcessor
    { 

    
       static Byte[] invoke_id_array;
       static Byte pos;
  
         static TsmProcessor()
       {
           invoke_id_array = new Byte[100];
            for(int i=0;i<10;i++)
            {
                invoke_id_array[i]=0;

            }
           pos = 0;
       }
        public static Byte next_free_id()
         {
             Byte now = pos;
             pos++;
            while(pos==now)
            {
                if (invoke_id_array[pos] == 0)
                {
                    invoke_id_array[pos] = 1;
                    return pos;

                }
                else 
                    pos++;
                if(pos==100)
                {
                    pos = 0;
                }
            }
            return 255; //代表错误
        }
        public static void free_invoke_id(Byte id)
        {
            invoke_id_array[pos] = 0;

        }
     
    }
}
