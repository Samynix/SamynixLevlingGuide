using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SamynixLevlingGuide.View.Legend
{
    public class LegendListItem
    {
        public LegendListItem(Color aColor, string aDescription)
        {
            Brush = new SolidColorBrush(aColor);
            Description = aDescription;
        }

        public SolidColorBrush Brush { get; private set; }
        public string Description { get; private set; }
    }
}
