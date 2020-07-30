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
 * 文件名称: UIDatetimePicker.cs
 * 文件说明: 日期时间选择框
 * 当前版本: V2.2
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-07-06: V2.2.6 重写下拉窗体，缩短创建时间
******************************************************************************/

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    public sealed partial class UIDatetimePicker : UIDropControl
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UIDateTimePicker
            // 
            this.Name = "UIDatetimePicker";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.ButtonClick += new System.EventHandler(this.UIDatetimePicker_ButtonClick);
            this.ResumeLayout(false);
            this.PerformLayout();

            DropDownStyle = UIDropDownStyle.DropDownList;
        }

        public UIDatetimePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;
        }

        public delegate void OnDateTimeChanged(object sender, DateTime value);


        public event OnDateTimeChanged ValueChanged;

        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            Value = (DateTime)value;
            Text = Value.ToString(dateFormat);
            Invalidate();
            ValueChanged?.Invoke(this, Value);
        }

        private readonly UIDateTimeItem item = new UIDateTimeItem();

        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        public DateTime Value
        {
            get => item.Date;
            set
            {
                Text = value.ToString(dateFormat);
                item.Date = value;
            }
        }

        private void UIDatetimePicker_ButtonClick(object sender, EventArgs e)
        {
            item.Date = Value;
            ItemForm.Show(this);
        }

        private string dateFormat = "yyyy-MM-dd HH:mm:ss";

        [Description("日期格式化掩码"), Category("自定义")]
        [DefaultValue("yyyy-MM-dd HH:mm:ss")]
        public string DateFormat
        {
            get => dateFormat;
            set
            {
                dateFormat = value;
                Text = Value.ToString(dateFormat);
            }
        }
    }
}
