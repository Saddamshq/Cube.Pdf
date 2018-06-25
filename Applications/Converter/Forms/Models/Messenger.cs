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
namespace Cube.Pdf.App.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// Messenger
    ///
    /// <summary>
    /// ViewModel から View を操作するためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class Messenger : Cube.Forms.Messenger
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// SetCulture
        ///
        /// <summary>
        /// 表示言語を設定するイベントを取得します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public RelayEvent<string> SetCulture { get; } = new RelayEvent<string>();

        #endregion
    }
}
