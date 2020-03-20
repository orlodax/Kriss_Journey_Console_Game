using System;
using System.IO;

namespace ConsoleGame.Classes
{
    public class Navigator
    {
        public ChapterBase LastChapter { get; set; }

        public Navigator()
        {
            LoadProgress();
        }

        void LoadProgress()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                LastChapter = Newtonsoft.Json.JsonConvert.DeserializeObject<ChapterBase>(json);
            }
            else
                LastChapter = new ChapterBase() { Number = 0, IsComplete = false };
        }

        public void LoadChapter(int number)
        {
            ChapsFactory.CreateChapter(number);
        }

        public void SaveProgress(int chapterNo)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
            
            LastChapter.Number = chapterNo;
            LastChapter.IsComplete = true;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(LastChapter, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
    }
}
