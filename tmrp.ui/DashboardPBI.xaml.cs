namespace tmrp.ui
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;

    public partial class DashboardPBI : UserControl
    {
        private readonly string dashboardUrl;
        public DashboardPBI(string url)
        {
            InitializeComponent();
            dashboardUrl = url;
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string arquivoCsv = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", "DashboardsPBI.csv");

                // Criação de pasta de dados do WebView2
                string diretorio = Path.GetDirectoryName(arquivoCsv) ?? throw new InvalidOperationException("Não foi possível localizar o diretório do arquivo de dashboards.");
                string pasta = Path.Combine(diretorio, "WebView2Data");
                Directory.CreateDirectory(pasta);

                var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, pasta);

                await webView.EnsureCoreWebView2Async(env);

                if (webView.CoreWebView2 != null)
                {
                    webView.Source = new Uri(dashboardUrl);
                }
                else
                {
                    MessageBox.Show("Não foi possível iniciar o WebView2.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o dashboard:\n" + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
