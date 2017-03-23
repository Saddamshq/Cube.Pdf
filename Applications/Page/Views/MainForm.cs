﻿/* ------------------------------------------------------------------------- */
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
using System.ComponentModel;
using System.Windows.Forms;

namespace Cube.Pdf.App.Page
{
    /* --------------------------------------------------------------------- */
    ///
    /// MainForm
    ///
    /// <summary>
    /// CubePDF Page メイン画面を表示するクラスです。
    /// </summary>
    ///
    /* --------------------------------------------------------------------- */
    public partial class MainForm : Cube.Forms.FormBase
    {
        #region Constructors

        /* ----------------------------------------------------------------- */
        ///
        /// MainForm
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public MainForm()
        {
            InitializeComponent();
            InitializeLayout();
            InitializeEvents();
            InitializePresenters();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// MainForm
        /// 
        /// <summary>
        /// オブジェクトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public MainForm(string[] args)
            : this()
        {
            if (args == null || args.Length == 0) return;
            EventAggregator.GetEvents()?.Add.Publish(args);
        }

        #endregion

        #region Initialize methods

        /* ----------------------------------------------------------------- */
        ///
        /// InitializeLayout
        /// 
        /// <summary>
        /// レイアウトを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InitializeLayout()
        {
            EventAggregator = new EventAggregator();

            var tips = new ToolTip
            {
                InitialDelay = 200,
                AutoPopDelay = 5000,
                ReshowDelay  = 1000
            };
            tips.SetToolTip(TitleButton, Properties.Resources.About);

            FileListView.SmallImageList = Icons.ImageList;
            FileListView.Converter = new FileConverter(Icons);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InitializeEvents
        /// 
        /// <summary>
        /// イベントを初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InitializeEvents()
        {
            TitleButton.Click  += (s, e) => EventAggregator.GetEvents()?.Version.Publish();
            FileButton.Click   += (s, e) => EventAggregator.GetEvents()?.Add.Publish(null);
            RemoveButton.Click += (s, e) => EventAggregator.GetEvents()?.Remove.Publish();
            ClearButton.Click  += (s, e) => EventAggregator.GetEvents()?.Clear.Publish();
            UpButton.Click     += (s, e) => EventAggregator.GetEvents()?.Move.Publish(-1);
            DownButton.Click   += (s, e) => EventAggregator.GetEvents()?.Move.Publish(1);
            MergeButton.Click  += (s, e) => EventAggregator.GetEvents()?.Merge.Publish();
            SplitButton.Click  += (s, e) => EventAggregator.GetEvents()?.Split.Publish();
            ExitButton.Click   += (s, e) => Close();

            FileMenu.EventAggregator = EventAggregator;
            FileListView.EventAggregator = EventAggregator;
            FileListView.ContextMenuStrip = FileMenu;
            FileListView.DragEnter += (s, e) => OnDragEnter(e);
            FileListView.DragDrop  += (s, e) => OnDragDrop(e);

            ButtonsPanel.DragEnter += (s, e) => OnDragEnter(e);
            ButtonsPanel.DragDrop  += (s, e) => OnDragDrop(e);

            FooterPanel.DragEnter += (s, e) => OnDragEnter(e);
            FooterPanel.DragDrop  += (s, e) => OnDragDrop(e);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// InitializePresenters
        /// 
        /// <summary>
        /// 各種 Presenter を初期化します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        private void InitializePresenters()
        {
            new FileCollectionPresenter(FileListView, Files, Settings, EventAggregator);
            new MenuPresenter(this, Settings, EventAggregator);
        }

        #endregion

        #region Properties

        /* ----------------------------------------------------------------- */
        ///
        /// AllowOperation
        /// 
        /// <summary>
        /// 各種操作を受け付けるかどうかを取得または設定します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        [Browsable(true)]
        public bool AllowOperation
        {
            get { return ButtonsPanel.Enabled && FooterPanel.Enabled; }
            set
            {
                ButtonsPanel.Enabled = value;
                FooterPanel.Enabled  = value;
                Cursor = value ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        #endregion

        #region Methods

        /* ----------------------------------------------------------------- */
        ///
        /// Refresh
        /// 
        /// <summary>
        /// 再描画します。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        public override void Refresh()
        {
            try
            {
                SuspendLayout();

                MergeButton.Enabled = FileListView.Items.Count > 1;
                SplitButton.Enabled = FileListView.Items.Count > 0;

                UpButton.Enabled             =
                DownButton.Enabled           =
                RemoveButton.Enabled         =
                FileMenu.PreviewMenu.Enabled =
                FileMenu.UpMenu.Enabled      =
                FileMenu.DownMenu.Enabled    =
                FileMenu.RemoveMenu.Enabled  =
                FileListView.AnyItemsSelected;
            }
            finally
            {
                ResumeLayout();
                base.Refresh();
            }
        }

        #endregion

        #region Override methods

        /* ----------------------------------------------------------------- */
        ///
        /// OnLoad
        /// 
        /// <summary>
        /// フォームのロード時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnLoad(EventArgs e)
        {
            var asm = new AssemblyReader(Settings.Assembly);
            var version = new SoftwareVersion(asm.Assembly);
            version.Digit = 3;
            Text = $"{asm.Product} {version}";
            base.OnLoad(e);
            Refresh();
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnReceived
        /// 
        /// <summary>
        /// 他プロセスからデータ受信時に実行されるハンドラです。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnReceived(ValueEventArgs<object> e)
        {
            try
            {
                var args = e.Value as string[];
                if (args == null) return;
                EventAggregator.GetEvents()?.Add.Publish(args);
            }
            finally { base.OnReceived(e); }
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnDragEnter
        ///
        /// <summary>
        /// ファイルがドラッグされた時に実行されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnDragEnter(DragEventArgs e)
        {
            var prev = e.Effect;
            base.OnDragEnter(e);
            if (e.Effect != prev || !e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            e.Effect = DragDropEffects.Copy;
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnDragDrop
        ///
        /// <summary>
        /// ファイルがドロップされた時に実行されます。
        /// </summary>
        ///
        /* ----------------------------------------------------------------- */
        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            var files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if (files == null) return;

            EventAggregator.GetEvents()?.Add.Publish(files);
        }

        /* ----------------------------------------------------------------- */
        ///
        /// OnKeyDown
        /// 
        /// <summary>
        /// キーボードのキーが押下された時に実行されるハンドラです。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        protected override void OnKeyDown(KeyEventArgs e)
        {
            try { ShortcutKeys(e); }
            finally { base.OnKeyDown(e); }
        }

        #endregion

        #region Shortcut keys

        /* ----------------------------------------------------------------- */
        ///
        /// ShortcutKeys
        /// 
        /// <summary>
        /// ショートカットキーを処理します。
        /// </summary>
        /// 
        /* ----------------------------------------------------------------- */
        private void ShortcutKeys(KeyEventArgs e)
        {
            if (!e.Control) return;

            var results = true;
            switch (e.KeyCode)
            {
                case Keys.A:
                    foreach (ListViewItem item in FileListView.Items) item.Selected = true;
                    break;
                case Keys.D:
                    if (e.Shift) EventAggregator.GetEvents()?.Clear.Publish();
                    else EventAggregator.GetEvents()?.Remove.Publish();
                    break;
                case Keys.H:
                    EventAggregator.GetEvents()?.Version.Publish();
                    break;
                case Keys.M:
                    EventAggregator.GetEvents()?.Merge.Publish();
                    break;
                case Keys.O:
                    EventAggregator.GetEvents()?.Add.Publish(null);
                    break;
                case Keys.R:
                    EventAggregator.GetEvents()?.Preview.Publish();
                    break;
                case Keys.S:
                    EventAggregator.GetEvents()?.Split.Publish();
                    break;
                case Keys.K:
                case Keys.Up:
                    EventAggregator.GetEvents()?.Move.Publish(-1);
                    break;
                case Keys.J:
                case Keys.Down:
                    EventAggregator.GetEvents()?.Move.Publish(1);
                    break;
                default:
                    results = false;
                    break;
            }
            e.Handled = results;
        }

        #endregion

        #region Models
        private FileCollection Files = new FileCollection();
        private IconCollection Icons = new IconCollection();
        private Settings Settings = new Settings();
        #endregion

        #region Views
        private FileMenuControl FileMenu = new FileMenuControl();
        #endregion
    }
}
