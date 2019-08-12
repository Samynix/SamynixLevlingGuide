using SamyLib.GuiWPF.View;
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

namespace SamynixLevlingGuide.View.StepView
{
    /// <summary>
    /// Interaction logic for SubStepView.xaml
    /// </summary>
    public partial class SubStepView : SamyViewBase
    {
        public SubStepView()
        {
            InitializeComponent();

            ViewModel = (SubStepViewModel)DataContext;
        }

        public SubStepViewModel ViewModel { get; }


        private void Preview_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CheckBoxIsDone.IsChecked = !CheckBoxIsDone.IsChecked;
            ViewModel.SetIsDone(CheckBoxIsDone.IsChecked.GetValueOrDefault());
        }

        private void CheckBoxIsDone_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
