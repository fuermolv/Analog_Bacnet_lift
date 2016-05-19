using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace Analog_Bacnet_lift
{
    public partial class Form_main : Form
    {

        public static List<lift> lift_list;
        public static int lift_count=0;
        public static List<ListView> View_List;
        public static lift true_lift;
        public static List<VScrollBar> Scroll_Bar_List;
        public static List<Panel> panel_list;
        private Lift_Guarder guarder;
     
        public Form_main()
        {
            InitializeComponent();
            View_List = new List<ListView>();
            Scroll_Bar_List = new List<VScrollBar>();
            lift_list = new List<lift>();
            panel_list = new List<Panel>();
            Control.CheckForIllegalCrossThreadCalls = false;
           
                
        }

   

   

        private void Form_main_Load(object sender, EventArgs e)
        {
            
            View_List.Add(listView1);
            View_List.Add(listView2);
            View_List.Add(listView3);
            Scroll_Bar_List.Add(vScrollBar1);
            Scroll_Bar_List.Add(vScrollBar2);
            Scroll_Bar_List.Add(vScrollBar3);
            panel_list.Add(panel1);
            panel_list.Add(panel2);
            panel_list.Add(panel3);


            listview_init();
            for(int i=0;i<3;i++)
            {
                panel_list[i].Hide();
            }
           
            lift_update_timer.Start();
            guarder = new Lift_Guarder();

            
        
          
           
           
            


        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.button4.Enabled == true)
            {

                UdpReceiver udp_re = new UdpReceiver();
                Thread t1 = new Thread(new ThreadStart(udp_re.setserver));
                t1.Start();

                this.button4.Enabled = false;
            } 
        }

        private void button1_Click(object sender, EventArgs e)
        {   if(lift_count<=3)
        {
            
            Form_New_Lift lift_form = new Form_New_Lift();
            lift_form.ShowDialog();

        }
        }

       private void listview_init()
        {
            for (int i = 0; i < 3; i++)
            {
                View_List[i].Items.Add(new ListViewItem(new string[] { "设备标识符", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "对象标识符", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "对象名称", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "服务状态", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "运行状态", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "运行方向", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "门区", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "当前楼层", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "关门到位", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "轿内人员", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "运行时间", "", }));
                View_List[i].Items.Add(new ListViewItem(new string[] { "运行次数", "", }));
            }
        
        }
        private void Lift_Update()
       {
           for (int i = 0; i < lift_count; i++)
           {  
               switch(lift_list[i].Service_Mode)
               {
                   case lift.BACnetLiftServiceMode.Stop:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "停止";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Normal:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "正常";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Repair:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "修理";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Eps:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "应急电源";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Fire_operation:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "消防运行";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Fire_return:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "消防返回";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Earthquake:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "地震模式";
                           break;
                       }
                   case lift.BACnetLiftServiceMode.Unknown:
                       {
                           View_List[i].Items[3].SubItems[1].Text = "未知";
                           break;
                       }
               }
               switch(lift_list[i].Car_Status)
               {
                   case 0:
                       {
                           View_List[i].Items[4].SubItems[1].Text="停止";
                           break;
                       }
                   case 1:
                        {
                            View_List[i].Items[4].SubItems[1].Text ="运行";
                            break;
                       }
               }

                switch(lift_list[i].Car_Direction)
                {
                    case 0:
                        {
                            View_List[i].Items[5].SubItems[1].Text = "无方向";
                            break;
                        }
                    case 1:
                        {
                            View_List[i].Items[5].SubItems[1].Text = "上行";
                            break;
                        }
                    case 2:
                        {
                            View_List[i].Items[5].SubItems[1].Text = "下行";
                            break;
                        }
                }
              if(lift_list[i].Door_Zone)
               
                  
                       {
                           View_List[i].Items[6].SubItems[1].Text = "在门区";
                          
                       }
              else
                       {
                           View_List[i].Items[6].SubItems[1].Text = "非门区";
                        
                       }

             
               View_List[i].Items[7].SubItems[1].Text = lift_list[i].Car_Position.ToString();

               if (lift_list[i].Door_Status)
              
                  
                       {
                           View_List[i].Items[8].SubItems[1].Text = "到位";
                          
                       }
               else
                       {
                           View_List[i].Items[8].SubItems[1].Text = "不到位";
                          
                       }

            
              if(lift_list[i].Passenger_Status)
              
                       {
                           View_List[i].Items[9].SubItems[1].Text = "有人";
                         
                       }
              else
                       {
                           View_List[i].Items[9].SubItems[1].Text = "没人";
                           
                       }

               View_List[i].Items[10].SubItems[1].Text=lift_list[i].Total_Running_Time.ToString()+"小时";
               View_List[i].Items[11].SubItems[1].Text=lift_list[i].Present_Counter_Value.ToString()+"次";

               lift_list[i].SetTimeStamp();

               }
             

              
           }

       
        private void lift_update_timer_Tick(object sender, EventArgs e)
        {
            Lift_Update();
            guarder.Check();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Enabled == true)
            {

                Thread t1 = new Thread(new ParameterizedThreadStart(lift_list[0].Move_To));
                t1.Start(Convert.ToInt32(textBox1.Text));
               
            } 
           
        }

 

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void vScrollBar3_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.button3.Enabled == true)
            {

                Thread t2 = new Thread(new ParameterizedThreadStart(lift_list[1].Move_To));
                t2.Start(Convert.ToInt32(textBox2.Text));

            } 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.button5.Enabled == true)
            {

                Thread t2 = new Thread(new ParameterizedThreadStart(lift_list[2].Move_To));
                t2.Start(Convert.ToInt32(textBox3.Text));

            } 
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            BACNET_EVENT_NOTIFICATION_DATA data = new BACNET_EVENT_NOTIFICATION_DATA();
            UnconfirmedEventNotificationService unconf = new UnconfirmedEventNotificationService();
            data.processIdentifier = 123;
            data.ackRequired = true;
            BACNET_OBJECT_ID obj_id=new BACNET_OBJECT_ID();
            obj_id.type=(UInt16)BACNET_OBJECT_TYPE.BACNET_LIFT;
            obj_id.instance=Form_main.lift_list[0].Object_Identifier.instance;
            data.eventObjectIdentifier = obj_id;
            data.eventType = BACNET_EVENT_TYPE.EVENT_CHANGE_OF_LIFE_SAFETY;
            data.fromState = BACNET_EVENT_STATE.EVENT_STATE_NORMAL;
            data.timeStamp = Form_main.lift_list[0].Time_Stamps;
            data.notifyType = BACNET_NOTIFY_TYPE.NOTIFY_ALARM;
            data.toState = BACNET_EVENT_STATE.EVENT_STATE_FAULT;
            data.priority = 3;


            BACNET_OBJECT_ID obj_temp=new BACNET_OBJECT_ID();
             obj_temp.type=(UInt16)BACNET_OBJECT_TYPE.OBJECT_DEVICE;
             obj_temp.instance=9527;
             data.notificationParams.change_of_lifesafety.statusFlags.set_bits_used(1, 5);
             data.notificationParams.change_of_lifesafety.statusFlags.set_octet(0, 4);
             data.notificationParams.change_of_lifesafety.newState = BACNET_LIFE_SAFETY_STATE.LIFE_SAFETY_STATE_ALARM;
             data.notificationParams.change_of_lifesafety.newMode = BACNET_LIFE_SAFETY_MODE.LIFE_SAFETY_MODE_DISABLED;
             data.notificationParams.change_of_lifesafety.operationExpected = BACNET_LIFE_SAFETY_OPERATION.LIFE_SAFETY_OP_RESET_ALARM;

           //  data.notificationParams.newState.tag = BACNET_PROPERTY_STATE_TYPE.UNSIGNED_VALUE;
         //    data.notificationParams.newState.value = 100;
         //    data.notificationParams.statusFlags.set_bits_used(1, 5);
         //    data.notificationParams.statusFlags.set_octet(0,4);
           
          

             data.initiatingObjectIdentifier = obj_temp;
             data.notificationClass = 123;

            

            Byte[] buff = new Byte[1024];
         

            int Send_len = unconf.Unconfirmed_Event_Notification_Pack(ref buff, ref data);

            IPEndPoint dest = new IPEndPoint(IPAddress.Parse("10.10.161.153"), 60);//假设60测试？
            UdpSender sendder = new UdpSender(ref buff, dest);
            sendder.Send(Send_len);
        }


     
       }
     

     

     

    }

