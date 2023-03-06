using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools;

namespace LockSelf
{
    public partial class LockSelfRibbon
    {
        Microsoft.Office.Tools.CustomTaskPane _myCustomTaskPane;

        private void LockSelfRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void toggleBtn_Click(object sender, RibbonControlEventArgs e)
        {
            Outlook.Inspector inspector = (Outlook.Inspector)e.Control.Context;
            InspectorWrapper inspectorWrapper = Globals.ThisAddIn.InspectorWrappers[inspector];
            CustomTaskPane taskPane = inspectorWrapper.CustomTaskPane;
            taskPane.Visible = ((RibbonToggleButton)sender).Checked;
        }
    }
}