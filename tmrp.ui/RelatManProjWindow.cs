 namespace tmrp.ui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Win32;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using System.IO;

    public partial class RelatManProjWindow : Window
    {
        private List<DadosManutencao> manutencoesProj;
        private int totalManutencoesProj;

        public RelatManProjWindow(List<DadosManutencao> dadosManutencaoProj, int totalManutencoesProj)
        {
            InitializeComponent();
            manutencoesProj = dadosManutencaoProj;
            this.totalManutencoesProj = totalManutencoesProj;

            Title = "Q4Main: Análise de manutenções";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            geraCabecalhoProj();
            popFiltroProj();
            geraTabelaProj();
        }

        private void geraCabecalhoProj()
        {
            txtTotalManutencoesProj.Text = $"Total de manutenções registradas no projeto: {totalManutencoesProj}";

            double mediaTempoExecucao = manutencoesProj
                .Where(m => DateTime.TryParse(m.DataAbertura, out _) && DateTime.TryParse(m.DataFechamento, out _))
                .Select(m => CalcularTempoExecucaoProj(m.DataAbertura, m.DataFechamento))
                .DefaultIfEmpty(0)
                .Average();
            txtMediaTempoExecucaoProj.Text = $"Tempo médio de execução das manutenções: {mediaTempoExecucao:F1} dias";
        }   
    
        private void popFiltroProj()
        {
            var tiposManutencao = manutencoesProj.Select(m => m.TipoManutencao).Distinct().ToList();
            tiposManutencao.Insert(0, "Todos");
            tiposManutencao.Sort();
            cmbFiltroTipoManutencaoProj.ItemsSource = tiposManutencao;
            cmbFiltroTipoManutencaoProj.SelectedItem = "Todos";

            var tiposServico = manutencoesProj.Select(m => m.TipoServico).Distinct().ToList();
            tiposServico.Insert(0, "Todos");
            tiposServico.Sort();
            cmbFiltroTipoServicoProj.ItemsSource = tiposServico;
            cmbFiltroTipoServicoProj.SelectedItem = "Todos";
        }

        private void geraTabelaProj()
        {
            var filtroTipo = cmbFiltroTipoManutencaoProj.SelectedItem?.ToString() ?? "Todos";
            var filtroServico = cmbFiltroTipoServicoProj.SelectedItem?.ToString() ?? "Todos";

            var listaFiltrada = manutencoesProj
                .Where(m =>
                    (filtroTipo == "Todos" || m.TipoManutencao == filtroTipo) &&
                    (filtroServico == "Todos" || m.TipoServico == filtroServico))
                .Select(m => new
                {
                    m.ElementId,
                    m.TipoServico,
                    m.TipoManutencao,
                    DataAbertura = m.DataAbertura,
                    DataFechamento = !string.IsNullOrEmpty(m.DataFechamento) ? m.DataFechamento : "-",
                    TempoExecucao = CalcularTempoExecucaoProj(m.DataAbertura, m.DataFechamento)
                })
                .ToList();

            dgManutencoesProj.ItemsSource = listaFiltrada;
        }

        private int CalcularTempoExecucaoProj(string dataAbertura, string dataFechamento)
        {
            if (DateTime.TryParse(dataAbertura, out DateTime abertura) &&
                DateTime.TryParse(dataFechamento, out DateTime fechamento))
            {
                return (fechamento - abertura).Days;
            }
            return 0;
        }

        private void cmbFiltroTipoManutencaoProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            geraTabelaProj();
        }

        private void cmbFiltroTipoServicoProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            geraTabelaProj();
        }

        private void ExportarPDFProj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog salvarArquivo = new SaveFileDialog
                {
                    Filter = ".pdf",
                    Title = "Salvar Relatório",
                    FileName = "Q4Main-RelatorioManutençõesProjeto",
                    DefaultExt = ".pdf",
                };
                if (salvarArquivo.ShowDialog() == true)
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    PdfWriter.GetInstance(doc, new FileStream(salvarArquivo.FileName, FileMode.Create));
                    doc.Open();
                    iTextSharp.text.Font font = FontFactory.GetFont("Arial Unicode MS", 12, iTextSharp.text.Font.NORMAL);

                    Paragraph tituloPDF = new Paragraph($"Q4Main: Relatório de Manutenções do Projeto\n\n", font)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER
                    };
                    doc.Add(tituloPDF);

                    iTextSharp.text.Font fonteTab = FontFactory.GetFont("Arial Unicode MS", 7, iTextSharp.text.Font.NORMAL);

                    PdfPCell CriaCelula(string texto)
                    {

                        PdfPCell celula = new PdfPCell(new Phrase(texto, fonteTab));
                        celula.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        return celula;
                    }

                    PdfPTable tabela = new PdfPTable(6);
                    tabela.AddCell(CriaCelula("Element ID"));
                    tabela.AddCell(CriaCelula("Tipo de Manutenção"));
                    tabela.AddCell(CriaCelula("Tipo de Serviço"));
                    tabela.AddCell(CriaCelula("Data de Abertura"));
                    tabela.AddCell(CriaCelula("Data de Fechamento"));
                    tabela.AddCell(CriaCelula("Tempo de Execução (dias)"));

                    foreach (var item in dgManutencoesProj.Items)
                    {
                        dynamic row = item;
                        tabela.AddCell(CriaCelula(row.ElementId));
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

        private void dgManutencoesProj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
