using GalaSoft.MvvmLight;
using SamyLib.GuiWPF.View;
using SamynixLevlingGuide.Model;
using SamynixLevlingGuide.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SamynixLevlingGuide.View.StepView
{
    public class SubStepViewModel : SamyViewModelBase
    {
        public static Action<string, bool> SubStepDoneChanged;

        private SubStepView _subStepView;
        private Brush _legendBrush;
        private Visibility _visibility;


        protected override void ViewInitialized(FrameworkElement aView)
        {
            _subStepView = aView as SubStepView;
        }

        protected override void ViewLoaded(FrameworkElement aView)
        {

        }

        internal void SetSubStep(SubStep aSubStep)
        {
            SubStep = aSubStep;
            _subStepView.StepTextBox.SetText(aSubStep.StepDirectory, aSubStep.StepOrder, aSubStep.StepText, aSubStep.Legend);
            _subStepView.StepTextBox.QuestLogButtonClicked += () =>
            {
                MainViewModel.Instance.ShowQuestLog(QuestLog);
            };

            SubStep.IsDoneChanged += (isDone) =>
            {
                SetIsDone(isDone, false);
            };
        }

        internal void Update(int aSubstepNumber, bool isAddLineSpacer)
        {
            _subStepView.StepTextBox.Update(aSubstepNumber, QuestLog.Sum(a => a.Quests.Count()), isAddLineSpacer);
        }

        internal bool Search(string aSearchString, bool isSearchNext)
        {
            if (string.IsNullOrEmpty(aSearchString))
            {
                return false;
            }

            return _subStepView.StepTextBox.DoSearch(aSearchString, isSearchNext);
        }

        internal void ResetSearch()
        {
            _subStepView.StepTextBox.ResetSearch();
        }

        public SubStep SubStep { get; private set; }

        public List<Quest> Quests => _subStepView.StepTextBox.Quests;

        public List<QuestCategory> QuestLog { get; set; } = new List<QuestCategory>();

        public Brush LegendBrush
        {
            get => _legendBrush;
            set
            {
                _legendBrush = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LegendVisibility));
            }
        }

        public Visibility Visibility
        {
            get => _visibility; set
            {
                _visibility = value;
                RaisePropertyChanged();
            }
        }

        public Visibility LegendVisibility => LegendBrush != null ? Visibility.Visible : Visibility.Collapsed;

        public void SetIsDone(bool isDone, bool isInvokeChanged = true)
        {
            if (isInvokeChanged)
            {
                SubStepDoneChanged?.Invoke(SubStep.Key, isDone);
            }

            SubStep.IsDone = isDone;
            _subStepView.CheckBoxIsDone.IsChecked = isDone;
        }

        protected override void ViewClosing()
        {

        }


    }
}
