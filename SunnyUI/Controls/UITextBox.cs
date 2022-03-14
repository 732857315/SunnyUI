﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2022 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * 文件名称: UITextBox.cs
 * 文件说明: 输入框
 * 当前版本: V3.1
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-06-03: V2.2.5 增加多行，增加滚动条
 * 2020-09-03: V2.2.7 增加FocusedSelectAll属性，激活时全选
 * 2021-04-15: V3.0.3 修改文字可以居中显示
 * 2021-04-17: V3.0.3 不限制高度为根据字体计算，可进行调整，解决多行输入时不能输入回车的问题
 * 2021-04-18: V3.0.3 增加ShowScrollBar属性，单独控制垂直滚动条
 * 2021-06-01: V3.0.4 增加图标和字体图标的显示
 * 2021-07-18: V3.0.5 修改Focus可用
 * 2021-08-03: V3.0.5 增加GotFocus和LostFocus事件
 * 2021-08-15: V3.0.6 重写了水印文字的画法，并增加水印文字颜色
 * 2021-09-07: V3.0.6 增加按钮
 * 2021-10-14: V3.0.8 调整最小高度限制
 * 2021-10-15: V3.0.8 支持修改背景色
 * 2022-01-07: V3.1.0 按钮支持自定义颜色
 * 2022-02-16: V3.1.1 增加了只读的颜色设置
