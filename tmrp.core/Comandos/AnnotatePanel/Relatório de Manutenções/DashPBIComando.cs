namespace tmrp.core
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]

    // Permite o acesso ao dashboard do Power BI
    public class DashPBIComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", "DashboardsPBI.csv");

            if (!File.Exists(diretorio))
            {
                File.WriteAllText(diretorio, "Projeto;LinkDashboard\n");
            }

            var linhas = File.ReadAllLines(diretorio);
            var projetos = linhas.Select(l => l.Split(';')[0]).ToList();

            using (var form = new System.Windows.Forms.Form())
            {
                form.Text = "Q4Main: Exibir dashboard";
                form.Width = 520;
                form.Height = 170;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var combo = new System.Windows.Forms.ComboBox
                {
                    Width = 250,
                    Left = 125,
                    Top = 20,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                combo.Items.Add("Selecione um projeto");
                combo.Items.AddRange(projetos.Skip(1).Where(p => !string.IsNullOrWhiteSpace(p)).OrderBy(p => p, StringComparer.CurrentCultureIgnoreCase).ToArray());
                combo.SelectedIndex = 0;

                var btnOk = new System.Windows.Forms.Button
                {
                    Text = "Ok",
                    Width = 90,
                    Left = 155,
                    Top = 70,
                    Enabled = false
                };

                var btnCancel = new System.Windows.Forms.Button
                {
                    Text = "Cancelar",
                    Width = 90,
                    Left = 255,
                    Top = 70,
                };

                combo.SelectedIndexChanged += (s, e) =>
                {
                    btnOk.Enabled = combo.SelectedIndex > 0;
                };

                form.Controls.Add(combo);
                form.Controls.Add(btnOk);
                form.Controls.Add(btnCancel);

                string projetoSelecionado = null;

                btnOk.Click += (s, e) =>
                {
                    projetoSelecionado = combo.SelectedItem.ToString();
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                };

                btnCancel.Click += (s, e) =>
                {
                    form.DialogResult = DialogResult.Cancel;
                    form.Close();
                };

                if (form.ShowDialog() == DialogResult.OK && projetoSelecionado != null)
                {
                    string link = linhas.Skip(1).FirstOrDefault(l => l.StartsWith(projetoSelecionado))?.Split(';')[1];
                    if (!string.IsNullOrEmpty(link))
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            var window = new System.Windows.Window
                            {
                                Title = $"Dashboard de Manutenção [{projetoSelecionado}]",
                                Content = new tmrp.ui.DashboardPBI(link),
                                WindowState = WindowState.Maximized
                            };
                            window.ShowDialog();
                        });
                    }
                }
            }

            return Result.Succeeded;
        }

        public static string GetPath()
        {
            return typeof(DashPBIComando).Namespace + "." + nameof(DashPBIComando);
        }
    }
}