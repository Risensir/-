using ScottPlot;
using ScottPlot.Demo.WinForms.WinFormsDemos;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using System.Xml.Linq;
using TEST_APP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using static App_v02.user_FormsPlot;
using static App_v02.user_Button;
using System.ComponentModel;
using ScottPlot.MarkerShapes;
using static System.Net.Mime.MediaTypeNames;

namespace App_v02
{
    public class Form2 : Form
    {
        ScottPlot.FormsPlot formsPlot1;
        private ScottPlot.Plottable.ScatterPlot MyScatterPlot;
        //private Button button1;
        //private Button button3;
        //private Panel panel1;
        //public Panel panel2;
        //private Panel panel3;

        int Count = 0;
        private System.Windows.Forms.Label label1;
        private NumericUpDown numericUpDown1;
        int SIZE_INIT = 0;

        public List<double> dataX = new List<double>();
        public List<double> dataY = new List<double>();
        private Button button2;
        private Button SLIDE_1;
        private Button FULL_1;
        private Button JUMP_1;
        private Button CLEAN_1;
        private TableLayoutPanel table_main;
        private List<TableLayoutPanel> table_sub = new List<TableLayoutPanel>();
        private List<Panel> panel_graph = new List<Panel>();
        private List<Panel> panel_but = new List<Panel>();
        private Button SAVE_1;
        private List<User_Graphs> user_graph = new List<User_Graphs>();
        private CheckBox[] chBox_arr;

        class user_BUT : user_Button
        {
           public int index;
           public float init_size_COLUMN;
           public float init_size_ROW;
        }

        private ScottPlot.Plottable.DataLogger[] logger;
        private Panel panel1;
        private Button PLAY_TLMTR;
        private Button CLEAN_BUT;
        private Button SAVE_BUT;
        private TableLayoutPanel tableLayoutPanel1;
        private Button JUMP_GLOBAL;
        private Button SLIDE_GLOBAL;
        private Button FULL_GLOBAL;
        private int NUM_GRAPH = 0;
        private int NUM_FLAGS = 0;
        StreamWriter fs;
        private TableLayoutPanel table_global;
        List<string> name_tlmtr = new List<string>();
        // public Form user_graph_1;
        //private List<ScottPlot.Plottable.DataLogger> Logger = new List<ScottPlot.Plottable.DataLogger>();
        public Form2()
        {
            InitializeComponent();
            try
            {
                
            }
            catch 
            { 
            
            }
            int i = 0;
            string path = "LOGS/GRAPH_LOGS/" + DateTime.Now.Year.ToString() + "_" +  DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString();
            while (File.Exists(path + "_" + i.ToString() + ".txt"))
                i++;

            fs = new StreamWriter(path + "_" + i.ToString() + ".txt");
            XML_Read();
            //Refresh_chart(3, 0, 0.5);
            //Refresh_chart(3, 1, 1);
            //Refresh_chart(3, 2, 0.5);
            //Refresh_chart(3, 3, 1);
            //Refresh_chart(3, 4, 0.5);
            //Refresh_chart(3, 5, 1);

            if (!(Directory.Exists("LOGS/GRAPH_LOGS")))
                Directory.CreateDirectory("LOGS/GRAPH_LOGS");

            
        }

        private void XML_Read()

        {
            // ..................................................................Создание списка кнопок , основанных на раннее объявленном методе


            XmlDocument xml = new XmlDocument();
            xml.Load(@"GRAPH.xml");
            XmlElement element = xml.DocumentElement;
            if (element.Name == "mainWindow")
            {
                if (element.Attributes.GetNamedItem("width") != null)
                    this.Width = int.Parse(element.Attributes.GetNamedItem("width").Value);

                if (element.Attributes.GetNamedItem("height") != null)
                    this.Height = int.Parse(element.Attributes.GetNamedItem("height").Value);
            }
            // ........................................................................Очистка стилей колонок и строк таблицы
            TableLayoutPanel table_flag = new TableLayoutPanel();
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.Name == "graph")
                    NUM_GRAPH++;
                if (node.Name == "flag")
                    NUM_FLAGS++;
            }
            chBox_arr = new  CheckBox[NUM_FLAGS];
            // NUM_GRAPH = element.ChildNodes.Count;
            table_flag.ColumnStyles.Clear();
            table_flag.RowStyles.Clear();

            table_flag.ColumnCount = 1;
            table_flag.RowCount = NUM_FLAGS+1;
            table_flag.CellBorderStyle  = TableLayoutPanelCellBorderStyle.Outset;

