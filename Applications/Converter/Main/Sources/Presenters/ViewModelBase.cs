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
using Cube.Mixin.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cube.Pdf.Converter
{
    /* --------------------------------------------------------------------- */
    ///
    /// ViewModelBase
    ///
    /// <summary>
    /// Represents the base class of ViewModel classes.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public abstract class ViewModelBase : PresentableBase
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// CommonViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the CommonViewModel with the
        /// specified arguments.
        /// </summary>
        ///
        /// <param name="aggregator">Message aggregator.</param>
        /// <param name="context">Synchronization context.</param>
        ///
        /* ----------------------------------------------------------------- */
        protected ViewModelBase(Aggregator aggregator, SynchronizationContext context) :
            base(aggregator, context) { }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Send
        ///
        /// <summary>
        /// Sends the specified message and invokes the specified action.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected void Send<T>(T message, Action<T> next)
        {
            Send(message);
            _ = Track(() => next(message), MessageFactory.Create, true);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Confirm
        ///
        /// <summary>
        /// Sends the specified dialog message and determines if the status
        /// is not Cancel.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected bool Confirm(DialogMessage message)
        {
            Send(message);
            return message.Status != DialogStatus.Cancel;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// TrackClose
        ///
        /// <summary>
        /// Invokes the specified action as an asynchronous manner and
        /// sends the close message.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected Task TrackClose(Action action) => TaskEx.Run(() =>
        {
            try { action(); }
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception err)
            {
                this.LogError(err);
                Send(MessageFactory.Create(err));
            }
            finally { Post<CloseMessage>(); }
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Dispose
        ///
        /// <summary>
        /// Releases the unmanaged resources used by the object and
        /// optionally releases the managed resources.
        /// </summary>
        ///
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// </param>
        ///
        /* ----------------------------------------------------------------- */
        protected override void Dispose(bool disposing) { }

        #endregion
    }
}
