
namespace LockSelf
{
    partial class LockSelfRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public LockSelfRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.LockSelf = this.Factory.CreateRibbonGroup();
            this.toggleBtn = this.Factory.CreateRibbonToggleButton();
            this.tab1.SuspendLayout();
            this.LockSelf.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.ControlId.OfficeId = "TabNewMailMessage";
            this.tab1.Groups.Add(this.LockSelf);
            this.tab1.Label = "TabNewMailMessage";
            this.tab1.Name = "tab1";
            // 
            // LockSelf
            // 
            this.LockSelf.Items.Add(this.toggleBtn);
            this.LockSelf.Label = "LockSelf";
            this.LockSelf.Name = "LockSelf";
            // 
            // toggleBtn
            // 
            this.toggleBtn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.toggleBtn.Image = global::LockSelf.Properties.Resources.lockself_ico;
            this.toggleBtn.Label = "Sécuriser des pièces jointes";
            this.toggleBtn.Name = "toggleBtn";
            this.toggleBtn.ShowImage = true;
            this.toggleBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.toggleBtn_Click);
            // 
            // LockSelfRibbon
            // 
            this.Name = "LockSelfRibbon";
            this.RibbonType = "Microsoft.Outlook.Mail.Compose, Microsoft.Outlook.Mail.Read";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.LockSelfRibbon_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.LockSelf.ResumeLayout(false);
            this.LockSelf.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup LockSelf;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton toggleBtn;
    }

    partial class ThisRibbonCollection
    {
        internal LockSelfRibbon LockSelfRibbon
        {
            get { return this.GetRibbon<LockSelfRibbon>(); }
        }
    }
}
