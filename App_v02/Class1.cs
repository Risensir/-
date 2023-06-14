using System;
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
    public class Form2 : Form
    {
        private Button button1;
        private Chart chart1;
        private Button button3;
        private Panel panel1;
        public Panel panel2;
        private Panel panel3;
        private Button button2;

        int Count = 0;
        private Label label1;
        private NumericUpDown numericUpDown1;
        int SIZE_INIT = 0;

        public void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.AlignmentOrientation = ((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations)((System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Vertical | System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal)));
            chartArea1.AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Lines;
            chartArea1.AxisX.LabelAutoFitMaxFontSize = 5;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisX.ScaleView.MinSize = 1D;
            chartArea1.AxisX.ScrollBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            chartArea1.AxisX.ScrollBar.ButtonColor = System.Drawing.Color.DarkGray;
            chartArea1.AxisX.ScrollBar.LineColor = System.Drawing.Color.Black;
            chartArea1.AxisX.ScrollBar.Size = 20D;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.IsSameFontSizeForAllAxes = true;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.BackColor = System.Drawing.Color.White;
            legend1.Enabled = false;
            legend1.HeaderSeparator = System.Windows.Forms.DataVisualization.Charting.LegendSeparatorStyle.Line;
            legend1.Name = "Legend1";
            legend1.Position.Auto = false;
            legend1.Position.Height = 9.725159F;
            legend1.Position.Width = 13.73057F;
            legend1.Position.X = 83.26943F;
            legend1.Position.Y = 3F;
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Black;
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(1)), true);
            series1.IsValueShownAsLabel = true;
            series1.LabelFormat = "F2";
            series1.Legend = "Legend1";
            series1.MarkerColor = System.Drawing.Color.Black;
            series1.MarkerImageTransparentColor = System.Drawing.Color.Black;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Square;
            series1.Name = "Ток ";
            series1.SmartLabelStyle.AllowOutsidePlotArea = System.Windows.Forms.DataVisualization.Charting.LabelOutsidePlotAreaStyle.Yes;
            series1.SmartLabelStyle.CalloutLineWidth = 0;
            series1.SmartLabelStyle.CalloutStyle = System.Windows.Forms.DataVisualization.Charting.LabelCalloutStyle.None;
            series1.SmartLabelStyle.MaxMovingDistance = 100D;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Fuchsia;
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(1)), true);
            series2.IsValueShownAsLabel = true;
            series2.LabelForeColor = System.Drawing.Color.Fuchsia;
            series2.LabelFormat = "F2";
            series2.Legend = "Legend1";
            series2.MarkerColor = System.Drawing.Color.Fuchsia;
            series2.MarkerImageTransparentColor = System.Drawing.Color.White;
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series2.Name = "Duty cycle ШИМ";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(1499, 609);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
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
            this.panel1.Location = new System.Drawing.Point(0, 630);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1409, 55);
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
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.AutoScrollMargin = new System.Drawing.Size(100, 0);
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1409, 630);
            this.panel2.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.chart1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1503, 613);
            this.panel3.TabIndex = 1;
            // 
            // Form2
            // 
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1409, 685);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public void Init_chart()
        {
            SIZE_INIT = panel3.Size.Width;
            chart1.ChartAreas[0].AxisY.Maximum = 5;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 10;
            chart1.ChartAreas[0].AxisY.Minimum = 0;

            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0";
            // chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Auto;

            //chart1.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.;       

            /*chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScrollBar.BackColor = Color.DarkGray;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.DimGray;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;


            chart1.ChartAreas[0].CursorX.AutoScroll = true;*/
            //chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            //chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.NotSet;

            //chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0,10);
            //chart1.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = 5;
            //chart1.ChartAreas[0].AxisX.Maximum = DateTime.Now;
            //chart1.ChartAreas[0].AxisX.MaximumAutoSize = 2000;
            //  chart1.ChartAreas[0].AxisX.Minimum = 0;
            // chart1.ChartAreas[0].AxisX.Interval = 5;
            //   chart1.ChartAreas[0].AxisY.Interval = 0.2;
            // chart1.ChartAreas[0].AxisX.ScaleView.Size = 2000;
            // chart1.ChartAreas[0].CursorX.AutoScroll = true;
            //chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
        }

        delegate void SetChartCallback(float val1, float val2, int time);

        public void Refresh_chart(float val1, float val2, int time)
        {
            // Если процесс пытающийся установить текст в элементах формы не тот же из которого они были созданы...
            if (this.chart1.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                SetChartCallback d = new SetChartCallback(Refresh_chart);
                this.Invoke(d, new object[] { val1, val2, time });
            }
            // ...иначе все по старинке
            else
            {
                this.panel3.Size = new Size(panel3.Width+50, panel3.Height);
                
                chart1.Series[0].Points.AddXY(time, (double)val1);

                chart1.Series[1].Points.AddXY(time, (double)val2);

                if (Count == 200)
                {
                    Count = 0;
                    Save_graph();
                    Clean_chart();
                    this.panel3.Size = new Size(SIZE_INIT, panel3.Height);
                }
                else
                    Count++;



                this.panel2.HorizontalScroll.Value = this.panel2.HorizontalScroll.Maximum;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TEST_APP.Form1.Press_COM || TEST_APP.Form1.Press_ETH)
            {
                if (!TEST_APP.Form1.timer1.Enabled)
                {
                    TEST_APP.Form1.timer1.Enabled = true;
                    button1.Text = "Стоп запрос";
                    Form1.time = 0;
                }
                else
                {
                    TEST_APP.Form1.timer1.Enabled = false;
                    button1.Text = "Запрос";
                }
            }
        }

        int i = 0;

        void Save_graph()
        {
            Directory.CreateDirectory("Chart");
            string temp = String.Format(@"Chart\Current_{4}_{1}_{2}_{3}.jpeg", i, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Day);
            this.chart1.SaveImage(temp, System.Drawing.Imaging.ImageFormat.Jpeg);
            i++;
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
            if (this.chart1.InvokeRequired)
            {
                // ...тогда создаем обратный вызов...
                CleanChartCallback d = new CleanChartCallback(Clean_chart);
                this.Invoke(d, new object[] { });
            }
            // ...иначе все по старинке
            else
            {
                chart1.Width = 751;
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Form1.timer1.Interval = (int)numericUpDown1.Value;
        }
    }
}
