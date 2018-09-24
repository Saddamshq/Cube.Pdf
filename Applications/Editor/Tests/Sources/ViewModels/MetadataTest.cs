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
using Cube.FileSystem.TestService;
using Cube.Pdf.App.Editor;
using Cube.Pdf.Itext;
using Cube.Xui.Mixin;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cube.Pdf.Tests.Editor.ViewModels
{
    /* --------------------------------------------------------------------- */
    ///
    /// EncryptionTest
    ///
    /// <summary>
    /// Tests for the EncryptionViewModel class.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    class MetadataTest : ViewModelFixture
    {
        #region Tests

        /* ----------------------------------------------------------------- */
        ///
        /// Set
        ///
        /// <summary>
        /// Executes the test to set the metadata.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCaseSource(nameof(TestCases))]
        public async Task Set(int index, Metadata cmp)
        {
            await CreateAsync("Sample.pdf", "", 2, async (vm) =>
            {
                var cts = new CancellationTokenSource();
                using (var _ = Register(vm, cmp, cts))
                {
                    Assert.That(vm.Ribbon.Metadata.Command.CanExecute(), Is.True);
                    vm.Ribbon.Metadata.Command.Execute();
                    var done = await Wait.ForAsync(cts.Token).ConfigureAwait(false);
                    Assert.That(done, $"Timeout (Metadata)");
                }

                Assert.That(vm.Data.History.Undoable, Is.True);
                Assert.That(vm.Data.History.Redoable, Is.False);

                Destination = Path(Args(index, cmp.Title));
                await ExecuteAsync(vm, vm.Ribbon.SaveAs).ConfigureAwait(false);
                var save = await Wait.ForAsync(() => IO.Exists(Destination)).ConfigureAwait(false);
                Assert.That(save, $"Timeout (SaveAs)");
            });

            AssertMetadata(Destination, cmp);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// Cancel
        ///
        /// <summary>
        /// Executes the test to cancel the MetadataWindow.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public Task Cancel() => CreateAsync("Sample.pdf", "", 2, async (vm) =>
        {
            var cts = new CancellationTokenSource();
            var dp  = vm.Register<MetadataViewModel>(this, e =>
            {
                e.Document.Value = "dummy";
                Assert.That(e.Cancel.Command.CanExecute(), Is.True);
                e.Cancel.Command.Execute();
                cts.Cancel(); // done
            });

            vm.Ribbon.Metadata.Command.Execute();
            await Wait.ForAsync(cts.Token);
            dp.Dispose();

            Assert.That(vm.Data.History.Undoable, Is.False);
            Assert.That(vm.Data.History.Redoable, Is.False);
            Assert.That(vm.Data.Metadata.Value.Title, Is.Not.EqualTo("dummy"));
        });

        #endregion

        #region TestCases

        /* ----------------------------------------------------------------- */
        ///
        /// TestCases
        ///
        /// <summary>
        /// Gets test cases.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                var index = 0;

                yield return new TestCaseData(++index, new Metadata
                {
                    Title    = "Test title",
                    Author   = "Test author",
                    Subject  = "Test subject",
                    Keywords = "Test keywords",
                    Creator  = "Test creator",
                    Producer = "Test producer",
                    Version  = new Version(1, 6),
                    Viewer   = ViewerPreferences.TwoColumnRight,
                });

                yield return new TestCaseData(++index, new Metadata
                {
                    Title    = "日本語のタイトル",
                    Author   = "日本語の著者",
                    Subject  = "日本語のサブタイトル",
                    Keywords = "日本語のキーワード",
                    Creator  = "日本語のアプリケーション",
                    Producer = "日本語の PDF 変換",
                    Version  = new Version(1, 7),
                    Viewer   = ViewerPreferences.OneColumn,
                });
            }
        }

        #endregion

        #region Others

        /* ----------------------------------------------------------------- */
        ///
        /// Register
        ///
        /// <summary>
        /// Sets the operation corresponding to the MetadataViewModel
        /// message.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private IDisposable Register(MainViewModel vm, Metadata src, CancellationTokenSource cts) =>
            vm.Register<MetadataViewModel>(this, e =>
        {
            Assert.That(e.Filename.Value,      Is.Not.Null.And.Not.Empty);
            Assert.That(e.Producer.Value,      Is.Not.Null.And.Not.Empty);
            Assert.That(e.Length.Value,        Is.GreaterThan(0));
            Assert.That(e.CreationTime.Value,  Is.GreaterThan(DateTime.MinValue));
            Assert.That(e.LastWriteTime.Value, Is.GreaterThan(DateTime.MinValue));

            e.Document.Value = src.Title;
            e.Author.Value   = src.Author;
            e.Subject.Value  = src.Subject;
            e.Keywords.Value = src.Keywords;
            e.Creator.Value  = src.Creator;
            e.Viewer.Value   = src.Viewer;
            e.Version.Value  = src.Version;

            Assert.That(e.OK.Command.CanExecute(), Is.True);
            e.OK.Command.Execute();
            cts.Cancel(); // done
        });

        /* ----------------------------------------------------------------- */
        ///
        /// AssertMetadata
        ///
        /// <summary>
        /// Confirms that properties of the specified objects are equal.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void AssertMetadata(string src, Metadata cmp)
        {
            using (var reader = new DocumentReader(src))
            {
                AssertMetadata(reader.Metadata, cmp);
            }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// AssertMetadata
        ///
        /// <summary>
        /// Confirms that properties of the specified objects are equal.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void AssertMetadata(Metadata src, Metadata cmp)
        {
            Assert.That(src.Title,         Is.EqualTo(cmp.Title),    nameof(src.Title));
            Assert.That(src.Author,        Is.EqualTo(cmp.Author),   nameof(src.Author));
            Assert.That(src.Subject,       Is.EqualTo(cmp.Subject),  nameof(src.Subject));
            Assert.That(src.Keywords,      Is.EqualTo(cmp.Keywords), nameof(src.Keywords));
            Assert.That(src.Creator,       Is.EqualTo(cmp.Creator),  nameof(src.Creator));
            Assert.That(src.Viewer,        Is.EqualTo(cmp.Viewer));
            Assert.That(src.Version.Major, Is.EqualTo(cmp.Version.Major));
            Assert.That(src.Version.Minor, Is.EqualTo(cmp.Version.Minor));
        }

        #endregion
    }
}
