using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MouseGestureRecognition
{
    public class Form1 : Form
    {
        MouseGestureRecognizer m_PredictionModule = null;
        DateTime startTime = new DateTime();

        public Form1()
        {
            InitializeComponent();
        }
        
        private List<Coordinates> m_TrainingPoints = new List<Coordinates>();
        private List<Coordinates> m_TemplatePoints = new List<Coordinates>();
        private List<Coordinates> m_ValidationPoints = new List<Coordinates>();
        private Button btnStartStop;
        private Label lblTraining;
        private Label lblTrainingCounter;
        private Label lblResult;
        private Button btnReset;
        private CheckBox chkboxTime;
        private PictureBox shapePB;
        private Point mouseDownPoint = Point.Empty;        

        /// <summary>
        /// This Method is called when Mouse Pointer is down.
        /// It starts the drawing process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shapePB_MouseDown(object sender, MouseEventArgs e)
        {
            lblResult.Visible = false;
            lblResult.SendToBack();
            m_TrainingPoints.Clear();
            m_ValidationPoints.Clear();
            mouseDownPoint = new Point(e.X, e.Y);
            startTime = DateTime.Now;
        }

        /// <summary>
        /// This Method is called when Mouse Pointer is moved.
        /// It provides the Mouse Coordinates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shapePB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownPoint.IsEmpty)
                return;
            shapePB.Invalidate();
            addShape(e);
            shapePB.Invalidate();
        }

        /// <summary>
        /// This Method is called when Mouse Pointer is up.
        /// It stops the drawing process, gives training data to MouseGestureRecognizer
        /// and also asks for the prediction results from it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shapePB_MouseUp(object sender, MouseEventArgs e)
        {
            addShape(e);
            mouseDownPoint = Point.Empty;
            switch (btnStartStop.Text)
            {
                case "Check results":
                    if (lblTrainingCounter.Text == "0")
                        m_PredictionModule.Train(m_TemplatePoints);
                    else
                        m_PredictionModule.Train(m_TrainingPoints);

                    lblTrainingCounter.Text = (int.Parse(lblTrainingCounter.Text) + 1).ToString();
                    break;
                case "Continue training":
                    bool res = m_PredictionModule.IsThisRightGesture(m_ValidationPoints);
                    if (res)
                    {
                        lblResult.Text = "OK!";
                        lblResult.ForeColor = Color.Green;
                        lblResult.Enabled = true;
                        lblResult.Visible = true;
                        lblResult.BringToFront();
                    }
                    else
                    {
                        lblResult.Text = "Wrong!";
                        lblResult.ForeColor = Color.Red;
                        lblResult.Enabled = true;
                        lblResult.Visible = true;
                        lblResult.BringToFront();
                    }
                    break;
            }
        }

        /// <summary>
        /// Give Mouse Coordinate points to draw them in Paint Box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shapePB_Paint(object sender, PaintEventArgs e)
        {
            foreach (Coordinates point in m_TrainingPoints)
                point.Draw(e.Graphics);

            foreach (Coordinates point in m_TemplatePoints)
                point.Draw(e.Graphics);

            foreach (Coordinates point in m_ValidationPoints)
                point.Draw(e.Graphics);
        }

        /// <summary>
        /// This Method is called when Button named btnStartStop is clicked.
        /// It starts and stops the process for both training and prediction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            btnReset.Visible = true;

            switch (btnStartStop.Text)
            {
                case "Start training":
                    m_PredictionModule = new MouseGestureRecognizer();
                    btnStartStop.Text = "Check results";
                    lblTrainingCounter.Text = "0";
                    chkboxTime.Enabled = false;
                    break;
                case "Check results":
                    btnStartStop.Text = "Continue training";
                    break;
                case "Continue training":
                    btnStartStop.Text = "Check results";
                    break;
            }
        }

        /// <summary>
        /// This Method is called when Button btnReset is clicked.
        /// It resets all training and prediction data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            m_ValidationPoints.Clear();
            m_TrainingPoints.Clear();
            m_TemplatePoints.Clear();
            lblTrainingCounter.Text = "0";
            lblResult.Visible = false;
            btnStartStop.Text = "Start training";
            chkboxTime.Enabled = true;
            chkboxTime.Checked = false;
            m_PredictionModule = null;
            shapePB.Invalidate();
            lblResult.Invalidate();
            lblTrainingCounter.Invalidate();
        }        

        /// <summary>
        /// Add Mouse Coordinates for drawing, training and prediction.
        /// </summary>
        /// <param name="e"></param>
        private void addShape(MouseEventArgs e)
        {
            switch (btnStartStop.Text)
            {
                case "Check results":
                    if (lblTrainingCounter.Text == "0")
                    {
                        if(chkboxTime.Checked)
                        {
                            m_TemplatePoints.Add(new Coordinates(new Point(e.X, e.Y), Math.Round((DateTime.Now - startTime).TotalMilliseconds), 2, Brushes.Green));
                            m_TemplatePoints.Add(new Coordinates(new Point(e.X, e.Y), Math.Round((DateTime.Now - startTime).TotalMilliseconds), 2, Brushes.Green));
                        }
                        else
                        {
                            m_TemplatePoints.Add(new Coordinates(new Point(e.X, e.Y), 0, 2, Brushes.Green));
                            m_TemplatePoints.Add(new Coordinates(new Point(e.X, e.Y), 0, 2, Brushes.Green));
                        }                        
                    }
                    else
                    {
                        if (chkboxTime.Checked)
                        {
                            m_TrainingPoints.Add(new Coordinates(new Point(e.X, e.Y), Math.Round((DateTime.Now - startTime).TotalMilliseconds), 2, Brushes.Gray));
                            m_TrainingPoints.Add(new Coordinates(new Point(e.X, e.Y), Math.Round((DateTime.Now - startTime).TotalMilliseconds), 2, Brushes.Gray));
                        }
                        else
                        {
                            m_TrainingPoints.Add(new Coordinates(new Point(e.X, e.Y), 0, 2, Brushes.Gray));
                            m_TrainingPoints.Add(new Coordinates(new Point(e.X, e.Y), 0, 2, Brushes.Gray));
                        }
                        
                    }
                    break;
                case "Continue training":                    
                    if (chkboxTime.Checked)
                    {
                        m_ValidationPoints.Add(new Coordinates(new Point(e.X, e.Y), Math.Round((DateTime.Now - startTime).TotalMilliseconds), 2, Brushes.Blue));
                        m_ValidationPoints.Add(new Coordinates(new Point(e.X, e.Y), Math.Round((DateTime.Now - startTime).TotalMilliseconds), 2, Brushes.Blue));
                    }
                    else
                    {
                        m_ValidationPoints.Add(new Coordinates(new Point(e.X, e.Y), 0, 2, Brushes.Blue));
                        m_ValidationPoints.Add(new Coordinates(new Point(e.X, e.Y), 0, 2, Brushes.Blue));
                    }
                    break;
            }
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
            this.shapePB = new System.Windows.Forms.PictureBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lblTraining = new System.Windows.Forms.Label();
            this.lblTrainingCounter = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.chkboxTime = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.shapePB)).BeginInit();
            this.SuspendLayout();
            // 
            // shapePB
            // 
            this.shapePB.BackColor = System.Drawing.SystemColors.Window;
            this.shapePB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shapePB.Location = new System.Drawing.Point(12, 15);
            this.shapePB.Name = "shapePB";
            this.shapePB.Size = new System.Drawing.Size(715, 497);
            this.shapePB.TabIndex = 1;
            this.shapePB.TabStop = false;
            this.shapePB.Paint += new System.Windows.Forms.PaintEventHandler(this.shapePB_Paint);
            this.shapePB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.shapePB_MouseDown);
            this.shapePB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.shapePB_MouseMove);
            this.shapePB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.shapePB_MouseUp);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(766, 33);
            this.btnStartStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(64, 37);
            this.btnStartStop.TabIndex = 2;
            this.btnStartStop.Text = "Start training";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // lblTraining
            // 
            this.lblTraining.AutoSize = true;
            this.lblTraining.Enabled = false;
            this.lblTraining.Location = new System.Drawing.Point(774, 142);
            this.lblTraining.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTraining.Name = "lblTraining";
            this.lblTraining.Size = new System.Drawing.Size(50, 13);
            this.lblTraining.TabIndex = 3;
            this.lblTraining.Text = "Trainings";
            // 
            // lblTrainingCounter
            // 
            this.lblTrainingCounter.AutoSize = true;
            this.lblTrainingCounter.Enabled = false;
            this.lblTrainingCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 17.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrainingCounter.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblTrainingCounter.Location = new System.Drawing.Point(786, 113);
            this.lblTrainingCounter.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTrainingCounter.Name = "lblTrainingCounter";
            this.lblTrainingCounter.Size = new System.Drawing.Size(26, 29);
            this.lblTrainingCounter.TabIndex = 4;
            this.lblTrainingCounter.Text = "0";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.BackColor = System.Drawing.Color.White;
            this.lblResult.Enabled = false;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 37.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(301, 197);
            this.lblResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 59);
            this.lblResult.TabIndex = 5;
            this.lblResult.Visible = false;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(772, 457);
            this.btnReset.Margin = new System.Windows.Forms.Padding(2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(58, 27);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // chkboxTime
            // 
            this.chkboxTime.AutoSize = true;
            this.chkboxTime.Location = new System.Drawing.Point(739, 281);
            this.chkboxTime.Name = "chkboxTime";
            this.chkboxTime.Size = new System.Drawing.Size(125, 17);
            this.chkboxTime.TabIndex = 7;
            this.chkboxTime.Text = "Add Time Coordinate";
            this.chkboxTime.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 523);
            this.Controls.Add(this.chkboxTime);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblTrainingCounter);
            this.Controls.Add(this.lblTraining);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.shapePB);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "CSharpPaint";
            ((System.ComponentModel.ISupportInitialize)(this.shapePB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion        

    }

    /// <summary>
    /// Provides different parameters to Mouse Coordinates for
    /// drawing, training and prediction.
    /// </summary>
    class Coordinates
    {
        Point p;
        int radius;

        public int X { get; set; }
        public int Y { get; set; }
        public double T { get; set; }
        public Brush Color { get; set; }        

        /// <summary>
        /// Takes different parameters for Mouse Coordinates.
        /// </summary>
        /// <param name="p">2D Space Coordinates</param>
        /// <param name="time">Time Coordinate</param>
        /// <param name="radius">Radius for Drawing</param>
        /// <param name="color">Color of Drawing</param>
        public Coordinates(Point p, double time, int radius, Brush color)
        {
            this.X = p.X;
            this.Y = p.Y;
            this.T = time;

            this.p = p;
            this.radius = radius;
            this.Color = color;
        }       
        
        /// <summary>
        /// Draw the Mouse Coordinates.
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillEllipse(this.Color, new Rectangle(new Point(p.X - radius, p.Y - radius), new Size(2 * radius, 2 * radius)));
        }
    }

}