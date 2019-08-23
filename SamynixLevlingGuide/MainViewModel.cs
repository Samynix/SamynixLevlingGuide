using GalaSoft.MvvmLight.Command;
using SamyLib.GuiWPF.Extension;
using SamyLib.GuiWPF.View;
using SamynixLevlingGuide.Model;
using SamynixLevlingGuide.Tags;
using SamynixLevlingGuide.View.Legend;
using SamynixLevlingGuide.View.StepView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SamynixLevlingGuide
{

    public class MainViewModel : SamyViewModelBase
    {
        static int? OnlyLoadStep = 2;

        public static MainViewModel Instance { get; }

        static MainViewModel()
        {
            Instance = new MainViewModel();
        }

        private MainViewModel()
        {
            TextBackgroundColor = Color.FromArgb(255, 68, 63, 58);
            AllViewsLoadedEvent += ApplicationLoaded;
        }

        public Color TextBackgroundColor
        {
            get => textBackgroundColor; set
            {
                textBackgroundColor = value;
                RaisePropertyChanged();
            }
        }

        private Dictionary<Step, Tuple<StepView, double>> _stepScrollBounds = new Dictionary<Step, Tuple<StepView, double>>();

        private MainWindow _mainWindow;
        private StackPanel _scrollViewerPanel;

        private LegendViewModel _legendViewModel;

        private RelayCommand _closePopupCommand;
        private RelayCommand<string> _searchCommand;
        private RelayCommand<bool> _searchPrevCommand;
        private RelayCommand<bool> _searchNextCommand;

        private Guide _selectedGuide;
        private ClassEnum _selectedClass;
        private StepListItem _selectedStep;

        private bool _isPopupImageOpen;
        private bool _isPopupQuestLogOpen;

        private bool _isScrollStepIntoView = true;

        private StepView _lastStepView;
        private RelayCommand<string> _copyQuestTitleCommand;

        public override Brush InfoMessageBackground { get => Brushes.Transparent; }

        protected override void ViewInitialized(FrameworkElement aView)
        {
            _mainWindow = aView as MainWindow;
            _legendViewModel = _mainWindow.LegendView.ViewModel;
        }


        protected override void ViewLoaded(FrameworkElement aView)
        {
            AddLegends();
            LoadGuides();
            LoadConfig();
        }

        private void AddLegends()
        {
            foreach (var textColors in TextColors.ColorsAndDescriptions)
            {
                _legendViewModel.LegendItemsSource.Add(new LegendListItem(textColors.Key, textColors.Value));
            }
        }

        private void LoadGuides()
        {
            var guides = new List<Guide>();
            foreach (var directory in Directory.EnumerateDirectories(Path.Combine(Environment.CurrentDirectory, "Guide")))
            {
                var guide = Guide.Parse(directory, OnlyLoadStep);
                if (guide.IsValid)
                {
                    guides.Add(guide);
                }
            }

            GuideItemsSource = guides;
            RaisePropertyChanged(nameof(GuideItemsSource));
        }

        public List<Guide> GuideItemsSource { get; private set; }

        public Guide SelectedGuide
        {
            get => _selectedGuide; set
            {
                _selectedGuide = value;
                RaisePropertyChanged();

                if (_selectedGuide != null)
                {
                    AddClassFilters(_selectedGuide);
                    PopulateStepItemsSource(_selectedGuide);

                    GuideConfig.Instance.LastUsedGuide = _selectedGuide?.Title;
                }
            }
        }

        private void AddClassFilters(Guide aGuide)
        {
            foreach (var classEnum in Enum.GetValues(typeof(ClassEnum)).Cast<ClassEnum>().Where(ce => ce != ClassEnum.All && aGuide.Classes.HasFlag(ce)))
            {
                ClassItemsSource.Add(classEnum);
            }
        }

        public ObservableCollection<ClassEnum> ClassItemsSource { get; } = new ObservableCollection<ClassEnum>();

        private void PopulateStepItemsSource(Guide aGuide)
        {
            foreach (var step in aGuide.Steps)
            {
                var stepView = new StepView();
                stepView.ViewModel.SetStep(step);
                stepView.Margin = new Thickness(0, 0, 0, 20);

                StepItemsSource.Add(stepView);
                SelectStepItemsSource.Add(new StepListItem(stepView));

                _lastStepView = stepView;
            }
        }

        public ObservableCollection<StepView> StepItemsSource { get; } = new ObservableCollection<StepView>();
        public ObservableCollection<StepListItem> SelectStepItemsSource { get; } = new ObservableCollection<StepListItem>();


        public ClassEnum SelectedClass
        {
            get => _selectedClass; set
            {
                if (_selectedClass == value)
                {
                    return;
                }

                _selectedClass = value;
                RaisePropertyChanged();

                GuideConfig.Instance.LastUsedClass = _selectedClass;
                SelectedClassChanged(value);
            }
        }

        public StepListItem SelectedStep
        {
            get => _selectedStep; set
            {
                if (_selectedStep == value)
                {
                    return;
                }

                _selectedStep = value;
                RaisePropertyChanged();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentImageSource));

                GuideConfig.Instance.LastUsedStep = _selectedStep?.Title;
                if (_isScrollStepIntoView && _selectedStep != null)
                {
                    SrollSelectedStepIntoView();
                }
            }
        }

        private void SrollSelectedStepIntoView()
        {
            var stepBounds = _stepScrollBounds.FirstOrDefault(scb => scb.Value.Item1 == _selectedStep.StepView).Value;
            if (stepBounds != null)
            {
                _mainWindow.StepSrollViewer.ScrollToVerticalOffset(stepBounds.Item2);
            }
            else
            {
                _mainWindow.StepSrollViewer.ScrollToTop();
                _selectedStep.StepView.BringIntoView();
            }

        }

        private void SelectedClassChanged(ClassEnum aClass)
        {
            Dictionary<string, Dictionary<string, Quest>> currentQuestLog = new Dictionary<string, Dictionary<string, Quest>>();
            foreach (var stepView in StepItemsSource)
            {
                foreach (var subStep in stepView.ViewModel.SubStepItemsSource.Where(st => st.ViewModel.SubStep.HasClass(aClass)))
                {
                    foreach (var quest in subStep.ViewModel.Quests)
                    {

                        if (!currentQuestLog.ContainsKey(quest.Category))
                        {
                            currentQuestLog[quest.Category] = new Dictionary<string, Quest>();
                        }

                        if (quest.HasBeenDelivered || quest.HasBeenMarkedForSkip)
                        {
                            currentQuestLog[quest.Category].Remove(quest.QuestKey);
                            if (!currentQuestLog[quest.Category].Any())
                            {
                                currentQuestLog.Remove(quest.Category);
                            }

                            quest.HasBeenMarkedForSkip = false;
                        }
                        else
                        {
                            currentQuestLog[quest.Category][quest.QuestKey] = quest;
                        }

                        if (currentQuestLog.Sum(c => c.Value.Count()) > 20)
                        {

                        }
                    }

                    subStep.ViewModel.QuestLog = currentQuestLog.OrderBy(vp => vp.Key).Select(vp =>
                    new QuestCategory
                    {
                        Category = vp.Key,
                        Quests = vp.Value.OrderBy(vp2 => vp2.Value.QuestLevel).ThenBy(vp2 => vp2.Value.Title).Select(vp2 => vp2.Value).ToList()
                    }).ToList();
                }

                stepView.ViewModel.SetClass(aClass);
            }

            RaisePropertyChanged(nameof(CurrentImageSource));
        }


        public string CurrentImageSource => SelectedStep?.StepView.ViewModel.StepImageSource;

        private void LoadConfig()
        {
            if (!string.IsNullOrEmpty(GuideConfig.Instance.LastUsedGuide))
            {
                SelectedGuide = GuideItemsSource.FirstOrDefault(g => g.Title == GuideConfig.Instance.LastUsedGuide);
            }
            else
            {
                SelectedGuide = GuideItemsSource.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(GuideConfig.Instance.LastUsedStep))
            {
                var lastUsedStep = SelectStepItemsSource.FirstOrDefault(s => s.Title == GuideConfig.Instance.LastUsedStep);
                if (lastUsedStep != null)
                {
                    SelectedStep = lastUsedStep;
                }
            }
            else
            {
                SelectedStep = SelectStepItemsSource.FirstOrDefault();
            }

            if (ClassItemsSource.Contains(GuideConfig.Instance.LastUsedClass))
            {
                SelectedClass = GuideConfig.Instance.LastUsedClass;
            }
            else
            {
                SelectedClass = ClassItemsSource.FirstOrDefault();
            }

            _mainWindow.WindowState = GuideConfig.Instance.LastUsedWindowState;
            _mainWindow.StepSrollViewer.ScrollToVerticalOffset(GuideConfig.Instance.VerticalScrollOffset);

            foreach (GuideConfig.CheckedSubStep checkedSubStep in GuideConfig.Instance.CheckedSubSteps)
            {
                foreach (var subStep in GuideItemsSource.SelectMany(g => g.Steps.SelectMany(s => s.SubSteps)).Where(st => st.Key == checkedSubStep.Key))
                {
                    subStep.SetIsDone(true);
                }
            }
        }

        private void ApplicationLoaded()
        {
            UpdateLastStepViewMinHeight();

            var scrollViewer = _mainWindow.StepSrollViewer;
            if (_scrollViewerPanel == null)
            {
                _scrollViewerPanel = scrollViewer.GetFirstChildOfType<StackPanel>();
                _scrollViewerPanel.SizeChanged += (a, b) => { UpdateScrollStepBounds(); };
                UpdateScrollStepBounds();
            }

            scrollViewer.SizeChanged += StepSrollViewer_SizeChanged;
            scrollViewer.ScrollChanged += StepSrollViewer_ScrollChanged;
        }

        private void StepSrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (SelectedStep == null)
            {
                return;
            }

            var scrollViewer = (ScrollViewer)sender;
            ScrollChanged(e.VerticalOffset, e.VerticalChange, scrollViewer.ActualHeight);
        }

        private void UpdateScrollStepBounds()
        {
            if (_scrollViewerPanel != null)
            {
                var heightThreshold = _mainWindow.StepSrollViewer.ActualHeight / 6; ;
                _stepScrollBounds.Clear();

                foreach (var stepView in StepItemsSource)
                {
                    var point = stepView.TransformToAncestor(_scrollViewerPanel).Transform(new Point(0, 0));
                    var minValue = point.Y - 20 > 0 ? point.Y - 20 : 0;
                    _stepScrollBounds[stepView.ViewModel.Step] = new Tuple<StepView, double>(stepView, point.Y);
                }
            }
        }

        private void ScrollChanged(double aVerticalOffset, double aScrollChange, double aVisibleHeight)
        {
            StackTrace stackTrace = new StackTrace();
            var stackOverFlow = stackTrace.GetFrames().Skip(1).FirstOrDefault(f => f.GetMethod()?.Name == nameof(ScrollChanged));
            if (stackOverFlow != null)
            {
                return;
            }

            double bottomOfView = aVerticalOffset + aVisibleHeight;
            double topOfView = aVerticalOffset;

            StepView newStepView = null;
            bool isSroll = false;
            if (aScrollChange > 0 && SelectedStep.StepView.ViewModel.NextStep != null)
            {
                isSroll = true;
                var stepAndBounds = _stepScrollBounds[SelectedStep.StepView.ViewModel.NextStep];
                if (topOfView >= stepAndBounds.Item2)
                {
                    newStepView = stepAndBounds.Item1;
                }
            }
            else if (aScrollChange < 0 && SelectedStep.StepView.ViewModel.PreviousStep != null)
            {
                var stepAndBounds = _stepScrollBounds[SelectedStep.StepView.ViewModel.PreviousStep];
                if (topOfView <= stepAndBounds.Item2 || bottomOfView < _stepScrollBounds[SelectedStep.StepView.ViewModel.Step].Item2)
                {
                    newStepView = stepAndBounds.Item1;
                }
            }

            if (newStepView != null)
            {
                var newSelectedStepItem = SelectStepItemsSource.FirstOrDefault(s => s.StepView == newStepView);
                if (newSelectedStepItem == SelectedStep)
                {
                    return;
                }

                _isScrollStepIntoView = isSroll;
                SelectedStep = newSelectedStepItem;
                _isScrollStepIntoView = true;
            }
        }

        private void StepSrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLastStepViewMinHeight();
        }

        private void UpdateLastStepViewMinHeight()
        {
            if (_lastStepView != null)
            {
                _lastStepView.MinHeight = _mainWindow.StepSrollViewer.ActualHeight;
            }
        }

        public void ShowImage(string aUrl)
        {
            PopupImageSource = aUrl;
            IsPopupImageOpen = true;

            RaisePropertyChanged(nameof(PopupImageSource));
        }

        public override void SizeChanged(Size aSize)
        {
            PopupWidth = aSize.Width * 0.85;
            PopupHeight = aSize.Height * 0.85;

            RaisePropertyChanged(nameof(PopupWidth));
            RaisePropertyChanged(nameof(PopupHeight));
        }

        public double PopupWidth { get; set; }
        public double PopupHeight { get; set; }

        public bool IsPopupImageOpen
        {
            get => _isPopupImageOpen;
            set
            {
                _isPopupImageOpen = value;
                RaisePropertyChanged();
            }
        }

        public string PopupImageSource { get; set; }

        public void ShowQuestLog(List<QuestCategory> aQuestLog)
        {
            QuestLogPopupItemsSource = aQuestLog;
            IsPopupQuestLogOpen = true;

            RaisePropertyChanged(nameof(QuestLogPopupItemsSource));
        }

        public bool IsPopupQuestLogOpen
        {
            get => _isPopupQuestLogOpen; set
            {
                _isPopupQuestLogOpen = value;
                RaisePropertyChanged();
            }
        }
        public List<QuestCategory> QuestLogPopupItemsSource { get; private set; } = new List<QuestCategory>();


        public RelayCommand<string> CopyQuestTitleCommand
        {
            get
            {
                _copyQuestTitleCommand = _copyQuestTitleCommand ?? new RelayCommand<string>((title) =>
                {
                    Clipboard.SetText(title);
                    MainViewModel.Instance.ShowInfoMessage(title + " copied to clipboard");
                });

                return _copyQuestTitleCommand;
            }
        }

        public RelayCommand ClosePopupCommand
        {
            get
            {
                _closePopupCommand = _closePopupCommand ?? new RelayCommand(() =>
                {
                    IsPopupImageOpen = false;
                    IsPopupQuestLogOpen = false;
                });

                return _closePopupCommand;
            }
        }

        public RelayCommand<string> SearchCommand
        {
            get
            {
                _searchCommand = _searchCommand ?? new RelayCommand<string>((searchString) =>
                {
                    Search(searchString, false, false, false);
                });

                return _searchCommand;
            }
        }

        public RelayCommand<bool> SearchPreviousCommand
        {
            get
            {
                _searchPrevCommand = _searchPrevCommand ?? new RelayCommand<bool>((isWrapAround) =>
                {
                    Search(string.Empty, true, false, isWrapAround);
                });

                return _searchPrevCommand;
            }
        }

        public RelayCommand<bool> SearchNextCommand
        {
            get
            {
                _searchNextCommand = _searchNextCommand ?? new RelayCommand<bool>((isWrapAround) =>
                {
                    Search(string.Empty, false, true, isWrapAround);
                });

                return _searchNextCommand;
            }
        }


        private List<SubStepView> _searchResults = new List<SubStepView>();
        private int? _currentSearchIndex = null;
        private Color textBackgroundColor;

        private void Search(string aSearchString, bool isSearchPrevious, bool isSearchNext, bool isWrapAround)
        {
            if ((!isSearchNext && !isSearchPrevious) || !_searchResults.Any())
            {
                _searchResults.Clear();
                _currentSearchIndex = null;

                foreach (var view in StepItemsSource.SelectMany(s => s.ViewModel.SubStepItemsSource.Where(st => st.ViewModel.Visibility == Visibility.Visible)))
                {
                    view.ViewModel.ResetSearch();
                    bool isFirstSearch = true;
                    while (view.ViewModel.Search(aSearchString, !isFirstSearch))
                    {
                        isFirstSearch = false;
                        if (!_searchResults.Contains(view))
                        {
                            _searchResults.Add(view);
                        }
                    }

                }
            }

            if (!_searchResults.Any())
            {
                return;
            }

            if (isSearchNext)
            {
                _currentSearchIndex = _currentSearchIndex.HasValue ? _currentSearchIndex : -1;
                if (isWrapAround && _searchResults.Count - 1 < _currentSearchIndex)
                {
                    _currentSearchIndex = -1;
                }

                _currentSearchIndex++;
                if (_searchResults.Count - 1 < _currentSearchIndex)
                {
                    return;
                }

                _searchResults[_currentSearchIndex.Value].BringIntoView();
            }
            else
            {
                _currentSearchIndex = _currentSearchIndex.HasValue ? _currentSearchIndex : _searchResults.Count;
                if (isWrapAround && _currentSearchIndex - 1 < 0)
                {
                    _currentSearchIndex = _searchResults.Count;
                }

                _currentSearchIndex--;
                if (_currentSearchIndex < 0)
                {
                    return;
                }

                _searchResults[_currentSearchIndex.Value].BringIntoView();
            }
        }



        protected override void ViewClosing()
        {
            GuideConfig.Instance.LastUsedWindowState = _mainWindow.WindowState;
            GuideConfig.Instance.VerticalScrollOffset = (int)_mainWindow.StepSrollViewer.VerticalOffset;

            GuideConfig.Instance.CheckedSubSteps.Clear();
            foreach (var subStep in GuideItemsSource.SelectMany(g => g.Steps.SelectMany(s => s.SubSteps)).Where(st => st.IsDone))
            {
                GuideConfig.Instance.CheckedSubSteps.Add(subStep.Key);
            }

            GuideConfig.Instance.Save();
        }
    }
}
