namespace TEST_APP
{ 
    using System;
    using System.IO.Ports;
    using System.Windows.Forms;
    using System.Xml;
    using System.Drawing;
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using System.Text;
    using System.Linq;
    using App_v02;
    using System.Net.Sockets;
    using System.Net;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public partial class Form1 : Form
    {
        List<Button_user> but = new List<Button_user>();
        List<Slider_user> slider = new List<Slider_user>();

        static SerialPort _serialPort;

        public static bool Press_COM = false;
        public static bool Press_ETH = false;

        string IP_ETH = "192.168.1.193";
        int PORT_ETH = 1234;

        string default_PortName = "COM1";
        int default_BaudRate = 115200;
        System.IO.Ports.Parity default_Parity = Parity.None;
        int default_DataBits = 8;
        System.IO.Ports.StopBits default_StopBits = StopBits.One;
        byte default_header = (byte)0x3F;
        string filePath_XML = @"Commands.xml";

        App_v02.Form2 graph = new App_v02.Form2();
        public static int time;

        TextBox Value_slider = new TextBox();
        static IPEndPoint tcpEndPoint;
        static Socket tcpSocket;
        int Slider_val = 0;

        uint _TIME_ = 0;
        string _TIME_STRING_ = "";
        uint _LAST_TIMER_ = 0; 
        //public System.Windows.Forms.Timer timer1;

        //...........................................................�������� ����� ��������� ��� ����������� ������
        class Button_user : System.Windows.Forms.Button
        {
            public string name;
            public byte var;
            public byte[] vars;
            public byte header;
            public byte bytescol;
            public bool bytescol_flag;
            public byte control_sum;
            public bool control_sum_flag;
            public bool press_flag;
        }

        class Slider_user : System.Windows.Forms.TrackBar
        {
            public string name;
            public byte[] var;
            public byte header;
            public byte bytescol;
            public bool bytescol_flag;
            public byte control_sum;
            public bool control_sum_flag;

            public byte var_a;
            public byte var_b;
        }


        public Form1()
        {

            InitializeComponent();
            TEST_APP.Form1.timer1 = new System.Windows.Forms.Timer(this.components);
            TEST_APP.Form1.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            TEST_APP.Form1.timer1.Interval = 1000;
            _serialPort = new SerialPort(); //.................................�������� ���������� ������ SerialPort
            string[] ports = SerialPort.GetPortNames(); //........................�������� ���������� ��������� SerialPorts


            try
            {
                comboBox1.Items.AddRange(ports); //..................................... ���������� ��������� ������ � ���������
                default_PortName = ports[0];
            }
            catch
            {

            }

            Initialize_combobox();


            _serialPort.ReceivedBytesThreshold = 3;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            /// TEST_APP.Form1.timer1.Interval = 1000;
            textBox2.Text = filePath_XML;

        }

        //.........................................................................���������� ����������� ������� �� ���������
        private void Initialize_combobox()
        {
            comboBox1.Text = default_PortName;
            comboBox2.Text = default_BaudRate.ToString();
            comboBox3.Text = default_Parity.ToString();
            comboBox4.Text = default_DataBits.ToString();
            comboBox5.Text = default_StopBits.ToString();
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Press_COM)
            {
                _serialPort.PortName = comboBox1.SelectedItem.ToString();
                // textBox1.Text += "PortName :" + _serialPort.PortName + Environment.NewLine;
            }
            else
                textBox1.Text += "����� ������� :" + _serialPort.PortName + Environment.NewLine;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Press_COM)
            {
                switch (comboBox2.SelectedItem.ToString())
                {
                    case "110":
                        _serialPort.BaudRate = 110;
                        break;
                    case "300":
                        _serialPort.BaudRate = 300;
                        break;
                    case "1200":
                        _serialPort.BaudRate = 1200;
                        break;
                    case "2400":
                        _serialPort.BaudRate = 2400;
                        break;
                    case "4800":
                        _serialPort.BaudRate = 4800;
                        break;
                    case "9600":
                        _serialPort.BaudRate = 9600;
                        break;
                    case "19200":
                        _serialPort.BaudRate = 19200;
                        break;
                    case "38400":
                        _serialPort.BaudRate = 38400;
                        break;
                    case "57600":
                        _serialPort.BaudRate = 576000;
                        break;
                    case "115200":
                        _serialPort.BaudRate = 115200;
                        break;
                    default:
                        _serialPort.BaudRate = 115200;
                        break;
                }
                //textBox1.Text += "BaudRate :" + _serialPort.BaudRate.ToString() + Environment.NewLine;
            }
            else
                textBox1.Text += "Need stop :" + _serialPort.PortName + Environment.NewLine;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Press_COM)
            {
                switch (comboBox3.SelectedItem.ToString())
                {
                    case "Even":
                        _serialPort.Parity = Parity.Even;
                        break;
                    case "Odd":
                        _serialPort.Parity = Parity.Odd;
                        break;
                    case "Mark":
                        _serialPort.Parity = Parity.Mark;
                        break;
                    case "None":
                        _serialPort.Parity = Parity.None;
                        break;
                    default:
                        _serialPort.Parity = Parity.None;
                        break;
                }
                //textBox1.Text += "Parity :" + _serialPort.Parity + Environment.NewLine;
            }
            else
                textBox1.Text += "Need close :" + _serialPort.PortName + Environment.NewLine;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Press_COM)
            {
                switch (comboBox4.SelectedItem.ToString())
                {
                    case "5":
                        _serialPort.DataBits = 5;
                        break;
                    case "6":
                        _serialPort.DataBits = 6;
                        break;
                    case "7":
                        _serialPort.DataBits = 7;
                        break;
                    case "8":
                        _serialPort.DataBits = 8;
                        break;
                    default:
                        _serialPort.DataBits = 8;
                        break;
                }
                // textBox1.Text += "DataBits :" + _serialPort.DataBits + Environment.NewLine;
            }
            else
                textBox1.Text += "Need close :" + _serialPort.PortName + Environment.NewLine;
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Press_COM)
            {
                switch (comboBox5.SelectedItem.ToString())
                {
                    case "1":
                        _serialPort.StopBits = StopBits.One;
                        break;
                    case "2":
                        _serialPort.StopBits = StopBits.Two;
                        break;
                    default:
                        _serialPort.StopBits = StopBits.One;
                        break;
                }
                // textBox1.Text += "StopBits :" + _serialPort.StopBits + Environment.NewLine;
            }
            else
                textBox1.Text += "Need close :" + _serialPort.PortName + Environment.NewLine;
        }

        private void CLEAN_Cycle()
        {
            CheckBox[] i = { X1, X2, X3, X4, X5, X6, X7, X8, X9};
            foreach (CheckBox x in i)
            {
                SetCycl(false, x);
            }
        }

        private void User_button_reset_color_Click(object sender, EventArgs e)
        {
            
            foreach (Button_user x in but)
            {
                x.ForeColor = Color.Black;
                x.press_flag = false;
            }
            
            
        }
        //---------------------------------------------------------XML-���������----------------------------------------------------
        // .........................................................������� ���������� XML ��������� � �������� ������ �� ������ ��������� ������
        private void XML_Read()
        {
            // ..................................................................�������� ������ ������ , ���������� �� ������ ����������� ������
            

            XmlDocument xml = new XmlDocument();
            xml.Load(filePath_XML);
            XmlElement element = xml.DocumentElement;

            // ........................................................................������� ������ ������� � ����� �������
            tableLayoutPanel3.ColumnStyles.Clear();
            tableLayoutPanel3.RowStyles.Clear();

            tableLayoutPanel4.ColumnStyles.Clear();
            tableLayoutPanel4.RowStyles.Clear();

            //........................................................................ �������� ����� ������ ������� � �������
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 50));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.AutoSize, 50));

            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 50));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.AutoSize, 50));


            int i = 0, j = 0, k = 0;

            //.......................................................................���������� ��������� XML
            foreach (XmlNode xnode in element)
            {
                //...............................................................���������� ����� �������
                if (xnode.Name != "var")
                    tableLayoutPanel3.ColumnCount += 1;
                //..............................................................���������� �������� ���������
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (xnode.Name != "var")
                        tableLayoutPanel3.RowCount += 1;
                    //--------------------------------------------------------------------------- VAR
                    switch (childnode.Name)
                    {
                        case "send_const":
                            {
                                //.............................................................���������� � ������ ����� ����������� ����� ���������� ������
                                but.Add(new Button_user());

                                //.............................................................���������� ��������� ��������� ������
                                but[but.Count - 1].Text = childnode.Attributes.GetNamedItem("name").Value;

                                string temp_string_header = childnode.Attributes.GetNamedItem("header").Value;

                                //--------------------------------------------------------------------------- HEADER
                                if (!((temp_string_header == "") | (temp_string_header == null)))
                                    but[but.Count - 1].header = Convert.ToByte(temp_string_header, 16);

                                //--------------------------------------------------------------------------- BYTESCOL
                                if (childnode.Attributes.GetNamedItem("bytescol").Value == "y")
                                {
                                    but[but.Count - 1].bytescol_flag = true;
                                    but[but.Count - 1].bytescol = 4;
                                }
                                else
                                    but[but.Count - 1].bytescol_flag = false;

                                string temp_string_var = childnode.Attributes.GetNamedItem("var").Value;
                                if (!((temp_string_var == "") | (temp_string_var == null)))
                                    but[but.Count - 1].var = Convert.ToByte(temp_string_var, 16);

                                if (childnode.Attributes.GetNamedItem("control_sum").Value == "y")
                                {
                                    but[but.Count - 1].control_sum_flag = true;
                                    but[but.Count - 1].control_sum = crc_out(new byte[] { but[but.Count - 1].header, but[but.Count - 1].bytescol, but[but.Count - 1].var });
                                }
                                else
                                    but[but.Count - 1].control_sum_flag = false;

                                but[but.Count - 1].press_flag = false;

                                but[but.Count - 1].AutoSize  = false;
                                but[but.Count - 1].Width  = 150;
                                but[but.Count - 1].Height  = 30;
                                but[but.Count - 1].ForeColor  = Color.Black;

                                // ..............................................................�������� ������ ������� ��� ������� ������
                                but[but.Count - 1].Click  += new System.EventHandler(this.button_user_Click);

                                //..............................................................���������� � ������� ��������� ������
                                tableLayoutPanel3.Controls.Add(but[but.Count - 1], j, i);

                                break;
                            }
                        case "send_var":
                            {
                                k++;
                                tableLayoutPanel4.ColumnCount += 2;
                                //.............................................................���������� � ������ ����� ����������� ����� ���������� ������
                                slider.Add(new Slider_user());

                                //.............................................................���������� ��������� ��������� ������
                                slider[slider.Count - 1].name = childnode.Attributes.GetNamedItem("name").Value;

                                string temp_string_header = childnode.Attributes.GetNamedItem("header").Value;

                                //--------------------------------------------------------------------------- HEADER
                                if (!((temp_string_header == "") | (temp_string_header == null)))
                                    slider[slider.Count - 1].header = Convert.ToByte(temp_string_header, 16);

                                //--------------------------------------------------------------------------- BYTESCOL
                                if (childnode.Attributes.GetNamedItem("bytescol").Value == "y")
                                {
                                    slider[slider.Count - 1].bytescol_flag = true;
                                    slider[slider.Count - 1].bytescol = 6;
                                }
                                else
                                    slider[slider.Count - 1].bytescol_flag = false;

                                string temp_string_var = childnode.Attributes.GetNamedItem("var_a").Value;
                                if (!((temp_string_var == "") | (temp_string_var == null)))
                                    slider[slider.Count - 1].var_a = Convert.ToByte(temp_string_var, 16);

                                temp_string_var = childnode.Attributes.GetNamedItem("var_b").Value;
                                if (!((temp_string_var == "") | (temp_string_var == null)))
                                    slider[slider.Count - 1].var_b = Convert.ToByte(temp_string_var, 16);

                                if (childnode.Attributes.GetNamedItem("control_sum").Value == "y")
                                {
                                    slider[slider.Count - 1].control_sum_flag = true;
                                }
                                else
                                    slider[slider.Count - 1].control_sum_flag = false;

                                slider[slider.Count - 1].Orientation = Orientation.Vertical;
                                //slider[slider.Count - 1].Maximum = slider[slider.Count - 1].var_b * 10;
                                //slider[slider.Count - 1].Minimum = slider[slider.Count - 1].var_a;

                                slider[slider.Count - 1].Maximum = 100;
                                slider[slider.Count - 1].Minimum = 0;

                                slider[slider.Count - 1].Height = 370;
                                slider[slider.Count - 1].Width = 50;

                                TextBox label_slider = new TextBox();
                                TextBox max_slider = new TextBox();
                                TextBox min_slider = new TextBox();
                                Button_user take_current = new Button_user();

                                take_current.Text = "���������";
                                take_current.header = default_header;
                                take_current.bytescol = 6;

                                take_current.Click  += new System.EventHandler(button_take_current_Click);

                                label_slider.Text = slider[slider.Count - 1].name;
                                label_slider.BorderStyle = BorderStyle.None;

                                max_slider.Text = slider[slider.Count - 1].var_b.ToString();
                                max_slider.BorderStyle = BorderStyle.None;

                                min_slider.Text = slider[slider.Count - 1].var_a.ToString();
                                min_slider.BorderStyle = BorderStyle.None;

                                Value_slider.BorderStyle = BorderStyle.None;

                                Value_slider.Text = "��������: " + 0;

                                Slider_val = (byte)slider[slider.Count - 1].Value;

                                tableLayoutPanel4.RowCount += 3;


                                tableLayoutPanel4.Controls.Add(max_slider, k, 0);
                                tableLayoutPanel4.Controls.Add(slider[slider.Count - 1], k, 1);
                                tableLayoutPanel4.Controls.Add(min_slider, k, 2);
                                tableLayoutPanel4.Controls.Add(label_slider, k, 3);
                                tableLayoutPanel4.Controls.Add(Value_slider, k + 1, 2);
                                tableLayoutPanel4.Controls.Add(take_current, k + 1, 3);


                                slider[slider.Count - 1].Scroll += new System.EventHandler(slider_handler);
                                break;
                            }
                        default:
                            {
                                but[but.Count - 1].var = (byte)0x00;
                                break;
                            }
                    }

                    i++;
                }
                i = 0;
                j++;
            }
        }
        // ������ ���������
        private void button5_Click(object sender, EventArgs e)
        {
            tableLayoutPanel3.Controls.Clear();
            tableLayoutPanel4.Controls.Clear();
            XML_Read();
        }

        // ������ ..
        private void button4_Click(object sender, EventArgs e)
        {
            Choose_xml();
            textBox2.Text = filePath_XML;
        }

        private void Choose_xml()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "..\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath_XML = @openFileDialog.FileName;
            }
        }
        // ������ �������� ������� ����
        private async void button_take_current_Click(object sender, EventArgs e)
        {
                if (Press_ETH)
                {
                    try
                    {
                      
                    (sender as Button_user).vars = new byte[] { (byte)0xCC, (byte)(Slider_val >> 8), (byte)Slider_val };
                    await Task.Run(() => ETH_send(sender as Button_user));
                    }
                    catch
                    {
                        SetText_ETH("������ � " + IP_ETH.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    SetText_ETH("��� �����������" +  Environment.NewLine);
                    SetText_COM("��� �����������" +  Environment.NewLine);
                }
            //}

        }
        // ��������� �������� ������������� ����
        private void slider_handler(object sender, EventArgs e)
        {
            Slider_val = (int)((float)(sender as TrackBar).Value);
            //(sender as Slider_user).var = new byte[] {(byte)0xCC, (byte)(Slider_val >> 8), (byte)Slider_val};


            Value_slider.Text = "��������: " +  ((float)(sender as TrackBar).Value);

            List<byte> data = new List<byte>();
            if (Press_COM)
            {
                data.Add((byte)(Slider_val >> 8)); 
                data.Add((byte)(Slider_val & 0xFF));
                byte[] temp = combine_message(data.ToArray());
                //SetText_COM("����������: " + BitConverter.ToString(temp.ToArray()).Replace("-", " ") + Environment.NewLine);
                _serialPort.Write(temp, 0, temp.Length);
            }

        }
        //..........................................................................�������� ����������� ������� ���������� ��������� ������
        private async void button_user_Click(object sender, EventArgs e)
        {
            if (Press_COM)
            {
                try
                {
                    List<byte> out_word = new List<byte>();

                    out_word.Add((sender as Button_user).header);

                    if ((sender as Button_user).bytescol_flag)
                    {
                        out_word.Add((sender as Button_user).bytescol);
                    }

                    out_word.Add((sender as Button_user).var);



                    if ((sender as Button_user).control_sum_flag)
                        out_word.Add((sender as Button_user).control_sum);


                    if (Press_COM)
                    {
                        _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
                        SetText_COM("����������: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") +  Environment.NewLine);
                    }
                    if (!((sender as Button_user).press_flag))
                    {
                        (sender as Button_user).ForeColor = Color.Red;
                        (sender as Button_user).press_flag = true;
                    }
                    else
                    {
                        (sender as Button_user).ForeColor = Color.Black;
                        (sender as Button_user).press_flag = false;
                    }
                }
                catch
                {
                    SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }

            }
            else
            {
                if (Press_ETH)
                {
                    try
                    {
                        await Task.Run(() => ETH_send(sender as Button_user));
                    }
                    catch
                    {
                        SetText_ETH("������ � " + IP_ETH.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    SetText_ETH("��� �����������" +  Environment.NewLine);
                    SetText_COM("��� �����������" +  Environment.NewLine);
                }

            }
        }
        private void ETH_send(byte[] buf)
        {
            byte[] temp_buf = buf;
            tcpSocket.Send(temp_buf);
            var buffer = new byte[256];
            var size = 0;
            var answer = new StringBuilder();
            do
            {
                size = tcpSocket.Receive(buffer);
                answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (tcpSocket.Available > 0);

            if (buffer[0] == 0xDD)
            {
                float temp1 = (float)(((int)buffer[3] << 8) | ((int)buffer[4])) / 100;
                float temp2 = (float)(((int)buffer[5] << 8) | ((int)buffer[6])) / 10;
                time++;
                graph.Refresh_chart(temp1,temp2, time);
                SetText_ETH(BitConverter.ToString(buffer, 0, 3) + Environment.NewLine);
            }
            else 
            {
                if (buffer[0] == 0xAA)
                {
                    switch (buffer[1])
                    {
                        case 0xA0:
                            {
                                break;
                            }
                        case 0xA1:
                            {
                                SetCycl(true, X1);
                                break;
                            }
                        case 0xA2:
                            {
                                SetCycl(true, X2);
                                break;
                            }
                        case 0xA3:
                            {
                                SetCycl(true, X3);
                                break;
                            }
                        case 0xA4:
                            {
                                SetCycl(true, X4);
                                break;
                            }
                        case 0xA5:
                            {
                                SetCycl(true, X5);
                                break;
                            }
                        case 0xA6:
                            {
                                SetCycl(true, X6);
                                break;
                            }
                        case 0xA7:
                            {
                                SetCycl(true, X7);
                                break;
                            }
                        case 0xA8:
                            {
                                SetCycl(true, X8);
                                break;
                            }
                        case 0xA9:
                            {
                                SetCycl(true, X9);
                                break;
                            }
                        default:
                            {
                                SetText_ETH(answer.ToString() + Environment.NewLine);
                                break;
                            }
                    }
                }

            }    

            //SetText_ETH(answer.ToString() + Environment.NewLine);
            
            //answer.Clear();
        }
        private void ETH_send(Button_user button)
        {
            byte[] temp_buf;
            if (button.var == 0)
                temp_buf = button.vars;
            else
                temp_buf =  new byte[] {(byte) button.var };

            tcpSocket.Send(temp_buf);
            var buffer = new byte[256];
            var size = 0;
            var answer = new StringBuilder();
            do
            {
                size = tcpSocket.Receive(buffer);
                answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }
            while (tcpSocket.Available > 0);

            SetText_ETH(answer.ToString() + Environment.NewLine);
            answer.Clear();
            if (!((button).press_flag))
            {
                (button).ForeColor = Color.Red;
                (button).press_flag = true;
            }
            else
            {
                (button).ForeColor = Color.Black;
                (button).press_flag = false;
            }
        }
        //--------------------------------------------------������������_���-------------------------------------
        private async void K_PID_button_Click(object sender, EventArgs e)
        {
            if (Press_ETH)
            {
                await Task.Run(() => ETH_send(new byte[]
                {
                  0xB1,

                  (byte)((((int)K_p_UpDown.Value)) >> 8),
                  (byte)((((float)K_p_UpDown.Value))),

                  (byte)((((int)K_i_UpDown.Value)) >> 8),
                  (byte)((((float)K_i_UpDown.Value))),

                  (byte)((((int)K_d_UpDown.Value)) >> 8),
                  (byte)((((float)K_d_UpDown.Value))),

                }));
            }
            else if (Press_COM)
            {
                byte command = (byte)0xB1;
                List<byte> out_word = new List<byte>();


                out_word.Add(default_header);
                out_word.Add(0xA);
                out_word.Add(command);
                out_word.Add((byte)((((int)K_p_UpDown.Value)) >> 8));
                out_word.Add((byte)((((float)K_p_UpDown.Value))));
                out_word.Add((byte)((((int)K_i_UpDown.Value)) >> 8));
                out_word.Add((byte)((((float)K_i_UpDown.Value))));
                out_word.Add((byte)((((int)K_d_UpDown.Value)) >> 8));
                out_word.Add((byte)((((float)K_d_UpDown.Value))));
                out_word.Add(crc_out(out_word.ToArray()));

                try
                {
                    _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
                    SetText_COM("����������: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }
            }
        }

        private async void K_PID_button_Click_2(object sender, EventArgs e)
        {
            if (Press_ETH)
            {
                await Task.Run(() => ETH_send(new byte[]
                {
                  0xB2,

                  (byte)((((int)K_p_UpDown_2.Value)) >> 8),
                  (byte)((((float)K_p_UpDown_2.Value))),

                  (byte)((((int)K_i_UpDown_2.Value)) >> 8),
                  (byte)((((float)K_i_UpDown_2.Value))),

                  (byte)((((int)K_d_UpDown_2.Value)) >> 8),
                  (byte)((((float)K_d_UpDown_2.Value))),

                }));
            }
            else if (Press_COM)
            {
                byte command = (byte)0xB2;
                List<byte> out_word = new List<byte>();


                out_word.Add(default_header);
                out_word.Add(0xA);
                out_word.Add(command);
                out_word.Add((byte)((((int)K_p_UpDown_2.Value) >> 8)));
                out_word.Add((byte)((((float)K_p_UpDown_2.Value))));
                out_word.Add((byte)((((int)K_i_UpDown_2.Value)) >> 8));
                out_word.Add((byte)((((float)K_i_UpDown_2.Value))));
                out_word.Add((byte)((((int)K_d_UpDown_2.Value)) >> 8));
                out_word.Add((byte)((((float)K_d_UpDown_2.Value))));
                out_word.Add(crc_out(out_word.ToArray()));

                try
                {
                    _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
                    SetText_COM("����������: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }
            }
        }
        //---------------------------------------------------�����������-------------------------------------
        delegate void SetCyclCallback(bool flag , CheckBox box);
        private void SetCycl(bool flag, CheckBox box)
        {
            // ���� ������� ���������� ���������� ����� � ��������� ����� �� ��� �� �� �������� ��� ���� �������...
            if (this.textBox1.InvokeRequired)
            {
                // ...����� ������� �������� �����...
                SetCyclCallback d = new SetCyclCallback(SetCycl);
                this.Invoke(d, new object[] { flag , box });
            }
            // ...����� ��� �� ��������
            else
            {
                box.Checked = flag;      
            }
        }
        delegate void GET_TIME_delegate();
        void GET_TIME()
        {
            if (this.InvokeRequired)
            {
                GET_TIME_delegate d = new GET_TIME_delegate(GET_TIME);
                this.Invoke(d, new object[] { });
            }
            else
            {
                uint millsec = 0;
                uint sec = 0;
                uint min = 0;
                uint hour = 0;

                string sec_str = "";
                string min_str = "";
                string hour_str = "";

                millsec = (((uint)DateTime.Now.Ticks - _TIME_) / 10000);
                sec = millsec /1000 % 60;
                min = (millsec / 1000 / 60) % 60;
                hour = millsec / 1000 / 60  /60;

                if (sec < 10)
                    sec_str = "0" + sec.ToString();
                else
                    sec_str = sec.ToString();

                if (min < 10)
                    min_str = "0" + min.ToString() + ":";
                else
                    min_str = min.ToString() + ":";

                if (hour < 10)
                    hour_str = "0" + hour.ToString() + ":";
                else
                    hour_str = hour.ToString() + ":";

                _TIME_STRING_ =  hour_str + min_str + sec_str;
            }
                

        }
        void TIME_CLEAN()
        {
            TIME_1.Text = "00:00:00";
            TIME_2.Text = "00:00:00";
            TIME_3.Text = "00:00:00";
            TIME_4.Text = "00:00:00";
            TIME_5.Text = "00:00:00";
            TIME_6.Text = "00:00:00";
            TIME_7.Text = "00:00:00";
            TIME_8.Text = "00:00:00";
        }
        //delegate void TIMEx_REFRESH_delegate(Label x);
        void TIME_REFRESH(Label x)
        {
            //if (x.Text == "00:00:00")  
            GET_TIME();
            TIME_CHANGE(x);
            
        }
        delegate void TIMEx_CHANGE_delegate(Label x);
        void TIME_CHANGE(Label x)
        {
            if (x.InvokeRequired)
            {
                TIMEx_CHANGE_delegate d = new TIMEx_CHANGE_delegate(TIME_CHANGE);
                this.Invoke(d, new object[] { x });
            }
            else
                x.Text = _TIME_STRING_;
        }
        private async void timer2_Tick(object sender, EventArgs e)
        {
            //.........................................������� ������� ������
            if (Press_COM)
            {
                byte command = (byte)0xAA;
                List<byte> out_word = new List<byte>();


                out_word.Add(default_header);
                out_word.Add(0x4);
                out_word.Add(command);
                out_word.Add(crc_out(out_word.ToArray()));
                

                try
                {
                    _serialPort.Write(out_word.ToArray(), 0, out_word.Count);

                    
                    //SetText_COM("����������: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    //SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }
            }
            else if (Press_ETH)
            {
                try
                {
                    await Task.Run(() => ETH_send(new byte[] { 0xAA }));
                }
                catch
                {
                   // SetText_ETH("������ � " + IP_ETH.ToString() + Environment.NewLine);
                }
            }
            else
            {
                //SetText_ETH("��� �����������" +  Environment.NewLine);
               // SetText_COM("��� �����������" +  Environment.NewLine);
            }
    }

        private void Cyclogramm_button_Click(object sender, EventArgs e)
        {
            if (Press_COM || Press_ETH)
            {
                if (!timer2.Enabled)
                {
                    CLEAN_Cycle();
                    _TIME_ = ((uint)DateTime.Now.Ticks);
                    TIME_CLEAN();
                    timer2.Interval = 1000;
                    timer2.Enabled = true;
                    Cyclogramm_button.Text = "����������";
                }
                else
                {
                    timer2.Enabled = false;
                    Cyclogramm_button.Text = "������������";
                }
            }
        }

        //------------------------------------------------------------��������� ������� ������ �����--------------------------------
        private async void ANODE_TIM_BUTT_Click(object sender, EventArgs e)
        {
            if (Press_ETH)
            {
                await Task.Run(() => ETH_send(new byte[]
                {
                  0xA1,

                  (byte)((int)(time_anode_active.Value) >> 24),
                  (byte)((int)(time_anode_active.Value) >> 16),

                  (byte)((int)(time_anode_active.Value) >> 8),
                  (byte)((int)(time_anode_active.Value))

                }));
            }
            else if (Press_COM)
            {
                byte command = (byte)0xA1;
                List<byte> out_word = new List<byte>();

                out_word.Add(default_header);
                out_word.Add(0x8);
                out_word.Add(command);
                out_word.Add((byte)((int)(time_anode_active.Value) >> 24));
                out_word.Add((byte)((int)(time_anode_active.Value) >> 16));
                out_word.Add((byte)((int)(time_anode_active.Value) >> 8));
                out_word.Add((byte)((int)(time_anode_active.Value)));
                out_word.Add(crc_out(out_word.ToArray()));

                try
                {
                    _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
                    SetText_COM("����������: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }
            }
        }

        //-----------------------------------------------------------������ ����----------------------------------------------------
        public async void timer1_Tick(object sender, EventArgs e)
        {
            //.........................................������� ������� ������
            if (Press_COM)
            {
                byte command = (byte)0xEE;
                List<byte> out_word = new List<byte>();


                out_word.Add(default_header);
                out_word.Add(0x4);
                out_word.Add(command);
                out_word.Add(crc_out(out_word.ToArray()));

                try
                {
                    _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
                    //SetText_COM("����������: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    //SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }
            }
            else if (Press_ETH)
            {
                try
                {
                    await Task.Run(() => ETH_send(new byte[] {0xEE}));               

                }
                    catch
                    {
                       // SetText_ETH("������ � " + IP_ETH.ToString() + Environment.NewLine);
                    }
            }
            else
            {
                SetText_ETH("��� �����������" +  Environment.NewLine);
                SetText_COM("��� �����������" +  Environment.NewLine);
            }

        }

        // ����� �������
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                graph.Show();
                graph.InitializeComponent();
                graph.Init_chart();
                graph.Activate();
            }
            catch
            {
                SetText_COM("���� �������, ������ ��������" + Environment.NewLine);
            };
        }

        // ------------------------------------------------UART-----------------------------------------------

        // ������ UART
        private void button1_Click(object sender, EventArgs e)
        {
            Press_COM = !Press_COM;
            if (Press_COM == true)
            {
                try
                {
                    button1.Text = "�������";
                    button1.ForeColor = Color.DarkRed;
                    _serialPort.WriteTimeout = 500;
                    _serialPort.Open();
                }
                catch
                {
                    SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);
                }
            }
            else
            {
                try
                {
                    button1.Text = "���������";
                    button1.ForeColor = Color.ForestGreen;
                    _serialPort.Close();
                }
                catch
                {
                    SetText_COM("������ � :" + _serialPort.PortName + Environment.NewLine);

                }
            }
        }
        // ��������� ���������� ������
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            List<byte> buffer = new List<byte>();
            byte temp_header = 0;
            int len = 0;

            while (temp_header != default_header)
                try
                {
                    temp_header = (byte)((sender as SerialPort).ReadByte());
                    if (temp_header != default_header)
                    {
                        if (temp_header == 0xD | temp_header == 0xA)
                        {
                            SetText_COM(Environment.NewLine);
                        }
                        SetText_COM(Encoding.UTF8.GetString(new byte[] { temp_header }, 0, 1));

                    }
                    
                }
                catch 
                {
                    //SetText_COM("�� ������� ������������ ����: " + BitConverter.ToString(new byte[] { temp_header }).Replace("-", " ") + Environment.NewLine);
                }

            if (temp_header == default_header)
            {
                len = (sender as SerialPort).ReadByte();

                buffer.Add(temp_header);
                buffer.Add((byte)len);

                for (int i = 0; i < len - 3; i++)
                {
                    buffer.Add((byte)((sender as SerialPort).ReadByte()));
                }

                byte crc = (byte)((sender as SerialPort).ReadByte());
                string answ = "";

                if (crc_out(buffer.ToArray()) != crc)
                {
                    SetText_COM(" CRC �� ������ " + Environment.NewLine);
                }
                else
                {
                    buffer.Add(crc);


                    if (buffer[2] == (byte)0xDD)// ������� ������� � ������ 
                    {
                        float temp1 = (float)(((int)buffer[3] << 8) | ((int)buffer[4])) / 100;
                        float temp2 = (float)(((int)buffer[5] << 8) | ((int)buffer[6])) / 10;
                        time++;
                        graph.Refresh_chart(temp1, temp2, time);
                    }
                    else if (buffer[2] == 0xAA)
                    {
                        if (_LAST_TIMER_ != buffer[3])
                        {
                            _TIME_ = ((uint)DateTime.Now.Ticks);
                            _LAST_TIMER_ = buffer[3];
                        }
                        switch (buffer[3])
                        {
                            case 0xA0:
                                {   
                                    break;
                                }
                            case 0xA1:
                                {
                                    TIME_REFRESH(TIME_1);
                                    SetCycl(true, X1);
                                    break;
                                }
                            case 0xA2:
                                {
                                    TIME_REFRESH(TIME_2);
                                    SetCycl(true, X2);
                                    break;
                                }
                            case 0xA3:
                                {
                                    TIME_REFRESH(TIME_3);
                                    SetCycl(true, X3);
                                    break;
                                }
                            case 0xA4:
                                {
                                    TIME_REFRESH(TIME_4);
                                    SetCycl(true, X4);
                                    break;
                                }
                            case 0xA5:
                                {
                                    TIME_REFRESH(TIME_5);
                                    SetCycl(true, X5);
                                    break;
                                }
                            case 0xA6:
                                {
                                    TIME_REFRESH(TIME_6);
                                    SetCycl(true, X6);
                                    break;
                                }
                            case 0xA7:
                                {
                                    TIME_REFRESH(TIME_7);
                                    SetCycl(true, X7);
                                    break;
                                }
                            case 0xA8:
                                {
                                    TIME_REFRESH(TIME_8);
                                    SetCycl(true, X8);
                                    break;
                                }
                            case 0xA9:
                                {
                                    SetCycl(true, X9);
                                    break;
                                }
                            default:
                                {

                                    break;
                                }
                        }
                    }
                }
               //SetText_COM("��������: " + BitConverter.ToString(buffer.ToArray()).Replace("-", " ") + Environment.NewLine);

            }
            //else SetText_COM("��������: " + BitConverter.ToString(new byte[]{temp_header}).Replace("-", " ") + Environment.NewLine);
        }
        // ������� crc
        private byte crc_out(byte[] x)
        {
            return (byte)(x.Select(t => (int)t).Sum() & 0xFF); ;
        }
        
        // ���������� ����� � �������
        delegate void SetTextCallback(string text1);
        private void SetText_COM(string text1)
        {
            // ���� ������� ���������� ���������� ����� � ��������� ����� �� ��� �� �� �������� ��� ���� �������...
            if (this.textBox1.InvokeRequired)
            {
                // ...����� ������� �������� �����...
                SetTextCallback d = new SetTextCallback(SetText_COM);
                this.Invoke(d, new object[] { text1 });
            }
            // ...����� ��� �� ��������
            else
            {
                this.textBox1.Text += text1;
            }
        }
        // ������� ������� UART
        /*private void button3_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }*/
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        byte[] combine_message(byte[] data)
        {
            List<byte> message = new List<byte>();

            message.Add(default_header);
            message.Add((byte)(data.Length + 4));
            message.Add(0xDD);
            foreach (byte x in data)
                message.Add(x);
            message.Add(crc_out(message.ToArray()));

            return message.ToArray();
        }
        // ------------------------------------------------ETH-----------------------------------------------
        delegate void SetConnect_ETH_Callback(string text1);
        private void SetConnect_ETH(string text1)
        {
            // ���� ������� ���������� ���������� ����� � ��������� ����� �� ��� �� �� �������� ��� ���� �������...
            if (this.Connect_ETH.InvokeRequired)
            {
                // ...����� ������� �������� �����...
                SetConnect_ETH_Callback d = new SetConnect_ETH_Callback(SetConnect_ETH);
                this.Invoke(d, new object[] { text1 });
            }
            // ...����� ��� �� ��������
            else
            {
                this.Connect_ETH.Text = text1;
                this.Connect_ETH.ForeColor = Color.Aqua;
            }
        }
        // ����������� � �������
        private async void Connect_ETH_Click(object sender, EventArgs e)
        {

            if (Press_ETH == false)
            {
                try
                {
                    tcpEndPoint = new IPEndPoint(IPAddress.Parse(IP_ETH), PORT_ETH);
                    tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await Task.Run(() => 
                    {
                        //SetConnect_ETH("�����������");
                        tcpSocket.ConnectAsync(tcpEndPoint);                     
                    });
                    Press_ETH = !Press_ETH;
                    Connect_ETH.Text = "���������";
                    Connect_ETH.ForeColor = Color.DarkRed;
                    
                    
                    //tcpSocket.Connect(tcpEndPoint);
                   
                }
                catch
                {
                    SetText_ETH("������ � :" + tcpEndPoint.Address.ToString() + Environment.NewLine);
                }
            }
            else
            {
                try
                {
                    Press_ETH = !Press_ETH;
                    Connect_ETH.Text = "����������";
                    Connect_ETH.ForeColor = Color.ForestGreen;
                    tcpSocket.Shutdown(SocketShutdown.Both);
                    tcpSocket.Close();      
                }
                catch
                {
                    SetText_ETH("������ � :" + tcpEndPoint.Address.ToString()  + Environment.NewLine);
                }
            }
        }

        // ������ IP
        private void IP_text_TextChanged(object sender, EventArgs e)
        {
            IP_ETH = (sender as TextBox).Text.ToString();
        }
        // ������� PORT
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            PORT_ETH = Convert.ToInt32((sender as TextBox).Text.ToString());
        }

        // �������� ������ � �������
        delegate void SetText_ETH_Callback(string text1);
        private void SetText_ETH(string text1)
        {
            // ���� ������� ���������� ���������� ����� � ��������� ����� �� ��� �� �� �������� ��� ���� �������...
            if (this.ETH_CONSOLE.InvokeRequired)
            {
                // ...����� ������� �������� �����...
                SetText_ETH_Callback d = new SetText_ETH_Callback(SetText_ETH);
                this.Invoke(d, new object[] { text1 });
            }
            // ...����� ��� �� ��������
            else
            {
                this.ETH_CONSOLE.Text += text1;
            }
        }

        // �������� ������� 
        private void button2_Click(object sender, EventArgs e)
        {
            this.ETH_CONSOLE.Text = "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TIME_CLEAN();
            CLEAN_Cycle();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer2.Interval = (int)numericUpDown1.Value;
        }
    }

}
