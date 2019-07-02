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
using Cube.Mixin.Commands;
using Cube.Tests;
using NUnit.Framework;
using System.Linq;

namespace Cube.Pdf.Editor.Tests.Presenters
{
    /* --------------------------------------------------------------------- */
    ///
    /// OthersTest
    ///
    /// <summary>
    /// Uncategorized tests of the MainViewModel class.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    class OthersTest : ViewModelFixture
    {
        #region Tests

        /* ----------------------------------------------------------------- */
        ///
        /// Properties
        ///
        /// <summary>
        /// Confirms default values of properties.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void Properties() => Create(vm =>
        {
            var pf = vm.Data.Images.Preferences;
            Assert.That(vm.Recent.Items,  Is.Not.Null);
            Assert.That(pf.ItemSize,      Is.EqualTo(250));
            Assert.That(pf.ItemSizeIndex, Is.EqualTo(3));
            Assert.That(pf.TextHeight,    Is.EqualTo(25));
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Close
        ///
        /// <summary>
        /// Tests the Close command.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [TestCase("Sample.pdf", 2, true )]
        [TestCase("Sample.pdf", 2, false)]
        public void Close(string filename, int n, bool modify) => Create(vm =>
        {
            var fi = IO.Get(GetSource(filename));
            Source = Get(MakeArgs(fi.BaseName, modify));
            IO.Copy(fi.FullName, Source, true);
            vm.Test(vm.Ribbon.Open);
            Assert.That(vm.Data.Count.Value, Is.EqualTo(n));

            if (modify)
            {
                vm.Test(vm.Ribbon.Select);
                vm.Test(vm.Ribbon.RotateLeft);
            }

            vm.Test(vm.Ribbon.Close);
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Extract
        ///
        /// <summary>
        /// Executes the test for extracting selected items as a new PDF
        /// document.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void Extract() => Open("Sample.pdf", "", vm =>
        {
            Destination = Get(MakeArgs("Sample"));
            Assert.That(IO.Exists(Destination), Is.False);

            Assert.That(vm.Ribbon.Extract.Command.CanExecute(), Is.False);
            vm.Data.Images.First().IsSelected = true;
            Assert.That(Wait.For(() => vm.Ribbon.Extract.Command.CanExecute()));

            vm.Test(vm.Ribbon.Extract);
            Assert.That(IO.Exists(Destination), Is.True);
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Rotate
        ///
        /// <summary>
        /// Executes the test for rotating selected items.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void Rotate() => Open("Sample.pdf", "", vm =>
        {
            var images = vm.Data.Images.ToList();
            var dest   = images[0];
            var dummy  = vm.Data.Images.Preferences.Dummy;
            Assert.That(Wait.For(() => dest.Image != dummy), "Timeout");

            Assert.That(vm.Ribbon.RotateLeft.Command.CanExecute(),  Is.False);
            Assert.That(vm.Ribbon.RotateRight.Command.CanExecute(), Is.False);

            var image  = dest.Image;
            var width  = dest.Width;
            var height = dest.Height;
            var count  = 0;
            dest.IsSelected = true;
            dest.PropertyChanged += (s, e) => ++count;

            vm.Test(vm.Ribbon.RotateLeft);
            Assert.That(Wait.For(() => dest.Image != dummy), "Timeout (Left)");
            Assert.That(dest.Width,  Is.Not.EqualTo(width),  nameof(width));
            Assert.That(dest.Height, Is.Not.EqualTo(height), nameof(height));

            vm.Test(vm.Ribbon.RotateRight);
            Assert.That(Wait.For(() => dest.Image != dummy), "Timeout (Right)");
            Assert.That(dest.Width,  Is.EqualTo(width),  nameof(width));
            Assert.That(dest.Height, Is.EqualTo(height), nameof(height));
        });

        /* ----------------------------------------------------------------- */
        ///
        /// Undo
        ///
        /// <summary>
        /// Executes the test for canceling the last action.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void Undo() => Open("SampleRotation.pdf", "", vm =>
        {
            vm.Test(vm.Ribbon.Select);
            vm.Test(vm.Ribbon.Remove);

            Assert.That(vm.Data.Images.Count, Is.EqualTo(0));
            Assert.That(vm.Data.History.Undoable, Is.True);
            Assert.That(vm.Data.History.Redoable, Is.False);

            vm.Test(vm.Ribbon.Undo);

            Assert.That(vm.Data.Images.Count, Is.EqualTo(9));
            Assert.That(vm.Data.History.Undoable, Is.False);
            Assert.That(vm.Data.History.Redoable, Is.True);

            vm.Test(vm.Ribbon.Redo);

            Assert.That(vm.Data.Images.Count, Is.EqualTo(0));
            Assert.That(vm.Data.History.Undoable, Is.True);
            Assert.That(vm.Data.History.Redoable, Is.False);
        });

        #endregion
    }
}
