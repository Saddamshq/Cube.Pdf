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
using System.Drawing;

namespace Cube.Pdf
{
    /* --------------------------------------------------------------------- */
    ///
    /// Page
    ///
    /// <summary>
    /// ページを表すクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Page : ObservableProperty
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// File
        ///
        /// <summary>
        /// Page オブジェクトが属する File オブジェクトを取得または
        /// 設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public File File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Number
        ///
        /// <summary>
        /// ページ番号を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Rotation
        ///
        /// <summary>
        /// ページオブジェクト表示時の回転角を度 (degree) 単位で取得
        /// または設定します。
        /// </summary>
        ///
        /// <remarks>
        /// 設定時に [0, 360) で正規化されます。
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public int Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, NormalizeDegree(value));
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Resolution
        ///
        /// <summary>
        /// 水平方法および垂直方向の解像度（1 インチあたりのピクセル数）を
        /// 取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public PointF Resolution
        {
            get => _resolution;
            set => SetProperty(ref _resolution, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Size
        ///
        /// <summary>
        /// ページオブジェクトのサイズ（ピクセル単位）を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public SizeF Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// NormalizeDegree
        ///
        /// <summary>
        /// 角度を [0, 360) の範囲に正規化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private int NormalizeDegree(int src)
        {
            var dest = src;
            while (dest <    0) dest += 360;
            while (dest >= 360) dest -= 360;
            return dest;
        }

        #endregion

        #region Fields
        private File _file;
        private int _number = -1;
        private int _rotation = 0;
        private PointF _resolution;
        private SizeF _size;
        #endregion
    }
}
