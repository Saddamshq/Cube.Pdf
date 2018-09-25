﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
/* ------------------------------------------------------------------------- */
using Cube.Collections.Mixin;
using Cube.FileSystem;
using Cube.Log;
using Cube.Pdf.Itext;
using Cube.Pdf.Mixin;
using Cube.Xui.Converters;
using Cube.Xui.Mixin;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Cube.Pdf.App.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentExtension
    ///
    /// <summary>
    /// Represents the extended methods to handle the PDF document.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    internal static class DocumentExtension
    {
        #region Methods

        #region Create

        /* ----------------------------------------------------------------- */
        ///
        /// Create
        ///
        /// <summary>
        /// Create a new instance of the ImageSource class with the
        /// specified parameters.
        /// </summary>
        ///
        /// <param name="src">Renderer object.</param>
        /// <param name="page">Page object.</param>
        /// <param name="ratio">Scale ratio.</param>
        ///
        /// <returns>ImageSource object.</returns>
        ///
        /* ----------------------------------------------------------------- */
        public static ImageSource Create(this IDocumentRenderer src, Page page, double ratio)
        {
            if (src == null || page == null) return null;
            var size = page.GetDisplaySize(ratio).Value;
            return src.Create(new Bitmap((int)size.Width, (int)size.Height), page);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Create
        ///
        /// <summary>
        /// Create a new instance of the ImageSource class with the
        /// specified parameters.
        /// </summary>
        ///
        /// <param name="src">Renderer object.</param>
        /// <param name="entry">Information of the creating image.</param>
        ///
        /// <returns>ImageSource object.</returns>
        ///
        /* ----------------------------------------------------------------- */
        public static ImageSource Create(this IDocumentRenderer src, ImageItem entry) =>
            src?.Create(new Bitmap(entry.Width, entry.Height), entry.RawObject);

        /* ----------------------------------------------------------------- */
        ///
        /// Create
        ///
        /// <summary>
        /// Create a new instance of the ImageSource class with the
        /// specified parameters.
        /// </summary>
        ///
        /// <param name="src">Renderer object.</param>
        /// <param name="dest">Image object.</param>
        /// <param name="page">Page object.</param>
        ///
        /// <returns>ImageSource object.</returns>
        ///
        /* ----------------------------------------------------------------- */
        private static ImageSource Create(this IDocumentRenderer src, Image dest, Page page)
        {
            using (var gs = Graphics.FromImage(dest))
            {
                gs.Clear(System.Drawing.Color.White);
                src.Render(gs, page);
            }
            return dest.ToBitmapImage(true);
        }

        #endregion

        #region Invoke

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Invokes the user action and clears the message.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Invoke(this MainFacade src, Action action) =>
            src.Invoke(action, string.Empty);

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Invokes the user action and registers the hisotry item.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Invoke(this MainFacade src, Func<HistoryItem> func) =>
            src.Invoke(() => src.Bindable.History.Register(func()));

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Invokes the user action and sets the result message.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static void Invoke(this MainFacade src, Action action, string format, params object[] args)
        {
            try
            {
                src.Bindable.Busy.Value = true;
                action();
                src.Bindable.SetMessage(format, args);
            }
            catch (OperationCanceledException) { /* ignore user cancel */ }
            catch (Exception err) { src.Bindable.SetMessage(err.Message); throw; }
            finally
            {
                src.Bindable.Modified.Raise();
                src.Bindable.Count.Raise();
                src.Bindable.Busy.Value = false;
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Invokes the specified action and creates a history item.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private static HistoryItem Invoke(Action forward, Action reverse)
        {
            forward(); // do
            return new HistoryItem { Undo = reverse, Redo = forward };
        }

        #endregion

        #region Metadata

        /* ----------------------------------------------------------------- */
        ///
        /// GetMetadata
        ///
        /// <summary>
        /// Gets the current Metadata object.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        ///
        /// <returns>Metadata object.</returns>
        ///
        /* ----------------------------------------------------------------- */
        public static Metadata GetMetadata(this MainFacade src)
        {
            if (src.Bindable.Source.Value != null &&
                src.Bindable.Metadata.Value == null) src.LoadMetadata();
            return src.Bindable.Metadata.Value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetMetadata
        ///
        /// <summary>
        /// Sets the Metadata object.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        /// <param name="value">Metadata object.</param>
        ///
        /// <returns>
        /// History item to execute undo and redo actions.
        /// </returns>
        ///
        /* ----------------------------------------------------------------- */
        public static HistoryItem SetMetadata(this MainFacade src, Metadata value)
        {
            var prev = src.Bindable.Metadata.Value;
            return Invoke(
                () => src.Bindable.Metadata.Value = value,
                () => src.Bindable.Metadata.Value = prev
            );
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetEncryption
        ///
        /// <summary>
        /// Gets the current Encryption object.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        ///
        /// <returns>Metadata object.</returns>
        ///
        /* ----------------------------------------------------------------- */
        public static Encryption GetEncryption(this MainFacade src)
        {
            if (src.Bindable.Source.Value != null &&
                src.Bindable.Encryption.Value == null) src.LoadMetadata();
            return src.Bindable.Encryption.Value;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SetEncryption
        ///
        /// <summary>
        /// Sets the Encryption object.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        /// <param name="value">Encryption object.</param>
        ///
        /// <returns>
        /// History item to execute undo and redo actions.
        /// </returns>
        ///
        /* ----------------------------------------------------------------- */
        public static HistoryItem SetEncryption(this MainFacade src, Encryption value)
        {
            var prev = src.Bindable.Encryption.Value;
            return Invoke(
                () => src.Bindable.Encryption.Value = value,
                () => src.Bindable.Encryption.Value = prev
            );
        }

        /* ----------------------------------------------------------------- */
        ///
        /// LoadMetadata
        ///
        /// <summary>
        /// Loads metadata of the current PDF document.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        ///
        /* ----------------------------------------------------------------- */
        private static void LoadMetadata(this MainFacade src) => src.Invoke(() =>
        {
            try
            {
                var data = src.Bindable;
                data.SetMessage(Properties.Resources.MessageLoadingMetadata);

                using (var r = GetReader(data.Source.Value, src.Settings.IO))
                {
                    if (data.Metadata.Value   == null) data.Metadata.Value   = r.Metadata;
                    if (data.Encryption.Value == null) data.Encryption.Value = r.Encryption;
                }
            }
            catch (Exception err) { src.LogWarn(err.ToString(), err); }
        });

        #endregion

        #region Save

        /* ----------------------------------------------------------------- */
        ///
        /// Overwrite
        ///
        /// <summary>
        /// Overwrites the PDF document.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void Overwrite(this MainFacade src)
        {
            if (src.Bindable.History.Undoable) src.Save(src.Bindable.Source.Value.FullName);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        ///
        /// <summary>
        /// Save the PDF document
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        /// <param name="dest">Saving file information.</param>
        /// <param name="close">Close action.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void Save(this MainFacade src, Information dest, Action close)
        {
            var io  = src.Settings.IO;
            var tmp = io.Combine(dest.DirectoryName, Guid.NewGuid().ToString("D"));

            try
            {
                var data   = src.Bindable;
                var reader = GetReader(data.Source.Value, io);

                if (data.Metadata.Value   == null) data.Metadata.Value   = reader.Metadata;
                if (data.Encryption.Value == null) data.Encryption.Value = reader.Encryption;

                using (var writer = new DocumentWriter())
                {
                    writer.Add(data.Images.Select(e => e.RawObject), reader);
                    writer.Set(data.Metadata.Value);
                    writer.Set(data.Encryption.Value);
                    writer.Save(tmp);
                }

                close();
                io.Copy(tmp, dest.FullName, true);
            }
            finally { io.TryDelete(tmp); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Restruct
        ///
        /// <summary>
        /// Restructs some properties with the specified new PDF document.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        /// <param name="doc">New PDF document.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void Restruct(this MainFacade src, IDocumentReader doc)
        {
            var items = doc.Pages.Select((v, i) => new { Value = v, Index = i });
            foreach (var e in items) src.Bindable.Images[e.Index].RawObject = e.Value;
            src.Bindable.Source.Value = doc.File;
            src.Bindable.History.Clear();
        }

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// Insert
        ///
        /// <summary>
        /// Inserts the page objects of the specified file path.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        /// <param name="path">File path.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void Insert(this MainFacade src, string path) =>
            src.Insert(src.Bindable.Selection.Last + 1, path);

        /* ----------------------------------------------------------------- */
        ///
        /// Select
        ///
        /// <summary>
        /// Sets or resets the IsSelected property of all items according
        /// to the current condition.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void Select(this MainFacade src) =>
            src.Select(src.Bindable.Selection.Count < src.Bindable.Images.Count);

        /* ----------------------------------------------------------------- */
        ///
        /// Zoom
        ///
        /// <summary>
        /// Executes the Zoom command by using the current settings.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void Zoom(this MainFacade src)
        {
            var items = src.Bindable.Images.Preferences.ItemSizeOptions;
            var prev  = src.Bindable.Images.Preferences.ItemSizeIndex;
            var next  = items.LastIndexOf(x => x <= src.Settings.Value.ItemSize);
            src.Zoom(next - prev);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// StartProcess
        ///
        /// <summary>
        /// Starts a new process with the specified arguments.
        /// </summary>
        ///
        /// <param name="src">Facade object.</param>
        /// <param name="args">User arguments.</param>
        ///
        /* ----------------------------------------------------------------- */
        public static void StartProcess(this MainFacade src, string args) =>
            Process.Start(new ProcessStartInfo
        {
            FileName  = Assembly.GetExecutingAssembly().Location,
            Arguments = args
        });

        /* ----------------------------------------------------------------- */
        ///
        /// GetReader
        ///
        /// <summary>
        /// Gets the DocumentReader of the specified file.
        /// </summary>
        ///
        /// <remarks>
        /// Partial モードは必ず無効にする必要があります。有効にした場合、
        /// ページ回転情報が正常に適用されない可能性があります。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private static DocumentReader GetReader(Information src, IO io) =>
            new DocumentReader(src.FullName, src is PdfFile f ? f.Password : "", false, io);

        #endregion
    }
}
