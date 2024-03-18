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
    using ScottPlot.Demo.WinForms.WinFormsDemos;
    using System.Diagnostics.Eventing.Reader;
    using System.ComponentModel;
    using System.Reflection;
    using ScottPlot.Drawing.Colormaps;
    using ScottPlot;
    using ScottPlot.Demo.WinForms.WinFormsDemos;
    using System.Drawing.Drawing2D;
    using System.Threading;

    public partial class Form1 : Form
    {
        DateTime time_global= DateTime.Now;
        List<Button_user> but = new List<Button_user>();
        List<Slider_user> slider = new List<Slider_user>();
        Button_user but_telemetry;
        class USER_struct
        {
            public string name;
            public byte var;
            public bool count;
            public bool crc;
            public string type;
            public string var_string;
        }
        class USER_struct_telemetry
        {
            public string name;
            public byte var;
            public bool count;
            public bool crc;
            public string type;
            public string var_string;
            public List<USER_tlm_data> user_tlm_data = new List<USER_tlm_data>();
        }

        class USER_tlm_data
        {
            public string name;

            public int size;
            public int index_graph;
            public int division;
            public int mult;
            public List<USER_temp_tlm_data> user_temp_tlm_data = new List<USER_temp_tlm_data>();
            public List<USER_Bool_tlm_data> user_bool_tlm_data = new List<USER_Bool_tlm_data>();
            public List<USER_uint_tlm_data> user_uint_tlm_data = new List<USER_uint_tlm_data>();
        }

        class USER_temp_tlm_data
        {
            public int min_plus_var;
            public int max_plus_var;
            public int min_minus_var;
            public int max_minus_var;
        }

        class USER_uint_tlm_data
        {
            public int max_var;
            public int min_var;
        }

        class USER_Bool_tlm_data
        {
            public string name;
            public int index;
            public int index_flag = -1;
        }

        List<USER_struct> STRUCT_ANSWER = new List<USER_struct>();
        List <USER_struct> STRUCT_COMMAND = new List<USER_struct>();
        List<USER_struct_telemetry> STRUCT_TELEMETRY = new List<USER_struct_telemetry>();

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

        //App_v02.Form2 graph = new App_v02.Form2();
        App_v02.Form2 graph;
        PLAY_PLOTS PLAY_GRAPH_win;
        public static int time;

        TextBox Value_slider = new TextBox();
        static IPEndPoint tcpEndPoint;
        static Socket tcpSocket;
        int Slider_val = 0;
        
        double temp_time = 0;

        long _TIME_ = 0;
        string _TIME_STRING_ = "";
        uint _LAST_TIMER_ = 0; 
        //public System.Windows.Forms.Timer timer1;

        //...........................................................Создание новой структуры для процедурных кнопок
        class Button_user : System.Windows.Forms.Button
        {
            public string name;
            public byte var;
            public byte[] vars;  
            public List<CheckedListBox> checkBox = new List<CheckedListBox>();
            public List<NumericUpDown> uint8 =  new List<NumericUpDown>();
            public List<NumericUpDown> int32 = new List<NumericUpDown>();
            public List<string> str_type = new List<string>();
            public bool press_flag;

            //protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
            //{
            //    GraphicsPath p = new GraphicsPath();
            //    p.AddEllipse(1, 1, base.Width - 4, base.Height - 4);
            //    base.Region = new Region(p);
            //}
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
            Окно_электрических_параметров panel_tlm = new Окно_электрических_параметров();
            panel_tlm.Show();
            InitializeComponent();
            TEST_APP.Form1.timer1 = new System.Windows.Forms.Timer(this.components);
            TEST_APP.Form1.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            TEST_APP.Form1.timer1.Interval = 1000;
            _serialPort = new SerialPort(); //.................................Создание экземпляра класса SerialPort
            string[] ports = SerialPort.GetPortNames(); //........................Получене количества возможных SerialPorts


            try
            {
                comboBox1.Items.AddRange(ports); //..................................... Добавление доступных портов в комбобокс
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
            comboBox1.Click += new EventHandler(combobox1_click_Hanlder);
            XML_STRUCT_COMMAND_READ();
            XML_STRUCT_ANSWER_READ();

            
        }

        //.........................................................................Заполнение комбобоксов данными по умолчанию
        private void Initialize_combobox()
        {
            comboBox1.Text = default_PortName;
            comboBox2.Text = default_BaudRate.ToString();
            comboBox3.Text = default_Parity.ToString();
            comboBox4.Text = default_DataBits.ToString();
            comboBox5.Text = default_StopBits.ToString();
        }

        private void combobox1_click_Hanlder(object sender , EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(ports);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Text = comboBox1.SelectedItem.ToString();
            if (!Press_COM)
            {
                if (comboBox1.SelectedItem != null)

                    _serialPort.PortName = comboBox1.SelectedItem.ToString();
                // textBox1.Text += "PortName :" + _serialPort.PortName + Environment.NewLine;
            }
            else
                textBox1.Text += "Нужно закрыть :" + _serialPort.PortName + Environment.NewLine;
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
        //---------------------------------------------------------XML-интерфейс----------------------------------------------------

        private void XML_STRUCT_COMMAND_READ()
        {
            // ..................................................................Создание списка кнопок , основанных на раннее объявленном методе


            XmlDocument xml = new XmlDocument();
            xml.Load("STRUCT_COMMAND.xml");
            XmlElement element = xml.DocumentElement;


            // int i = 0, j = 0, k = 0;

            //.......................................................................Считывание документа XML
            foreach (XmlNode xnode in element)
            {
                USER_struct user_struct = new USER_struct();
                string name = xnode.Attributes.GetNamedItem("name").Value;
                user_struct.name = name;

                if (xnode.Attributes.GetNamedItem("crc").Value == "y")
                    user_struct.crc = true;
                else
                    user_struct.crc = false;

                user_struct.type = xnode.Attributes.GetNamedItem("type").Value;

                if (xnode.Attributes.GetNamedItem("count").Value == "y")
                    user_struct.count = true;
                else
                    user_struct.count = false;


                if (xnode.ChildNodes.Count == 0)
                {
                    if ((xnode.Attributes.GetNamedItem("var").Value != "command") & (xnode.Attributes.GetNamedItem("var").Value != "iterable") & (xnode.Attributes.GetNamedItem("var").Value != "КС"))
                    {
                        user_struct.var = Convert.ToByte(xnode.Attributes.GetNamedItem("var").Value, 16);
                    }
                    else
                    {
                        user_struct.var_string = xnode.Attributes.GetNamedItem("var").Value;
                    }
                }
                else
                {
                    byte temp_var = 0;
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "bit")
                        {
                            if (childnode.Attributes.GetNamedItem("var").Value == "1")
                                temp_var |= (byte)(1 << int.Parse(childnode.Attributes.GetNamedItem("index").Value));
                        }
                    }
                    user_struct.var = temp_var;
                }
                STRUCT_COMMAND.Add(user_struct);
            }
               
        }

        private void XML_STRUCT_ANSWER_READ()
        {
            // ..................................................................Создание списка кнопок , основанных на раннее объявленном методе


            XmlDocument xml = new XmlDocument();
            xml.Load("STRUCT_ANSWER.xml");
            XmlElement element = xml.DocumentElement;

            
            // USER_struct user_struct = new USER_struct();
            // int i = 0, j = 0, k = 0;

            //.......................................................................Считывание документа XML
            foreach (XmlNode xnode in element)
            {
                switch (xnode.Name)
                {
                    case "answer":
                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            USER_struct user_struct = new USER_struct();
                            string name = childnode.Attributes.GetNamedItem("name").Value;
                            user_struct.name = name;

                            if (childnode.Attributes.GetNamedItem("crc").Value == "y")
                                user_struct.crc = true;
                            else
                                user_struct.crc = false;

                            user_struct.type = childnode.Attributes.GetNamedItem("type").Value;

                            if (childnode.Attributes.GetNamedItem("count").Value == "y")
                                user_struct.count = true;
                            else
                                user_struct.count = false;


                            if (childnode.ChildNodes.Count == 0)
                            {
                                if ((childnode.Attributes.GetNamedItem("var").Value != "command") & (childnode.Attributes.GetNamedItem("var").Value != "iterable") & (childnode.Attributes.GetNamedItem("var").Value != "КС"))
                                {
                                    user_struct.var = Convert.ToByte(childnode.Attributes.GetNamedItem("var").Value, 16);
                                }
                                else
                                {
                                    user_struct.var_string = childnode.Attributes.GetNamedItem("var").Value;
                                }
                            }
                            else
                            {
                                byte temp_var = 0;
                                foreach (XmlNode node in childnode.ChildNodes)
                                {
                                    if (node.Name == "bit")
                                    {
                                        if (node.Attributes.GetNamedItem("var").Value == "1")
                                            temp_var |= (byte)(1 << int.Parse(node.Attributes.GetNamedItem("index").Value));
                                    }
                                }
                                user_struct.var = temp_var;
                            }
                            STRUCT_ANSWER.Add(user_struct);
                        }
                        break;
                    case "telemetry":
                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            USER_struct_telemetry user_struct = new USER_struct_telemetry();
                            string name = childnode.Attributes.GetNamedItem("name").Value;
                            user_struct.name = name;

                            if (childnode.Attributes.GetNamedItem("crc").Value == "y")
                                user_struct.crc = true;
                            else
                                user_struct.crc = false;

                            if (childnode.Attributes.GetNamedItem("type") != null)
                                user_struct.type = childnode.Attributes.GetNamedItem("type").Value;

                            if (childnode.Attributes.GetNamedItem("count").Value == "y")
                                user_struct.count = true;
                            else
                                user_struct.count = false;


                            if (childnode.ChildNodes.Count == 0)
                            {
                                if ((childnode.Attributes.GetNamedItem("var").Value != "command") & (childnode.Attributes.GetNamedItem("var").Value != "iterable") & (childnode.Attributes.GetNamedItem("var").Value != "КС"))
                                {
                                    user_struct.var = Convert.ToByte(childnode.Attributes.GetNamedItem("var").Value, 16);
                                }
                                else
                                {
                                    user_struct.var_string = childnode.Attributes.GetNamedItem("var").Value;
                                }
                            }
                            else
                            {
                                byte temp_var = 0;
                                foreach (XmlNode node in childnode.ChildNodes)
                                {
                                    USER_tlm_data user_tlm = new USER_tlm_data();
                                    if (node.Name == "byte")
                                    {
                                        foreach (XmlNode nd in node.ChildNodes)
                                        {
                                            
                                            if ((nd.Name == "bit") & (nd.Attributes.GetNamedItem("type") != null))
                                            {
                                                USER_Bool_tlm_data user_bool = new USER_Bool_tlm_data();

                                                if (nd.Attributes.GetNamedItem("name") != null)
                                                    user_bool.name = nd.Attributes.GetNamedItem("name").Value;

                                                if (nd.Attributes.GetNamedItem("index") != null)
                                                    user_bool.index = int.Parse(nd.Attributes.GetNamedItem("index").Value);

                                                if (nd.Attributes.GetNamedItem("index_flag") != null)
                                                    user_bool.index_flag = int.Parse(nd.Attributes.GetNamedItem("index_flag").Value);

                                                user_tlm.user_bool_tlm_data.Add(user_bool);
 
                                            }
                                        }
                                    }else  
                                            if ((node.Name == "data") & (node.Attributes.GetNamedItem("type") != null))
                                            {
                                                    USER_uint_tlm_data user_uint_tlm = new USER_uint_tlm_data();
                                                   // USER_Bool_tlm_data user_bool = new USER_Bool_tlm_data();

                                                    if (node.Attributes.GetNamedItem("name") != null)
                                                        user_tlm.name = node.Attributes.GetNamedItem("name").Value;
                                               
                                                     if (node.Attributes.GetNamedItem("min_val") != null)
                                                     {
                                                        string temp = node.Attributes.GetNamedItem("min_val").Value;  
                                                        user_uint_tlm.min_var = convert_str_int(temp);
                                                     }

                                                    if (node.Attributes.GetNamedItem("max_val") != null)
                                                    {
                                                        string temp = node.Attributes.GetNamedItem("max_val").Value;
                                                        user_uint_tlm.max_var = convert_str_int(temp);
                                                    }

                                                    if (node.Attributes.GetNamedItem("type").Value == "uint8")
                                                        user_tlm.size = 1;
                                                    else
                                                    if (node.Attributes.GetNamedItem("type").Value == "uint16")
                                                        user_tlm.size = 2;

                                                     if (node.Attributes.GetNamedItem("index_graph") != null)
                                                        user_tlm.index_graph = int.Parse(node.Attributes.GetNamedItem("index_graph").Value);

                                                     if (node.Attributes.GetNamedItem("division") != null)
                                                        user_tlm.division = int.Parse(node.Attributes.GetNamedItem("division").Value);

                                                     if (node.Attributes.GetNamedItem("mult") != null)
                                                        user_tlm.mult = int.Parse(node.Attributes.GetNamedItem("mult").Value);

                                        user_tlm.user_uint_tlm_data.Add(user_uint_tlm);
                                                //user_tlm.user_bool_tlm_data.Add(user_bool);

                                            }else

                                                if (node.Name == "temp")
                                                {
                                                    USER_temp_tlm_data user_temp_tlm = new USER_temp_tlm_data();
                                                    // USER_Bool_tlm_data user_bool = new USER_Bool_tlm_data();

                                                        if (node.Attributes.GetNamedItem("name") != null)
                                                            user_tlm.name = node.Attributes.GetNamedItem("name").Value;

                                                        if (node.Attributes.GetNamedItem("min_plus_val") != null)
                                                        {
                                                            string temp = node.Attributes.GetNamedItem("min_plus_val").Value;  
                                                             user_temp_tlm.min_plus_var = convert_str_int(temp);
                                                        }

                                                        if (node.Attributes.GetNamedItem("max_plus_val") != null)
                                                        {
                                                            string temp = node.Attributes.GetNamedItem("max_plus_val").Value;  
                                                             user_temp_tlm.max_plus_var = convert_str_int(temp);
                                                        }

                                                        if (node.Attributes.GetNamedItem("min_minus_val") != null)
                                                        {
                                                            string temp = node.Attributes.GetNamedItem("min_minus_val").Value;
                                                            user_temp_tlm.min_minus_var = convert_str_int(temp);
                                                        }

                                                        if (node.Attributes.GetNamedItem("max_minus_val") != null)
                                                        {
                                                            string temp = node.Attributes.GetNamedItem("max_minus_val").Value;
                                                            user_temp_tlm.max_minus_var = convert_str_int(temp);
                                                        }
                                                        user_tlm.user_temp_tlm_data.Add(user_temp_tlm);

                                                        if (node.Attributes.GetNamedItem("type").Value == "int8")
                                                            user_tlm.size = 1;
                                                        else
                                                        if (node.Attributes.GetNamedItem("type").Value == "int16")
                                                            user_tlm.size = 2;

                                                        if (node.Attributes.GetNamedItem("index_graph") != null)
                                                            user_tlm.index_graph = int.Parse(node.Attributes.GetNamedItem("index_graph").Value);

                                                        if (node.Attributes.GetNamedItem("division") != null)
                                                        user_tlm.division = int.Parse(node.Attributes.GetNamedItem("division").Value);

                                                        if (node.Attributes.GetNamedItem("mult") != null)
                                                            user_tlm.mult = int.Parse(node.Attributes.GetNamedItem("mult").Value);
                                    }
                                    user_struct.user_tlm_data.Add(user_tlm);
                                    //if (node.Attributes.GetNamedItem("var").Value == "1")
                                    //    temp_var |= (byte)(1 << int.Parse(node.Attributes.GetNamedItem("index").Value));
                                }
                                } 
                            STRUCT_TELEMETRY.Add(user_struct);
                        }
                        break;
                }

            }   

        }

        private int convert_str_int(string str)
        {
            if (str.Length % 2 != 0)
                str = "0" + str;

            int temp = 0;

            for (int len = str.Length - 1; len > 0; len -=2)
            {
                string st2 = new string(new char[] { str[len-1], str[len] });
                temp |= Convert.ToByte(st2, 16) <<  ((str.Length - len - 1)*4);
            }
            return temp;
        }
        // .........................................................Функция считывания XML документа и создания кнопок на основе считанных данных
        private void XML_COMMAND_READ()
        {
            // ..................................................................Создание списка кнопок , основанных на раннее объявленном методе


            XmlDocument xml = new XmlDocument();
            xml.Load(filePath_XML);
            XmlElement element = xml.DocumentElement;

            // ........................................................................Очистка стилей колонок и строк таблицы
            tableLayoutPanel3.ColumnStyles.Clear();
            tableLayoutPanel3.RowStyles.Clear();

            tableLayoutPanel4.ColumnStyles.Clear();
            tableLayoutPanel4.RowStyles.Clear();

            //........................................................................ Создание новых стилей колонок и таблицы
            //tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 50));
            //tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.AutoSize, 50));
            
            //tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 50));
            //tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.AutoSize, 50));


            int i = 0, j = 0, k = 0;
            tableLayoutPanel3.ColumnCount = element.Attributes.Count;
            tableLayoutPanel3.RowCount = element.ChildNodes.Count;
            //.......................................................................Считывание документа XML
            foreach (XmlNode xnode in element)
            {
                //...............................................................Добавление новой колонки
                
                    //tableLayoutPanel3.ColumnCount += 1;
                //..............................................................Считывание дочерних элементов
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    
                        //tableLayoutPanel3.RowCount += 1;
                    //--------------------------------------------------------------------------- VAR
                    switch (childnode.Name)
                    {
                        case "send_const":
                            {
                                //.............................................................Добавление в список новых экземпляров ранее созданного метода
                                but.Add(new Button_user());

                                //.............................................................Заполнение атрибутов созданных кнопок
                                but[but.Count - 1].Text = childnode.Attributes.GetNamedItem("name").Value;

                                string temp_string_var = childnode.Attributes.GetNamedItem("var").Value;
                                if (!((temp_string_var == "") | (temp_string_var == null)))
                                    but[but.Count - 1].var = Convert.ToByte(temp_string_var, 16);
                                
                                int count_add = -1;


                               

                                TableLayoutPanel table_second = new TableLayoutPanel();
                                table_second.Dock = DockStyle.Fill;
                                table_second.ColumnCount = 2;
                                table_second.ColumnStyles.Add(new ColumnStyle());
                                table_second.ColumnStyles.Add(new ColumnStyle());

                                table_second.ColumnStyles[0].SizeType = SizeType.Percent;
                                table_second.ColumnStyles[0].Width = 60;

                                table_second.ColumnStyles[1].SizeType = SizeType.Percent;
                                table_second.ColumnStyles[1].Width = 40;
                                table_second.AutoSize = true;

                                if (childnode.Attributes.GetNamedItem("count_add") != null)
                                {
                                    count_add = int.Parse(childnode.Attributes.GetNamedItem("count_add").Value);
                                    TableLayoutPanel table_third = new TableLayoutPanel();
                                    table_third.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                                    table_third.Dock = DockStyle.Top;
                                    table_third.AutoSize = true;
                                    table_third.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                                    
                                    
                                    for (int index = 0; index <= count_add; index++)
                                    {
                                        Label label = new Label();
                                        if (childnode.Attributes.GetNamedItem("name_var_" + index.ToString()) != null)
                                        {
                                            label.Dock = DockStyle.Fill;
                                            label.Text = childnode.Attributes.GetNamedItem("name_var_" + index.ToString()).Value;
                                            //label.Font = new Font(label.Font);


                                            table_third.Controls.Add(label);
                                            
                                            if (childnode.Attributes.GetNamedItem("type_var_" + index.ToString()) != null)
                                            {
                                                string type_temp = childnode.Attributes.GetNamedItem("type_var_" + index.ToString()).Value;
                                                switch (type_temp)
                                                {
                                                    case "bool":
                                                        CheckedListBox checkBox = new CheckedListBox();
                                                        for (int sub_index = 0; sub_index < 2; sub_index++)
                                                        {
                                                            string temp_str = "name_var_" + index.ToString() + "_" + sub_index.ToString();
                                                            if (childnode.Attributes.GetNamedItem(temp_str) != null)
                                                            {
                                                                string temp_sub_string = childnode.Attributes.GetNamedItem(temp_str).Value;

                                                                checkBox.Items.AddRange(new object[] { temp_sub_string });

                                                                if (sub_index == 0)
                                                                    checkBox.SetItemCheckState(sub_index, CheckState.Checked);
                                                            }
                                                        }
                                                        checkBox.Dock = DockStyle.Fill;
                                                        checkBox.SetBounds(0, 0, 10, 40);
                                                        checkBox.ItemCheck += new ItemCheckEventHandler(this.CheckBox_Handler);
                                                        table_third.Controls.Add(checkBox);
                                                        
                                                        but[but.Count - 1].checkBox.Add(checkBox);
                                                        but[but.Count - 1].str_type.Add("bool");
                                                        break;

                                                    case "uint8":
                                                        string temp_min_str = "min_var_" + index.ToString();
                                                        string temp_max_str = "max_var_" + index.ToString();
                                                        string temp_basic_str = "basic_var_" + index.ToString();

                                                        if ((childnode.Attributes.GetNamedItem(temp_min_str) != null) & (childnode.Attributes.GetNamedItem(temp_max_str) != null) & (childnode.Attributes.GetNamedItem(temp_basic_str) != null))
                                                        {
                                                            int min = convert_str_int( childnode.Attributes.GetNamedItem(temp_min_str).Value);
                                                            int max = convert_str_int(childnode.Attributes.GetNamedItem(temp_max_str).Value);
                                                            int basic = convert_str_int(childnode.Attributes.GetNamedItem(temp_basic_str).Value);

                                                            NumericUpDown value = new NumericUpDown();

                                                            if (basic < min)
                                                                min = basic;

                                                            if (basic > max)
                                                                max = basic;

                                                            value.Minimum = min;
                                                            value.Maximum = max;
                                                            value.Value = basic;

                                                            value.Dock = DockStyle.Fill;
                                                            table_third.Controls.Add(value);

                                                            but[but.Count - 1].uint8.Add(value);
                                                            but[but.Count - 1].str_type.Add("uint8");
                                                        }
                                                        break;
                                                    case "int":
                                                        string temp_min_str_int = "min_var_" + index.ToString();
                                                        string temp_max_str_int = "max_var_" + index.ToString();
                                                        string temp_basic_str_int = "basic_var_" + index.ToString();
                                                        if ((childnode.Attributes.GetNamedItem(temp_min_str_int) != null) & (childnode.Attributes.GetNamedItem(temp_max_str_int) != null) & (childnode.Attributes.GetNamedItem(temp_basic_str_int) != null))
                                                        {

                                                            string min_str = childnode.Attributes.GetNamedItem(temp_min_str_int).Value;
                                                            string max_str = childnode.Attributes.GetNamedItem(temp_max_str_int).Value;
                                                            string basic_str = childnode.Attributes.GetNamedItem(temp_basic_str_int).Value;

                                                            if (min_str.Length % 2 != 0)
                                                                min_str = "0" + min_str;

                                                            if (max_str.Length % 2 != 0)
                                                                max_str = "0" + max_str;

                                                            if (basic_str.Length % 2 != 0)
                                                                basic_str ="0" + basic_str;

                                                            int temp_max = 0;
                                                            int temp_min = 0;
                                                            int temp_basic = 0;
                                                            NumericUpDown value = new NumericUpDown();
                                                            temp_min = convert_str_int(min_str);
                                                            temp_max = convert_str_int(max_str);
                                                            temp_basic = convert_str_int(basic_str);

                                                            if (temp_basic < temp_min)
                                                                temp_min = temp_basic;

                                                            if (temp_basic > temp_max)
                                                                temp_max = temp_basic;

                                                            value.Minimum = temp_min;
                                                            value.Maximum = temp_max;
                                                            value.Value = temp_basic;


                                                            value.Dock = DockStyle.Fill;
                                                            table_third.Controls.Add(value);

                                                            but[but.Count - 1].int32.Add(value);
                                                            but[but.Count - 1].str_type.Add("int");
                                                        }
                                                        break;

                                                }
                                            }
                                        }
                                    }
                                    table_second.Controls.Add(table_third, 1, 0);
                                }
                                but[but.Count - 1].press_flag = false;
                               // but[but.Count - 1].Dock = DockStyle.Fill;
                                but[but.Count - 1].AutoSize  = false;
                                but[but.Count - 1].Width  = 150;
                                but[but.Count - 1].Height  = 30;
                                but[but.Count - 1].ForeColor  = Color.Black;

                                if (but[but.Count - 1].Text == "TELEMETRY")
                                    but_telemetry = but[but.Count - 1];

                                // ..............................................................Создание нового события при нажатии кнопки
                                but[but.Count - 1].Click  += new System.EventHandler(this.button_user_Click);

                                //..............................................................Добавление в таблицу созданной кнопки
                                table_second.Controls.Add(but[but.Count - 1], 0, 0);
                                
                                tableLayoutPanel3.Controls.Add(table_second, j, i);
                                break;
                            }
                        case "send_var":
                            {
                                k++;
                                tableLayoutPanel4.ColumnCount += 2;
                                //.............................................................Добавление в список новых экземпляров ранее созданного метода
                                slider.Add(new Slider_user());

                                //.............................................................Заполнение атрибутов созданных кнопок
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

                                slider[slider.Count - 1].Orientation = System.Windows.Forms.Orientation.Vertical;
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

                                take_current.Text = "Отправить";
                                //take_current.header = default_header;
                                //take_current.bytescol = 6;

                                take_current.Click  += new System.EventHandler(button_take_current_Click);

                                label_slider.Text = slider[slider.Count - 1].name;
                                label_slider.BorderStyle = BorderStyle.None;

                                max_slider.Text = slider[slider.Count - 1].var_b.ToString();
                                max_slider.BorderStyle = BorderStyle.None;

                                min_slider.Text = slider[slider.Count - 1].var_a.ToString();
                                min_slider.BorderStyle = BorderStyle.None;

                                Value_slider.BorderStyle = BorderStyle.None;

                                Value_slider.Text = "Значение: " + 0;

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


        private void CheckBox_Handler(object sender ,   EventArgs e)
        {
            if (sender != null)
            {
                CheckedListBox checkBox = sender as CheckedListBox;
                try
                {
                    int index = checkBox.SelectedIndex;
                    checkBox.ItemCheck -= new ItemCheckEventHandler(this.CheckBox_Handler);
                    for (int i = 0; i < checkBox.Items.Count ; i++)
                    {
                        if (i != index)
                        {
                            checkBox.SetItemCheckState(i, CheckState.Unchecked);
                            //checkBox.Se = checkBox.Items[i];
                        }
                    }
                    checkBox.ItemCheck += new ItemCheckEventHandler(this.CheckBox_Handler);
                }
                catch { }
            }
        }


        //// .........................................................Функция считывания XML документа и создания кнопок на основе считанных данных
        //private void XML_Read()
        //{
        //    // ..................................................................Создание списка кнопок , основанных на раннее объявленном методе
            

        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(filePath_XML);
        //    XmlElement element = xml.DocumentElement;

        //    // ........................................................................Очистка стилей колонок и строк таблицы
        //    tableLayoutPanel3.ColumnStyles.Clear();
        //    tableLayoutPanel3.RowStyles.Clear();

        //    tableLayoutPanel4.ColumnStyles.Clear();
        //    tableLayoutPanel4.RowStyles.Clear();

        //    //........................................................................ Создание новых стилей колонок и таблицы
        //    tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 50));
        //    tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.AutoSize, 50));

        //    tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 50));
        //    tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.AutoSize, 50));


        //    int i = 0, j = 0, k = 0;

        //    //.......................................................................Считывание документа XML
        //    foreach (XmlNode xnode in element)
        //    {
        //        //...............................................................Добавление новой колонки
        //        if (xnode.Name != "var")
        //            tableLayoutPanel3.ColumnCount += 1;
        //        //..............................................................Считывание дочерних элементов
        //        foreach (XmlNode childnode in xnode.ChildNodes)
        //        {
        //            if (xnode.Name != "var")
        //                tableLayoutPanel3.RowCount += 1;
        //            //--------------------------------------------------------------------------- VAR
        //            switch (childnode.Name)
        //            {
        //                case "send_const":
        //                    {
        //                        //.............................................................Добавление в список новых экземпляров ранее созданного метода
        //                        but.Add(new Button_user());

        //                        //.............................................................Заполнение атрибутов созданных кнопок
        //                        but[but.Count - 1].Text = childnode.Attributes.GetNamedItem("name").Value;

        //                        string temp_string_header = childnode.Attributes.GetNamedItem("header").Value;

        //                        //--------------------------------------------------------------------------- HEADER
        //                        if (!((temp_string_header == "") | (temp_string_header == null)))
        //                            but[but.Count - 1].header = Convert.ToByte(temp_string_header, 16);

        //                        //--------------------------------------------------------------------------- BYTESCOL
        //                        if (childnode.Attributes.GetNamedItem("bytescol").Value == "y")
        //                        {
        //                            but[but.Count - 1].bytescol_flag = true;
        //                            but[but.Count - 1].bytescol = 4;
        //                        }
        //                        else
        //                            but[but.Count - 1].bytescol_flag = false;

        //                        string temp_string_var = childnode.Attributes.GetNamedItem("var").Value;
        //                        if (!((temp_string_var == "") | (temp_string_var == null)))
        //                            but[but.Count - 1].var = Convert.ToByte(temp_string_var, 16);

        //                        if (childnode.Attributes.GetNamedItem("control_sum").Value == "y")
        //                        {
        //                            but[but.Count - 1].control_sum_flag = true;
        //                            but[but.Count - 1].control_sum = crc_out(new byte[] { but[but.Count - 1].header, but[but.Count - 1].bytescol, but[but.Count - 1].var });
        //                        }
        //                        else
        //                            but[but.Count - 1].control_sum_flag = false;

        //                        but[but.Count - 1].press_flag = false;

        //                        but[but.Count - 1].AutoSize  = false;
        //                        but[but.Count - 1].Width  = 150;
        //                        but[but.Count - 1].Height  = 30;
        //                        but[but.Count - 1].ForeColor  = Color.Black;

        //                        // ..............................................................Создание нового события при нажатии кнопки
        //                        but[but.Count - 1].Click  += new System.EventHandler(this.button_user_Click);

        //                        //..............................................................Добавление в таблицу созданной кнопки
        //                        tableLayoutPanel3.Controls.Add(but[but.Count - 1], j, i);

        //                        break;
        //                    }
        //                case "send_var":
        //                    {
        //                        k++;
        //                        tableLayoutPanel4.ColumnCount += 2;
        //                        //.............................................................Добавление в список новых экземпляров ранее созданного метода
        //                        slider.Add(new Slider_user());

        //                        //.............................................................Заполнение атрибутов созданных кнопок
        //                        slider[slider.Count - 1].name = childnode.Attributes.GetNamedItem("name").Value;

        //                        string temp_string_header = childnode.Attributes.GetNamedItem("header").Value;

        //                        //--------------------------------------------------------------------------- HEADER
        //                        if (!((temp_string_header == "") | (temp_string_header == null)))
        //                            slider[slider.Count - 1].header = Convert.ToByte(temp_string_header, 16);

        //                        //--------------------------------------------------------------------------- BYTESCOL
        //                        if (childnode.Attributes.GetNamedItem("bytescol").Value == "y")
        //                        {
        //                            slider[slider.Count - 1].bytescol_flag = true;
        //                            slider[slider.Count - 1].bytescol = 6;
        //                        }
        //                        else
        //                            slider[slider.Count - 1].bytescol_flag = false;

        //                        string temp_string_var = childnode.Attributes.GetNamedItem("var_a").Value;
        //                        if (!((temp_string_var == "") | (temp_string_var == null)))
        //                            slider[slider.Count - 1].var_a = Convert.ToByte(temp_string_var, 16);

        //                        temp_string_var = childnode.Attributes.GetNamedItem("var_b").Value;
        //                        if (!((temp_string_var == "") | (temp_string_var == null)))
        //                            slider[slider.Count - 1].var_b = Convert.ToByte(temp_string_var, 16);

        //                        if (childnode.Attributes.GetNamedItem("control_sum").Value == "y")
        //                        {
        //                            slider[slider.Count - 1].control_sum_flag = true;
        //                        }
        //                        else
        //                            slider[slider.Count - 1].control_sum_flag = false;

        //                        slider[slider.Count - 1].Orientation = Orientation.Vertical;
        //                        //slider[slider.Count - 1].Maximum = slider[slider.Count - 1].var_b * 10;
        //                        //slider[slider.Count - 1].Minimum = slider[slider.Count - 1].var_a;

        //                        slider[slider.Count - 1].Maximum = 100;
        //                        slider[slider.Count - 1].Minimum = 0;

        //                        slider[slider.Count - 1].Height = 370;
        //                        slider[slider.Count - 1].Width = 50;

        //                        TextBox label_slider = new TextBox();
        //                        TextBox max_slider = new TextBox();
        //                        TextBox min_slider = new TextBox();
        //                        Button_user take_current = new Button_user();

        //                        take_current.Text = "Отправить";
        //                        take_current.header = default_header;
        //                        take_current.bytescol = 6;

        //                        take_current.Click  += new System.EventHandler(button_take_current_Click);

        //                        label_slider.Text = slider[slider.Count - 1].name;
        //                        label_slider.BorderStyle = BorderStyle.None;

        //                        max_slider.Text = slider[slider.Count - 1].var_b.ToString();
        //                        max_slider.BorderStyle = BorderStyle.None;

        //                        min_slider.Text = slider[slider.Count - 1].var_a.ToString();
        //                        min_slider.BorderStyle = BorderStyle.None;

        //                        Value_slider.BorderStyle = BorderStyle.None;

        //                        Value_slider.Text = "Значение: " + 0;

        //                        Slider_val = (byte)slider[slider.Count - 1].Value;

        //                        tableLayoutPanel4.RowCount += 3;


        //                        tableLayoutPanel4.Controls.Add(max_slider, k, 0);
        //                        tableLayoutPanel4.Controls.Add(slider[slider.Count - 1], k, 1);
        //                        tableLayoutPanel4.Controls.Add(min_slider, k, 2);
        //                        tableLayoutPanel4.Controls.Add(label_slider, k, 3);
        //                        tableLayoutPanel4.Controls.Add(Value_slider, k + 1, 2);
        //                        tableLayoutPanel4.Controls.Add(take_current, k + 1, 3);


        //                        slider[slider.Count - 1].Scroll += new System.EventHandler(slider_handler);
        //                        break;
        //                    }
        //                default:
        //                    {
        //                        but[but.Count - 1].var = (byte)0x00;
        //                        break;
        //                    }
        //            }

        //            i++;
        //        }
        //        i = 0;
        //        j++;
        //    }
        //}
        // Кнопка Загрузить
        private void button5_Click(object sender, EventArgs e)
        {
            tableLayoutPanel3.Controls.Clear();
            tableLayoutPanel4.Controls.Clear();
            XML_COMMAND_READ();
            //button_user_Click(but[but.Count - 1], new EventArgs());
        }

        // Кнопка ..
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
        // Кнопка отправки уставки тока
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
                        SetText_ETH("Ошибка с " + IP_ETH.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    SetText_ETH("Нет подключения" +  Environment.NewLine);
                    SetText_COM("Нет подключения" +  Environment.NewLine);
                }
            //}

        }
        // Изменение значения отправляемого тока
        private void slider_handler(object sender, EventArgs e)
        {
            Slider_val = (int)((float)(sender as TrackBar).Value);
            //(sender as Slider_user).var = new byte[] {(byte)0xCC, (byte)(Slider_val >> 8), (byte)Slider_val};


            Value_slider.Text = "Значение: " +  ((float)(sender as TrackBar).Value);

            List<byte> data = new List<byte>();
            if (Press_COM)
            {
                data.Add((byte)(Slider_val >> 8)); 
                data.Add((byte)(Slider_val & 0xFF));
                byte[] temp = combine_message(data.ToArray());
                //SetText_COM("Отправлено: " + BitConverter.ToString(temp.ToArray()).Replace("-", " ") + Environment.NewLine);
                _serialPort.Write(temp, 0, temp.Length);
            }

        }
        //..........................................................................Создание обработчика нажатия процедурно созданной кнопки
        private async void button_user_Click(object sender, EventArgs e)
        {
            Button_user but = (Button_user)sender;
            if (Press_COM)
            {
                try
                {
                    List<byte> data = new List<byte>();
                    if (Press_COM)
                    {
                        int ind_bool = 0;
                        int ind_uint8 = 0;
                        int ind_int = 0;
                        data.Add(but.var);
                        Dictionary<string, int> dict = new Dictionary<string, int>()
                        {
                            ["bool"] = 0,
                            ["uint8"] = 0,
                            ["int"] = 0
                        };
                        foreach (string str in (but.str_type))
                        {
                            if (str == "bool")
                            {
                                int index = 0;
                                dict.TryGetValue("bool", out index);

                                if (but.checkBox[index].GetItemChecked(0))
                                    data.Add(0);

                                if (but.checkBox[index].GetItemChecked(1))
                                    data.Add(1);
                                ind_bool++;
                                dict["bool"] = ind_bool;
                            }

                            if (str == "uint8")
                            {
                                int index = 0;
                                dict.TryGetValue("uint8", out index);

                                byte[] new_ar = BitConverter.GetBytes((int)but.uint8[index].Value);
                                List<byte> new_ar_list = new_ar.ToList();
                               
                                byte temp_byte = 0;
                                int ind = new_ar_list.Count - 1;
                                do
                                {
                                    temp_byte = new_ar_list[ind];

                                    if (new_ar_list[ind] == 0)
                                        new_ar_list.RemoveAt(ind);

                                    ind--;

                                } while ((temp_byte == 0) & (ind > 0));

                                new_ar_list.Reverse();
                                data.AddRange(new_ar_list);

                                ind_uint8++;
                                dict["uint8"] = ind_uint8;
                            }

                            if (str == "int")
                            {
                                int index = 0;
                                dict.TryGetValue("int", out index);

                                byte[] new_ar = BitConverter.GetBytes((int)but.int32[index].Value);
                                List<byte> new_ar_list = new_ar.ToList();

                                byte temp_byte = 0;
                                int ind = new_ar_list.Count - 1;
                                do
                                {
                                    temp_byte = new_ar_list[ind];

                                    if (new_ar_list[ind] == 0)
                                        new_ar_list.RemoveAt(ind);

                                    ind--;

                                } while ((temp_byte == 0) & (ind > 0));
                                new_ar_list.Reverse();
                                data.AddRange(new_ar_list);

                                ind_int++;
                                dict["int"] = ind_int;
                            }
                        }
                        List <byte> out_word = new List <byte>();
                        int index_byte = 0;
                        int index_Count = -1;
                        int index_CRC = - 1;
                        
                        List <byte> CRC = new List <byte>();
                        int count = 0;
                        bool fl_count_crc = false;
                        List<string> NAME = new List<string>();
                        foreach (USER_struct struc in STRUCT_COMMAND)
                        {
                            NAME.Add(struc.name);
                            if (struc.count == true)
                                count++;

                            if (struc.var_string == null)
                            {
                                out_word.Add(struc.var);
                                if (struc.crc == true)
                                    CRC.Add(struc.var);
                            }
                            else
                            {
                                if (struc.var_string == "command")
                                {
                                    if (struc.crc == true)
                                        CRC.AddRange(data);
                                    count+= data.Count - 1;
                                    out_word.AddRange(data);
                                    index_byte += data.Count - 2;
                                }

                                if (struc.var_string == "КС")
                                    index_CRC = index_byte;

                                if (struc.var_string == "iterable")
                                {
                                    fl_count_crc = struc.crc;
                                    index_Count = index_byte;
                                }
                            }

                            index_byte++;
                        }

                        if (fl_count_crc)
                            CRC.Add((byte)count);

                        if (index_CRC != -1)
                            out_word.Insert(index_CRC, crc_out(CRC.ToArray()));

                        if (index_Count != -1)
                            out_word.Insert(index_Count, (byte) count);

                        _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
                        SetText_COM(but.Text + ": " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") +  Environment.NewLine);
                        //SetText_COM("Отправлено: ");
                        //foreach (string str in NAME)
                        //{
                        //    SetText_COM(str + " | ");
                        //}
                        //SetText_COM(Environment.NewLine);
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
                    SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
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
                        SetText_ETH("Ошибка с " + IP_ETH.ToString() + Environment.NewLine);
                    }
                }
                else
                {
                    SetText_ETH("Нет подключения" +  Environment.NewLine);
                    SetText_COM("Нет подключения" +  Environment.NewLine);
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
               // graph.Refresh_chart(temp1,temp2, time);
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
        //--------------------------------------------------Коэффициенты_ПИД-------------------------------------
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
                    SetText_COM("Отправлено: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
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
                    SetText_COM("Отправлено: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
                }
            }
        }
        //---------------------------------------------------Циклограмма-------------------------------------
        delegate void SetCyclCallback(bool flag , CheckBox box);
        private void SetCycl(bool flag, CheckBox box)
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            if (this.textBox1.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                SetCyclCallback d = new SetCyclCallback(SetCycl);
                this.Invoke(d, new object[] { flag , box });
            }
            // ...иначе все по старинке
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
                long millsec = 0;
                UInt64 sec = 0;
                UInt64 min = 0;
                UInt64 hour = 0;

                string sec_str = "";
                string min_str = "";
                string hour_str = "";

                millsec = ((DateTime.Now.Ticks - _TIME_) / 10000);
                sec = (UInt64)millsec /1000 % 60;
                min = ((UInt64)millsec / 1000 / 60) % 60;
                hour = (UInt64)millsec / 1000 / 60  /60;
                //SetText_COM("NowTicks " + DateTime.Now.Ticks  + Environment.NewLine);
                //SetText_COM("--_TIME_: " + _TIME_ + Environment.NewLine);
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
        public async void timer2_Tick(object sender, EventArgs e)
        {
            //.........................................Команда запроса данных
            // graph.Clean_chart();
            
           // graph.Refresh_chart(Math.Sin(temp_time)*20,Math.Cos(temp_time)*10, temp_time);
            //SetText_COM(graph.dataX.Last().ToString() + Environment.NewLine);
            //graph.Refresh_chart();
            /*if (Press_COM)
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

                    
                    //SetText_COM("Отправлено: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    //SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
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
                   // SetText_ETH("Ошибка с " + IP_ETH.ToString() + Environment.NewLine);
                }
            }
            else
            {
                //SetText_ETH("Нет подключения" +  Environment.NewLine);
               // SetText_COM("Нет подключения" +  Environment.NewLine);
            }*/
        }

        private void Cyclogramm_button_Click(object sender, EventArgs e)
        {
            timer2.Interval = 1000;
            timer2.Enabled = true;

            if (Press_COM || Press_ETH)
            {
                if (!timer2.Enabled)
                {
                    CLEAN_Cycle();
                    _TIME_ = ((uint)DateTime.Now.Ticks);
                    TIME_CLEAN();
                    timer2.Interval = 1000;
                    timer2.Enabled = true;
                    Cyclogramm_button.Text = "Остановить";
                }
                else
                {
                    timer2.Enabled = false;
                    Cyclogramm_button.Text = "Отслеживание";
                }
            }
        }

        //------------------------------------------------------------Найстрока времени работы анода--------------------------------
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
                    SetText_COM("Отправлено: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
                }
                catch
                {
                    SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
                }
            }
        }

        //-----------------------------------------------------------Второе окно----------------------------------------------------
        public async void timer1_Tick(object sender, EventArgs e)
        {
            if ((Press_COM) & (but_telemetry != null))
            {

                button_user_Click(but_telemetry, new EventArgs());
            }

            
                
            //.........................................Команда запроса данных
            //if (Press_COM)
            //{
            //    byte command = (byte)0xEE;
            //    List<byte> out_word = new List<byte>();


            //    out_word.Add(default_header);
            //    out_word.Add(0x4);
            //    out_word.Add(command);
            //    out_word.Add(crc_out(out_word.ToArray()));

            //    try
            //    {
            //        _serialPort.Write(out_word.ToArray(), 0, out_word.Count);
            //        //SetText_COM("Отправлено: " + BitConverter.ToString(out_word.ToArray()).Replace("-", " ") + Environment.NewLine);
            //    }
            //    catch
            //    {
            //        //SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
            //    }
            //}
            //else if (Press_ETH)
            //{
            //    try
            //    {
            //        await Task.Run(() => ETH_send(new byte[] {0xEE}));               

            //    }
            //        catch
            //        {
            //           // SetText_ETH("Ошибка с " + IP_ETH.ToString() + Environment.NewLine);
            //        }
            //}
            //else
            //{
            //    SetText_ETH("Нет подключения" +  Environment.NewLine);
            //    SetText_COM("Нет подключения" +  Environment.NewLine);
            //}

        }

        // Старт графика
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
               // DataLogger logger = new DataLogger();
                //logger.Show();
                graph = new App_v02.Form2();
                time_global = DateTime.Now;
                graph.Show();
                //graph.InitializeComponent();
                //graph.Init_chart();
               // graph.Activate();
            }
            catch (Exception error)
            {

                SetText_COM(error.Message + Environment.NewLine);
            };
        }

        // ------------------------------------------------UART-----------------------------------------------

        // Запуск UART
        private void button1_Click(object sender, EventArgs e)
        {
            Press_COM = !Press_COM;
            if (Press_COM == true)
            {
                try
                {
                    button1.Text = "Закрыть";
                    button1.ForeColor = Color.DarkRed;
                    _serialPort.WriteTimeout = 500;
                    _serialPort.Open();
                }
                catch
                {
                    SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);
                }
            }
            else
            {
                try
                {
                    button1.Text = "Запустить";
                    button1.ForeColor = Color.ForestGreen;
                    _serialPort.Close();
                }
                catch
                {
                    SetText_COM("Ошибка с :" + _serialPort.PortName + Environment.NewLine);

                }
            }
        }
        // Обработка полученных данных
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            List<byte> buffer = new List<byte>();
            List<byte> crc_calc_answ = new List<byte>();
            List<byte> crc_calc_tlm = new List<byte>();
            byte temp_header = 0;
            int len = 0;

            while ((temp_header != STRUCT_ANSWER[0].var) | (temp_header != STRUCT_TELEMETRY[0].var))
                try
                {
                    temp_header = (byte)((sender as SerialPort).ReadByte());
                    if ((temp_header != STRUCT_ANSWER[0].var) | (temp_header != STRUCT_TELEMETRY[0].var))
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
                    //SetText_COM("Не получен заголовочный байт: " + BitConverter.ToString(new byte[] { temp_header }).Replace("-", " ") + Environment.NewLine);
                }

            if ((temp_header == STRUCT_ANSWER[0].var) | (temp_header == STRUCT_TELEMETRY[0].var))
            {
                buffer.Add(temp_header);

                bool FLAG_END = false;
                int i = 1;
                int index_crc = 0;
                bool flag_ANSWER = false;
                bool flag_TLM = false;
                int main_count = 0;
                int index_inf = 0;
                bool wrong_flags = false;
                bool wrong_addr_send = false;
                bool wrong_addr_reciever = false;

                do
                {
                    int count_temp = buffer.Count;

                    int temp = (byte)((sender as SerialPort).ReadByte());

                    if ((((STRUCT_ANSWER[i].var_string == null) & (STRUCT_ANSWER[i].type != null)) | ((STRUCT_TELEMETRY[i].var_string == null) & (STRUCT_TELEMETRY[i].type != null))) & (STRUCT_TELEMETRY[i].name != "Информация"))
                    {
                        if ((temp == STRUCT_ANSWER[i].var) | (temp == STRUCT_TELEMETRY[i].var))
                        {
                            buffer.Add((byte)temp);
                            //i++;
                        }
                        else if ((STRUCT_ANSWER[i].name == "Флаги") | (STRUCT_TELEMETRY[i].name == "Флаги"))
                        {
                            wrong_flags = true;
                            buffer.Add((byte)temp);
                        }
                        else if ((STRUCT_ANSWER[i].name == "Адрес получателя") | (STRUCT_TELEMETRY[i].name == "Адрес получателя"))
                        {
                            wrong_addr_reciever = true;
                            buffer.Add((byte)temp);
                        }
                        else if ((STRUCT_ANSWER[i].name == "Адрес отправителя") | (STRUCT_TELEMETRY[i].name == "Адрес отправителя"))
                        {
                            wrong_addr_send = true;
                            buffer.Add((byte)temp);
                        }
                    }

                    if ((STRUCT_ANSWER[i].var_string != null) | (STRUCT_TELEMETRY[i].var_string != null))
                    {
                        index_crc = i + main_count - 1;
                        buffer.Add((byte)temp);
                        //i++;
                    }

                    if (STRUCT_ANSWER[i].crc == true)
                        crc_calc_answ.Add((byte)temp);

                    if (STRUCT_TELEMETRY[i].crc == true)
                        crc_calc_tlm.Add((byte)temp);

                    if ((STRUCT_ANSWER[i].name == "Размер информационной части") | (STRUCT_TELEMETRY[i].name == "Размер информационной части"))
                    {
                        main_count = temp;
                        if (temp == STRUCT_ANSWER[i].var)
                            flag_ANSWER = true;
                        else if (temp == STRUCT_TELEMETRY[i].var)
                            flag_TLM = true;
                        else
                        {
                            SetText_COM("Количество пришедших байт не совпадает с настройками " + Environment.NewLine);
                            break;
                        }
                    }

                    if (flag_ANSWER == true)
                    {
                        if (STRUCT_ANSWER[i].name == "Информация")
                        {
                            index_inf = i;
                            if (temp != STRUCT_ANSWER[i].var)
                            {
                                buffer.Add((byte)temp);
                                //i++;
                            }
                        }
                        else if (STRUCT_ANSWER[i].name == "Стоп")
                        {
                            FLAG_END = true;
                        }
                    }
                    else if (flag_TLM == true)
                    {
                        if (STRUCT_TELEMETRY[i].name == "Информация")
                        {
                            buffer.Add((byte)temp);
                            index_inf = i;
                            for (int j = 0; j < main_count - 1; j++)
                            {
                                int temp_inf = 0;
                                try
                                {
                                    temp_inf = (byte)(sender as SerialPort).ReadByte();
                                    buffer.Add((byte)temp_inf);
                                    if (STRUCT_TELEMETRY[i].crc == true)
                                        crc_calc_tlm.Add((byte)temp_inf);
                                }
                                catch 
                                { }
                            }
                        }
                        else if (STRUCT_TELEMETRY[i].name == "Стоп")
                        {
                            FLAG_END = true;
                        }
                    }


                    if (count_temp == buffer.Count)
                        break;

                    i++;
                }
                while (FLAG_END != true);

                int crc = 0;

                if (flag_ANSWER == true)
                {
                    if (buffer.ToArray().Last<byte>() != STRUCT_ANSWER.ToArray().Last<USER_struct>().var)
                        SetText_COM("значение окончания посылки неверное" + Environment.NewLine);

                    else
                        crc = crc_out(crc_calc_answ.ToArray());
                }
                else if (flag_TLM == true)
                    crc = crc_out(crc_calc_tlm.ToArray());



                if (crc != buffer[index_crc])
                    SetText_COM(" CRC не совпал , принятый crc " + (int) buffer[index_crc] + " рассчитанный crc : " + (int) crc + Environment.NewLine);
                else
                {
                    if (flag_ANSWER == true)
                    {

                        if (wrong_addr_reciever | wrong_addr_send | wrong_flags)
                        {
                            if (wrong_flags)
                                SetText_COM("Получены неверные значение флагов" + Environment.NewLine);

                            if (wrong_addr_send)
                                SetText_COM("Неверный адрес отправителя" + Environment.NewLine);

                            if (wrong_addr_reciever)
                                SetText_COM("Неверный адрес получателя" + Environment.NewLine);
                        }
                        else
                        if (buffer.ToArray()[index_inf] != STRUCT_ANSWER[index_inf].var)
                            SetText_COM(" Пришло неверное значение квитанции : " + BitConverter.ToString(buffer.ToArray()).Replace("-", " ") + Environment.NewLine);
                        else
                            SetText_COM("КВИТАНЦИЯ ПРИНЯТА" + Environment.NewLine);
                    }
                    else if (flag_TLM == true)
                    {
                        int k = 0;
                        int byte_k = index_inf;
                        bool flag_double_k = false;
                        string save_string = "";
                        double temp_time = 0;
                        while (k < STRUCT_TELEMETRY[index_inf].user_tlm_data.Count)
                        {
                            if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].user_bool_tlm_data.Count != 0)
                            {
                                foreach (USER_Bool_tlm_data bool_tlm_data in STRUCT_TELEMETRY[index_inf].user_tlm_data[k].user_bool_tlm_data)
                                {
                                    if ((buffer[byte_k] & (1 << bool_tlm_data.index)) == (1 << bool_tlm_data.index))
                                    {
                                        if (graph != null)
                                            graph.flag_refresh(bool_tlm_data.index_flag, true);
                                    }
                                    else
                                        if (graph != null)
                                            graph.flag_refresh(bool_tlm_data.index_flag, false);

                                }
                            }
                            else
                            {
                                float temp = 0;
                                DateTime time = DateTime.Now;
                                if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].user_uint_tlm_data.Count != 0)
                                {
                                   // float temp = 0;
                                    if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].size == 1)
                                    {
                                        temp =(float)(buffer[byte_k]  * STRUCT_TELEMETRY[index_inf].user_tlm_data[k].mult)/STRUCT_TELEMETRY[index_inf].user_tlm_data[k].division;
                                        save_string = save_string + "\t" + temp.ToString();

                                    }
                                    else if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].size == 2)
                                    {
                                        temp = (float)(((buffer[byte_k] << 8)  + (buffer[byte_k + 1]))  * STRUCT_TELEMETRY[index_inf].user_tlm_data[k].mult) / STRUCT_TELEMETRY[index_inf].user_tlm_data[k].division;
                                        flag_double_k = true;
                                        save_string = save_string + "\t" + temp.ToString();
                                    }

                                    

                                }
                                else

                                if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].user_temp_tlm_data.Count != 0)
                                {
                                    
                                    if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].size == 1)
                                    {

                                        temp =(float)((buffer[byte_k] & 0x7F)  * STRUCT_TELEMETRY[index_inf].user_tlm_data[k].mult)/STRUCT_TELEMETRY[index_inf].user_tlm_data[k].division;
                                        if ((buffer[byte_k] & 0x80) == 0x80)
                                            temp *= -1;
                                        save_string = save_string + "\t" + temp.ToString();
                                    }
                                    //else if (STRUCT_TELEMETRY[index_inf].user_tlm_data[k].size == 2)
                                    //{
                                    //    temp = (float)(((buffer[byte_k] << 8)  + (buffer[byte_k + 1]))  * STRUCT_TELEMETRY[index_inf].user_tlm_data[k].mult) / STRUCT_TELEMETRY[index_inf].user_tlm_data[k].division;
                                    //    flag_double_k = true;
                                    //}

                                    

                                    

                                }
                                temp_time = (time - time_global).TotalSeconds;
                                if (graph != null)
                                    graph.Refresh_chart(STRUCT_TELEMETRY[index_inf].user_tlm_data[k].index_graph - 1, temp, temp_time);
                            }


                            if (flag_double_k)
                            {
                                byte_k+=2;
                                flag_double_k = false;
                            } 
                            else 
                                byte_k++;

                            k++;
                        }
                        if (graph != null)
                        {
                            save_string = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ":" + DateTime.Now.Millisecond.ToString() + save_string;
                            graph.save_info(save_string);
                        }
                    }
                }
            }



            //    len = (sender as SerialPort).ReadByte();

            //    buffer.Add(temp_header);
            //    buffer.Add((byte)len);

            //    for (int i = 0; i < len - 3; i++)
            //    {
            //        buffer.Add((byte)((sender as SerialPort).ReadByte()));
            //    }

            //    byte crc = (byte)((sender as SerialPort).ReadByte());
            //    string answ = "";

            //    if (crc_out(buffer.ToArray()) != crc)
            //    {
            //        SetText_COM(" CRC не совпал " + Environment.NewLine);
            //    }
            //    else
            //    {
            //        buffer.Add(crc);


            //        if (buffer[2] == (byte)0xDD)// Находим посылку о данных 
            //        {
            //            float temp1 = (float)(((int)buffer[3] << 8) | ((int)buffer[4])) / 100;
            //            float temp2 = (float)(((int)buffer[5] << 8) | ((int)buffer[6])) / 10;
            //            time++;
            //            SetText_COM("temp1: " + temp1.ToString()+ Environment.NewLine);
            //            SetText_COM("temp2: " + temp2.ToString()+ Environment.NewLine);
            //            graph.Refresh_chart(0, (double)buffer[3], time);
            //            graph.Refresh_chart(1, (double)buffer[4], time);
            //            graph.Refresh_chart(2, (double)buffer[5], time);
            //            graph.Refresh_chart(3, (double)buffer[6], time);
            //        }
            //        else if (buffer[2] == 0xAA)
            //        {
            //            if (_LAST_TIMER_ != buffer[3])
            //            {
            //                SetText_COM("Новый отсчет времени: " + BitConverter.ToString(buffer.ToArray()).Replace("-", " ") + Environment.NewLine);
            //                _TIME_ = (DateTime.Now.Ticks);
            //                _LAST_TIMER_ = buffer[3];
            //            }
            //            switch (buffer[3])
            //            {
            //                case 0xA0:
            //                    {   
            //                        break;
            //                    }
            //                case 0xA1:
            //                    {
            //                        TIME_REFRESH(TIME_1);
            //                        SetCycl(true, X1);
            //                        break;
            //                    }
            //                case 0xA2:
            //                    {
            //                        TIME_REFRESH(TIME_2);
            //                        SetCycl(true, X2);
            //                        break;
            //                    }
            //                case 0xA3:
            //                    {
            //                        TIME_REFRESH(TIME_3);
            //                        SetCycl(true, X3);
            //                        break;
            //                    }
            //                case 0xA4:
            //                    {
            //                        TIME_REFRESH(TIME_4);
            //                        SetCycl(true, X4);
            //                        break;
            //                    }
            //                case 0xA5:
            //                    {
            //                        TIME_REFRESH(TIME_5);
            //                        SetCycl(true, X5);
            //                        break;
            //                    }
            //                case 0xA6:
            //                    {
            //                        TIME_REFRESH(TIME_6);
            //                        SetCycl(true, X6);
            //                        break;
            //                    }
            //                case 0xA7:
            //                    {
            //                        TIME_REFRESH(TIME_7);
            //                        SetCycl(true, X7);
            //                        break;
            //                    }
            //                case 0xA8:
            //                    {
            //                        TIME_REFRESH(TIME_8);
            //                        SetCycl(true, X8);
            //                        break;
            //                    }
            //                case 0xA9:
            //                    {
            //                        SetCycl(true, X9);
            //                        break;
            //                    }
            //                default:
            //                    {

            //                        break;
            //                    }
            //            }
            //        }
            //    }
            //   //SetText_COM("Получено: " + BitConverter.ToString(buffer.ToArray()).Replace("-", " ") + Environment.NewLine);

            //}
            //else SetText_COM("Получено: " + BitConverter.ToString(new byte[]{temp_header}).Replace("-", " ") + Environment.NewLine);
        }
        // Подсчет crc
        private byte crc_out(byte[] x)
        {
            return (byte)(x.Select(t => (int)t).Sum() & 0xFF); ;
        }
        
        // Установить текст в консоль
        delegate void SetTextCallback(string text1);
        public void SetText_COM(string text1)
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            if (this.textBox1.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                SetTextCallback d = new SetTextCallback(SetText_COM);
                this.Invoke(d, new object[] { text1 });
            }
            // ...иначе все по старинке
            else
            {
                this.textBox1.AppendText(text1);
                //this.textBox1.Text += text1;
            }
        }
        // Очистка консоли UART
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
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            if (this.Connect_ETH.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                SetConnect_ETH_Callback d = new SetConnect_ETH_Callback(SetConnect_ETH);
                this.Invoke(d, new object[] { text1 });
            }
            // ...иначе все по старинке
            else
            {
                this.Connect_ETH.Text = text1;
                this.Connect_ETH.ForeColor = Color.Aqua;
            }
        }
        // Подключение к серверу
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
                        //SetConnect_ETH("Подключение");
                        tcpSocket.ConnectAsync(tcpEndPoint);                     
                    });
                    Press_ETH = !Press_ETH;
                    Connect_ETH.Text = "Отключить";
                    Connect_ETH.ForeColor = Color.DarkRed;
                    
                    
                    //tcpSocket.Connect(tcpEndPoint);
                   
                }
                catch
                {
                    SetText_ETH("Ошибка с :" + tcpEndPoint.Address.ToString() + Environment.NewLine);
                }
            }
            else
            {
                try
                {
                    Press_ETH = !Press_ETH;
                    Connect_ETH.Text = "Подключить";
                    Connect_ETH.ForeColor = Color.ForestGreen;
                    tcpSocket.Shutdown(SocketShutdown.Both);
                    tcpSocket.Close();      
                }
                catch
                {
                    SetText_ETH("Ошибка с :" + tcpEndPoint.Address.ToString()  + Environment.NewLine);
                }
            }
        }

        // Запись IP
        private void IP_text_TextChanged(object sender, EventArgs e)
        {
            IP_ETH = (sender as TextBox).Text.ToString();
        }
        // Записсь PORT
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            PORT_ETH = Convert.ToInt32((sender as TextBox).Text.ToString());
        }

        // Записать данные в консоль
        delegate void SetText_ETH_Callback(string text1);
        private void SetText_ETH(string text1)
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            if (this.ETH_CONSOLE.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                SetText_ETH_Callback d = new SetText_ETH_Callback(SetText_ETH);
                this.Invoke(d, new object[] { text1 });
            }
            // ...иначе все по старинке
            else
            {
                this.ETH_CONSOLE.Text += text1;
            }
        }

        // Очистить консоль 
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

        private void PLAY_GRAPH_Click(object sender, EventArgs e)
        {
            PLAY_GRAPH_win = new App_v02.PLAY_PLOTS();
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath_XML = @openFileDialog.FileName;
            }
            PLAY_GRAPH_win.Show();
            PLAY_GRAPH_win.read_file(filePath_XML);
            //time_global = DateTime.Now;
            
        }

        public void start_tlm_but_Click(object sender, EventArgs e)
        {
            if (Press_COM)
            {
                timer1.Interval = (int)Interval_timer.Value;
                timer1.Start();
            }
        }

        public void stop_tlm_but_Click(object sender, EventArgs e)
        {
            if (Press_COM)
            {
                timer1.Stop();
            }
        }

        private void RK_ON_Click(object sender, EventArgs e)
        {
            _serialPort.Write("#011001\r");
            Thread.Sleep(100);
            _serialPort.Write("#011000\r");
        }

        private void RK_OFF_Click(object sender, EventArgs e)
        {
            _serialPort.Write("#011101\r");
            Thread.Sleep(100);
            _serialPort.Write("#011100\r");
        }
    }

}
