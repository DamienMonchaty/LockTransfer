using Core.Services;
using Core.Services.Impl;
using LockSelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LockSelf.Controls
{
    /// <summary>
    /// Logique d'interaction pour PopUpControl.xaml
    /// </summary>
    public partial class PopUpControl : UserControl
    {
        public PopUpControl()
        {
            InitializeComponent();
           

        }

        private void Btn_Visibility_Click(object sender, EventArgs e)
        {
            TransferViewModel vm = this.DataContext as TransferViewModel;
            var popup = FindParent<PopUpControl>(sender as Button);
            if (popup != null)
                vm.ShowPopUp = false;
        }

        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = LogicalTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }
    }
}
