using ScottPlot;
using ScottPlot.Demo.WinForms.WinFormsDemos;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System;
using System.Drawing;

namespace App_v02
{
    public partial class PLAY_PLOTS : Form
    {
        StreamReader file_reader;
        List <System.Windows.Forms.CheckBox> list_box = new List<System.Windows.Forms.CheckBox>();
        List<List<float>> data_Y = new List<List<float>>();
        List<int> data_X = new List<int>();
        TableLayoutPanel table_plot;
        public class user_Plot : FormsPlot
        {
            public ScottPlot.Plottable.DataLogger logger;
            public int index;
            public AxisLimits limits;
            public VLine vline;
            public HLine hline;
            public Size size;
        }

        List<user_Plot> list_plot = new List<user_Plot>();

        public class user_chBox : System.Windows.Forms.CheckBox
        {
            public user_Plot plot;
           
        }
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.table_global = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // table_global
            // 
            this.table_global.AutoScroll = true;
            this.table_global.ColumnCount = 2;
            this.table_global.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_global.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_global.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_global.Location = new System.Drawing.Point(0, 0);
            this.table_global.Name = "table_global";
            this.table_global.RowCount = 1;
            this.table_global.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_global.Size = new System.Drawing.Size(800, 450);
            this.table_global.TabIndex = 0;
            // 
            // PLAY_PLOTS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.table_global);
            this.Name = "PLAY_PLOTS";
            this.Text = "PLAY_PLOTS";
            this.ResumeLayout(false);

        }

