using SamynixLevlingGuide.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamynixLevlingGuide.Model
{
    public class Step
    {
        private const string StepInfoFile = "StepInfo.txt";
        private const string SubstepPrefix = "Substep";

        private readonly Guide _guide;
        public Step(Guide aGuide)
        {
            _guide = aGuide;
        }

        public string StepDirectory { get; private set; }
        public int StepNumber { get; private set; }
        public string Title { get; private set; }
        public bool IsValid { get; private set; } 
        public Dictionary<ClassEnum, string> ImageSources { get; private set; } = new Dictionary<ClassEnum, string>();

        public Step PreviousStep { get; internal set; }
        public Step NextStep { get; internal set; }

        public List<SubStep> SubSteps { get; } = new List<SubStep>();


        private void ValidateImageSources()
        {
            foreach (var classEnum in _guide.GetListOfClasses().Concat(new[] { ClassEnum.All}))
            {
                if (!ImageSources.ContainsKey(classEnum) || !File.Exists(ImageSources[classEnum]))
                {
                    ImageSources[classEnum] = Guide.MissingImageUrl;
                }
            }
        }

        public static Step Parse(Guide aGuide, string aStepDirectory)
        {
            var result = new Step(aGuide);
            result.StepDirectory = aStepDirectory;

            var stepInfoFile = Path.Combine(aStepDirectory, StepInfoFile);
            if (!File.Exists(stepInfoFile)) {
                result.IsValid = false;
                return result;
            }


            var stepContent = File.ReadAllText(stepInfoFile);
            ExtractStepInfo(aGuide, result, stepContent, aStepDirectory);
            if (!result.IsValid)
            {
                return result;
            }

            IGrouping<int, SubStep> previousGrouping = null;
            foreach (var subStepGroup in result.SubSteps.GroupBy(ss => ss.StepOrder).OrderBy(g => g.Key))
            {
                if (previousGrouping == null)
                {
                    previousGrouping = subStepGroup;
                    continue;
                }

                foreach (var subStep in subStepGroup)
                {
                    foreach (var previousSubStep in previousGrouping)
                    {
                        previousSubStep.NextSubSteps.Add(subStep);
                    }
                }

                previousGrouping = subStepGroup;
            }

            return result;
        }

        private static void ExtractStepInfo(Guide aGuide, Step aStep, string aFileContents, string aStepDirectory)
        {
            aStep.IsValid = true;

            bool validTitleFound = false;
            bool validStepNumberFound = false;
            bool wasNewLine = true;
            for (int index = 0; index < aFileContents.Length; index++)
            {
                var character = aFileContents[index];
                if (wasNewLine && character == '#')
                {
                    continue;
                }

                wasNewLine = character == '\r' || character == '\n';
                if (TagParser.ParseTag(aFileContents, ref index, out var tag, out var tagContent, out var attributes, isIncrementIndex: true))
                {
                    if (SimpleTags.IsTag(SimpleTags.Tag.Title, tag))
                    {
                        aStep.Title = SimpleTags.GetContent<string>(tagContent);
                        validTitleFound = !string.IsNullOrEmpty(aStep.Title);
                    }
                    else if (SimpleTags.IsTag(SimpleTags.Tag.Image, tag))
                    {
                        ClassEnum classEnum = ClassEnum.All;
                        if (attributes.ContainsKey("class")) //TODO make hard reference
                        {
                            classEnum = SimpleTags.GetContent<ClassEnum>(attributes["class"]);
                        }

                        aStep.ImageSources[classEnum] = Path.Combine(aStepDirectory, SimpleTags.GetContent<string>(tagContent));
                    }
                    else if (SimpleTags.IsTag(SimpleTags.Tag.StepNumber, tag))
                    {
                        aStep.StepNumber = SimpleTags.GetContent<int>(tagContent);
                        validStepNumberFound = true;
                    }
                    else if (SimpleTags.IsTag(SimpleTags.Tag.SubStep, tag)) {
                        SubStep subStep = SubStep.Parse(aGuide, aStep, attributes, tagContent);
                        if (subStep.IsValid)
                        {
                            aStep.SubSteps.Add(subStep);
                        }
                    } 
                }
            }

            aStep.ValidateImageSources();
            if (!validTitleFound || !validStepNumberFound)
            {
                aStep.IsValid = false;
            }
        }
    }
}
