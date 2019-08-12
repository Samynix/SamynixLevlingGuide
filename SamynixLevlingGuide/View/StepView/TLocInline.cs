using SamyLib.GuiWPF.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SamynixLevlingGuide.View.StepView
{
    public class TLocInline : ClickableInlineBase
    {
        private readonly Point _point;

        public TLocInline(SubStepTextBox aParent, Point aPoint, string aText) : base(aParent, new Underline(new Run(aText)))
        {
            _point = aPoint;
        }

        protected override void InlineClicked(object sender, MouseButtonEventArgs e)
        {
            var textToClipboard = $"/tloc {_point.X.ToString().Replace(',', '.')}, {_point.Y.ToString().Replace(',', '.')}";
            Clipboard.SetText(textToClipboard);
            MainViewModel.Instance.ShowInfoMessage(textToClipboard + " copied to clipboard");
        }
    }
}
