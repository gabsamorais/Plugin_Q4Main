namespace tmrp.core
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System.Windows.Forms;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    // Implementa a lógica de verificação da confiabilidade dos dados do modelo
    // Comando para abrir o formulário de Análise de dados do modelo
    public class VerifDadosComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Exibe o formulário de análise dos dados
            using (var formVDM = new FormVerifDados(uidoc))
            {
                if (formVDM.ShowDialog() == DialogResult.Cancel)
                    return Result.Cancelled;
            }
            return Result.Succeeded;
        }

        // Retorna o caminho do comando
        public static string GetPath()
        {
            return typeof(VerifDadosComando).Namespace + "." + nameof(VerifDadosComando);
        }
    }
}
