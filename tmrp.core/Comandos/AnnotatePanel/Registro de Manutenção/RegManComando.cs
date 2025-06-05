namespace tmrp.core
{
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;
    using tmrp.core.Comandos.Type;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    // Implementa a lógica de decisão para realizar o registro de manutenções
    // Comando para abrir o formulário de Registro de Manutenção
    public class RegManComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var parametros = new[] { "Tipo de Serviço", "Tipo de Manutenção", "Data de Abertura", "Data de Fechamento" };
            foreach ( var parametro in parametros )
            {
                ParametrosCompartilhados.AdParametrosCompartilhados(doc, parametro, SpecTypeId.String.Text, true);
            }    

            if (doc.IsFamilyDocument)
            {
                Message.Display("Este comando não pode ser utilizado em arquivos de família.", WindowType.Warning);
                return Result.Cancelled;
            }

            var vistaAtual = uidoc.ActiveView;

            bool vistaHabilitada = vistaAtual.ViewType is
                ViewType.FloorPlan or
                ViewType.CeilingPlan or
                ViewType.Detail or
                ViewType.Elevation or
                ViewType.Section or
                ViewType.ThreeD;

            if (!vistaHabilitada)
            {
                Message.Display("Não é possível selecionar um elemento válido na visualização atual.", WindowType.Error);
                return Result.Cancelled;
            }

            try
            {
                Reference referencia = uidoc.Selection.PickObject(ObjectType.Element, new FiltroSelecaoElementosRevit(), "Selecione um elemento válido");
                var elemento = doc.GetElement(referencia.ElementId);

                if (elemento == null)
                {
                    Message.Display("Nenhum elemento válido foi selecionado.", WindowType.Warning);
                    return Result.Cancelled;
                }

                using (var window = new FormRegMan(uidoc, elemento))
                {
                    window.ShowDialog();
                    if(window.DialogResult == DialogResult.Cancel)
                        return Result.Cancelled;
                }

                return Result.Succeeded;
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar elemento: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }

        public static string GetPath()
        {
            return typeof(RegManComando).Namespace + "." + nameof(RegManComando);
        }
    }
}