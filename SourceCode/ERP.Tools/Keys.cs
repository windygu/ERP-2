using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ERP.Tools
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class Keys
    {
        /// <summary>
        /// 发布环境的URL
        /// </summary>
        public static string ERPUrl = WebConfigurationManager.AppSettings["ERPUrl"].ToString();

        /// <summary>
        /// 定时任务HangFire环境的URL
        /// </summary>
        public static string HangFireUrl = WebConfigurationManager.AppSettings["HangFireUrl"].ToString();

        /// <summary>
        /// 美元
        /// </summary>
        public const string USD = "美元";

        /// <summary>
        /// 人民币
        /// </summary>
        public const string RMB = "人民币";

        /// <summary>
        /// 美元符号 $
        /// </summary>
        public const string USD_Sign = "$";

        /// <summary>
        /// 人民币符号 ¥
        /// </summary>
        public const string RMB_Sign = "¥";

        /// <summary>
        /// 值：PO1
        /// </summary>
        public const string CustomerPO = "PO1";

        /// <summary>
        /// 值：PO2
        /// </summary>
        public const string ECHPO = "PO2";

        /// <summary>
        /// 默认页数：10
        /// </summary>
        public const int DEFAULT_PAGE_SIZE = 10;

        /// <summary>
        /// 错误提示。值：出错了！
        /// </summary>
        public const string ErrorMsg = "出错了！";

        /// <summary>
        /// 提示信息Email。字体颜色为红色是必填项。多个收件人、抄送、密送可以用分号;隔开。
        /// </summary>
        public const string Tip_Email = "字体颜色为红色是必填项。多个收件人、抄送、密送可以用分号;隔开。";

        /// <summary>
        /// EasyUI列表的设置：高度=620px
        /// </summary>
        public const string EasyUiDataGridSetting = "height:620";
    }
}