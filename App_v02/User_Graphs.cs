using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TEST_APP;
using ScottPlot;
using ScottPlot.Demo.WinForms.WinFormsDemos;
using ScottPlot.Drawing.Colormaps;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TEST_APP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using System.Security.Cryptography.X509Certificates;
using ScottPlot.Plottable;

namespace App_v02
{
    public class user_FormsPlot : ScottPlot.FormsPlot
    {
        public VLine vline;
        public HLine hline;
    }

    public class user_Button : Button
    {
        public int index_i;
        public int index_j;
    }

    internal class User_Graphs: Control
    {
        public List<Button> components = new List<Button>();

        
        public user_FormsPlot formsPlot1;

        int Count = 0;
        private Label label1;
        private NumericUpDown numericUpDown1;
        int SIZE_INIT = 0;

        public List<double> dataX = new List<double>();
        public List<double> dataY = new List<double>();
        public user_Button SLIDE;
        public user_Button FULL;
        public user_Button JUMP;
        public user_Button CLEAN;
        public user_Button SAVE;
        public user_Button HIDE;
        //public ScottPlot.Plottable.DataLogger Logger;

        private int WIDTH_graph = 1;
        private int HEIGHT_graph = 1;
        private int WIDTH_but= 1;
        private int HEIGHT_but = 1;
        public User_Graphs(int width1 , int height1 , int width2, int height2)
        {
            WIDTH_graph = width1;
            HEIGHT_graph = height1;
            WIDTH_but = width2;
            HEIGHT_but = height2;
            InitializeComponent();
            this.formsPlot1.Size = new System.Drawing.Size(WIDTH_graph, HEIGHT_graph);

            components.Add(HIDE);
            components.Add(SLIDE);
            components.Add(FULL);
            components.Add(JUMP);
            components.Add(CLEAN);
            components.Add(SAVE);
            

            int i = 0;
            //int k = 0;
            foreach (user_Button x in components)
            {
                x.Size = new System.Drawing.Size(WIDTH_but, (int)(HEIGHT_but/(components.Count)));
                x.Location = new System.Drawing.Point(0, i);
                i+=10 + x.Size.Height;
                x.Dock = DockStyle.Bottom;
                // k++;
            }



        }
        public void InitializeComponent()
        {

            this.formsPlot1 = new user_FormsPlot();
            this.JUMP = new user_Button();
            this.SLIDE = new user_Button();
            this.CLEAN = new user_Button();
            this.SAVE = new user_Button();
            this.FULL = new user_Button();
            this.HIDE = new user_Button();

            this.SuspendLayout();
            // 
            // formsPlot1
            // 
            this.formsPlot1.Location = new System.Drawing.Point(0,0);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(50, 50);
            this.formsPlot1.TabIndex = 0;
            this.formsPlot1.Dock = DockStyle.Fill;
            // 
            // JUMP
            // 
            this.JUMP.Location = new System.Drawing.Point(0, 0);
            this.JUMP.Name = "JUMP";
            this.JUMP.Size = new System.Drawing.Size(90, 37);
            this.JUMP.TabIndex = 3;
            this.JUMP.Text = "JUMP";
            this.JUMP.UseVisualStyleBackColor = true;
            this.JUMP.Dock = DockStyle.Top;
           
            // 
            // SLIDE
            // 
            this.SLIDE.Location = new System.Drawing.Point(0, 0);
            this.SLIDE.Name = "SLIDE";
            this.SLIDE.Size = new System.Drawing.Size(90, 37);
            this.SLIDE.TabIndex = 4;
            this.SLIDE.Text = "SLIDE";
            this.SLIDE.UseVisualStyleBackColor = true;
            this.SLIDE.Dock = DockStyle.Top;
            
            // 
            // CLEAN
            // 
            this.CLEAN.Location = new System.Drawing.Point(0, 0);
            this.CLEAN.Name = "CLEAN";
            this.CLEAN.Size = new System.Drawing.Size(90, 37);
            this.CLEAN.TabIndex = 5;
            this.CLEAN.Text = "Очистить";
            this.CLEAN.UseVisualStyleBackColor = true;
            this.CLEAN.Dock = DockStyle.Top;
          
            // 
            // SAVE
            // 
            this.SAVE.Location = new System.Drawing.Point(0, 0);
            this.SAVE.Name = "SAVE";
            this.SAVE.Size = new System.Drawing.Size(90, 37);
            this.SAVE.TabIndex = 6;
            this.SAVE.Text = "Сохранить";
            this.SAVE.UseVisualStyleBackColor = true;
            this.SAVE.Dock = DockStyle.Top;
            //this.SAVE.Click += new System.EventHandler(this.Save_graph);
            // 
            // FULL
            // 
            this.FULL.Location = new System.Drawing.Point(0, 0);
            this.FULL.Name = "FULL";
            this.FULL.Size = new System.Drawing.Size(90, 37);
            this.FULL.TabIndex = 7;
            this.FULL.Text = "FULL";
            this.FULL.UseVisualStyleBackColor = true;
            this.FULL.Dock = DockStyle.Top;

            // 
            // HIDE
            // 
            this.HIDE.Location = new System.Drawing.Point(0, 0);
            this.HIDE.Name = "HIDE";
            this.HIDE.Size = new System.Drawing.Size(90, 37);
            this.HIDE.TabIndex = 8;
            this.HIDE.Text = "HIDE";
            this.HIDE.UseVisualStyleBackColor = true;
            this.HIDE.Dock = DockStyle.Top;
            this.ResumeLayout(false);

        }


