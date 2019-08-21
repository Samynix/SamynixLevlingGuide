
using SamynixLevlingGuide.Model;
using SamynixLevlingGuide.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Utilities.StringUtil;
using static SamynixLevlingGuide.Tags.SimpleTags;

namespace SamynixLevlingGuide.View.StepView
{
    public class SubStepTextBox : RichTextBox
    {
        private Paragraph _paragraph;

        private InlineUIContainer _lineSpacerContainer;
        private InlineImageButton _questLogButtonContainer;
        private QuestLogButtonInline _questLogButton;
        //private InlineImageButton _questLogButtonContainer;
        private Line _lineSpacer;
        private Run _stepNumberRun;
        private Span _stepHeaderSpan;
        private string _stepDirectory;
        private const string SubStepNumberStringFormat = "{0}) ";

        public Action QuestLogButtonClicked;

        private List<Tuple<TextRange, Brush>> _foundSearches = new List<Tuple<TextRange, Brush>>();
        private TextPointer _startCarretPosition;

        public SubStepTextBox()
        {
            BorderThickness = new System.Windows.Thickness(0, 0, 0, 0);

            SizeChanged += (a, b) =>
            {
                if (_lineSpacer != null)
                {
                    _lineSpacer.X2 = ActualWidth;
                }
            };

            FlowDocument doc = new FlowDocument();
            
            _paragraph = new Paragraph();
            _paragraph.Margin = new System.Windows.Thickness(0, 0, 0, 0);

            
            _stepHeaderSpan = new Span();
            _stepNumberRun = new Run(string.Format(SubStepNumberStringFormat, "00"));
            _stepHeaderSpan.Inlines.Add(_stepNumberRun);
            _paragraph.Inlines.Add(_stepHeaderSpan);
            _paragraph.Inlines.Add(new LineBreak());
            doc.Blocks.Add(_paragraph);
            Document = doc;

            _lineSpacer = new Line
            {
                X1 = 0,
                X2 = ActualWidth,
                Y1 = 5,
                Y2 = 5,
            };

            _lineSpacer.StrokeThickness = 1;
            _lineSpacer.Margin = new System.Windows.Thickness(0, 5, 0, 5);
            _lineSpacer.Stroke = Brushes.LightGray;
            _lineSpacerContainer = new InlineUIContainer(_lineSpacer);

            _questLogButton = new QuestLogButtonInline(this);
            _questLogButton.ButtonClicked += (e) =>
            {
                QuestLogButtonClicked?.Invoke();
            };


            _questLogButtonContainer = new InlineImageButton(this, System.IO.Path.Combine(Environment.CurrentDirectory, "Guide", "questLogButton.png"));
            _questLogButtonContainer.Width = 30;
            _questLogButtonContainer.Height = 30;
            _questLogButtonContainer.Margin = new Thickness(5, 0, 5, 0);
            _questLogButtonContainer.RenderTransform = new TranslateTransform(0, 3);
            _questLogButtonContainer.ButtonClicked += (e) =>
            {
                QuestLogButtonClicked?.Invoke();
            };
        }


        public List<Quest> Quests { get; } = new List<Quest>();

        public void SetText(string aStepDirectory, int aSubStepNumber, string aText, Color? aLegend)
        {
            _stepDirectory = aStepDirectory;

            bool isQuestChanges = false;
            AddText(aText, ref isQuestChanges);
            if (isQuestChanges)
            {
                _paragraph.Inlines.Add("  ");
                _paragraph.Inlines.Add(_questLogButton);
            }

            _startCarretPosition = CaretPosition;

            if (aLegend.HasValue)
            {
                _stepHeaderSpan.Inlines.Add(CreateLegendRectangle(aLegend.Value));
            }
        }

