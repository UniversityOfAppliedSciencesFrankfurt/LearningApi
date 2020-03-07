// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace EuclideanFilterWinFormApp
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        /// 

        private void InitializeComponent()
        {
            this.choosePictureTextBox = new System.Windows.Forms.TextBox();
            this.choosePictureButton = new System.Windows.Forms.Button();
            this.originalPictureBox = new System.Windows.Forms.PictureBox();
            this.filteredPictureBox = new System.Windows.Forms.PictureBox();
            this.redTrackBar = new System.Windows.Forms.TrackBar();
            this.greenTrackBar = new System.Windows.Forms.TrackBar();
            this.blueTrackBar = new System.Windows.Forms.TrackBar();
            this.radiusTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.showCurrentValueGreenLabel = new System.Windows.Forms.Label();
            this.showCurrentValueBlueLabel = new System.Windows.Forms.Label();
            this.showCurrentValueRadiusLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.updatePictureWithNewValuesButton = new System.Windows.Forms.Button();
            this.showCurrentValueRedLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filteredPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radiusTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // choosePictureTextBox
            // 
            this.choosePictureTextBox.Location = new System.Drawing.Point(20, 20);
            this.choosePictureTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.choosePictureTextBox.Name = "choosePictureTextBox";
            this.choosePictureTextBox.Size = new System.Drawing.Size(762, 26);
            this.choosePictureTextBox.TabIndex = 0;
            // 
            // choosePictureButton
            // 
            this.choosePictureButton.Location = new System.Drawing.Point(792, 15);
            this.choosePictureButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.choosePictureButton.Name = "choosePictureButton";
            this.choosePictureButton.Size = new System.Drawing.Size(148, 35);
            this.choosePictureButton.TabIndex = 1;
            this.choosePictureButton.Text = "Choose picture...";
            this.choosePictureButton.UseVisualStyleBackColor = true;
            this.choosePictureButton.Click += new System.EventHandler(this.choosePictureButton_Click);
            // 
            // originalPictureBox
            // 
            this.originalPictureBox.Location = new System.Drawing.Point(112, 82);
            this.originalPictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.originalPictureBox.Name = "originalPictureBox";
            this.originalPictureBox.Size = new System.Drawing.Size(742, 546);
            this.originalPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.originalPictureBox.TabIndex = 2;
            this.originalPictureBox.TabStop = false;
            // 
            // filteredPictureBox
            // 
            this.filteredPictureBox.Location = new System.Drawing.Point(964, 82);
            this.filteredPictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.filteredPictureBox.Name = "filteredPictureBox";
            this.filteredPictureBox.Size = new System.Drawing.Size(747, 546);
            this.filteredPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.filteredPictureBox.TabIndex = 3;
            this.filteredPictureBox.TabStop = false;
            // 
            // redTrackBar
            // 
            this.redTrackBar.AccessibleRole = System.Windows.Forms.AccessibleRole.Cell;
            this.redTrackBar.Location = new System.Drawing.Point(660, 737);
            this.redTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.redTrackBar.Maximum = 255;
            this.redTrackBar.Name = "redTrackBar";
            this.redTrackBar.Size = new System.Drawing.Size(525, 69);
            this.redTrackBar.TabIndex = 4;
            this.redTrackBar.Scroll += new System.EventHandler(this.redTrackBar_Scroll);
            // 
            // greenTrackBar
            // 
            this.greenTrackBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.greenTrackBar.Location = new System.Drawing.Point(660, 794);
            this.greenTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.greenTrackBar.Maximum = 255;
            this.greenTrackBar.Name = "greenTrackBar";
            this.greenTrackBar.Size = new System.Drawing.Size(525, 69);
            this.greenTrackBar.TabIndex = 5;
            this.greenTrackBar.Scroll += new System.EventHandler(this.greenTrackBar_Scroll);
            // 
            // blueTrackBar
            // 
            this.blueTrackBar.Location = new System.Drawing.Point(660, 846);
            this.blueTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.blueTrackBar.Maximum = 255;
            this.blueTrackBar.Name = "blueTrackBar";
            this.blueTrackBar.Size = new System.Drawing.Size(525, 69);
            this.blueTrackBar.TabIndex = 6;
            this.blueTrackBar.Scroll += new System.EventHandler(this.blueTrackBar_Scroll);
            // 
            // radiusTrackBar
            // 
            this.radiusTrackBar.Location = new System.Drawing.Point(660, 897);
            this.radiusTrackBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.radiusTrackBar.Maximum = 442;
            this.radiusTrackBar.Name = "radiusTrackBar";
            this.radiusTrackBar.Size = new System.Drawing.Size(525, 69);
            this.radiusTrackBar.TabIndex = 7;
            this.radiusTrackBar.Scroll += new System.EventHandler(this.radiusTrackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(568, 737);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 25);
            this.label1.TabIndex = 8;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(544, 794);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 25);
            this.label2.TabIndex = 9;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(564, 846);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 25);
            this.label3.TabIndex = 10;
            this.label3.Text = "Blue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(537, 897);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "Radius";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(632, 740);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(632, 797);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 20);
            this.label7.TabIndex = 14;
            this.label7.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(632, 852);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 20);
            this.label8.TabIndex = 15;
            this.label8.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(632, 903);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(18, 20);
            this.label9.TabIndex = 16;
            this.label9.Text = "0";
            // 
            // showCurrentValueGreenLabel
            // 
            this.showCurrentValueGreenLabel.AutoSize = true;
            this.showCurrentValueGreenLabel.Location = new System.Drawing.Point(1180, 797);
            this.showCurrentValueGreenLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.showCurrentValueGreenLabel.Name = "showCurrentValueGreenLabel";
            this.showCurrentValueGreenLabel.Size = new System.Drawing.Size(224, 20);
            this.showCurrentValueGreenLabel.TabIndex = 18;
            this.showCurrentValueGreenLabel.Text = "showCurrentValueGreenLabel";
            // 
            // showCurrentValueBlueLabel
            // 
            this.showCurrentValueBlueLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.showCurrentValueBlueLabel.AutoSize = true;
            this.showCurrentValueBlueLabel.Location = new System.Drawing.Point(1180, 852);
            this.showCurrentValueBlueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.showCurrentValueBlueLabel.Name = "showCurrentValueBlueLabel";
            this.showCurrentValueBlueLabel.Size = new System.Drawing.Size(211, 20);
            this.showCurrentValueBlueLabel.TabIndex = 19;
            this.showCurrentValueBlueLabel.Text = "showCurrentValueBlueLabel";
            // 
            // showCurrentValueRadiusLabel
            // 
            this.showCurrentValueRadiusLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.showCurrentValueRadiusLabel.AutoSize = true;
            this.showCurrentValueRadiusLabel.Location = new System.Drawing.Point(1180, 903);
            this.showCurrentValueRadiusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.showCurrentValueRadiusLabel.Name = "showCurrentValueRadiusLabel";
            this.showCurrentValueRadiusLabel.Size = new System.Drawing.Size(229, 20);
            this.showCurrentValueRadiusLabel.TabIndex = 20;
            this.showCurrentValueRadiusLabel.Text = "showCurrentValueRadiusLabel";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(112, 638);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 25);
            this.label5.TabIndex = 21;
            this.label5.Text = "Original Picture";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(960, 638);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(157, 25);
            this.label14.TabIndex = 22;
            this.label14.Text = "Filtered Picture";
            // 
            // updatePictureWithNewValuesButton
            // 
            this.updatePictureWithNewValuesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updatePictureWithNewValuesButton.Location = new System.Drawing.Point(1276, 638);
            this.updatePictureWithNewValuesButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.updatePictureWithNewValuesButton.Name = "updatePictureWithNewValuesButton";
            this.updatePictureWithNewValuesButton.Size = new System.Drawing.Size(260, 46);
            this.updatePictureWithNewValuesButton.TabIndex = 23;
            this.updatePictureWithNewValuesButton.Text = "Update Image";
            this.updatePictureWithNewValuesButton.UseVisualStyleBackColor = true;
            this.updatePictureWithNewValuesButton.Click += new System.EventHandler(this.updatePictureWithNewValuesButton_Click);
            // 
            // showCurrentValueRedLabel
            // 
            this.showCurrentValueRedLabel.AutoSize = true;
            this.showCurrentValueRedLabel.Location = new System.Drawing.Point(1180, 740);
            this.showCurrentValueRedLabel.Name = "showCurrentValueRedLabel";
            this.showCurrentValueRedLabel.Size = new System.Drawing.Size(209, 20);
            this.showCurrentValueRedLabel.TabIndex = 24;
            this.showCurrentValueRedLabel.Text = "showCurrentValueRedLabel";
            // 
            // Form1
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1875, 997);
            this.Controls.Add(this.showCurrentValueRedLabel);
            this.Controls.Add(this.updatePictureWithNewValuesButton);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.showCurrentValueRadiusLabel);
            this.Controls.Add(this.showCurrentValueBlueLabel);
            this.Controls.Add(this.showCurrentValueGreenLabel);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radiusTrackBar);
            this.Controls.Add(this.blueTrackBar);
            this.Controls.Add(this.greenTrackBar);
            this.Controls.Add(this.redTrackBar);
            this.Controls.Add(this.filteredPictureBox);
            this.Controls.Add(this.originalPictureBox);
            this.Controls.Add(this.choosePictureButton);
            this.Controls.Add(this.choosePictureTextBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.originalPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filteredPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radiusTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox choosePictureTextBox;
        private System.Windows.Forms.Button choosePictureButton;
        private System.Windows.Forms.PictureBox originalPictureBox;
        private System.Windows.Forms.PictureBox filteredPictureBox;
        private System.Windows.Forms.TrackBar greenTrackBar;
        private System.Windows.Forms.TrackBar blueTrackBar;
        private System.Windows.Forms.TrackBar radiusTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label showCurrentValueGreenLabel;
        private System.Windows.Forms.Label showCurrentValueBlueLabel;
        private System.Windows.Forms.Label showCurrentValueRadiusLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button updatePictureWithNewValuesButton;
        private System.Windows.Forms.TrackBar redTrackBar;
        private System.Windows.Forms.Label showCurrentValueRedLabel;
    }
}