******************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    public sealed partial class UITextBox : UIPanel, ISymbol, IToolTip
    {
        private readonly UIEdit edit = new UIEdit();
        private readonly UIScrollBar bar = new UIScrollBar();
        private readonly UISymbolButton btn = new UISymbolButton();

        public UITextBox()
        {
            InitializeComponent();
            InitializeComponentEnd = true;
            SetStyleFlags();

            ShowText = false;
            Font = UIFontColor.Font();
            Padding = new Padding(0);
            MinimumSize = new Size(1, 16);

            Width = 150;
            Height = 29;

            edit.AutoSize = false;
            edit.Top = (Height - edit.Height) / 2;
            edit.Left = 4;
            edit.Width = Width - 8;
            edit.Text = String.Empty;
            edit.BorderStyle = BorderStyle.None;
            edit.TextChanged += Edit_TextChanged;
            edit.KeyDown += Edit_OnKeyDown;
            edit.KeyUp += Edit_OnKeyUp;
            edit.KeyPress += Edit_OnKeyPress;
            edit.MouseEnter += Edit_MouseEnter;
            edit.Click += Edit_Click;
            edit.DoubleClick += Edit_DoubleClick;
            edit.Leave += Edit_Leave;
            edit.Validated += Edit_Validated;
            edit.Validating += Edit_Validating;
            edit.GotFocus += Edit_GotFocus;
            edit.LostFocus += Edit_LostFocus;
            edit.MouseLeave += Edit_MouseLeave;
            edit.MouseWheel += Edit_MouseWheel;
            edit.MouseDown += Edit_MouseDown;
            edit.MouseUp += Edit_MouseUp;
            edit.MouseMove += Edit_MouseMove;

            btn.Parent = this;
            btn.Visible = false;
            btn.Text = "";
            btn.Symbol = 361761;
            btn.SymbolOffset = new Point(0, 1);
            btn.Top = 1;
            btn.Height = 25;
            btn.Width = 29;
            btn.BackColor = Color.Transparent;
            btn.Click += Btn_Click;
            btn.Radius = 3;

            edit.Invalidate();
            Controls.Add(edit);
            fillColor = Color.White;

            bar.Parent = this;
            bar.Dock = DockStyle.None;
            bar.Style = UIStyle.Custom;
            bar.Visible = false;
            bar.ValueChanged += Bar_ValueChanged;
            bar.MouseEnter += Bar_MouseEnter;
            TextAlignment = ContentAlignment.MiddleLeft;

            SizeChange();

            editCursor = Cursor;
            TextAlignmentChange += UITextBox_TextAlignmentChange;
        }

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("填充颜色，当值为背景色或透明色或空值则不填充"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public new Color FillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                if (fillColor != value)
                {
                    fillColor = value;
                    _style = UIStyle.Custom;
                    Invalidate();
                }

                AfterSetFillColor(value);
            }
        }

        /// <summary>
        /// 字体只读颜色
        /// </summary>
        public Color ForeReadOnlyColor
        {
            get => foreReadOnlyColor;
            set => SetForeReadOnlyColor(value);
        }

        /// <summary>
        /// 边框只读颜色
        /// </summary>
        public Color RectReadOnlyColor
        {
            get => rectReadOnlyColor;
            set => SetRectReadOnlyColor(value);
        }

        /// <summary>
        /// 填充只读颜色
        /// </summary>
        public Color FillReadOnlyColor
        {
            get => fillReadOnlyColor;
            set => SetFillReadOnlyColor(value);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }

        public event EventHandler ButtonClick;

        [DefaultValue(29), Category("SunnyUI"), Description("按钮宽度")]
        public int ButtonWidth { get => btn.Width; set { btn.Width = Math.Max(20, value); SizeChange(); } }

        [DefaultValue(false), Category("SunnyUI"), Description("显示按钮")]
        public bool ShowButton
        {
            get => btn.Visible;
            set
            {
                if (Multiline)
                {
                    btn.Visible = false;
                }
                else
                {
                    btn.Visible = value;
                }

                SizeChange();
            }
        }

        private void Edit_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        private void Edit_MouseUp(object sender, MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        private void Edit_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        private void Edit_MouseLeave(object sender, EventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }

        public Control ExToolTipControl()
        {
            return edit;
        }

        private void Edit_LostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(this, e);
        }

        private void Edit_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }

        private void Edit_Validating(object sender, CancelEventArgs e)
        {
            Validating?.Invoke(this, e);
        }

        public new event MouseEventHandler MouseDown;
        public new event MouseEventHandler MouseUp;
        public new event MouseEventHandler MouseMove;
        public new event EventHandler GotFocus;
        public new event EventHandler LostFocus;
        public new event CancelEventHandler Validating;
        public new event EventHandler Validated;
        public new event EventHandler MouseLeave;
        public new event EventHandler DoubleClick;
        public new event EventHandler Click;
        [Browsable(true)]
        public new event EventHandler TextChanged;
        public new event KeyEventHandler KeyDown;
        public new event KeyEventHandler KeyUp;
        public new event KeyPressEventHandler KeyPress;
        public new event EventHandler Leave;

        private void Edit_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(this, e);
        }

        public new void Focus()
        {
            base.Focus();
            edit.Focus();
        }

        [Browsable(false)]
        public TextBox TextBox => edit;

        private void Edit_Leave(object sender, EventArgs e)
        {
            Leave?.Invoke(this, e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            edit.BackColor = GetFillColor();
            edit.Enabled = Enabled;
        }

        public override bool Focused => edit.Focused;

        [DefaultValue(false)]
        [Description("激活时选中全部文字"), Category("SunnyUI")]
        public bool FocusedSelectAll
        {
            get => edit.FocusedSelectAll;
            set => edit.FocusedSelectAll = value;
        }

        private void UITextBox_TextAlignmentChange(object sender, ContentAlignment alignment)
        {
            if (edit == null) return;
            if (alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.MiddleLeft ||
                alignment == ContentAlignment.BottomLeft)
                edit.TextAlign = HorizontalAlignment.Left;

            if (alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.MiddleCenter ||
                alignment == ContentAlignment.BottomCenter)
                edit.TextAlign = HorizontalAlignment.Center;

            if (alignment == ContentAlignment.TopRight || alignment == ContentAlignment.MiddleRight ||
                alignment == ContentAlignment.BottomRight)
                edit.TextAlign = HorizontalAlignment.Right;
        }

        private void Edit_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        protected override void OnCursorChanged(EventArgs e)
        {
            base.OnCursorChanged(e);
            edit.Cursor = Cursor;
        }

        private Cursor editCursor;

        private void Bar_MouseEnter(object sender, EventArgs e)
        {
            editCursor = Cursor;
            Cursor = Cursors.Default;
        }

        private void Edit_MouseEnter(object sender, EventArgs e)
        {
            Cursor = editCursor;
            if (FocusedSelectAll)
            {
                SelectAll();
            }
        }

        private void Edit_MouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
            if (bar != null && bar.Visible && edit != null)
            {
                var si = ScrollBarInfo.GetInfo(edit.Handle);
                if (e.Delta > 10)
                {
                    if (si.nPos > 0)
                    {
                        ScrollBarInfo.ScrollUp(edit.Handle);
                    }
                }
                else if (e.Delta < -10)
                {
                    if (si.nPos < si.ScrollMax)
                    {
                        ScrollBarInfo.ScrollDown(edit.Handle);
                    }
                }
            }

            SetScrollInfo();
        }

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            if (edit != null)
            {
                ScrollBarInfo.SetScrollValue(edit.Handle, bar.Value);
            }
        }

        private bool multiline;

        [DefaultValue(false)]
        public bool Multiline
        {
            get => multiline;
            set
            {
                multiline = value;
                edit.Multiline = value;
                // edit.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
                // bar.Visible = multiline;

                if (value && Type != UIEditType.String)
                {
                    Type = UIEditType.String;
                }

                SizeChange();
            }
        }

        private bool showScrollBar;

        [DefaultValue(false)]
        [Description("显示垂直滚动条"), Category("SunnyUI")]
        public bool ShowScrollBar
        {
            get => showScrollBar;
            set
            {
                value = value && Multiline;
                showScrollBar = value;
                if (value)
                {
                    edit.ScrollBars = ScrollBars.Vertical;
                    bar.Visible = true;
                }
                else
                {
                    edit.ScrollBars = ScrollBars.None;
                    bar.Visible = false;
                }
            }
        }

        [DefaultValue(true)]
        public bool WordWarp
        {
            get => edit.WordWrap;
            set => edit.WordWrap = value;
        }

        public void Select(int start, int length)
        {
            edit.Focus();
            edit.Select(start, length);
        }

        public void ScrollToCaret()
        {
            edit.ScrollToCaret();
        }

        private void Edit_OnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        private void Edit_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoEnter?.Invoke(this, e);
            }

            KeyDown?.Invoke(this, e);
        }

        public event EventHandler DoEnter;

        private void Edit_OnKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        [DefaultValue(null)]
        [Description("水印文字"), Category("SunnyUI")]
        public string Watermark
        {
            get => edit.Watermark;
            set => edit.Watermark = value;
        }

        [DefaultValue(typeof(Color), "Gray")]
        [Description("水印文字颜色"), Category("SunnyUI")]
        public Color WatermarkColor
        {
            get => edit.WaterMarkColor;
            set => edit.WaterMarkColor = value;
        }

        public void SelectAll()
        {
            edit.Focus();
            edit.SelectAll();
        }

        public void CheckMaxMin()
        {
            edit.CheckMaxMin();
        }

        private void Edit_TextChanged(object s, EventArgs e)
        {
            TextChanged?.Invoke(this, e);
            SetScrollInfo();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            edit.IsScaled = true;
            edit.Font = Font;
            SizeChange();
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            SizeChange();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            SizeChange();
        }

        public void SetScrollInfo()
        {
            if (bar == null)
            {
                return;
            }

            var si = ScrollBarInfo.GetInfo(edit.Handle);
            if (si.ScrollMax > 0)
            {
                bar.Maximum = si.ScrollMax;
                bar.Value = si.nPos;
            }
            else
            {
                bar.Maximum = si.ScrollMax;
            }
        }

        private void SizeChange()
        {
            if (!InitializeComponentEnd) return;
            if (edit == null) return;
            if (btn == null) return;

            if (!multiline)
            {
                if (Height < UIGlobal.EditorMinHeight) Height = UIGlobal.EditorMinHeight;
                if (Height > UIGlobal.EditorMaxHeight) Height = UIGlobal.EditorMaxHeight;

                edit.Height = Math.Min(Height - RectSize * 2, edit.PreferredHeight);
                edit.Top = (Height - edit.Height) / 2;

                if (icon == null && Symbol == 0)
                {
                    edit.Left = 4 + Padding.Left;
                    edit.Width = Width - 8 - Padding.Left - Padding.Right;
                }
                else
                {
                    if (icon != null)
                    {
                        edit.Left = 4 + iconSize + Padding.Left;
                        edit.Width = Width - 8 - iconSize - Padding.Left - Padding.Right;
                    }
                    else if (Symbol > 0)
                    {
                        edit.Left = 4 + SymbolSize + Padding.Left;
                        edit.Width = Width - 8 - SymbolSize - Padding.Left - Padding.Right;
                    }
                }

                btn.Left = Width - 2 - ButtonWidth;
                btn.Top = 2;
                btn.Height = Height - 4;

                if (ShowButton)
                {
                    edit.Width = edit.Width - btn.Width - 3;
                }
            }
            else
            {
                btn.Visible = false;
                edit.Top = 3;
                edit.Height = Height - 6;
                edit.Left = 1;
                edit.Width = Width - 2;

                bar.Top = 2;
                bar.Width = ScrollBarInfo.VerticalScrollBarWidth();
                bar.Left = Width - bar.Width - 1;
                bar.Height = Height - 4;
                bar.BringToFront();

                SetScrollInfo();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            edit.Focus();
        }

        public void Clear()
        {
            edit.Clear();
        }

        [DefaultValue('\0')]
        [Description("密码掩码"), Category("SunnyUI")]
        public char PasswordChar
        {
            get => edit.PasswordChar;
            set => edit.PasswordChar = value;
        }

        [DefaultValue(false)]
        [Description("是否只读"), Category("SunnyUI")]
        public bool ReadOnly
        {
            get => isReadOnly;
            set
            {
                isReadOnly = value;
                edit.ReadOnly = value;
                edit.BackColor = GetFillColor();
                Invalidate();
            }
        }

        [Description("输入类型"), Category("SunnyUI")]
        [DefaultValue(UIEditType.String)]
        public UIEditType Type
        {
            get => edit.Type;
            set => edit.Type = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，能输入的最大值
        /// </summary>
        [Description("当InputType为数字类型时，能输入的最大值。"), Category("SunnyUI")]
        [DefaultValue(int.MaxValue)]
        public double Maximum
        {
            get => edit.MaxValue;
            set => edit.MaxValue = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，能输入的最小值
        /// </summary>
        [Description("当InputType为数字类型时，能输入的最小值。"), Category("SunnyUI")]
        [DefaultValue(int.MinValue)]
        public double Minimum
        {
            get => edit.MinValue;
            set => edit.MinValue = value;
        }

        [DefaultValue(false)]
        [Description("是否判断最大值显示"), Category("SunnyUI")]
        public bool MaximumEnabled
        {
            get => HasMaximum;
            set => HasMaximum = value;
        }

        [DefaultValue(false)]
        [Description("是否判断最小值显示"), Category("SunnyUI")]
        public bool MinimumEnabled
        {
            get => HasMinimum;
            set => HasMinimum = value;
        }

        [DefaultValue(false), Browsable(false)]
        [Description("是否判断最大值显示"), Category("SunnyUI")]
        public bool HasMaximum
        {
            get => edit.HasMaxValue;
            set => edit.HasMaxValue = value;
        }

        [DefaultValue(false), Browsable(false)]
        [Description("是否判断最小值显示"), Category("SunnyUI")]
        public bool HasMinimum
        {
            get => edit.HasMinValue;
            set => edit.HasMinValue = value;
        }

        [DefaultValue(0.00)]
        [Description("浮点返回值"), Category("SunnyUI")]
        public double DoubleValue
        {
            get => edit.DoubleValue;
            set => edit.DoubleValue = value;
        }

        [DefaultValue(0)]
        [Description("整形返回值"), Category("SunnyUI")]
        public int IntValue
        {
            get => edit.IntValue;
            set => edit.IntValue = value;
        }

        [Description("文本返回值"), Category("SunnyUI")]
        [Browsable(true)]
        [DefaultValue("")]
        public override string Text
        {
            get => edit.Text;
            set => edit.Text = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，小数位数。
        /// </summary>
        [Description("当InputType为数字类型时，小数位数。")]
        [DefaultValue(2), Category("SunnyUI")]
        [Browsable(false)]
        public int DecLength
        {
            get => edit.DecLength;
            set => edit.DecLength = Math.Max(value, 0);
        }

        [Description("浮点数，显示文字小数位数"), Category("SunnyUI")]
        [DefaultValue(2)]
        public int DecimalPlaces
        {
            get => DecLength;
            set => DecLength = value;
        }

        [DefaultValue(false)]
        [Description("整形或浮点输入时，是否可空显示"), Category("SunnyUI")]
        public bool CanEmpty
        {
            get => edit.CanEmpty;
            set => edit.CanEmpty = value;
        }

        public void Empty()
        {
            if (edit.CanEmpty)
                edit.Text = "";
        }

        public bool IsEmpty => edit.Text == "";

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ActiveControl = edit;
        }

        [DefaultValue(32767)]
        public int MaxLength
        {
            get => edit.MaxLength;
            set => edit.MaxLength = Math.Max(value, 1);
        }

        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);

            if (uiColor.IsCustom()) return;

            fillColor = uiColor.EditorBackColor;
            foreColor = UIFontColor.Primary;
            edit.BackColor = GetFillColor();
            edit.ForeColor = GetForeColor();

            if (bar != null)
            {
                bar.ForeColor = uiColor.PrimaryColor;
                bar.HoverColor = uiColor.ButtonFillHoverColor;
                bar.PressColor = uiColor.ButtonFillPressColor;
                bar.FillColor = fillColor;
                scrollBarColor = uiColor.PrimaryColor;
                scrollBarBackColor = fillColor;
            }

            if (btn != null)
            {
                btn.ForeColor = uiColor.ButtonForeColor;
                btn.FillColor = uiColor.ButtonFillColor;
                btn.RectColor = uiColor.RectColor;

                btn.FillHoverColor = uiColor.ButtonFillHoverColor;
                btn.RectHoverColor = uiColor.RectHoverColor;
                btn.ForeHoverColor = uiColor.ButtonForeHoverColor;

                btn.FillPressColor = uiColor.ButtonFillPressColor;
                btn.RectPressColor = uiColor.RectPressColor;
                btn.ForePressColor = uiColor.ButtonForePressColor;
            }

            Invalidate();
        }

        private Color scrollBarColor = Color.FromArgb(80, 160, 255);

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("填充颜色"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ScrollBarColor
        {
            get => scrollBarColor;
            set
            {
                scrollBarColor = value;
                bar.HoverColor = bar.PressColor = bar.ForeColor = value;
                Invalidate();
            }
        }

        private Color scrollBarBackColor = Color.White;

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("填充颜色"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ScrollBarBackColor
        {
            get => scrollBarBackColor;
            set
            {
                scrollBarBackColor = value;
                bar.FillColor = value;
                _style = UIStyle.Custom;
                Invalidate();
            }
        }

        protected override void AfterSetForeColor(Color color)
        {
            base.AfterSetForeColor(color);
            edit.ForeColor = GetForeColor();
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            edit.BackColor = GetFillColor();
            bar.FillColor = color;
        }

        protected override void AfterSetFillReadOnlyColor(Color color)
        {
            base.AfterSetFillReadOnlyColor(color);
            edit.BackColor = GetFillColor();
        }

        protected override void AfterSetForeReadOnlyColor(Color color)
        {
            base.AfterSetForeReadOnlyColor(color);
            edit.ForeColor = GetForeColor();
        }

        public enum UIEditType
        {
            /// <summary>
            /// 字符串
            /// </summary>
            String,

            /// <summary>
            /// 整数
            /// </summary>
            Integer,

            /// <summary>
            /// 浮点数
            /// </summary>
            Double
        }

        [DefaultValue(false)]
        public bool AcceptsReturn
        {
            get => edit.AcceptsReturn;
            set => edit.AcceptsReturn = value;
        }

        [DefaultValue(AutoCompleteMode.None), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get => edit.AutoCompleteMode;
            set => edit.AutoCompleteMode = value;
        }

        [
            DefaultValue(AutoCompleteSource.None),
            TypeConverterAttribute(typeof(TextBoxAutoCompleteSourceConverter)),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource
        {
            get => edit.AutoCompleteSource;
            set => edit.AutoCompleteSource = value;
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            Localizable(true),
            Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => edit.AutoCompleteCustomSource;
            set => edit.AutoCompleteCustomSource = value;
        }

        [DefaultValue(CharacterCasing.Normal)]
        public CharacterCasing CharacterCasing
        {
            get => edit.CharacterCasing;
            set => edit.CharacterCasing = value;
        }

        public void Paste(string text)
        {
            edit.Paste(text);
        }

        internal class TextBoxAutoCompleteSourceConverter : EnumConverter
        {
            public TextBoxAutoCompleteSourceConverter(Type type) : base(type)
            {
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection values = base.GetStandardValues(context);
                ArrayList list = new ArrayList();
                int count = values.Count;
                for (int i = 0; i < count; i++)
                {
                    string currentItemText = values[i].ToString();
                    if (currentItemText != null && !currentItemText.Equals("ListItems"))
                    {
                        list.Add(values[i]);
                    }
                }

                return new StandardValuesCollection(list);
            }
        }

        [DefaultValue(false)]
        public bool AcceptsTab
        {
            get => edit.AcceptsTab;
            set => edit.AcceptsTab = value;
        }

        [DefaultValue(false)]
        public bool EnterAsTab
        {
            get => edit.EnterAsTab;
            set => edit.EnterAsTab = value;
        }

        [DefaultValue(true)]
        public bool ShortcutsEnabled
        {
            get => edit.ShortcutsEnabled;
            set => edit.ShortcutsEnabled = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanUndo
        {
            get => edit.CanUndo;
        }

        [DefaultValue(true)]
        public bool HideSelection
        {
            get => edit.HideSelection;
            set => edit.HideSelection = value;
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            MergableProperty(false),
            Localizable(true),
            Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))
        ]
        public string[] Lines
        {
            get => edit.Lines;
            set => edit.Lines = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get => edit.Modified;
            set => edit.Modified = value;
        }

        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int PreferredHeight
        {
            get => edit.PreferredHeight;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get => edit.SelectedText;
            set => edit.SelectedText = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get => edit.SelectionLength;
            set => edit.SelectionLength = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get => edit.SelectionStart;
            set => edit.SelectionStart = value;
        }

        [Browsable(false)]
        public int TextLength
        {
            get => edit.TextLength;
        }

        public void AppendText(string text)
        {
            edit.AppendText(text);
        }

        public void ClearUndo()
        {
            edit.ClearUndo();
        }

        public void Copy()
        {
            edit.Copy();
        }

        public void Cut()
        {
            edit.Cut();
        }

        public void Paste()
        {
            edit.Paste();
        }

        public char GetCharFromPosition(Point pt)
        {
            return edit.GetCharFromPosition(pt);
        }

        public int GetCharIndexFromPosition(Point pt)
        {
            return edit.GetCharIndexFromPosition(pt);
        }

        public int GetLineFromCharIndex(int index)
        {
            return edit.GetLineFromCharIndex(index);
        }

        public Point GetPositionFromCharIndex(int index)
        {
            return edit.GetPositionFromCharIndex(index);
        }

        public int GetFirstCharIndexFromLine(int lineNumber)
        {
            return edit.GetFirstCharIndexFromLine(lineNumber);
        }

        public int GetFirstCharIndexOfCurrentLine()
        {
            return edit.GetFirstCharIndexOfCurrentLine();
        }

        public void DeselectAll()
        {
            edit.DeselectAll();
        }

        public void Undo()
        {
            edit.Undo();
        }

        private Image icon;
        [Description("图标"), Category("SunnyUI")]
        [DefaultValue(null)]
        public Image Icon
        {
            get => icon;
            set
            {
                icon = value;
                SizeChange();
                Invalidate();
            }
        }

        private int iconSize = 24;
        [Description("图标大小(方形)"), Category("SunnyUI"), DefaultValue(24)]
        public int IconSize
        {
            get => iconSize;
            set
            {
                iconSize = Math.Min(UIGlobal.EditorMinHeight, value);
                SizeChange();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (multiline) return;
            if (icon != null)
            {
                e.Graphics.DrawImage(icon, new Rectangle(4, (Height - iconSize) / 2, iconSize, iconSize), new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
            }
            else if (Symbol != 0)
            {
                e.Graphics.DrawFontImage(Symbol, SymbolSize, SymbolColor, new Rectangle(4 + symbolOffset.X, (Height - SymbolSize) / 2 + 1 + symbolOffset.Y, SymbolSize, SymbolSize), SymbolOffset.X, SymbolOffset.Y);
            }
        }

        public Color _symbolColor = UIFontColor.Primary;
        [DefaultValue(typeof(Color), "48, 48, 48")]
        [Description("字体图标颜色"), Category("SunnyUI")]
        public Color SymbolColor
        {
            get => _symbolColor;
            set
            {
                _symbolColor = value;
                Invalidate();
            }
        }

        private int _symbol;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(0)]
        [Description("字体图标"), Category("SunnyUI")]
        public int Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                SizeChange();
                Invalidate();
            }
        }

        private int _symbolSize = 24;

        [DefaultValue(24)]
        [Description("字体图标大小"), Category("SunnyUI")]
        public int SymbolSize
        {
            get => _symbolSize;
            set
            {
                _symbolSize = Math.Max(value, 16);
                _symbolSize = Math.Min(value, UIGlobal.EditorMaxHeight);
                SizeChange();
                Invalidate();
            }
        }

        private Point symbolOffset = new Point(0, 0);

        [DefaultValue(typeof(Point), "0, 0")]
        [Description("字体图标的偏移位置"), Category("SunnyUI")]
        public Point SymbolOffset
        {
            get => symbolOffset;
            set
            {
                symbolOffset = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("Sunny.UI.UIImagePropertyEditor, " + AssemblyRefEx.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(361761)]
        [Description("按钮字体图标"), Category("SunnyUI")]
        public int ButtonSymbol
        {
            get => btn.Symbol;
            set => btn.Symbol = value;
        }

        [DefaultValue(24)]
        [Description("按钮字体图标大小"), Category("SunnyUI")]
        public int ButtonSymbolSize
        {
            get => btn.SymbolSize;
            set => btn.SymbolSize = value;
        }

        [DefaultValue(typeof(Point), "0, 0")]
        [Description("按钮字体图标的偏移位置"), Category("SunnyUI")]
        public Point ButtonSymbolOffset
        {
            get => btn.SymbolOffset;
            set => btn.SymbolOffset = value;
        }

        /// <summary>
        /// 填充颜色，当值为背景色或透明色或空值则不填充
        /// </summary>
        [Description("按钮填充颜色"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonFillColor
        {
            get => btn.FillColor;
            set
            {
                btn.FillColor = value;
                _style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// 字体颜色
        /// </summary>
        [Description("按钮字体颜色"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonForeColor
        {
            get => btn.ForeColor;
            set
            {
                btn.SymbolColor = btn.ForeColor = value;
                _style = UIStyle.Custom;
            }
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        [Description("按钮边框颜色"), Category("SunnyUI")]
        [DefaultValue(typeof(Color), "80, 160, 255")]
        public Color ButtonRectColor
        {
            get => btn.RectColor;
            set
            {
                btn.RectColor = value;
                _style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "111, 168, 255"), Category("SunnyUI")]
        [Description("按钮鼠标移上时填充颜色")]
        public Color ButtonFillHoverColor
        {
            get => btn.FillHoverColor;
            set
            {
                btn.FillHoverColor = value;
                _style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("按钮鼠标移上时字体颜色")]
        public Color ButtonForeHoverColor
        {
            get => btn.ForeHoverColor;
            set
            {
                btn.SymbolHoverColor = btn.ForeHoverColor = value;
                _style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "111, 168, 255"), Category("SunnyUI")]
        [Description("鼠标移上时边框颜色")]
        public Color ButtonRectHoverColor
        {
            get => btn.RectHoverColor;
            set
            {
                btn.RectHoverColor = value;
                _style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "74, 131, 229"), Category("SunnyUI")]
        [Description("按钮鼠标按下时填充颜色")]
        public Color ButtonFillPressColor
        {
            get => btn.FillPressColor;
            set
            {
                btn.FillPressColor = value;
                _style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "White"), Category("SunnyUI")]
        [Description("按钮鼠标按下时字体颜色")]
        public Color ButtonForePressColor
        {
            get => btn.ForePressColor;
            set
            {
                btn.SymbolPressColor = btn.ForePressColor = value;
                _style = UIStyle.Custom;
            }
        }

        [DefaultValue(typeof(Color), "74, 131, 229"), Category("SunnyUI")]
        [Description("按钮鼠标按下时边框颜色")]
        public Color ButtonRectPressColor
        {
            get => btn.RectPressColor;
            set
            {
                btn.RectPressColor = value;
                _style = UIStyle.Custom;
            }
        }
    }
}