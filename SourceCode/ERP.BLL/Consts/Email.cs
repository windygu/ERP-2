using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Tools.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;

namespace ERP.Tools
{
    public class Email
    {
        /// <summary>
        /// 邮件配置
        /// </summary>
        private class MailSetting
        {
            public string MailServer;
            public int MailSendPort;
            public string MailSendAddress;
            public string MailSendName;
            public string MailSendPwd;
            public bool EnableSsl;

            public static MailSetting GetSetting(string MailSendAddress, string MailSendPwd)
            {

                if (!string.IsNullOrEmpty(MailSendPwd))
                {
                    MailSendPwd = DTRequest.Decrypt(MailSendPwd);
                }

                MailSetting setting = new MailSetting
                {
                    MailServer = WebConfigurationManager.AppSettings["MailServer"],
                    MailSendPort = Convert.ToInt32(WebConfigurationManager.AppSettings["MailSendPort"]),
                    MailSendName = WebConfigurationManager.AppSettings["MailSendName"],

                    //MailSendAddress = WebConfigurationManager.AppSettings["MailSendAddress"],
                    //MailSendPwd = WebConfigurationManager.AppSettings["MailSendPwd"],
                    MailSendAddress = MailSendAddress,
                    MailSendPwd = MailSendPwd,

                    EnableSsl = Convert.ToBoolean(WebConfigurationManager.AppSettings["EnableSsl"]),
                };

                return setting;
            }
        }

        /// <summary>
        /// 把string转换成List
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<MailAddress> StrToList(string str)
        {
            List<MailAddress> list = new List<MailAddress>();
            if (!string.IsNullOrEmpty(str))
            {
                var temp = str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in temp)
                {
                    list.Add(new MailAddress(item));
                }
            }
            return list;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toAddress">目标地址列表</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="bodyContent">邮件正文</param>
        /// <param name="attachs">邮件附件,格式要求相对路径(网站虚拟路径)</param>
        /// <param name="mailType">邮件类别(用来记录日志)</param>
        /// <param name="userID">当前用户</param>
        public static VMAjaxProcessResult SendEmail(string toAddress, string subject, string bodyContent, List<string> attachs, MailType mailType, VMERPUser currentUser, string ccAddress = null, string bccAddress = null)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                MailSetting Setting = MailSetting.GetSetting(currentUser.Email, currentUser.EmailPassword);

                if (string.IsNullOrEmpty(Setting.MailSendAddress) || string.IsNullOrEmpty(Setting.MailServer))
                {
                    result.IsSuccess = false;
                    result.Msg = "邮件发送失败，邮件服务器配置错误！";
                    return result;
                }
                if (string.IsNullOrEmpty(Setting.MailSendPwd))
                {
                    result.IsSuccess = false;
                    result.Msg = "邮件发送失败，发件人的密码不能为空！";
                    return result;
                }


                // 读取附件
                List<Attachment> attachments = new List<Attachment>();
                foreach (var attach in attachs)
                {
                    if (!string.IsNullOrEmpty(attach))
                    {
                        attachments.Add(new Attachment(attach));
                    }
                }

                Encoding encoder = Encoding.UTF8;


                List<MailAddress> list_ToAddress = StrToList(toAddress);
                List<MailAddress> list_CcAddress = StrToList(ccAddress);
                List<MailAddress> list_BccAddress = StrToList(bccAddress);

                MailMessage msg = new MailMessage();
                list_ToAddress.ForEach(t => msg.To.Add(t));

                if (list_CcAddress != null && list_CcAddress.Count > 0)
                {
                    list_CcAddress.ForEach(t => msg.CC.Add(t));
                }
                if (list_BccAddress != null && list_BccAddress.Count > 0)
                {
                    list_BccAddress.ForEach(t => msg.Bcc.Add(t));
                }

                msg.From = new MailAddress(Setting.MailSendAddress, Setting.MailSendName, encoder);
                msg.Subject = subject;
                msg.SubjectEncoding = encoder;
                msg.Body = bodyContent;
                msg.BodyEncoding = encoder;
                msg.IsBodyHtml = false;
                msg.Priority = MailPriority.Normal;
                if (attachments != null)
                {
                    foreach (Attachment attachment in attachments)
                    {
                        msg.Attachments.Add(attachment);
                    }
                }

                SmtpClient client = new SmtpClient();
                //client.UseDefaultCredentials = true;

                if (Setting.MailSendPwd.Length > 0)
                {
                    client.Credentials = new NetworkCredential(Setting.MailSendAddress, Setting.MailSendPwd);
                }

                client.Host = Setting.MailServer;
                client.Port = Setting.MailSendPort;
                client.EnableSsl = Setting.EnableSsl;
                client.Send(msg);

