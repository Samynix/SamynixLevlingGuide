using SamyLib.GuiWPF.Extension;
using SamyLib.GuiWPF.View;
using SamynixLevlingGuide.View;
using SamynixLevlingGuide.View.StepView;
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

namespace SamynixLevlingGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SamyWindowBase
    {
        private StepView _previousVisibleStepView;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override SamyViewModelBase DataContextAtInitialized => MainViewModel.Instance;

        protected override void ViewLoadedInternal()
        {
        
        
        }

    }
}
