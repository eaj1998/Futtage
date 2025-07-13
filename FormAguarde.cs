using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Futtage
{
    public partial class FormAguarde : Form
    {
        public FormAguarde()
        {
            InitializeComponent();
        }

        private void FormAguarde_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        public void AtualizarProgresso(int porcentagem)
        {
            if (this.InvokeRequired)
            {
                var acao = new Action<int>(AtualizarProgresso);
                this.Invoke(acao, porcentagem);
            }
            else
            {
                if (porcentagem > progressBarUpload.Maximum)
                {
                    porcentagem = progressBarUpload.Maximum;
                }

                progressBarUpload.Value = porcentagem;
                lblPorcentagem.Text = $"{porcentagem}%";

                progressBarUpload.Refresh();
                lblPorcentagem.Refresh();

                Application.DoEvents();
            }
        }

        public void ConfigurarVisibilidadeDaBarra(bool mostrarBarra)
        {
            progressBarUpload.Visible = mostrarBarra;
            lblPorcentagem.Visible = mostrarBarra;

            if (mostrarBarra)
            {
                lbProcessandoVideos.Visible = !mostrarBarra;
                lbYoutube.Visible = mostrarBarra;
            }
            else
            {
                lbProcessandoVideos.Visible = !mostrarBarra;
                lbYoutube.Visible = mostrarBarra;
            }
        }
    }
}
