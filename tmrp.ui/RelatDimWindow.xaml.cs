namespace tmrp.ui
{
    using System.IO;
    using System.Windows;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Microsoft.Win32;

    public partial class RelatDimWindow : Window
    {
        private string servTypeRDE;

        public RelatDimWindow(string reportText, string servType)
        {
            InitializeComponent();
            txtReport.Text = reportText;
            servTypeRDE = servType;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;
        }

        private void ExportarPDFRDE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog salvarArquivoRDE = new SaveFileDialog 
                { 
                    Filter = ".pdf", 
                    Title = "Salvar Relatório",
                    FileName = "Q4Main-RelatorioDimensionamentoEquipes-" + servTypeRDE,
                    DefaultExt = ".pdf",
                };

                if (salvarArquivoRDE.ShowDialog() == true)
                {
                    iTextSharp.text.Document docRDE = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    PdfWriter.GetInstance(docRDE, new FileStream(salvarArquivoRDE.FileName, FileMode.Create));
                    docRDE.Open();
                    iTextSharp.text.Font fonte = FontFactory.GetFont("Arial Unicode MS", 12, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font fonteRelat = FontFactory.GetFont("Arial Unicode MS", 10, iTextSharp.text.Font.NORMAL);
                    Paragraph tituloPDF = new Paragraph($"\nQ4Main: Relatório de Dimensionamento de Equipes\n", fonte)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER
                    };
                    docRDE.Add(tituloPDF);
                    Paragraph tipServPDF = new Paragraph($"Serviço em análise - " + servTypeRDE, fonte)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER
                    };
                    docRDE.Add(tipServPDF);
                    docRDE.Add(new Paragraph(txtReport.Text, fonteRelat));

                    iTextSharp.text.Font infoFonte = FontFactory.GetFont("Arial Unicode MS", 6, iTextSharp.text.Font.NORMAL);
                    string dataGeracao = "\n\nEste relatório foi gerado em: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    Paragraph paragrafoData = new Paragraph(dataGeracao, infoFonte)
                    {
                        Alignment = iTextSharp.text.Element.ALIGN_CENTER
                    };
                    docRDE.Add(paragrafoData);

                    docRDE.Close();
                    MessageBox.Show("Relatório salvo com sucesso.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar PDF: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
