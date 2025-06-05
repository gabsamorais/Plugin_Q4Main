namespace tmrp.core
{
    using Autodesk.Revit.Attributes;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    [Transaction(TransactionMode.Manual)]

    // Exibe a janela com informações sobre a autoria do plugin
    public class SobreComando : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog caixaDialogo = new TaskDialog("Saiba mais sobre o plugin")
            {
                MainContent = "O Q4Main (Queue for Maintenance) é um plugin desenvolvido para fins educacionais, como parte da pesquisa de doutorado da discente Gabriela Alves T. de Morais, realizada na Universidade Federal de Pernambuco (UFPE)." +
                                "O plugin permite o registro de manutenções diretamente no modelo BIM e auxilia no dimensionamento de equipes de manutenção predial por meio de um modelo de filas (M/M/s).\n" +
                                    "O plugin possibilita ainda visualizar e exportar relatórios em PDF bem como acessar dashboards desenvolvidos no Power BI.\n" +
                                        "Para mais informações, entre em contato pelo e-mail: gabriela.alvesm@ufpe.br.",

                CommonButtons = TaskDialogCommonButtons.Ok
            };

            caixaDialogo.Show();

            return Result.Succeeded;
        }

        public static string GetPath()
        {
            return typeof(SobreComando).Namespace + "." + nameof(SobreComando);
        }
    }
}