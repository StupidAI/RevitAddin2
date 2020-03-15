#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace RevitAddin2
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Element selectedElement = null;
            foreach (ElementId id in uidoc.Selection.GetElementIds())
            {
                selectedElement = doc.GetElement(id);
                break;  // just get one selected element
            }
            // gittest
            // Get the category instance from the Category property
            Category category = selectedElement.Category;

            BuiltInCategory enumCategory = (BuiltInCategory)category.Id.IntegerValue;

            // Format the prompt string, which contains the category information
            String prompt = "The category information of the selected element is: ";
            prompt += "\n\tName:\t" + category.Name;   // Name information

            prompt += "\n\tId:\t" + enumCategory.ToString();    // Id information
            prompt += "\n\tParent:\t";
            if (null == category.Parent)
            {
                prompt += "No Parent Category";   // Parent information, it may be null
            }
            else
            {
                prompt += category.Parent.Name;
            }

            prompt += "\n\tSubCategories:"; // SubCategories information, 
            CategoryNameMap subCategories = category.SubCategories;
            if (null == subCategories || 0 == subCategories.Size) // It may be null or has no item in it
            {
                prompt += "No SubCategories;";
            }
            else
            {
                foreach (Category ii in subCategories)
                {
                    prompt += "\n\t\t" + ii.Name;
                }
            }

            // Give the user some information
            TaskDialog.Show("Revit", prompt);

            Random rnd = new Random();

            OverrideGraphicSettings myOGS = new OverrideGraphicSettings();
            
            using (Transaction t = new Transaction(doc, "Change color"))
            {
                t.Start();
                if (null == subCategories || 0 == subCategories.Size) // It may be null or has no item in it
                {
                }
                else
                {
                    foreach (Category ii in subCategories)
                    {
                        ElementId elemID = ii.Id;
                        myOGS.SetProjectionLineColor(new Color(
                                                            (byte)rnd.Next(0, 256), 
                                                            (byte)rnd.Next(0, 256), 
                                                            (byte)rnd.Next(0, 256)
                                                               ));
                        doc.ActiveView.SetCategoryOverrides(elemID, myOGS);
                    }
                }
                t.Commit();
            }
            

            return Result.Succeeded;
        }
    }
}
