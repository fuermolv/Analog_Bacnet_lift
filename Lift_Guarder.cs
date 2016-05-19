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
    class Lift_Guarder
    {
        private List<lift> Lift_Record;
        private List<BACNET_PROPERTY_VALUE> cov_value;
        private Boolean change;
        public static int ss=0;
        public Lift_Guarder()
        {
            Lift_Record = new List<lift>();
            for(int i=0;i<3;i++)
            {
                lift temp = new lift();
                Lift_Record.Add(temp);
            }
            change = false;
            cov_value = new List<BACNET_PROPERTY_VALUE>();

        }
        public void Check()
        {
            
           
            for(int i=0;i<Form_main.lift_count;i++)
            {
                if (Form_main.lift_list[i].cov_subcription.valid)
                {
                    if (Lift_Record[i].Service_Mode != Form_main.lift_list[i].Service_Mode)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].Service_Mode = Form_main.lift_list[i].Service_Mode;
                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Service_Mode;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                        temp.value.value.Unsigned_Int = (uint)Lift_Record[i].Service_Mode;
                        cov_value.Add(temp);

                        change = true;
                    }
                    if (Lift_Record[i].Car_Status != Form_main.lift_list[i].Car_Status)
                    {
                        Lift_Record[i].Car_Status = Form_main.lift_list[i].Car_Status;

                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;

                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Car_Status;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                        temp.value.value.Unsigned_Int = (uint)Lift_Record[i].Car_Status;
                        cov_value.Add(temp);

                        change = true;

                    }
                    if (Lift_Record[i].Car_Direction != Form_main.lift_list[i].Car_Direction)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].Car_Direction = Form_main.lift_list[i].Car_Direction;

                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Car_Direction;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                        temp.value.value.Unsigned_Int = (uint)Lift_Record[i].Car_Direction;
                        cov_value.Add(temp);
                        change = true;
                    }
                    if (Lift_Record[i].Door_Zone != Form_main.lift_list[i].Door_Zone)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].Door_Zone = Form_main.lift_list[i].Door_Zone;

                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Door_Zone;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_BOOLEAN;
                        temp.value.value.Boolean = Lift_Record[i].Door_Zone;
                        cov_value.Add(temp);
                        change = true;
                    }
                    if (Lift_Record[i].Car_Position != Form_main.lift_list[i].Car_Position)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].Car_Position = Form_main.lift_list[i].Car_Position;

                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Car_Position;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_UNSIGNED_INT;
                        temp.value.value.Unsigned_Int = Lift_Record[i].Car_Position;

                        cov_value.Add(temp);
                        change = true;
                    }
                    if (Lift_Record[i].Door_Status != Form_main.lift_list[i].Door_Status)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].Door_Status = Form_main.lift_list[i].Door_Status;

                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Door_Status;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_BOOLEAN;
                        temp.value.value.Boolean = Lift_Record[i].Door_Status;
                        cov_value.Add(temp);
                        change = true;
                    }
                    if (Lift_Record[i].Passenger_Status != Form_main.lift_list[i].Passenger_Status)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].Passenger_Status = Form_main.lift_list[i].Passenger_Status;

                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LIFT_Passenger_Status;
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_BOOLEAN;
                        temp.value.value.Boolean = Lift_Record[i].Passenger_Status;
                        cov_value.Add(temp);
                        change = true;
                    }
                    //Message Code未实现

                    if (change)
                    {
                        BACNET_PROPERTY_VALUE temp = new BACNET_PROPERTY_VALUE();
                        temp.value.context_specific = true;
                        temp.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp.value.context_tag = 2;
                        Lift_Record[i].SetTimeStamp();
                        temp.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LOCAL_DATE;
                       
                        temp.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_DATE;
                        temp.value.value.Date = Lift_Record[i].Time_Stamps.value.date;
                        cov_value.Add(temp);


                        BACNET_PROPERTY_VALUE temp1 = new BACNET_PROPERTY_VALUE();
                        temp1.value.context_specific = true;
                        temp1.propertyArrayIndex = BacnetConst.BACNET_ARRAY_ALL;
                        temp1.value.context_tag = 2;
                        temp1.propertyIdentifier = BACNET_PROPERTY_ID.PROP_LOCAL_TIME;
                        temp1.value.tag = (Byte)BACNET_APPLICATION_TAG.BACNET_APPLICATION_TAG_TIME;
                        temp1.value.value.Time = Lift_Record[i].Time_Stamps.value.time;
                        cov_value.Add(temp1);

                   

                    //发送COV 
                 

                        BACNET_COV_DATA cov_data = new BACNET_COV_DATA();
                        UnconFirmCovService Service = new UnconFirmCovService();
                        cov_data.subscriberProcessIdentifier = Form_main.lift_list[i].cov_subcription.subscriberProcessIdentifier;
                        cov_data.monitoredObjectIdentifier = Form_main.lift_list[i].cov_subcription.monitoredObjectIdentifier;
                        cov_data.timeRemaining = Form_main.lift_list[i].cov_subcription.lifetime;
                        cov_data.initiatingDeviceIdentifier = Form_main.lift_list[i].Device_Identifier;
                        cov_data.listOfValues = cov_value;
                       
                        Byte[] buff = new Byte[1024];

                        int Send_len=Service.Unconfirm_Cov_Service_Pack(ref buff, ref cov_data);



                        IPEndPoint dest = new IPEndPoint(IPAddress.Broadcast, 60);//假设60测试？
                        UdpSender sendder = new UdpSender(ref buff, dest);
                        sendder.SendBrocast(Send_len);
                        change = false;



                    }

                   
                    cov_value.Clear();

                }
               



            }
        }
    }
}
