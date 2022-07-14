using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;

namespace DC_Motor_PID_Controller
{
    public partial class Form1 : Form
    {
        double Kp = 0.003;
        double Ki = 0.003;
        double Kd = 0.003;
        double Speed = 100;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedItem = comboBox1.Items[5];
            String[] portList = SerialPort.GetPortNames();
            comboBox2.Items.AddRange(portList);
            if(portList.Length > 0)
            {
                comboBox2.SelectedItem = comboBox2.Items[0];
            }
            comboBox2.Enabled = false;
            button2.Enabled = false;
            comboBox3.SelectedItem = comboBox3.Items[0];
            this.serialPort1 += () => Threading.Dispatcher->invoke(updateGraph);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text=="" || comboBox2.Text == "")
            {
                MessageBox.Show("Baud Rate or Port is empty!");
                return;
            }
            serialPort1.PortName = comboBox2.Text;
            serialPort1.BaudRate = Int32.Parse(comboBox1.Text);
            serialPort1.Open();
            if(!serialPort1.IsOpen)
            {
                MessageBox.Show("Faild to connect to the device!");
                return;
            }
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            if (serialPort1.IsOpen)
            {
                MessageBox.Show("Faild to disconnect the device!");
                return;
            }
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox3.Text == "PID")
            {
                textBox2.Enabled = true;
                textBox2.Text = Kp.ToString();
                textBox5.Enabled = true;
                textBox5.Text = Ki.ToString();
                textBox4.Enabled = true;
                textBox4.Text = Kd.ToString();
            }
            else if(comboBox3.Text == "PI")
            {
                textBox2.Enabled = true;
                textBox2.Text = Kp.ToString();
                textBox5.Enabled = true;
                textBox5.Text = Ki.ToString();
                textBox4.Enabled = false;
                Kd = (textBox4.Text != (0.0).ToString()) ? Convert.ToDouble(textBox4.Text) : Kd;
                textBox4.Text = (0.0).ToString();
            }
            else if (comboBox3.Text == "PD")
            {
                textBox2.Enabled = true;
                textBox2.Text = Kp.ToString();
                textBox5.Enabled = false;
                Ki = (textBox5.Text != (0.0).ToString()) ? Convert.ToDouble(textBox5.Text) : Ki;
                textBox5.Text = (0.0).ToString();
                textBox4.Enabled = true;
                textBox4.Text = Kd.ToString();
            }
            else if (comboBox3.Text == "P")
            {
                textBox2.Enabled = true;
                Kp = Convert.ToDouble(textBox2.Text);
                textBox2.Text = Kp.ToString();
                textBox5.Enabled = false;
                Ki = (textBox5.Text != (0.0).ToString()) ? Convert.ToDouble(textBox5.Text) : Ki;
                textBox5.Text = (0.0).ToString();
                textBox4.Enabled = false;
                Kd = (textBox4.Text != (0.0).ToString()) ? Convert.ToDouble(textBox4.Text) : Kd;
                textBox4.Text = (0.0).ToString();
            }
            else if (comboBox3.Text == "I")
            {
                textBox2.Enabled = false;
                Kp = (textBox2.Text != (0.0).ToString()) ? Convert.ToDouble(textBox2.Text):Kp;
                textBox2.Text = (0.0).ToString();
                textBox5.Enabled = true;
                textBox5.Text = Ki.ToString();
                textBox4.Enabled = false;
                Kd = (textBox4.Text != (0.0).ToString()) ? Convert.ToDouble(textBox4.Text) : Kd;
                textBox4.Text = (0.0).ToString();
            }
            else if (comboBox3.Text == "D")
            {
                textBox2.Enabled = false;
                Kp = (textBox2.Text != (0.0).ToString()) ? Convert.ToDouble(textBox2.Text):Kp;
                textBox2.Text = Kp.ToString();
                textBox5.Enabled = false;
                Ki = (textBox5.Text != (0.0).ToString()) ? Convert.ToDouble(textBox5.Text):Kd;
                textBox5.Text = (0.0).ToString();
                textBox4.Enabled = true;
                textBox4.Text = Kd.ToString();
            }
        }

        private void readSerial(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadLine();
        }

        private void updateGraph(double x, double y)
        {
            chart.Series[0].Points.AddXY(x, y);
            chart.Update();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                return;
            }
            String data = (1).ToString() + '|' +
                            Convert.ToDouble(textBox2.Text).ToString() + '|' +
                            Convert.ToDouble(textBox5.Text).ToString() + '|' +
                            Convert.ToDouble(textBox4.Text).ToString() + '|' +
                            Convert.ToDouble(numericUpDown1.Text).ToString();
            serialPort1.WriteLine(data);
        }
    }
}
