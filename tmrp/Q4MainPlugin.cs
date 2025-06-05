 namespace tmrp
{
    using Autodesk.Revit.UI;

    public class Q4MainPlugin: IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            var setup = new SetupInterface();
            setup.Initialize(application);

            return Result.Succeeded;
        }   

        public Result OnShutdown(UIControlledApplication application)
        {
            // Nenhuma ação de encerramento é necessária
            return Result.Succeeded;
        }

    }
}
