\+ 增加； \- 删除； \* 修改

#### 2021-04-11 V3.0.2
\+ UIMarkLabel：增加带颜色标签的Label
\+ UIRoundProcess：圆形滚动条
\+ UIBreadcrumb：增加面包屑导航
\+ UILedLabel：增加Led标签
\* UIHeaderButton：在工具箱中显示
\* UILineChart：支持拖拽选取放大
\* UIDateTimePicker：修复下拉选择日期后关闭的Bug
\* UINavMenu：增加设置二级菜单底色
\* UIColorPicker：增加单击事件以选中颜色
\* UITitlePage：增加ShowTitle可控制是否显示标题
\* UINavBar：增加可设置背景图片
\* 框架增加IFrame接口，方便页面跳转
\* UIDataGridView：修改垂直滚动条和原版一致，并增加翻页方式滚动
\* UIPagination: 修正因两次查询数量相等而引起的不刷新
\* UIHeaderButton: 增加字体图标背景时鼠标移上背景色
\* UITabControl：修改第一个TabPage关不掉的Bug
\* UIDataGridView：增加EnterAsTab属性，编辑输入时，用Enter键代替Tab键跳到下一个单元格
\* UILineChart：增加鼠标框选放大，可多次放大，右键点击恢复一次，双击恢复
\* UITitlePanel：修复OnMouseMove事件
\* UITrackBar：增加垂直显示方式
\* UIFlowLayoutPanel：修改了一处因为其加入控件大小发生变化而引起的滚动条出错。

#### 2021-02-26 V3.0.1
\+ UIForm：标题栏增加扩展按钮
\+ UIHeaderButton：新增大图标的导航按钮
\+ 新增UIComboboxEx，从Combobox原生控件继承，以方便做查询过滤等操作
\* UIForm：修正不显示标题栏时，标题栏位置可放置控件
\* UIListBox：增加一些原有属性
\* FCombobox：增加数据绑定Demo
\* UICombobox：更改索引改变事件的多次触发
\* UIForm：修改一处Icon图片显示的问题
\* UIEditForm：修改通过代码生成窗体控件的TabIndex
\* UIDatePicker，UIDateTimePicker：将日期选择控件的最小值调整为1900年
\* UIHeaderButton：将其命名空间从Sunny.UI.Control改为Sunny.UI

#### 2021-01-26 V3.0.0
\+ 同时兼容.Net Framework 4.0+、.Net Core3.1、.Net 5 框架
\* 更新UIMessageTip
\* UIForm：增加ShowTitleIcon用来显示标题栏图标，与ShowIcon分开
\* UINavBar：增加下拉菜单可设置自动高度或者固定高度，可显示ImageList绑定
\* UIDataGridView更新行头和列头的选中颜色

#### 2021-01-05 V2.2.10
\* V2.2 .Net Framewok 4.0最终版本
\* V3.0 开始将同时兼容.Net Framework 4.0+、.Net Core3.1、.Net 5 框架

#### 2020-12-20 V2.2.9
\+ UIWaitForm：等待窗体
\+ UIComboTreeView：新增下拉框TreeView
\+ UIMessageForm：消息提示框增加黑色半透明遮罩层
\+ Win32API：新增Win32API函数
\+ UJsonConfig：不引用第三方控件，用.Net自带的序列化实现Json，增加Json文件配置类
\+ UIDataGridViewForm：增加了一个表格模板基类
\* UIDataGridView：修改DataSource赋值后Column改变引起的水平滚动条错误
\* UIDoubleUpDown，UIIntegerUpDown：增加双击可编辑数值
\* UINavMenu：增加选中后图标的背景色或应用选中图片索引
\* 页面框架增加页面内跳转方法
\* 日期、时间选择框增加CanEmpty，输入可为空

#### 2020-10-12 V2.2.8
\+ UILineChart：完成曲线图表
\+ UIScale：增加坐标轴刻度计算类
\+ UIFlowLayoutPanel：增加
\+ UIBarChartEx：增加了一个新的柱状图类型，序列个数可以不相等
\+ UDateTimeInt64：增加DateTimeInt64类，时间整形互转类
\* UIForm：增加窗体阴影
\* UIMainFrame：页面框架增加Selecting事件，在页面切换时执行该事件
\* UITextBox：解决Anchor包含Top、Bottom时，在窗体最小化后恢复时高度变化
\* UISwitch：增加长方形形状开关，取消长宽比锁定
\* UITreeView：背景色可改，设置FillColor，以及SystemCustomMode = true
\* UIDataGridView：解决水平滚动条在有列冻结时出错的问题

