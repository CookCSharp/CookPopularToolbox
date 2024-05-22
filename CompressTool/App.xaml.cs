using DryIoc;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompressTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : System.Windows.Application
    {
        internal static readonly Container DryIocContainer = new Container();

        public App()
        {
            InitializeComponent();

            ConfigService();
        }

        private void ConfigService()
        {
            //DryIocContainer.Register<HelperBase, SystemIoCompressionHelper>(serviceKey: nameof(SystemIoCompressionHelper));
            //DryIocContainer.Register<HelperBase, DotNetZipHelper>(serviceKey: nameof(DotNetZipHelper));
            //DryIocContainer.Register<HelperBase, SharpZipLibHelper>(serviceKey: nameof(SharpZipLibHelper));
            DryIocContainer.Register<HelperBase, SharpCompressHelper>(serviceKey: nameof(SharpCompressHelper));
            DryIocContainer.Register<HelperBase, SevenZipSharpHelper>(serviceKey: nameof(SevenZipSharpHelper));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}
