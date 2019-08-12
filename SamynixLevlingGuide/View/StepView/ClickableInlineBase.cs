using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SamynixLevlingGuide.View.StepView
{
    public abstract class ClickableInlineBase : Span
    {
        private SubStepTextBox _parent;
        private SubStepView _subStepView;

        protected Inline Child { get; }

        public ClickableInlineBase(SubStepTextBox aParent, Inline aChild) : base(aChild)
        {
            _parent = aParent;
            Child = aChild;
            Loaded += ClickableInlineBase_Loaded;
        }


        private void ClickableInlineBase_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Cursor = Cursors.Hand;
            base.PreviewMouseDown += ClickableInlineBase_PreviewMouseDown;
            _subStepView = FindSubStepParent(_parent);
        }

        private SubStepView FindSubStepParent(DependencyObject aDependencyObject)
        {
            if (aDependencyObject is SubStepView)
            {
                return (SubStepView)aDependencyObject;
            }

            return FindSubStepParent(VisualTreeHelper.GetParent(aDependencyObject));
        }

        private void ClickableInlineBase_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InlineClicked(sender, e);
            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            _subStepView.CheckBoxIsDone.IsChecked = !_subStepView.CheckBoxIsDone.IsChecked;//Mega hax
        }

        protected abstract void InlineClicked(object sender, MouseButtonEventArgs e);


    }
}
