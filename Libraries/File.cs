﻿/* ------------------------------------------------------------------------- */
///
/// Copyright (c) 2010 CubeSoft, Inc.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///  http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Drawing;

namespace Cube.Pdf
{
    /* --------------------------------------------------------------------- */
    ///
    /// FileBase
    /// 
    /// <summary>
    /// ファイル情報を保持するための基底クラスです。
    /// </summary>
    /// 
    /// <remarks>
    /// FileInfo は継承できないため、FileInfo の互換クラスとして実装されて
    /// います。
    /// </remarks>
    ///
    /* --------------------------------------------------------------------- */
    public class FileBase : IEquatable<FileBase>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// FileBase
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected FileBase(string path)
        {
            _base = new System.IO.FileInfo(path);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// PageCount
        /// 
        /// <summary>
        /// ページ数を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int PageCount { get; set; } = 0;

        /* ----------------------------------------------------------------- */
        ///
        /// Resolution
        /// 
        /// <summary>
        /// ファイルの解像度を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Point Resolution { get; set; } = Point.Empty;

        #region FileInfo

        /* ----------------------------------------------------------------- */
        ///
        /// Name
        ///
        /// <summary>
        /// ファイルの名前を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Name
        {
            get { return _base.Name; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FullName
        ///
        /// <summary>
        /// ファイルの絶対パスを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string FullName
        {
            get { return _base.FullName; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// DirectoryName
        ///
        /// <summary>
        /// ディレクトリの絶対パスを表す文字列を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string DirectoryName
        {
            get { return _base.DirectoryName; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Extension
        ///
        /// <summary>
        /// ファイルの拡張子部分を表す文字列を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Extension
        {
            get { return _base.Extension; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Exists
        ///
        /// <summary>
        /// ファイルが存在するかどうかを示す値を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Exists
        {
            get { return _base.Exists; }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Length
        ///
        /// <summary>
        /// 現在のファイルのサイズをバイト単位で取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public long Length => _base.Length;

        /* ----------------------------------------------------------------- */
        ///
        /// Attributes
        ///
        /// <summary>
        /// 現在のファイルの属性を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public System.IO.FileAttributes Attributes => _base.Attributes;

        /* ----------------------------------------------------------------- */
        ///
        /// IsReadOnly
        ///
        /// <summary>
        /// 現在のファイルが読み取り専用であるかどうかを判断する値を
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool IsReadOnly => _base.IsReadOnly;

        /* ----------------------------------------------------------------- */
        ///
        /// CreationTime
        ///
        /// <summary>
        /// 現在のファイルの作成日時を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime CreationTime => _base.CreationTime;

        /* ----------------------------------------------------------------- */
        ///
        /// CreationTimeUtc
        ///
        /// <summary>
        /// 現在のファイルの作成日時を世界協定時刻 (UTC) で取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime CreationTimeUtc => _base.CreationTimeUtc;

        /* ----------------------------------------------------------------- */
        ///
        /// LastAccessTime
        ///
        /// <summary>
        /// 現在のファイルに最後にアクセスした時刻を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime LastAccessTime => _base.LastAccessTime;

        /* ----------------------------------------------------------------- */
        ///
        /// LastAccessTimeUtc
        ///
        /// <summary>
        /// 現在のファイルに最後にアクセスした時刻を世界協定時刻 (UTC) で
        /// 取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime LastAccessTimeUtc => _base.LastAccessTimeUtc;

        /* ----------------------------------------------------------------- */
        ///
        /// LastWriteTime
        ///
        /// <summary>
        /// 現在のファイルに最後に書き込みがなされた時刻を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime LastWriteTime => _base.LastWriteTime;

        /* ----------------------------------------------------------------- */
        ///
        /// LastWriteTimeUtc
        ///
        /// <summary>
        /// 現在のファイルに最後に書き込みがなされた時刻を世界協定時刻 (UTC)
        /// で取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public DateTime LastWriteTimeUtc => _base.LastWriteTimeUtc;

        /* ----------------------------------------------------------------- */
        ///
        /// RawData
        ///
        /// <summary>
        /// FileBase クラスが参照しているオブジェクトを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public System.IO.FileInfo RawData => _base;

        #endregion

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Refresh
        ///
        /// <summary>
        /// オブジェクトの状態を更新します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Refresh() => _base.Refresh();

        #region IEquatable<FileBase>

        /* ----------------------------------------------------------------- */
        ///
        /// Equals
        ///
        /// <summary>
        /// 引数に指定されたオブジェクトと等しいかどうか判別します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Equals(FileBase other)
        {
            return FullName == other.FullName;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Equals
        ///
        /// <summary>
        /// 引数に指定されたオブジェクトと等しいかどうか判別します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var other = obj as Page;
            if (other == null) return false;

            return Equals(other);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// GetHashCode
        ///
        /// <summary>
        /// 特定の型のハッシュ関数として機能します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public override int GetHashCode() => base.GetHashCode();

        #endregion

        #endregion

        #region Fields
        private System.IO.FileInfo _base;
        #endregion
    }

    /* --------------------------------------------------------------------- */
    ///
    /// File
    /// 
    /// <summary>
    /// ファイル情報を保持するためのクラスです。
    /// </summary>
    /// 
    /// <remarks>
    /// 添付予定のファイル情報を保持する際などに使用されます。
    /// </remarks>
    ///
    /* --------------------------------------------------------------------- */
    public class File : FileBase
    {
        /* ----------------------------------------------------------------- */
        ///
        /// File
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        /// 
        /// <param name="path">ファイルのパス</param>
        ///
        /* ----------------------------------------------------------------- */
        public File(string path) : base(path) { }
    }

    /* --------------------------------------------------------------------- */
    ///
    /// PdfFile
    /// 
    /// <summary>
    /// PDF のファイル情報を保持するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class PdfFile : FileBase
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// File
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PdfFile(string path) : this(path, string.Empty) { }

        /* ----------------------------------------------------------------- */
        ///
        /// File
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PdfFile(string path, string password) : base(path)
        {
            Password = password;
            Resolution = new Point(72, 72);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Password
        /// 
        /// <summary>
        /// オーナパスワードまたはユーザパスワードを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public string Password { get; set; } = string.Empty;

        /* ----------------------------------------------------------------- */
        ///
        /// FullAccess
        /// 
        /// <summary>
        /// ファイルの全ての内容にアクセス可能かどうかを示す値を取得または
        /// 設定します。
        /// </summary>
        /// 
        /// <remarks>
        /// このプロパティは、PDF ファイルにパスワードによって暗号化されて
        /// おり、かつユーザパスワードを用いてファイルを開いた場合 false に
        /// 設定されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public bool FullAccess { get; set; } = false;

        #endregion
    }

    /* --------------------------------------------------------------------- */
    ///
    /// ImageFile
    /// 
    /// <summary>
    /// 画像ファイルの情報を保持するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ImageFile : FileBase
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// ImageFile
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageFile(string path) : base(path)
        {
            using (var image = Image.FromFile(path))
            {
                InitializeValues(image);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ImageFile
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public ImageFile(string path, Image image) : base(path)
        {
            InitializeValues(image);
        }

        #endregion

        #region Others

        /* ----------------------------------------------------------------- */
        ///
        /// InitializeValues
        /// 
        /// <summary>
        /// 各種プロパティを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InitializeValues(Image image)
        {
            var guid = image.FrameDimensionsList[0];
            var dim = new System.Drawing.Imaging.FrameDimension(guid);
            PageCount = image.GetFrameCount(dim);
            Resolution = new Point((int)image.HorizontalResolution, (int)image.VerticalResolution);
        }

        #endregion
    }
}
