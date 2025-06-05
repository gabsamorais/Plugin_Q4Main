namespace tmrp.core
{
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System;
    using System.IO;
    using System.Windows.Forms;

    [Transaction(TransactionMode.Manual)]

    // Permite inserir novos links de dashboard do Power BI
    public class LinkDashPBIComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", "DashboardsPBI.csv");

            if (!File.Exists(diretorio))
            {
                File.WriteAllText(diretorio, "Projeto;LinkDashboard\n");
            }

            var form = new System.Windows.Forms.Form
            {
                Text = "Q4Main: Registrar novo dashboard",
                Width = 500,
                Height = 220,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                StartPosition = FormStartPosition.CenterScreen
            };

            var rotuloProjeto = new Label { Text = "Nome do projeto:", Left = 20, Top = 20, Width = 180 };
            var entradaProjeto = new System.Windows.Forms.TextBox { Left = 210, Top = 18, Width = 250 };

            var rotuloLink = new Label { Text = "Link do dashboard:", Left = 20, Top = 60, Width = 180 };
            var entradaLink = new System.Windows.Forms.TextBox { Left = 210, Top = 58, Width = 250 };

            var btnOk = new Button { Text = "Ok", Left = 360, Top = 110, Width = 100, Enabled = false };
            var btnCancel = new Button { Text = "Cancelar", Left = 255, Top = 110, Width = 100 };
            
            btnCancel.Click += (s, e) => form.Close();

            entradaProjeto.TextChanged += (s, e) => ValidarInfos();
            entradaLink.TextChanged += (s, e) => ValidarInfos();

            void ValidarInfos()
            {
                bool validaUrl = Uri.TryCreate(entradaLink.Text, UriKind.Absolute, out Uri uriResult)
                                  && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                btnOk.Enabled = !string.IsNullOrWhiteSpace(entradaProjeto.Text) && validaUrl;
            }

            string nomeProjeto = null;
            string linkDashboard = null;

            btnOk.Click += (s, e) =>
            {
                nomeProjeto = entradaProjeto.Text.Trim();
                linkDashboard = entradaLink.Text.Trim();
                form.DialogResult = DialogResult.OK;
                form.Close();
            };

            form.Controls.Add(rotuloProjeto);
            form.Controls.Add(entradaProjeto);
            form.Controls.Add(rotuloLink);
            form.Controls.Add(entradaLink);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);

            if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(nomeProjeto) && !string.IsNullOrWhiteSpace(linkDashboard))
            {
                try
                {
                    File.AppendAllLines(diretorio, new[] { $"{nomeProjeto};{linkDashboard}" });
                    MessageBox.Show("Novo dashboard registrado com sucesso.", "Dashboard Registrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Falha ao registrar o dashboard {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }

        public static string GetPath()
        {
            return typeof(LinkDashPBIComando).Namespace + "." + nameof(LinkDashPBIComando);
        }
    }
}
