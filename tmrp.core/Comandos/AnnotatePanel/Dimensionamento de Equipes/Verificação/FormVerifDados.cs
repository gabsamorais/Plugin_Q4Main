namespace tmrp.core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Windows.Forms;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.DB;
    using static System.Net.Mime.MediaTypeNames;

    // Formulário de análise de dados do modelo
    public partial class FormVerifDados : System.Windows.Forms.Form
    {
        private UIDocument uidoc = null;
        private Document doc = null;

        public FormVerifDados(UIDocument uIDocument)
        {
            // O formulário é exibido no centro da tela, sem a possibilidade de maximização
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            uidoc = uIDocument;
            doc = uidoc.Document;
        }

        // Verifica a existência do arquivo de registro de manutenção e popula a lista de serviços
        private void CheckListaServicos(Document doc)
        {
            string nomeProjeto = Path.GetFileNameWithoutExtension(doc.PathName);
            string nomeArquivo = $"RegistroManutencao_{nomeProjeto}.json";
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", nomeArquivo);
            // Retorna uma mensagem de erro caso o arquivo não seja encontrado
            if (!File.Exists(diretorio))
            {
                MessageBox.Show("A lista de serviços de manutenção não foi encontrada. Verifique o arquivo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string json = File.ReadAllText(diretorio);
            List<RegManDados> registros = JsonSerializer.Deserialize<List<RegManDados>>(json);

            if (registros != null && registros.Count > 0)
            {
                // Popula lista apenas com os serviços para os quais foram registradas solicitações de manutenção
                var servicos = registros.Select(r => r.TipoServico).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();
                servicos.Sort();
                cmbServTypeVM.DataSource = servicos;
            }
        }

        // Implementa a lógica matemática para análise do ajuste dos dados do modelo à distribuição exponencial
        // Retorna uma lista com os parâmetros estatísticos da análise [lambda, erro padrão, coeficiente de determinação, estatística KS e valor-p]
        public class AjusteExponencial
        {
            public static (double lambda, double erroPad, double coefDet, double ksEstat, double ksPValor) FitExpIntervalos(double[] dados)
            {
                int n = dados.Length;
                double media = dados.Average();
                double lambda = 1.0 / media;

                var sorted = (double[])dados.Clone();
                Array.Sort(sorted);

                double somaErroQuad = 0.0;
                double[] empCDF = new double[n];
                double[] teoCDF = new double[n];
                double ksEstat = 0.0;

                for (int i = 0; i < n; i++)
                {
                    empCDF[i] = (i + 1) / (double)n;
                    teoCDF[i] = 1 - Math.Exp(-lambda * sorted[i]);
                    double diff = Math.Abs(empCDF[i] - teoCDF[i]);
                    if (diff > ksEstat)
                        ksEstat = diff;
                    somaErroQuad += Math.Pow(empCDF[i] - teoCDF[i], 2);
                }

                // Erro padrão
                double erroPad = Math.Sqrt(somaErroQuad / n);
                double medEmp = empCDF.Average();
                double somatq = empCDF.Sum(x => Math.Pow(x - medEmp, 2));
                // Coeficiente de Determinação
                double coefDet = 1 - (somaErroQuad / somatq);
                // Estatística do teste KS
                double ksAdj = ksEstat * Math.Sqrt(n);
                // Valor - p do teste KS
                double ksPValor = KS_PValor(ksAdj);

                return (lambda, erroPad, coefDet, ksEstat, ksPValor);
            }

            // Retorna o valor-p do teste de Kolmogorov-Smirnov (KS)
            private static double KS_PValor(double ksValor)
            {
                double sum = 0.0;
                for (int j = 1; j <= 100; j++)
                {
                    sum += 2 * Math.Pow(-1, j - 1) * Math.Exp(-2 * j * j * ksValor * ksValor);
                }
                return sum;
            }
        }

        private void btnOkVM_Click(object sender, EventArgs e)
        {
            string servSelecionado = cmbServTypeVM.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(servSelecionado))
            {
                MessageBox.Show("Selecione um tipo de serviço.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nomeProjeto = Path.GetFileNameWithoutExtension(doc.PathName);
            string nomeArquivo = $"RegistroManutencao_{nomeProjeto}.json";
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", nomeArquivo);
            if (!File.Exists(diretorio))
            {
                MessageBox.Show("O arquivo RegistroManutencao.json não foi encontrado. Verifique no diretório.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string json = File.ReadAllText(diretorio);
            List<RegManDados> registros = JsonSerializer.Deserialize<List<RegManDados>>(json);
            if (registros == null || registros.Count == 0)
            {
                MessageBox.Show("Nenhum registro de manutenção foi identificado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var registrosFiltrados = registros.Where(r => r.TipoServico.Equals(servSelecionado, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (registrosFiltrados.Count == 0)
            {
                MessageBox.Show("Não há registros de manutenção para o serviço selecionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var registrosOrdenados = registrosFiltrados
                .OrderBy(r => DateTime.ParseExact(r.DataAbertura, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture))
                .ToList();
            List<double> interChegadas = new List<double>();
            for (int i = 1; i < registrosOrdenados.Count; i++)
            {
                DateTime dtAnterior = DateTime.ParseExact(registrosOrdenados[i - 1].DataAbertura, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date;
                DateTime dtAtual = DateTime.ParseExact(registrosOrdenados[i].DataAbertura, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date;
                interChegadas.Add((dtAtual - dtAnterior).TotalDays);
            }

            // Compilação das análises sobre os intervalos entre chegadas
            string relInterChegadas = "";

            if (interChegadas.Count >= 2)
            {
                double mediaInterChegadas = interChegadas.Average();
                var fitInterChegadas = AjusteExponencial.FitExpIntervalos(interChegadas.ToArray());
                double lambdaInterChegadas = fitInterChegadas.lambda;
                double coefDetInterChegadas = fitInterChegadas.coefDet;
                double ksEstatInterChegadas = fitInterChegadas.ksEstat;
                double ksPValorInterChegadas = fitInterChegadas.ksPValor;
                bool ajusteInterChegadasOk = (ksPValorInterChegadas >= 0.05 && coefDetInterChegadas >= 0.80);
                string recomInterChegadas = ajusteInterChegadasOk ?
                    "Os dados apresentam um bom ajuste à distribuição exponencial." :
                    "Os dados não se ajustam satisfatoriamente à distribuição exponencial. Recomenda-se utilizar dados elicitados com o apoio de um especialista.";
                relInterChegadas = "\nAnálise dos Intervalos entre Chegadas\n" +
                    $"Tempo médio entre chegadas (em dias): {mediaInterChegadas:F2}\n" +
                    $"Lambda estimado: {lambdaInterChegadas:F4}\n" +
                    $"p-valor KS: {ksPValorInterChegadas:F6}\n" +
                    $"Estatística KS: {ksEstatInterChegadas:F4}\n" +
                    $"Coeficiente de Determinação (R²): {coefDetInterChegadas:F4}\n" +
                    $"Recomendação: {recomInterChegadas}\n";
            }
            else
            {
                relInterChegadas = "Não há intervalos suficientes para análise dos Intervalos entre Chegadas.\n";
            }

            List<double> temposAtend = new List<double>();
            foreach (var reg in registrosFiltrados)
            {
                if (!string.IsNullOrEmpty(reg.DataFechamento))
                {
                    DateTime dtAbert = DateTime.ParseExact(reg.DataAbertura, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date;
                    DateTime dtFech = DateTime.ParseExact(reg.DataFechamento, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date;
                    temposAtend.Add((dtFech - dtAbert).TotalDays);
                }
            }

            // Compilação das análises sobre os tempos de atendimento
            string relTemposAtend = "";
            if (temposAtend.Count >= 2)
            {
                double mediaTemposAtend = temposAtend.Average();
                var fitTemposAtend = AjusteExponencial.FitExpIntervalos(temposAtend.ToArray());
                double lambdaTemposAtend = fitTemposAtend.lambda;
                double coefDetTemposAtend = fitTemposAtend.coefDet;
                double ksEstatTemposAtend = fitTemposAtend.ksEstat;
                double ksPValorTemposAtend = fitTemposAtend.ksPValor;
                bool ajustTemposAtendOk = (ksPValorTemposAtend >= 0.05 && coefDetTemposAtend >= 0.80);
                string recomTemposAtend = ajustTemposAtendOk ?
                    "Os dados apresentam um bom ajuste à distribuição exponencial." :
                    "Os dados não se ajustam satisfatoriamente à distribuição exponencial. Recomenda-se utilizar dados elicitados com o apoio de um especialista.";
                relTemposAtend = "\nAnálise dos Tempos de Atendimento\n" +
                    $"Tempo médio (em dias): {mediaTemposAtend:F2}\n" +
                    $"Lambda estimado: {lambdaTemposAtend:F4}\n" +
                    $"p-valor KS: {ksPValorTemposAtend:F6}\n" +
                    $"Estatística KS: {ksEstatTemposAtend:F4}\n" +
                    $"Coeficiente de Determinação (R²): {coefDetTemposAtend:F4}\n" +
                    $"Recomendação: {recomTemposAtend}\n";
            }
            else
            {
                relTemposAtend = "Não há registros suficientes para análise dos Tempos de Atendimento.\n";
            }

            // Conteúdo do relatório final exibido ao usuário
            var relatorioADM = new StringBuilder();
            relatorioADM.AppendLine("=======================================");
            relatorioADM.AppendLine("\nRelatório de Análise\n");
            relatorioADM.AppendLine("=======================================");
            relatorioADM.AppendLine($"\nServiço: " + servSelecionado);
            relatorioADM.AppendLine($"Número de registros: {temposAtend.Count}\n");
            relatorioADM.AppendLine("=======================================");
            relatorioADM.AppendLine(relInterChegadas);
            relatorioADM.AppendLine("=======================================");
            relatorioADM.AppendLine(relTemposAtend);
            relatorioADM.AppendLine("=======================================");

            MessageBox.Show(relatorioADM.ToString(), "Q4Main: Análise de Fontes de Dados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FormVerifDados_Load(object sender, EventArgs e)
        {
            Document doc = uidoc.Document;
            CheckListaServicos(doc);
        }

        private void btnCancelVM_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
