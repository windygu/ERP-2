using ERP.BLL.Consts;
using ERP.Models.CustomEnums;
using ERP.Models.InspectionClearance;
using ERP.Models.InspectionCustoms;
using ERP.Models.OutSourcing;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.Logs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ERP.BLL.ERP.Consts
{
    public class MakerExcel
    {
        /// <summary>
        /// 根据excel模板生成excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">模型</param>
        /// <param name="MakerTypeEnum">类型</param>
        /// <param name="OutExcelName">导出的文件名</param>
        /// <returns></returns>
        public static string Maker<T>(T model, MakerTypeEnum MakerTypeEnum, string OutExcelName)
        {
            if (model == null)
            {
                return null;
            }
            List<string> outFileList = new List<string>();
            try
            {
                string templatePath = HttpContext.Current.Server.MapPath("~/data/Template/" + MakerTypeEnum.ToString() + ".xls");

                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                //生成的Excel文件的文件夹
                string outDir = "/data/Template/Out/" + MakerTypeEnum.ToString();
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outDir);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                //Excel文件
                OutExcelName = OutExcelName + "_" + CommonCode.GetRandomNumber() + ".xls";
                string excelPath = outRealDir + "/" + OutExcelName;

                IWorkbook workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));

                HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(0);

                switch (MakerTypeEnum)
                {
                    case MakerTypeEnum.PurchaseContract:

                        VMPurchase model2 = model as VMPurchase;
                        OutExcelName = model2.CustomerCode.Replace("/", "_") + " " + model2.PurchaseNumber + " " + model2.FactoryAbbreviation + ".xls";
                        excelPath = outRealDir + "/" + OutExcelName;

                        MakerPurchaseContract(model, workbook, sheet);
                        break;

                    case MakerTypeEnum.PurchaseContract_ProductFitting:

                        model2 = model as VMPurchase;
                        OutExcelName = model2.CustomerCode.Replace("/", "_") + " " + model2.PurchaseNumber + " " + model2.FactoryAbbreviation + ".xls";
                        excelPath = outRealDir + "/" + OutExcelName;

                        MakerPurchaseContract_ProductFitting(model, workbook, sheet);
                        break;

                    case MakerTypeEnum.OutSourcingContract:
                        DTOOutsourcing model3 = model as DTOOutsourcing;
                        OutExcelName = model3.OutContracNo + ".xls";
                        excelPath = outRealDir + "/" + OutExcelName;

                        MakerOutsourcingContract(model, workbook, sheet);
                        break;

                    case MakerTypeEnum.DocumentsIndexing_SaleOrder:
                        MakerDocumentsIndexing(model, workbook, sheet, MakerTypeEnum);
                        break;

                    default:
                        break;
                }

                //生成excel文件
                FileStream swQuoteSheet = File.OpenWrite(excelPath);
                workbook.Write(swQuoteSheet);
                swQuoteSheet.Close();
                swQuoteSheet.Dispose();

                return outDir + "/" + OutExcelName;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// 生成采购合同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        private static void MakerPurchaseContract<T>(T item, IWorkbook workbook, HSSFSheet sheet)
        {
            VMPurchase model = item as VMPurchase;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle.WrapText = false;

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Top, true);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);

            #region 填充数据

            if (model.CustomerCode == SelectCustomerEnum.S188.ToString())
            {
                ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), "WIC#", cellStyle2);
            }

            ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('A'), "厂名：" + model.FactoryName, cellStyle);
            ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('I'), "编号：" + model.PurchaseNumber, cellStyle);

            ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('A'), "联络人：" + model.CallPeople, cellStyle);
            ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('I'), "日期：" + Utils.DateTimeToStr3(Utils.StrToDateTime(model.PurchaseDate)), cellStyle);

            ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('A'), "电话：" + model.Telephone, cellStyle);
            ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('I'), "交货地：" + model.Port, cellStyle);

            ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('A'), "传真：" + model.Fax, cellStyle);
            ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('I'), "交货期：" + Utils.DateTimeToStr3(Utils.StrToDateTime(model.DateStartFormatter)), cellStyle);

            ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), "客人代号：" + model.CustomerCode, cellStyle);

            if (model.SelectCustomer == SelectCustomerEnum.S135.ToString())
            {
                ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "", cellStyle);
            }
            else
            {
                ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "PO#：" + model.POID, cellStyle);
                ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "DC：" + model.DestinationPortName, cellStyle);

            }

            Font font2 = new Font("Times New Roman", 11, FontStyle.Bold);
            List<string> list_temp = new List<string>();

            int thisRow = 10;
            int SumQty = 0;
            decimal SumAmount = 0;
            StringBuilder sb_OtherComment = new StringBuilder();
            foreach (var item_batch in model.list_batch)
            {
                foreach (var item_product in item_batch.listProduct)
                {
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item_product.No, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item_product.SkuNumber, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item_product.Name, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item_product.MixedMode, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item_product.PackageName, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item_product.PDQPackRate.ToString(), cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item_product.InnerBoxRate.ToString(), cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item_product.OuterBoxRate.ToString(), cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item_product.Qty, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item_product.UnitName, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), model.CurrentSign + item_product.PriceFactory, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), model.CurrentSign + (item_product.PriceFactory * item_product.Qty), cellStyle3);

                    int count = 0;

                    list_temp = CommonCode.SplitStringByWidth(item_product.No, 80, font2);
                    count = list_temp.Count;

                    list_temp = CommonCode.SplitStringByWidth(item_product.Name, 179, font2);
                    if (list_temp.Count > count)
                    {
                        count = list_temp.Count;
                    }

                    list_temp = CommonCode.SplitStringByWidth(item_product.PackageName, 149, font2);
                    if (list_temp.Count > count)
                    {
                        count = list_temp.Count;
                    }

                    sheet.GetRow(thisRow).Height = (short)(350 * count);//设置备注的高度

                    SumQty += item_product.Qty;
                    SumAmount += item_product.Qty * item_product.PriceFactory;
                    sb_OtherComment.Append(item_product.OtherComment + " ");
                    ++thisRow;
                }
            }

            if (model.OtherFee != null && model.OtherFee.Value > 0)//有其他费用时，显示出来。
            {
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "其他费用", cellStyle2);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle3);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), model.CurrentSign + model.OtherFee, cellStyle3);

                list_temp = CommonCode.SplitStringByWidth(sb_OtherComment.ToString(), 1250, font2);
                sheet.GetRow(thisRow).Height = (short)(350);//设置备注的高度

                ++thisRow;
            }

            ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), "", cellStyle4);

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(thisRow, thisRow, ExcelHelper.GetNumByChar('A'), ExcelHelper.GetNumByChar('L')));//合并单元格
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "备注：" + sb_OtherComment.ToString(), cellStyle4);

            list_temp = CommonCode.SplitStringByWidth(sb_OtherComment.ToString(), 1250, font2);
            sheet.GetRow(thisRow).Height = (short)(350 * list_temp.Count);//设置备注的高度

            ++thisRow;

            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "合 计", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumQty, cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), model.CurrentSign + (SumAmount + model.OtherFee), cellStyle2);

            list_temp = CommonCode.SplitStringByWidth(sb_OtherComment.ToString(), 1250, font2);
            sheet.GetRow(thisRow).Height = (short)(350);//设置备注的高度

            ++thisRow;
            ++thisRow;
            ExcelHelper.SetCellValue(sheet, thisRow, 0, "备 注：", cellStyle);

            ++thisRow;

            IRichTextString ContractTerms = ExcelHelper.SetHtmlToRichTextString(model.ContractTerms, boldFont);//处理备注的内容

            List<string> list = new List<string>();

            if (ContractTerms.String.Contains("\n"))
            {
                string[] arr = ContractTerms.ToString().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item2 in arr)
                {
                    Font font = new Font("Times New Roman", 11, FontStyle.Bold);
                    var temp = CommonCode.SplitStringByWidth(item2, 1250, font);
                    if (temp != null)
                    {
                        list.AddRange(temp);
                    }

                    //int tempIndex = 130;

                    //int length2 = item2.Length;
                    //int length = CommonCode.GetStringLengthWithChinlish(item2);

                    //string temp2 = item2;
                    //var rowCount = Math.Ceiling(length / 130m);
                    //for (int i = 0; i < rowCount; i++)
                    //{
                    //    int startIndex = i * tempIndex;
                    //    int count = tempIndex;
                    //    string temp3 = "";
                    //    if (i == rowCount - 1)
                    //    {
                    //        temp3 = temp2;
                    //    }
                    //    else
                    //    {
                    //        temp3 = Utils.SubString(temp2, count);

                    //    }

                    //    list.Add(temp3);
                    //    temp2 = temp2.Replace(temp3, "");
                    //}
                }
            }

            foreach (var item2 in list)
            {
                IRichTextString richTextString = new HSSFRichTextString(item2);
                if (item2.Contains("XXXX"))
                {
                    int startIndex = item2.IndexOf("XXXX");
                    int endIndex = startIndex + 4;

                    var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
                    font.Color = ExcelHelper.GetXLColour((HSSFWorkbook)workbook, Color.Red);

                    richTextString.ApplyFont(startIndex, endIndex, font);//设置红色字体
                }
                ExcelHelper.SetCellValue(sheet, thisRow, 0, richTextString, cellStyle);

                sheet.GetRow(thisRow).Height = 400;//设置备注的高度

                ++thisRow;
            }

            //ExcelHelper.SetCellValue(sheet, thisRow, 0, ContractTerms, cellStyle);

            //int ContractTerms_RowCount = ExcelHelper.GetExcelRowCount(ContractTerms.ToString(), 100);
            //sheet.GetRow(thisRow).Height = (short)(ContractTerms_RowCount * 300);//设置备注的高度

            //ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, 0, ExcelHelper.GetNumByChar('M'));

            //ICellStyle style3 = workbook.CreateCellStyle();
            //style3.WrapText = true;//自动换行
            //style3.VerticalAlignment = VerticalAlignment.Top;
            //ExcelHelper.GetCellStyle(sheet, thisRow, 0, style3);//设置备注的样式

            ++thisRow;
            ExcelHelper.SetCellValue(sheet, thisRow, 0, "    工厂 （签章）   ", cellStyle_boldFont);
            string factoryName = "上海哲联工艺品有限公司（签章）";
            if (model.CurrentSign == Keys.USD_Sign)
            {
                factoryName = "啟欣（香港）有限公司（签章）";
            }

            ExcelHelper.SetCellValue(sheet, thisRow, 7, factoryName, cellStyle_boldFont);

            ++thisRow;
            ++thisRow;
            //if (model.CurrentSign == Keys.USD_Sign)
            //{
            //    ExcelHelper.CreatePicture(workbook, sheet, "/images/啟欣（香港）有限公司章.PNG", 0, 0, 140, 100, ExcelHelper.GetNumByChar('I'), thisRow, ExcelHelper.GetNumByChar('J'), thisRow + 5);//2.57cm 2.81cm
            //}
            //else
            //{
            //    ExcelHelper.CreatePicture(workbook, sheet, "/images/哲联-合同专用章.PNG", 0, 0, 180, 100, ExcelHelper.GetNumByChar('I'), thisRow, ExcelHelper.GetNumByChar('K'), thisRow + 8);//4cm 4.29cm
            //}

            #endregion 填充数据
        }

        /// <summary>
        /// 生成采购合同——配件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        private static void MakerPurchaseContract_ProductFitting<T>(T item, IWorkbook workbook, HSSFSheet sheet)
        {
            VMPurchase model = item as VMPurchase;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle.WrapText = false;

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);

            #region 填充数据

            if (model.CustomerCode == SelectCustomerEnum.S188.ToString())
            {
                ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), "WIC#", cellStyle2);
            }

            ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('A'), "厂名：" + model.FactoryName, cellStyle);
            ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('F'), "编号：" + model.PurchaseNumber, cellStyle);

            ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('A'), "联络人：" + model.CallPeople, cellStyle);
            ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('F'), "日期：" + model.PurchaseDate, cellStyle);

            ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('A'), "电话：" + model.Telephone, cellStyle);
            ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('F'), "交货地：" + model.Port, cellStyle);

            ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('A'), "传真：" + model.Fax, cellStyle);
            ExcelHelper.SetCellValue(sheet, 4, ExcelHelper.GetNumByChar('F'), "交货期：" + model.DateStartFormatter, cellStyle);

            ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('F'), "客人代号：" + model.CustomerCode, cellStyle);

            //if (model.SelectCustomer == SelectCustomerEnum.S135.ToString())
            //{
            //    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "", cellStyle);
            //    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "", cellStyle);
            //}
            //else
            //{
            //    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "PO#：" + model.POID, cellStyle);
            //    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "DC：" + model.DestinationPortName, cellStyle);

            //}


            int thisRow = 8;
            int SumQty = 0;
            decimal? SumAmount = 0;
            foreach (var item_batch in model.list_batch)
            {
                foreach (var item_product in item_batch.listProductFitting)
                {
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item_product.No, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item_product.Name, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item_product.PackageName, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item_product.Comment, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item_product.Qty, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), model.CurrentSign + item_product.PriceFactory, cellStyle3);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), model.CurrentSign + (item_product.PriceFactory * item_product.Qty), cellStyle3);

                    SumQty += item_product.Qty ?? 0;
                    SumAmount += item_product.Qty * item_product.PriceFactory;
                    ++thisRow;
                }
            }

            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "合 计", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), SumQty, cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), model.CurrentSign + SumAmount, cellStyle2);

            ++thisRow;
            ++thisRow;
            ExcelHelper.SetCellValue(sheet, thisRow, 0, "备 注：", cellStyle);

            ++thisRow;

            IRichTextString ContractTerms = ExcelHelper.SetHtmlToRichTextString(model.ContractTerms, boldFont);//处理备注的内容

            List<string> list = new List<string>();

            if (ContractTerms.String.Contains("\n"))
            {
                string[] arr = ContractTerms.ToString().Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item2 in arr)
                {
                    Font font = new Font("Times New Roman", 11, FontStyle.Bold);
                    var temp = CommonCode.SplitStringByWidth(item2, 850, font);
                    if (temp != null)
                    {
                        list.AddRange(temp);
                    }
                }
            }

            foreach (var item2 in list)
            {
                IRichTextString richTextString = new HSSFRichTextString(item2);
                if (item2.Contains("XXXX"))
                {
                    int startIndex = item2.IndexOf("XXXX");
                    int endIndex = startIndex + 4;

                    var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
                    font.Color = ExcelHelper.GetXLColour((HSSFWorkbook)workbook, Color.Red);

                    richTextString.ApplyFont(startIndex, endIndex, font);//设置红色字体
                }
                ExcelHelper.SetCellValue(sheet, thisRow, 0, richTextString, cellStyle);

                sheet.GetRow(thisRow).Height = 400;//设置备注的高度

                ++thisRow;
            }

            ++thisRow;
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "    工厂 （签章）   ", cellStyle_boldFont);
            string factoryName = "上海哲联工艺品有限公司（签章）";

            if (model.CurrentSign == Keys.USD_Sign)
            {
                factoryName = "啟欣（香港）有限公司（签章）";
            }

            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), factoryName, cellStyle_boldFont);

            ++thisRow;
            ++thisRow;

            //if (model.CurrentSign == Keys.USD_Sign)
            //{
            //    ExcelHelper.CreatePicture(workbook, sheet, "/images/啟欣（香港）有限公司章.PNG", 0, 0, 290, 30, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 5);//2.57cm 2.81cm
            //}
            //else
            //{
            //    ExcelHelper.CreatePicture(workbook, sheet, "/images/哲联-合同专用章.PNG", 0, 0, 800, 0, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('G'), thisRow + 8);//4cm 4.29cm
            //}

            #endregion 填充数据
        }

        /// <summary>
        /// 生成代购合同
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        private static void MakerOutsourcingContract<T>(T item, IWorkbook workbook, HSSFSheet sheet)
        {
            DTOOutsourcing model = item as DTOOutsourcing;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11);
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 11, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);

            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Justify, VerticalAlignment.Center, false);

            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            cellStyle4.WrapText = false;

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Top, false);
            cellStyle5.WrapText = false;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Top, false);
            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            #region 填充数据

            ExcelHelper.SetCellValue(sheet, 2, 0, "厂  名：" + model.OutCompany, cellStyle5);
            ExcelHelper.SetCellValue(sheet, 2, 3, "编   号：" + model.OutContracNo, cellStyle5);
            ExcelHelper.SetCellValue(sheet, 3, 0, "联络人：" + model.CallPeoPle, cellStyle5);
            ExcelHelper.SetCellValue(sheet, 3, 3, "日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), cellStyle5);
            ExcelHelper.SetCellValue(sheet, 4, 0, "电 话：" + model.TelePhone, cellStyle5);
            ExcelHelper.SetCellValue(sheet, 4, 3, "交货地：" + model.DeliveryName, cellStyle5);
            ExcelHelper.SetCellValue(sheet, 5, 3, "交货期：" + Utils.StrToDateTime(model.DeliveryDateStart).ToString("yyyy年MM月dd日"), cellStyle5);
            ExcelHelper.SetCellValue(sheet, 6, 3, "客人代号：" + model.CustomerCode, cellStyle5);

            #endregion 填充数据

            decimal SumQty = 0;
            decimal SumAmt = 0;
            int thisRow = 8;
            Font font2 = new Font("Times New Roman", 11, FontStyle.Bold);
            List<string> list_temp = new List<string>();

            for (var i = 0; i < model.OCPacksData.Count; i++)
            {
                thisRow += 1;

                string PacksRemark = model.OCPacksData[i].PacksRemark;
                if (!string.IsNullOrEmpty(PacksRemark))
                {
                    PacksRemark = "(" + PacksRemark.StrTrim() + ")";
                }
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), model.OCPacksData[i].TagName + PacksRemark, cellStyle7);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);

                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(thisRow, thisRow, ExcelHelper.GetNumByChar('A'), ExcelHelper.GetNumByChar('F')));//合并单元格

                list_temp = CommonCode.SplitStringByWidth(PacksRemark, 645, font2);
                sheet.GetRow(thisRow).Height = (short)(350 * list_temp.Count);//设置备注的高度

                for (var j = 0; j < model.OCPacksData[i].OCPacksProducts.Count; j++)
                {
                    thisRow += 1;
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    var thisProduct = model.OCPacksData[i].OCPacksProducts[j];
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), thisProduct.OrderProductNO, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), thisProduct.TagDescribe, cellStyle);

                    list_temp = CommonCode.SplitStringByWidth(thisProduct.TagDescribe, 220, font2);
                    sheet.GetRow(thisRow).Height = (short)(350 * list_temp.Count);//设置备注的高度
                    if (list_temp.Count == 0)
                    {
                        sheet.GetRow(thisRow).Height = (short)(350);//设置备注的高度
                    }

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), thisProduct.ProductUPC, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), thisProduct.ProductTagsNumber, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.RMB_Sign + thisProduct.OrderProductFPrice, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.RMB_Sign + thisProduct.ProductTagsAmount, cellStyle);

                    SumQty += thisProduct.ProductTagsNumber;
                    SumAmt += thisProduct.ProductTagsAmount;
                }
            }
            thisRow += 1;


            if (model.OthersFee > 0)//有其他费用时，显示出来。
            {
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "其他费用", cellStyle2);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.RMB_Sign + model.OthersFee, cellStyle);
                ++thisRow;

                SumAmt += model.OthersFee;
            }
            
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "要求" + model.Remark, cellStyle);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle);
            ++thisRow;

            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "合 计", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SumQty.ToString(), cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.RMB_Sign + SumAmt.ToString(), cellStyle2);

            thisRow += 2;
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "备注：", cellStyle_boldFont);

            ++thisRow;
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), string.IsNullOrEmpty(model.Clasue) ? model.Clasue : model.Clasue.Replace("\t\t", ""), cellStyle6);

            list_temp = CommonCode.SplitStringByWidth(model.Clasue, 645, font2, true);
            sheet.GetRow(thisRow).Height = (short)(350 * list_temp.Count);//设置备注的高度

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(thisRow, thisRow, ExcelHelper.GetNumByChar('A'), ExcelHelper.GetNumByChar('F')));//合并单元格

            ++thisRow;
            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "    工厂 （签章）   ", cellStyle_boldFont);

            string factoryName = "上海哲联工艺品有限公司（签章）";
            //if (model.CurrentSign == Keys.USD_Sign)
            //{
            //    factoryName = "啟欣（香港）有限公司（签章）";
            //}

            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), factoryName, cellStyle_boldFont);

            ++thisRow;
            ++thisRow;
            //if (model.CurrentSign == Keys.USD_Sign)
            //{
            //    ExcelHelper.CreatePicture(workbook, sheet, "/images/啟欣（香港）有限公司章.PNG", 0, 0, 140, 100, ExcelHelper.GetNumByChar('I'), thisRow, ExcelHelper.GetNumByChar('J'), thisRow + 5);//2.57cm 2.81cm
            //}
            //else
            //{
            //ExcelHelper.CreatePicture(workbook, sheet, "/images/哲联-合同专用章.PNG", 0, 0, 960, 50, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('D'), thisRow + 7);//4cm 4.29cm
            //}

            thisRow += 2;
        }

        /// <summary>
        /// 生成单证索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="MakerTypeEnum"></param>
        private static void MakerDocumentsIndexing<T>(T item, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum MakerTypeEnum)
        {
            VMInspectionCustoms model = item as VMInspectionCustoms;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle_boldFont = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont);

            #region 填充数据

            switch (MakerTypeEnum)
            {
                case MakerTypeEnum.DocumentsIndexing_SaleOrder:
                    //销售合同上部
                    ExcelHelper.SetCellValue(sheet, 6, 1, model.CustomerName);
                    ExcelHelper.SetCellValue(sheet, 6, 5, model.SCNo);
                    ExcelHelper.SetCellValue(sheet, 7, 1, model.CustomerStreet);
                    ExcelHelper.SetCellValue(sheet, 7, 5, model.CustomerDateFormatter);
                    ExcelHelper.SetCellValue(sheet, 8, 1, model.CustomerReg);

                    int thisRow = 15;
                    decimal sumQty = 0;
                    decimal sumAmount = 0;

                    foreach (var item2 in model.list_OrdersProducts)
                    {
                        sumQty += item2.Qty;
                        sumAmount += item2.Amount;

                        //销售合同中部
                        ExcelHelper.SetCellValue(sheet, thisRow, 0, "", cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 1, item2.HsCode, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 2, item2.HsName, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 3, item2.OuterBoxRate, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 4, item2.ProductPrice, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 5, item2.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 6, item2.Amount, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 7, item2.Qty, cellStyle);
                        ExcelHelper.SetCellValue(sheet, thisRow, 8, item2.Amount, cellStyle);

                        ++thisRow;
                    }

                    //TOTAL
                    ExcelHelper.SetCellValue(sheet, thisRow, 0, "TOTAL", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, 1, "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, 2, "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, 3, "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, 4, "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, 5, "", cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, 6, sumQty, cellStyle);
                    ExcelHelper.SetCellValue(sheet, thisRow, 7, sumAmount, cellStyle);

                    thisRow += 2;

                    //销售合同下部
                    ExcelHelper.SetCellValue(sheet, thisRow, 0, "SHIPPING MARK:", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, thisRow, 1, model.ShippingMark);
                    ExcelHelper.SetCellValue(sheet, thisRow, 3, "TERM OF THE PAYMENT:", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, thisRow, 5, model.TeamOfThePayment);

                    ++thisRow;
                    ExcelHelper.SetCellValue(sheet, thisRow, 0, "SHIP DATE:", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, thisRow, 1, model.CustomerDateFormatter);

                    ICellStyle style3 = workbook.CreateCellStyle();
                    style3.WrapText = true;//自动换行
                    style3.VerticalAlignment = VerticalAlignment.Top;

                    ++thisRow;
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(thisRow, thisRow + 6, 1, 7));//合并单元格
                    ExcelHelper.SetCellValue(sheet, thisRow, 0, "REMARKS:", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, thisRow, 1, model.SaleContractContent, style3);

                    thisRow += 8;
                    ExcelHelper.SetCellValue(sheet, thisRow, 0, "THE  SELLER : ", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, thisRow, 1, "tom");
                    ExcelHelper.SetCellValue(sheet, thisRow, 4, "THE BUYER :", cellStyle_boldFont);
                    ExcelHelper.SetCellValue(sheet, thisRow, 5, model.CallPeople);

                    #region 插入图片

                    string imgPath = ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath("/images/SaleContract.png"));
                    if (File.Exists(imgPath))
                    {
                        using (Image img = System.Drawing.Image.FromFile(imgPath))
                        {
                            bool horizontal = img.Width > img.Height;

                            HSSFClientAnchor anchor = horizontal ?
                                new HSSFClientAnchor(75, 0, 800, 0, 6, thisRow, 7, thisRow + 6) :
                                    new HSSFClientAnchor(600, 0, 0, 0, 6, thisRow, 7, thisRow + 6);

                            //插入图片
                            IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                            int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                            HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                            picture.LineStyle = LineStyle.Solid;
                        }
                    }

                    #endregion 插入图片

                    break;

                default:
                    break;
            }

            #endregion 填充数据
        }

        /// <summary>
        /// 获取UPC
        /// </summary>
        /// <param name="UPC"></param>
        /// <returns></returns>
        public static string CalculateUPC(string UPC)//695265886001
        {
            //From:http://www.signalcn.com/News/News_279.Html
            //代码位置序号
            //代码位置序号是指包括校验码在内的，由右至左的顺序号（校验码的代码位置序号为1）。
            //计算步骤
            //校验码的计算步骤如下：
            //a.从代码位置序号2开始，所有偶数位的数字代码求和。
            //b.将步骤a的和乘以3。
            //c.从代码位置序号3开始，所有奇数位的数字代码求和。
            //d.将步骤b与步骤c的结果相加。
            //e.用大于或等于步骤d所得结果且为10最小整数倍的数减去步骤d所得结果，其差即为所求校验码的值。
            List<int> list = new List<int>();
            int sum1 = 0;
            int sum2 = 0;
            for (int i = 0; i < UPC.Length; i++)
            {
                int temp = Utils.StrToInt(UPC[i].ToString(), 0);
                if (i % 2 == 0)
                {
                    sum1 += temp;//26
                }
                else
                {
                    sum2 += temp;//34
                }
            }
            int lastNumber = 10 - (sum1 + sum2 * 3) % 10;
            return UPC + lastNumber;
        }
    }
}