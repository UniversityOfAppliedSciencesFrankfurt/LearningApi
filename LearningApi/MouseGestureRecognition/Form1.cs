using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MouseGestureRecognition
{
    public class Form1 : Form
    {
        MouseGestureRecognizer m_PredictionModule = null;

        public Form1()
        {
            InitializeComponent();
        }

        private List<Point> m_TrainingPoints = new List<Point>();
        private List<Point> m_TemplatePoints = new List<Point>();
        private List<Point> m_ValidationPoints = new List<Point>();
        private Button btnStartStop;
        private Label label1;
        private Label lblTrainingCounter;
        private Label lblResult;
        private Button btnReset;
        private System.Drawing.Point mouseDownPoint = System.Drawing.Point.Empty;

        private void shapePB_MouseDown(object sender, MouseEventArgs e)
        {
            lblResult.Visible = false;
            lblResult.SendToBack();
            m_TrainingPoints.Clear();
            m_ValidationPoints.Clear();
            mouseDownPoint = new System.Drawing.Point(e.X, e.Y);
        }

        private void shapePB_MouseUp(object sender, MouseEventArgs e)
        {
            addShape(e);
            mouseDownPoint = System.Drawing.Point.Empty;
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
                    if(res)
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

        private void shapePB_Paint(object sender, PaintEventArgs e)
        {
            foreach (Point point in m_TrainingPoints)
                point.Draw(e.Graphics);

            foreach (Point point in m_TemplatePoints)
                point.Draw(e.Graphics);

            foreach (Point point in m_ValidationPoints)
                point.Draw(e.Graphics);

        }

        private void addShape(MouseEventArgs e)
        {
            switch (btnStartStop.Text)
            {
                case "Check results":
                    if (lblTrainingCounter.Text == "0")
                    {
                        m_TemplatePoints.Add(new Point(new System.Drawing.Point(e.X, e.Y), 2, Brushes.Green));
                        m_TemplatePoints.Add(new Point(new System.Drawing.Point(e.X, e.Y), 2, Brushes.Green));
                    }
                    else
                    {
                        m_TrainingPoints.Add(new Point(new System.Drawing.Point(e.X, e.Y), 2, Brushes.Gray));
                        m_TrainingPoints.Add(new Point(new System.Drawing.Point(e.X, e.Y), 2, Brushes.Gray));
                    }
                    break;
                case "Continue training":
                    m_ValidationPoints.Add(new Point(new System.Drawing.Point(e.X, e.Y), 2, Brushes.Blue));
                    m_ValidationPoints.Add(new Point(new System.Drawing.Point(e.X, e.Y), 2, Brushes.Blue));
                    break;
            }
        }

        private void shapePB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownPoint.IsEmpty)
                return;
            //if (m_Points.Count > 0)
            //    m_Points.RemoveAt(m_Points.Count - 1);
            shapePB.Invalidate();
            addShape(e);
            shapePB.Invalidate();
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblTrainingCounter = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.shapePB)).BeginInit();
            this.SuspendLayout();
            // 
            // shapePB
            // 
            this.shapePB.BackColor = System.Drawing.SystemColors.Window;
            this.shapePB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shapePB.Location = new System.Drawing.Point(24, 29);
            this.shapePB.Margin = new System.Windows.Forms.Padding(6);
            this.shapePB.Name = "shapePB";
            this.shapePB.Size = new System.Drawing.Size(1428, 954);
            this.shapePB.TabIndex = 1;
            this.shapePB.TabStop = false;
            this.shapePB.Paint += new System.Windows.Forms.PaintEventHandler(this.shapePB_Paint);
            this.shapePB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.shapePB_MouseDown);
            this.shapePB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.shapePB_MouseMove);
            this.shapePB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.shapePB_MouseUp);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(1477, 64);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(129, 71);
            this.btnStartStop.TabIndex = 2;
            this.btnStartStop.Text = "Start training";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(1491, 273);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Trainings";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblTrainingCounter
            // 
            this.lblTrainingCounter.AutoSize = true;
            this.lblTrainingCounter.Enabled = false;
            this.lblTrainingCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 17.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrainingCounter.ForeColor = System.Drawing.Color.OrangeRed;
            this.lblTrainingCounter.Location = new System.Drawing.Point(1516, 218);
            this.lblTrainingCounter.Name = "lblTrainingCounter";
            this.lblTrainingCounter.Size = new System.Drawing.Size(51, 55);
            this.lblTrainingCounter.TabIndex = 4;
            this.lblTrainingCounter.Text = "0";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.BackColor = System.Drawing.Color.White;
            this.lblResult.Enabled = false;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 37.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(602, 379);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 114);
            this.lblResult.TabIndex = 5;
            this.lblResult.Visible = false;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(1491, 878);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(115, 52);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Visible = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1630, 1008);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.lblTrainingCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.shapePB);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "CSharpPaint";
            ((System.ComponentModel.ISupportInitialize)(this.shapePB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox shapePB;

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            btnReset.Visible = true;

            switch (btnStartStop.Text)
            {
                case "Start training":
                    m_PredictionModule = new MouseGestureRecognizer();
                    btnStartStop.Text = "Check results";
                    lblTrainingCounter.Text = "0";
                    break;
                case "Check results":
                    btnStartStop.Text = "Continue training";
                    break;
                case "Continue training":
                    btnStartStop.Text = "Check results";
                    break;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_ValidationPoints.Clear();
            m_TrainingPoints.Clear();
            m_TemplatePoints.Clear();
            lblTrainingCounter.Text = "0";
            lblResult.Visible =false;
            btnStartStop.Text = "Start training";
            m_PredictionModule = null;
            shapePB.Invalidate();
            lblResult.Invalidate();
            lblTrainingCounter.Invalidate();
        }
    }

    class Point 
    {
        System.Drawing.Point p;
        int radius;
        bool fill;

        public int X { get; set; }
        public int Y { get; set; }
        public Brush Color { get; set; }

        public Point(System.Drawing.Point p, int radius, Brush color)
        {
            this.X = p.X;
            this.Y = p.Y;

            this.p = p;
            this.radius = radius;
            this.Color = color;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillEllipse(this.Color, new Rectangle(new System.Drawing.Point(p.X - radius, p.Y - radius), new Size(2 * radius, 2 * radius)));

        }
    }
    
}