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
using Cube.Xui;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;

namespace Cube.Pdf.App.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// InsertViewModel
    ///
    /// <summary>
    /// Represents the ViewModel for a InsertWindow instance.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class InsertViewModel : DialogViewModel
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// InsertViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the InsertViewModel with the
        /// specified argumetns.
        /// </summary>
        ///
        /// <param name="n">Number of pages.</param>
        /// <param name="context">Synchronization context.</param>
        ///
        /* ----------------------------------------------------------------- */
        public InsertViewModel(int n, SynchronizationContext context) :
            base(() => Properties.Resources.TitleInsert, new Messenger(), context)
        {
            PageCount = new BindableElement<string>(
                () => string.Format(Properties.Resources.MessagePage, n),
                e  => throw new InvalidOperationException(),
                () => Properties.Resources.MenuPageCount
            );
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// PageCount
        ///
        /// <summary>
        /// Gets the menu that represents the number of pages.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> PageCount { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Position
        ///
        /// <summary>
        /// Gets the label that represents the insert position.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Position { get; } = new BindableElement(
            () => Properties.Resources.MenuInsertPosition
        );

        /* ----------------------------------------------------------------- */
        ///
        /// First
        ///
        /// <summary>
        /// Gets the menu that represents the begging of the document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement First { get; } = new BindableElement(
            () => Properties.Resources.MenuPositionFirst
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Last
        ///
        /// <summary>
        /// Gets the menu that represents the end of the document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Last { get; } = new BindableElement(
            () => Properties.Resources.MenuPositionLast
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Selected
        ///
        /// <summary>
        /// Gets the menu that represents the selected position of the
        /// document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Selected { get; } = new BindableElement(
            () => Properties.Resources.MenuPositionSelected
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Specified
        ///
        /// <summary>
        /// Gets the menu that represents the user specified position
        /// of the document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Specified { get; } = new BindableElement(
            () => Properties.Resources.MenuPositionSpecified
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Add
        ///
        /// <summary>
        /// Gets the menu that represents the add button.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Add { get; } = new BindableElement(
            () => Properties.Resources.MenuAdd
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        ///
        /// <summary>
        /// Gets the menu that represents the remove button.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Remove { get; } = new BindableElement(
            () => Properties.Resources.MenuRemove
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        ///
        /// <summary>
        /// Gets the menu that represents the clear button.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Clear { get; } = new BindableElement(
            () => Properties.Resources.MenuClear
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Up
        ///
        /// <summary>
        /// Gets the menu that represents the up button.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Up { get; } = new BindableElement(
            () => Properties.Resources.MenuUp
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Down
        ///
        /// <summary>
        /// Gets the menu that represents the down button.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Down { get; } = new BindableElement(
            () => Properties.Resources.MenuDown
        );

        #endregion
    }
}
