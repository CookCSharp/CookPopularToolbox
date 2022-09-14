using CookPopularControl.Windows;
using CookPopularCSharpToolkit.Communal;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace FileTools
{
    ///// <summary>
    ///// Interaction logic for MainWindow.xaml
    ///// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class MainWindow : NormalWindow
    {
        public string SourceFolder { get; set; }
        public string SearchPattern { get; set; } = "nupkg";
        public string DestinationFolder { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SourceFolder))
            {
                MessageDialog.ShowWarning($"{SourceFolder}搜索目录不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(DestinationFolder))
            {
                MessageDialog.ShowWarning($"{DestinationFolder}输出目录不能为空");
                return;
            }

            if (!Directory.Exists(SourceFolder))
            {
                MessageDialog.ShowWarning($"{SourceFolder}输出目录不存在");
                return;
            }

            if (!Directory.Exists(DestinationFolder))
            {
                Directory.CreateDirectory(DestinationFolder);
            }

            //var files = Directory.GetFiles(SourceFolder, $"*.{SearchPattern}", SearchOption.AllDirectories);
            var files = Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(SourceFolder, Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories, $"*.{SearchPattern}");
            files.ForEach(source =>
            {
                var fileName = Path.GetFileName(source);
                var destination = Path.Combine(DestinationFolder, fileName);
                Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(source, destination);
            });
        }

        private static Task<long> GetFileLengthsAsync(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                return Task.FromException<long>(new DirectoryNotFoundException("Invalid directory name."));
            }
            else
            {
                string[] files = Directory.GetFiles(filePath);
                if (files.Length == 0)
                    return Task.FromResult(0L);
                else
                    return Task.Run(() =>
                    {
                        long total = 0;
                        Parallel.ForEach(files, (fileName) =>
                        {
                            var fs = new FileStream(fileName, FileMode.Open,
                                                    FileAccess.Read, FileShare.ReadWrite,
                                                    1, true);
                            long length = fs.Length;
                            Interlocked.Add(ref total, length);
                            fs.Close();
                        });
                        return total;
                    });
            }
        }
    }
}
