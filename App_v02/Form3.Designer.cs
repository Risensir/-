namespace App_v02
{
    partial class Form3
    {
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
        public void InitializeComponent()
        {
            this.JUMP = new System.Windows.Forms.Button();
            this.SLIDE = new System.Windows.Forms.Button();
            this.CLEAN = new System.Windows.Forms.Button();
            this.SAVE = new System.Windows.Forms.Button();
            this.FULL = new System.Windows.Forms.Button();
            this.formsPlot1 = new ScottPlot.FormsPlot();
            this.SuspendLayout();
            // 
            // JUMP
            // 
            this.JUMP.Location = new System.Drawing.Point(556, 171);
            this.JUMP.Name = "JUMP";
            this.JUMP.Size = new System.Drawing.Size(90, 37);
            this.JUMP.TabIndex = 3;
            this.JUMP.Text = "JUMP";
            this.JUMP.UseVisualStyleBackColor = true;
            // 
            // SLIDE
            // 
            this.SLIDE.Location = new System.Drawing.Point(556, 130);
            this.SLIDE.Name = "SLIDE";
            this.SLIDE.Size = new System.Drawing.Size(90, 37);
            this.SLIDE.TabIndex = 4;
            this.SLIDE.Text = "SLIDE";
            this.SLIDE.UseVisualStyleBackColor = true;
            // 
            // CLEAN
            // 
            this.CLEAN.Location = new System.Drawing.Point(556, 250);
            this.CLEAN.Name = "CLEAN";
            this.CLEAN.Size = new System.Drawing.Size(90, 37);
            this.CLEAN.TabIndex = 5;
            this.CLEAN.Text = "Очистить";
            this.CLEAN.UseVisualStyleBackColor = true;
            // 
            // SAVE
            // 
            this.SAVE.Location = new System.Drawing.Point(556, 210);
            this.SAVE.Name = "SAVE";
            this.SAVE.Size = new System.Drawing.Size(90, 37);
            this.SAVE.TabIndex = 6;
            this.SAVE.Text = "Сохранить";
            this.SAVE.UseVisualStyleBackColor = true;
            // 
            // FULL
            // 
            this.FULL.Location = new System.Drawing.Point(556, 90);
            this.FULL.Name = "FULL";
            this.FULL.Size = new System.Drawing.Size(90, 37);
            this.FULL.TabIndex = 7;
            this.FULL.Text = "FULL";
            this.FULL.UseVisualStyleBackColor = true;
            // 
            // formsPlot1
            // 
            this.formsPlot1.Location = new System.Drawing.Point(0, 2);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(573, 326);
            this.formsPlot1.TabIndex = 2;
            // 
            // Form3
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(659, 317);
            this.Controls.Add(this.JUMP);
            this.Controls.Add(this.SLIDE);
            this.Controls.Add(this.CLEAN);
            this.Controls.Add(this.SAVE);
            this.Controls.Add(this.FULL);
            this.Controls.Add(this.formsPlot1);
            this.Name = "Form3";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button JUMP;
        private System.Windows.Forms.Button SLIDE;
        private System.Windows.Forms.Button CLEAN;
        private System.Windows.Forms.Button SAVE;
        private System.Windows.Forms.Button FULL;
        private ScottPlot.FormsPlot formsPlot1;
    }
}