namespace tmrp
{
    using Autodesk.Revit.UI;
    using tmrp.core;
    using tmrp.res;
    using System;

    public class SetupInterface
    {
        public void Initialize(UIControlledApplication app)
        {
            string tabName = "Q4Main";
            app.CreateRibbonTab(tabName);

            var painelManutencao = app.CreateRibbonPanel(tabName, "Manutenção");
            var painelEquipes = app.CreateRibbonPanel(tabName, "Equipes");
            var painelAnalises = app.CreateRibbonPanel(tabName, "Análises");
            var painelSobre = app.CreateRibbonPanel(tabName, "Sobre");

            // Painel Manutenção
            // Botão "Registro de Manutenção
            var botaoRegistro = new RevitPushButtonDataModel
            {
                Label = "Registro\nde Manutenção",
                Panel = painelManutencao,
                Tooltip = "Faça o registro de uma nova manutenção - preventiva ou corretiva.",
                CommandNamespacePath = RegManComando.GetPath(),
                IconImageName = "regmain.png",
                TooltipImageName = "building2.png",
            };
            RevitPushButton.Create(botaoRegistro);

            // Painel Equipes
            // Botão "Analisar fontes de dados"
            var botaoAnaliseDados = new RevitPushButtonDataModel
            {
                Label = "Analisar fontes\n de dados",
                Panel = painelEquipes,
                Tooltip = "Analise os registros de manutenção do modelo para verificar se o ajuste à distribuição exponencial é válido. Se os indicadores estatísticos forem satisfatórios, recomenda-se a utilização dos dados do modelo; caso contrário, opte por dados elicitados com o apoio de um especialista.",
                CommandNamespacePath = VerifDadosComando.GetPath(),
                IconImageName = "decision.png",
                TooltipImageName = "building.png",
            };
            RevitPushButton.Create(botaoAnaliseDados);

            // Botão "Dimensionamento de equipes"
            var botaoDimEquipes = new RevitPushButtonDataModel
            {
                Label = "Dimensionamento\nde Equipes",
                Panel = painelEquipes,
                Tooltip = "Faça o dimensionamento de uma equipe com base nos dados das solicitações de manutenção.",
                CommandNamespacePath = DimEqpComando.GetPath(),
                IconImageName = "team.png",
                TooltipImageName = "building.png",
            };
            RevitPushButton.Create(botaoDimEquipes);

            // Painel Análises
            // Botão "Relatório do projeto"
            var botaoRelatProj = new RevitPushButtonDataModel
            {
                Label = "Relatório\n do Projeto",
                Panel = painelAnalises,
                Tooltip = "Verifique o histórico de manutenções registradas no projeto.",
                CommandNamespacePath = RelatManProjComando.GetPath(),
                IconImageName = "construction.png",
                TooltipImageName = "data.png",
            };
            RevitPushButton.Create(botaoRelatProj);

            // Botão "Relatório do elemento"
            var botaoRelatElem = new RevitPushButtonDataModel
            {
                Label = "Relatório\n do Elemento",
                Panel = painelAnalises,
                Tooltip = "Verifique o histórico de manutenções de um elemento selecionado.",
                CommandNamespacePath = RelatManElemComando.GetPath(),
                IconImageName = "brick.png",
                TooltipImageName = "data.png",
            };
            RevitPushButton.Create(botaoRelatElem);

            // Botão suspenso "Dashboard Power BI"
            var botaoSuspDash = new PulldownButtonData("DashboardPowerBI", "Dashboard\nPower BI")
            {
                ToolTip = "Verifique o dashboard com estatísticas das manutenção do projeto.",
                LargeImage = ResourceImage.GetIcon("dashboard1.png"),
                ToolTipImage = ResourceImage.GetIcon("data.png")
            };
            PulldownButton dashSusp = painelAnalises.AddItem(botaoSuspDash) as PulldownButton;

            // Botão "Inserir Link"
            var dashSuspInserir = new PushButtonData(
                "LinkDashPBICommand",
                "Inserir link",
                CoreAssembly.GetAssemblyLocation(),
                LinkDashPBIComando.GetPath())
            {
                ToolTip = "Insira o link de um novo dashboard do Power BI.",
                LargeImage = ResourceImage.GetIcon("dashboard1.png")
            };

            var dashSuspExibir = new PushButtonData(
                "DashPBICommand",
                "Exibir dashboard",
                CoreAssembly.GetAssemblyLocation(),
                DashPBIComando.GetPath())
            {
                ToolTip = "Selecione e visualize o dashboard do projeto no Power BI.",
                LargeImage = ResourceImage.GetIcon("dashboard1.png")
            };

            dashSusp.AddPushButton(dashSuspInserir);
            dashSusp.AddPushButton(dashSuspExibir);

            // Painel Sobre
            // Botão "Q4Main Plugin""
            var botaoSobre = new RevitPushButtonDataModel
            {
                Label = "Q4Main\nPlugin",
                Panel = painelSobre,
                Tooltip = "Saiba mais sobre o Q4Main.",
                CommandNamespacePath = SobreComando.GetPath(),
                IconImageName = "Sobre.png",
                TooltipImageName = "Sobre.png",
            };
            RevitPushButton.Create(botaoSobre);
        }
    }

    public static class RevitPushButton
    {
        public static Autodesk.Revit.UI.PushButton Create(RevitPushButtonDataModel data)
        {
            var botaoNome = Guid.NewGuid().ToString();

            var botao = new PushButtonData(botaoNome, data.Label, CoreAssembly.GetAssemblyLocation(), data.CommandNamespacePath)
            {
                ToolTip = data.Tooltip,
                LargeImage = ResourceImage.GetIcon(data.IconImageName),
                ToolTipImage = ResourceImage.GetIcon(data.TooltipImageName)
            };
            return data.Panel.AddItem(botao) as Autodesk.Revit.UI.PushButton;
        }
    }

    public class RevitPushButtonDataModel
    {
        public string Label { get; set; }
        public RibbonPanel Panel { get; set; }
        public string CommandNamespacePath { get; set; }
        public string Tooltip { get; set; }
        public string IconImageName { get; set; }
        public string TooltipImageName { get; set; }


        public RevitPushButtonDataModel()
        {

        }
    }
}
