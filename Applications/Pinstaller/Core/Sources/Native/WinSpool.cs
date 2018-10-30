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
using System;
using System.Runtime.InteropServices;

namespace Cube.Pdf.App.Pinstaller
{
    /* --------------------------------------------------------------------- */
    ///
    /// NativeMethods
    ///
    /// <summary>
    /// Represents declarations in the winspool.drv.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    internal static class NativeMethods
    {
        #region Methods

        #region PortMonitor

        /* ----------------------------------------------------------------- */
        ///
        /// EnumMonitors
        ///
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/printdocs/enummonitors
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DllImport(LibName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnumMonitors(
            string pName,
            uint Level,
            IntPtr pMonitors,
            uint cbBuf,
            ref uint pcbNeeded,
            ref uint pcReturned
        );

        /* ----------------------------------------------------------------- */
        ///
        /// AddMonitor
        ///
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/printdocs/addmonitor
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DllImport(LibName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool AddMonitor(
            string pName,
            uint Level,
            ref MonitorInfo2 pMonitors
        );

        /* ----------------------------------------------------------------- */
        ///
        /// DeleteMonitor
        ///
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/printdocs/deletemonitor
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DllImport(LibName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteMonitor(
            string pName,
            string pEnvironment,
            string pMonitorName
        );

        #endregion

        #region PrinterDriver

        /* ----------------------------------------------------------------- */
        ///
        /// EnumPrinterDrivers
        ///
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/printdocs/enumprinterdrivers
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DllImport(LibName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnumPrinterDrivers(
            string pName,
            string pEnvironment,
            uint Level,
            IntPtr pDriverInfo,
            uint cbBuf,
            ref uint pcbNeeded,
            ref uint pcReturned
        );

        /* ----------------------------------------------------------------- */
        ///
        /// AddPrinterDriver
        ///
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/printdocs/addprinterdriver
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DllImport(LibName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool AddPrinterDriver(
            string pName,
            uint Level,
            ref DriverInfo3 pDriverInfo
        );

        /* ----------------------------------------------------------------- */
        ///
        /// DeletePrinterDriver
        ///
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/desktop/printdocs/deleteprinterdriver
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DllImport(LibName, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeletePrinterDriver(
            string pName,
            string pEnvironment,
            string pDriverName
        );

        #endregion

        #endregion

        #region Fields
        private const string LibName = "winspool.drv";
        #endregion
    }
}
