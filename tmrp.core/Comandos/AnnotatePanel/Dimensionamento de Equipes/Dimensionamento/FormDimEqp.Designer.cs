namespace tmrp.core
{
    using System.Windows.Forms;
    partial class FormDimEqp
    {
   
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDimEqp));
            gbServTypeDE = new GroupBox();
            cmbServTypeDE = new ComboBox();
            gbTaxChegDE = new GroupBox();
            picbInfoTaxaChegElici = new PictureBox();
            txtbDadosElicDE = new TextBox();
            ckbDadosElicitadosDE = new CheckBox();
            ckbDadosModeloDE = new CheckBox();
            gbTaxAtendDE = new GroupBox();
            picbInfoPercentAtend = new PictureBox();
            txtbPercAtendDE = new TextBox();
            ckbPercATendDE = new CheckBox();
            ckbDadosModeloTADE = new CheckBox();
            gbCustosDE = new GroupBox();
            picbInfoCustEsp = new PictureBox();
            picbInfoCustServ = new PictureBox();
            txtbCustEsp = new TextBox();
            lbCustEpsDE = new Label();
            txtbCustServ = new TextBox();
            lbCustoServDE = new Label();
            btnCancelDE = new Button();
            btnOkDE = new Button();
            toolTipPicbInfoTaxaChegElici = new ToolTip(components);
            toolTipPicbInfoPercentAtend = new ToolTip(components);
            gbServTypeDE.SuspendLayout();
            gbTaxChegDE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picbInfoTaxaChegElici).BeginInit();
            gbTaxAtendDE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picbInfoPercentAtend).BeginInit();
            gbCustosDE.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picbInfoCustEsp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picbInfoCustServ).BeginInit();
            SuspendLayout();
            // 
            // gbServTypeDE
            // 
            gbServTypeDE.BackColor = System.Drawing.SystemColors.ControlLight;
            gbServTypeDE.Controls.Add(cmbServTypeDE);
            gbServTypeDE.Location = new System.Drawing.Point(12, 12);
            gbServTypeDE.Name = "gbServTypeDE";
            gbServTypeDE.Size = new System.Drawing.Size(320, 61);
            gbServTypeDE.TabIndex = 0;
            gbServTypeDE.TabStop = false;
            gbServTypeDE.Text = "Tipo de Serviço";
            // 
            // cmbServTypeDE
            // 
            cmbServTypeDE.FormattingEnabled = true;
            cmbServTypeDE.Location = new System.Drawing.Point(6, 22);
            cmbServTypeDE.Name = "cmbServTypeDE";
            cmbServTypeDE.Size = new System.Drawing.Size(308, 23);
            cmbServTypeDE.TabIndex = 0;
            // 
            // gbTaxChegDE
            // 
            gbTaxChegDE.BackColor = System.Drawing.SystemColors.ControlLight;
            gbTaxChegDE.Controls.Add(picbInfoTaxaChegElici);
            gbTaxChegDE.Controls.Add(txtbDadosElicDE);
            gbTaxChegDE.Controls.Add(ckbDadosElicitadosDE);
            gbTaxChegDE.Controls.Add(ckbDadosModeloDE);
            gbTaxChegDE.Location = new System.Drawing.Point(12, 79);
            gbTaxChegDE.Name = "gbTaxChegDE";
            gbTaxChegDE.Size = new System.Drawing.Size(320, 108);
            gbTaxChegDE.TabIndex = 1;
            gbTaxChegDE.TabStop = false;
            gbTaxChegDE.Text = "Taxa Média de Chegada (λ)";
            // 
            // picbInfoTaxaChegElici
            // 
            picbInfoTaxaChegElici.Cursor = Cursors.Help;
            picbInfoTaxaChegElici.Image = (System.Drawing.Image)resources.GetObject("picbInfoTaxaChegElici.Image");
            picbInfoTaxaChegElici.Location = new System.Drawing.Point(295, 72);
            picbInfoTaxaChegElici.Name = "picbInfoTaxaChegElici";
            picbInfoTaxaChegElici.Size = new System.Drawing.Size(19, 23);
            picbInfoTaxaChegElici.SizeMode = PictureBoxSizeMode.CenterImage;
            picbInfoTaxaChegElici.TabIndex = 3;
            picbInfoTaxaChegElici.TabStop = false;
            toolTipPicbInfoTaxaChegElici.SetToolTip(picbInfoTaxaChegElici, resources.GetString("picbInfoTaxaChegElici.ToolTip"));
            // 
            // txtbDadosElicDE
            // 
            txtbDadosElicDE.Location = new System.Drawing.Point(24, 72);
            txtbDadosElicDE.Name = "txtbDadosElicDE";
            txtbDadosElicDE.Size = new System.Drawing.Size(265, 23);
            txtbDadosElicDE.TabIndex = 2;
            // 
            // ckbDadosElicitadosDE
            // 
            ckbDadosElicitadosDE.AutoSize = true;
            ckbDadosElicitadosDE.Location = new System.Drawing.Point(6, 47);
            ckbDadosElicitadosDE.Name = "ckbDadosElicitadosDE";
            ckbDadosElicitadosDE.Size = new System.Drawing.Size(112, 19);
            ckbDadosElicitadosDE.TabIndex = 1;
            ckbDadosElicitadosDE.Text = "Dados elicitados";
            ckbDadosElicitadosDE.UseVisualStyleBackColor = true;
            // 
            // ckbDadosModeloDE
            // 
            ckbDadosModeloDE.AutoSize = true;
            ckbDadosModeloDE.Location = new System.Drawing.Point(6, 22);
            ckbDadosModeloDE.Name = "ckbDadosModeloDE";
            ckbDadosModeloDE.Size = new System.Drawing.Size(120, 19);
            ckbDadosModeloDE.TabIndex = 0;
            ckbDadosModeloDE.Text = "Dados do modelo";
            ckbDadosModeloDE.UseVisualStyleBackColor = true;
            // 
            // gbTaxAtendDE
            // 
            gbTaxAtendDE.BackColor = System.Drawing.SystemColors.ControlLight;
            gbTaxAtendDE.Controls.Add(picbInfoPercentAtend);
            gbTaxAtendDE.Controls.Add(txtbPercAtendDE);
            gbTaxAtendDE.Controls.Add(ckbPercATendDE);
            gbTaxAtendDE.Controls.Add(ckbDadosModeloTADE);
            gbTaxAtendDE.Location = new System.Drawing.Point(12, 193);
            gbTaxAtendDE.Name = "gbTaxAtendDE";
            gbTaxAtendDE.Size = new System.Drawing.Size(320, 108);
            gbTaxAtendDE.TabIndex = 2;
            gbTaxAtendDE.TabStop = false;
            gbTaxAtendDE.Text = "Taxa Média de Atendimento (μ)";
            // 
            // picbInfoPercentAtend
            // 
            picbInfoPercentAtend.Cursor = Cursors.Help;
            picbInfoPercentAtend.Image = (System.Drawing.Image)resources.GetObject("picbInfoPercentAtend.Image");
            picbInfoPercentAtend.Location = new System.Drawing.Point(295, 72);
            picbInfoPercentAtend.Name = "picbInfoPercentAtend";
            picbInfoPercentAtend.Size = new System.Drawing.Size(19, 23);
            picbInfoPercentAtend.SizeMode = PictureBoxSizeMode.CenterImage;
            picbInfoPercentAtend.TabIndex = 3;
            picbInfoPercentAtend.TabStop = false;
            toolTipPicbInfoPercentAtend.SetToolTip(picbInfoPercentAtend, resources.GetString("picbInfoPercentAtend.ToolTip"));
            // 
            // txtbPercAtendDE
            // 
            txtbPercAtendDE.Location = new System.Drawing.Point(24, 72);
            txtbPercAtendDE.Name = "txtbPercAtendDE";
            txtbPercAtendDE.Size = new System.Drawing.Size(265, 23);
            txtbPercAtendDE.TabIndex = 2;
            // 
            // ckbPercATendDE
            // 
            ckbPercATendDE.AutoSize = true;
            ckbPercATendDE.Location = new System.Drawing.Point(6, 47);
            ckbPercATendDE.Name = "ckbPercATendDE";
            ckbPercATendDE.Size = new System.Drawing.Size(169, 19);
            ckbPercATendDE.TabIndex = 1;
            ckbPercATendDE.Text = "Percentual de atendimento";
            ckbPercATendDE.UseVisualStyleBackColor = true;
            // 
            // ckbDadosModeloTADE
            // 
            ckbDadosModeloTADE.AutoSize = true;
            ckbDadosModeloTADE.Location = new System.Drawing.Point(6, 22);
            ckbDadosModeloTADE.Name = "ckbDadosModeloTADE";
            ckbDadosModeloTADE.Size = new System.Drawing.Size(120, 19);
            ckbDadosModeloTADE.TabIndex = 0;
            ckbDadosModeloTADE.Text = "Dados do modelo";
            ckbDadosModeloTADE.UseVisualStyleBackColor = true;
            // 
            // gbCustosDE
            // 
            gbCustosDE.BackColor = System.Drawing.SystemColors.ControlLight;
            gbCustosDE.Controls.Add(picbInfoCustEsp);
            gbCustosDE.Controls.Add(picbInfoCustServ);
            gbCustosDE.Controls.Add(txtbCustEsp);
            gbCustosDE.Controls.Add(lbCustEpsDE);
            gbCustosDE.Controls.Add(txtbCustServ);
            gbCustosDE.Controls.Add(lbCustoServDE);
            gbCustosDE.Location = new System.Drawing.Point(12, 307);
            gbCustosDE.Name = "gbCustosDE";
            gbCustosDE.Size = new System.Drawing.Size(320, 100);
            gbCustosDE.TabIndex = 3;
            gbCustosDE.TabStop = false;
            gbCustosDE.Text = "Parâmetros de Custo";
            // 
            // picbInfoCustEsp
            // 
            picbInfoCustEsp.Cursor = Cursors.Help;
            picbInfoCustEsp.Image = (System.Drawing.Image)resources.GetObject("picbInfoCustEsp.Image");
            picbInfoCustEsp.Location = new System.Drawing.Point(295, 52);
            picbInfoCustEsp.Name = "picbInfoCustEsp";
            picbInfoCustEsp.Size = new System.Drawing.Size(19, 23);
            picbInfoCustEsp.SizeMode = PictureBoxSizeMode.CenterImage;
            picbInfoCustEsp.TabIndex = 7;
            picbInfoCustEsp.TabStop = false;
            toolTipPicbInfoPercentAtend.SetToolTip(picbInfoCustEsp, resources.GetString("picbInfoCustEsp.ToolTip"));
            // 
            // picbInfoCustServ
            // 
            picbInfoCustServ.Cursor = Cursors.Help;
            picbInfoCustServ.Image = (System.Drawing.Image)resources.GetObject("picbInfoCustServ.Image");
            picbInfoCustServ.Location = new System.Drawing.Point(295, 22);
            picbInfoCustServ.Name = "picbInfoCustServ";
            picbInfoCustServ.Size = new System.Drawing.Size(19, 23);
            picbInfoCustServ.SizeMode = PictureBoxSizeMode.CenterImage;
            picbInfoCustServ.TabIndex = 4;
            picbInfoCustServ.TabStop = false;
            toolTipPicbInfoPercentAtend.SetToolTip(picbInfoCustServ, resources.GetString("picbInfoCustServ.ToolTip"));
            // 
            // txtbCustEsp
            // 
            txtbCustEsp.Location = new System.Drawing.Point(149, 52);
            txtbCustEsp.Name = "txtbCustEsp";
            txtbCustEsp.Size = new System.Drawing.Size(140, 23);
            txtbCustEsp.TabIndex = 6;
            // 
            // lbCustEpsDE
            // 
            lbCustEpsDE.AutoSize = true;
            lbCustEpsDE.Location = new System.Drawing.Point(12, 55);
            lbCustEpsDE.Name = "lbCustEpsDE";
            lbCustEpsDE.Size = new System.Drawing.Size(127, 15);
            lbCustEpsDE.TabIndex = 5;
            lbCustEpsDE.Text = "Custo da espera (R$/h)";
            // 
            // txtbCustServ
            // 
            txtbCustServ.Location = new System.Drawing.Point(149, 22);
            txtbCustServ.Name = "txtbCustServ";
            txtbCustServ.Size = new System.Drawing.Size(140, 23);
            txtbCustServ.TabIndex = 4;
            // 
            // lbCustoServDE
            // 
            lbCustoServDE.AutoSize = true;
            lbCustoServDE.Location = new System.Drawing.Point(12, 25);
            lbCustoServDE.Name = "lbCustoServDE";
            lbCustoServDE.Size = new System.Drawing.Size(131, 15);
            lbCustoServDE.TabIndex = 0;
            lbCustoServDE.Text = "Custo do serviço (R$/h)";
            // 
            // btnCancelDE
            // 
            btnCancelDE.Location = new System.Drawing.Point(176, 413);
            btnCancelDE.Name = "btnCancelDE";
            btnCancelDE.Size = new System.Drawing.Size(75, 23);
            btnCancelDE.TabIndex = 4;
            btnCancelDE.Text = "Cancelar";
            btnCancelDE.UseVisualStyleBackColor = true;
            btnCancelDE.Click += btnCancelDE_Click;
            // 
            // btnOkDE
            // 
            btnOkDE.Location = new System.Drawing.Point(257, 413);
            btnOkDE.Name = "btnOkDE";
            btnOkDE.Size = new System.Drawing.Size(75, 23);
            btnOkDE.TabIndex = 5;
            btnOkDE.Text = "Ok";
            btnOkDE.UseVisualStyleBackColor = true;
            btnOkDE.Click += btnOkDE_Click;
            // 
            // FormDimEqp
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 457);
            Controls.Add(btnOkDE);
            Controls.Add(btnCancelDE);
            Controls.Add(gbCustosDE);
            Controls.Add(gbTaxAtendDE);
            Controls.Add(gbTaxChegDE);
            Controls.Add(gbServTypeDE);
            Name = "FormDimEqp";
            Text = "Q4Main: Equipes";
            Load += FormDimEqp_Load;
            gbServTypeDE.ResumeLayout(false);
            gbTaxChegDE.ResumeLayout(false);
            gbTaxChegDE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picbInfoTaxaChegElici).EndInit();
            gbTaxAtendDE.ResumeLayout(false);
            gbTaxAtendDE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picbInfoPercentAtend).EndInit();
            gbCustosDE.ResumeLayout(false);
            gbCustosDE.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picbInfoCustEsp).EndInit();
            ((System.ComponentModel.ISupportInitialize)picbInfoCustServ).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox gbServTypeDE;
        private System.Windows.Forms.ComboBox cmbServTypeDE;
        private System.Windows.Forms.GroupBox gbTaxChegDE;
        private System.Windows.Forms.TextBox txtbDadosElicDE;
        private System.Windows.Forms.CheckBox ckbDadosElicitadosDE;
        private System.Windows.Forms.CheckBox ckbDadosModeloDE;
        private System.Windows.Forms.PictureBox picbInfoTaxaChegElici;
        private System.Windows.Forms.GroupBox gbTaxAtendDE;
        private System.Windows.Forms.PictureBox picbInfoPercentAtend;
        private System.Windows.Forms.TextBox txtbPercAtendDE;
        private System.Windows.Forms.CheckBox ckbPercATendDE;
        private System.Windows.Forms.CheckBox ckbDadosModeloTADE;
        private System.Windows.Forms.GroupBox gbCustosDE;
        private System.Windows.Forms.TextBox txtbCustServ;
        private System.Windows.Forms.Label lbCustoServDE;
        private System.Windows.Forms.TextBox txtbCustEsp;
        private System.Windows.Forms.Label lbCustEpsDE;
        private System.Windows.Forms.Button btnCancelDE;
        private System.Windows.Forms.Button btnOkDE;
        private ToolTip toolTipPicbInfoTaxaChegElici;
        private ToolTip toolTipPicbInfoPercentAtend;
        private PictureBox picbInfoCustEsp;
        private PictureBox picbInfoCustServ;
    }
}