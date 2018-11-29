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
using Cube.Collections;
using Cube.FileSystem;
using Cube.Generics;
using Cube.Log;
using Cube.Pdf.Ghostscript;
using Cube.Pdf.Mixin;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cube.Pdf.App.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// SettingsFolder
    ///
    /// <summary>
    /// 各種設定を保持するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class SettingsFolder : SettingsFolder<Settings>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// SettingsFolder
        ///
        /// <summary>
        /// Initializes static fields.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        static SettingsFolder()
        {
            Locale.Configure(e =>
            {
                var src = e.ToCultureInfo();
                var cmp = Properties.Resources.Culture?.Name;
                var opt = StringComparison.InvariantCultureIgnoreCase;
                if (cmp.HasValue() && cmp.Equals(src.Name, opt)) return false;
                Properties.Resources.Culture = src;
                return true;
            });
        }

        /* ----------------------------------------------------------------- */
        ///
        /// SettingsFolder
        ///
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public SettingsFolder() : this(
            Cube.DataContract.Format.Registry,
            @"CubeSoft\CubePDF\v2",
            new IO()
        ) { }

        /* ----------------------------------------------------------------- */
        ///
        /// SettingsFolder
        ///
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /// <param name="format">設定情報の保存方法</param>
        /// <param name="path">設定情報の保存パス</param>
        /// <param name="io">I/O オブジェクト</param>
        ///
        /* ----------------------------------------------------------------- */
        public SettingsFolder(Cube.DataContract.Format format, string path, IO io) :
            base(System.Reflection.Assembly.GetExecutingAssembly(), format, path, io)
        {
            AutoSave       = false;
            MachineName    = Environment.MachineName;
            UserName       = Environment.UserName;
            DocumentName   = new DocumentName(string.Empty, Assembly.Product, IO);
            WorkDirectory  = GetWorkDirectory();
            Version.Digit  = 3;
            Version.Suffix = $"RC{Assembly.Version.Revision}";
            UpdateProgram  = IO.Combine(IO.Get(Assembly.Location).DirectoryName, "UpdateChecker.exe");

        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Uri
        ///
        /// <summary>
        /// Web ページの URL を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Uri Uri { get; } = new Uri("https://www.cube-soft.jp/cubepdf/");

        /* ----------------------------------------------------------------- */
        ///
        /// DocumentName
        ///
        /// <summary>
        /// ドキュメント名を取得します。
        /// </summary>
        ///
        /// <remarks>
        /// 主に仮想プリンタ経由時に指定されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public DocumentName DocumentName { get; private set; }

        /* ----------------------------------------------------------------- */
        ///
        /// MachineName
        ///
        /// <summary>
        /// 端末名を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string MachineName { get; private set; }

        /* ----------------------------------------------------------------- */
        ///
        /// MachineName
        ///
        /// <summary>
        /// ユーザ名を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string UserName { get; private set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Digest
        ///
        /// <summary>
        /// Gets the SHA-256 message digest of the source file that
        /// specified at command line.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Digest { get; private set; }

        /* ----------------------------------------------------------------- */
        ///
        /// WorkDirectory
        ///
        /// <summary>
        /// 作業ディレクトリのパスを取得または設定します。
        /// </summary>
        ///
        /// <remarks>
        /// Ghostscript はパスにマルチバイト文字が含まれる場合、処理に
        /// 失敗する場合があります。そのため、マルチバイト文字の含まれない
        /// ディレクトリに移動して処理を実行します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public string WorkDirectory { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// UpdateProgram
        ///
        /// <summary>
        /// アップデート確認用プログラムのパスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string UpdateProgram { get; }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Set
        ///
        /// <summary>
        /// プログラム引数の内容を設定します。
        /// </summary>
        ///
        /// <param name="args">プログラム引数</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Set(string[] args)
        {
            var src = new ArgumentCollection(args, '/', true);
            var op  = src.Options;

            if (TryGet(op, nameof(MachineName), out var pc)) MachineName = pc;
            if (TryGet(op, nameof(UserName), out var user)) UserName = user;
            if (TryGet(op, nameof(DocumentName), out var doc)) DocumentName = new DocumentName(doc, Assembly.Product, IO);
            if (TryGet(op, nameof(Digest), out var digest)) Digest = digest;
            if (TryGet(op, "InputFile", out var input)) Value.Source = input;

            var dest = IO.Get(IO.Combine(Value.Destination, DocumentName.Name));
            var name = dest.NameWithoutExtension;
            var ext  = Value.Format.GetExtension();

            Value.Destination  = IO.Combine(dest.DirectoryName, $"{name}{ext}");
            Value.DeleteSource = op.ContainsKey("deleteonclose");
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CheckUpdate
        ///
        /// <summary>
        /// アップデートの確認を実行します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void CheckUpdate()
        {
            try
            {
                if (!Value.CheckUpdate) return;
                var time = GetLastCheckUpdate();
                this.LogDebug($"LastCheckUpdate:{time}");
                if (time.AddDays(1) < DateTime.Now) Process.Start(UpdateProgram, Assembly.Product);
            }
            catch (Exception err) { this.LogWarn($"{nameof(CheckUpdate)}:{err}", err); }
        }

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// OnLoaded
        ///
        /// <summary>
        /// 読み込み時に実行されます。
        /// </summary>
        ///
        /// <remarks>
        /// 1.0.0RC12 より Resolution を ComboBox のインデックスに対応
        /// する値から直接の値に変更しました。これに伴い、インデックスを
        /// 指していると予想される値を初期値にリセットしています。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnLoaded(ValueChangedEventArgs<Settings> e)
        {
            e.NewValue.Format = NormalizeFormat(e.NewValue);
            e.NewValue.Resolution = NormalizeResolution(e.NewValue);
            e.NewValue.Orientation = NormalizeOrientation(e.NewValue);
            e.NewValue.Destination = NormalizeDestination(e.NewValue);
            e.NewValue.Metadata.Creator = Assembly.Product;
            e.NewValue.Metadata.Viewer = ViewerPreferences.OneColumn;
            e.NewValue.Encryption.Deny();
            e.NewValue.Encryption.Permission.Accessibility = PermissionValue.Allow;

            base.OnLoaded(e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnSaved
        ///
        /// <summary>
        /// 保存時に実行されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnSaved(KeyValueEventArgs<Cube.DataContract.Format, string> e)
        {
            try
            {
                if (Value == null) return;

                new Startup("cubepdf-checker")
                {
                    Command = $"{UpdateProgram.Quote()} {Assembly.Product}",
                    Enabled = Value.CheckUpdate,
                }.Save();
            }
            finally { base.OnSaved(e); }
        }

        #region Get

        /* ----------------------------------------------------------------- */
        ///
        /// GetWorkDirectory
        ///
        /// <summary>
        /// 作業ディレクトリのパスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetWorkDirectory()
        {
            var str   = GetString(Registry.LocalMachine, "LibPath");
            var root  = str.HasValue() ?
                        str :
                        IO.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            Assembly.Company, Assembly.Product
                        );
            return IO.Combine(root, Guid.NewGuid().ToString("D"));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetLastCheckUpdate
        ///
        /// <summary>
        /// 最後にアップデートの更新を実行した日時を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private DateTime GetLastCheckUpdate()
        {
            var str = GetString(Registry.CurrentUser, "LastCheckUpdate");
            return str.HasValue() ?
                   DateTime.Parse(str).ToLocalTime() :
                   DateTime.MinValue;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetString
        ///
        /// <summary>
        /// レジストリから文字列を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private string GetString(RegistryKey root, string name)
        {
            var keyname = $@"Software\{Assembly.Company}\{Assembly.Product}";
            using (var key = root.OpenSubKey(keyname, false)) return key?.GetValue(name) as string;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TryGet
        ///
        /// <summary>
        /// Tries to get the value corresponding to the specified name.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private bool TryGet(IDictionary<string, string> src, string name, out string dest) =>
            src.TryGetValue(name.ToLowerInvariant(), out dest);

        #endregion

        #region Normalize

        /* ----------------------------------------------------------------- */
        ///
        /// NormalizeFormat
        ///
        /// <summary>
        /// Format の値を正規化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Format NormalizeFormat(Settings src) =>
            ViewResource.Formats.Any(e => e.Value == src.Format) ?
            src.Format :
            Ghostscript.Format.Pdf;

        /* ----------------------------------------------------------------- */
        ///
        /// NormalizeOrientation
        ///
        /// <summary>
        /// Orientation の値を正規化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private Orientation NormalizeOrientation(Settings src) =>
            ViewResource.Orientations.Any(e => e.Value == src.Orientation) ?
            src.Orientation :
            Orientation.Auto;

        /* ----------------------------------------------------------------- */
        ///
        /// NormalizeResolution
        ///
        /// <summary>
        /// Resolution の値を正規化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private int NormalizeResolution(Settings src) =>
            src.Resolution >= 72 ?
            src.Resolution :
            600;

        /* ----------------------------------------------------------------- */
        ///
        /// NormalizeDestination
        ///
        /// <summary>
        /// 保存パスを正規化します。
        /// </summary>
        ///
        /// <remarks>
        /// パスにファイル名が残っている場合、ファイル名部分を除去します。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        private string NormalizeDestination(Settings src)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            try
            {
                if (!src.Destination.HasValue()) return desktop;
                var dest = IO.Get(src.Destination);
                return dest.IsDirectory ? dest.FullName : dest.DirectoryName;
            }
            catch (Exception err)
            {
                this.LogWarn(err.ToString(), err);
                return desktop;
            }
        }

        #endregion

        #endregion
    }
}
