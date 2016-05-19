using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analog_Bacnet_lift
{
    public partial class Form_New_Lift : Form
    {
        public Form_New_Lift()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            lift new_lift = new lift();
            new_lift.Object_Identifier.instance = Convert.ToUInt32(textBox1.Text);
            new_lift.Identification_Number = textBox2.Text;
            new_lift.Service_Mode = (lift.BACnetLiftServiceMode)Convert.ToByte(textBox3.Text);
            new_lift.Car_Position = Convert.ToByte(textBox4.Text);
            new_lift.Passenger_Status = Convert.ToBoolean(textBox5.Text);
            new_lift.Object_Name = textBox6.Text;


            Form_main.lift_list.Add(new_lift);
            Form_main.View_List[Form_main.lift_count].Items[0].SubItems[1].Text = textBox2.Text;
            Form_main.View_List[Form_main.lift_count].Items[1].SubItems[1].Text = textBox1.Text;
            Form_main.View_List[Form_main.lift_count].Items[2].SubItems[1].Text = textBox6.Text;
            Form_main.Scroll_Bar_List[Form_main.lift_count].Value = 100 - (new_lift.Car_Position - 1) * 25;
           
            Form_main.panel_list[Form_main.lift_count].Show();
            new_lift.number = Form_main.lift_count;
            Form_main.lift_count++;
           
           

          

        }

  
      
        
    }
}
