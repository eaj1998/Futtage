namespace Futtage
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSelecionarArquivo = new Button();
            openFileDialog1 = new OpenFileDialog();
            lstArquivosSelecionados = new ListBox();
            btnJuntarVideos = new Button();
            btnMoverCima = new Button();
            btnMoverBaixo = new Button();
            btnExcluirItem = new Button();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            btnFazerUpload = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelecionarArquivo
            // 
            btnSelecionarArquivo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnSelecionarArquivo.Cursor = Cursors.Hand;
            btnSelecionarArquivo.Location = new Point(3, 19);
            btnSelecionarArquivo.Name = "btnSelecionarArquivo";
            btnSelecionarArquivo.Size = new Size(886, 23);
            btnSelecionarArquivo.TabIndex = 0;
            btnSelecionarArquivo.Text = "Selecionar Arquivo...";
            btnSelecionarArquivo.UseVisualStyleBackColor = true;
            btnSelecionarArquivo.Click += button1_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Selecione um arquivo";
            // 
            // lstArquivosSelecionados
            // 
            lstArquivosSelecionados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstArquivosSelecionados.FormattingEnabled = true;
            lstArquivosSelecionados.ItemHeight = 15;
            lstArquivosSelecionados.Location = new Point(141, 22);
            lstArquivosSelecionados.Name = "lstArquivosSelecionados";
            lstArquivosSelecionados.Size = new Size(659, 229);
            lstArquivosSelecionados.TabIndex = 1;
            lstArquivosSelecionados.SelectedIndexChanged += lstArquivosSelecionados_SelectedIndexChanged;
            // 
            // btnJuntarVideos
            // 
            btnJuntarVideos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnJuntarVideos.BackColor = SystemColors.InactiveBorder;
            btnJuntarVideos.Cursor = Cursors.Hand;
            btnJuntarVideos.Location = new Point(141, 22);
            btnJuntarVideos.Name = "btnJuntarVideos";
            btnJuntarVideos.Size = new Size(659, 23);
            btnJuntarVideos.TabIndex = 2;
            btnJuntarVideos.Text = "Juntar Vídeos";
            btnJuntarVideos.UseVisualStyleBackColor = false;
            btnJuntarVideos.Click += btnJuntarVideos_Click;
            // 
            // btnMoverCima
            // 
            btnMoverCima.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoverCima.Location = new Point(806, 22);
            btnMoverCima.Name = "btnMoverCima";
            btnMoverCima.Size = new Size(40, 40);
            btnMoverCima.TabIndex = 3;
            btnMoverCima.Text = "↑";
            btnMoverCima.UseVisualStyleBackColor = true;
            btnMoverCima.Click += btnMoverCima_Click;
            // 
            // btnMoverBaixo
            // 
            btnMoverBaixo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoverBaixo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMoverBaixo.Location = new Point(806, 68);
            btnMoverBaixo.Name = "btnMoverBaixo";
            btnMoverBaixo.Size = new Size(40, 40);
            btnMoverBaixo.TabIndex = 4;
            btnMoverBaixo.Text = "↓";
            btnMoverBaixo.UseVisualStyleBackColor = true;
            btnMoverBaixo.Click += btnMoverBaixo_Click;
            // 
            // btnExcluirItem
            // 
            btnExcluirItem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcluirItem.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExcluirItem.Location = new Point(806, 114);
            btnExcluirItem.Name = "btnExcluirItem";
            btnExcluirItem.Size = new Size(40, 40);
            btnExcluirItem.TabIndex = 5;
            btnExcluirItem.Text = "X";
            btnExcluirItem.UseVisualStyleBackColor = true;
            btnExcluirItem.Click += btnExcluirItem_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSelecionarArquivo);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(3, 6, 3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(892, 91);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Passo 1: Selecionar os Vídeos";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lstArquivosSelecionados);
            groupBox2.Controls.Add(btnMoverCima);
            groupBox2.Controls.Add(btnExcluirItem);
            groupBox2.Controls.Add(btnMoverBaixo);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 91);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(892, 359);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Passo 2: Organizar a Ordem";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnFazerUpload);
            groupBox3.Controls.Add(btnJuntarVideos);
            groupBox3.Dock = DockStyle.Bottom;
            groupBox3.Location = new Point(0, 353);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(892, 97);
            groupBox3.TabIndex = 8;
            groupBox3.TabStop = false;
            groupBox3.Text = "Passo 3: Ação Final";
            // 
            // btnFazerUpload
            // 
            btnFazerUpload.Location = new Point(141, 51);
            btnFazerUpload.Name = "btnFazerUpload";
            btnFazerUpload.Size = new Size(659, 23);
            btnFazerUpload.TabIndex = 3;
            btnFazerUpload.Text = "Fazer Upload para o YouTube";
            btnFazerUpload.UseVisualStyleBackColor = true;
            btnFazerUpload.Click += btnFazerUpload_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(892, 450);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Futtage";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnSelecionarArquivo;
        private OpenFileDialog openFileDialog1;
        private ListBox lstArquivosSelecionados;
        private Button btnJuntarVideos;
        private Button btnMoverCima;
        private Button btnMoverBaixo;
        private Button btnExcluirItem;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private Button btnFazerUpload;
    }
}