        delegate void SetChartCallback(double val1, double val2, double time);

        public void Refresh_chart(double val1, double val2, double time)
        //public void Refresh_chart()
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            //if (this.formsPlot1.InvokeRequired)
            //{
            //    // ...тогда создаем обратный вызов...
            //    SetChartCallback d = new SetChartCallback(Refresh_chart);
            //    this.Invoke(d, new object[] { val1, val2, time });
            //}
            // ...иначе все по старинке
            //else
            //{

                //dataX.Add(time);
                //dataY.Add(val1);
                ////formsPlot1.Plot.Clear();
                //Logger[0].Add(time, val1);
                //Logger[0].MarkerSize = 4;
                //Logger[0].Color = Color.Blue;
                //Logger[0].ManageAxisLimits = true;

                //Logger[1].Add(time, val2);
                //Logger[1].MarkerSize = 4;
                //Logger[1].Color = Color.Orange;
                //Logger[1].ManageAxisLimits = true;

                //formsPlot1.Plot.Legend(true);

                ////formsPlot1.Plot.AddPoint(time , val1 , Color.Blue);

                //formsPlot1.Refresh();

           // }
        }


        int i = 0;

        void Save_graph()
        {
            Directory.CreateDirectory("Chart");
            string temp = String.Format(@"Chart\Current_{4}_{1}_{2}_{3}.jpeg", i, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Day);
            //this.chart1.SaveImage(temp, System.Drawing.Imaging.ImageFormat.Jpeg);
            formsPlot1.Plot.SaveFig(temp);
            i++;

            /*  double[] data = ScottPlot.Generate.Sin();
              formsPlot1.Plot.AddSignal(data);



              formsPlot1.Plot.XLabel("Time[s]");
              formsPlot1.Plot.YLabel("Temperature[°C]");
              formsPlot1.Plot.Title("Temperature Sensor");


              formsPlot1.Refresh();*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save_graph();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clean_chart();
        }
        delegate void CleanChartCallback();

        public void Clean_chart()
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            //if (this.formsPlot1.InvokeRequired)
            //{
            //    // ...тогда создаем обратный вызов...
            //    CleanChartCallback d = new CleanChartCallback(Clean_chart);
            //    this.Invoke(d, new object[] { });
            //}
            // ...иначе все по старинке
            //else
            //{
                //Init_chart(); 
                //formsPlot1.Plot.Clear();
                //Logger[0].Clear();
                //Logger[1].Clear();
                //dataX.Clear();
                //dataY.Clear();
                //formsPlot1.Refresh();
            //}

        }


        private void EDIT_chart(char id, string rezim)
        {
            ScottPlot.Plottable.DataLogger temp_logger;
            //int temp_id = 0;
            //switch (id)
            //{
            //    case '1':
            //        temp_id = 0;
            //        break;
            //    case '2':
            //        temp_id = 1;
            //        break;
            //    case '3':
            //        temp_id = 2;
            //        break;
            //    case '4':
            //        temp_id = 3;
            //        break;
            //    case '5':
            //        temp_id = 4;
            //        break;
            //    case '6':
            //        temp_id = 5;
            //        break;
            //    case '7':
            //        temp_id = 6;
            //        break;
            //    case '8':
            //        temp_id = 7;
            //        break;
            //    case '9':
            //        temp_id = 8;
            //        break;


            //}
            //temp_logger = Logger[temp_id];
            //switch (rezim)
            //{
            //    case "FULL":
            //        Logger[0].ViewFull();
            //        break;
            //    case "SLIDE":
            //        Logger[0].ViewSlide(10);
            //        break;
            //    case "JUMP":
            //        Logger[0].ViewJump(10);
            //        break;
            //}
            //formsPlot1.Refresh();
        }


       
    }
}

