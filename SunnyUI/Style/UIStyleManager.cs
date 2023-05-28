﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2023 ShenYongHua(沈永华).
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
 * 文件名称: UIStyleManager.cs
 * 文件说明: 主题样式管理类
 * 当前版本: V3.1
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2021-10-16: V3.0.8 增加系统DPI缩放自适应
******************************************************************************/

using System.ComponentModel;
using static System.Drawing.FontConverter;

namespace Sunny.UI
{
    /// <summary>
    /// 主题样式管理类
    /// </summary>
    public class UIStyleManager : Component
    {
        /// <summary>
        /// 主题样式
        /// </summary>
        [DefaultValue(UIStyle.Blue), Description("主题样式"), Category("SunnyUI")]
        public UIStyle Style
        {
            get => UIStyles.Style;
            set
            {
                if (UIStyles.Style != value && value != UIStyle.Custom)
                {
                    UIStyles.SetStyle(value);
                }
            }
        }

        public void Render()
        {
            if (Style != UIStyle.Custom)
            {
                UIStyles.SetStyle(Style);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UIStyleManager()
        {
            Version = UIGlobal.Version;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        public UIStyleManager(IContainer container) : this()
        {
            container.Add(this);
            Version = UIGlobal.Version;
        }

        [DefaultValue(false), Description("DPI缩放"), Category("SunnyUI")]
        public bool DPIScale
        {
            get => UIStyles.DPIScale;
            set => UIStyles.DPIScale = value;
        }

        [Editor("System.Drawing.Design.FontNameEditor", "System.Drawing.Design.UITypeEditor")]
        [TypeConverter(typeof(FontNameConverter))]
        [DefaultValue("微软雅黑")]
        [Description("全局字体设置开启后，可调字体名称"), Category("SunnyUI")]
        public string GlobalFontName
        {
            get => UIStyles.FontName;
            set => UIStyles.FontName = value;
        }

        [DefaultValue(100)]
        [Description("全局字体设置开启后，可调字体大小缩放百分比，默认100%"), Category("SunnyUI")]
        public int GlobalFontScale
        {
            get => UIStyles.FontScale;
            set => UIStyles.FontScale = value;
        }

        [DefaultValue(false)]
        [Description("全局字体设置"), Category("SunnyUI")]
        public bool GlobalFont
        {
            get => UIStyles.GlobalFont;
            set => UIStyles.GlobalFont = value;
        }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get;
        }
    }
}