
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Futtage
{
    public partial class FormDetalhesVideo : Form
    {
        public string TituloDoVideo { get; private set; } = string.Empty;
        public string DescricaoDoVideo { get; private set; } = string.Empty;
        public bool IsConteudoInfantil { get; private set; }
        public string PrivacyStatus { get; private set; } = "private";

        private GroupBox gbPrivacy;
        private RadioButton rbPrivate;
        private RadioButton rbUnlisted;
        private RadioButton rbPublic;

        public FormDetalhesVideo(DateTime dataDeCriacao)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            LoadIcon();
            SetupDefaultValues(dataDeCriacao);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.TopMost = false;
            this.Activate();
        }


        private void InitializePrivacyControls()
        {
            // GroupBox para privacidade
            this.gbPrivacy = new GroupBox();
            this.rbPrivate = new RadioButton();
            this.rbUnlisted = new RadioButton();
            this.rbPublic = new RadioButton();

            this.gbPrivacy.SuspendLayout();

            // 
            // gbPrivacy
            // 
            this.gbPrivacy.Controls.Add(this.rbPublic);
            this.gbPrivacy.Controls.Add(this.rbUnlisted);
            this.gbPrivacy.Controls.Add(this.rbPrivate);
            this.gbPrivacy.Location = new Point(42, 320);
            this.gbPrivacy.Name = "gbPrivacy";
            this.gbPrivacy.Size = new Size(400, 80);
            this.gbPrivacy.TabIndex = 7;
            this.gbPrivacy.TabStop = false;
            this.gbPrivacy.Text = "Privacidade do Vídeo";

            // 
            // rbPrivate
            // 
            this.rbPrivate.AutoSize = true;
            this.rbPrivate.Checked = true;
            this.rbPrivate.Location = new Point(15, 25);
            this.rbPrivate.Name = "rbPrivate";
            this.rbPrivate.Size = new Size(67, 19);
            this.rbPrivate.TabIndex = 0;
            this.rbPrivate.TabStop = true;
            this.rbPrivate.Text = "Privado";
            this.rbPrivate.UseVisualStyleBackColor = true;

            // 
            // rbUnlisted
            // 
            this.rbUnlisted.AutoSize = true;
            this.rbUnlisted.Location = new Point(100, 25);
            this.rbUnlisted.Name = "rbUnlisted";
            this.rbUnlisted.Size = new Size(86, 19);
            this.rbUnlisted.TabIndex = 1;
            this.rbUnlisted.Text = "Não listado";
            this.rbUnlisted.UseVisualStyleBackColor = true;

            // 
            // rbPublic
            // 
            this.rbPublic.AutoSize = true;
            this.rbPublic.Location = new Point(200, 25);
            this.rbPublic.Name = "rbPublic";
            this.rbPublic.Size = new Size(65, 19);
            this.rbPublic.TabIndex = 2;
            this.rbPublic.Text = "Público";
            this.rbPublic.UseVisualStyleBackColor = true;

            // Adicionar tooltip explicativo
            var toolTip = new ToolTip();
            toolTip.SetToolTip(this.rbPrivate, "Apenas você pode ver o vídeo");
            toolTip.SetToolTip(this.rbUnlisted, "Qualquer pessoa com o link pode ver");
            toolTip.SetToolTip(this.rbPublic, "Qualquer pessoa pode encontrar e assistir");

            this.gbPrivacy.ResumeLayout(false);
            this.gbPrivacy.PerformLayout();

            // Adicionar ao formulário
            this.Controls.Add(this.gbPrivacy);

            // Ajustar posição dos botões
            this.btnOk.Location = new Point(701, 420);
            this.btnCancelar.Location = new Point(608, 420);

            // Ajustar tamanho do formulário se necessário
            this.ClientSize = new Size(800, 460);
        }

        private void LoadIcon()
        {
            try
            {
                byte[] iconBytes = Properties.Resources.app_icon;
                using (MemoryStream ms = new MemoryStream(iconBytes))
                {
                    this.Icon = new Icon(ms);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar ícone: {ex.Message}");
            }
        }

        private void SetupDefaultValues(DateTime dataDeCriacao)
        {
            txtTitulo.Text = $"Viana - {dataDeCriacao:dd/MM/yyyy} - ";

            txtDescricao.Text = @"📹 Câmera: SJ4000 AIR

⚽ Time Meu:
🧤 

⚽ Time Teu:
🧤 

🔔 Inscreva-se no canal para mais vídeos!
";

            txtTitulo.SelectionStart = txtTitulo.Text.Length;
            txtTitulo.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("O título do vídeo é obrigatório.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }

            if (txtTitulo.Text.Length > 100)
            {
                var result = MessageBox.Show(
                    $"O título tem {txtTitulo.Text.Length} caracteres. O YouTube permite até 100.\n\nDeseja continuar mesmo assim?",
                    "Título muito longo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    txtTitulo.Focus();
                    return;
                }
            }

            if (txtDescricao.Text.Length > 5000)
            {
                var result = MessageBox.Show(
                    $"A descrição tem {txtDescricao.Text.Length} caracteres. O YouTube permite até 5000.\n\nDeseja continuar mesmo assim?",
                    "Descrição muito longa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    txtDescricao.Focus();
                    return;
                }
            }

            TituloDoVideo = txtTitulo.Text.Trim();
            DescricaoDoVideo = txtDescricao.Text.Trim();
            IsConteudoInfantil = chkConteudoInfantil.Checked;

            PrivacyStatus = rbPrivate.Checked ? "private" :
                           rbUnlisted.Checked ? "unlisted" : "public";

            System.Diagnostics.Debug.WriteLine($"Detalhes do vídeo capturados:");
            System.Diagnostics.Debug.WriteLine($"  Título: {TituloDoVideo}");
            System.Diagnostics.Debug.WriteLine($"  Descrição: {DescricaoDoVideo.Length} caracteres");
            System.Diagnostics.Debug.WriteLine($"  Conteúdo infantil: {IsConteudoInfantil}");
            System.Diagnostics.Debug.WriteLine($"  Privacidade: {PrivacyStatus}");

            DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void txtTitulo_TextChanged(object sender, EventArgs e)
        {
            // Mostrar contador de caracteres em tempo real
            var remaining = 100 - txtTitulo.Text.Length;
            var color = remaining < 0 ? Color.Red : remaining < 20 ? Color.Orange : Color.Gray;

            // Você pode adicionar um label para mostrar isso se quiser
            // lblTituloCount.Text = $"{txtTitulo.Text.Length}/100";
            // lblTituloCount.ForeColor = color;
        }

        private void txtDescricao_TextChanged(object sender, EventArgs e)
        {
            // Mostrar contador de caracteres em tempo real
            var remaining = 5000 - txtDescricao.Text.Length;
            var color = remaining < 0 ? Color.Red : remaining < 200 ? Color.Orange : Color.Gray;

            // Você pode adicionar um label para mostrar isso se quiser
            // lblDescricaoCount.Text = $"{txtDescricao.Text.Length}/5000";
            // lblDescricaoCount.ForeColor = color;
        }
    }
}