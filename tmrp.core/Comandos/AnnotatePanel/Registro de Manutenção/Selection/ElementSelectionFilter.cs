namespace tmrp.core
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI.Selection;
    using System.Collections.Generic;

    // Define as categorias de elementos Revit que ficam habilitadas para seleção
    public class FiltroSelecaoElementosRevit : ISelectionFilter
    {
        private readonly HashSet<BuiltInCategory> categoriasHabilitadas = new HashSet<BuiltInCategory>
        {
            // Categorias permitidas para seleção
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Ceilings,
            BuiltInCategory.OST_Roofs,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Windows,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_SpecialityEquipment,
            BuiltInCategory.OST_Furniture,
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_LightingFixtures,
            BuiltInCategory.OST_PlumbingFixtures,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_Stairs,
            BuiltInCategory.OST_Railings
        };

        public bool AllowElement(Element elem)
        {
            if (elem == null || elem.Category == null)
                return false;
            return categoriasHabilitadas.Contains((BuiltInCategory)elem.Category.Id.IntegerValue);
        }

        public bool AllowReference(Reference reference, XYZ position)
        { 
            return false;
        }
    }
}
