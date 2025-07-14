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
            tabControlPrincipal = new TabControl();
            tabPageSelecao = new TabPage();
            btnProximoPasso1 = new Button();
            btnExcluirItem = new Button();
            btnMoverCima = new Button();
            lstArquivosSelecionados = new ListBox();
            btnMoverBaixo = new Button();
            tabPageJuntar = new TabPage();
            btnVoltarPasso1 = new Button();
            btnJuntarVideos = new Button();
            tabPageCorte = new TabPage();
            btnPularCorte = new Button();
            txtFimCorte = new TextBox();
            txtInicioCorte = new TextBox();
            lbFimCorte = new Label();
            lbInicioCorte = new Label();
            btnCortarVideo = new Button();
            tabPageCapa = new TabPage();
            btnPulaPassoFinal = new Button();
            lblCaminhoCapa = new Label();
            pictureBoxCapa = new PictureBox();
            btnSelecionarCapa = new Button();
            tabPageUpload = new TabPage();
            btnFazerUpload = new Button();
            tabControlPrincipal.SuspendLayout();
            tabPageSelecao.SuspendLayout();
            tabPageJuntar.SuspendLayout();
            tabPageCorte.SuspendLayout();
            tabPageCapa.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCapa).BeginInit();
            tabPageUpload.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelecionarArquivo
            // 
            btnSelecionarArquivo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnSelecionarArquivo.Cursor = Cursors.Hand;
            btnSelecionarArquivo.Location = new Point(11, 3);
            btnSelecionarArquivo.Name = "btnSelecionarArquivo";
            btnSelecionarArquivo.Size = new Size(675, 23);
            btnSelecionarArquivo.TabIndex = 0;
            btnSelecionarArquivo.Text = "Selecionar Arquivo...";
            btnSelecionarArquivo.UseVisualStyleBackColor = true;
            btnSelecionarArquivo.Click += btnSelecionarArquivo_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Selecione um arquivo";
            // 
            // tabControlPrincipal
            // 
            tabControlPrincipal.Controls.Add(tabPageSelecao);
            tabControlPrincipal.Controls.Add(tabPageJuntar);
            tabControlPrincipal.Controls.Add(tabPageCorte);
            tabControlPrincipal.Controls.Add(tabPageCapa);
            tabControlPrincipal.Controls.Add(tabPageUpload);
            tabControlPrincipal.Dock = DockStyle.Fill;
            tabControlPrincipal.Location = new Point(0, 0);
            tabControlPrincipal.Name = "tabControlPrincipal";
            tabControlPrincipal.SelectedIndex = 0;
            tabControlPrincipal.Size = new Size(708, 450);
            tabControlPrincipal.TabIndex = 1;
            tabControlPrincipal.Selecting += tabControl1_Selecting;
            // 
            // tabPageSelecao
            // 
            tabPageSelecao.Controls.Add(btnProximoPasso1);
            tabPageSelecao.Controls.Add(btnSelecionarArquivo);
            tabPageSelecao.Controls.Add(btnExcluirItem);
            tabPageSelecao.Controls.Add(btnMoverCima);
            tabPageSelecao.Controls.Add(lstArquivosSelecionados);
            tabPageSelecao.Controls.Add(btnMoverBaixo);
            tabPageSelecao.Location = new Point(4, 24);
            tabPageSelecao.Name = "tabPageSelecao";
            tabPageSelecao.Padding = new Padding(3);
            tabPageSelecao.Size = new Size(700, 422);
            tabPageSelecao.TabIndex = 0;
            tabPageSelecao.Text = "Passo 1: Seleção e Ordem";
            tabPageSelecao.UseVisualStyleBackColor = true;
            // 
            // btnProximoPasso1
            // 
            btnProximoPasso1.Location = new Point(516, 391);
            btnProximoPasso1.Name = "btnProximoPasso1";
            btnProximoPasso1.Size = new Size(181, 23);
            btnProximoPasso1.TabIndex = 6;
            btnProximoPasso1.Text = "Próximo Passo";
            btnProximoPasso1.UseVisualStyleBackColor = true;
            btnProximoPasso1.Click += btnProximoPasso1_Click;
            // 
            // btnExcluirItem
            // 
            btnExcluirItem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExcluirItem.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExcluirItem.Location = new Point(646, 124);
            btnExcluirItem.Name = "btnExcluirItem";
            btnExcluirItem.Size = new Size(40, 40);
            btnExcluirItem.TabIndex = 5;
            btnExcluirItem.Text = "X";
            btnExcluirItem.UseVisualStyleBackColor = true;
            btnExcluirItem.Click += btnExcluirItem_Click;
            // 
            // btnMoverCima
            // 
            btnMoverCima.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoverCima.Location = new Point(646, 32);
            btnMoverCima.Name = "btnMoverCima";
            btnMoverCima.Size = new Size(40, 40);
            btnMoverCima.TabIndex = 3;
            btnMoverCima.Text = "↑";
            btnMoverCima.UseVisualStyleBackColor = true;
            btnMoverCima.Click += btnMoverCima_Click;
            // 
            // lstArquivosSelecionados
            // 
            lstArquivosSelecionados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstArquivosSelecionados.FormattingEnabled = true;
            lstArquivosSelecionados.ItemHeight = 15;
            lstArquivosSelecionados.Location = new Point(11, 32);
            lstArquivosSelecionados.Name = "lstArquivosSelecionados";
            lstArquivosSelecionados.Size = new Size(629, 319);
            lstArquivosSelecionados.TabIndex = 1;
            lstArquivosSelecionados.SelectedIndexChanged += lstArquivosSelecionados_SelectedIndexChanged;
            // 
            // btnMoverBaixo
            // 
            btnMoverBaixo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoverBaixo.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMoverBaixo.Location = new Point(646, 78);
            btnMoverBaixo.Name = "btnMoverBaixo";
            btnMoverBaixo.Size = new Size(40, 40);
            btnMoverBaixo.TabIndex = 4;
            btnMoverBaixo.Text = "↓";
            btnMoverBaixo.UseVisualStyleBackColor = true;
            btnMoverBaixo.Click += btnMoverBaixo_Click;
            // 
            // tabPageJuntar
            // 
            tabPageJuntar.Controls.Add(btnVoltarPasso1);
            tabPageJuntar.Controls.Add(btnJuntarVideos);
            tabPageJuntar.Location = new Point(4, 24);
            tabPageJuntar.Name = "tabPageJuntar";
            tabPageJuntar.Padding = new Padding(3);
            tabPageJuntar.Size = new Size(700, 422);
            tabPageJuntar.TabIndex = 1;
            tabPageJuntar.Text = "Passo 2: Juntar Vídeos";
            tabPageJuntar.UseVisualStyleBackColor = true;
            // 
            // btnVoltarPasso1
            // 
            btnVoltarPasso1.Location = new Point(8, 391);
            btnVoltarPasso1.Name = "btnVoltarPasso1";
            btnVoltarPasso1.Size = new Size(197, 23);
            btnVoltarPasso1.TabIndex = 3;
            btnVoltarPasso1.Text = "Voltar";
            btnVoltarPasso1.UseVisualStyleBackColor = true;
            btnVoltarPasso1.Click += btnVoltarPasso1_Click;
            // 
            // btnJuntarVideos
            // 
            btnJuntarVideos.BackColor = SystemColors.InactiveBorder;
            btnJuntarVideos.Cursor = Cursors.Hand;
            btnJuntarVideos.Dock = DockStyle.Top;
            btnJuntarVideos.Location = new Point(3, 3);
            btnJuntarVideos.Name = "btnJuntarVideos";
            btnJuntarVideos.Size = new Size(694, 23);
            btnJuntarVideos.TabIndex = 2;
            btnJuntarVideos.Text = "Juntar Vídeos";
            btnJuntarVideos.UseVisualStyleBackColor = false;
            btnJuntarVideos.Click += btnJuntarVideos_Click;
            // 
            // tabPageCorte
            // 
            tabPageCorte.Controls.Add(btnPularCorte);
            tabPageCorte.Controls.Add(txtFimCorte);
            tabPageCorte.Controls.Add(txtInicioCorte);
            tabPageCorte.Controls.Add(lbFimCorte);
            tabPageCorte.Controls.Add(lbInicioCorte);
            tabPageCorte.Controls.Add(btnCortarVideo);
            tabPageCorte.Location = new Point(4, 24);
            tabPageCorte.Name = "tabPageCorte";
            tabPageCorte.Size = new Size(700, 422);
            tabPageCorte.TabIndex = 2;
            tabPageCorte.Text = "Passo 3: Corte Final";
            tabPageCorte.UseVisualStyleBackColor = true;
            // 
            // btnPularCorte
            // 
            btnPularCorte.Dock = DockStyle.Bottom;
            btnPularCorte.Location = new Point(0, 376);
            btnPularCorte.Name = "btnPularCorte";
            btnPularCorte.Size = new Size(700, 23);
            btnPularCorte.TabIndex = 5;
            btnPularCorte.Text = "Pular Corte";
            btnPularCorte.UseVisualStyleBackColor = true;
            btnPularCorte.Click += btnPularCorte_Click;
            // 
            // txtFimCorte
            // 
            txtFimCorte.Location = new Point(433, 9);
            txtFimCorte.Name = "txtFimCorte";
            txtFimCorte.Size = new Size(262, 23);
            txtFimCorte.TabIndex = 4;
            // 
            // txtInicioCorte
            // 
            txtInicioCorte.Location = new Point(122, 9);
            txtInicioCorte.Name = "txtInicioCorte";
            txtInicioCorte.Size = new Size(189, 23);
            txtInicioCorte.TabIndex = 3;
            // 
            // lbFimCorte
            // 
            lbFimCorte.AutoSize = true;
            lbFimCorte.Location = new Point(328, 9);
            lbFimCorte.Name = "lbFimCorte";
            lbFimCorte.Size = new Size(99, 15);
            lbFimCorte.TabIndex = 2;
            lbFimCorte.Text = "Fim (HH:MM:SS):";
            // 
            // lbInicioCorte
            // 
            lbInicioCorte.AutoSize = true;
            lbInicioCorte.Location = new Point(8, 9);
            lbInicioCorte.Name = "lbInicioCorte";
            lbInicioCorte.Size = new Size(108, 15);
            lbInicioCorte.TabIndex = 1;
            lbInicioCorte.Text = "Início (HH:MM:SS):";
            // 
            // btnCortarVideo
            // 
            btnCortarVideo.Dock = DockStyle.Bottom;
            btnCortarVideo.Location = new Point(0, 399);
            btnCortarVideo.Name = "btnCortarVideo";
            btnCortarVideo.Size = new Size(700, 23);
            btnCortarVideo.TabIndex = 0;
            btnCortarVideo.Text = "Cortar";
            btnCortarVideo.UseVisualStyleBackColor = true;
            btnCortarVideo.Click += btnCortarVideo_Click;
            // 
            // tabPageCapa
            // 
            tabPageCapa.Controls.Add(btnPulaPassoFinal);
            tabPageCapa.Controls.Add(lblCaminhoCapa);
            tabPageCapa.Controls.Add(pictureBoxCapa);
            tabPageCapa.Controls.Add(btnSelecionarCapa);
            tabPageCapa.Location = new Point(4, 24);
            tabPageCapa.Name = "tabPageCapa";
            tabPageCapa.Size = new Size(700, 422);
            tabPageCapa.TabIndex = 4;
            tabPageCapa.Text = "Passo 4: Thumbnail";
            tabPageCapa.UseVisualStyleBackColor = true;
            tabPageCapa.Enter += tabPageCapa_Enter;
            // 
            // btnPulaPassoFinal
            // 
            btnPulaPassoFinal.Location = new Point(562, 391);
            btnPulaPassoFinal.Name = "btnPulaPassoFinal";
            btnPulaPassoFinal.Size = new Size(135, 23);
            btnPulaPassoFinal.TabIndex = 3;
            btnPulaPassoFinal.Text = "Próximo Passo";
            btnPulaPassoFinal.UseVisualStyleBackColor = true;
            btnPulaPassoFinal.Click += btnPulaPassoFinal_Click;
            // 
            // lblCaminhoCapa
            // 
            lblCaminhoCapa.AutoSize = true;
            lblCaminhoCapa.Location = new Point(285, 16);
            lblCaminhoCapa.Name = "lblCaminhoCapa";
            lblCaminhoCapa.Size = new Size(0, 15);
            lblCaminhoCapa.TabIndex = 2;
            // 
            // pictureBoxCapa
            // 
            pictureBoxCapa.Location = new Point(8, 41);
            pictureBoxCapa.Name = "pictureBoxCapa";
            pictureBoxCapa.Size = new Size(684, 279);
            pictureBoxCapa.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCapa.TabIndex = 1;
            pictureBoxCapa.TabStop = false;
            // 
            // btnSelecionarCapa
            // 
            btnSelecionarCapa.Location = new Point(8, 12);
            btnSelecionarCapa.Name = "btnSelecionarCapa";
            btnSelecionarCapa.Size = new Size(257, 23);
            btnSelecionarCapa.TabIndex = 0;
            btnSelecionarCapa.Text = "Trocar Capa Padrão...";
            btnSelecionarCapa.UseVisualStyleBackColor = true;
            btnSelecionarCapa.Click += btnSelecionarCapa_Click;
            // 
            // tabPageUpload
            // 
            tabPageUpload.Controls.Add(btnFazerUpload);
            tabPageUpload.Location = new Point(4, 24);
            tabPageUpload.Name = "tabPageUpload";
            tabPageUpload.Size = new Size(700, 422);
            tabPageUpload.TabIndex = 3;
            tabPageUpload.Text = "Passo 5: Upload para o YouTube";
            tabPageUpload.UseVisualStyleBackColor = true;
            // 
            // btnFazerUpload
            // 
            btnFazerUpload.Dock = DockStyle.Top;
            btnFazerUpload.Location = new Point(0, 0);
            btnFazerUpload.Name = "btnFazerUpload";
            btnFazerUpload.Size = new Size(700, 23);
            btnFazerUpload.TabIndex = 3;
            btnFazerUpload.Text = "Fazer Upload para o YouTube";
            btnFazerUpload.UseVisualStyleBackColor = true;
            btnFazerUpload.Click += btnFazerUpload_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(708, 450);
            Controls.Add(tabControlPrincipal);
            Name = "Form1";
            Text = "Futtage";
            Load += Form1_Load;
            tabControlPrincipal.ResumeLayout(false);
            tabPageSelecao.ResumeLayout(false);
            tabPageJuntar.ResumeLayout(false);
            tabPageCorte.ResumeLayout(false);
            tabPageCorte.PerformLayout();
            tabPageCapa.ResumeLayout(false);
            tabPageCapa.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCapa).EndInit();
            tabPageUpload.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnSelecionarArquivo;
        private OpenFileDialog openFileDialog1;

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private TabControl tabControlPrincipal;
        private TabPage tabPageSelecao;
        private TabPage tabPageJuntar;
        private TabPage tabPageCorte;
        private TabPage tabPageUpload;
        private Button btnExcluirItem;
        private Button btnMoverCima;
        private ListBox lstArquivosSelecionados;
        private Button btnMoverBaixo;
        private Button btnJuntarVideos;
        private Button btnFazerUpload;
        private Label lbInicioCorte;
        private Button btnCortarVideo;
        private Label lbFimCorte;
        private TextBox txtFimCorte;
        private TextBox txtInicioCorte;
        private Button btnPularCorte;
        private Button btnProximoPasso1;
        private Button btnVoltarPasso1;
        private TabPage tabPageCapa;
        private PictureBox pictureBoxCapa;
        private Button btnSelecionarCapa;
        private Label lblCaminhoCapa;
        private Button btnPulaPassoFinal;
    }
}
