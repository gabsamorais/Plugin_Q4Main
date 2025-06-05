namespace tmrp.core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using tmrp.ui;

    [Transaction(TransactionMode.Manual)]

    // Apresenta o relatório de manutenções de todo o projeto
    public class RelatManProjComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
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

                int totalProjeto = dadosManutencao.Count;

                if (totalProjeto == 0)
                {
                    TaskDialog.Show("Aviso", "Este projeto não possui manutenções registradas.");
                    return Result.Cancelled;
                }

                RelatManProjWindow reportWindow = new RelatManProjWindow(dadosManutencao, totalProjeto);
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
            return typeof(RelatManProjComando).Namespace + "." + nameof(RelatManProjComando);
        }
    }

}
