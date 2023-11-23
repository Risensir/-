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

namespace App_v02
{
    partial class Form3 : Form
    {

        int Count = 0;
        int SIZE_INIT = 0;

        public List<double> dataX = new List<double>();
        public List<double> dataY = new List<double>();
        private List<ScottPlot.Plottable.DataLogger> Logger = new List<ScottPlot.Plottable.DataLogger>();
        public Form3()
        {
            InitializeComponent();
        }
        

        public void Init_chart()
        {
            Logger.Add(formsPlot1.Plot.AddDataLogger(label: "Ток Анода"));
            Logger.Add(formsPlot1.Plot.AddDataLogger(label: "Ток Регулятора"));
            /*double[] data = ScottPlot.Generate.Sin();
            formsPlot1.Plot.AddSignal(data);
            */
            formsPlot1.Plot.AddHorizontalLine(2, Color.Red);
            formsPlot1.Plot.XLabel("Время");
            formsPlot1.Plot.YLabel("Ток Анода");
            formsPlot1.Plot.Title("Ток Анода");

            //Logger.ViewFull();
            formsPlot1.Refresh();

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

            dataX.Add(time);
            dataY.Add(val1);
            //formsPlot1.Plot.Clear();
            Logger[0].Add(time, val1);
            Logger[0].MarkerSize = 4;
            Logger[0].Color = Color.Blue;
            Logger[0].ManageAxisLimits = true;

            Logger[1].Add(time, val2);
            Logger[1].MarkerSize = 4;
            Logger[1].Color = Color.Orange;
            Logger[1].ManageAxisLimits = true;

            formsPlot1.Plot.Legend(true);

            //formsPlot1.Plot.AddPoint(time , val1 , Color.Blue);

            formsPlot1.Refresh();

            // }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (TEST_APP.Form1.Press_COM || TEST_APP.Form1.Press_ETH)
        //    {
        //        if (!TEST_APP.Form1.timer1.Enabled)
        //        {
        //            TEST_APP.Form1.timer1.Enabled = true;
        //            button1.Text = "Стоп запрос";
        //            Form1.time = 0;
        //        }
        //        else
        //        {
        //            TEST_APP.Form1.timer1.Enabled = false;
        //            button1.Text = "Запрос";
        //        }
        //    }
        //}

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
            Logger[0].Clear();
            Logger[1].Clear();
            dataX.Clear();
            dataY.Clear();
            formsPlot1.Refresh();
            //}

        }

        //private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        //{
        //    Form1.timer1.Interval = (int)numericUpDown1.Value;
        //}

        private void EDIT_chart(char id, string rezim)
        {
            ScottPlot.Plottable.DataLogger temp_logger;
            int temp_id = 0;
            switch (id)
            {
                case '1':
                    temp_id = 0;
                    break;
                case '2':
                    temp_id = 1;
                    break;
                case '3':
                    temp_id = 2;
                    break;
                case '4':
                    temp_id = 3;
                    break;
                case '5':
                    temp_id = 4;
                    break;
                case '6':
                    temp_id = 5;
                    break;
                case '7':
                    temp_id = 6;
                    break;
                case '8':
                    temp_id = 7;
                    break;
                case '9':
                    temp_id = 8;
                    break;


            }
            temp_logger = Logger[temp_id];
            switch (rezim)
            {
                case "FULL":
                    temp_logger.ViewFull();
                    break;
                case "SLIDE":
                    temp_logger.ViewSlide(10);
                    break;
                case "JUMP":
                    temp_logger.ViewJump(10);
                    break;
            }
            formsPlot1.Refresh();
        }


        private void FULL_Click(object sender, EventArgs e)
        {
            Button temp = sender as Button;
            EDIT_chart(temp.Name.Last<char>(), "FULL");
        }

        private void SLIDE_Click(object sender, EventArgs e)
        {
            Button temp = sender as Button;
            EDIT_chart(temp.Name.Last<char>(), "SLIDE");
        }

        private void JUMP_Click(object sender, EventArgs e)
        {
            Button temp = sender as Button;
            EDIT_chart(temp.Name.Last<char>(), "JUMP");
        }

        private void CLEAN_Click(object sender, EventArgs e)
        {
            Clean_chart();
        }
    }
}
