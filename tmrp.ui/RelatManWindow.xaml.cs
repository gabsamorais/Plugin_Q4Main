namespace tmrp.ui
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.Generic;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Microsoft.Win32;

    public partial class RelatManWindow : Window
    {
        private List<DadosManutencao> manutencoes;
        public string elementoID { get; set; }

        private int totalManutencoesProjeto;

        public RelatManWindow(List<DadosManutencao> dadosManutencao, string elementoSelecionadoID, int totalManutencoesProjeto)
        {
            InitializeComponent();
            elementoID = elementoSelecionadoID;
            DataContext = this;
            manutencoes = dadosManutencao;
            this.elementoID = elementoSelecionadoID;
            this.totalManutencoesProjeto = totalManutencoesProjeto;

            this.totalManutencoesProjeto = totalManutencoesProjeto;

            Title = "Q4Main: Análise de Manutenções do Elemento - ID #"+ elementoID;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            geraCabecalho();
            popFiltro();
            geraTabela();
        }

        private void geraCabecalho()
        {
            txtTotalManutencoes.Text = $"Total de manutenções registradas no projeto: {totalManutencoesProjeto}";
            int totalElemento = manutencoes.Count(m => m.ElementId == elementoID);
            txtTotalManutencoesElemento.Text = $"Total de manutenções registradas no elemento ID #{elementoID}: {totalElemento}";

            double percentual = totalManutencoesProjeto > 0 ? (totalElemento / (double)totalManutencoesProjeto) * 100 : 0;
            txtPercentualManutencoes.Text = $"Percentual das manutenções do elemento em relação ao total do projeto: {percentual:F2}%";
        }

        private void popFiltro()
        {
            var tiposManutencao = manutencoes.Select(m => m.TipoManutencao).Distinct().ToList();
            tiposManutencao.Insert(0, "Todos");
            tiposManutencao.Sort();
            cmbFiltroTipoManutencao.ItemsSource = tiposManutencao;
            cmbFiltroTipoManutencao.SelectedItem = "Todos";

            var tiposServico = manutencoes.Select(m => m.TipoServico).Distinct().ToList();
            tiposServico.Insert(0, "Todos");
            tiposServico.Sort();
            cmbFiltroTipoServico.ItemsSource = tiposServico;
            cmbFiltroTipoServico.SelectedItem = "Todos";
        }

        private void geraTabela()
        {
            var filtroTipo = cmbFiltroTipoManutencao.SelectedItem?.ToString() ?? "Todos";
            var filtroServico = cmbFiltroTipoServico.SelectedItem?.ToString() ?? "Todos";

            var listaFiltrada = manutencoes
                .Where(m =>
                    m.ElementId == elementoID &&
                    (filtroTipo == "Todos" || m.TipoManutencao == filtroTipo) &&
                    (filtroServico == "Todos" || m.TipoServico == filtroServico))
                .Select(m => new
                {
                    m.TipoServico,
                    m.TipoManutencao,
                    DataAbertura = !string.IsNullOrEmpty(m.DataAbertura) ? m.DataAbertura : "-",
                    DataFechamento = !string.IsNullOrEmpty(m.DataFechamento) ? m.DataFechamento : "-",
                    TempoExecucao = CalcularTempoExecucao(m.DataAbertura, m.DataFechamento)
                })
                .ToList();

            dgManutencoes.ItemsSource = listaFiltrada;
        }

        private int CalcularTempoExecucao(string dataAbertura, string dataFechamento)
        {
            if (DateTime.TryParse(dataAbertura, out DateTime abertura) &&
                DateTime.TryParse(dataFechamento, out DateTime fechamento))
            {
                return (fechamento - abertura).Days;
            }
            return 0;
        }

        private void cmbFiltroTipoManutencao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            geraTabela();
        }

        private void cmbFiltroTipoServico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            geraTabela();
        }

        private void ExportarPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog salvarArquivo = new SaveFileDialog
                {
                    Filter = ".pdf",
                    Title = "Salvar Relatório",
                    FileName = "Q4Main-RelatorioManutençõesElemento-" + elementoID,
                    DefaultExt = ".pdf",
                };
                if (salvarArquivo.ShowDialog() == true)
                {
                    Document doc = new Document(iTextSharp.text.PageSize.A4);
                    PdfWriter.GetInstance(doc, new FileStream(salvarArquivo.FileName, FileMode.Create));
                    doc.Open();
                    iTextSharp.text.Font fonte = FontFactory.GetFont("Arial Unicode MS", 12, iTextSharp.text.Font.NORMAL);
                    Paragraph tituloPDF = new Paragraph($"Q4Main: Relatório de Manutenções do Elemento - ID #{elementoID}\n\n", fonte)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER
                    };
                    doc.Add(tituloPDF);

                    iTextSharp.text.Font fonteTab = FontFactory.GetFont("Arial Unicode MS", 7, iTextSharp.text.Font.NORMAL);

                    PdfPCell CriaCelula(string texto)
                    {

                        PdfPCell cell = new PdfPCell(new Phrase(texto, fonteTab));
                        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        return cell;
                    }

                    PdfPTable tabela = new PdfPTable(5);
                    tabela.AddCell(CriaCelula("Tipo de Manutenção"));
                    tabela.AddCell(CriaCelula("Tipo de Serviço"));
                    tabela.AddCell(CriaCelula("Data de Abertura"));
                    tabela.AddCell(CriaCelula("Data de Fechamento"));
                    tabela.AddCell(CriaCelula("Tempo de Execução (dias)"));

                    foreach (var item in dgManutencoes.Items)
                    {
                        dynamic row = item;
                        tabela.AddCell(CriaCelula(row.TipoManutencao));
                        tabela.AddCell(CriaCelula(row.TipoServico));
                        tabela.AddCell(CriaCelula(row.DataAbertura));
                        tabela.AddCell(CriaCelula(row.DataFechamento));
                        tabela.AddCell(CriaCelula(row.TempoExecucao.ToString()));
                    }

                    doc.Add(tabela);

                    iTextSharp.text.Font infoFonte = FontFactory.GetFont("Arial Unicode MS", 6, iTextSharp.text.Font.NORMAL);
                    string dataGeracao = "\n\nEste relatório foi gerado em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    Paragraph paragrafoData = new Paragraph(dataGeracao, infoFonte)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER
                    };
                    doc.Add(paragrafoData);

                    doc.Close();
                    MessageBox.Show("Relatório salvo com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgManutencoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