            table_global.ColumnStyles.Clear();
            table_global.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            for (int k = 0; k < NUM_FLAGS; k++)
            {
                table_flag.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent , 10));
            }
            table_flag.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            table_main.ColumnStyles.Clear();
            table_main.RowStyles.Clear();
            logger = new ScottPlot.Plottable.DataLogger[NUM_GRAPH];

            if (NUM_GRAPH < 3)
            {
                table_main.ColumnCount = NUM_GRAPH;
                table_main.RowCount = 1;

                for (int k = 0; k < 2; k++)
                {
                    this.table_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
                }
                this.table_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
            }
            else
            {
                if (NUM_GRAPH == 4)
                {
                    table_main.ColumnCount = 2;
                    table_main.RowCount = 2;
                 

                    for (int k = 0; k < 2; k++)
                    {
                        this.table_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
                    }
                    for (int k = 0; k < 2; k++)
                    {
                        this.table_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
                    }

                }
                else
                {
                    table_main.ColumnCount = 3;

                    if (NUM_GRAPH % 3 == 0)
                        table_main.RowCount = NUM_GRAPH/3;
                    else
                        table_main.RowCount = NUM_GRAPH/3 + 1;

                    for (int k = 0; k < 3; k++)
                    {
                        this.table_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
                    }



                    if (NUM_GRAPH % 3 == 0)
                        for (int k = 0; k < NUM_GRAPH/3; k++)
                        {
                            this.table_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
                        }
                    else
                    {

                    }
                    for (int k = 0; k < NUM_GRAPH/3 + 1; k++)
                    {
                        this.table_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
                    }
                }
            }


            //........................................................................ Создание новых стилей колонок и таблицы

            //table_main.ColumnStyles[0].Width = 500;
            //table_main.RowStyles[0].Height = 500;

            List<System.Windows.Forms.Label> label_list = new List<System.Windows.Forms.Label>();
            
            int i = 0, j = 0;
            int num_flag = 0;
            //System.Windows.Forms.Label label_test = new System.Windows.Forms.Label();
            //label_test.Text = element.Name.ToString();
            //panel_bt.Controls.Add(label_test);
            //label_test.Dock = DockStyle.Bottom;
            //.......................................................................Считывание документа XML
            foreach (XmlNode xnode in element)
            {
                if (xnode.Name == "graph")
                {
                    TableLayoutPanel table = new TableLayoutPanel();
                    Panel panel_gr = new Panel();
                    Panel panel_bt = new Panel();

                    panel_gr.Dock = System.Windows.Forms.DockStyle.Fill;
                    panel_bt.Dock = System.Windows.Forms.DockStyle.Fill;
                    table.Dock = System.Windows.Forms.DockStyle.Fill;

                    table.ColumnStyles.Add(new ColumnStyle());
                    table.ColumnStyles.Add(new ColumnStyle());

                    table.ColumnStyles[0].SizeType = SizeType.Percent;
                    table.ColumnStyles[0].Width = 85;

                    table.ColumnStyles[1].SizeType = SizeType.Percent;
                    table.ColumnStyles[1].Width = 15;

                    table.Controls.Add(panel_gr, 0, 0);
                    table.Controls.Add(panel_bt, 1, 0);


                    table_main.Controls.Add(table, i, j);
                    User_Graphs user_gr = new User_Graphs(panel_gr.Size.Width, panel_gr.Size.Height, panel_bt.Size.Width, panel_bt.Size.Height);


                    if (xnode.Attributes.GetNamedItem("crit_var") != null)
                        user_gr.formsPlot1.Plot.AddHorizontalLine(double.Parse(xnode.Attributes.GetNamedItem("crit_var").Value), Color.Red);

                    int index = int.Parse(xnode.Attributes.GetNamedItem("index").Value);

                    user_gr.formsPlot1.Plot.XLabel(xnode.Attributes.GetNamedItem("x_label").Value);
                    user_gr.formsPlot1.Plot.YLabel(xnode.Attributes.GetNamedItem("y_label").Value);
                    user_gr.formsPlot1.Plot.Title(xnode.Attributes.GetNamedItem("name").Value);

                    VLine temp_vline = user_gr.formsPlot1.Plot.AddVerticalLine(0);
                    //temp_vline.X = 0;
                    temp_vline.PositionLabel = true;

                    HLine temp_hline = user_gr.formsPlot1.Plot.AddHorizontalLine(0);
                    //temp_hline.Y = 0;
                    temp_hline.PositionLabel = true;

                    user_gr.formsPlot1.hline = temp_hline;
                    user_gr.formsPlot1.vline = temp_vline;

                    user_gr.formsPlot1.MouseMove += new System.Windows.Forms.MouseEventHandler(formsPlot1_MouseMove);

                    //user_gr.formsPlot1.Plot.AddAnnotation("hello");
                    //user_gr.formsPlot1.;

                    logger[index - 1] = new ScottPlot.Plottable.DataLogger(user_gr.formsPlot1.Plot);

                    logger[index - 1] = user_gr.formsPlot1.Plot.AddDataLogger();
                    logger[index - 1].Label =  xnode.Attributes.GetNamedItem("name").Value;
                    name_tlmtr.Add(logger[index - 1].Label);

                    logger[index - 1].Plot.YAxis.SetBoundary((double.Parse(xnode.Attributes.GetNamedItem("min_var").Value)), double.Parse(xnode.Attributes.GetNamedItem("max_var").Value));
                    //logger[index - 1].Plot.data


                    user_gr.formsPlot1.Refresh();


                    panel_gr.Controls.Add(user_gr.formsPlot1);
                    TableLayoutPanel table_bt = new TableLayoutPanel();
                    table_bt.Dock = DockStyle.Fill;

                    for (int k = 0; k < 5; k++)
                    {
                        table_bt.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50));
                    }

                    foreach (user_Button y in user_gr.components)
                    {

                        user_BUT but = new user_BUT();
                        but.Name = y.Name;
                        but.Size = y.Size;
                        but.Text = y.Text;
                        but.Dock = y.Dock;
                        but.index = index - 1;
                        but.Dock = DockStyle.Fill;

                        if (but.Name == "HIDE")
                        {
                            but.index_i = i;
                            but.index_j = j;
                            but.init_size_COLUMN = table_main.ColumnStyles[i].Width;
                            but.init_size_ROW = table_main.RowStyles[i].Height;
                        }

                        set_Handler(but);

                        table_bt.Controls.Add(but);

                        // panel_bt.Controls.Add(but);

                    }
                    panel_bt.Controls.Add(table_bt);
                    user_graph.Add(user_gr);
                    panel_graph.Add(panel_gr);
                    panel_but.Add(panel_bt);
                    table_sub.Add(table);


                    if ((j <= table_main.RowCount - 1) | (i <= table_main.ColumnCount - 1))
                    {
                        if (i == table_main.ColumnCount - 1)
                        {
                            i = 0;
                            if (j != table_main.RowCount - 1)
                                j++;
                            else
                                j = 0;
                        }
                        else
                            i++;
                    }


                }
                else
                {
                    if (xnode.Name == "flag")
                    { 
                        Panel temp_panel = new Panel(); 
                        CheckBox checkBox = new CheckBox();
                        temp_panel.Dock = DockStyle.Fill;

                        if (xnode.Attributes.GetNamedItem("name") != null)
                            checkBox.Text = xnode.Attributes.GetNamedItem("name").Value;
                        checkBox.CheckState = CheckState.Unchecked;
                       // checkBox.BackColor = Color.Black;
                        checkBox.Dock = DockStyle.Fill;

                        temp_panel.Controls.Add(checkBox);
                        table_flag.Dock = DockStyle.Fill;   
                        table_flag.Controls.Add(temp_panel);
                        if (xnode.Attributes.GetNamedItem("index_flag") != null)
                        {
                            int index_temp = int.Parse(xnode.Attributes.GetNamedItem("index_flag").Value);
                            chBox_arr[index_temp - 1] = checkBox;
                        }
                    }
                }
            }
            table_global.Controls.Add(table_flag);
            string temp = "";
            foreach (string str in name_tlmtr)
                temp = temp + "\t" + str;

            fs.WriteLine(temp);

        }
        private void formsPlot1_MouseMove(object sender , MouseEventArgs e)
        {
            user_FormsPlot plt = (sender as user_FormsPlot);
            (double x, double y) = plt.GetMouseCoordinates();
           // MyScatterPlot = plt.Plot.AddScatterPoints();
            plt.hline.Y = y;
            plt.vline.X = x;
           // plt.Plot.AddVerticalLine(x);
            plt.Refresh();
            //Console.WriteLine($"Mouse at ({x}, {y})");
        }
        private void set_Handler(Control but)
        {
            switch (but.Name)
            {
                case "FULL":
                    but.Click += new System.EventHandler(this.FULL_Click);
                    break;
                case "SLIDE":
                    but.Click += new System.EventHandler(this.SLIDE_Click);
                    break;
                case "JUMP":
                    but.Click += new System.EventHandler(this.JUMP_Click);
                    break;
                case "CLEAN":
                    but.Click += new System.EventHandler(this.CLEAN_Click);
                    break;
                case "SAVE":
                    but.Click += new System.EventHandler(this.SAVE_Click);
                    break;
                case "HIDE":
                    but.Click += new System.EventHandler(this.HIDE_Click);
                    break;
            }
        }
        private void InitializeComponent()
        {
            this.table_main = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.JUMP_GLOBAL = new System.Windows.Forms.Button();
            this.SLIDE_GLOBAL = new System.Windows.Forms.Button();
            this.FULL_GLOBAL = new System.Windows.Forms.Button();
            this.PLAY_TLMTR = new System.Windows.Forms.Button();
            this.CLEAN_BUT = new System.Windows.Forms.Button();
            this.SAVE_BUT = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.table_global = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.table_global.SuspendLayout();
            this.SuspendLayout();
            // 
            // table_main
            // 
            this.table_main.AutoScroll = true;
            this.table_main.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.table_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.table_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_main.Location = new System.Drawing.Point(3, 3);
            this.table_main.Name = "table_main";
            this.table_main.Size = new System.Drawing.Size(1386, 888);
            this.table_main.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.JUMP_GLOBAL);
            this.panel1.Controls.Add(this.SLIDE_GLOBAL);
            this.panel1.Controls.Add(this.FULL_GLOBAL);
            this.panel1.Controls.Add(this.PLAY_TLMTR);
            this.panel1.Controls.Add(this.CLEAN_BUT);
            this.panel1.Controls.Add(this.SAVE_BUT);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 897);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1386, 94);
            this.panel1.TabIndex = 1;
            // 
            // JUMP_GLOBAL
            // 
            this.JUMP_GLOBAL.Dock = System.Windows.Forms.DockStyle.Left;
            this.JUMP_GLOBAL.Location = new System.Drawing.Point(759, 0);
            this.JUMP_GLOBAL.Name = "JUMP_GLOBAL";
            this.JUMP_GLOBAL.Size = new System.Drawing.Size(166, 94);
            this.JUMP_GLOBAL.TabIndex = 5;
            this.JUMP_GLOBAL.Text = "JUMP";
            this.JUMP_GLOBAL.UseVisualStyleBackColor = true;
            this.JUMP_GLOBAL.Click += new System.EventHandler(this.JUMP_GLOBAL_Click);
            // 
            // SLIDE_GLOBAL
            // 
            this.SLIDE_GLOBAL.Dock = System.Windows.Forms.DockStyle.Left;
            this.SLIDE_GLOBAL.Location = new System.Drawing.Point(593, 0);
            this.SLIDE_GLOBAL.Name = "SLIDE_GLOBAL";
            this.SLIDE_GLOBAL.Size = new System.Drawing.Size(166, 94);
            this.SLIDE_GLOBAL.TabIndex = 4;
            this.SLIDE_GLOBAL.Text = "SLIDE";
            this.SLIDE_GLOBAL.UseVisualStyleBackColor = true;
            this.SLIDE_GLOBAL.Click += new System.EventHandler(this.SLIDE_GLOBAL_Click);
            // 
            // FULL_GLOBAL
            // 
            this.FULL_GLOBAL.Dock = System.Windows.Forms.DockStyle.Left;
            this.FULL_GLOBAL.Location = new System.Drawing.Point(427, 0);
            this.FULL_GLOBAL.Name = "FULL_GLOBAL";
            this.FULL_GLOBAL.Size = new System.Drawing.Size(166, 94);
            this.FULL_GLOBAL.TabIndex = 3;
            this.FULL_GLOBAL.Text = "FULL";
            this.FULL_GLOBAL.UseVisualStyleBackColor = true;
            this.FULL_GLOBAL.Click += new System.EventHandler(this.FULL_GLOBAL_Click);
            // 
            // PLAY_TLMTR
            // 
            this.PLAY_TLMTR.Dock = System.Windows.Forms.DockStyle.Left;
            this.PLAY_TLMTR.Location = new System.Drawing.Point(278, 0);
            this.PLAY_TLMTR.Name = "PLAY_TLMTR";
            this.PLAY_TLMTR.Size = new System.Drawing.Size(149, 94);
            this.PLAY_TLMTR.TabIndex = 2;
            this.PLAY_TLMTR.Text = "ЗАПУСТИТЬ ОПРОС";
            this.PLAY_TLMTR.UseVisualStyleBackColor = true;
            this.PLAY_TLMTR.Click += new System.EventHandler(this.PLAY_TLMTR_Click);
            // 
            // CLEAN_BUT
            // 
            this.CLEAN_BUT.Dock = System.Windows.Forms.DockStyle.Left;
            this.CLEAN_BUT.Location = new System.Drawing.Point(133, 0);
            this.CLEAN_BUT.Name = "CLEAN_BUT";
            this.CLEAN_BUT.Size = new System.Drawing.Size(145, 94);
            this.CLEAN_BUT.TabIndex = 1;
            this.CLEAN_BUT.Text = "ОЧИСТИТЬ";
            this.CLEAN_BUT.UseVisualStyleBackColor = true;
            this.CLEAN_BUT.Click += new System.EventHandler(this.CLEAN_BUT_Click);
            // 
            // SAVE_BUT
            // 
            this.SAVE_BUT.Dock = System.Windows.Forms.DockStyle.Left;
            this.SAVE_BUT.Location = new System.Drawing.Point(0, 0);
            this.SAVE_BUT.Name = "SAVE_BUT";
            this.SAVE_BUT.Size = new System.Drawing.Size(133, 94);
            this.SAVE_BUT.TabIndex = 0;
            this.SAVE_BUT.Text = "СОХРАНИТЬ";
            this.SAVE_BUT.UseVisualStyleBackColor = true;
            this.SAVE_BUT.Click += new System.EventHandler(this.SAVE_BUT_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.table_main, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(405, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1392, 994);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // table_global
            // 
            this.table_global.ColumnCount = 2;
            this.table_global.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.33333F));
            this.table_global.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.66666F));
            this.table_global.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.table_global.Location = new System.Drawing.Point(0, 0);
            this.table_global.Name = "table_global";
            this.table_global.RowCount = 1;
            this.table_global.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_global.Size = new System.Drawing.Size(1800, 1000);
            this.table_global.TabIndex = 3;
            // 
            // Form2
            // 
            this.ClientSize = new System.Drawing.Size(1800, 1000);
            this.Controls.Add(this.table_global);
            this.Name = "Form2";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.table_global.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1.timer1.Stop();
            fs.Close();
        }

        /*public void InitializeComponent()
{
this.button1 = new System.Windows.Forms.Button();
this.button3 = new System.Windows.Forms.Button();
this.panel1 = new System.Windows.Forms.Panel();
this.label1 = new System.Windows.Forms.Label();
this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
this.button2 = new System.Windows.Forms.Button();
this.panel2 = new System.Windows.Forms.Panel();
this.panel3 = new System.Windows.Forms.Panel();
this.JUMP_1 = new System.Windows.Forms.Button();
this.formsPlot1 = new ScottPlot.FormsPlot();
this.SLIDE_1 = new System.Windows.Forms.Button();
this.CLEAN_1 = new System.Windows.Forms.Button();
this.SAVE_1 = new System.Windows.Forms.Button();
this.FULL_1 = new System.Windows.Forms.Button();
this.panel1.SuspendLayout();
((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
this.panel2.SuspendLayout();
this.panel3.SuspendLayout();
this.SuspendLayout();
// 
// button1
// 
this.button1.AllowDrop = true;
this.button1.Dock = System.Windows.Forms.DockStyle.Left;
this.button1.Location = new System.Drawing.Point(199, 0);
this.button1.Name = "button1";
this.button1.Size = new System.Drawing.Size(100, 55);
this.button1.TabIndex = 1;
this.button1.Text = "Запрос";
this.button1.UseVisualStyleBackColor = true;
this.button1.Click += new System.EventHandler(this.button1_Click);
// 
// button3
// 
this.button3.AllowDrop = true;
this.button3.Dock = System.Windows.Forms.DockStyle.Left;
this.button3.Location = new System.Drawing.Point(0, 0);
this.button3.Name = "button3";
this.button3.Size = new System.Drawing.Size(100, 55);
this.button3.TabIndex = 3;
this.button3.Text = "Очистить";
this.button3.UseVisualStyleBackColor = true;
this.button3.Click += new System.EventHandler(this.button3_Click);
// 
// panel1
// 
this.panel1.Controls.Add(this.label1);
this.panel1.Controls.Add(this.numericUpDown1);
this.panel1.Controls.Add(this.button1);
this.panel1.Controls.Add(this.button2);
this.panel1.Controls.Add(this.button3);
this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
this.panel1.Location = new System.Drawing.Point(0, 495);
this.panel1.Name = "panel1";
this.panel1.Size = new System.Drawing.Size(1426, 55);
this.panel1.TabIndex = 4;
// 
// label1
// 
this.label1.AutoSize = true;
this.label1.Location = new System.Drawing.Point(305, 16);
this.label1.Name = "label1";
this.label1.Size = new System.Drawing.Size(124, 13);
this.label1.TabIndex = 5;
this.label1.Text = "Интервал запроса , мс";
// 
// numericUpDown1
// 
this.numericUpDown1.Location = new System.Drawing.Point(305, 32);
this.numericUpDown1.Maximum = new decimal(new int[] {
1000000,
0,
0,
0});
this.numericUpDown1.Name = "numericUpDown1";
this.numericUpDown1.Size = new System.Drawing.Size(92, 20);
this.numericUpDown1.TabIndex = 4;
this.numericUpDown1.Value = new decimal(new int[] {
1000,
0,
0,
0});
this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
// 
// button2
// 
this.button2.Dock = System.Windows.Forms.DockStyle.Left;
this.button2.Location = new System.Drawing.Point(100, 0);
this.button2.Name = "button2";
this.button2.Size = new System.Drawing.Size(99, 55);
this.button2.TabIndex = 2;
this.button2.Text = "Сохранить";
this.button2.UseVisualStyleBackColor = true;
this.button2.Click += new System.EventHandler(this.button2_Click);
// 
// panel2
// 
this.panel2.AutoScroll = true;
this.panel2.AutoScrollMargin = new System.Drawing.Size(100, 0);
this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
this.panel2.Controls.Add(this.panel3);
this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
this.panel2.Location = new System.Drawing.Point(0, 0);
this.panel2.Name = "panel2";
this.panel2.Size = new System.Drawing.Size(1426, 495);
this.panel2.TabIndex = 5;
// 
// panel3
// 
this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
this.panel3.Controls.Add(this.JUMP_1);
this.panel3.Controls.Add(this.SLIDE_1);
this.panel3.Controls.Add(this.CLEAN_1);
this.panel3.Controls.Add(this.SAVE_1);
this.panel3.Controls.Add(this.FULL_1);
this.panel3.Controls.Add(this.formsPlot1);
this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
this.panel3.Location = new System.Drawing.Point(0, 0);
this.panel3.Name = "panel3";
this.panel3.Size = new System.Drawing.Size(1503, 478);
this.panel3.TabIndex = 1;
// 
// JUMP_1
// 
this.JUMP_1.Location = new System.Drawing.Point(-2, 166);
this.JUMP_1.Name = "JUMP_1";
this.JUMP_1.Size = new System.Drawing.Size(90, 37);
this.JUMP_1.TabIndex = 1;
this.JUMP_1.Text = "JUMP";
this.JUMP_1.UseVisualStyleBackColor = true;
this.JUMP_1.Click += new System.EventHandler(this.JUMP_1_Click);
// 
// formsPlot1
// 
this.formsPlot1.Location = new System.Drawing.Point(-2, -2);
this.formsPlot1.Name = "formsPlot1";
this.formsPlot1.Size = new System.Drawing.Size(550, 326);
this.formsPlot1.TabIndex = 0;
// 
// SLIDE_1
// 
this.SLIDE_1.Location = new System.Drawing.Point(554, 126);
this.SLIDE_1.Name = "SLIDE_1";
this.SLIDE_1.Size = new System.Drawing.Size(90, 37);
this.SLIDE_1.TabIndex = 1;
this.SLIDE_1.Text = "SLIDE";
this.SLIDE_1.UseVisualStyleBackColor = true;
this.SLIDE_1.Click += new System.EventHandler(this.SLIDE_1_Click);
// 
// CLEAN_1
// 
this.CLEAN_1.Location = new System.Drawing.Point(554, 246);
this.CLEAN_1.Name = "CLEAN_1";
this.CLEAN_1.Size = new System.Drawing.Size(90, 37);
this.CLEAN_1.TabIndex = 1;
this.CLEAN_1.Text = "Очистить";
this.CLEAN_1.UseVisualStyleBackColor = true;
this.CLEAN_1.Click += new System.EventHandler(this.CLEAN_1_Click);
// 
// SAVE_1
// 
this.SAVE_1.Location = new System.Drawing.Point(554, 206);
this.SAVE_1.Name = "SAVE_1";
this.SAVE_1.Size = new System.Drawing.Size(90, 37);
this.SAVE_1.TabIndex = 1;
this.SAVE_1.Text = "Сохранить";
this.SAVE_1.UseVisualStyleBackColor = true;
// 
// FULL_1
// 
this.FULL_1.Location = new System.Drawing.Point(554, 86);
this.FULL_1.Name = "FULL_1";
this.FULL_1.Size = new System.Drawing.Size(90, 37);
this.FULL_1.TabIndex = 1;
this.FULL_1.Text = "FULL";
this.FULL_1.UseVisualStyleBackColor = true;
this.FULL_1.Click += new System.EventHandler(this.FULL_1_Click);
// 
// Form2
// 
this.AutoScroll = true;
this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
this.ClientSize = new System.Drawing.Size(1426, 550);
this.Controls.Add(this.panel2);
this.Controls.Add(this.panel1);
this.Name = "Form2";
this.panel1.ResumeLayout(false);
this.panel1.PerformLayout();
((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
this.panel2.ResumeLayout(false);
this.panel3.ResumeLayout(false);
this.ResumeLayout(false);

}*/


        delegate Task SetChartCallback(int index, double val, double time);

        public async Task Refresh_chart(int index, double val, double time)
        //public void Refresh_chart()
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            if (user_graph[index].formsPlot1.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                SetChartCallback d = new SetChartCallback(Refresh_chart);
                this.Invoke(d, new object[] { index, val, time });
            }
            // ...иначе все по старинке
            else
            {
                logger[index].Add(time, val);
                logger[index].MarkerSize = 4;
                logger[index].Color = Color.Blue;
                logger[index].ManageAxisLimits = true;
                
                //logger[index].GetLegendItems();
                //logger[index].Plot.AddPoint(time, val);
                //logger[index].Plot.Add;
                user_graph[index].formsPlot1.Refresh();
            }
        }

        delegate Task flag_refresh_Callback(int index, bool flag);

        public async Task flag_refresh(int index, bool flag)
        {
            if (chBox_arr[index - 1].InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                flag_refresh_Callback d = new flag_refresh_Callback(flag_refresh);
                this.Invoke(d, new object[] { index, flag });
            }
            else 
            {
                if (flag == true)
                    chBox_arr[index - 1].CheckState = CheckState.Checked;
                else
                    chBox_arr[index - 1].CheckState = CheckState.Unchecked;
            }
        }

        //    private void button1_Click(object sender, EventArgs e)
        //    {
        //        if (TEST_APP.Form1.Press_COM || TEST_APP.Form1.Press_ETH)
        //        {
        //            if (!TEST_APP.Form1.timer1.Enabled)
        //            {
        //                TEST_APP.Form1.timer1.Enabled = true;
        //                button1.Text = "Стоп запрос";
        //                Form1.time = 0;
        //            }
        //            else
        //            {
        //                TEST_APP.Form1.timer1.Enabled = false;
        //                button1.Text = "Запрос";
        //            }
        //        }
        //    }

        //    int i = 0;

        //    void Save_graph()
        //    {
        //        Directory.CreateDirectory("Chart");
        //        string temp = String.Format(@"Chart\Current_{4}_{1}_{2}_{3}.jpeg", i, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Day);
        //        //this.chart1.SaveImage(temp, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        formsPlot1.Plot.SaveFig(temp);
        //        i++;

        //      /*  double[] data = ScottPlot.Generate.Sin();
        //        formsPlot1.Plot.AddSignal(data);



        //        formsPlot1.Plot.XLabel("Time[s]");
        //        formsPlot1.Plot.YLabel("Temperature[°C]");
        //        formsPlot1.Plot.Title("Temperature Sensor");


        //        formsPlot1.Refresh();*/
        //    }

        //    private void button2_Click(object sender, EventArgs e)
        //    {
        //        Save_graph();
        //    }

        //    

        delegate Task saveCallback(string str);

        public async Task save_info(string str)
        //public void Refresh_chart()
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            //if (fs.InvokeRequired)
            //{
            //    // ...тогда создаем обратный вызов...
            //    SetChartCallback d = new SetChartCallback(Refresh_chart);
            //    this.Invoke(d, new object[] { index, val, time });
            //}
            // ...иначе все по старинке
            //else
            //{
            fs.WriteLine(str);
            //}
        }




        private void FULL_Click(object sender, EventArgs e)
        {
            user_BUT temp = sender as user_BUT;

            logger[temp.index].ViewFull();
            user_graph[temp.index].formsPlot1.Refresh();

            //EDIT_chart(temp.Name.Last<char>(), "FULL");
        }

        private void SLIDE_Click(object sender, EventArgs e)
        {
            user_BUT temp = sender as user_BUT;
            logger[temp.index].ViewSlide(2);
            user_graph[temp.index].formsPlot1.Refresh();
        }

        private void JUMP_Click(object sender, EventArgs e)
        {
            user_BUT temp = sender as user_BUT;
            logger[temp.index].ViewJump(2);
            user_graph[temp.index].formsPlot1.Refresh();
        }

        private void CLEAN_Click(object sender, EventArgs e)
        {
            user_BUT temp = sender as user_BUT;
            Clean_chart(temp.index);
            user_graph[temp.index].formsPlot1.Refresh();
        }

        private void SAVE_Click(object sender, EventArgs e)
        {
            user_BUT temp = sender as user_BUT;
            // Clean_c(temp.index);
            save_graph(temp);
        }

        private void HIDE_Click(object sender, EventArgs e)
        {
            user_BUT temp = sender as user_BUT;
            TableLayoutControlCollection cmpt = table_main.Controls;

            if ((table_main.ColumnStyles[temp.index_i].Width == temp.init_size_COLUMN) | (table_main.RowStyles[temp.index_j].Height == temp.init_size_ROW))
            {
                table_main.ColumnStyles[temp.index_i].Width = 500;
                table_main.RowStyles[temp.index_j].Height = 500;

                for (int i = 0; i < cmpt.Count; i++)
                {
                    if (i != temp.index)
                        cmpt[i].Hide();
                }
            }
            else
            {
                table_main.ColumnStyles[temp.index_i].Width = temp.init_size_COLUMN;
                table_main.RowStyles[temp.index_j].Height = temp.init_size_ROW;
                for (int i = 0; i < cmpt.Count; i++)
                {
                    if (i != temp.index)
                        cmpt[i].Show();
                }
            }
 
            // Clean_c(temp.index);

        }

        private void save_graph(user_BUT temp)
        {
        //    user_BUT temp = sender as user_BUT;
            // Clean_c(temp.index);
          //  save_graph();
            if (!(Directory.Exists("CHARTS/" + logger[temp.index].Label)))
                Directory.CreateDirectory("CHARTS/" + logger[temp.index].Label);

            user_graph[temp.index].formsPlot1.Plot.SaveFig("CHARTS/" + logger[temp.index].Label  + "/" + logger[temp.index].Label +  "_"  + DateTime.Now.Date.Year.ToString()  + "_" + DateTime.Now.Date.Month.ToString()  + "_" + DateTime.Now.Date.Day.ToString()  + "_"  + DateTime.Now.Hour.ToString()  + "_"  + DateTime.Now.Minute.ToString()  + "_"  + DateTime.Now.Second.ToString()  + ".png");
        }

        private void save_graph(ScottPlot.Plottable.DataLogger logger)
        {
            //    user_BUT temp = sender as user_BUT;
            // Clean_c(temp.index);
            //  save_graph();
            if (!(Directory.Exists("CHARTS/" + logger.Label)))
                Directory.CreateDirectory("CHARTS/" + logger.Label);

            logger.Plot.SaveFig("CHARTS/" + logger.Label  + "/" + logger.Label +  "_"  + DateTime.Now.Date.Year.ToString()  + "_" + DateTime.Now.Date.Month.ToString()  + "_" + DateTime.Now.Date.Day.ToString()  + "_"  + DateTime.Now.Hour.ToString()  + "_"  + DateTime.Now.Minute.ToString()  + "_"  + DateTime.Now.Second.ToString()  + ".png");
        }

        delegate void CleanChartCallback(int index);

        public void Clean_chart(int index)
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            //if (logger[index].InvokeRequired)
            //{
            //    // ...тогда создаем обратный вызов...
            //    CleanChartCallback d = new CleanChartCallback(Clean_chart);
            //    this.Invoke(d, new object[] {index});
            //}
            //// ...иначе все по старинке
           // else
           // {
                logger[index].Clear();
                user_graph[index].formsPlot1.Refresh();
            // }
        }

        private void CLEAN_BUT_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < NUM_GRAPH; i++)
            {
                Clean_chart(i);
                user_graph[i].formsPlot1.Refresh();
            }
        }

        private void SAVE_BUT_Click(object sender, EventArgs e)
        {
            foreach (ScottPlot.Plottable.DataLogger lg in logger)
            {
                save_graph(lg);
            }

        }

        private void FULL_GLOBAL_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < logger.Length; i++)
            {
                logger[i].ViewFull();
                user_graph[i].formsPlot1.Refresh();
            }
        }

        private void SLIDE_GLOBAL_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < logger.Length; i++)
            {
                logger[i].ViewSlide(2);
                user_graph[i].formsPlot1.Refresh();
            }
        }

        private void JUMP_GLOBAL_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < logger.Length; i++)
            {
                logger[i].ViewJump(2);
                user_graph[i].formsPlot1.Refresh();
            }
        }

        private void PLAY_TLMTR_Click(object sender, EventArgs e)
        {
            Button but = sender as Button;

            if (but.Text == "ЗАПУСТИТЬ ОПРОС")
            {
                but.Text = "СТОП ОПРОС";
                Form1.timer1.Start();
            }
            else
            {
                but.Text = "ЗАПУСТИТЬ ОПРОС";
                Form1.timer1.Stop();
            }
        }
    }


}
