﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/* ------------------------------------------------------------------------- */
using Cube.Generics;
using Cube.Iteration;
using Cube.Pdf.Pinstaller.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Cube.Pdf.Pinstaller
{
    /* --------------------------------------------------------------------- */
    ///
    /// PortMonitor
    ///
    /// <summary>
    /// Provides functionality to install or uninstall a port monitor.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class PortMonitor : IInstallable
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// PortMonitor
        ///
        /// <summary>
        /// Initializes a new instance of the PortMonitor class with the
        /// specified name.
        /// </summary>
        ///
        /// <param name="name">Name of the port monitor.</param>
        ///
        /* ----------------------------------------------------------------- */
        public PortMonitor(string name) : this(name, GetElements()) { }

        /* ----------------------------------------------------------------- */
        ///
        /// PortMonitor
        ///
        /// <summary>
        /// Initializes a new instance of the PortMonitor class with the
        /// specified name.
        /// </summary>
        ///
        /// <param name="name">Name of the port monitor.</param>
        /// <param name="elements">
        /// Collection of installed port monitors.
        /// </param>
        ///
        /* ----------------------------------------------------------------- */
        public PortMonitor(string name, IEnumerable<PortMonitor> elements) : this(new MonitorInfo2())
        {
            var obj = elements.FirstOrDefault(e => e.Name.FuzzyEquals(name));
            Exists = obj != null;
            if (Exists) _core = obj._core;
            else
            {
                Name = name;
                Environment = this.GetEnvironment();
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// PortMonitor
        ///
        /// <summary>
        /// Initializes a new instance of the PortMonitor class.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private PortMonitor(MonitorInfo2 core)
        {
            _core = core;
            TargetDirectory = System.Environment.SpecialFolder.System.GetName();
            RetryCount = 5;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Name
        ///
        /// <summary>
        /// Gets the name of the port monitor.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Name
        {
            get => _core.pName;
            private set => _core.pName = value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FileName
        ///
        /// <summary>
        /// Gets or sets the DLL name of the port monitor.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FileName
        {
            get => _core.pDLLName;
            set => _core.pDLLName = value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Config
        ///
        /// <summary>
        /// Gets or sets the name of UI config file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Config { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Environment
        ///
        /// <summary>
        /// Gets the name of architecture (Windows NT x86 or Windows x64).
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Environment
        {
            get => _core.pEnvironment;
            private set => _core.pEnvironment = value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Exists
        ///
        /// <summary>
        /// Gets the value indicating whether the port monitor has been
        /// already installed.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Exists { get; private set; }

        /* ----------------------------------------------------------------- */
        ///
        /// TargetDirectory
        ///
        /// <summary>
        /// Gets the default path that monitor resources are installed.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string TargetDirectory { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// RetryCount
        ///
        /// <summary>
        /// Gets or sets the maximum number of attempts for an installation
        /// or uninstallation operation to succeed.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int RetryCount { get; set; }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetElements
        ///
        /// <summary>
        /// Gets the collection of currently installed port monitors.
        /// </summary>
        ///
        /// <returns>Collection of port monitors.</returns>
        ///
        /* ----------------------------------------------------------------- */
        public static IEnumerable<PortMonitor> GetElements()
        {
            if (GetEnumApi(IntPtr.Zero, 0, out var bytes, out _)) return new PortMonitor[0];
            if (Marshal.GetLastWin32Error() != 122) throw new Win32Exception();

            var ptr = Marshal.AllocHGlobal((int)bytes);
            try
            {
                if (GetEnumApi(ptr, bytes, out _, out var n)) return Convert(ptr, n);
                else throw new Win32Exception();
            }
            finally { Marshal.FreeHGlobal(ptr); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CanInstall
        ///
        /// <summary>
        /// Determines that the port monitor can be installed.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool CanInstall() => Name.HasValue() && FileName.HasValue();

        /* ----------------------------------------------------------------- */
        ///
        /// Install
        ///
        /// <summary>
        /// Installs the port monitor.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Install()
        {
            this.Log();

            if (!Exists && CanInstall()) this.Log(() => this.Try(RetryCount, () =>
            {
                if (!NativeMethods.AddMonitor(null, 2u, ref _core)) throw new Win32Exception();
                Exists = true;
            }));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Uninstall
        ///
        /// <summary>
        /// Uninstalls the port monitor.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Uninstall()
        {
            this.Log();

            if (Exists) this.Log(() => this.Try(RetryCount, () =>
            {
                if (!NativeMethods.DeleteMonitor(null, Environment, Name)) throw new Win32Exception();
                Exists = false;
            }));
        }

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// GetEnumApi
        ///
        /// <summary>
        /// Executes the API of gettings currently installed port
        /// monitors.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static bool GetEnumApi(IntPtr src, uint n, out uint bytes, out uint count) =>
            NativeMethods.EnumMonitors("", 2, src, n, out bytes, out count);

        /* ----------------------------------------------------------------- */
        ///
        /// Convert
        ///
        /// <summary>
        /// Converts unmanaged resources to the PortMonitor collection.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static IEnumerable<PortMonitor> Convert(IntPtr src, uint n)
        {
            var dest = new List<PortMonitor>();
            var ptr  = src;

            for (var i = 0; i < n; ++i)
            {
                var e = (MonitorInfo2)Marshal.PtrToStructure(ptr, typeof(MonitorInfo2));
                dest.Add(new PortMonitor(e) { Exists = true });
                ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(MonitorInfo2)));
            }

            return dest;
        }

        #endregion

        #region Fields
        private MonitorInfo2 _core;
        #endregion
    }
}
