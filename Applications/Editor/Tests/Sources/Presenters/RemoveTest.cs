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
using Cube.Mixin.Commands;
using Cube.Tests;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cube.Pdf.Editor.Tests.Presenters
{
    /* --------------------------------------------------------------------- */
    ///
    /// RemoveTest
    ///
    /// <summary>
    /// Tests for Remove commands and the RemoveViewModel class.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [TestFixture]
    class RemoveTest : ViewModelFixture
    {
        #region Tests

        /* ----------------------------------------------------------------- */
        ///
        /// Remove
        ///
        /// <summary>
        /// Executes the test for removing selected items.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void Remove() => Open("SampleRotation.pdf", "", vm =>
        {
            var src = vm.Data.Images.ToList();
            src[3].IsSelected = true;
            src[5].IsSelected = true;
            vm.Test(vm.Ribbon.Remove);

            var dest = vm.Data.Images.ToList();
            Assert.That(dest.Count, Is.EqualTo(7));
            for (var i = 0; i < dest.Count; ++i) Assert.That(dest[i].Index, Is.EqualTo(i));
        });

        /* ----------------------------------------------------------------- */
        ///
        /// RemoveOthers
        ///
        /// <summary>
        /// Executes the test for showing the RemoveWindow and remove
        /// specified items.
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Test]
        public void RemoveOthers() => Open("SampleRotation.pdf", "", vm =>
        {
            var cts = new CancellationTokenSource();
            var dp  = vm.Subscribe<RemoveViewModel>(e =>
            {
                Assert.That(e.Title.Text,         Is.Not.Null.And.Not.Empty);
                Assert.That(e.PageCaption.Text,   Is.Not.Null.And.Not.Empty);
                Assert.That(e.PageCaption.Value,  Is.Not.Null.And.Not.Empty);
                Assert.That(e.RangeCaption.Text,  Is.Not.Null.And.Not.Empty);
                Assert.That(e.RangeCaption.Value, Is.Not.Null.And.Not.Empty);

                Assert.That(e.Range.Value, Is.Empty);
                Assert.That(e.OK.Command.CanExecute(), Is.False);

                e.Range.Value = "1,3,5-7,9";
                Assert.That(e.OK.Command.CanExecute(), Is.True);

                e.OK.Command.Execute();
                cts.Cancel(); // done
            });

            Assert.That(vm.Ribbon.RemoveOthers.Command.CanExecute(), Is.True);
            TaskEx.Run(() => vm.Ribbon.RemoveOthers.Command.Execute());
            Assert.That(Wait.For(cts.Token), Is.True, "Timeout (Remove)");
            dp.Dispose();
        });

        #endregion
    }
}