        private void AddText(string aText, ref bool isQuestChanges, Span aCurrentSpan = null, string aCurrentText = null, bool isUnderline = false)
        {
            var textLines = aText.Split(Environment.NewLine.ToCharArray());
            if (aCurrentSpan == null)
            {
                aCurrentSpan = new Span();
                _paragraph.Inlines.Add(aCurrentSpan);
            }

            if (aCurrentText == null)
            {
                aCurrentText = string.Empty;
            }

            string[] realLines = textLines.Where(t => !t.Trim().StartsWith("#") && !string.IsNullOrEmpty(t.Trim())).ToArray();
            for (int i = 0; i < realLines.Length; i++)
            {
                string line = realLines[i];
                for (int charInde = 0; charInde < line.Length; charInde++)
                {
                    if (line[charInde] == '\t')
                    {
                        aCurrentText += "      ";
                        continue;
                    }

                    if (TagParser.ParseTag(line, ref charInde, out var tag, out var tagContent, out var attributes))
                    {
                        //Regular text
                        if (!string.IsNullOrEmpty(aCurrentText))
                        {
                            aCurrentSpan.Inlines.Add(GetText(aCurrentText, isUnderline));
                            aCurrentText = string.Empty;
                        }

                        if (SimpleTags.IsTag(SimpleTags.Tag.Legend, tag))
                        {
                            var legendColor = SimpleTags.GetContent<Color>(tagContent);
                            aCurrentSpan.Inlines.Add(CreateLegendRectangle(legendColor));
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.TLoc, tag))
                        {
                            var tloc = SimpleTags.GetContent<Point>(tagContent);
                            if (double.IsNaN(tloc.X) || double.IsNaN(tloc.Y))
                            {
                                aCurrentSpan.Inlines.Add(GetText("(error)", isUnderline));
                            }
                            else
                            {
                                var tlocRun = new TLocInline(this, tloc, tagContent);
                                aCurrentSpan.Inlines.Add(tlocRun);
                            }
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.Bold, tag))
                        {
                            var boldSpan = new Span();
                            boldSpan.FontWeight = FontWeights.Bold;
                            aCurrentSpan.Inlines.Add(boldSpan);

                            AddText(tagContent, ref isQuestChanges, aCurrentSpan: boldSpan, aCurrentText: aCurrentText, isUnderline: isUnderline);
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.Place, tag)) //Maybe do something else for this, same as bold atm
                        {
                            var boldSpan = new Span();
                            boldSpan.FontWeight = FontWeights.Bold;
                            aCurrentSpan.Inlines.Add(boldSpan);

                            AddText(tagContent, ref isQuestChanges, aCurrentSpan: boldSpan, aCurrentText: aCurrentText, isUnderline: isUnderline);
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.Italic, tag))
                        {
                            var italicSpan = new Span();
                            italicSpan.FontStyle = FontStyles.Italic;
                            aCurrentSpan.Inlines.Add(italicSpan);

                            AddText(tagContent, ref isQuestChanges, aCurrentSpan: italicSpan, aCurrentText: aCurrentText, isUnderline: isUnderline);
                        }
                        else if(SimpleTags.IsTag(SimpleTags.Tag.Underline, tag))
                        {
                            AddText(tagContent, ref isQuestChanges, aCurrentSpan: aCurrentSpan, aCurrentText: aCurrentText, isUnderline: true);
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.DONTFORGET, tag))
                        {
                            var empesizedSpan = new Span();
                            empesizedSpan.FontStyle = FontStyles.Italic;
                            empesizedSpan.FontWeight = FontWeights.ExtraBold;
                            empesizedSpan.FontSize = aCurrentSpan.FontSize + 5;
                            aCurrentSpan.Inlines.Add(empesizedSpan);
                            AddText(tagContent, ref isQuestChanges, aCurrentSpan: empesizedSpan, aCurrentText: aCurrentText, isUnderline: isUnderline);
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.Picture, tag))
                        {
                            string url = string.Empty;
                            attributes.TryGetValue("url", out url); { //TODO make hard reference
                                url = System.IO.Path.Combine(_stepDirectory, url);
                                aCurrentSpan.Inlines.Add(new PictureLinkInline(this, url, tagContent));
                            }
                        }
                        else if (SimpleTags.IsTag(SimpleTags.Tag.LineBreak, tag))
                        {
                            //aCurrentSpan.Inlines.Add(new LineBreak());
                        }
                        else
                        {
                            var tagType = SimpleTags.GetTagFromString(tag);
                            var colorSpan = new Span();
                            colorSpan.Foreground = new SolidColorBrush(TextColors.GetColorFromTag(tagType));

                            if (IsQuestTag(tagType))
                            {
                                isQuestChanges = true;
                                tagContent = HandleQuestTags(tagType, tagContent, attributes);
                            }

                            aCurrentSpan.Inlines.Add(colorSpan);
                            if (attributes.Any(a => a.Key == "isTargetLink".ToLower() && bool.TryParse(a.Value, out bool isTargetLink) && isTargetLink))
                            {
                                colorSpan.Inlines.Add(new CopyToClipboardInline(this, tagContent, $"/targetexact {tagContent}"));
                            }
                            else
                            {
                                AddText(tagContent, ref isQuestChanges, aCurrentSpan: colorSpan, aCurrentText: aCurrentText, isUnderline: isUnderline);
                            }
                        }
                    }
                    else
                    {
                        aCurrentText += line[charInde];
                    }
                }

                if (!string.IsNullOrEmpty(aCurrentText))
                {
                    aCurrentSpan.Inlines.Add(GetText(aCurrentText, isUnderline));
                    aCurrentText = string.Empty;
                }

                if (i+1 < realLines.Length)
                {
                    aCurrentSpan.Inlines.Add(new LineBreak());
                }
            }
        }

        private Rectangle CreateLegendRectangle(Color aLegendColor)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 20;
            rectangle.Height = 20;
            rectangle.Margin = new Thickness(2.5, 0, 2.5, 0);
            rectangle.RenderTransform = new TranslateTransform(0, 3.2);
            rectangle.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            rectangle.Fill = new SolidColorBrush(aLegendColor);
            return rectangle;
        }

        private string HandleQuestTags(Tag aTagType, string aTagContent, Dictionary<string, string> aDictionaryOfAttributes)
        {
            Quest quest = null;
            if (aDictionaryOfAttributes.ContainsKey("Id"))
            {
                if (!int.TryParse(aDictionaryOfAttributes["Id"], out var questId))
                {
                    //TODO log
                    Console.WriteLine($"Error parsing questtag with id. {aTagType.ToString()}, Id={aDictionaryOfAttributes["Id"]}");
                    return "Error";
                }

                if (!Repository.QuestsById.ContainsKey(questId)) {
                    //TODO log
                    Console.WriteLine($"Could not find quest with id={questId}");
                    return $"Unknown id:{questId}";
                }

                var questDAO = Repository.QuestsById[questId];
                quest = new Quest
                {
                    QuestId = questId,
                    Title = Quest.GetQuestTitle(questDAO.Title, questDAO.ChainLevel),
                    QuestKey = Quest.GetQuestKey(questDAO.Title, questDAO.ChainLevel),
                    ChainLevel = questDAO.ChainLevel,
                    QuestLevel = questDAO.QuestLevel,
                    Category = questDAO.Category,
                };
            }
            else
            {
                string questTitle = aTagContent.Trim().TrimEnd('.').TrimEnd(',').TrimStart('-');
                int chainLevel = 0;
                int indexOfSemicolon = questTitle.LastIndexOf(';');
                if (indexOfSemicolon > 0 && indexOfSemicolon > questTitle.Length - 4)
                {
                    if (int.TryParse(questTitle.Substring(indexOfSemicolon + 1, questTitle.Length - indexOfSemicolon - 1), out chainLevel))
                    {
                        chainLevel -= 1;
                    }

                    questTitle = questTitle.Substring(0, indexOfSemicolon);
                }

                quest = new Quest
                {
                    QuestKey = Quest.GetQuestKey(questTitle, chainLevel),
                    Title = Quest.GetQuestTitle(questTitle, chainLevel),
                    ChainLevel = chainLevel,
                    Category = "Uncategorized"
                };

                if (Repository.QuestsByTitleAndPart.TryGetValue(quest.QuestKey, out var questDAO))
                {
                    quest.QuestId = questDAO.QuestId;
                    quest.Category = questDAO.Category;
                    quest.QuestLevel = questDAO.QuestLevel;
                }
            }

           
            if (aTagType == SimpleTags.Tag.ReturnQuest)
            {
                quest.HasBeenDelivered = true;
                Quests.Add(quest);
            }
            else if (aTagType == SimpleTags.Tag.SkipQuest)
            {
                quest.HasBeenMarkedForSkip = true;
            }
            else
            {
                Quests.Add(quest);
            }

            quest.TextBrush = new SolidColorBrush(TextColors.GetColorFromTag(aTagType));
            return quest.Title;
        }

        private Inline GetText(string aText, bool isUnderline)
        {
            var run = new Run(aText);
            if (isUnderline)
            {
                return new Underline(run);
            }

            return run;
        }

        public void Update(int aStepNumber, int aNumberOfQuests, bool isLineSpacer)
        {
            _stepNumberRun.Text = string.Format(SubStepNumberStringFormat, aStepNumber.ToString().PadLeft(2, '0'));
            _questLogButton.SetText(aNumberOfQuests.ToString());


            if (isLineSpacer && !_paragraph.Inlines.Contains(_lineSpacerContainer))
            {
                _paragraph.Inlines.Add(_lineSpacerContainer);
            }
            else if (!isLineSpacer && _paragraph.Inlines.Contains(_lineSpacerContainer))
            {
                _paragraph.Inlines.Remove(_lineSpacerContainer);
            }
        }

        private static bool IsQuestTag(Tag aTag)
        {
            switch (aTag)
            {
                case SimpleTags.Tag.AcceptQuest:
                case SimpleTags.Tag.ReturnQuest:
                case SimpleTags.Tag.DoQuest:
                case SimpleTags.Tag.WorkOnQuest:
                case SimpleTags.Tag.SkipQuest:
                    return true;
                default:
                    return false;
            }
        }


        public void ResetSearch()
        {
            foreach (var textRangeBrushTuple in _foundSearches)
            {
                textRangeBrushTuple.Item1.ApplyPropertyValue(TextElement.BackgroundProperty, textRangeBrushTuple.Item2);
            }
        }

        /// <summary>
        /// Copied from https://stackoverflow.com/questions/1756844/making-a-simple-search-function-making-the-cursor-jump-to-or-highlight-the-wo
        /// Modified it abit
        /// </summary>
        /// <param name="aSearchText"></param>
        /// <param name="isSearchNext"></param>
        /// <returns></returns>
        public bool DoSearch(string aSearchText, bool isSearchNext)
        {
            TextRange searchRange;

            // Get the range to search
            if (isSearchNext)
                searchRange = new TextRange(
                    Selection.Start.GetPositionAtOffset(1),
                    Document.ContentEnd);
            else
                searchRange = new TextRange(
                    Document.ContentStart,
                    Document.ContentEnd);

            // Do the search
            TextRange foundRange = FindTextInRange(searchRange, aSearchText);
            if (foundRange == null)
                return false;

            IsReadOnly = false;

            _foundSearches.Add(new Tuple<TextRange, Brush>(foundRange, foundRange.GetPropertyValue(TextElement.BackgroundProperty) as Brush));
            foundRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
            Selection.Select(foundRange.Start, foundRange.End);

            return true;
        }

        public TextRange FindTextInRange(TextRange aSearchRange, string aSearchText)
        {
            // Search the text with IndexOf
            int offset = aSearchRange.Text.IndexOf(aSearchText, StringComparison.InvariantCultureIgnoreCase);
            if (offset < 0)
                return null;  // Not found

            // Try to select the text as a contiguous range
            for (TextPointer start = aSearchRange.Start.GetPositionAtOffset(offset); start != aSearchRange.End; start = start.GetPositionAtOffset(1))
            {
                TextRange result = new TextRange(start, start.GetPositionAtOffset(aSearchText.Length));
                if (result.Text.ToLower() == aSearchText.ToLower())
                    return result;
            }

            return null;
        }

    }
}
