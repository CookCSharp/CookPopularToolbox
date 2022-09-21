using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;


namespace ToolboxManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        [NativeCppClass]
        [CLSCompliant(false)]
        [UnsafeValueType]
        [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
        public struct ProcessInfo
        {
            public uint CurrentProcessPID;
            public uint ParentProcessPID;
            [MarshalAs(UnmanagedType.LPTStr, SizeConst = 512)]
            public unsafe uint ParentProcessPath;
            [MarshalAs(UnmanagedType.LPTStr, SizeConst = 64)]
            public unsafe uint ParentProcessName;

            private static object GetDebuggerDisplay()
            {
                throw new NotImplementedException();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GetProcessInfo();
        }

        private void GetProcessInfo()
        {
            ProcessTools.ProcessHelper processHelper = new ProcessTools.ProcessHelper();
            unsafe
            {
                ulong currentProcessID, parentProcessID;
                uint errCodeForBuffer = 0;
                var intptr = Marshal.AllocHGlobal(512);

                processHelper.GetProcessPID(&currentProcessID, &parentProcessID);
                processHelper.GetProcessPIDAndName(&currentProcessID, &parentProcessID, (char*)intptr, &errCodeForBuffer);

                string parentProcessPath = new string((char*)intptr);
                Debug.WriteLine($"**********CurrentProcessPID:{currentProcessID}");
                Debug.WriteLine($"**********ParentProcessPID:{parentProcessID}");
                Debug.WriteLine($"**********ParentProcessPath:{parentProcessPath}");
                Debug.WriteLine($"**********ParentProcessName:{System.IO.Path.GetFileName(parentProcessPath)}");
                Debug.WriteLine($"**********ErrCodeForBuffer:{errCodeForBuffer}");

                Marshal.FreeHGlobal(intptr);
            }
        }
    }
}
