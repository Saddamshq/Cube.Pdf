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
using System.Collections.Generic;
using System.Threading;

namespace Cube.Pdf.App.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// History
    ///
    /// <summary>
    /// Provides functionality to undo and redo actions.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class History : ObservableProperty
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// History
        ///
        /// <summary>
        /// Initializes a new instance of the History class.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public History()
        {
            Context = SynchronizationContext.Current;
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Undoable
        ///
        /// <summary>
        /// Gets the value indicating whether any of undo actions exist.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Undoable => _forward.Count > 0;

        /* ----------------------------------------------------------------- */
        ///
        /// Redoable
        ///
        /// <summary>
        /// Gets the value indicating whether any of redo actions exist.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Redoable => _reverse.Count > 0;

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Register
        ///
        /// <summary>
        /// Registers a history item.
        /// </summary>
        ///
        /// <param name="item">History item.</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Register(HistoryItem item) => Invoke(() =>
        {
            _reverse.Clear();
            _forward.Push(item);
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Clear
        ///
        /// <summary>
        /// Removes all of undo and redo actions.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Clear() => Invoke(() =>
        {
            _forward.Clear();
            _reverse.Clear();
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        ///
        /// <summary>
        /// Executes the undo action.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Undo() => Invoke(() =>
        {
            if (!Undoable) return;
            var item = _forward.Pop();
            item.Undo();
            _reverse.Push(item);
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Redo
        ///
        /// <summary>
        /// Executes the redo action.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Redo() => Invoke(() =>
        {
            if (!Redoable) return;
            var item = _reverse.Pop();
            item.Redo();
            _forward.Push(item);
        });

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// Invoke
        ///
        /// <summary>
        /// Invokes the action and raises property changed events.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public void Invoke(Action action)
        {
            action();
            RaisePropertyChanged(nameof(Undoable));
            RaisePropertyChanged(nameof(Redoable));
        }

        #endregion

        #region Fields
        private readonly Stack<HistoryItem> _forward = new Stack<HistoryItem>();
        private readonly Stack<HistoryItem> _reverse = new Stack<HistoryItem>();
        #endregion
    }

    /* --------------------------------------------------------------------- */
    ///
    /// HistoryItem
    ///
    /// <summary>
    /// Represents a pair of undo and redo actions.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class HistoryItem
    {
        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        ///
        /// <summary>
        /// Gets the action that represents the undo command.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Action Undo { get; set; }

        /* ----------------------------------------------------------------- */
        ///
        /// Redo
        ///
        /// <summary>
        /// Gets the action that represents the redo command.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public Action Redo { get; set; }

        #endregion
    }
}
