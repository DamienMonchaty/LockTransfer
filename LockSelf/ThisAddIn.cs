using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using LockSelf.Controls;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using Autofac;
using Autofac.Features.ResolveAnything;
using Core.Services.Impl;
using Core.Services;
using LockSelf.ViewModels;

namespace LockSelf
{
    // Outlook 2013 asp net 3.15 
    // Update

    // VSTO GoToMeeting a partir de 4.6

    // Classe qui affiche le customtaskPane selon l'explorer
    // nous permet d'ouvrir le TaskPane ds Mail Compose
    public class InspectorWrapper
    {
        private Outlook.Inspector inspector;
        private Microsoft.Office.Tools.CustomTaskPane _myCustomTaskPane;

        // User control
        private UserControl _usr;

        public InspectorWrapper(Outlook.Inspector Inspector)
        {
            inspector = Inspector;
            ((Outlook.InspectorEvents_Event)inspector).Close +=
                new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);

            //Create an instance of the user control
            _usr = new TransferControlForm();

            // Create an ElementHost and create the WPF user control and assign to the ElementHost child
            ElementHost _eh = new ElementHost { Child = new TransferControlWpf() };

            // Add the ElementHost to the standard user control Controls
            _usr.Controls.Add(_eh);
            // Dock the ElementHost to fill
            _eh.Dock = DockStyle.Fill;

            // Connect the user control and the custom task pane 
            _myCustomTaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(_usr, "LockSelf", inspector);
            //_myCustomTaskPane.Visible = true;
            _myCustomTaskPane.Width = 440;
            _myCustomTaskPane.VisibleChanged += new EventHandler(TaskPane_VisibleChanged);
        }

        void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            Globals.Ribbons[inspector].LockSelfRibbon.toggleBtn.Checked =
                _myCustomTaskPane.Visible;
        }

        void InspectorWrapper_Close()
        {
            if (_myCustomTaskPane != null)
            {
                Globals.ThisAddIn.CustomTaskPanes.Remove(_myCustomTaskPane);
            }

            _myCustomTaskPane = null;
            Globals.ThisAddIn.InspectorWrappers.Remove(inspector);
            ((Outlook.InspectorEvents_Event)inspector).Close -=
                new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);
            inspector = null;
        }

        public Microsoft.Office.Tools.CustomTaskPane CustomTaskPane
        {
            get
            {
                return _myCustomTaskPane;
            }
        }
    }
    public partial class ThisAddIn
    {
        private Dictionary<Outlook.Inspector, InspectorWrapper> inspectorWrappersValue = new Dictionary<Outlook.Inspector, InspectorWrapper>();
        private Outlook.Inspectors inspectors;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            inspectors = this.Application.Inspectors;
            inspectors.NewInspector +=
                new Outlook.InspectorsEvents_NewInspectorEventHandler(
                Inspectors_NewInspector);

            foreach (Outlook.Inspector inspector in inspectors)
            {
                Inspectors_NewInspector(inspector);
            }


            // Chargement de la langue du systeme
            System.Globalization.CultureInfo ciTest = new System.Globalization.CultureInfo("de-DE");

            Microsoft.Office.Interop.Outlook.Application app = this.GetHostItem<Microsoft.Office.Interop.Outlook.Application>(typeof(Microsoft.Office.Interop.Outlook.Application), "Application");
            int lcid = app.LanguageSettings.get_LanguageID(Office.MsoAppLanguageID.msoLanguageIDUI);

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(lcid);

            //LockSelf.Properties.Resources.Culture = ciTest;

            LockSelf.Properties.Resources.Culture = new CultureInfo(lcid);

            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            Debug.WriteLine("Cultureinfo locale -> " + ci + " & id -> " + lcid);

            // Autofac / DI
            var builder = new ContainerBuilder();
            //allow the Autofac container resolve unknown types
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            
            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<AuthSSOService>().As<IAuthSSOService>().SingleInstance();
            builder.RegisterType<TransferService>().As<ITransferService>().SingleInstance();

            IContainer container = builder.Build();
            //get a TransferViewModel instance
            TransferViewModel mainViewModel = container.Resolve<TransferViewModel>();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            inspectors.NewInspector -= new Outlook.InspectorsEvents_NewInspectorEventHandler(Inspectors_NewInspector);
            inspectors = null;
            inspectorWrappersValue = null;
        }

        void Inspectors_NewInspector(Outlook.Inspector Inspector)
        {
            if (Inspector.CurrentItem is Outlook.MailItem)
            {
                inspectorWrappersValue.Add(Inspector, new InspectorWrapper(Inspector));
            }
        }

        public Dictionary<Outlook.Inspector, InspectorWrapper> InspectorWrappers
        {
            get
            {
                return inspectorWrappersValue;
            }
        }

        #region Code généré par VSTO

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
