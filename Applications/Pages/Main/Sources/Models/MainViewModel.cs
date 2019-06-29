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
using Cube.Mixin.Syntax;
using Cube.Mixin.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Cube.Pdf.Pages
{
    /* --------------------------------------------------------------------- */
    ///
    /// MainViewModel
    ///
    /// <summary>
    /// Represents the ViewModel for the MainWindow.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class MainViewModel : ViewModelBase<MainFacade>
    {
        #region Constructors

        /* --------------------------------------------------------------------- */
        ///
        /// MainViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        public MainViewModel() : this(SynchronizationContext.Current) { }

        /* --------------------------------------------------------------------- */
        ///
        /// MainViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the MainViewModel class with the
        /// specified context.
        /// </summary>
        ///
        /// <param name="context">Synchronization context.</param>
        ///
        /* --------------------------------------------------------------------- */
        public MainViewModel(SynchronizationContext context) : base(
            new MainFacade(new IO(), context),
            new Aggregator(),
            context
        ) {
            Facade.PropertyChanged += (s, e) => OnPropertyChanged(e);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Busy
        ///
        /// <summary>
        /// Gets a value indicating whether the class is busy.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public bool Busy => Facade.Busy;

        #endregion

        #region Methods

        /* --------------------------------------------------------------------- */
        ///
        /// Add
        ///
        /// <summary>
        /// Invokes the Add command.
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        public void Add() => Send(MessageFactory.CreateForAdd(), e => e.Each(f => Facade.Add(f))).Forget();

        /* --------------------------------------------------------------------- */
        ///
        /// Merge
        ///
        /// <summary>
        /// Invokes the Merge command.
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        public void Merge() => Send(MessageFactory.CreateForMerge(), e => Facade.Merge(e)).Forget();

        /* --------------------------------------------------------------------- */
        ///
        /// Split
        ///
        /// <summary>
        /// Invokes the Split command.
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        public void Split() => Send(MessageFactory.CreateForSplit(), e => Facade.Split(e, new List<string>())).Forget();

        /* ----------------------------------------------------------------- */
        ///
        /// Move
        ///
        /// <summary>
        /// Moves the specified items by the specified offset.
        /// </summary>
        ///
        /// <param name="indices">Indices of files.</param>
        /// <param name="offset">Offset to move.</param>
        ///
        /* ----------------------------------------------------------------- */
        public void Move(IEnumerable<int> indices, int offset) => Track(() => Facade.Move(indices, offset)).Forget();

        #endregion
    }
}