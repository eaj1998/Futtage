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
    public partial class FormDetalhesVideo : Form
    {
        public string TituloDoVideo { get; private set; }
        public string DescricaoDoVideo { get; private set; }
        public bool IsConteudoInfantil { get; private set; }

        public FormDetalhesVideo()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("O título do vídeo é obrigatório.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.TituloDoVideo = txtTitulo.Text;
            this.DescricaoDoVideo = txtDescricao.Text;
            this.IsConteudoInfantil = chkConteudoInfantil.Checked;
            this.DialogResult = DialogResult.OK;

        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
