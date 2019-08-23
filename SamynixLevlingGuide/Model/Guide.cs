using SamynixLevlingGuide.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamynixLevlingGuide.Model
{
    public class Guide
    {
        private const string GuideInfoFile = "GuideInfo.txt";
        private const string StepDirectoryPrefix = "Step-";

        public static string MissingImageUrl => Path.Combine(Environment.CurrentDirectory, "Guide", "missingImage.png");

        public bool IsValid { get; private set; }
        public string Title { get; private set; }
        public ClassEnum Classes { get; private set; }

        public List<Step> Steps { get; private set; } = new List<Step>();


        public override string ToString()
        {
            var classList = GetListOfClasses();
            return $"{Title} ({string.Join(", ", classList)})";
        }

        private void AddClassToListIfPresent(ClassEnum aClassEnum, List<ClassEnum> aList)
        {
            if (Classes.HasFlag(aClassEnum))
            {
                aList.Add(aClassEnum);
            }
        }

        public List<ClassEnum> GetListOfClasses()
        {
            List<ClassEnum> classList = new List<ClassEnum>();
            AddClassToListIfPresent(ClassEnum.Druid, classList);
            AddClassToListIfPresent(ClassEnum.Hunter, classList);
            AddClassToListIfPresent(ClassEnum.Mage, classList);
            AddClassToListIfPresent(ClassEnum.Paladin, classList);
            AddClassToListIfPresent(ClassEnum.Priest, classList);
            AddClassToListIfPresent(ClassEnum.Rogue, classList);
            AddClassToListIfPresent(ClassEnum.Shaman, classList);
            AddClassToListIfPresent(ClassEnum.Warlock, classList);
            AddClassToListIfPresent(ClassEnum.Warrior, classList);
            return classList;
        }

        public static Guide Parse(string aDirectory, int? aOnlyLoadThisStep)
        {
            Guide result = new Guide();
            bool titleFound = false;
            bool classFound = false;

            if (!File.Exists(Path.Combine(aDirectory, GuideInfoFile))) {
                return result;
            }

            string guideFileContent = File.ReadAllText(Path.Combine(aDirectory, GuideInfoFile));
            foreach (var line in guideFileContent.Split(Environment.NewLine.ToCharArray()).Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
            {
                int index = 0;
                if (TagParser.ParseTag(line, ref index, out string tag, out string tagContent, out var attributes, isIncrementIndex: false))
                {
                    if (SimpleTags.IsTag(SimpleTags.Tag.Title, tag)) {
                        result.Title = SimpleTags.GetContent<string>(tagContent);
                        titleFound = true;
                    }
                    else if (SimpleTags.IsTag(SimpleTags.Tag.Class, tag))
                    {
                        result.Classes = SimpleTags.GetContent<ClassEnum>(tagContent);
                        classFound = true;
                    }
                }
            }

            result.IsValid = titleFound && classFound;
            if (result.IsValid)
            {
                ParseSteps(result, aDirectory, aOnlyLoadThisStep);
            }

            return result;
        }

        private static void ParseSteps(Guide aGuide, string aGuideDirectory, int? aOnlyLoadThisStep)
        {
            Dictionary<int, Step> validSteps = new Dictionary<int, Step>();
            foreach (var directory in Directory.EnumerateDirectories(aGuideDirectory))
            {
                int directoryLastIndexOfSlash = directory.LastIndexOf('\\');
                string directoryName = directory.Substring(directoryLastIndexOfSlash + 1, directory.Length - directoryLastIndexOfSlash - 1);

                if (!directoryName.StartsWith(StepDirectoryPrefix))
                {
                    //TODO warn
                    Console.WriteLine($"Could not find index of {StepDirectoryPrefix} in {directory}");
                    continue;
                }

                var step = Step.Parse(aGuide, directory);
                if (step.IsValid && (!aOnlyLoadThisStep.HasValue || aOnlyLoadThisStep.Value == step.StepNumber))
                {
                    if (validSteps.ContainsKey(step.StepNumber))
                    {
                        //TODO warn
                        Console.WriteLine($"Multiple steps with same number {step.StepNumber}. Skipping");
                        continue;
                    }

                    validSteps[step.StepNumber] = step;
                }
            }

            AddSteps(aGuide, validSteps);
        }

        private static void AddSteps(Guide aGuide, Dictionary<int, Step> aDictionaryOfSteps)
        {
            Step previousStep = null;
            foreach (var valuePair in aDictionaryOfSteps.OrderBy(vp => vp.Key))
            {
                Step step = valuePair.Value;
                if (!step.IsValid)
                {
                    continue;
                }

                if (previousStep != null)
                {
                    step.PreviousStep = previousStep;
                    step.PreviousStep.NextStep = step;
                }

                previousStep = step;
                aGuide.Steps.Add(step);
            }
        }
    }
}
