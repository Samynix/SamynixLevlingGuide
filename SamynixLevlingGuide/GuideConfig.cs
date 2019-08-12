using SamynixLevlingGuide.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SamynixLevlingGuide
{
    public class GuideConfig : ConfigurationSection
    {
        public static GuideConfig Instance { get; }

        static GuideConfig()
        {
            var configFile = Path.Combine(Environment.CurrentDirectory, $"{nameof(GuideConfig)}");
            Configuration configuration = null;
            int tries = 0;
            while (tries < 2 && configuration == null)
            {
                try
                {
                    configuration = ConfigurationManager.OpenExeConfiguration(configFile);
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                    sb.AppendLine("<configuration>");
                    sb.AppendLine("</configuration>");

                    File.WriteAllText(configFile, sb.ToString());
                }
                finally
                {
                    tries++;
                }
            }

            if (configuration.Sections[nameof(GuideConfig)] == null)
            {
                Instance = new GuideConfig();
                configuration.Sections.Add(nameof(GuideConfig), Instance);
                configuration.Save(ConfigurationSaveMode.Modified);
            }
            else
            {
                Instance = (GuideConfig)configuration.Sections[nameof(GuideConfig)];
            }
        }

        private bool _isAutoSave = false;

        private GuideConfig()
        {


        }



        [ConfigurationProperty(nameof(LastUsedGuide))]
        public string LastUsedGuide
        {
            get => (string)this[nameof(LastUsedGuide)];
            set
            {
                this[nameof(LastUsedGuide)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(LastUsedStep))]
        public string LastUsedStep
        {
            get => (string)this[nameof(LastUsedStep)];
            set
            {
                this[nameof(LastUsedStep)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(LastUsedClass))]
        public ClassEnum LastUsedClass
        {
            get
            {
                try
                {
                    return (ClassEnum)this[nameof(LastUsedClass)];
                }
                catch (Exception ex)
                {
                    return ClassEnum.All;
                }
            }

            set
            {
                this[nameof(LastUsedClass)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(WindowWidth))]
        public int WindowWidth
        {
            get
            {
                try
                {
                    return (int)this[nameof(WindowWidth)];
                }
                catch (Exception ex)
                {
                    return 800;
                }

            }
            set
            {
                this[nameof(WindowWidth)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(WindowHeight))]
        public int WindowHeight
        {
            get
            {
                try
                {
                    return (int)this[nameof(WindowHeight)];
                }
                catch (Exception ex)
                {
                    return 600;
                }
            }
            set
            {
                this[nameof(WindowHeight)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(LastUsedWindowState))]
        public WindowState LastUsedWindowState
        {
            get
            {
                try
                {
                    return (WindowState)this[nameof(LastUsedWindowState)];
                }
                catch (Exception ex)
                {
                    return WindowState.Normal;
                }
            }
            set
            {
                this[nameof(LastUsedWindowState)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(VerticalScrollOffset))]
        public int VerticalScrollOffset
        {
            get
            {
                try
                {
                    return (int)this[nameof(VerticalScrollOffset)];
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            set
            {
                this[nameof(VerticalScrollOffset)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        [ConfigurationProperty(nameof(CheckedSubSteps))]
        public CheckedSubStepCollection CheckedSubSteps
        {
            get
            {
                try
                {
                    return (CheckedSubStepCollection)this[nameof(CheckedSubSteps)];
                }
                catch(Exception ex)
                {
                    return new CheckedSubStepCollection();
                }
            }
            set
            {
                this[nameof(CheckedSubSteps)] = value;
                if (_isAutoSave)
                {
                    Save();
                }
            }
        }

        public void Save()
        {
            var configFile = Path.Combine(Environment.CurrentDirectory, $"{nameof(GuideConfig)}");
            Configuration config = ConfigurationManager.OpenExeConfiguration(configFile);
            GuideConfig section = (GuideConfig)config.Sections[nameof(GuideConfig)];
            section._isAutoSave = false;
            section.LastUsedGuide = this.LastUsedGuide;
            section.LastUsedStep = this.LastUsedStep;
            section.LastUsedClass = this.LastUsedClass;
            section.LastUsedWindowState = this.LastUsedWindowState;
            section.WindowHeight = this.WindowHeight;
            section.WindowWidth = this.WindowWidth;
            section.VerticalScrollOffset = this.VerticalScrollOffset;
            section.CheckedSubSteps = this.CheckedSubSteps;

            config.Save(ConfigurationSaveMode.Full); //Try with "Modified" to see the difference

        }

        public class CheckedSubStepCollection : ConfigurationElementCollection
        {
            public void Add(string aKey)
            {
                LockItem = false;
                BaseAdd(new CheckedSubStep
                {
                    Key = aKey
                });
            }

            public void Clear()
            {
                LockItem = false;
                BaseClear();
            }

            protected override ConfigurationElement CreateNewElement()
            {
                return new CheckedSubStep();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((CheckedSubStep)element).Key;
            }

            public new CheckedSubStep this[string elementName]
            {
                get
                {
                    return this.OfType<CheckedSubStep>().FirstOrDefault(item => item.Key == elementName);
                }
            }
        }

        public class CheckedSubStep : ConfigurationElement
        {
            [ConfigurationProperty(nameof(Key), IsKey = true, IsRequired = true)]
            public string Key
            {
                get { return (string)base[nameof(Key)]; }
                set { base[nameof(Key)] = value; }
            }
        }
    }
}
