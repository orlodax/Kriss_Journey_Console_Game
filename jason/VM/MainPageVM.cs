using jason.Classes;
using jason.V;
using lybra;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;

namespace jason.VM
{
    public class MainPageVM : Observable
    {
        #region Props and fields
        private ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();
        private Chapter selectedChapter;
        private NodeBase selectedNode;
        private string jPath;
        private bool typeDetailVisibility;
        private FrameworkElement typeDetailPane;

        public ObservableCollection<Chapter> Chapters { get => chapters; set => SetValue(ref chapters, value); }
        public Chapter SelectedChapter { get => selectedChapter; set => SetValue(ref selectedChapter, value); }
        public NodeBase SelectedNode { get => selectedNode; set { SetValue(ref selectedNode, value); SelectedNodeChanged(); } }
        public string JPath { get => jPath; set => SetValue(ref jPath, value); }

        // the first level of details (list of choices, dialogues, actions)
        public bool TypeDetailVisibility { get => typeDetailVisibility; set => SetValue(ref typeDetailVisibility, value); }
        public FrameworkElement TypeDetailPane { get => typeDetailPane; set => SetValue(ref typeDetailPane, value); }

        public string[] NodeTypes { get; set; } = { "Story", "Choice", "Dialogue", "Action" };


        ApplicationDataContainer LocalSettings;
        Dictionary<Chapter, StorageFile> ChapterFiles;
        

        public ICommand ChooseFolder { get; private set; }
        public ICommand NewChapter { get; private set; }
        public ICommand NewNode { get; private set; }
        #endregion

        #region CTOR
        public MainPageVM()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
            //LocalSettings.Values["workFolder"] = null;
            ChooseFolder = new RelayCommand(Exec_ChooseFolder);
            NewChapter = new RelayCommand(Exec_NewChapter);
            NewNode = new RelayCommand(Exec_NewNode);

            Init();
        }
        private async void Init()
        {
            if (LocalSettings.Values.TryGetValue("workFolder", out object path))
            {
                var workFolder = await StorageFolder.GetFolderFromPathAsync(path.ToString());
                if (workFolder != null)
                    LoadChapters(workFolder);
            }
            else
                Exec_ChooseFolder(null);
        }
        #endregion

        private void SelectedNodeChanged()
        {
            Task.Run(() => SaveChapters());

            if (SelectedNode != null)
            {
                switch (SelectedNode.Type)
                {
                    case "Story":
                        TypeDetailVisibility = false;
                        break;

                    case "Action":
                        TypeDetailVisibility = true;
                        TypeDetailPane = new NActionDetail() { DataContext = new NActionDetailVM(SelectedNode.Actions) };
                        break;

                    case "Choice":
                        TypeDetailVisibility = true;
                        break;

                    case "Dialogue":
                        TypeDetailVisibility = true;
                        break;
                }
            }
        }

        private async void LoadChapters(StorageFolder folder)
        {
            ChapterFiles = new Dictionary<Chapter, StorageFile>();

            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".json")
                {
                    string json = await FileIO.ReadTextAsync(file);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var chapter = Newtonsoft.Json.JsonConvert.DeserializeObject<Chapter>(json);
                        ChapterFiles.Add(chapter, file);
                        Chapters.Add(chapter);
                    }
                }
            }
        }
        private async void SaveChapters()
        {
            if (SelectedNode != null && SelectedChapter != null)
            {
                if (LocalSettings.Values.TryGetValue("workFolder", out object path))
                {
                    var workFolder = await StorageFolder.GetFolderFromPathAsync(path.ToString());
                    if (workFolder != null)
                    {
                        if (ChapterFiles.TryGetValue(SelectedChapter, out StorageFile file))
                        {
                            string uglyString = Newtonsoft.Json.JsonConvert.SerializeObject(SelectedChapter);
                            string okString = JToken.Parse(uglyString).ToString();
                            await FileIO.WriteTextAsync(file, okString);
                        }
                    }
                }
            }
        }

        #region Command methods
        private async void Exec_ChooseFolder(object parameter)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                if (LocalSettings.Values.ContainsKey("workFolder"))
                    LocalSettings.Values["workFolder"] = folder.Path;
                else
                    LocalSettings.Values.Add("workFolder", folder.Path);

                LoadChapters(folder);
            }
        }
        private void Exec_NewChapter(object parameter)
        {

        }
        private void Exec_NewNode(object parameter)
        {

        }
        #endregion
    }
}
