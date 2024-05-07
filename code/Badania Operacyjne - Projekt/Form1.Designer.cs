
namespace Badania_Operacyjne___Projekt
{
    partial class Form1
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
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView_input_table = new System.Windows.Forms.DataGridView();
            this.Czynność = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Czas_trwania = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Od = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Do = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.Label();
            this.button_output = new System.Windows.Forms.Button();
            this.button_work_dir = new System.Windows.Forms.Button();
            this.pictureBox_legend = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_input_table)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_legend)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // dataGridView_input_table
            // 
            this.dataGridView_input_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_input_table.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Czynność,
            this.Czas_trwania,
            this.Od,
            this.Do});
            this.dataGridView_input_table.Location = new System.Drawing.Point(72, 95);
            this.dataGridView_input_table.Name = "dataGridView_input_table";
            this.dataGridView_input_table.Size = new System.Drawing.Size(463, 245);
            this.dataGridView_input_table.TabIndex = 1;
            // 
            // Czynność
            // 
            this.Czynność.HeaderText = "Activity";
            this.Czynność.Name = "Czynność";
            // 
            // Czas_trwania
            // 
            this.Czas_trwania.HeaderText = "Duration";
            this.Czas_trwania.Name = "Czas_trwania";
            // 
            // Od
            // 
            this.Od.HeaderText = "From";
            this.Od.Name = "Od";
            // 
            // Do
            // 
            this.Do.HeaderText = "To";
            this.Do.Name = "Do";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(72, 365);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 55);
            this.button1.TabIndex = 2;
            this.button1.Text = "save records";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // log
            // 
            this.log.AutoSize = true;
            this.log.Location = new System.Drawing.Point(69, 68);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(31, 13);
            this.log.TabIndex = 3;
            this.log.Text = "aaaa";
            // 
            // button_output
            // 
            this.button_output.Location = new System.Drawing.Point(420, 365);
            this.button_output.Name = "button_output";
            this.button_output.Size = new System.Drawing.Size(115, 55);
            this.button_output.TabIndex = 4;
            this.button_output.Text = "display output";
            this.button_output.UseVisualStyleBackColor = true;
            this.button_output.Click += new System.EventHandler(this.button_output_Click);
            // 
            // button_work_dir
            // 
            this.button_work_dir.Location = new System.Drawing.Point(298, 365);
            this.button_work_dir.Name = "button_work_dir";
            this.button_work_dir.Size = new System.Drawing.Size(115, 55);
            this.button_work_dir.TabIndex = 5;
            this.button_work_dir.Text = "see working directory";
            this.button_work_dir.UseVisualStyleBackColor = true;
            this.button_work_dir.Click += new System.EventHandler(this.button_work_dir_Click);
            // 
            // pictureBox_legend
            // 
            this.pictureBox_legend.Location = new System.Drawing.Point(599, 95);
            this.pictureBox_legend.Name = "pictureBox_legend";
            this.pictureBox_legend.Size = new System.Drawing.Size(267, 265);
            this.pictureBox_legend.TabIndex = 6;
            this.pictureBox_legend.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(347, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "If it is impossible to create CPM diagram with your data the app will crash";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 472);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox_legend);
            this.Controls.Add(this.button_work_dir);
            this.Controls.Add(this.button_output);
            this.Controls.Add(this.log);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView_input_table);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "CPM";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_input_table)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_legend)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView_input_table;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label log;
        private System.Windows.Forms.Button button_output;
        private System.Windows.Forms.Button button_work_dir;
        private System.Windows.Forms.PictureBox pictureBox_legend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Czynność;
        private System.Windows.Forms.DataGridViewTextBoxColumn Czas_trwania;
        private System.Windows.Forms.DataGridViewTextBoxColumn Od;
        private System.Windows.Forms.DataGridViewTextBoxColumn Do;
    }
}

