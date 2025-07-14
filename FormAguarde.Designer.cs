namespace Futtage
{
    partial class FormAguarde
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
            pictureBox1 = new PictureBox();
            progressBarUpload = new ProgressBar();
            lblPorcentagem = new Label();
            lbProcessandoVideos = new Label();
            lbYoutube = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Image = Properties.Resources.Football_Soccer_GIF_by_happydog;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(358, 200);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // progressBarUpload
            // 
            progressBarUpload.Location = new Point(12, 238);
            progressBarUpload.Name = "progressBarUpload";
            progressBarUpload.Size = new Size(358, 23);
            progressBarUpload.TabIndex = 1;
            // 
            // lblPorcentagem
            // 
            lblPorcentagem.AutoSize = true;
            lblPorcentagem.BackColor = Color.Transparent;
            lblPorcentagem.Location = new Point(246, 220);
            lblPorcentagem.Name = "lblPorcentagem";
            lblPorcentagem.Size = new Size(23, 15);
            lblPorcentagem.TabIndex = 2;
            lblPorcentagem.Text = "0%";
            lblPorcentagem.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbProcessandoVideos
            // 
            lbProcessandoVideos.AutoSize = true;
            lbProcessandoVideos.Location = new Point(78, 240);
            lbProcessandoVideos.Name = "lbProcessandoVideos";
            lbProcessandoVideos.Size = new Size(215, 15);
            lbProcessandoVideos.TabIndex = 3;
            lbProcessandoVideos.Text = "Processando vídeo, por favor aguarde...";
            // 
            // lbYoutube
            // 
            lbYoutube.AutoSize = true;
            lbYoutube.Location = new Point(54, 220);
            lbYoutube.Name = "lbYoutube";
            lbYoutube.Size = new Size(186, 15);
            lbYoutube.TabIndex = 4;
            lbYoutube.Text = "Fazendo upload para o YouTube...";
            // 
            // FormAguarde
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(382, 264);
            ControlBox = false;
            Controls.Add(lbYoutube);
            Controls.Add(lbProcessandoVideos);
            Controls.Add(lblPorcentagem);
            Controls.Add(progressBarUpload);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormAguarde";
            StartPosition = FormStartPosition.CenterParent;
            Load += FormAguarde_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private ProgressBar progressBarUpload;
        private Label lblPorcentagem;
        private Label lbProcessandoVideos;
        private Label lbYoutube;
    }
}