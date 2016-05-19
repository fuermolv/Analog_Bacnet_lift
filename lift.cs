using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Analog_Bacnet_lift
{
    public class lift
    {
        public UInt32 Device_Identifier;//设备标识符
        public BACNET_OBJECT_ID Object_Identifier;//标识符
        public String Object_Name;//名称
        public BACNET_OBJECT_TYPE Object_Type;//类型
        public String Identification_Number;//设备识别码
        public BACNET_TIMESTAMP Time_Stamps;//数据生成时间
        public BACnetLiftServiceMode Service_Mode;//当前服务状态         
        public Byte Car_Status;//轿厢运行状态
        public Byte Car_Direction;//轿厢运行方向
        public Boolean Door_Zone;//门区
        public Byte Car_Position;//电梯当前楼层
        public Boolean Door_Status;//关门到 位
        public Boolean Passenger_Status;//轿内是否有人      
        public UInt32 Total_Running_Time;//设备累计运行时间              
        public UInt32 Present_Counter_Value;//设备当前运行次数             
        public List<BACnetMessageCode> Message_Code;//信息代码
        public BACNET_COV_SUBSCRIPTION cov_subcription;//cov处理
        public int number;//编号
          public lift()
          {
              Device_Identifier = 9527;
              Object_Identifier.type = (ushort)BACNET_OBJECT_TYPE.BACNET_LIFT;
              Object_Identifier.instance = 0;
              Object_Name = "test_lift";
              Object_Type = BACNET_OBJECT_TYPE.BACNET_LIFT;
              Identification_Number = "0";
              Time_Stamps = new BACNET_TIMESTAMP();
              Service_Mode = BACnetLiftServiceMode.Normal;
              cov_subcription = new BACNET_COV_SUBSCRIPTION();
              Car_Status =0;
              Car_Direction = 0;
              Door_Zone = true;
              Car_Position = 0;
              Door_Status = true;
              Passenger_Status = false;
              Total_Running_Time = 10;
              Present_Counter_Value = 10;
              Message_Code = new List<BACnetMessageCode>();
              Message_Code.Add(BACnetMessageCode.Fault_Free);
              

              



          }
        private void Up(int stop_position)
          {
            if(Car_Position<5)
            {
                Service_Mode = BACnetLiftServiceMode.Normal;
                Car_Direction = 1;
                Door_Status = true;
                Passenger_Status = true;
                Car_Status = 1;
                for (int i = 100 - ((Car_Position-1) * 25); i >= 100-((Car_Position)  * 25); i--)
                {
                    Form_main.Scroll_Bar_List[number].Value = i;
                    Thread.Sleep(400);

                }
                Car_Position++;
                if(Car_Position==stop_position)
                {
                     Car_Direction = 0;
                     Car_Status=0;
                     Door_Status = false;
                     Passenger_Status = false;
                }
                
            }
          }
        public void SetTimeStamp()
        {
            DateTime now = DateTime.Now;
            Time_Stamps.value.date.day=(Byte)now.Day;
            Time_Stamps.value.date.year = (UInt16)now.Year;
            Time_Stamps.value.date.month = (Byte)now.Month;
            Time_Stamps.value.date.wday = (Byte)now.DayOfWeek;
            Time_Stamps.value.time.hour=(Byte)now.Hour;
            Time_Stamps.value.time.min=(Byte)now.Minute;
            Time_Stamps.value.time.sec=(Byte)now.Second;
            Time_Stamps.value.time.hundredths = (Byte)now.Millisecond;


        }
        private void Down(int stop_position)
        {   
            if (Car_Position >1 )
            {
                Service_Mode = BACnetLiftServiceMode.Normal;
                Car_Direction = 2;
                Door_Status = true;
                Car_Status = 1;
                Passenger_Status = true;
                for (int i = 100 - ((Car_Position - 1) * 25); i <= 100 - ((Car_Position-2) * 25) ; i++)
                {
                    Form_main.Scroll_Bar_List[number].Value = i;
                    Thread.Sleep(400);

                }
                Car_Position--;
                if (Car_Position == stop_position)
                {
                    Car_Direction = 0;
                    Car_Status = 0;
                    Door_Zone = true;
                    Door_Status = false;
                    Passenger_Status = false;
                }

            }
        }

        public void Move_To(Object dest)
          {
              int destination=(int)dest;
              int now_position = Car_Position;
              if (destination == now_position)
                  return;
              if(destination> Car_Position)
              {
                  for (int i = 0; i < destination - now_position; i++)
                  {
                      Up(destination);
                  }
              }
              if (destination < now_position)
             {
                 for (int i = 0; i < now_position - destination; i++)
                 {
                     Down(destination);
                 }
             }
              
             
          }
        public Boolean cov_subscribe( BACNET_ADDRESS src, BACNET_SUBSCRIBE_COV_DATA data)
        {
            Boolean successful = false;
            if (data.cancellationRequest)
            {
              

                cov_subcription.valid = false;
                successful = true;
            }

            else
            {
                cov_subcription.valid = true;
                cov_subcription.invokeID = 0;
                cov_subcription.issueConfirmedNotifications = data.issueConfirmedNotifications;
                cov_subcription.lifetime = data.lifetime;
                cov_subcription.monitoredObjectIdentifier = data.monitoredObjectIdentifier;
                cov_subcription.subscriberProcessIdentifier = data.subscriberProcessIdentifier;
                cov_subcription.dest = src;
                successful = true;
                



            }

            
            return successful;
        }
            public enum BACnetMessageCode
    {

       Fault_Free=0,//电梯无故障
       Safe_Switch_Off=1 , //电梯运行时安全回路断路
       Close_Door_Fault= 2, //关门故障
       Open_Door_Fault = 3, //关门故障
       Acident_Break_Off=4   ,   //轿厢在开锁区域外停止
       Acident_Move = 5, //轿厢意外移动
       Motor_Restrict =6,//电动机运转时间限制器动作
       Car_Position_Miss = 7,//楼层位置丢失
       Prevent_Falut_Again=8, //防止电梯再运行故障
       Auto_Operation = 40,//电梯恢复自动运行模式
       Outage = 41,//主电源断电
       Stop_Pattern=42,//进入停止服务
       Repair_Pattern = 43,//进入检修运行模式
       Fire_Return_Pattern = 44,//进入消防返回模式
       Fire_Pattern = 45,//进入消防员运行模式
       Emergency_Power_Parttern = 46,//进入应急电源运行
       Earthquake_Parttern=47,//地震模式
       Alarm_button = 90, //报警按钮动作
       


    }

         public enum BACnetLiftServiceMode
    {
        Stop = 0,
        Normal = 1,
        Repair = 2,
        Fire_return = 3,
        Fire_operation = 4,
        Eps = 5,
        Earthquake = 6,
        Unknown = 7

    }

     

     }
}
