using DryIoc;
using DryIoc.ImTools;
using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//特殊字符 转义字符
//&	      &
//<       &lt;
//>       &gt;
//"	      &quot;
//'	      &apos;

namespace CompressTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Methods { get; set; }

        public ObservableCollection<string> Levels { get; set; }

        public ObservableCollection<string> Formats { get; set; }

        public ObservableCollection<ResultInfo> Results { get; set; }

        public string OpenSourceExplain { get; set; }

        public int MethodSelectedIndex { get; set; }

        public int LevelSelectedIndex { get; set; }

        public int FormatSelectedIndex { get; set; }

        public string CompressDirectory { get; set; } = ConfigurationManager.AppSettings.Get("CompressDirectory");

        public string CompressFile { get; set; } = ConfigurationManager.AppSettings.Get("CompressFile");

        public string CompressContentName { get; set; }

        public string OutputDirectory { get; set; } = ConfigurationManager.AppSettings.Get("OutputDirectory");

        public double TotallPercent { get; set; }

        public double SinglePercent { get; set; }

        public string SingleFileName { get; set; }

        public int FileCount => Results == null ? 0 : Results.Count;

        public OpenSourceDllType OpenSourceDllType { get; set; }

        public TestType TestType { get; set; }

        public CompressContentType CompressContentType { get; set; }

        public OrderType OrderType { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            UpdateSelectTestType();
        }

        private void UpdateResults()
        {
            switch (CompressContentType)
            {
                case CompressContentType.Directory:
                    if (!string.IsNullOrEmpty(CompressDirectory))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(CompressDirectory);
                        if (directoryInfo.Exists)
                        {
                            CompressContentName = directoryInfo.Name;
                        }
                    }
                    break;
                case CompressContentType.File:
                    if (!string.IsNullOrEmpty(CompressFile))
                    {
                        FileInfo fileInfo = new FileInfo(CompressFile);
                        if (fileInfo.Exists)
                        {
                            CompressContentName = fileInfo.Name;
                        }
                    }
                    break;
                default:
                    break;
            }

            MethodSelectedIndex = MethodSelectedIndex < 0 ? 0 : MethodSelectedIndex;
            LevelSelectedIndex = LevelSelectedIndex < 0 ? 0 : LevelSelectedIndex;
            FormatSelectedIndex = FormatSelectedIndex < 0 ? 0 : FormatSelectedIndex;

            string archiveFileFormat = "";
            List<string> results = new List<string>();
            switch (OpenSourceDllType)
            {
                case OpenSourceDllType.SevenZipSharp:
                    Methods = new ObservableCollection<string>(Enum.GetNames<SevenZip.CompressionMethod>());
                    Levels = new ObservableCollection<string>(Enum.GetNames<SevenZip.CompressionLevel>());
                    Formats = new ObservableCollection<string>(Enum.GetNames<SevenZip.OutArchiveFormat>());

                    switch (TestType)
                    {
                        case TestType.Method:
                            results = new List<string>(Enum.GetNames<SevenZip.CompressionMethod>()
                                                           .Select(m => $"_{m}_{Enum.GetValues<SevenZip.CompressionLevel>()[LevelSelectedIndex]}"));
                            break;
                        case TestType.Level:
                            results = new List<string>(Enum.GetNames<SevenZip.CompressionLevel>()
                                                           .Select(l => $"_{Enum.GetValues<SevenZip.CompressionMethod>()[MethodSelectedIndex]}_{l}"));
                            break;
                        case TestType.All:
                            var methods = Enum.GetNames<SevenZip.CompressionMethod>();
                            var levels = Enum.GetNames<SevenZip.CompressionLevel>();
                            results = new List<string>(Enum.GetNames<SevenZip.CompressionMethod>()
                                                           .SelectMany(m => Enum.GetNames<SevenZip.CompressionLevel>().Select(l => $"_{m}_{l}")));
                            break;
                        default:
                            break;
                    }

                    switch (Enum.GetValues<SevenZip.OutArchiveFormat>()[FormatSelectedIndex])
                    {
                        case SevenZip.OutArchiveFormat.SevenZip:
                            archiveFileFormat = ".7z";
                            break;
                        case SevenZip.OutArchiveFormat.Zip:
                            archiveFileFormat = ".zip";
                            break;
                        case SevenZip.OutArchiveFormat.GZip:
                            archiveFileFormat = ".gz";
                            break;
                        case SevenZip.OutArchiveFormat.BZip2:
                            archiveFileFormat = ".bz2";
                            break;
                        case SevenZip.OutArchiveFormat.Tar:
                            archiveFileFormat = ".tar";
                            break;
                        case SevenZip.OutArchiveFormat.XZ:
                            archiveFileFormat = ".xz";
                            break;
                        default:
                            break;
                    }
                    break;
                case OpenSourceDllType.SharpCompress:
                    Methods = new ObservableCollection<string>(Enum.GetNames<SharpCompress.Common.CompressionType>());
                    if (FormatSelectedIndex == 0)
                        Levels = new ObservableCollection<string>(Enum.GetNames<SharpCompress.Compressors.Deflate.CompressionLevel>());
                    else
                    {
                        LevelSelectedIndex = 0;
                        Levels = new ObservableCollection<string>();
                    }
                    Formats = new ObservableCollection<string>() { "zip", "lz", "bz2", "gz(File)", "tar" };
                    archiveFileFormat = "." + Formats[FormatSelectedIndex].Replace("(File)", "");

                    switch (TestType)
                    {
                        case TestType.Method:
                            results = new List<string>(Enum.GetNames<SharpCompress.Common.CompressionType>()
                                                           .Select(m =>
                                                           {
                                                               if (Levels.Count > 0)
                                                               {
                                                                   return $"_{m}_{Enum.GetValues<SharpCompress.Compressors.Deflate.CompressionLevel>()[LevelSelectedIndex]}";
                                                               }
                                                               else
                                                                   return $"_{m}";
                                                           }));
                            break;
                        case TestType.Level:
                            if (Levels.Count > 0)
                                results = new List<string>(Enum.GetNames<SharpCompress.Compressors.Deflate.CompressionLevel>()
                                                               .Select(l => $"_{Enum.GetValues<SharpCompress.Common.CompressionType>()[MethodSelectedIndex]}_{l}"));
                            else
                                results = new List<string>(Enum.GetNames<SharpCompress.Common.CompressionType>().Select(m => $"_{m}"));
                            break;
                        case TestType.All:
                            if (Levels.Count > 0)
                            {
                                var methods = Enum.GetNames<SharpCompress.Common.CompressionType>();
                                var levels = Enum.GetNames<SharpCompress.Compressors.Deflate.CompressionLevel>();
                                results = new List<string>(Enum.GetNames<SharpCompress.Common.CompressionType>()
                                                               .SelectMany(m => Enum.GetNames<SharpCompress.Compressors.Deflate.CompressionLevel>().Select(l => $"_{m}_{l}")));
                            }
                            else
                                results = new List<string>(Enum.GetNames<SharpCompress.Common.CompressionType>().Select(m => $"_{m}"));
                            break;
                        default:
                            break;
                    }

                    //switch (Enum.GetValues<SharpCompress.Common.ArchiveType>()[FormatSelectedIndex])
                    //{
                    //    case SharpCompress.Common.ArchiveType.SevenZip:
                    //        archiveFileFormat = ".7z";
                    //        break;
                    //    case SharpCompress.Common.ArchiveType.Zip:
                    //        archiveFileFormat = ".zip";
                    //        break;
                    //    case SharpCompress.Common.ArchiveType.GZip:
                    //        archiveFileFormat = ".gz";
                    //        break;
                    //    case SharpCompress.Common.ArchiveType.Rar:
                    //        archiveFileFormat = ".rar";
                    //        break;
                    //    case SharpCompress.Common.ArchiveType.Tar:
                    //        archiveFileFormat = ".tar";
                    //        break;
                    //    default:
                    //        break;
                    //}
                    break;
                case OpenSourceDllType.SharpZipLib:
                    Methods = new ObservableCollection<string>(Enum.GetNames<ICSharpCode.SharpZipLib.Zip.CompressionMethod>());
                    Levels = new ObservableCollection<string>(Enum.GetNames<ICSharpCode.SharpZipLib.Zip.Compression.Deflater.CompressionLevel>());
                    Formats = new ObservableCollection<string>() { "zip", "bz2", "gz", "tar", "z" };
                    break;
                case OpenSourceDllType.DotNetZip:
                    Methods = new ObservableCollection<string>(Enum.GetNames<Ionic.Zip.CompressionMethod>());
                    Levels = new ObservableCollection<string>(Enum.GetNames<Ionic.Zlib.CompressionLevel>());
                    Formats = new ObservableCollection<string>() { "zip", "bz2" };
                    break;
                case OpenSourceDllType.SystemIOCompression:
                    Methods = new ObservableCollection<string>() { "ZLib", "GZip", "Deflat", "Brotli" };
                    Levels = new ObservableCollection<string>(Enum.GetNames<System.IO.Compression.CompressionLevel>());
                    Formats = new ObservableCollection<string>() { "zip" };
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(CompressContentName))
            {
                Results = new ObservableCollection<ResultInfo>(results.Select(s => new ResultInfo
                {
                    FileName = $"{CompressContentName}{s}{archiveFileFormat}",
                    ResultBrush = System.Windows.SystemColors.ControlDarkBrush
                }));
            }
        }

        private void CompressDll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object s = OpenSourceDllType switch
            {
                OpenSourceDllType.SevenZipSharp => OpenSourceExplain = "InArchiveFormat - such as 7-zip itself, zip, rar or cab and the format is automatically guessed by the archive signature;OutArchiveFormat - 7-zip, Xz, Zip, GZip, BZip2 and Tar. Please note that GZip and BZip2 compresses only one file at a time",
                OpenSourceDllType.SharpCompress => OpenSourceExplain = "unrar, decompress 7zip, decompress xz, zip/unzip, tar/untar lzip/unlzip, bzip2/unbzip2 and gzip/ungzip",
                OpenSourceDllType.SharpZipLib => OpenSourceExplain = "support Zip, GZip, Tar, BZip2, unZ",
                OpenSourceDllType.DotNetZip => OpenSourceExplain = "support Zip(Deflate、BZip2、LZMA)",
                OpenSourceDllType.SystemIOCompression => OpenSourceExplain = "support Zip(ZLib、GZip、Deflate、Brotli)",
                _ => throw new NotImplementedException(),
            };

            OpenSourceDllType = (OpenSourceDllType)e.AddedItems[0];
            UpdateResults();
        }

        private void CompressSelected_Changed(object sender, SelectionChangedEventArgs e)
        {
            UpdateResults();
        }

        private void UpdateSelectTestType()
        {
            switch (TestType)
            {
                case TestType.Method:
                    methodList.SelectionMode = System.Windows.Controls.SelectionMode.Multiple;
                    levelList.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                    methodList.IsHitTestVisible = false;
                    levelList.IsHitTestVisible = true;
                    methodList.SelectAll();
                    break;
                case TestType.Level:
                    methodList.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                    levelList.SelectionMode = System.Windows.Controls.SelectionMode.Multiple;
                    methodList.IsHitTestVisible = true;
                    levelList.IsHitTestVisible = false;
                    levelList.SelectAll();
                    break;
                case TestType.All:
                    methodList.SelectionMode = System.Windows.Controls.SelectionMode.Multiple;
                    levelList.SelectionMode = System.Windows.Controls.SelectionMode.Multiple;
                    methodList.IsHitTestVisible = false;
                    levelList.IsHitTestVisible = false;
                    methodList.SelectAll();
                    levelList.SelectAll();
                    break;
                default:
                    break;
            }
        }

        private void CompressTestType_Click(object sender, RoutedEventArgs e)
        {
            var children = (sender as StackPanel).Children;
            for (int i = 0; i < children.Count; i++)
            {
                if ((children[i] as System.Windows.Controls.RadioButton).IsChecked == true)
                    TestType = Enum.GetValues<TestType>()[i];
            }

            UpdateSelectTestType();

            UpdateResults();
        }

        private void CompressContentType_Click(object sender, RoutedEventArgs e)
        {
            var children = (sender as StackPanel).Children;
            for (int i = 0; i < children.Count; i++)
            {
                if ((children[i] as System.Windows.Controls.RadioButton).IsChecked == true)
                    CompressContentType = Enum.GetValues<CompressContentType>()[i];
            }

            UpdateResults();
        }

        private void CompressContentSelect_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            if (btn.Content.ToString() == "文件夹")
            {
                FolderBrowserDialog browserDialog = new FolderBrowserDialog();
                if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CompressDirectory = browserDialog.SelectedPath;
                    TestConst.SetInputDirectory(CompressDirectory);
                }
            }
            else
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    CompressFile = openFileDialog.FileName;
                    TestConst.SetInputDirectory(CompressFile);
                }
            }

            UpdateResults();
        }

        private void UpdateOrder()
        {
            switch (OrderType)
            {
                case OrderType.Size:
                    Results = new ObservableCollection<ResultInfo>(Results.OrderBy(f => f.CompressedSize));
                    break;
                case OrderType.Time:
                    Results = new ObservableCollection<ResultInfo>(Results.OrderBy(f =>
                    {
                        if (string.IsNullOrEmpty(f.CompressedTime))
                            return default;
                        else
                        {
                            if (f.CompressedTime.Contains("Failed"))
                            {
                                return TimeSpan.FromSeconds(0);
                            }
                            else
                            {
                                var timeStr = "0:" + f.CompressedTime.Replace("m", ":").Replace("s", "");
                                return TimeSpan.Parse(timeStr);
                            }
                        }
                    }));
                    break;
                case OrderType.SizeTime:
                    Results = new ObservableCollection<ResultInfo>(Results.OrderBy(f => f, new ResultCompressedSizeCompare()));
                    break;
                case OrderType.TimeSize:
                    Results = new ObservableCollection<ResultInfo>(Results.Order(new ResultCompressedTimeCompare()));
                    break;
                default:
                    break;
            }
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            var children = (sender as StackPanel).Children;
            for (int i = 1; i < children.Count; i++)
            {
                if ((children[i] as System.Windows.Controls.RadioButton).IsChecked == true)
                    OrderType = Enum.GetValues<OrderType>()[i - 1];
            }
            UpdateOrder();
        }

        private void UpdateDecompressResults(string name = "All")
        {
            var files = Directory.GetFiles(OutputDirectory, "*", SearchOption.AllDirectories)
                                 .Where(s => !s.Contains("Failed"));

            if (name == "Directory")
                files = files.Where(f => !System.IO.Path.HasExtension(f.Split('_').First()));
            else if (name == "File")
                files = files.Where(f => System.IO.Path.HasExtension(f.Split('_').First()));

            var results = files.Select(f =>
            {
                var fileInfo = new FileInfo(f);
                var time = fileInfo.Name.Split('_', '.')[^2];
                var resultInfo = new ResultInfo
                {
                    FileName = fileInfo.Name.Replace($"_{time}", ""),
                    CompressedSize = Math.Round(fileInfo.Length / 1024D / 1024D, 2),
                    CompressedTime = time,
                    Data = System.Windows.Application.Current.Resources["SuccessGeometry"] as Geometry,
                    ResultBrush = System.Windows.Media.Brushes.Green,
                    IsShow = true,
                };

                return resultInfo;
            });

            Results = new ObservableCollection<ResultInfo>(results);
            UpdateOrder();
        }

        private void ReadDirectory_Click(object sender, RoutedEventArgs e)
        {
            UpdateDecompressResults("All");
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            UpdateResults();
        }

        private async void Compress_Click(object sender, RoutedEventArgs e)
        {
            var helper = App.DryIocContainer.Resolve<HelperBase>($"{OpenSourceDllType}Helper");
            OutputDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", helper.CompressDllName, "Compressed");
            SaveAppConfig("CompressDirectory", CompressDirectory);
            SaveAppConfig("CompressFile", CompressFile);
            SaveAppConfig("OutputDirectory", OutputDirectory);

            if (Directory.Exists(OutputDirectory))
                Directory.Delete(OutputDirectory, true);

            OrderType = OrderType.Size;
            UpdateResults();
            Array.ForEach(Results.ToArray(), f => f.ResultBrush = System.Windows.Media.Brushes.Gray);

            Register(OpenSourceDllType, helper);
            await helper.Compress(this, TestType, CompressContentType == CompressContentType.Directory);
        }

        private async void Decompress_Click(object sender, RoutedEventArgs e)
        {
            var helper = App.DryIocContainer.Resolve<HelperBase>($"{OpenSourceDllType}Helper");
            OutputDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", helper.CompressDllName, "Compressed");
            SaveAppConfig("OutputDirectory", OutputDirectory);

            string decompressDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", helper.CompressDllName, "Decompressed");
            if (Directory.Exists(decompressDirectory))
                Directory.Delete(decompressDirectory, true);

            OrderType = OrderType.Time;
            UpdateDecompressResults(CompressContentType == CompressContentType.Directory ? "Directory" : "File");
            Array.ForEach(Results.ToArray(), f => f.ResultBrush = System.Windows.Media.Brushes.Gray);

            Register(OpenSourceDllType, helper);
            await helper.Decompress(this, TestType, CompressContentType == CompressContentType.Directory);
        }

        private void SaveAppConfig(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();
        }

        private void Register(OpenSourceDllType openSourceDllType, HelperBase instance)
        {
            switch (openSourceDllType)
            {
                case OpenSourceDllType.SevenZipSharp:
                    var sevenZipSharpHelper = (SevenZipSharpHelper)instance;
                    sevenZipSharpHelper.UpdateSingleFileHandler += Helper_UpdateSingleFileHandler;
                    sevenZipSharpHelper.UpdateTotalPercentHandler += Helper_UpdateTotalPercentHandler;
                    sevenZipSharpHelper.UpdateResultHandler += MainWindow_UpdateResultHandler;
                    break;
                case OpenSourceDllType.SharpCompress:
                    var sharpCompressHelper = (SharpCompressHelper)instance;
                    sharpCompressHelper.UpdateSingleFileHandler += Helper_UpdateSingleFileHandler;
                    sharpCompressHelper.UpdateTotalPercentHandler += Helper_UpdateTotalPercentHandler;
                    sharpCompressHelper.UpdateResultHandler += MainWindow_UpdateResultHandler;
                    break;
                case OpenSourceDllType.SharpZipLib:
                    break;
                case OpenSourceDllType.DotNetZip:
                    break;
                case OpenSourceDllType.SystemIOCompression:
                    break;
                default:
                    break;
            }
        }

        private void Helper_UpdateSingleFileHandler(string fileName, byte percent)
        {
            SingleFileName = fileName;
            SinglePercent = percent;
        }

        private void Helper_UpdateTotalPercentHandler(byte percent)
        {
            TotallPercent = percent;
        }

        private async void MainWindow_UpdateResultHandler(ResultInfo resultInfo)
        {
            await Dispatcher.BeginInvoke(() =>
            {
                var result = Results.Where(r => r.FileName == resultInfo.FileName).FirstOrDefault();
                var index = Results.IndexOf(result);
                Results.RemoveAt(index);
                Results.Insert(index, resultInfo);
            });

            UpdateOrder();
        }
    }

    [AddINotifyPropertyChangedInterface]
    public record ResultInfo
    {
        public string FileName { get; set; }

        public System.Windows.Media.Brush ResultBrush { get; set; }

        public Geometry Data { get; set; } = System.Windows.Application.Current.Resources["SuccessGeometry"] as Geometry;

        /// <summary>
        /// 单位：M
        /// </summary>
        public double CompressedSize { get; set; }

        /// <summary>
        /// 单位：XmYs
        /// </summary>
        public string CompressedTime { get; set; }

        /// <summary>
        /// 压缩/解压完成时显示
        /// </summary>
        public bool IsShow { get; set; }
    }
}
