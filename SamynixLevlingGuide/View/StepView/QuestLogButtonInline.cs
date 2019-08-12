using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SamynixLevlingGuide.View.StepView
{
    public class QuestLogButtonInline : ClickableInlineBase
    {
        private Run _textRun;

        //public QuestLogButtonInline(SubStepTextBox aParent) : base(aParent, new InlineUIContainer(new Border
        //{
        //    Width = 30,
        //    Height = 30,
        //    Background = Brushes.Orange,
        //    CornerRadius= new System.Windows.CornerRadius(10),
        //    BorderBrush = Brushes.Black,
        //    Child = new TextBlock(),

        //}))
        //{

        //}

        public QuestLogButtonInline(SubStepTextBox aParent) : base(aParent, new Underline(new Run()) {
            Background = Brushes.Orange, 
            
        })
        {

        }

        public void SetText(string aText)
        {
            if (_textRun == null)
            {
                _textRun = ((Underline)Child).Inlines.First() as Run;
            }

            _textRun.Text = $"Quests ({aText})";
        }

        protected override void InlineClicked(object sender, MouseButtonEventArgs e)
        {
            ButtonClicked?.Invoke(e);
        }

        public Action<MouseButtonEventArgs> ButtonClicked;
    }
}
