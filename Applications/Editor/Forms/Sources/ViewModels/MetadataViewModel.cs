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
using Cube.Xui;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Cube.Pdf.App.Editor
{
    /* --------------------------------------------------------------------- */
    ///
    /// MetadataViewModel
    ///
    /// <summary>
    /// Represents the ViewModel for a MetadataWindow instance.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class MetadataViewModel : DialogViewModel
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// MetadataViewModel
        ///
        /// <summary>
        /// Initializes a new instance of the MetadataViewModel
        /// with the specified argumetns.
        /// </summary>
        ///
        /// <param name="callback">Callback method when applied.</param>
        /// <param name="src">Metadata object.</param>
        /// <param name="file">File information.</param>
        /// <param name="context">Synchronization context.</param>
        ///
        /* ----------------------------------------------------------------- */
        public MetadataViewModel(Action<Metadata> callback, Metadata src, Information file, SynchronizationContext context) :
            base(() => Properties.Resources.TitleMetadata, new Messenger(), context)
        {
            Filename      = this.Create(() => file.Name,          () => Properties.Resources.MenuFilename     );
            Producer      = this.Create(() => src.Producer,       () => Properties.Resources.MenuProducer     );
            Length        = this.Create(() => file.Length,        () => Properties.Resources.MenuFilesize     );
            CreationTime  = this.Create(() => file.CreationTime,  () => Properties.Resources.MenuCreationTime );
            LastWriteTime = this.Create(() => file.LastWriteTime, () => Properties.Resources.MenuLastWriteTime);

            Document = this.Create(() => src.Title,    e => src.Title    = e, () => Properties.Resources.MenuTitle   );
            Author   = this.Create(() => src.Author,   e => src.Author   = e, () => Properties.Resources.MenuAuthor  );
            Subject  = this.Create(() => src.Subject,  e => src.Subject  = e, () => Properties.Resources.MenuSubject );
            Keywords = this.Create(() => src.Keywords, e => src.Keywords = e, () => Properties.Resources.MenuKeywords);
            Creator  = this.Create(() => src.Creator,  e => src.Creator  = e, () => Properties.Resources.MenuCreator );
            Version  = this.Create(() => src.Version,  e => src.Version  = e, () => Properties.Resources.MenuVersion );
            Viewer   = CreateViewerPreferences(src);

            OK.Command = new RelayCommand(() => { Send<CloseMessage>(); callback(src); });
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// Filename
        ///
        /// <summary>
        /// Gets the menu that represents the filename.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Filename { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Document
        ///
        /// <summary>
        /// Gets the menu that represents the title of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Document { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Author
        ///
        /// <summary>
        /// Gets the menu that represents the author of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Author { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Subject
        ///
        /// <summary>
        /// Gets the menu that represents the subject of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Subject { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Keywords
        ///
        /// <summary>
        /// Gets the menu that represents keywords of the specified
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Keywords { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Version
        ///
        /// <summary>
        /// Gets the menu that represents the PDF version of the specified
        /// document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<Version> Version { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Viewer
        ///
        /// <summary>
        /// Gets the menu that represents the viewer preferences of the
        /// PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<ViewerPreferences> Viewer { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Creator
        ///
        /// <summary>
        /// Gets the menu that represents the creation program of the
        /// specified PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Creator { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Producer
        ///
        /// <summary>
        /// Gets the menu that represents the generating program of the
        /// specified PDF document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<string> Producer { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// Length
        ///
        /// <summary>
        /// Gets the menu that represents the length of the specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<long> Length { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// CreationTime
        ///
        /// <summary>
        /// Gets the menu that represents the creation time of the
        /// specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<DateTime> CreationTime { get; }

        /* ----------------------------------------------------------------- */
        ///
        /// LastWriteTime
        ///
        /// <summary>
        /// Gets the menu that represents the last updated time of the
        /// specified file.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement<DateTime> LastWriteTime { get; }

        #region Texts

        /* ----------------------------------------------------------------- */
        ///
        /// Summary
        ///
        /// <summary>
        /// Gets the menu that represents the summary group.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Summary { get; } = new BindableElement(
            () => Properties.Resources.MenuSummary
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Details
        ///
        /// <summary>
        /// Gets the menu that represents the details group.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public BindableElement Details { get; } = new BindableElement(
            () => Properties.Resources.MenuDetails
        );

        /* ----------------------------------------------------------------- */
        ///
        /// Versions
        ///
        /// <summary>
        /// Gets the collection of PDF version numbers.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<Version> Versions { get; } = new[]
        {
            new Version(1, 7),
            new Version(1, 6),
            new Version(1, 5),
            new Version(1, 4),
            new Version(1, 3),
            new Version(1, 2),
        };

        /* ----------------------------------------------------------------- */
        ///
        /// ViewerPreferences
        ///
        /// <summary>
        /// Gets the collection of viewer preferences.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public IEnumerable<ViewerPreferences> ViewerPreferences { get; } = new[]
        {
            Pdf.ViewerPreferences.SinglePage,
            Pdf.ViewerPreferences.OneColumn,
            Pdf.ViewerPreferences.TwoColumnLeft,
            Pdf.ViewerPreferences.TwoColumnRight,
            Pdf.ViewerPreferences.TwoPageLeft,
            Pdf.ViewerPreferences.TwoPageRight,
        };

        #endregion

        #endregion

        #region Implementations

        /* ----------------------------------------------------------------- */
        ///
        /// CreateViewerPreferences
        ///
        /// <summary>
        /// Creates a new menu for the ViewerPreferences property.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private BindableElement<ViewerPreferences> CreateViewerPreferences(Metadata src)
        {
            if (src.Viewer == Pdf.ViewerPreferences.None) src.Viewer = Pdf.ViewerPreferences.OneColumn;
            return this.Create(() => src.Viewer, e => src.Viewer = e, () => Properties.Resources.MenuLayout);
        }

        #endregion
    }
}
