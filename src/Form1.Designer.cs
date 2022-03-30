namespace fshwrite
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
            this.components = new System.ComponentModel.Container();
            this.openbtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Writebtn = new System.Windows.Forms.Button();
            this.readbtn = new System.Windows.Forms.Button();
            this.bmpbox = new System.Windows.Forms.TextBox();
            this.alphabox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.alphabtn = new System.Windows.Forms.Button();
            this.alphalbl = new System.Windows.Forms.Label();
            this.TypeBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dirBox1 = new System.Windows.Forms.TextBox();
            this.outdirbtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // openbtn
            // 
            this.openbtn.Location = new System.Drawing.Point(304, 55);
            this.openbtn.Name = "openbtn";
            this.openbtn.Size = new System.Drawing.Size(29, 23);
            this.openbtn.TabIndex = 1;
            this.openbtn.Text = "...";
            this.openbtn.UseVisualStyleBackColor = true;
            this.openbtn.Click += new System.EventHandler(this.openbtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Image files (*.bmp;*.png)|*.bmp; *.png";
            // 
            // Writebtn
            // 
            this.Writebtn.Location = new System.Drawing.Point(241, 231);
            this.Writebtn.Name = "Writebtn";
            this.Writebtn.Size = new System.Drawing.Size(75, 23);
            this.Writebtn.TabIndex = 2;
            this.Writebtn.Text = "Convert";
            this.Writebtn.UseVisualStyleBackColor = true;
            this.Writebtn.Click += new System.EventHandler(this.Writebtn_Click);
            // 
            // readbtn
            // 
            this.readbtn.Location = new System.Drawing.Point(47, 193);
            this.readbtn.Name = "readbtn";
            this.readbtn.Size = new System.Drawing.Size(75, 23);
            this.readbtn.TabIndex = 3;
            this.readbtn.Text = "Read";
            this.readbtn.UseVisualStyleBackColor = true;
            this.readbtn.Visible = false;
            // 
            // bmpbox
            // 
            this.bmpbox.Location = new System.Drawing.Point(69, 57);
            this.bmpbox.Name = "bmpbox";
            this.bmpbox.Size = new System.Drawing.Size(229, 20);
            this.bmpbox.TabIndex = 4;
            // 
            // alphabox
            // 
            this.alphabox.Location = new System.Drawing.Point(69, 83);
            this.alphabox.Name = "alphabox";
            this.alphabox.Size = new System.Drawing.Size(229, 20);
            this.alphabox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Bitmap = ";
            // 
            // alphabtn
            // 
            this.alphabtn.Location = new System.Drawing.Point(304, 83);
            this.alphabtn.Name = "alphabtn";
            this.alphabtn.Size = new System.Drawing.Size(29, 23);
            this.alphabtn.TabIndex = 7;
            this.alphabtn.Text = "...";
            this.alphabtn.UseVisualStyleBackColor = true;
            this.alphabtn.Click += new System.EventHandler(this.alphabtn_Click);
            // 
            // alphalbl
            // 
            this.alphalbl.AutoSize = true;
            this.alphalbl.Location = new System.Drawing.Point(12, 86);
            this.alphalbl.Name = "alphalbl";
            this.alphalbl.Size = new System.Drawing.Size(46, 13);
            this.alphalbl.TabIndex = 8;
            this.alphalbl.Text = "Alpha = ";
            // 
            // TypeBox1
            // 
            this.TypeBox1.FormattingEnabled = true;
            this.TypeBox1.Items.AddRange(new object[] {
            "24 Bit RGB (0:8:8:8)",
            "32 Bit ARGB (8:8:8:8)",
            "DXT1 Compressed, no Alpha",
            "DXT3 Compressed, with Alpha"});
            this.TypeBox1.Location = new System.Drawing.Point(12, 118);
            this.TypeBox1.Name = "TypeBox1";
            this.TypeBox1.Size = new System.Drawing.Size(146, 21);
            this.TypeBox1.TabIndex = 9;
            this.TypeBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(177, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Dir name = ";
            // 
            // dirBox1
            // 
            this.dirBox1.Location = new System.Drawing.Point(245, 122);
            this.dirBox1.MaxLength = 4;
            this.dirBox1.Name = "dirBox1";
            this.dirBox1.Size = new System.Drawing.Size(53, 20);
            this.dirBox1.TabIndex = 11;
            this.dirBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // outdirbtn
            // 
            this.outdirbtn.Location = new System.Drawing.Point(137, 231);
            this.outdirbtn.Name = "outdirbtn";
            this.outdirbtn.Size = new System.Drawing.Size(98, 23);
            this.outdirbtn.TabIndex = 12;
            this.outdirbtn.Text = "Output Directory";
            this.outdirbtn.UseVisualStyleBackColor = true;
            this.outdirbtn.Click += new System.EventHandler(this.outdirbtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 266);
            this.Controls.Add(this.outdirbtn);
            this.Controls.Add(this.dirBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TypeBox1);
            this.Controls.Add(this.alphalbl);
            this.Controls.Add(this.alphabtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.alphabox);
            this.Controls.Add(this.bmpbox);
            this.Controls.Add(this.readbtn);
            this.Controls.Add(this.Writebtn);
            this.Controls.Add(this.openbtn);
            this.Name = "Form1";
            this.Text = "fshwrite";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openbtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button Writebtn;
        private System.Windows.Forms.Button readbtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button alphabtn;
        private System.Windows.Forms.Label alphalbl;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ComboBox TypeBox1;
        internal System.Windows.Forms.TextBox bmpbox;
        internal System.Windows.Forms.TextBox alphabox;
        internal System.Windows.Forms.TextBox dirBox1;
        private System.Windows.Forms.Button outdirbtn;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

