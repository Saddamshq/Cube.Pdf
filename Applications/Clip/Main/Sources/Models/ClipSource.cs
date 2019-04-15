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
using Cube.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cube.Pdf.Clip
{
    /* --------------------------------------------------------------------- */
    ///
    /// ClipSource
    ///
    /// <summary>
    /// PDF ファイルおよび添付ファイル一覧を管理するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ClipSource : ObservableProperty, IDisposable
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Source
        ///
        /// <summary>
        /// 添付元の PDF ファイルを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IDocumentReader Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IO
        ///
        /// <summary>
        /// I/O オブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IO IO { get; } = new IO();

        /* ----------------------------------------------------------------- */
        ///
        /// Clips
        ///
        /// <summary>
        /// 添付ファイル一覧を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ObservableCollection<ClipItem> Clips { get; } =
            new ObservableCollection<ClipItem>();

        #endregion

        #region Events

        /* ----------------------------------------------------------------- */
        ///
        /// Locked
        ///
        /// <summary>
        /// ファイルが他のプロセスによって使用されているなどの理由で
        /// ロックされている時に発生するイベントです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public event ValueCancelEventHandler<string> Locked;

        /* ----------------------------------------------------------------- */
        ///
        /// OnLocked
        ///
        /// <summary>
        /// Locked イベントを発生させます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void OnLocked(ValueCancelEventArgs<string> e)
        {
            if (Locked != null) Locked(this, e);
            else e.Cancel = true;
        }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Open
        ///
        /// <summary>
        /// PDF ファイルを読み込みます。
        /// </summary>
        ///
        /// <param name="src">PDF ファイルのパス</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Open(string src)
        {
            while (IsLocked(src))
            {
                var e = ValueEventArgs.Create(src, false);
                OnLocked(e);
                if (e.Cancel) return;
            }

            Close();
            Source = new Cube.Pdf.Itext.DocumentReader(src, "", true, IO);
            Reset();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        ///
        /// <summary>
        /// ファイルの添付状況を PDF ファイルを読み込んだ直後の状態に
        /// リセットします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Reset()
        {
            Clips.Clear();
            var msg = Properties.Resources.ConditionEmbedded;
            foreach (var item in Source.Attachments)
            {
                Clips.Add(new ClipItem(item) { Condition = msg });
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Save
        ///
        /// <summary>
        /// PDF ファイルを上書き保存します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Save()
        {
            var dest  = Source.File.FullName;
            var tmp   = System.IO.Path.GetTempFileName();
            var items = Clips.Select(e => e.RawObject).Where(e => IO.Exists(e.Source));

            using (var writer = new Cube.Pdf.Itext.DocumentWriter())
            {
                writer.UseSmartCopy = true;
                writer.Set(Source.Metadata);
                writer.Set(Source.Encryption);
                writer.Add(Source.Pages);
                writer.Add(items);

                IO.TryDelete(tmp);
                writer.Save(tmp);
            }

            Close();
            IO.Copy(tmp, dest, true);
            IO.TryDelete(tmp);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Attach
        ///
        /// <summary>
        /// 新しいファイルを添付します。
        /// </summary>
        ///
        /// <param name="files">添付ファイル一覧</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Attach(IEnumerable<string> files)
        {
            foreach (var file in files) Attach(file);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Attach
        ///
        /// <summary>
        /// 新しいファイルを添付します。
        /// </summary>
        ///
        /// <param name="file">添付ファイル</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Attach(string file)
        {
            if (Clips.Any(e => e.RawObject.Source == file)) return;

            while (IsLocked(file))
            {
                var e = ValueEventArgs.Create(file, false);
                OnLocked(e);
                if (e.Cancel) return;
            }

            Clips.Insert(0, new ClipItem(new Attachment(file, IO))
            {
                Condition = Properties.Resources.ConditionNew
            });
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Detach
        ///
        /// <summary>
        /// 添付ファイルを削除します。
        /// </summary>
        ///
        /// <param name="indices">
        /// 削除する添付ファイルのインデックス一覧
        /// </param>
        ///
        /* ----------------------------------------------------------------- */
        public void Detach(IEnumerable<int> indices)
        {
            foreach (var index in indices.OrderByDescending(x => x))
            {
                Detach(index);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Detach
        ///
        /// <summary>
        /// 添付ファイルを削除します。
        /// </summary>
        ///
        /// <param name="index">削除する添付ファイルのインデックス</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Detach(int index)
        {
            if (index < 0 || index >= Clips.Count) return;
            Clips.RemoveAt(index);
        }

        #region IDisposable

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        ///
        /// <summary>
        /// リソースを開放します。
        /// </summary>
        ///
        /// <remarks>
        /// クリーンアップコードを Dispose(bool) に記述します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public void Dispose()
        {
            Dispose(true);

            // TODO: ファイナライザーがオーバーライドされる場合は、
            // 次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        ///
        /// <summary>
        /// リソースを開放します。
        /// </summary>
        ///
        /// <param name="disposing">
        /// マネージオブジェクトを開放するかどうかを表す値
        /// </param>
        ///
        /* ----------------------------------------------------------------- */
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) Close();
                _disposed = true;
            }
        }

        #endregion

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        ///
        /// <summary>
        /// PDF ファイルを閉じます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Close()
        {
            Clips.Clear();
            Source.Dispose();
            Source = null;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// IsLocked
        ///
        /// <summary>
        /// ファイルがロックされているかどうかを判別します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private bool IsLocked(string path)
        {
            try
            {
                using (new System.IO.FileStream(path,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.ReadWrite,
                    System.IO.FileShare.None)) return false;
            }
            catch (System.IO.IOException) { return true; }
            catch (System.Security.SecurityException) { return true; }
            catch (UnauthorizedAccessException) { return true; }
            catch { return true; }
        }

        #endregion

        #region Fields
        private bool _disposed = false;
        private IDocumentReader _source = null;
        #endregion
    }
}