#### 2020-09-17 V2.2.7
\+ 新增双主键线程安全字典，分组线程安全字典
\+ UIHorScrollBarEx，UIVerScrollBarEx：重写了两个滚动条
\* UIForm：恢复了WindowState，增加了窗体可拉拽调整大小
\* 增加控件属性显示值及Sunny UI分类
\* UIDateTimePicker,UITimePicker：更改滚轮选择时间的方向
\* UIButton：Tips颜色可设置
\* UIChart：增加图表的边框线颜色设置
\* UITextBox：增加FocusedSelectAll属性，激活时全选。
\* UINavBar：增加节点的Image绘制
\* UIDataGridView：调整水平滚动条
\* UIButton：添加'是否启用双击事件'属性，解决连续点击效率问题
\* UIDataGridView：更新了水平和垂直滚动条的显示，优化滚动效果。
\* UIBbutton：空格键按下press背景效果
\* UIListBox优化滚轮快速滚动流畅性
\* UIBarChart：可设置柱状图最小宽度
\* UIIntegerUpDown, UIDoubleUpDown：增加字体调整
\* UITabControl：标题垂直居中
\* UITreeView：更新可设置背景色
\* UIDatePicker，UITimePicker，UIDateTimePicker：可编辑输入，日期范围控制
\* UIDatePicker：更改日期范围最小值和最大值
\* UITitlePanel：更新大小调整后的按钮位置

#### 2020-07-30 V2.2.6
\+ UIPagination：新增分页控件
\+ UIToolTip：新增控件，可修改字体
\+ UIHorScrollBar：新增水平滚动条
\+ UIWaitingBar：新增等待滚动条控件
\* UIDataGridView：重绘水平滚动条，更新默认设置为原生控件设置
\* UITitlePanel：增加可收缩选项
\* UIPieChart,UIBarChart：增加序列自定义颜色
\* UISymbolButton：增加Image属性，增加图片和文字的摆放位置
\* UIButton：增加Selected及选中颜色配置
\* UIForm：支持点击窗体任务栏图标，可以进行最小化
\* UIForm：增加标题栏ICON图标绘制
\* UIDateTimePicker：重写下拉窗体，缩短创建时间
\* UITreeView：全部重写，增加圆角，CheckBoxes等
\* UIDatePicker：重写下拉窗体，缩短创建时间
\* UICheckBoxGroup,UIRadioButtonGroup：可以设置初始选中值
\* UILedBulb：边缘平滑
\* UIForm：仿照QQ，重绘标题栏按钮。

#### 2020-06-29 V2.2.5
\+ UIDoughnutChart：环状图
\+ UILoginForm：登录窗体
\+ UIScrollingText：滚动文字
\+ UIBarChart：柱状图
\+ UIPieChart：饼状图
\+ UIRichTextBox：富文本框
\+ UIBattery：电池电量显示
\+ UIDatetimePicker：日期时间选择框
\+ UIColorPicker：颜色选择框
\+ UITimePicker：时间选择框
\+ UIMessageTipHelper：增加MessageTip扩展方法
\* UIComboBox：增加数据绑定
\* 页面框架支持通过PageIndex和PageGuid关联
\* UITextBox：增加Multiline属性，增加滚动条
\* UITabControl：新增关闭按钮，重绘左右移动按钮
\* UIForm：更新标题移动、双击最大化/正常、到顶最大化、最大化后拖拽正常
\* UINavMenu：增加字体图标显示
\* 字体图标字体调整从资源文件中加载字体，不用另存为文件
\* UIListBox 增加跟随鼠标滑过高亮
\* UIDatePicker：重写日期选择界面
\* UIButton：增加ShowFocusLine，可获得焦点并显示

#### 2020.05.05 V2.2.5
\+ 增加页面框架
\+ 增加下拉框窗体，进度提升窗体
\+ UITreeView

#### 2020.04.25 V2.2.4
\* 更新主题风格类，各控件主题颜色调用不交叉，便于新增主题
\+ 更新Sunny.Demo程序
\+ 增加UIDataGridView，基于DataGridView增强、美化
\- UIGrid效率待改，暂时隐藏

#### 2020.04.19 V2.2.3
\+ UICheckBoxGroup,UIRadioButtonGroup

#### 2020.04.11 V2.2.2
\+ 新增UIGrid
\- 继承DataGridView更改主题风格的UIGridView

#### 2020.02.15 V2.2.1
\* Bug修复

#### 2020.01.01 V2.2.0
\* 增加文件说明，为开源做准备
\+ 增加Office主题风格

#### 2019.10.01 V2.1.0
\+ 增加Element主题风格

#### 2019.03.12 V2.0.0
\+ 增加自定义控件

#### 2012.03.31 V1.0.0
\+ 增加工具类、扩展类