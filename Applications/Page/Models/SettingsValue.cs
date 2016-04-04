﻿/* ------------------------------------------------------------------------- */
///
/// SettingsValue.cs
///
/// Copyright (c) 2010 CubeSoft, Inc.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
///
/* ------------------------------------------------------------------------- */
using System.Reflection;

namespace Cube.Pdf.App.Page
{
    /* --------------------------------------------------------------------- */
    ///
    /// SettingsValue
    ///
    /// <summary>
    /// 各種設定を保持するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class SettingsValue : ObservableSettingsValue
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// SettingsValue
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public SettingsValue()
        {
            Assembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Assembly
        /// 
        /// <summary>
        /// アセンブリ情報を取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Assembly Assembly { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// AllowOperation
        /// 
        /// <summary>
        /// ユーザからの操作を受け付けるかどうかを示す値を取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool AllowOperation
        {
            get { return _allowOperation; }
            set
            {
                if (_allowOperation == value) return;
                _allowOperation = value;
                RaisePropertyChanged(nameof(AllowOperation));
            }
        }

        #endregion

        #region Fields
        private bool _allowOperation = true;
        #endregion
    }
}
