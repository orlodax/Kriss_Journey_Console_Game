using System;
using System.IO;

namespace ConsoleGame.Classes
{
    public static class Navigator
    {
        public static ChapterBase LastChapter { get; private set; } = new ChapterBase();

        public static void LoadChapter(int number)
        {
            NodeFactory.CreateChapter(number);
        }

        #region Progress
        public static void LoadProgress()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                LastChapter = Newtonsoft.Json.JsonConvert.DeserializeObject<ChapterBase>(json);
                var a = LastChapter;
            }
            else
                LastChapter = new ChapterBase() { Number = 0, IsComplete = false };
        }
        public static void SaveProgress(int chapterNo)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
            
            LastChapter.Number = chapterNo;
            LastChapter.IsComplete = true;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(LastChapter, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
        #endregion
    }
}
