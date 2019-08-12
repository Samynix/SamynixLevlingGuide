using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamynixLevlingGuide.View.StepView
{
    public class StepListItem
    {
        public string Title => StepView.ViewModel.Title;

        public StepView StepView { get; }

        public StepListItem(StepView aStepView)
        {
            StepView = aStepView;
        }

    }
}
