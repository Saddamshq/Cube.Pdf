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
using Cube.FileSystem;
using Cube.FileSystem.TestService;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Cube.Pdf.Tests
{
    /* --------------------------------------------------------------------- */
    ///
    /// DocumentReaderFixture
    ///
    /// <summary>
    /// IDocumentReader の実装クラスをテストする際の補助用クラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    class DocumentReaderFixture : FileFixture
    {
        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// GetClassIds
        ///
        /// <summary>
        /// テスト対象となる IDocumentReader 実装クラスを示す文字列の
        /// 一覧を取得します。
        /// </summary>
        ///
        /// <returns>文字列一覧</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected static IEnumerable<string> GetClassIds() => GetFactory().Keys;

        /* ----------------------------------------------------------------- */
        ///
        /// GetFactory
        ///
        /// <summary>
        /// IDocumentReader の生成ルール一覧を取得します。
        /// </summary>
        ///
        /// <returns>生成ルール一覧</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected static IDictionary<string, Func<string, IQuery<string>, IO, IDocumentReader>> GetFactory() =>
            new Dictionary<string, Func<string, IQuery<string>, IO, IDocumentReader>>
            {
                { nameof(Cube.Pdf.Itext),  (s, q, io) => new Cube.Pdf.Itext.DocumentReader(s, q, io) },
                { nameof(Cube.Pdf.Pdfium), (s, q, io) => new Cube.Pdf.Pdfium.DocumentReader(s, q, io) },
            };

        /* ----------------------------------------------------------------- */
        ///
        /// Create
        ///
        /// <summary>
        /// IDocumentReader の実装オブジェクトを生成します。
        /// </summary>
        ///
        /// <param name="klass">生成オブジェクトを示す文字列</param>
        /// <param name="src">入力ファイルのパス</param>
        /// <param name="password">パスワード</param>
        ///
        /// <returns>IDocumentReader オブジェクト</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected IDocumentReader Create(string klass, string src, string password) =>
            Create(klass, src, new QueryValue<string>(password));

        /* ----------------------------------------------------------------- */
        ///
        /// Create
        ///
        /// <summary>
        /// IDocumentReader の実装オブジェクトを生成します。
        /// </summary>
        ///
        /// <param name="klass">生成オブジェクトを示す文字列</param>
        /// <param name="src">入力ファイルのパス</param>
        /// <param name="query">パスワード問い合わせ用オブジェクト</param>
        ///
        /// <returns>IDocumentReader オブジェクト</returns>
        ///
        /* ----------------------------------------------------------------- */
        protected IDocumentReader Create(string klass, string src, IQuery<string> query)
        {
            Assert.That(GetFactory().TryGetValue(klass, out var factory), Is.True);
            return factory(src, query, IO);
        }

        #endregion
    }
}
