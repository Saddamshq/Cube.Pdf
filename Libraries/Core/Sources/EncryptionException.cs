﻿/* ------------------------------------------------------------------------- */
//
// Copyright (c) 2010 CubeSoft, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/* ------------------------------------------------------------------------- */
using System;

namespace Cube.Pdf
{
    /* --------------------------------------------------------------------- */
    ///
    /// EncryptionException
    ///
    /// <summary>
    /// Represents an exception about the encrypted operations.
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    [Serializable]
    public class EncryptionException : Exception
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionException
        ///
        /// <summary>
        /// Initializes a new instance of the EncryptionException class
        /// with the specified message.
        /// </summary>
        ///
        /// <param name="message">Message.</param>
        ///
        /* ----------------------------------------------------------------- */
        public EncryptionException(string message) : base(message) { }

        /* ----------------------------------------------------------------- */
        ///
        /// EncryptionException
        ///
        /// <summary>
        /// Initializes a new instance of the EncryptionException class
        /// with the specified parameters.
        /// </summary>
        ///
        /// <param name="message">Message.</param>
        /// <param name="inner">Inner exception object.</param>
        ///
        /* ----------------------------------------------------------------- */
        public EncryptionException(string message, Exception inner) : base(message, inner) { }

        #endregion
    }
}