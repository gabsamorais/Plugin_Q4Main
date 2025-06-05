namespace tmrp.core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    // Formulário de registro de manutenção
    // Implementa a lógica de uso e validação dos campos preenchidos pelo usuário 
    public partial class FormRegMan : System.Windows.Forms.Form
    {
        private UIDocument uidoc;
        private Document doc;
        private Element elementoSelecionado;
        private string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", "ListaServicos.json");
             
        public FormRegMan(UIDocument uIDocument, Element element)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            uidoc = uIDocument;
            doc = uIDocument.Document;
            elementoSelecionado = element;
            btnOkRM.Enabled = false;
        }

        private void RegManForm_Load(object sender, EventArgs e)
        {
            PopulateServiceTypeList();
            PopulateMaintenanceTypeList();
            ConfigurarDatas();
            ValidarCampos();
            ConfigurarDateTimePicker(dtOpenDate);
            ConfigurarDateTimePicker(dtCloseDate);

            if (elementoSelecionado != null)
            {
                LoadElementData();
            }

            else
            {
                MessageBox.Show("Seleção inválida. Nenhum elemento foi selecionado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadElementData()
        {
            if (elementoSelecionado != null)
            {
                if (elementoSelecionado.LookupParameter("Tipo de Serviço") == null ||
                    elementoSelecionado.LookupParameter("Tipo de Manutenção") == null ||
                    elementoSelecionado.LookupParameter("Data de Abertura") == null ||
                    elementoSelecionado.LookupParameter("Data de Fechamento") == null)
                    {
                        MessageBox.Show("Não foi possível localizar todos os parâmetros no elemento selecionado.","Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
            }
        }

        public List<string> CarregarTiposServico()
        {
            if (!File.Exists(diretorio))
            {
                var tiposPadroes = new List<string>
                // Lista inicial de tipos de serviços
                {
                    "Alvenaria", "Carpintaria", "Elétrica", "Hidrossanitária",
                    "Gesso", "Impermeabilização", "Marcenaria", "Pintura",
                    "Serralharia", "Vidraçaria"
                };
                File.WriteAllText(diretorio, JsonSerializer.Serialize(tiposPadroes));
                return tiposPadroes;
            }
            else
            {
                string json = File.ReadAllText(diretorio);
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
        }

        // Permite adicionar novos serviços além dos definidos na lista padrão
        private void btnAdicionarServico_Click(object sender, EventArgs e)
        {
            string novoTipo = Microsoft.VisualBasic.Interaction.InputBox(
                "Digite o novo tipo de serviço:",
                "Adicionar novo serviço",
                ""
            );

            if (!string.IsNullOrWhiteSpace(novoTipo))
            {
                var tipos = CarregarTiposServico();

                if (!tipos.Contains(novoTipo))
                {
                    tipos.Add(novoTipo);
                    File.WriteAllText(diretorio, JsonSerializer.Serialize(tipos));
                    PopulateServiceTypeList(); 
                    cmbServTypeRM.SelectedItem = novoTipo; 
                }
                else
                {
                    MessageBox.Show("Este serviço já está cadastrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Popula a lista de tipos de serviço
        private void PopulateServiceTypeList()
        {
            cmbServTypeRM.Items.Clear();
            cmbServTypeRM.Items.Add("Selecione o tipo de serviço");

            var tipos = CarregarTiposServico();
            tipos = tipos.OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase).ToList();
            foreach (var tipo in tipos)
                cmbServTypeRM.Items.Add(tipo);

            cmbServTypeRM.SelectedIndex = 0;
        }

        // Popula a lista de tipos de manutenção
        private void PopulateMaintenanceTypeList()
        {
            cmbManType.Items.Add("Selecione o tipo de manutenção");
            cmbManType.Items.AddRange(new string[] { "Preventiva", "Corretiva" });
            cmbManType.SelectedIndex = 0;
            cmbManType.SelectedIndexChanged += (s, e) => ValidarCampos();
        }

        // Define a configuração das datas de abertura e fechamento
        private void ConfigurarDatas()
        {
            dtOpenDate.Format = DateTimePickerFormat.Custom;
            dtOpenDate.CustomFormat = " ";
            dtOpenDate.ValueChanged += (s, e) =>
            {
                dtOpenDate.CustomFormat = "dd/MM/yyyy HH:mm";
                ValidarCampos();
            };

            dtCloseDate.Format = DateTimePickerFormat.Custom;
            dtCloseDate.CustomFormat = " ";
            dtCloseDate.ValueChanged += (s, e) =>
            {
                if (dtCloseDate.Value < dtOpenDate.Value)
                {
                    MessageBox.Show("A data de fechamento não pode ser anterior à data de abertura", "Data Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtCloseDate.Value = dtOpenDate.Value;
                }

                dtCloseDate.CustomFormat = "dd/MM/yyyy HH:mm";
                ValidarCampos();
            };
        }

        private void ValidarCampos()
        {
            bool camposPreenchidos =
                cmbServTypeRM.SelectedIndex > 0 &&
                cmbManType.SelectedIndex > 0 &&
                dtOpenDate.CustomFormat != "'Selecione uma data'" &&
                dtCloseDate.CustomFormat != "'Selecione uma data'";
            btnOkRM.Enabled = camposPreenchidos;
        }

        private void ConfigurarDateTimePicker(DateTimePicker dtp)
        {
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "'Selecione uma data'";
            dtp.ValueChanged += (s, e) => dtp.CustomFormat = "dd/MM/yyyy HH:mm";
        }

        private void btnOkRM_Click(object sender, System.EventArgs e)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "Registro de Manutenção"))
                {
                    trans.Start();

                    elementoSelecionado.LookupParameter("Tipo de Serviço")?.Set(cmbServTypeRM.Text);
                    elementoSelecionado.LookupParameter("Tipo de Manutenção")?.Set(cmbManType.Text);
                    elementoSelecionado.LookupParameter("Data de Abertura")?.Set(dtOpenDate.Value.ToString("dd/MM/yyyy HH:mm"));
                    elementoSelecionado.LookupParameter("Data de Fechamento")?.Set(dtCloseDate.Value.ToString("dd/MM/yyyy HH:mm"));

                    trans.Commit();
                }

                SalvarDadosJson();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            catch(Exception ex)
            {
                MessageBox.Show($"Não foi possível registrar manutenção: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SalvarDadosJson()
        {
            string nomeProjeto = Path.GetFileNameWithoutExtension(doc.PathName);
            string nomeArquivo = $"RegistroManutencao_{nomeProjeto}.json";
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", nomeArquivo);

            List<RegManDados> dadosManutencao = new List<RegManDados>();

            if (File.Exists(diretorio))
            {
                string jsonAnt = File.ReadAllText(diretorio);
                if (!string.IsNullOrWhiteSpace(jsonAnt))
                {
                    try
                    {
                        dadosManutencao = JsonSerializer.Deserialize<List<RegManDados>>(jsonAnt) ?? new List<RegManDados>();
                    }
                    
                    catch (JsonException)
                    {
                        dadosManutencao = new List<RegManDados>();
                    }
                }
            }

            var altRegManDados = new RegManDados
            {
                ElementId = elementoSelecionado?.Id.ToString(),
                TipoServico = cmbServTypeRM.Text,
                TipoManutencao = cmbManType.Text,
                DataAbertura = dtOpenDate.Value.ToString("dd/MM/yyyy HH:mm"),
                DataFechamento = dtCloseDate.Value.ToString("dd/MM/yyyy HH:mm")
            };

            dadosManutencao.Add(altRegManDados);

            string newJson = JsonSerializer.Serialize(dadosManutencao, new JsonSerializerOptions 
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true 
            });
            
            File.WriteAllText(diretorio, newJson);
            MessageBox.Show($"Os dados foram salvos em: {diretorio}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancelDEM_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
