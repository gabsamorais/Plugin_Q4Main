namespace tmrp.core
{
    using System.Linq;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    // Implementa a lógica de decisão para o dimensionamento de equipes
    // Comando para abrir o formulário de Dimensionamento de equipes
    public class DimEqpComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
                        
            var dadosUsuario = new DimEqpComandoDados();

            // Exibe o formulário de Dimensionamento de equipes para coleta dos dados
            using (var windowDE = new FormDimEqp(uidoc))
            {
                windowDE.ShowDialog();

                if (windowDE.DialogResult == DialogResult.Cancel)
                    return Result.Cancelled;
                dadosUsuario = windowDE.GetInformation();
            }

            double[] chegadaElic;
            double lambda = 0;
            double mi = 0;

            // Determinação do método de cálculo da taxa de chegada
            // Se o usuário optar por usar os dados do modelo para taxa de chegada
            if (dadosUsuario.TaxChegadaModelo)
            {
                lambda = FilaMMS.CalcularTaxCheg(dadosUsuario.TipoServicoDE, doc);
                if (lambda == 0)
                {
                    MessageBox.Show("Não foi possível calcular a taxa de chegada com base nos dados registrados no modelo. Verifique os registros de manutenção do projeto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Result.Failed;
                }
            }

            // Se o usuário optar por usar os dados elicitados para taxa de chegada
            else if (dadosUsuario.TaxChegadaElic)
            {
                if (dadosUsuario.ListaTaxChegadaElic == null || !dadosUsuario.ListaTaxChegadaElic.Any())
                {
                    MessageBox.Show("Os dados elicitados para a taxa de chegada não foram informados corretamente. Verifique as informações inseridas.",  "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Result.Failed;
                }

                chegadaElic = FilaMMS.AjusteTmc(dadosUsuario.ListaTaxChegadaElic.Select(x => (double)x).ToArray());
                lambda = chegadaElic[2];
            }
            else
            {
                MessageBox.Show("Nenhuma opção foi selecionada para determinar a taxa de chegada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }

            // Determinação do método de cálculo da taxa de atendimento
            // Se o usuário optar por usar os dados do modelo para taxa de atendimento
            if (dadosUsuario.TaxAtendModelo)
            {
                mi = FilaMMS.CalcularTaxAtend(dadosUsuario.TipoServicoDE, doc);
                if (mi == 0)
                {
                    MessageBox.Show("Não foi possível calcular a taxa de atendimento com base nos dados registrados no modelo. Verifique os registros de manutenção do projeto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Result.Failed;
                }
            }

            // Se o usuário optar por usar um percentual da taxa de chegada como taxa de atendimento
            else if (dadosUsuario.TaxAtendElic)
            {
                mi = lambda * dadosUsuario.PercentAtend;
            }
            else
            {
                MessageBox.Show("Nenhuma opção foi selecionada para determinar a taxa de atendimento.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }

            // Relatório dos resultados com base na solução proposta em "SolucaoMMS"
            string relatorio = FilaMMS.GerarRelatorio(dadosUsuario.TipoServicoDE, lambda, mi, dadosUsuario.CustoAtend, dadosUsuario.CustoEsp);
            // Janela de exibição dos resultados
            var reportWindow = new tmrp.ui.RelatDimWindow(relatorio, dadosUsuario.TipoServicoDE);
            reportWindow.ShowDialog();

            return Result.Succeeded;
        }

        // Retorna o caminho do comando
        public static string GetPath()
        {
            return typeof(DimEqpComando).Namespace + "." + nameof(DimEqpComando);
        }
    }
}
