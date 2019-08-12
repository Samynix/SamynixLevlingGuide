using SamynixLevlingGuide.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SamynixLevlingGuide.Model
{
    public class SubStep
    {
        private readonly Guide _guide;
        private readonly Step _step;

        public Action<bool> IsDoneChanged;

        public SubStep(Guide aGuide, Step aStep)
        {
            _guide = aGuide;
            _step = aStep;
        }

        public string StepDirectory { get; private set; }

        public bool IsValid { get; private set; } = true;
        public int StepOrder { get; private set; }
        public ClassEnum ClassFlags { get; private set; } = ClassEnum.All;

        public Color? Legend { get; private set; }

        public string StepText { get; private set; }

        public List<SubStep> NextSubSteps { get; } = new List<SubStep>();

        public void SetIsDone(bool isDone)
        {
            IsDoneChanged?.Invoke(isDone);
            IsDone = isDone;
        }

        public bool IsDone { get; set; }


        public static SubStep Parse(Guide aGuide, Step aStep, Dictionary<string, string> aDictionaryOfAttributes, string aFileContent)
        {
            SubStep result = new SubStep(aGuide, aStep);
            result.StepDirectory = aStep.StepDirectory;

            if (!aDictionaryOfAttributes.TryGetValue("order", out string orderAttribute) || !int.TryParse(orderAttribute, out int order))
            {
                result.IsValid = false;
                return result;
            }
            result.StepOrder = order;

            result.IsValid = true;
            if (aDictionaryOfAttributes.TryGetValue("class", out string classAttribute))
            {
                result.ClassFlags = SimpleTags.GetContent<ClassEnum>(classAttribute);
            }
            else
            {
                result.ClassFlags = ClassEnum.All;
            }

            if (aDictionaryOfAttributes.TryGetValue("legend", out string legendAttribute))
            {
                result.Legend = SimpleTags.GetContent<Color>(legendAttribute);
            }

            StringBuilder contentBuilder = new StringBuilder();
            bool? hasFirstLineTab = null;
            foreach (var line in aFileContent.Split(Environment.NewLine.ToCharArray()))
            {
                if (string.IsNullOrEmpty(line.Trim()) || line.Trim().StartsWith("#"))
                {
                    continue;
                }

                if (!hasFirstLineTab.HasValue) {
                    hasFirstLineTab = line.StartsWith("\t");
                }

                if (hasFirstLineTab.Value && line.StartsWith("\t"))
                {
                    contentBuilder.AppendLine(line.Substring(1, line.Length - 1));
                }
                else
                {
                    contentBuilder.AppendLine(line);
                }
            }
        
            result.StepText = contentBuilder.ToString();
            return result;
        }

        public string Key => $"{_guide.Title}-{_step.Title}-{StepOrder}-{(int)ClassFlags}";

  

        public bool HasClass(ClassEnum aClassEnum)
        {
            return ((int)ClassFlags & (int)aClassEnum) == (int)aClassEnum;
        }
    }
}
