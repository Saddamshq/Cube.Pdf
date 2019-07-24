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
using Cube.Mixin.Observing;
using Cube.Mixin.Syntax;
using Cube.Xui;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cube.Pdf.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// ExtractViewModel
    ///
    /// <summary>
    /// Represents the ViewModel associated with the ExtractWindow class.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public sealed class ExtractViewModel : DialogViewModel<ExtractFacade>
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// ExtractViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the RemoveViewModel with the
        /// specified arguments.
        /// </summary>
        ///
        /// <param name="callback">Callback method when applied.</param>
        /// <param name="selection">Page selection.</param>
        /// <param name="count">Number of pages.</param>
        /// <param name="context">Synchronization context.</param>
        ///
        /* ----------------------------------------------------------------- */
        public ExtractViewModel(Action<ExtractOption> callback,
            ImageSelection selection,
            int count,
            SynchronizationContext context
        ) : base(new ExtractFacade(selection, count, new ContextInvoker(context, false)),
            new Aggregator(),
            context
        ) {
            OK.Command = new DelegateCommand(
                () => Track(() => {
                    callback(Facade.Value);
                    Send<CloseMessage>();
                }),
                () => true
            );
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Formats
        ///
        /// <summary>
        /// Gets the collection of extract formats.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<ExtractFormat> Formats => Facade.Formats;

        /* ----------------------------------------------------------------- */
        ///
        /// Destination
        ///
        /// <summary>
        /// Gets the destination menu.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Destination => Get(() => new BindableElement<string>(
            () => Properties.Resources.MenuDestination,
            () => Facade.Value.Destination,
            e  => Facade.Value.Destination = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Format
        ///
        /// <summary>
        /// Gets the format menu.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<ExtractFormat> Format => Get(() => new BindableElement<ExtractFormat>(
            () => Properties.Resources.MenuFormat,
            () => Facade.Value.Format,
            e  => Facade.Value.Format = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Target
        ///
        /// <summary>
        /// Gets the target menu.
        /// </summary>
        ///
        /// <remarks>
        /// Value determines whether the Selected element is enabled.
        /// </remarks>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> Target => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuTarget,
            () => Facade.Selection.Count > 0,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Selected
        ///
        /// <summary>
        /// Gets the menu to determine whether the selected pages are the
        /// target to extract.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> Selected => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuExtractSelected,
            () => Facade.Value.Target == ExtractTarget.Selected,
            e  => e.Then(() => Facade.Value.Target = ExtractTarget.Selected),
            GetInvoker(false)
        )).Associate(Facade.Value, nameof(ExtractOption.Target));

        /* ----------------------------------------------------------------- */
        ///
        /// All
        ///
        /// <summary>
        /// Gets the menu to determine whether the all pages are the
        /// target to extract.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> All => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuExtractAll,
            () => Facade.Value.Target == ExtractTarget.All,
            e  => e.Then(() => Facade.Value.Target = ExtractTarget.All),
            GetInvoker(false)
        )).Associate(Facade.Value, nameof(ExtractOption.Target));

        /* ----------------------------------------------------------------- */
        ///
        /// Specified
        ///
        /// <summary>
        /// Gets the menu to determine whether the specified range of
        /// pages is the target to extract.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> Specified => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuExtractRange,
            () => Facade.Value.Target == ExtractTarget.Range,
            e  => e.Then(() => Facade.Value.Target = ExtractTarget.Range),
            GetInvoker(false)
        )).Associate(Facade.Value, nameof(ExtractOption.Target));

        /* ----------------------------------------------------------------- */
        ///
        /// Range
        ///
        /// <summary>
        /// Gets the page range menu.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<string> Range => Get(() => new BindableElement<string>(
            () => Properties.Resources.MessageRangeExample,
            () => Facade.Value.Range,
            e  => Facade.Value.Range = e,
            GetInvoker(false)
        ));

        /* ----------------------------------------------------------------- */
        ///
        /// Split
        ///
        /// <summary>
        /// Gets the menu to determine whether to save as a separate file
        /// per page.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement<bool> Split => Get(() => new BindableElement<bool>(
            () => Properties.Resources.MenuSplit,
            () => Facade.Value.Split,
            e  => Facade.Value.Split = e,
            GetInvoker(false)
        ) {
            Command = new DelegateCommand(
                () => { },
                () => Facade.Value.Format == ExtractFormat.Pdf
            ).Associate(Facade.Value, nameof(ExtractOption.Format))
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Option
        ///
        /// <summary>
        /// Gets the option menu. The property has text only.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IElement Option => Get(() => new BindableElement(
            () => Properties.Resources.MenuOptions,
            GetInvoker(false)
        ));

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// GetTitle
        ///
        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        ///
        /// <returns>String value.</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected override string GetTitle() => Properties.Resources.TitleExtract;

        #endregion
    }
}
