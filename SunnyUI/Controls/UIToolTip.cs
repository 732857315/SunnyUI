﻿using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    public class UIToolTip : ToolTip
    {
        private readonly ConcurrentDictionary<Control, ToolTipControl> ToolTipControls =
            new ConcurrentDictionary<Control, ToolTipControl>();

        public UIToolTip()
        {
            InitOwnerDraw();
        }

        public UIToolTip(IContainer cont)
            : base(cont)
        {
            InitOwnerDraw();
        }

        public new string ToolTipTitle { get; set; } = "ToolTip title";

        [DefaultValue(typeof(Font), "微软雅黑, 9pt")]
        public Font Font { get; set; } = new Font("微软雅黑", 9);

        [DefaultValue(typeof(Font), "微软雅黑, 12pt")]
        public Font TitleFont { get; set; } = new Font("微软雅黑", 12);

        [DefaultValue(typeof(Color), "239, 239, 239")]
        public Color RectColor { get; set; } = UIChartStyles.Dark.ForeColor;

        [DefaultValue(true)] public bool AutoSize { get; set; } = true;

        [DefaultValue(typeof(Size), "100, 70")]
        public Size Size { get; set; } = new Size(100, 70);

        public void SetToolTip(Control control, string description, string title, int symbol, int symbolSize,
            Color symbolColor)
        {
            if (title == null) title = string.Empty;

            if (ToolTipControls.ContainsKey(control))
            {
                ToolTipControls[control].Title = title;
                ToolTipControls[control].Description = description;
                ToolTipControls[control].Symbol = symbol;
                ToolTipControls[control].SymbolSize = symbolSize;
                ToolTipControls[control].SymbolColor = symbolColor;
            }
            else
            {
                var ctrl = new ToolTipControl()
                {
                    Control = control,
                    Title = title,
                    Description = description,
                    Symbol = symbol,
                    SymbolSize = symbolSize,
                    SymbolColor = symbolColor
            };

                ToolTipControls.TryAdd(control, ctrl);
            }

            base.SetToolTip(control, description);
        }

        public void SetToolTip(Control control, string description, string title)
        {
            if (title == null) title = string.Empty;

            if (ToolTipControls.ContainsKey(control))
            {
                ToolTipControls[control].Title = title;
                ToolTipControls[control].Description = description;
            }
            else
            {
                var ctrl = new ToolTipControl()
                {
                    Control = control,
                    Title = title,
                    Description = description
                };

                ToolTipControls.TryAdd(control, ctrl);
            }

            base.SetToolTip(control, description);
        }

        public new void SetToolTip(Control control, string description)
        {
            if (ToolTipControls.ContainsKey(control))
            {
                ToolTipControls[control].Title = string.Empty;
                ToolTipControls[control].Description = description;
            }
            else
            {
                var ctrl = new ToolTipControl
                {
                    Control = control,
                    Title = string.Empty,
                    Description = description
                };

                ToolTipControls.TryAdd(control, ctrl);
            }

            base.SetToolTip(control, description);
        }

        public void RemoveToolTip(Control control)
        {
            if (ToolTipControls.ContainsKey(control))
                ToolTipControls.TryRemove(control, out _);
        }

        public new ToolTipControl GetToolTip(Control control)
        {
            return ToolTipControls.ContainsKey(control) ? ToolTipControls[control] : new ToolTipControl();
        }

        private void InitOwnerDraw()
        {
            OwnerDraw = true;
            Draw += ToolTipExDraw;
            Popup += UIToolTip_Popup;

            BackColor = UIChartStyles.Dark.BackColor;
            ForeColor = UIChartStyles.Dark.ForeColor;
            RectColor = UIChartStyles.Dark.ForeColor;
        }

        private void UIToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (ToolTipControls.ContainsKey(e.AssociatedControl))
            {
                var tooltip = ToolTipControls[e.AssociatedControl];

                if (tooltip.Description.IsValid())
                {
                    if (!AutoSize)
                    {
                        e.ToolTipSize = Size;
                    }
                    else
                    {
                        var bmp = new Bitmap(e.ToolTipSize.Width, e.ToolTipSize.Height);
                        var g = Graphics.FromImage(bmp);

                        int symbolWidth = tooltip.Symbol > 0 ? tooltip.SymbolSize : 0;
                        int symbolHeight = tooltip.Symbol > 0 ? tooltip.SymbolSize : 0;

                        SizeF titleSize = new SizeF(0, 0);
                        if (tooltip.Title.IsValid())
                        {
                            titleSize = g.MeasureString(tooltip.Title, TitleFont);
                        }

                        SizeF textSize = g.MeasureString(tooltip.Description, Font);

                        TitleHeight = (int)Math.Max(symbolHeight, titleSize.Height);

                        e.ToolTipSize = new Size((int)Math.Max(textSize.Width, symbolWidth + titleSize.Width) + 10, (int)textSize.Height + TitleHeight + 10);
                        bmp.Dispose();
                    }
                }
            }
        }

        private int TitleHeight;

        private void ToolTipExDraw(object sender, DrawToolTipEventArgs e)
        {
            if (ToolTipControls.ContainsKey(e.AssociatedControl))
            {
                var tooltip = ToolTipControls[e.AssociatedControl];

                var bounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);

                e.Graphics.FillRectangle(BackColor, bounds);
                e.Graphics.DrawRectangle(RectColor, bounds);

                if (tooltip.Symbol > 0)
                    e.Graphics.DrawFontImage(tooltip.Symbol, tooltip.SymbolSize, tooltip.SymbolColor, new Rectangle(5, 5, tooltip.SymbolSize, tooltip.SymbolSize));
                if (tooltip.Title.IsValid())
                {
                    SizeF sf = e.Graphics.MeasureString(tooltip.Title, TitleFont);
                    e.Graphics.DrawString(tooltip.Title,TitleFont,ForeColor, tooltip.Symbol>0?tooltip.SymbolSize+5:5, (TitleHeight-sf.Height)/2);
                }
                
                e.Graphics.DrawString(e.ToolTipText, Font, ForeColor, 6, TitleHeight + 6);
            }
            else
            {
                e.Graphics.DrawString(e.ToolTipText, e.Font, ForeColor, 0, 0);
            }
        }

        public class ToolTipControl
        {
            public Control Control { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

            public int Symbol { get; set; } = 0;

            public int SymbolSize { get; set; } = 32;

            public Color SymbolColor { get; set; } = UIChartStyles.Dark.ForeColor;
        }
    }
}