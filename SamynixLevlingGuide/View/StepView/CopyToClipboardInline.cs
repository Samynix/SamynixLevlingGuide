using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SamynixLevlingGuide.View.StepView
{
    public class CopyToClipboardInline : ClickableInlineBase
    {
        private readonly string _copyToClipboardText;

        public CopyToClipboardInline(SubStepTextBox aParent, string aText, string aCopyToCLipboardText) : base(aParent, new Underline(new Run(aText)))
        {
            _copyToClipboardText = aCopyToCLipboardText;
        }

        protected override void InlineClicked(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(_copyToClipboardText);
            MainViewModel.Instance.ShowInfoMessage(_copyToClipboardText + " copied to clipboard");
        }
    }
}
