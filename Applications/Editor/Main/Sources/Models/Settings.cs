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
using System;
using System.Runtime.Serialization;

namespace Cube.Pdf.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// Settings
    ///
    /// <summary>
    /// ユーザ設定を保持するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [DataContract]
    public class Settings : ObservableProperty
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// Settings
        ///
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Settings() { Reset(); }

        #endregion

        #region Properties

        #region DataMember

        /* ----------------------------------------------------------------- */
        ///
        /// Width
        ///
        /// <summary>
        /// Gets or sets the width of main window.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DataMember]
        public int Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Height
        ///
        /// <summary>
        /// Gets or sets the height of main window.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DataMember]
        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// ItemSize
        ///
        /// <summary>
        /// Gets or sets the displayed item size.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DataMember(Name = "ViewSize")]
        public int ItemSize
        {
            get => _itemSize;
            set => SetProperty(ref _itemSize, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// FrameOnly
        ///
        /// <summary>
        /// Gets or sets the value indicating whether only the frame
        /// of each item is displayed.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DataMember]
        public bool FrameOnly
        {
            get => _frameOnly;
            set => SetProperty(ref _frameOnly, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Language
        ///
        /// <summary>
        /// Gets or sets the display language.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DataMember]
        public Language Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// CheckUpdate
        ///
        /// <summary>
        /// Gets or sets the value indicating whether checking update.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [DataMember]
        public bool CheckUpdate
        {
            get => _update;
            set => SetProperty(ref _update, value);
        }

        #endregion

        /* ----------------------------------------------------------------- */
        ///
        /// Uri
        ///
        /// <summary>
        /// Web ページの URL を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Uri Uri { get; } = new Uri("https://www.cube-soft.jp/cubepdfutility/");

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// OnDeserializing
        ///
        /// <summary>
        /// デシリアライズ直前に実行されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) => Reset();

        /* ----------------------------------------------------------------- */
        ///
        /// Reset
        ///
        /// <summary>
        /// 値をリセットします。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void Reset()
        {
            _width    = 800;
            _height   = 600;
            _itemSize = 250;
            _language = Language.Auto;
            _update   = true;
        }

        #endregion

        #region Fields
        private int _width;
        private int _height;
        private int _itemSize;
        private bool _frameOnly;
        private Language _language;
        private bool _update;
        #endregion
    }
}
