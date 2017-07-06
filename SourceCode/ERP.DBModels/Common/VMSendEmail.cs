using System.Collections.Generic;
using System.Net.Mail;

namespace ERP.Models.Common
{
    /// <summary>
    /// 电子邮件Model
    /// </summary>
    public class VMSendEmail
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string BodyContent { get; set; }

        /// <summary>
        /// 是否内容支持Html
        /// </summary>
        public bool IsBodyHtml { get; set; }

        /// <summary>
        /// 附件。多个附件用;隔开
        /// </summary>
        public string Attachs { get; set; }

        /// <summary>
        /// 抄送。多个抄送用;隔开
        /// </summary>
        public string CcAddress { get; set; }

        /// <summary>
        /// 密送。多个密送用;隔开
        /// </summary>
        public string BccAddress { get; set; }

        /// <summary>
        /// 发送电邮方式。默认Network
        /// </summary>
        public SmtpDeliveryMethod DeliveryMethod { get; set; }

        /// <summary>
        /// 是否加密。默认false
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// 主机地址。默认136.196.110.1
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口号。默认25
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否包含生成的附件。true:包含,false:不包含。
        /// </summary>
        public bool IsContainMakerExcel { get; set; }

        /// <summary>
        /// 是否包含生成的附件。true:包含,false:不包含。
        /// </summary>
        public bool IsContainMakerExcel_pdf { get; set; }

        /// <summary>
        /// 是否包含生成的附件。true:包含,false:不包含。
        /// </summary>
        public bool IsContainMakerExcel_jpg { get; set; }

        public int StatusID { get; set; }

        public List<VMUpLoadFile> UpLoadFileList { get; set; }
    }
}