using CookPopularControl.Windows;
using CookPopularCSharpToolkit.Communal;
using CookPopularCSharpToolkit.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;


namespace ImageTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>
    /// 将Geometry转为图片(png、ico等格式)
    /// </remarks>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public partial class MainWindow : NormalWindow
    {
        public int ImageWidth { get; set; } = 100;
        public int ImageHeight { get; set; } = 100;
        public double ImageActualWidth { get; set; }
        public double ImageActualHeight { get; set; }
        public Geometry GeometryData { get; set; } = ResourceHelper.GetResource<Geometry>("DemoImageGeometry"); //DemoImageGeometry
        public System.Windows.Media.Brush GeometryBrush => GeometryBrushes[SelectedIndexBrush];
        public IReadOnlyCollection<string> BrushNames { get; set; }
        public int SelectedIndexBrush { get; set; }
        public bool IsUseActualSize { get; set; }
        public bool? IsGenerating { get; set; }
        public System.Windows.Media.Brush GenerateBrush { get; set; } = System.Windows.SystemColors.ControlLightBrush;

        private static readonly string ImageFolder = AppDomain.CurrentDomain.BaseDirectory + "ImageTools\\";
        private readonly IList<System.Windows.Media.Brush> GeometryBrushes = new List<System.Windows.Media.Brush>();
        private string _imageFile = ImageFolder + "App.png";
        //private System.Drawing.Size _size = new System.Drawing.Size(40, 40);

        public MainWindow()
        {
            InitializeComponent();

            var type = typeof(System.Windows.Media.Brushes);
            var instance = (System.Windows.Media.Brushes)Activator.CreateInstance(type, true);

            IList<string> brushNames = new List<string>();
            type.GetProperties().ForEach(prop =>
            {
                brushNames.Add(prop.Name);
                GeometryBrushes.Add(prop.GetValue(instance) as System.Windows.Media.Brush);
            });

            BrushNames = new ReadOnlyCollection<string>(brushNames);
            SelectedIndexBrush = BrushNames.IndexOf(name => name.Equals("Red"));

            SetImageSource();

            if (!Directory.Exists(ImageFolder))
                Directory.CreateDirectory(ImageFolder);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SetImageSource();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            IsGenerating = true;
            GenerateBrush = System.Windows.SystemColors.ControlLightBrush;

            SetImageSource();
            ConvertGeometryToPicture();
        }

        private void SetImageSource()
        {
            GeometryDrawing geometryDrawing = new GeometryDrawing()
            {
                Brush = GeometryBrush,
                Geometry = GeometryData,
            };
            DrawingImage drawingImage = new DrawingImage();
            drawingImage.Drawing = geometryDrawing;
            img.Width = ImageWidth / DpiHelper.GetScaleX();
            img.Height = ImageHeight / DpiHelper.GetScaleY();
            img.Source = drawingImage;
            img.HorizontalAlignment = IsUseActualSize ? HorizontalAlignment.Left : HorizontalAlignment.Center;
            img.VerticalAlignment = IsUseActualSize ? VerticalAlignment.Top : VerticalAlignment.Center;
            img.LayoutUpdated += (s, e) =>
            {
                ImageActualWidth = img.ActualWidth * DpiHelper.GetScaleX();
                ImageActualHeight = img.ActualHeight * DpiHelper.GetScaleY();
            };
        }

        private async void ConvertGeometryToPicture()
        {
            await Task.Run(() =>
            {
                img.InvokeOnLayoutUpdated(async () =>
                {
                    SaveAsPicture(img, _imageFile);
                    using (var bitmap = new Bitmap(_imageFile))
                    {
                        SaveAsIconFile(bitmap, ImageFolder + "App.ico");
                    }

                    //img.SaveAsPicture(fileName);
                    //using (var bitmap = new Bitmap(fileName))
                    //{                    
                    //    bitmap.SaveAsIconFile(new System.Drawing.Size(40, 40), AppDomain.CurrentDomain.BaseDirectory + "Logo.ico");
                    //}

                    await Task.Delay(200);

                    IsGenerating = false;
                    GenerateBrush = System.Windows.Media.Brushes.ForestGreen;
                });
            });
        }

        private void SaveAsPicture(FrameworkElement element, string fileName, System.Drawing.Size? size = null)
        {
            var dpiX = DpiHelper.DeviceDpiX;
            var dpiY = DpiHelper.DeviceDpiY;

            int width, height;
            if (IsUseActualSize)
            {
                double elementWidth = 0;
                double elementHeight = 0;
                CheckElementSide(ref elementWidth, ref elementHeight);

                width = (int)(elementWidth * DpiHelper.GetScaleX());
                height = (int)(elementHeight * DpiHelper.GetScaleY());
            }
            else
            {
                width = (int)(element.Width * DpiHelper.GetScaleX());
                height = (int)(element.Height * DpiHelper.GetScaleY());
            }

            var bitmapSource = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Default);
            bitmapSource.Render(element);
            GenerateImage(bitmapSource, fileName);

            ////生成透明背景图片
            //var pixels = new int[width * height];
            //bitmapSource.CopyPixels(pixels, width * 4, 0);
            //using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
            //{
            //    for (int y = 0; y < height; y++)
            //        for (int x = 0; x < width; x++)
            //            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixels[y * width + x]));

            //    if (size.HasValue)
            //    {
            //        using (var newBitmap = new Bitmap(bitmap, size.Value))
            //        {
            //            newBitmap.Save(fileName, ImageFormat.Png);
            //        }
            //    }
            //    else
            //        bitmap.Save(fileName, ImageFormat.Png);
            //}

            void CheckElementSide(ref double elementWidth, ref double elementHeight)
            {
                if (!double.IsNaN(element.ActualWidth) && element.ActualWidth.CompareTo(0) > 0)
                    elementWidth = element.ActualWidth;
                else if (element.Width.CompareTo(0) > 0)
                    elementWidth = element.Width;
                else
                    elementWidth = 100;

                if (!double.IsNaN(element.ActualHeight) && element.ActualWidth.CompareTo(0) > 0)
                    elementHeight = element.ActualHeight;
                else if (element.Height.CompareTo(0) > 0)
                    elementHeight = element.Height;
                else
                    elementHeight = 100;
            }
        }

        private void SaveAsIconFile(Bitmap bitmap, string saveFilePath, System.Drawing.Size? newSize = null)
        {
            using (Icon icon = newSize.HasValue ? bitmap.ToIcon(newSize.Value) : bitmap.ToIcon())
            {
                using (Stream stream = new FileStream(saveFilePath, FileMode.Create))
                {
                    icon.Save(stream);
                }
            }
        }

        private void GenerateImage(BitmapSource bitmap, string path)
        {
            BitmapEncoder encoder = null;
            encoder = new PngBitmapEncoder();

            using (Stream destStream = File.Open(path, FileMode.OpenOrCreate))
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(destStream);
            }

            //switch (format)
            //{
            //    case ImageFormat.Jpeg:
            //        encoder = new JpegBitmapEncoder();
            //        break;
            //    case ImageFormat.PNG:
            //        encoder = new PngBitmapEncoder();
            //        break;
            //    case ImageFormat.BMP:
            //        encoder = new BmpBitmapEncoder();
            //        break;
            //    case ImageFormat.GIF:
            //        encoder = new GifBitmapEncoder();
            //        break;
            //    case ImageFormat.TIF:
            //        encoder = new TiffBitmapEncoder();
            //        break;
            //    default:
            //        throw new InvalidOperationException();
            //}
        }
    }
}
