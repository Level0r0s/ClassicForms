﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test.Examples;

namespace Test
{
    public class frmAnchorTest : Form
    {
        private Button button2;
        private Button button3;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button4;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private Button button1;

        public frmAnchorTest() : base()
        {            
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(931, 146);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 55);
            this.button1.TabIndex = 0;
            this.button1.Tag = "btn btn-primary";
            this.button1.Text = "Open Normal Modal";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(931, 85);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(160, 55);
            this.button2.TabIndex = 1;
            this.button2.Tag = "btn btn-primary";
            this.button2.Text = "Open Dialog Modal";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(1097, 83);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(84, 479);
            this.button3.TabIndex = 2;
            this.button3.Tag = "btn btn-light";
            this.button3.Text = "Anchor Top, Bottom, Right";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(102, 497);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(989, 65);
            this.button5.TabIndex = 4;
            this.button5.Tag = "btn btn-light";
            this.button5.Text = "Anchor Bottom, Left, Right";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.button6.Location = new System.Drawing.Point(12, 12);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(84, 550);
            this.button6.TabIndex = 5;
            this.button6.Tag = "btn btn-light";
            this.button6.Text = "Anchor Top, Bottom, Left";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.Location = new System.Drawing.Point(102, 12);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(989, 65);
            this.button7.TabIndex = 6;
            this.button7.Tag = "btn btn-light";
            this.button7.Text = "Anchor Top, Left, Right";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.Location = new System.Drawing.Point(1097, 12);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(84, 65);
            this.button8.TabIndex = 7;
            this.button8.Tag = "btn btn-danger";
            this.button8.Text = "Close";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button9.Location = new System.Drawing.Point(931, 207);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(160, 55);
            this.button9.TabIndex = 8;
            this.button9.Tag = "btn btn-primary";
            this.button9.Text = "Open Docking Test";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(102, 83);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(64, 57);
            this.button4.TabIndex = 9;
            this.button4.Tag = "btn btn-primary";
            this.button4.Text = "Calculator";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Location = new System.Drawing.Point(102, 146);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(823, 345);
            this.dataGridView1.TabIndex = 10;
            this.dataGridView1.Tag = "table table-striped table-bordered table-hover";
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "Test1";
            this.Column1.HeaderText = "Test 1";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Test2";
            this.Column2.HeaderText = "Test 2";
            this.Column2.Name = "Column2";
            // 
            // frmAnchorTest
            // 
            this.ClientSize = new System.Drawing.Size(1193, 574);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Location = new System.Drawing.Point(50, 50);
            this.Name = "frmAnchorTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "modal-content";
            this.Text = "Main Menu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmAnchorTest_Load);
            this.MouseEnter += new System.EventHandler(this.frmAnchorTest_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.frmAnchorTest_MouseLeave);
            this.Resize += new System.EventHandler(this.frmCustomer_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var x = new frmModal();
            x.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var x = new frmModal();
            x.ShowDialog();
        }

        private void frmCustomer_Resize(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Clicked the close button...");
            //this.Close();
        }

        private void frmAnchorTest_Load(object sender, EventArgs e)
        {
            var dt = new DataTable();

            dt.Columns.Add("Test1", typeof(string));
            dt.Columns.Add("Test2", typeof(string));

            for (int i = 0; i < 1000; i++)
            {
                var dr = dt.NewRow();

                dr[0] = i.ToString();
                dr[1] = i.ToString();

                dt.Rows.Add(dr);
            }

            dt.AcceptChanges();

            dataGridView1.DataSource = dt;
        }

        private void frmAnchorTest_MouseLeave(object sender, EventArgs e)
        {

        }

        private void frmAnchorTest_MouseEnter(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            var x = new frmDockingTest();
            x.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var x = new frmCalculator();
            x.Show();
        }
    }
}