                // 邮件发送记录保存到数据库
                LogHelper.WriteEmail(JsonConvert.SerializeObject(new
                {
                    UserName = currentUser.UserName,
                    Host = Setting.MailSendAddress,
                    FromAddress = msg.From,
                    ToAddress = toAddress,
                    Subject = subject,
                    BodyContent = bodyContent,
                    Attachs = attachs,
                    MailType = mailType
                }));

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "邮件发送失败，" + ex.Message;
                LogHelper.WriteError(ex);
                return result;
            }
            return result;
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static bool SendEmail(string userName, VMSendEmail vm)
        {
            var FromAddress = new System.Net.Mail.MailAddress(vm.FromAddress);

            List<MailAddress> list_ToAddress = StrToList(vm.ToAddress);
            List<MailAddress> list_CcAddress = StrToList(vm.CcAddress);
            List<MailAddress> list_BccAddress = StrToList(vm.BccAddress);

            List<string> list_Attachs = new List<string>();
            if (!string.IsNullOrEmpty(vm.Attachs))
            {
                var attachs = vm.Attachs.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in attachs)
                {
                    list_Attachs.Add(item);
                }
            }

            vm.Host = "136.196.110.1";
            vm.Port = 25;

            return SendMail(userName, vm.Host, vm.Port, FromAddress, list_ToAddress, vm.Subject, vm.BodyContent, vm.IsBodyHtml, list_Attachs, list_CcAddress, list_BccAddress, vm.DeliveryMethod, vm.EnableSsl);
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="userName">当前用户名</param>
        /// <param name="host">主机名</param>
        /// <param name="port">端口号</param>
        /// <param name="fromAddress">发送人</param>
        /// <param name="toAddress">收件人</param>
        /// <param name="subject">主题</param>
        /// <param name="bodyContent">内容</param>
        /// <param name="isBodyHtml">是否内容支持Html</param>
        /// <param name="attachs">附件。格式要求相对路径</param>
        /// <param name="ccAddress">抄送</param>
        /// <param name="bccAddress">密送</param>
        /// <param name="deliveryMethod">发送电邮方式。默认Network</param>
        /// <param name="enableSsl">是否加密</param>
        /// <returns></returns>
        public static bool SendMail(string userName, string host, int port, MailAddress fromAddress, List<MailAddress> toAddress, string subject, string bodyContent,
            bool isBodyHtml = false, List<string> attachs = null, List<MailAddress> ccAddress = null, List<MailAddress> bccAddress = null, SmtpDeliveryMethod deliveryMethod = SmtpDeliveryMethod.Network, bool enableSsl = false)
        {
            bool result = false;
            try
            {
                LogHelper.WriteEmail(JsonConvert.SerializeObject(new
                {
                    UserName = userName,
                    Host = host,
                    Port = port,
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    Subject = subject,
                    BodyContent = bodyContent,
                    IsBodyHtml = isBodyHtml,
                    Attachs = attachs,
                    CCAddress = ccAddress,
                    BCCAddress = bccAddress,
                    DeliveryMethod = deliveryMethod,
                    EnableSsl = enableSsl
                }));

                if (fromAddress == null)
                {
                    throw new Exception("发件人不能为空！");
                }

                if (toAddress == null || toAddress.Count == 0)
                {
                    throw new Exception("至少有一个收件人！");
                }

                MailMessage msg = new MailMessage();
                msg.SubjectEncoding = Encoding.UTF8;
                msg.BodyEncoding = Encoding.UTF8;

                msg.From = fromAddress;
                toAddress.ForEach(t => msg.To.Add(t));
                if (ccAddress != null && ccAddress.Count > 0)
                {
                    ccAddress.ForEach(t => msg.CC.Add(t));
                }
                if (bccAddress != null && bccAddress.Count > 0)
                {
                    bccAddress.ForEach(t => msg.Bcc.Add(t));
                }

                msg.Subject = subject;
                msg.Body = bodyContent;
                msg.IsBodyHtml = isBodyHtml;
                msg.Priority = MailPriority.Normal;

                // 附件
                if (attachs != null && attachs.Count > 0)
                {
                    List<Attachment> attachments = new List<Attachment>();
                    foreach (var attach in attachs)
                    {
                        msg.Attachments.Add(new Attachment(attach));
                    }
                }

                // deliveryMethod使用PickupDirectoryFromIis部署之后一直报错，应该和服务器权限有关，因为是直接通过服务器的IIS发送，直接使用Network进行发送时并没有异常
                SmtpClient client = new SmtpClient(host, port) { DeliveryMethod = deliveryMethod };
                client.EnableSsl = enableSsl;
                client.Send(msg);

                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw ex;
            }
            return result;
        }
    }
}