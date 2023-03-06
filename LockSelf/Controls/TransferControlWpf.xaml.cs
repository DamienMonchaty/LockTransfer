using Core.Services;
using Core.Services.Impl;
using LockSelf.Converters;
using LockSelf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LockSelf.Controls
{
    /// <summary>
    /// Logique d'interaction pour Transfer2ControlWpf.xaml
    /// </summary>
    public partial class TransferControlWpf : UserControl
    {
        Microsoft.Office.Tools.CustomTaskPane _myCustomTaskPane;

        public List<string> myCollection { get; set; }

        public TransferControlWpf()
        {
            
            IAuthService authService = new AuthService();
            ITransferService transferService = new TransferService();
            IAuthSSOService authSSOService = new AuthSSOService();
            DataContext = new TransferViewModel(transferService, authService, authSSOService);

            //ComboBox comboBox1 = GetVisualChild<ComboBox>(cp);

            //ComboBox comboBox1 = (ComboBox)cp.ContentTemplate.FindName("comboBox1", cp);
            //myCollection = new List<string> { "1", "2", "3" };
            //comboBox1.ItemsSource = myCollection;

            //MenuItem menuItemLanguages = GetVisualChild<MenuItem>(cp);

            //MenuItem menuItemLanguages = (MenuItem)cp.ContentTemplate.FindName("menuItemLanguages", cp);
            //foreach (MenuItem item in menuItemLanguages.Items)
            //{
            //    if (item.Tag.ToString() == Thread.CurrentThread.CurrentCulture.ToString())
            //    {
            //        item.IsChecked = true;
            //    }
            //}

            InitializeComponent();
        }

        bool isCollapsed = false;
        private void STCK_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stck = (StackPanel)cp.ContentTemplate.FindName("stck", cp);
            StackPanel stck2 = (StackPanel)cp.ContentTemplate.FindName("stck2", cp);
            DoubleAnimation db = new DoubleAnimation();
            if (isCollapsed)
            {
                db.To = 80;
                db.Duration = TimeSpan.FromSeconds(0.5);
                db.AutoReverse = false;
                db.RepeatBehavior = new RepeatBehavior(1);
                stck.BeginAnimation(StackPanel.HeightProperty, db);
                stck2.BeginAnimation(StackPanel.HeightProperty, db);

            }
            else
            {
                db.To = 0;
                db.Duration = TimeSpan.FromSeconds(0.5);
                db.AutoReverse = false;
                db.RepeatBehavior = new RepeatBehavior(1);
                stck.BeginAnimation(StackPanel.HeightProperty, db);
                stck2.BeginAnimation(StackPanel.HeightProperty, db);

            }
            isCollapsed = !isCollapsed;
        }     

        private void btn_Add(object sender, EventArgs e)
        {
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.AddCommand.CanExecute(null))) 
            {
                vm.AddCommand.Execute(null);
            }             
        }

        private void btn_Clear(object sender, EventArgs e)
        {
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.DeleteAllCommand.CanExecute(null)))
            {
                vm.DeleteAllCommand.Execute(null);
            }
        }

        private void btn_DeleteOne(object sender, RoutedEventArgs e)
        {
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.DeleteOneCommand.CanExecute(null)))
            {
                vm.DeleteOneCommand.Execute(null);
            }
        }

        private void btn_GenPassword(object sender, EventArgs e)
        {
            StackPanel stackPanelPhoneBtn = (StackPanel)cp.ContentTemplate.FindName("stackPanelPhoneBtn", cp);
            TextBox txtPasswordTransfer = (TextBox)cp.ContentTemplate.FindName("txtPasswordTransfer", cp);
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.GenPasswordCommand.CanExecute(null)))
            {
                stackPanelPhoneBtn.Visibility = Visibility.Visible;
                vm.GenPasswordCommand.Execute(null);
                txtPasswordTransfer.Text = vm.PasswordTransfer;
            }
        }

        private void btn_GenLink(object sender, EventArgs e)
        {
            TextBox txtPasswordTransfer = (TextBox)cp.ContentTemplate.FindName("txtPasswordTransfer", cp);
            TextBox txtPasswordTransfer2 = (TextBox)cp.ContentTemplate.FindName("txtPasswordTransfer2", cp);
            //TextBlock isoCountry = (TextBlock)cp.ContentTemplate.FindName("isoCountry", cp);

            //IsoToCountryCodeConverter isoToCountryCode = new IsoToCountryCodeConverter();
            //Binding binding = new Binding("")
            //{
            //    Converter = isoToCountryCode
            //};

            //isoCountry.SetBinding(TextBlock.TextProperty, binding);

            TransferViewModel vm = this.DataContext as TransferViewModel;

            //vm.Iso2 = isoCountry.Text;
            //Call command from viewmodel
            if ((vm != null) && (vm.GenLinkCommand.CanExecute(null)))
            {
                cbProgress.IsChecked = vm.ProgressLoading;
                vm.GenLinkCommand.Execute(null);
                txtPasswordTransfer.Clear();
                // txtPasswordTransfer2.Clear();
            }
            cbSuccess.IsChecked = vm.JobSuccess;
            
        }

        private T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void border1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TransferViewModel vm = this.DataContext as TransferViewModel;
            if ((e.ClickCount == 1) && (vm != null) && (vm.AddCommand.CanExecute(null)))
            {    
                vm.AddCommand.Execute(null);
                cbTransfer.IsChecked = true;
            }

        }
        
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition((IInputElement)e.Source);
            Debug.WriteLine("POINT -> " + point);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel txtPhone = (StackPanel)cp.ContentTemplate.FindName("stackPanelPhone", cp);
            if ((bool)(sender as ToggleButton).IsChecked)
            {
                txtPhone.Visibility = Visibility.Visible;
            }
            else
            {
                txtPhone.Visibility = Visibility.Hidden;
            }
        }

        private void border_DragEnter(object sender, DragEventArgs e)
        {
            Border border = (Border)cp.ContentTemplate.FindName("border", cp);
            border.Background = new SolidColorBrush(Colors.Blue);

            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
            TransferViewModel vm = this.DataContext as TransferViewModel;

            //Call command from viewmodel     
            if ((vm != null) && (vm.Add2Command.CanExecute(files))) 
            {
                vm.Add2Command.Execute(files);
                cbTransfer.IsChecked = true;
            }

        }

        private void border_DragLeave(object sender, DragEventArgs e)
        {
           

        }

        private void border_Drop(object sender, DragEventArgs e)
        {
            
        }

        private void btn_LogInStandart(object sender, EventArgs e)
        {
            //LoadingStandart.Visibility = Visibility.Visible;
            TextBox txtPassword = (TextBox)cp.ContentTemplate.FindName("txtPassword", cp);
            // Get the viewmodel from the DataContext
            TransferViewModel vm = this.DataContext as TransferViewModel;

            //Call command from viewmodel     
            if ((vm != null) && (vm.LoginCommand.CanExecute(txtPassword.Text)))
            {
                vm.LoginCommand.Execute(txtPassword.Text);
                cbLogin.IsChecked = vm.LoginDone;
            }
        }

        private void btn_LogOut(object sender, EventArgs e)
        {
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.LogOutCommand.CanExecute(null)))
            {
                vm.LogOutCommand.Execute(null);
                cbSuccess.IsChecked = false;
                cbProgress.IsChecked = false;
                cbTransfer.IsChecked = false;
                cbLogin.IsChecked = false;
            }              
        }

        private void btn_LogInSSO(object sender, EventArgs e)
        {     
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.LoginSSOCommand.CanExecute(null))) {
                vm.LoginSSOCommand.Execute(null);
                cbLogin.IsChecked = vm.LoginDone;
            }

        }

        private void MenuItem_Style_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItemLanguages = (MenuItem)cp.ContentTemplate.FindName("menuItemLanguages", cp);
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                item.IsChecked = false;
            }
            MenuItem mi = sender as MenuItem;
            mi.IsChecked = true;
            this.ChangeCulture(new CultureInfo(mi.Tag.ToString()));
        }

        public void ChangeCulture(CultureInfo cultureInfo)
        {
            this.removeTaskPane("LockSelf");

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            LockSelf.Properties.Resources.Culture = cultureInfo;

            TransferControlForm _usr = new TransferControlForm();

            // Create an ElementHost and create the WPF user control and assign to the ElementHost child
            ElementHost _eh = new ElementHost { Child = new TransferControlWpf() };

            // Add the ElementHost to the standard user control Controls
            _usr.Controls.Add(_eh);
            // Dock the ElementHost to fill
            _eh.Dock = System.Windows.Forms.DockStyle.Fill;

            // Connect the user control and the custom task pane 
            _myCustomTaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(_usr, "LockSelf");
            _myCustomTaskPane.Visible = true;
            _myCustomTaskPane.Width = 500;

        }

        private void btn_GobackTransfer(object sender, EventArgs e)
        {
            cbSuccess.IsChecked = false;
            cbProgress.IsChecked = false;
            cbTransfer.IsChecked = false;
            TransferViewModel vm = this.DataContext as TransferViewModel;
            //Call command from viewmodel     
            if ((vm != null) && (vm.DeleteAllCommand.CanExecute(null)))
            {
                vm.DeleteAllCommand.Execute(null);
                cbLogin.IsChecked = true;
            }
        }

        public void removeTaskPane(string title)
        {
            for (int i = Globals.ThisAddIn.CustomTaskPanes.Count; i > 0; i--)
            {
                _myCustomTaskPane = Globals.ThisAddIn.CustomTaskPanes[i - 1];
                if (_myCustomTaskPane.Title == title)
                {
                    Globals.ThisAddIn.CustomTaskPanes.RemoveAt(i - 1);
                }
            }
        }
    }
}
