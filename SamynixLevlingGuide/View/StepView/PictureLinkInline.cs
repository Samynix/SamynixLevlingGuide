using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace SamynixLevlingGuide.View.StepView
{
    public class PictureLinkInline : ClickableInlineBase
    {
        private readonly string _url;

        public PictureLinkInline(SubStepTextBox aParent, string aUrl, string aText) : base(aParent, new Underline(new Run(aText)))
        {
            _url = aUrl;
        }

        protected override void InlineClicked(object sender, MouseButtonEventArgs e)
        {
            MainViewModel.Instance.ShowImage(_url);
        }
    }
}
