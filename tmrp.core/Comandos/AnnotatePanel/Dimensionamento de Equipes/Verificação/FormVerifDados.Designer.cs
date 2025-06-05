namespace tmrp.core
{
    partial class FormVerifDados
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

        private void InitializeComponent()
        {
            gbServTypeVM = new System.Windows.Forms.GroupBox();
            cmbServTypeVM = new System.Windows.Forms.ComboBox();
            btnOkVM = new System.Windows.Forms.Button();
            btnCancelVM = new System.Windows.Forms.Button();
            gbServTypeVM.SuspendLayout();
            SuspendLayout();
            // 
            // gbServTypeVM
            // 
            gbServTypeVM.BackColor = System.Drawing.SystemColors.ControlLight;
            gbServTypeVM.Controls.Add(cmbServTypeVM);
            gbServTypeVM.Location = new System.Drawing.Point(12, 16);
            gbServTypeVM.Name = "gbServTypeVM";
            gbServTypeVM.Size = new System.Drawing.Size(320, 61);
            gbServTypeVM.TabIndex = 6;
            gbServTypeVM.TabStop = false;
            gbServTypeVM.Text = "Tipo de Serviço";
            // 
            // cmbServTypeVM
            // 
            cmbServTypeVM.FormattingEnabled = true;
            cmbServTypeVM.Location = new System.Drawing.Point(6, 22);
            cmbServTypeVM.Name = "cmbServTypeVM";
            cmbServTypeVM.Size = new System.Drawing.Size(308, 23);
            cmbServTypeVM.TabIndex = 0;
            // 
            // btnOkVM
            // 
            btnOkVM.Location = new System.Drawing.Point(257, 83);
            btnOkVM.Name = "btnOkVM";
            btnOkVM.Size = new System.Drawing.Size(75, 23);
            btnOkVM.TabIndex = 8;
            btnOkVM.Text = "Ok";
            btnOkVM.UseVisualStyleBackColor = true;
            btnOkVM.Click += btnOkVM_Click;
            // 
            // btnCancelVM
            // 
            btnCancelVM.Location = new System.Drawing.Point(176, 83);
            btnCancelVM.Name = "btnCancelVM";
            btnCancelVM.Size = new System.Drawing.Size(75, 23);
            btnCancelVM.TabIndex = 7;
            btnCancelVM.Text = "Cancelar";
            btnCancelVM.UseVisualStyleBackColor = true;
            btnCancelVM.Click += btnCancelVM_Click;
            // 
            // FormVerifDados
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 116);
            Controls.Add(gbServTypeVM);
            Controls.Add(btnOkVM);
            Controls.Add(btnCancelVM);
            Name = "FormVerifDados";
            Text = "Q4Main: Analisar dados";
            Load += FormVerifDados_Load;
            gbServTypeVM.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox gbServTypeVM;
        private System.Windows.Forms.ComboBox cmbServTypeVM;
        private System.Windows.Forms.Button btnOkVM;
        private System.Windows.Forms.Button btnCancelVM;
    }
}