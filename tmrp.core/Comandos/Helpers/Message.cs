namespace tmrp.core
{
    using Autodesk.Revit.UI;
    using tmrp.core.Comandos.Type;

    public static class Message
    {
        public static void Display(string message, WindowType type)
        {
            string title = "";

            var icon = TaskDialogIcon.TaskDialogIconNone;

            switch(type)
            {
                case WindowType.Information:
                    title = "~INFORMATION~";
                    icon = TaskDialogIcon.TaskDialogIconInformation;
                    break;
                case WindowType.Warning:
                    title = "~WARNING~";
                    icon = TaskDialogIcon.TaskDialogIconWarning;
                    break;
                case WindowType.Error:
                    title = "~ERROR~";
                    icon = TaskDialogIcon.TaskDialogIconError;
                    break;
                default:
                    break;
            }

            var window = new TaskDialog(title)
            {
                MainContent = message,
                MainIcon = icon,
                CommonButtons = TaskDialogCommonButtons.Ok
            };
            window.Show();
        }
    }
}
