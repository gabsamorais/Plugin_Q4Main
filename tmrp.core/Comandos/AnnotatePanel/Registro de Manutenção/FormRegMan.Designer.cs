namespace tmrp.core
{
    using System.Windows.Forms;
    using System.Drawing;

    partial class FormRegMan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRegMan));
            label1 = new Label();
            cmbServTypeRM = new ComboBox();
            label2 = new Label();
            cmbManType = new ComboBox();
            label3 = new Label();
            dtOpenDate = new DateTimePicker();
            dtCloseDate = new DateTimePicker();
            label4 = new Label();
            btnOkRM = new Button();
            btnCancelRM = new Button();
            btnAdicionarServico = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(88, 15);
            label1.TabIndex = 3;
            label1.Text = "Tipo de Serviço";
            // 
            // cmbServTypeRM
            // 
            cmbServTypeRM.AccessibleRole = AccessibleRole.Grip;
            cmbServTypeRM.FormattingEnabled = true;
            cmbServTypeRM.Location = new Point(12, 36);
            cmbServTypeRM.Name = "cmbServTypeRM";
            cmbServTypeRM.Size = new Size(290, 23);
            cmbServTypeRM.TabIndex = 2;
            cmbServTypeRM.Text = "Selecione o tipo de serviço";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 68);
            label2.Name = "label2";
            label2.Size = new Size(117, 15);
            label2.TabIndex = 5;
            label2.Text = "Tipo de Manutenção";
            // 
            // cmbManType
            // 
            cmbManType.FormattingEnabled = true;
            cmbManType.Location = new Point(12, 92);
            cmbManType.Name = "cmbManType";
            cmbManType.Size = new Size(315, 23);
            cmbManType.TabIndex = 4;
            cmbManType.Text = "Selecione o tipo de manutenção";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 124);
            label3.Name = "label3";
            label3.Size = new Size(172, 15);
            label3.TabIndex = 6;
            label3.Text = "Data de Abertura da Solicitação";
            // 
            // dtOpenDate
            // 
            dtOpenDate.CustomFormat = "dd/MM/yyyy hh:mm";
            dtOpenDate.Format = DateTimePickerFormat.Custom;
            dtOpenDate.Location = new Point(12, 148);
            dtOpenDate.Name = "dtOpenDate";
            dtOpenDate.Size = new Size(315, 23);
            dtOpenDate.TabIndex = 7;
            // 
            // dtCloseDate
            // 
            dtCloseDate.CustomFormat = "dd/MM/yyyy hh:mm";
            dtCloseDate.Format = DateTimePickerFormat.Custom;
            dtCloseDate.Location = new Point(12, 204);
            dtCloseDate.Name = "dtCloseDate";
            dtCloseDate.RightToLeft = RightToLeft.No;
            dtCloseDate.Size = new Size(315, 23);
            dtCloseDate.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 180);
            label4.Name = "label4";
            label4.Size = new Size(182, 15);
            label4.TabIndex = 8;
            label4.Text = "Data de Conclusão da Solicitação";
            // 
            // btnOkRM
            // 
            btnOkRM.Location = new Point(252, 249);
            btnOkRM.Name = "btnOkRM";
            btnOkRM.Size = new Size(75, 23);
            btnOkRM.TabIndex = 10;
            btnOkRM.Text = "Ok";
            btnOkRM.UseVisualStyleBackColor = true;
            btnOkRM.Click += btnOkRM_Click;
            // 
            // btnCancelRM
            // 
            btnCancelRM.Location = new Point(171, 249);
            btnCancelRM.Name = "btnCancelRM";
            btnCancelRM.Size = new Size(75, 23);
            btnCancelRM.TabIndex = 11;
            btnCancelRM.Text = "Cancelar";
            btnCancelRM.UseVisualStyleBackColor = true;
            btnCancelRM.Click += btnCancelDEM_Click;
            // 
            // btnAdicionarServico
            // 
            btnAdicionarServico.BackColor = SystemColors.ControlLight;
            btnAdicionarServico.BackgroundImageLayout = ImageLayout.None;
            btnAdicionarServico.Cursor = Cursors.Hand;
            btnAdicionarServico.FlatAppearance.BorderColor = SystemColors.ControlDark;
            btnAdicionarServico.FlatStyle = FlatStyle.Flat;
            btnAdicionarServico.Font = new Font("Segoe UI", 9F);
            btnAdicionarServico.ForeColor = SystemColors.ControlText;
            btnAdicionarServico.Image = (Image)resources.GetObject("btnAdicionarServico.Image");
            btnAdicionarServico.Location = new Point(304, 36);
            btnAdicionarServico.Name = "btnAdicionarServico";
            btnAdicionarServico.Size = new Size(23, 23);
            btnAdicionarServico.TabIndex = 12;
            btnAdicionarServico.TabStop = false;
            btnAdicionarServico.TextAlign = ContentAlignment.TopCenter;
            btnAdicionarServico.UseVisualStyleBackColor = false;
            btnAdicionarServico.Click += btnAdicionarServico_Click;
            // 
            // RegManForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(344, 291);
            Controls.Add(btnAdicionarServico);
            Controls.Add(btnCancelRM);
            Controls.Add(btnOkRM);
            Controls.Add(dtCloseDate);
            Controls.Add(label4);
            Controls.Add(dtOpenDate);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cmbManType);
            Controls.Add(label1);
            Controls.Add(cmbServTypeRM);
            Name = "FormRegMan";
            Text = "Q4Main: Registro de manutenção";
            Load += RegManForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbServTypeRM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbManType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtOpenDate;
        private System.Windows.Forms.DateTimePicker dtCloseDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOkRM;
        private System.Windows.Forms.Button btnCancelRM;
        private System.Windows.Forms.Button btnAdicionarServico;
    }
}