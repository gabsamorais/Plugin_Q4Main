namespace tmrp.core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.UI.Selection;
    using tmrp.core.Comandos.Type;
    using tmrp.ui;

    [Transaction(TransactionMode.Manual)]   

    // Apresenta o relatório de manutenções do elemento selecionado
    public class RelatManElemComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (doc.IsFamilyDocument)
            {
                Message.Display("Não é possível usar o comando no documento de família", WindowType.Warning);
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
                Message.Display("Não é possível selecionar um elemento na visualização atual.", WindowType.Error);
                return Result.Cancelled;
            }

            try
            {
                Reference referencia = uidoc.Selection.PickObject(ObjectType.Element, new FiltroSelecaoElementosRevit(), "Selecione um elemento válido para gerar o relatório de manutenções.");
                if (referencia == null )
                {
                    TaskDialog.Show("Erro", "Nenhum elemento foi selecionado.");
                    return Result.Cancelled;
                }

                Element elemento = doc.GetElement(referencia.ElementId);
                if (elemento == null)
                {
                    TaskDialog.Show("Erro", "Elemento inválido.");
                    return Result.Failed;
                }

                string nomeProjeto = Path.GetFileNameWithoutExtension(doc.PathName);
                string nomeArquivo = $"RegistroManutencao_{nomeProjeto}.json";
                string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", nomeArquivo);

                if (!File.Exists(diretorio))
                {
                    TaskDialog.Show("Erro", "O arquivo de registros de manutençao não foi encontrado");
                    return Result.Failed;
                }

                string json = File.ReadAllText(diretorio);
                List<DadosManutencao> dadosManutencao = JsonSerializer.Deserialize<List<DadosManutencao>>(json) ?? new List<DadosManutencao>();

                string elementId = elemento.Id.ToString();

                List<DadosManutencao> manutencoesElemento = dadosManutencao.Where(m => m.ElementId == elementId).ToList();

                int totalProjeto = dadosManutencao.Count;

                int totalElemento = dadosManutencao.Count(m => m.ElementId == elementId);

                if (totalElemento == 0)
                {
                    TaskDialog.Show("Aviso", "O elemento selecionado não possui manutenções registradas.");
                    return Result.Cancelled;
                }

                RelatManWindow reportWindow = new RelatManWindow(dadosManutencao, elementId, totalProjeto);
                reportWindow.ShowDialog();

                return Result.Succeeded;    
            }

            catch (Exception ex) 
            {
                TaskDialog.Show("Erro", $"Falha ao gerar o relatório: {ex.Message}");
                return Result.Failed;
            }
        }

        public static string GetPath()
        {
            return typeof(RelatManElemComando).Namespace + "." + nameof(RelatManElemComando);
        }
    }
    
}
