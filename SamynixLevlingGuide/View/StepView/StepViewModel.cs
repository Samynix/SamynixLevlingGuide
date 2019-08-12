using GalaSoft.MvvmLight;
using SamynixLevlingGuide.Model;
using SamynixLevlingGuide.Tags;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SamynixLevlingGuide.View.StepView
{
    public class StepViewModel : ViewModelBase
    {
        private List<SubStepViewModel> _subSteps = new List<SubStepViewModel>();

        private ClassEnum _selectedClass;

        public StepViewModel()
        {

        }

        public string Title => Step?.Title;
        public string StepImageSource
        {
            get
            {
                var imageSource = Step.ImageSources.FirstOrDefault(vp => vp.Key.HasFlag(_selectedClass));
                return imageSource.Value;
            }
        }

        public Step PreviousStep => Step.PreviousStep;
        public Step Step { get; private set; }
        public Step NextStep => Step.NextStep;

        public ObservableCollection<SubStepView> SubStepItemsSource { get; private set; } = new ObservableCollection<SubStepView>();

        internal void SetStep(Step aStep)
        {
            Step = aStep;

            foreach (var subStep in Step.SubSteps)
            {
                AddStep(subStep);
            }

            RaisePropertyChanged(nameof(Title));
        }
       
        private SubStepView AddStep(SubStep aSubStep)
        {
            var subStepView = new SubStepView();
            subStepView.ViewModel.SetSubStep(aSubStep);
            _subSteps.Add(subStepView.ViewModel);
            SubStepItemsSource.Add(subStepView);

            return subStepView;
        }

        internal void SetClass(ClassEnum aClassEnum)
        {
            _selectedClass = aClassEnum;
            RaisePropertyChanged(nameof(StepImageSource));

            int stepNumber = 1;
            foreach (var subStepViewModel in SubStepItemsSource.Select(stv => stv.ViewModel))
            {
                if (subStepViewModel.SubStep.HasClass(aClassEnum))
                {
                    subStepViewModel.Update(stepNumber++, HasMoreVisibleSubSteps(subStepViewModel.SubStep, aClassEnum));
                    subStepViewModel.Visibility = Visibility.Visible;
                }
                else
                {
                    subStepViewModel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool HasMoreVisibleSubSteps(SubStep aSubStep, ClassEnum aClassEnum)
        {
            if (aSubStep.NextSubSteps.Any(ss => ss.HasClass(aClassEnum)))
            {
                return true;
            }

            foreach (var subStep in aSubStep.NextSubSteps)
            {
                return HasMoreVisibleSubSteps(subStep, aClassEnum);
            }

            return false;
        }





    }
}
