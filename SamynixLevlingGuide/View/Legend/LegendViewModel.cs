using SamyLib.GuiWPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamynixLevlingGuide.View.Legend
{
    public class LegendViewModel : SamyViewModelBase
    {
        protected override void ViewInitialized(FrameworkElement aView)
        {
            
        }

        public ObservableCollection<LegendListItem> LegendItemsSource { get; } = new ObservableCollection<LegendListItem>();

        protected override void ViewLoaded(FrameworkElement aView)
        {
           
        }

        protected override void ViewClosing()
        {
            
        }
    }
}
