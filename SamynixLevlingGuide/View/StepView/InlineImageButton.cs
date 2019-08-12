using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SamynixLevlingGuide.View.StepView
{
    public class InlineImageButton : Image
    {
        private SubStepTextBox _parent;
        private SubStepView _subStepView;


        public InlineImageButton(SubStepTextBox aParent, string aImageSource) 
        {
            Source = new BitmapImage(new Uri(aImageSource));

      

            _parent = aParent;
            Loaded += SubStepButton_Loaded;

            aParent.PreviewMouseDown += AParent_PreviewMouseDown;
            aParent.PreviewMouseMove += AParent_PreviewMouseMove;
        }

        private void AParent_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (VisualTreeHelper.HitTest(this, e.GetPosition(this))?.VisualHit == this)
            {
                Mouse.SetCursor(Cursor);
            }
            else
            {
                
                base.OnMouseMove(e);
            }
        }


        private void AParent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (VisualTreeHelper.HitTest(this, e.GetPosition(this))?.VisualHit == this)
            {
                ButtonClicked?.Invoke(e);
                if (e.Handled)
                {
                    return;
                }

                e.Handled = true;
                _subStepView.CheckBoxIsDone.IsChecked = !_subStepView.CheckBoxIsDone.IsChecked;//Mega hax
            }
        }


        private void SubStepButton_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Cursor = Cursors.Hand;
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

        public Action<MouseButtonEventArgs> ButtonClicked;


    }
}
