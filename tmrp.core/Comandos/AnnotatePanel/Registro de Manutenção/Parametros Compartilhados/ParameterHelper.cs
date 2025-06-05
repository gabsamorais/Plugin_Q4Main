namespace tmrp.core
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using Autodesk.Revit.ApplicationServices;
    using System.Collections.Generic;
    using System.IO;

    // Define a estrutura do arquivo de parâmetros compartilhados
    public class ParametrosCompartilhados
    {
        public static void AdParametrosCompartilhados(Document doc, string paramNome, ForgeTypeId paramTipo, bool isInstance)
        {
            Application app = doc.Application;
            string diretorio = app.SharedParametersFilename;
           
            if (string.IsNullOrEmpty(diretorio) || !File.Exists(diretorio))
            {
                diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", "SharedParameters.txt");

                try
                {
                    using (StreamWriter writer = new StreamWriter(diretorio, false))
                    {
                        writer.WriteLine("# This is a Revit shared parameter file.");
                        writer.WriteLine("# Do not edit manually.");
                        writer.WriteLine("*META\tVERSION\tMINVERSION");
                        writer.WriteLine("META\t2\t1");
                        writer.WriteLine("*GROUP\tID\tNAME");
                        writer.WriteLine("GROUP\t1\tGestão da Manutenção\r\n");
                        writer.WriteLine("*PARAM\tGUID\tNAME\tDATATYPE\tDATACATEGORY\tGROUP\tVISIBLE\tDESCRIPTION\tUSERMODIFIABLE\tHIDEWHENNOVALUE");
                    }
                    app.SharedParametersFilename = diretorio;
                    Autodesk.Revit.UI.TaskDialog.Show("Sucesso", "O arquivo de parâmetros compartilhados foi criado.");
                }
                catch (IOException ex)
                {
                    TaskDialog.Show("Erro", "Falha ao criar o arquivo de parâmetros compartilhados." + ex.Message);
                    return;
                }
            }

            app.SharedParametersFilename = diretorio;
            DefinitionFile defFile = app.OpenSharedParameterFile();
            if (defFile == null)
            {
                TaskDialog.Show("Erro", "Falha ao abrir o arquivo de parâmetros compartilhados.");
                return;
            }

            BindingMap bindingMap = doc.ParameterBindings;
            DefinitionBindingMapIterator iter = bindingMap.ForwardIterator();

            DefinitionGroup defGroup = defFile.Groups.get_Item("Gestão da manutenção") ?? defFile.Groups.Create("Gestão da manutenção");
            if (defGroup ==  null)
            {
                TaskDialog.Show("Erro", "Falha ao criar ou acessar o grupo de parâmetros compartilhados.");
                return;
            }

            using(Transaction trans = new Transaction(doc, "Adicionar Parâmetro Compartilhado"))
            {
                trans.Start();
                Definition definition = defGroup.Definitions.get_Item(paramNome)?? defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(paramNome, paramTipo));

                // Categorias Revit habilitadas para inserção dos parâmetros compartilhados
                CategorySet catSet = new CategorySet();
                List<BuiltInCategory> categorias = new List<BuiltInCategory>
                {
                    BuiltInCategory.OST_Walls,
                    BuiltInCategory.OST_Floors,
                    BuiltInCategory.OST_Ceilings,
                    BuiltInCategory.OST_Roofs,
                    BuiltInCategory.OST_Doors,
                    BuiltInCategory.OST_Windows,
                    BuiltInCategory.OST_MechanicalEquipment,
                    BuiltInCategory.OST_Furniture,
                    BuiltInCategory.OST_Columns,
                    BuiltInCategory.OST_LightingFixtures
                };

                foreach (var catItem in categorias)
                {
                    Category cat = doc.Settings.Categories.get_Item(catItem);
                    if (cat != null)
                    {
                        catSet.Insert(cat);
                    }
                }

                Binding binding = isInstance ? (Binding)new InstanceBinding(catSet) : new TypeBinding(catSet);
                
                if (!bindingMap.Contains(definition))
                {
                    bool sucess = bindingMap.Insert(definition, binding, GroupTypeId.Data);
                    if (!sucess)
                    {
                        TaskDialog.Show("Erro", $"Não foi possível adicionar o parâmetro '{paramNome}' ao projeto.");
                    }
                }
                trans.Commit();
            }
        }
    }
}