        public void read_file(string file)
        {
            if ((file != null) & (file != ""))
            {
                try
                {
                    file_reader = new StreamReader(file);
                    string str = file_reader.ReadLine();
                    string[] str_arr = str.Split('\t');
                    TableLayoutPanel table_checkBox = new TableLayoutPanel();
                    table_checkBox.RowCount = str_arr.Length;
                    table_checkBox.Dock = DockStyle.Fill;
                    table_checkBox.AutoScroll = false;
                    table_checkBox.AutoSize = true;
                    table_global.ColumnStyles.Clear();
                    table_global.RowStyles.Clear();
                    table_global.AutoSize = true;
                    table_global.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                    table_global.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
                    table_global.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

                    table_plot = new TableLayoutPanel();
                    table_plot.RowCount = str_arr.Length;
                    table_plot.Dock = DockStyle.Fill;

                    //for(int i = 0; i < table_plot.RowCount - 1; i++)
                    //    table_plot.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
                    //table_plot.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

                    table_plot.AutoSize = true;
                    table_plot.AutoScroll = true;
                    table_plot.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                    table_global.Controls.Add(table_plot, 0, 0);
                    int index = 0;
                    foreach (string temp_string in str_arr)
                    {
                        if (temp_string != "")
                        {
                            user_chBox checkBox = new user_chBox();
                            checkBox.Name = temp_string;
                            checkBox.Text = temp_string;
                            checkBox.Checked = true;
                            checkBox.Dock = DockStyle.Fill;
                            table_checkBox.Controls.Add(checkBox);
                            list_box.Add(checkBox);

                            checkBox.CheckedChanged += CheckBox_CheckedChanged;

                            user_Plot plot = new user_Plot();
                            plot.Plot.YAxis.SetBoundary();
                            plot.Plot.XAxis.SetBoundary();

                            plot.AxesChanged += OnAxesChanged;
                            plot.Plot.Title(temp_string);
                            plot.logger = plot.Plot.AddDataLogger();

                            plot.logger.MarkerSize = 4;
                            plot.logger.Color = Color.Blue;
                            plot.index = index;
                            plot.MouseMove += new System.Windows.Forms.MouseEventHandler(plot_MouseMove);


                            list_plot.Add(plot);

                            checkBox.plot = plot;
                            plot.Dock = DockStyle.Fill;
                            table_plot.Controls.Add(plot);
                            index++;


                            VLine temp_vline = plot.Plot.AddVerticalLine(0);
                            //temp_vline.X = 0;
                            temp_vline.PositionLabel = true;

                            HLine temp_hline = plot.Plot.AddHorizontalLine(0);
                            //temp_hline.Y = 0;
                            temp_hline.PositionLabel = true;

                            plot.hline = temp_hline;
                            plot.vline = temp_vline;
                        }
                    }
                    table_global.Controls.Add(table_checkBox, 1, 0);




                    int init_time = -1;
                    string str_data = "";
                    while (file_reader.EndOfStream != true)
                    {
                        str_data = file_reader.ReadLine();
                        string[] str_arr_data = str_data.Split('\t');
                        if (str_arr_data.Length == str_arr.Length)
                        {
                            List<float> data_y_tmp = new List<float>();
                            for (int i = 1; i < str_arr_data.Length; i++)
                                data_y_tmp.Add(float.Parse(str_arr_data[i]));

                            data_Y.Add(data_y_tmp);
                            int time_now = 0;
                            string[] time_arr = str_arr_data[0].Split(':');
                            for (int i = time_arr.Length - 1; i >= 0; i--)
                            {
                                //if (i == time_arr.Length - 1)
                                //    time_now += int.Parse(time_arr[i]);
                                //else
                                if (i == time_arr.Length - 2)
                                    time_now += int.Parse(time_arr[i]) * 1000;
                                else
                                if (i == time_arr.Length - 3)
                                    time_now += int.Parse(time_arr[i]) * 1000 * 60;
                                else
                                if (i == time_arr.Length - 4)
                                    time_now += int.Parse(time_arr[i]) * 1000 * 60 * 60;


                            }
                            if (init_time == -1)
                                init_time = time_now;

                            data_X.Add(time_now - init_time);


                            foreach (user_Plot plot in list_plot)
                            {
                                plot.logger.Add(data_X[data_X.Count - 1], data_Y[data_Y.Count - 1][plot.index]);
                            }
                        }
                    }
                    foreach (user_Plot plot in list_plot)
                    {
                        plot.logger.ManageAxisLimits = true;

                        plot.Refresh();
                        plot.limits = plot.Plot.GetAxisLimits();
                        plot.logger.ManageAxisLimits = false;
                        plot.size = plot.Size;
                    }
                }
                catch
                {
                    string message = "Во время чтения файла произошла ошибка";
                    string caption = "Error";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;
                    result =  MessageBox.Show(message, caption, buttons);
                    this.Close();
                }

            }

        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            user_chBox chBox = sender as user_chBox;
            if (chBox.Checked)
            {
                //foreach (user_Plot plot in list_plot)
                //    plot.Height =  plot.Height - plot.Height/list_plot.Count;
                chBox.plot.Show();
                table_plot.RowCount++;
            }
            else
            {
                //foreach (user_Plot plot in list_plot)
                //    plot.Height =  plot.Height + plot.Height/list_plot.Count;
                chBox.plot.Hide();
                table_plot.RowCount--;
                
            }

            
            
            chBox.plot.Refresh();


        }
        #endregion
        private void OnAxesChanged(object sender, EventArgs e)
        {
            user_Plot changedPlot = sender as user_Plot;

            AxisLimits newAxisLimits = changedPlot.Plot.GetAxisLimits();
            
            foreach (user_Plot plot in list_plot)
            {
                if (plot == changedPlot)
                {
                    plot.limits = newAxisLimits;
                    continue;
                }

                AxisLimits newLimits = new AxisLimits(newAxisLimits.XMin, newAxisLimits.XMax , plot.limits.YMin , plot.limits.YMax);
                //plot.Plot.
                // disable events briefly to avoid an infinite loop

                //double xMin = plot.limits.XMin + (newAxisLimits.XMin - changedPlot.limits.XMin)/changedPlot.limits.XMin * plot.limits.XMin;
                //double xMax = plot.limits.XMax + (newAxisLimits.XMax - changedPlot.limits.XMax)/changedPlot.limits.XMax * plot.limits.XMax;
                //double yMin = plot.limits.YMin + (newAxisLimits.YMin - changedPlot.limits.YMin)/changedPlot.limits.YMin * plot.limits.YMin;
                //double yMax = plot.limits.YMax + (newAxisLimits.YMax - changedPlot.limits.YMax)/changedPlot.limits.YMax * plot.limits.YMax;

                //newLimits.WithX(xMin , xMax);
                //newLimits.WithY(yMin , yMax);
                //newLimits.
                //newLimits.WithX(newAxisLimits.XMin , newAxisLimits.XMax);
                //newLimits.WithY(plot.limits.YMin, plot.limits.YMax);

                plot.Configuration.AxesChangedEventEnabled = false;
                plot.Plot.SetAxisLimits(newLimits);
                plot.Render();
                plot.Configuration.AxesChangedEventEnabled = true;

                plot.limits = newLimits;
            }
        }

        private void plot_MouseMove(object sender, MouseEventArgs e)
        {
            user_Plot plt = (sender as user_Plot);
            (double x, double y) = plt.GetMouseCoordinates();
            // MyScatterPlot = plt.Plot.AddScatterPoints();
            plt.hline.Y = y;
            plt.vline.X = x;
            // plt.Plot.AddVerticalLine(x);
            plt.Refresh();
            //Console.WriteLine($"Mouse at ({x}, {y})");
        }

        private TableLayoutPanel table_global;
    }
}