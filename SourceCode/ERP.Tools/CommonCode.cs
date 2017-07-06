using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace ERP.Tools
{
    public class CommonCode
    {
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            try
            {
                string result = string.Empty;
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }
                if (string.IsNullOrEmpty(result))
                {
                    return "127.0.0.1";
                }
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取当前时间的时间戳 DateTime.Now.ToString("yyyyMMddHHmmssfff")
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 随机生成销售核算单号。格式：2位年份 + 4位自增数
        /// </summary>
        /// <returns></returns>
        public static string GetRandom_OrderNumber(string maxOrderNumber)
        {
            string thisYear = DateTime.Now.Year.ToString().Substring(2, 2);
            if (string.IsNullOrEmpty(maxOrderNumber))//没有销售核算单号
            {
                return thisYear + "0001";
            }
            else
            {
                int temp = Utils.StrToInt(maxOrderNumber.Substring(2, maxOrderNumber.Length - 2), 0) + 1;
                return thisYear + temp.ToString().PadLeft(4, '0');
            }
        }

        /// <summary>
        /// 随机生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNumber()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss_") + RandomString(3, false);
        }

        /// <summary>
        /// 随机生成字符串
        /// </summary>
        /// <param name="numbers">个数</param>
        /// <param name="hasLetter">是否有英文字母</param>
        /// <returns></returns>
        public static string RandomString(int numbers, bool hasLetter)
        {
            string chars = (hasLetter ? "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" : "0123456789");
            var random = new Random(Environment.TickCount);
            var builder = new StringBuilder(numbers);
            for (int i = 0; i < numbers; ++i)
            {
                builder.Append(chars[random.Next(chars.Length)]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件名</param>
        /// <param name="fileName">下载的文件名</param>
        public static void DownLoadFile(string path, string fileName)
        {
            //以字符流的形式下载文件
            FileStream fs = new FileStream(path, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开

            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            HttpContext.Current.Response.BinaryWrite(bytes);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 获取价格代码
        /// </summary>
        /// <returns></returns>
        public static string GetPriceCode(string price)
        {
            string priceCode = price.Replace("1", "U").Replace("2", "P").Replace("3", "R").Replace("4", "I").Replace("5", "G").Replace("6", "H").Replace("7", "T").Replace("8", "N").Replace("9", "O").Replace("0", "W");
            return priceCode;
        }

        /// <summary>
        /// 把idList转换成List。默认逗号,隔开
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static List<int> IdListToList(string idList)
        {
            List<int> list = null;
            if (!string.IsNullOrEmpty(idList))
            {
                list = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            return list;
        }

        /// <summary>
        /// 把idList转换成List
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static List<int> IdListToList(string idList, char separator)
        {
            List<int> list = null;
            if (!string.IsNullOrEmpty(idList))
            {
                list = idList.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            return list;
        }

        /// <summary>
        /// 把字符串转换成List。默认逗号,隔开
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static List<string> StrToList(string idList, char separator = ',')
        {
            List<string> list = null;
            if (!string.IsNullOrEmpty(idList))
            {
                list = idList.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return list;
        }

        /// <summary>
        /// 发送Post请求到HangFire
        /// </summary>
        public static string PostHangFire()
        {
            string URI = Keys.HangFireUrl + "/Home/HangfireNotify";
            string myParameters = "userName=michael&ToAddress=1044626922@qq.com";
            string method = "POST";
            return PostHangFire(URI, myParameters, method);
        }

        /// <summary>
        /// 发送Post请求到HangFire
        /// </summary>
        public static string PostHangFire(string URI, string myParameters, string method)
        {
            byte[] postData = Encoding.UTF8.GetBytes(myParameters);

            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Headers[System.Net.HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                byte[] HtmlResult = wc.UploadData(URI, method, postData);
                string srcString = Encoding.UTF8.GetString(HtmlResult);//解码
                return srcString;
            }
        }

        /// <summary>
        /// 获取Web.config支持的上传文件的扩展名。格式如：*.jpg;*.jpeg;*.gif;*.bmp;*.png;
        /// </summary>
        /// <returns></returns>
        public static string GetUploadFileExtensions()
        {
            return GetUploadFileExtensions(ConfigurationManager.AppSettings["validUploadFileExtensions"]);
        }

        ///// <summary>
        ///// 获取Web.config支持的上传文件的扩展名。格式如：*.jpg;*.jpeg;*.gif;*.bmp;*.png;
        ///// </summary>
        ///// <returns></returns>
        //public static string GetUploadFileExtensions_Upload()
        //{
        //    return GetUploadFileExtensions("jpg;png;xls;xlsx;doc;docx;pdf;");
        //}

        /// <summary>
        /// 获取Web.config支持的上传文件的扩展名。格式如：*.jpg;*.jpeg;*.gif;*.bmp;*.png;
        /// </summary>
        /// <returns></returns>
        public static string GetUploadFileExtensions(string extensions)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(extensions))
            {
                list = extensions.Split(new char[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            string extension = "";
            foreach (var item in list)
            {
                extension += "*." + item + ";";
            }
            return extension;
        }

        /// <summary>
        /// 获取结束日期。筛选时包含结束日期这一天。
        /// </summary>
        /// <param name="DateEnd"></param>
        /// <returns></returns>
        public static DateTime GetDateEnd(string DateEnd)
        {
            DateTime dt = Utils.StrToDateTime(DateEnd);
            dt = dt.AddDays(1);
            return dt;
        }

        /// <summary>
        /// 获取数据状态下拉框数据
        /// </summary>
        /// <returns></returns>
        public static SelectList GetSelectDataList(Dictionary<int, string> dictionary)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            list.Add(new SelectListItem() { Text = "", Value = "" });

            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public static SelectList GetSelectDataList(Dictionary<string, string> dictionary)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            foreach (var item in dictionary)
            {
                list.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");

            return generateList;
        }

        public static SelectList GetSelectDataList_Enum(Type type)
        {
            var enumValues = EnumHelper.EnumHelper.GetEnumKeyValuesWithDescription<short>(type);
            Dictionary<int, string> di = new Dictionary<int, string>();
            foreach (var item in enumValues)
            {
                di.Add(item.Key, item.Value.Value);
            }
            return GetSelectDataList(di);
        }

        /// <summary>
        /// 获取新建报检明细》销售合同里面的备注
        /// </summary>
        /// <returns></returns>
        public static string GetInspectionComment()
        {
            string str = "";// Environment.NewLine;
                            //sContent += "2. SHIPPING MARK:" + Environment.NewLine;
                            //sContent += "3. TERM OF THE PAYMENT:" + Environment.NewLine;
            str += "   SELLER'S BANK INFORMATION:" + Environment.NewLine;
            str += "   COMPANY NAME: SHANGHAI Jet CRAFTS, INC." + Environment.NewLine;
            str += "   ACCOUNT NO.: 31014007000220007211             ***SWIFT: PCBCCNBJSHX" + Environment.NewLine;
            str += "   BRANCH ADDRESS: CHINA CONSTRUCTION BANK CORPORATION SHANGHAI BRANCH" + Environment.NewLine;
            str += "   (ZHANGJIANG SUB-BRANCH)" + Environment.NewLine;
            str += "   NO. 220 KE YUAN ROAD, SHANGHAI, CHINA" + Environment.NewLine;
            str += "   COMPANY ADD: BUILDING 22, SHANGHAI HEADQUARTERS BAY, NO. 2500, XIUPU ROAD," + Environment.NewLine;
            str += "   PUDONG ,SHANGHAI , CHINA" + Environment.NewLine;
            return str;
        }

        /// <summary>
        /// 新建待印合同的默认条款
        /// </summary>
        /// <returns></returns>
        public static string GetOutsourcingDefaultContract()
        {
            string str = @"1）吊卡、价标和外箱标上的油墨均须使用环保材料，其重金属含量不得超过100PPM。
     若因质量问题造成客户索赔，一切费用和责任由贵方承担。
2)  颜色：需严格按照客人要求的颜色做准，需做到与客人样张完全一致。
3）贵方需免费提供余量备用，每个货号的数量不少于生产单数量的2 %。
4）本合同为送货至我司黄岩办事处的含税价格；具体如何送货，请同我司黄岩办事处生产部主管胡先军
     确认。
5）付款方式：2015年12月底与我司该年度其他生产单统一结算。
6）请于收到该合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。
      如有异议，请立即联系我司黄岩办事处于先生。";
            return str;
        }

        /// <summary>
        /// 获取状态枚举的名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetStatusEnumName(int id, Type t)
        {
            Dictionary<int, string> dictionary = EnumHelper.EnumHelper.GetCustomEnums<int>(t);
            string name = "";
            foreach (var item in dictionary)
            {
                if (item.Key == id)
                {
                    name = item.Value;
                }
            }
            return name;
        }

        /// <summary>
        /// List转换为有分隔符的字符串。默认为,隔开
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator">默认为,隔开</param>
        /// <returns></returns>
        public static string ListToString<T>(List<T> list, string separator = null)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            if (separator == null)
            {
                separator = ",";
            }
            return string.Join(separator, list);
        }

        private static readonly string[] enSmallNumber = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
        private static readonly string[] enLargeNumber = { "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };
        private static readonly string[] enUnit = { "", "THOUSAND", "MILLION", "BILLION", "TRILLION" };

        /// <summary>
        /// 以下是货币金额英文大写转换方法
        /// </summary>
        /// <param name="MoneyString"></param>
        /// <returns></returns>
        public static string GetEnMoneyString(string MoneyString)
        {
            string[] tmpString = MoneyString.Split('.');
            string intString = MoneyString;   // 默认为整数
            string decString = "";            // 保存小数部分字串
            string engCapital = "";            // 保存英文大写字串
            string strBuff1;
            string strBuff2;
            string strBuff3;
            int curPoint;
            int i1;
            int i2;
            int i3;
            int k;
            int n;

            if (tmpString.Length > 1)
            {
                intString = tmpString[0];             // 取整数部分
                decString = tmpString[1];             // 取小数部分
            }
            decString += "00";
            decString = decString.Substring(0, 2);   // 保留两位小数位

            try
            {
                // 以下处理整数部分
                curPoint = intString.Length - 1;
                if (curPoint >= 0 && curPoint < 15)
                {
                    k = 0;
                    while (curPoint >= 0)
                    {
                        strBuff1 = "";
                        strBuff2 = "";
                        strBuff3 = "";
                        if (curPoint >= 2)
                        {
                            n = int.Parse(intString.Substring(curPoint - 2, 3));
                            if (n != 0)
                            {
                                i1 = n / 100;            // 取佰位数值
                                i2 = (n - i1 * 100) / 10;    // 取拾位数值
                                i3 = n - i1 * 100 - i2 * 10;   // 取个位数值
                                if (i1 != 0)
                                {
                                    strBuff1 = enSmallNumber[i1] + " HUNDRED ";
                                }
                                if (i2 != 0)
                                {
                                    if (i2 == 1)
                                    {
                                        strBuff2 = enSmallNumber[i2 * 10 + i3] + " ";
                                    }
                                    else
                                    {
                                        strBuff2 = enLargeNumber[i2 - 2] + " ";
                                        if (i3 != 0)
                                        {
                                            strBuff3 = enSmallNumber[i3] + " ";
                                        }
                                    }
                                }
                                else
                                {
                                    if (i3 != 0)
                                    {
                                        strBuff3 = enSmallNumber[i3] + " ";
                                    }
                                }
                                engCapital = strBuff1 + strBuff2 + strBuff3 + enUnit[k] + " " + engCapital;
                            }
                        }
                        else
                        {
                            n = int.Parse(intString.Substring(0, curPoint + 1));
                            if (n != 0)
                            {
                                i2 = n / 10;      // 取拾位数值
                                i3 = n - i2 * 10;   // 取个位数值
                                if (i2 != 0)
                                {
                                    if (i2 == 1)
                                    {
                                        strBuff2 = enSmallNumber[i2 * 10 + i3] + " ";
                                    }
                                    else
                                    {
                                        strBuff2 = enLargeNumber[i2 - 2] + " ";
                                        if (i3 != 0)
                                        {
                                            strBuff3 = enSmallNumber[i3] + " ";
                                        }
                                    }
                                }
                                else
                                {
                                    if (i3 != 0)
                                    {
                                        strBuff3 = enSmallNumber[i3] + " ";
                                    }
                                }
                                engCapital = strBuff2 + strBuff3 + enUnit[k] + " " + engCapital;
                            }
                        }

                        ++k;
                        curPoint -= 3;
                    }
                    engCapital = engCapital.TrimEnd();
                }

                // 以下处理小数部分
                strBuff2 = "";
                strBuff3 = "";
                n = int.Parse(decString);
                if (n != 0)
                {
                    i2 = n / 10;      // 取拾位数值
                    i3 = n - i2 * 10;   // 取个位数值
                    if (i2 != 0)
                    {
                        if (i2 == 1)
                        {
                            strBuff2 = enSmallNumber[i2 * 10 + i3] + " ";
                        }
                        else
                        {
                            strBuff2 = enLargeNumber[i2 - 2] + " ";
                            if (i3 != 0)
                            {
                                strBuff3 = enSmallNumber[i3] + " ";
                            }
                        }
                    }
                    else
                    {
                        if (i3 != 0)
                        {
                            strBuff3 = enSmallNumber[i3] + " ";
                        }
                    }

                    // 将小数字串追加到整数字串后
                    if (engCapital.Length > 0)
                    {
                        engCapital = engCapital + " AND CENTS " + strBuff2 + strBuff3;   // 有整数部分时
                    }
                    else
                    {
                        engCapital = "CENTS " + strBuff2 + strBuff3;   // 只有小数部分时
                    }
                }

                engCapital = engCapital.TrimEnd();
                return engCapital;
            }
            catch
            {
                return "";   // 含非数字字符时，返回零长字串
            }
        }

        /// <summary>
        /// 合并成一个pdf
        /// </summary>
        /// <param name="filesAsolutePathList"></param>
        /// <param name="filesPhysicalPathList"></param>
        /// <returns></returns>
        public static string CreatePdfList(List<string> filesAsolutePathList, List<string> filesPhysicalPathList, List<string> list_ShippingMark = null)
        {
            return CreatePdfList(filesAsolutePathList, filesPhysicalPathList, list_ShippingMark, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// 合并成一个pdf
        /// </summary>
        /// <param name="filesAsolutePathList"></param>
        /// <param name="filesPhysicalPathList"></param>
        /// <returns></returns>
        public static string CreatePdfList(List<string> filesAsolutePathList, List<string> filesPhysicalPathList, List<string> list_ShippingMark = null, string fileName = null)
        {
            if (filesAsolutePathList.Count == 0)
            {
                return "";
            }
            string tempAsolutePath = "/data/Template/PacksDownload.docx";//写入数据所需要的模板文件
            string sFilesPath = "/data/Template/Out/Temp/";//生成新文件所在路径

            string outRealDir = HttpContext.Current.Server.MapPath("~" + sFilesPath);
            if (!Directory.Exists(outRealDir))
            {
                Directory.CreateDirectory(outRealDir);
            }

            string sNewFileName = fileName + ".pdf";
            string pdfFileName = sFilesPath + sNewFileName;
            AsposeX aspose = new AsposeX();
            aspose.MakeWordFile(filesAsolutePathList, filesPhysicalPathList, tempAsolutePath, sFilesPath, sNewFileName, list_ShippingMark);
            return pdfFileName;
        }

        /// <summary>
        /// 合并成一个pdf
        /// </summary>
        /// <returns></returns>
        public static string CreatePdfList(List<string> list_PdfFile, string MakerTypeEnum, string id)
        {
            string sFilesPath = "/data/Template/Out/Temp/";//生成新文件所在路径

            string outRealDir = HttpContext.Current.Server.MapPath("~" + sFilesPath);
            if (!Directory.Exists(outRealDir))
            {
                Directory.CreateDirectory(outRealDir);
            }

            string sNewFileName = Guid.NewGuid() + ".pdf";
            string pdfFileName = sFilesPath + sNewFileName;

            string sourceDir = Utils.GetMapPath("~/data/Template/Out/" + MakerTypeEnum.Split('_')[0] + "/" + id + "/PDFAndExcel");

            if (AsposeX.CombineMultiplePDFs(list_PdfFile, Utils.GetMapPath("~" + pdfFileName)))
            {
                return pdfFileName;
            }
            return "";
        }

        /// <summary>
        /// Y或者N。如果为空就是N
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string GetBoolString(bool? b)
        {
            if (b.HasValue)
            {
                if (b.Value)
                {
                    return "Y";
                }
            }
            return "N";
        }

        /// <summary>
        /// 截取字符串，不截断单词。
        /// </summary>
        /// <param name="words"></param>
        /// <param name="position">截取的位置</param>
        /// <returns></returns>
        public static string GetSumbstring_NotWord(string words, int position)
        {
            bool flag = false;
            while (!flag)
            {
                string str = words.Substring(position, 1);//要截取的位置的字符
                byte a = Encoding.ASCII.GetBytes(str)[0];
                if ((a >= 65 && a <= 90) || (a >= 97 && a <= 122))
                {
                    position -= 1;
                }
                else
                {
                    flag = true;
                }
            }
            return words.Substring(0, position + 1);
        }

        /// <summary>
        /// 检查中英文混合字符长度（英文字符算1，中文算2）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int GetStringLengthWithChinlish(string source)
        {
            Regex r = new Regex(@"[\u4E00-\u9fa5]");//中文
            int len = 0;
            char[] stringChar = source.ToCharArray();
            foreach (char chr in stringChar)
            {
                Console.Write(chr.ToString());
                if (r.IsMatch(chr.ToString()))
                {
                    len += 2;
                }
                else
                {
                    len += 1;
                }
            }

            return len;
        }

        public static string GetSpecificallyWidthStrFromStr(int width, string value, Font font)
        {
            string measuredString = string.Empty;
            try
            {
                char[] charArray = value.ToCharArray();
                StringBuilder sBuilder = new StringBuilder();
                foreach (char cha in charArray)
                {
                    sBuilder.Append(cha);
                    measuredString = sBuilder.ToString();
                    if (MeasureString(measuredString, font).Width > width)
                    {
                        measuredString.Remove(measuredString.Length - 1);
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                LogHelper.WriteError(exp);
                return null;
            }

            return measuredString;
        }

        public static Size MeasureString(string s, Font font)
        {
            Size proposedSize = new Size(int.MaxValue, int.MaxValue);
            Size size = TextRenderer.MeasureText(s, font, proposedSize);
            return size;
        }


        /// <summary>
        /// 根据长度拆分
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static List<string> SplitStringByWidth(string str, int width, Font font)
        {
            List<string> strList = new List<string>();

            if (string.IsNullOrEmpty(str))
            {
                return strList;
            }

            try
            {
                var size = MeasureString(str, font);
                if (size.Width < width)
                {
                    strList.Add(str);
                    return strList;
                }

                string leaveStr = str;
                while (!string.IsNullOrEmpty(leaveStr))
                {
                    string line = GetSpecificallyWidthStrFromStr(width, leaveStr, font);
                    strList.Add(line);
                    leaveStr = leaveStr.Remove(0, line.Length);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return strList;
        }

        /// <summary>
        /// 根据长度拆分
        /// </summary>
        /// <param name="str"></param>
        /// <param name="width"></param>
        /// <param name="font"></param>
        /// <param name="isLineFeed">是否包含换行</param>
        /// <returns></returns>
        public static List<string> SplitStringByWidth(string str, int width, Font font, bool isLineFeed)
        {
            List<string> strList = new List<string>();
            if (string.IsNullOrEmpty(str))
            {
                return strList;
            }
            if (!isLineFeed)
            {
                return SplitStringByWidth(str, width, font);
            }


            string[] arr = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item2 in arr)
            {
                var temp = CommonCode.SplitStringByWidth(item2, 1250, font);
                if (temp != null)
                {
                    strList.AddRange(temp);
                }
            }
            return strList;
        }

        /// <summary>
        /// 获取日期。格式dd-MMM-yy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime2(DateTime? dt)
        {
            if (dt.HasValue)
            {
                return GetDateTime2(dt.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取日期。格式dd-MMM-yy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime2(DateTime dt)
        {
            string dateString = dt.ToString(@"dd-MMM-yy", System.Globalization.CultureInfo.InvariantCulture);
            return dateString;
        }

        /// <summary>
        /// 获取日期。格式MMM. dd\t\h, yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime3(DateTime? dt)
        {
            if (dt.HasValue)
            {
                return GetDateTime3(dt.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取日期。格式MMM. dd\t\h, yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime3(DateTime dt)
        {
            string dateString = dt.ToString(@"MMM. dd\t\h, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return dateString;
        }

        /// <summary>
        /// 获取日期。格式dd/MMM/yy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime4(DateTime? dt)
        {
            if (dt.HasValue)
            {
                return GetDateTime4(dt.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取日期。格式dd/MMM/yy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime4(DateTime dt)
        {
            string dateString = dt.ToString(@"dd/MMM/yy", System.Globalization.CultureInfo.InvariantCulture);
            return dateString;
        }

        /// <summary>
        /// 获取日期。格式MMM. dd\t\h, yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime5(DateTime? dt)
        {
            if (dt.HasValue)
            {
                return GetDateTime5(dt.Value);
            }
            return null;
        }

        /// <summary>
        /// 获取日期。格式MMM.dd,yyyy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTime5(DateTime dt)
        {
            string dateString = dt.ToString(@"MMM.dd,yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return dateString;
        }

        /// <summary>
        /// 获取POID，如果POID有2个时用&隔开。如果超过3个时 前面的用,隔开 最后一个用&隔开。
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string GetPOIDList(List<string> list)
        {
            int count = list.Count;
            if (list != null && count > 0)
            {
                if (count == 1)
                {
                    return list[0];
                }
                else if (count == 2)
                {
                    return string.Join(" & ", list);
                }
                else
                {
                    return string.Join(" , ", list.Take(count - 1)) + " & " + list[count - 1];
                }
            }
            return "";
        }

        /// <summary>
        /// 获取条形码的图片
        /// </summary>
        /// <param name="StringToEncode"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image GetBarcodeImage(string StringToEncode, int width, int height, bool IncludeLabel = true)
        {
            if (string.IsNullOrEmpty(StringToEncode))
            {
                return null;
            }

            BarcodeLib.Barcode b = new BarcodeLib.Barcode();

            b.IncludeLabel = IncludeLabel;//在底部显示编码的文字
            b.Alignment = BarcodeLib.AlignmentPositions.CENTER;

            //b.RotateFlipType = RotateFlipType.Rotate180FlipX;

            var type = BarcodeLib.TYPE.CODE128;
            Image img = b.Encode(type, StringToEncode, width, height);
            return img;
        }


    }
}