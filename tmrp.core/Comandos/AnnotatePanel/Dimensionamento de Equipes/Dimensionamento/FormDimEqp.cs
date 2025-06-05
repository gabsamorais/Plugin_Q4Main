namespace tmrp.core
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Forms;
    using Autodesk.Revit.UI;
    using System.Text.Json;

    // Formulário de dimensionamento de equipes
    // Implementa a lógica de uso e validação dos campos preenchidos pelo usuário 
    public partial class FormDimEqp : System.Windows.Forms.Form
    {
        private UIDocument uidoc = null;
        private string servicosDiretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", "ListaServicos.json");

        public FormDimEqp(UIDocument uIDocument)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            uidoc = uIDocument;

            cmbServTypeDE.SelectedIndexChanged += (s, e) => ValidarCamposDE();
            txtbDadosElicDE.TextChanged += (s, e) => ValidarCamposDE();
            txtbPercAtendDE.TextChanged += (s, e) => ValidarCamposDE();
            txtbCustServ.TextChanged += (s, e) => ValidarCamposDE();
            txtbCustEsp.TextChanged += (s, e) => ValidarCamposDE();

            ckbDadosModeloDE.CheckedChanged += (s, e) =>
            {
                if (ckbDadosModeloDE.Checked)
                {
                    ckbDadosElicitadosDE.Checked = false;
                    txtbDadosElicDE.Enabled = false;
                }
                ValidarCamposDE();
            };

            ckbDadosElicitadosDE.CheckedChanged += (s, e) =>
            {
                if (ckbDadosElicitadosDE.Checked)
                {
                    ckbDadosModeloTADE.Checked = false;
                    ckbDadosModeloTADE.Enabled = false;
                    ckbDadosModeloDE.Checked = false;
                    txtbDadosElicDE.Enabled = true;
                }
                else
                {
                    ckbDadosModeloTADE.Enabled = true;
                    txtbDadosElicDE.Enabled = false;
                }
                ValidarCamposDE();
            };

            ckbDadosModeloTADE.CheckedChanged += (s, e) =>
            {
                if (ckbDadosModeloTADE.Checked)
                {
                    ckbPercATendDE.Checked = false;
                    txtbPercAtendDE.Enabled = false;
                }
                ValidarCamposDE();
            };

            ckbPercATendDE.CheckedChanged += (s, e) =>
            {
                if (ckbPercATendDE.Checked)
                {
                    ckbDadosModeloTADE.Checked = false;
                    txtbPercAtendDE.Enabled = true;
                }
                else
                {
                    txtbPercAtendDE.Enabled = false;
                }
                ValidarCamposDE();
            };

            txtbDadosElicDE.KeyPress += VerifDadosElicDE;
            txtbDadosElicDE.Leave += ValidarElicDE_Leave;

            txtbPercAtendDE.KeyPress += VerifPercAtend;

            txtbCustServ.KeyPress += VerifCusto;
            txtbCustEsp.KeyPress += VerifCusto;
        }

        public DimEqpComandoDados GetInformation()
        {
            var informationDE = new DimEqpComandoDados();
            {
                informationDE.TipoServicoDE = cmbServTypeDE.SelectedItem.ToString();
                informationDE.TaxChegadaModelo = ckbDadosModeloDE.Checked;
                informationDE.TaxChegadaElic = ckbDadosElicitadosDE.Checked;
                informationDE.TaxAtendModelo = ckbDadosModeloTADE.Checked;
                informationDE.TaxAtendElic = ckbPercATendDE.Checked;
            };

            if (informationDE.TaxChegadaElic)
            {
                try
                {
                    informationDE.ListaTaxChegadaElic = VerifEntrElicDE(txtbDadosElicDE.Text);
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao converter os dados elicitados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            float percentAtend;
            float custoAtend;
            float custoEsp;
            float.TryParse(txtbPercAtendDE.Text, NumberStyles.Any, new System.Globalization.CultureInfo("pt-BR"), out percentAtend);
            float.TryParse(txtbCustServ.Text, NumberStyles.Any, new System.Globalization.CultureInfo("pt-BR"), out custoAtend);
            float.TryParse(txtbCustEsp.Text, NumberStyles.Any, new System.Globalization.CultureInfo("pt-BR"), out custoEsp);
            informationDE.PercentAtend = percentAtend;
            informationDE.CustoAtend = custoAtend;
            informationDE.CustoEsp = custoEsp;

            return informationDE;
        }

        // Preparação dos dados elicitados para validação
        private List<double> VerifEntrElicDE(string input)
        {
            var listaElicDE = new List<double>();
            string[] tokens = input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            { 
                if (double.TryParse(token.Trim(), NumberStyles.Any, new CultureInfo("pt-BR"), out double value))
                {
                    listaElicDE.Add(value);
                }
                else
                {
                    throw new FormatException("Valor inválido: " + token);
                }
            }
            return listaElicDE;
        }

        // Verifica a entrada da lista de dados elicitados
        // O campo não pode ser vazio ou ter menos do que nove itens
        private void ValidarElicDE_Leave(object sender, EventArgs e)
        {
            if (ckbDadosElicitadosDE.Checked)
            {
                string rotuloDE = "Insira os valores elicitados";

                if (txtbDadosElicDE.Text.Trim().Equals(rotuloDE, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                string[] tokensEntElic = txtbDadosElicDE.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                bool dadosElicContagem = tokensEntElic.Length == 9;

                if (!dadosElicContagem)
                {
                    MessageBox.Show("A lista inserida não possui o tamanho correto. Insira nove valores elicitados, separando-os com o caractere ';'", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtbDadosElicDE.Text = rotuloDE;
                    txtbDadosElicDE.ForeColor = System.Drawing.Color.Gray;
                }
            }

            ValidarCamposDE();
        }

        // Validação dos campos do formulário
        // Os campos precisam estar preenchidos
        private void ValidarCamposDE()
        {
            bool servicoSelecionado = cmbServTypeDE.SelectedIndex > 0;

            bool taxChegSelecionada = ckbDadosModeloDE.Checked || ckbDadosElicitadosDE.Checked;

            bool dadosElicPreenchido = true;
            if (ckbDadosElicitadosDE.Checked)
            {
                dadosElicPreenchido = !string.IsNullOrWhiteSpace(txtbDadosElicDE.Text) && !txtbDadosElicDE.Text.Trim().Equals("Insira os nove valores elicitados", StringComparison.OrdinalIgnoreCase);
            }

            bool taxAtendSelecionada = ckbDadosModeloTADE.Checked || ckbPercATendDE.Checked;

            bool percAtendPreenchido = true;

            if (ckbPercATendDE.Checked)
            {
                string percText = txtbPercAtendDE.Text.Trim();
                if (percText.Equals("Insira o percentual de atendimento", StringComparison.OrdinalIgnoreCase) ||
                    !float.TryParse(percText, NumberStyles.Any, new CultureInfo("pt-BR"), out float percValue) ||
                    percValue <= 0 || percValue >= 1)
                {
                    percAtendPreenchido = false;
                }
                else
                {
                    percAtendPreenchido = true;
                }
            }

            bool custServPreenchido = !string.IsNullOrWhiteSpace(txtbCustServ.Text) && !txtbCustServ.Text.Trim().Equals("Insira o custo do serviço", StringComparison.OrdinalIgnoreCase);

            bool custEspPreenchido = !string.IsNullOrWhiteSpace(txtbCustEsp.Text) && !txtbCustEsp.Text.Trim().Equals("Insira o custo da espera", StringComparison.OrdinalIgnoreCase);

            bool custoComparado = true;

            float valorCustServ;
                
            float valorCustEsp;

            if (float.TryParse(txtbCustServ.Text, NumberStyles.Any, new CultureInfo("pt-BR"), out valorCustServ) && float.TryParse(txtbCustEsp.Text, NumberStyles.Any, new CultureInfo("pt-BR"), out valorCustEsp))
            {
                custoComparado = valorCustEsp > valorCustServ;
            }
            else
            {
                custoComparado = false;
            }

            btnOkDE.Enabled = servicoSelecionado && taxChegSelecionada && dadosElicPreenchido && taxAtendSelecionada && percAtendPreenchido && custServPreenchido && custEspPreenchido && custoComparado;
        }

        // Verificação dos custos
        private void VerifCusto(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;
            if (e.KeyChar == ',' && tb.Text.Contains(","))
            {
                e.Handled = true;
            }
        }

        // Verificação dos dados elicitados
        private void VerifDadosElicDE(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != ';' && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Verificação do percentual de atendimento
        private void VerifPercAtend(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            System.Windows.Forms.TextBox tb = sender as System.Windows.Forms.TextBox;

            if (e.KeyChar == ',' && tb.Text.Contains(","))
            {
                e.Handled = true;
            }
        }

        // Formatação do texto
        private void InserirTextoPadTextBox(System.Windows.Forms.TextBox textoBox, string textopad)
        {
            textoBox.Text = textopad;
            textoBox.ForeColor = System.Drawing.Color.Gray;
            textoBox.Enter += (s, e) =>
            {
                if (textoBox.Text.Trim().Equals(textopad, StringComparison.OrdinalIgnoreCase))
                {
                        textoBox.Text = "";
                    textoBox.ForeColor = System.Drawing.Color.Black;
                }
            };
            textoBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textoBox.Text))
                {
                    textoBox.Text = textopad;
                    textoBox.ForeColor = System.Drawing.Color.Gray;
                }
            };
        }

        // Popula a lista de serviços
        private void PopServDEList()
        {
            cmbServTypeDE.Items.Clear();
            cmbServTypeDE.Items.Add("Selecione o serviço desejado");

            if (File.Exists(servicosDiretorio))
            {
                try
                {
                    var listaServTypeDE = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(servicosDiretorio)) ?? new List<string>();
                    listaServTypeDE.Sort();
                    foreach (var servicoDE in listaServTypeDE)
                    {
                        cmbServTypeDE.Items.Add(servicoDE);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar a lista de serviços: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("O arquivo que armazena a lista de serviços não foi encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            cmbServTypeDE.SelectedIndex = 0;
        }

        private void FormDimEqp_Load(object sender, EventArgs e)
        {
            PopServDEList();
            ValidarCamposDE();
            InserirTextoPadTextBox(txtbDadosElicDE, "Insira os valores elicitados");
            InserirTextoPadTextBox(txtbPercAtendDE, "Insira o percentual de atendimento");
            InserirTextoPadTextBox(txtbCustServ, "Insira o custo do serviço");
            InserirTextoPadTextBox(txtbCustEsp, "Insira o custo da espera");
        }

            private void btnOkDE_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelDE_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
