﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2020 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@qq.com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * 文件名称: UIPieChart.cs
 * 文件说明: 饼状图
 * 当前版本: V2.2
 * 创建日期: 2020-06-06
 *
 * 2020-06-06: V2.2.5 增加文件说明
******************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    public sealed class UIPieChart : UIChart
    {
        protected override void CreateEmptyOption()
        {
            if (emptyOption != null) return;

            emptyOption = new UIOption();

            emptyOption.Title = new UITitle();
            emptyOption.Title.Text = "SunnyUI";
            emptyOption.Title.SubText = "PieChart";

            var series = new UISeries();
            series.Name = "饼状图";
            series.Type = UISeriesType.Pie;
            series.Center = new UICenter(50, 55);
            series.Radius = 70;
            for (int i = 0; i < 5; i++)
            {
                series.AddData("Data" + i, (i + 1) * 20);
            }

            emptyOption.Series.Add(series);
        }

        protected override void DrawTitle(Graphics g, UITitle title)
        {
            if (title == null) return;
            SizeF sf = g.MeasureString(title.Text, Font);
            float left = 0;
            switch (title.Left)
            {
                case UILeftAlignment.Left: left = TextInterval; break;
                case UILeftAlignment.Center: left = (Width - sf.Width) / 2.0f; break;
                case UILeftAlignment.Right: left = Width - TextInterval - sf.Width; break;
            }

            float top = 0;
            switch (title.Top)
            {
                case UITopAlignment.Top: top = TextInterval; break;
                case UITopAlignment.Center: top = (Height - sf.Height) / 2.0f; break;
                case UITopAlignment.Bottom: top = Height - TextInterval - sf.Height; break;
            }

            g.DrawString(title.Text, Font, ChartStyle.ForeColor, left, top);

            SizeF sfs = g.MeasureString(title.SubText, SubFont);
            switch (title.Left)
            {
                case UILeftAlignment.Left: left = TextInterval; break;
                case UILeftAlignment.Center: left = (Width - sfs.Width) / 2.0f; break;
                case UILeftAlignment.Right: left = Width - TextInterval - sf.Width; break;
            }
            switch (title.Top)
            {
                case UITopAlignment.Top: top = top + sf.Height; break;
                case UITopAlignment.Center: top = top + sf.Height; break;
                case UITopAlignment.Bottom: top = top - sf.Height; break;
            }

            g.DrawString(title.SubText, subFont, ChartStyle.ForeColor, left, top);
        }

        protected override void CalcData(UIOption o)
        {
            Angles.Clear();
            if (o == null || o.Series == null || o.Series.Count == 0) return;
            UITemplate template = null;
            if (o.ToolTip != null)
            {
                template = new UITemplate(o.ToolTip.formatter);
            }


            for (int pieIndex = 0; pieIndex < o.Series.Count; pieIndex++)
            {
                var pie = o.Series[pieIndex];
                Angles.TryAdd(pieIndex, new ConcurrentDictionary<int, Angle>());

                double all = 0;
                foreach (var data in pie.Data)
                {
                    all += data.Value;
                }

                if (all.IsZero()) return;
                float start = 0;
                for (int i = 0; i < pie.Data.Count; i++)
                {
                    float angle = (float)(pie.Data[i].Value * 360.0f / all);
                    float percent = (float)(pie.Data[i].Value * 100.0f / all);
                    string text = "";
                    if (o.ToolTip != null)
                    {
                        try
                        {
                            if (template != null)
                            {
                                template.Set("a", pie.Name);
                                template.Set("b", pie.Data[i].Name);
                                template.Set("c", pie.Data[i].Value.ToString("F" + DecimalNumber));
                                template.Set("d", percent.ToString("F2"));
                                text = template.Render();
                            }
                        }
                        catch
                        {
                            text = pie.Data[i].Name + " : " + pie.Data[i].Value.ToString("F" + DecimalNumber) + "(" + percent.ToString("F2") + "%)";
                            if (pie.Name.IsValid()) text = pie.Name + '\n' + text;
                        }
                    }

                    Angles[pieIndex].AddOrUpdate(i, new Angle(start, angle, text));
                    start += angle;
                }
            }
        }

        protected override void DrawSeries(Graphics g, UIOption o, List<UISeries> series)
        {
            if (series == null || series.Count == 0) return;

            for (int pieIndex = 0; pieIndex < series.Count; pieIndex++)
            {
                var pie = series[pieIndex];
                RectangleF rect = GetSeriesRect(pie);
                for (int azIndex = 0; azIndex < pie.Data.Count; azIndex++)
                {
                    Color color = ChartStyle.SeriesColor[azIndex % ChartStyle.ColorCount];
                    RectangleF rectx = new RectangleF(rect.X - 10, rect.Y - 10, rect.Width + 20, rect.Width + 20);
                    g.FillPie(color, (ActivePieIndex == pieIndex && ActiveAzIndex == azIndex) ? rectx : rect, Angles[pieIndex][azIndex].Start - 90, Angles[pieIndex][azIndex].Sweep);
                    Angles[pieIndex][azIndex].Size = g.MeasureString(Angles[pieIndex][azIndex].Text, legendFont);
                }
            }
        }

        private readonly ConcurrentDictionary<int, ConcurrentDictionary<int, Angle>> Angles = new ConcurrentDictionary<int, ConcurrentDictionary<int, Angle>>();

        protected override void DrawLegend(Graphics g, UILegend legend)
        {
            if (legend == null) return;

            float totalHeight = 0;
            float totalWidth = 0;
            float maxWidth = 0;
            float oneHeight = 0;

            foreach (var data in legend.Data)
            {
                SizeF sf = g.MeasureString(data, LegendFont);
                totalHeight += sf.Height;
                totalWidth += sf.Width;
                totalWidth += 20;

                maxWidth = Math.Max(sf.Width, maxWidth);
                oneHeight = sf.Height;
            }

            float top = 0;
            float left = 0;

            if (legend.Orient == Orient.Horizontal)
            {
                if (legend.Left == UILeftAlignment.Left) left = TextInterval;
                if (legend.Left == UILeftAlignment.Center) left = (Width - totalWidth) / 2.0f;
                if (legend.Left == UILeftAlignment.Right) left = Width - totalWidth - TextInterval;

                if (legend.Top == UITopAlignment.Top) top = TextInterval;
                if (legend.Top == UITopAlignment.Center) top = (Height - oneHeight) / 2.0f;
                if (legend.Top == UITopAlignment.Bottom) top = Height - oneHeight - TextInterval;
            }

            if (legend.Orient == Orient.Vertical)
            {
                if (legend.Left == UILeftAlignment.Left) left = TextInterval;
                if (legend.Left == UILeftAlignment.Center) left = (Width - maxWidth) / 2.0f - 10;
                if (legend.Left == UILeftAlignment.Right) left = Width - maxWidth - TextInterval - 20;

                if (legend.Top == UITopAlignment.Top) top = TextInterval;
                if (legend.Top == UITopAlignment.Center) top = (Height - totalHeight) / 2.0f;
                if (legend.Top == UITopAlignment.Bottom) top = Height - totalHeight - TextInterval;
            }

            float startleft = left;
            float starttop = top;
            for (int i = 0; i < legend.DataCount; i++)
            {
                var data = legend.Data[i];
                SizeF sf = g.MeasureString(data, LegendFont);
                if (legend.Orient == Orient.Horizontal)
                {
                    g.FillRoundRectangle(ChartStyle.SeriesColor[i % ChartStyle.ColorCount], (int)startleft, (int)top + 1, 18, (int)oneHeight - 2, 5);
                    g.DrawString(data, LegendFont, ChartStyle.ForeColor, startleft + 20, top);
                    startleft += 20;
                    startleft += sf.Width;
                }

                if (legend.Orient == Orient.Vertical)
                {
                    g.FillRoundRectangle(ChartStyle.SeriesColor[i % ChartStyle.ColorCount], (int)left, (int)starttop + 1, 18, (int)oneHeight - 2, 5);
                    g.DrawString(data, LegendFont, ChartStyle.ForeColor, left + 20, starttop);
                    starttop += oneHeight;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UIOption option = Option ?? EmptyOption;

            if (option.SeriesCount == 0)
            {
                SetPieAndAzIndex(-1, -1);
                return;
            }

            for (int pieIndex = 0; pieIndex < option.SeriesCount; pieIndex++)
            {
                RectangleF rect = GetSeriesRect(option.Series[pieIndex]);
                if (!e.Location.InRect(rect)) continue;

                PointF pf = new PointF(rect.Left + rect.Width / 2.0f, rect.Top + rect.Height / 2.0f);
                if (MathEx.CalcDistance(e.Location, pf) * 2 > rect.Width) continue;

                double az = MathEx.CalcAngle(e.Location, pf);
                for (int azIndex = 0; azIndex < option.Series[pieIndex].Data.Count; azIndex++)
                {
                    if (az >= Angles[pieIndex][azIndex].Start && az <= Angles[pieIndex][azIndex].Start + Angles[pieIndex][azIndex].Sweep)
                    {
                        SetPieAndAzIndex(pieIndex, azIndex);
                        if (tip.Text != Angles[pieIndex][azIndex].Text)
                        {
                            tip.Text = Angles[pieIndex][azIndex].Text;
                            tip.Size = new Size((int)Angles[pieIndex][azIndex].Size.Width + 4, (int)Angles[pieIndex][azIndex].Size.Height + 4);
                        }

                        if (az >= 0 && az < 90)
                        {
                            tip.Top = e.Location.Y + 20;
                            tip.Left = e.Location.X - tip.Width;
                        }
                        else if (az >= 90 && az < 180)
                        {
                            tip.Left = e.Location.X - tip.Width;
                            tip.Top = e.Location.Y - tip.Height - 2;
                        }
                        else if (az >= 180 && az < 270)
                        {
                            tip.Left = e.Location.X;
                            tip.Top = e.Location.Y - tip.Height - 2;
                        }
                        else if (az >= 270 && az < 360)
                        {
                            tip.Left = e.Location.X + 15;
                            tip.Top = e.Location.Y + 20;
                        }

                        if (!tip.Visible) tip.Visible = Angles[pieIndex][azIndex].Text.IsValid();
                        return;
                    }
                }
            }

            SetPieAndAzIndex(-1, -1);
            tip.Visible = false;
        }

        private int ActiveAzIndex = -1;
        private int ActivePieIndex = -1;

        private void SetPieAndAzIndex(int pieIndex, int azIndex)
        {
            if (ActivePieIndex != pieIndex || ActiveAzIndex != azIndex)
            {
                ActivePieIndex = pieIndex;
                ActiveAzIndex = azIndex;
                Invalidate();
            }
        }

        private RectangleF GetSeriesRect(UISeries series)
        {
            int left = series.Center.Left;
            int top = series.Center.Top;
            left = Width * left / 100;
            top = Height * top / 100;
            float halfRadius = Math.Min(Width, Height) * series.Radius / 200.0f;
            return new RectangleF(left - halfRadius, top - halfRadius, halfRadius * 2, halfRadius * 2);
        }

        public class Angle
        {
            public float Start { get; set; }
            public float Sweep { get; set; }

            public Angle(float start, float sweep, string text)
            {
                Start = start;
                Sweep = sweep;
                Text = text;
            }

            public string Text { get; set; }

            public SizeF Size { get; set; }
        }
    }
}