﻿/* ------------------------------------------------------------------------- */
///
/// ThumbnailPresenter.cs
///
/// Copyright (c) 2010 CubeSoft, Inc.
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.
///
/* ------------------------------------------------------------------------- */
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskEx = System.Threading.Tasks.Task;

namespace Cube.Pdf.App.ImageEx
{
    /* --------------------------------------------------------------------- */
    ///
    /// Cube.Pdf.App.ImageEx.ThumbnailPresenter
    ///
    /// <summary>
    /// ThumbnailForm をモデルを関連付けるためのクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public class ThumbnailPresenter : Cube.Forms.PresenterBase<ThumbnailForm, PickTask>
    {
        #region Constructors

        /* --------------------------------------------------------------------- */
        ///
        /// ThumbnailPresenter
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        public ThumbnailPresenter(ThumbnailForm view, PickTask model)
            : base(view, model)
        {
            View.FileName = System.IO.Path.GetFileNameWithoutExtension(Model.Path);
            View.Save    += View_Save;
            View.SaveAll += View_SaveAll;
            View.Preview += View_Preview;
            View.Removed += View_Removed;
            View.KeyDown += View_KeyDown;

            AddImages();
        }

        #endregion

        #region Events

        /* --------------------------------------------------------------------- */
        ///
        /// Completed
        /// 
        /// <summary>
        /// 操作が完了した時に発生するイベントです。
        /// </summary>
        /// 
        /// <remarks>
        /// このイベントは View.Save, View.SaveAll いずれかのイベントで
        /// 画像の保存処理が正常に完了した時に発生します。
        /// </remarks>
        ///
        /* --------------------------------------------------------------------- */
        public EventHandler Completed;

        #endregion

        #region Virtual methods

        /* --------------------------------------------------------------------- */
        ///
        /// OnCompleted
        /// 
        /// <summary>
        /// 操作が完了した時に実行されるハンドラです。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        protected virtual void OnCompleted(EventArgs e)
        {
            if (Completed != null) Completed(this, e);
        }

        #endregion

        #region Event handlers

        /* --------------------------------------------------------------------- */
        ///
        /// View_Save
        /// 
        /// <summary>
        /// 選択した抽出画像を保存する時に実行されるハンドラです。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        private async void View_Save(object sender, EventArgs ev)
        {
            var task = new SaveTask();
            if (string.IsNullOrEmpty(task.AskFolder(Model.Path))) return;

            var basename = System.IO.Path.GetFileNameWithoutExtension(Model.Path);
            task.Images = Model.Images;
            await task.RunAsync(basename, View.SelectedIndices);

            OnCompleted(new EventArgs());
            View.Close();
        }

        /* --------------------------------------------------------------------- */
        ///
        /// View_Save
        /// 
        /// <summary>
        /// 全ての抽出画像を保存する時に実行されるハンドラです。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        private async void View_SaveAll(object sender, EventArgs ev)
        {
            var task = new SaveTask();
            if (string.IsNullOrEmpty(task.AskFolder(Model.Path))) return;

            var basename = System.IO.Path.GetFileNameWithoutExtension(Model.Path);
            task.Images = Model.Images;
            await task.RunAsync(basename);

            OnCompleted(new EventArgs());
            View.Close();
        }

        /* --------------------------------------------------------------------- */
        ///
        /// View_Preview
        /// 
        /// <summary>
        /// 選択した抽出画像のプレビュー画面を表示する時に実行されるハンドラです。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        private void View_Preview(object sender, EventArgs ev)
        {
            var indices = View.SelectedIndices;
            if (indices == null || indices.Count <= 0) return;

            var index = indices[0];
            var filename = System.IO.Path.GetFileNameWithoutExtension(Model.Path);
            var dialog = new PreviewForm();
            dialog.FileName = string.Format("{0} ({1}/{2})", filename, index, Model.Images.Count);
            dialog.Image = Model.Images[index];
            dialog.ShowDialog();
        }

        /* --------------------------------------------------------------------- */
        ///
        /// View_Removed
        /// 
        /// <summary>
        /// 画像が削除された時に実行されるハンドラです。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        private void View_Removed(object sender, DataEventArgs<int> ev)
        {
            Model.Images.RemoveAt(ev.Value);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// View_KeyDown
        /// 
        /// <summary>
        /// キーボードのキーが押下された時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Control) View.SelectAll();
                    break;
                case Keys.D:
                    if (e.Control) View.Remove();
                    break;
                case Keys.S:
                    if (e.Control)
                    {
                        if (e.Shift) View_SaveAll(sender, e);
                        else if (View.AnyItemsSelected) View_Save(sender, e);
                    }
                    break;
                case Keys.Delete:
                    View.Remove();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Other private methods

        /* --------------------------------------------------------------------- */
        ///
        /// AddImagesAsync
        /// 
        /// <summary>
        /// 画像を View に追加します。
        /// </summary>
        ///
        /* --------------------------------------------------------------------- */
        private void AddImages()
        {
            var upper = View.ImageSize;
            for (var i = 0; i < Model.Images.Count; ++i)
            {
                var image = Model.GetImage(i, upper);
                if (image != null) View.Add(image);
            }
        }

        #endregion
    }
}
