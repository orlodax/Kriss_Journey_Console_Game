using jason.Classes;
using lybra;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Storage;

namespace jason.VM
{
    public class MainPageVM : Observable
    {
        private ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();
        private Chapter selectedChapter;
        private NodeBase selectedNode;
        private string jPath;

        public ObservableCollection<Chapter> Chapters { get => chapters; set => SetValue(ref chapters, value); }
        public Chapter SelectedChapter { get => selectedChapter; set => SetValue(ref selectedChapter, value); }
        public NodeBase SelectedNode { get => selectedNode; set => SetValue(ref selectedNode, value); }
        public string JPath { get => jPath; set => SetValue(ref jPath, value); }

        public string[] NodeTypes { get; set; } = { "Story", "Choice", "Dialogue", "Action" };


        ApplicationDataContainer LocalSettings;


        public ICommand ChooseFolder { get; private set; }
        public ICommand NewChapter { get; private set; }
        public ICommand NewNode { get; private set; }


        public MainPageVM()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;

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
        }
        private async void LoadChapters(StorageFolder folder)
        {
            foreach (StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".json")
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    ulong size = stream.Size;
                    using (var inputStream = stream.GetInputStreamAt(0))
                    {
                        using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                        {
                            uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                            string text = dataReader.ReadString(numBytesLoaded);
                            if (!string.IsNullOrEmpty(text))
                                Chapters.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Chapter>(text));
                        }
                    }
                }
            }
        }
        private async void Exec_ChooseFolder(object parameter)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

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
    }
}
