namespace Futtage
{
    partial class FormDetalhesVideo
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
            label1 = new Label();
            txtTitulo = new TextBox();
            label2 = new Label();
            txtDescricao = new TextBox();
            btnOk = new Button();
            btnCancelar = new Button();
            chkConteudoInfantil = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(42, 24);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 0;
            label1.Text = "Título do Vídeo:";
            // 
            // txtTitulo
            // 
            txtTitulo.Location = new Point(42, 42);
            txtTitulo.Name = "txtTitulo";
            txtTitulo.Size = new Size(746, 23);
            txtTitulo.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(42, 81);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 2;
            label2.Text = "Descrição:";
            // 
            // txtDescricao
            // 
            txtDescricao.Location = new Point(42, 99);
            txtDescricao.Multiline = true;
            txtDescricao.Name = "txtDescricao";
            txtDescricao.ScrollBars = ScrollBars.Vertical;
            txtDescricao.Size = new Size(746, 167);
            txtDescricao.TabIndex = 3;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(701, 415);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(87, 23);
            btnOk.TabIndex = 4;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(608, 415);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(87, 23);
            btnCancelar.TabIndex = 5;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // chkConteudoInfantil
            // 
            chkConteudoInfantil.AutoSize = true;
            chkConteudoInfantil.Location = new Point(42, 285);
            chkConteudoInfantil.Name = "chkConteudoInfantil";
            chkConteudoInfantil.Size = new Size(187, 19);
            chkConteudoInfantil.TabIndex = 6;
            chkConteudoInfantil.Text = "Este conteúdo é para crianças?";
            chkConteudoInfantil.UseVisualStyleBackColor = true;
            // 
            // FormDetalhesVideo
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancelar;
            ClientSize = new Size(800, 450);
            Controls.Add(chkConteudoInfantil);
            Controls.Add(btnCancelar);
            Controls.Add(btnOk);
            Controls.Add(txtDescricao);
            Controls.Add(label2);
            Controls.Add(txtTitulo);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormDetalhesVideo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Detalhes do Vídeo para Upload";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtTitulo;
        private Label label2;
        private TextBox txtDescricao;
        private Button btnOk;
        private Button btnCancelar;
        private CheckBox chkConteudoInfantil;
    }
}