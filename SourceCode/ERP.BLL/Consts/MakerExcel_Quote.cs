using ERP.Models.CustomEnums;
using ERP.Models.Quote;
using ERP.Tools;
using ERP.Tools.Logs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ERP.BLL.ERP.Consts
{
    /// <summary>
    /// 根据不同客户生成不同的报价单
    /// </summary>
    public class MakerExcel_Quote
    {
        private static Dictionary<QuotProductTypeEnum, string> directory = new Dictionary<QuotProductTypeEnum, string>();

        /// <summary>
        /// 获取报价单的后缀名。默认.xls
        /// </summary>
        /// <param name="QuotProductTypeEnum"></param>
        /// <returns></returns>
        public static string GetQuotTemplateExtension(QuotProductTypeEnum QuotProductTypeEnum)
        {
            string extension = ".xls";
            if (QuotProductTypeEnum.ToString() == QuotProductTypeEnum.S188.ToString() ||
                QuotProductTypeEnum.ToString() == QuotProductTypeEnum.S05.ToString() ||
                QuotProductTypeEnum.ToString() == QuotProductTypeEnum.F20.ToString())
            {
                extension = ".xlsx";
            }
            if (QuotProductTypeEnum.ToString() == QuotProductTypeEnum.DG.ToString())
            {
                extension = ".xlsm";
            }
            return extension;
        }

        /// <summary>
        /// 根据顾客及产品生成报价单
        /// </summary>
        /// <param name="QuotProductTypeEnum"></param>
        /// <param name="productList"></param>
        /// <returns>返回生成的报价单路径(相对路径, 取真实需要MapPath)</returns>
        public static List<string> BuildQuot(List<VMQuoteProduct> list_vm, QuotProductTypeEnum QuotProductTypeEnum)
        {
            if (list_vm.Count() <= 0)
            {
                return null;
            }
            List<string> outFileList = new List<string>();
            try
            {
                if (!directory.ContainsKey(QuotProductTypeEnum))
                {
                    directory.Add(QuotProductTypeEnum, "/data/quot/template/quotesheet" + QuotProductTypeEnum.ToString() + GetQuotTemplateExtension(QuotProductTypeEnum));
                }

                int QuotID = list_vm[0].QuotID;

                // 读取模板文件
                if (!directory.ContainsKey(QuotProductTypeEnum))
                {
                    throw new Exception("无该顾客的模板配置！");
                }
                string templatePath = HttpContext.Current.Server.MapPath("~" + directory[QuotProductTypeEnum]);
                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！");
                }

                // 遍历产品信息生成Excel文件
                string outRealDir = HttpContext.Current.Server.MapPath("~/data/quot/out/" + QuotID + "/" + CommonCode.GetTimeStamp());
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                IWorkbook workbook = null;

                if (QuotProductTypeEnum.ToString() == QuotProductTypeEnum.S188.ToString())
                {
                    workbook = new XSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                    ISheet newSheet = (XSSFSheet)workbook.GetSheetAt(0);

                    Bind_S188(list_vm, workbook, newSheet);
                    SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);

                    //生成压缩文件
                    ExcelHelper.Zip(outRealDir + "/PDFAndExcel");
                    return outFileList;
                }
                else if (QuotProductTypeEnum.ToString() == QuotProductTypeEnum.F20.ToString())
                {
                    workbook = new XSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                    ISheet newSheet = (XSSFSheet)workbook.GetSheetAt(0);

                    Bind_F20(list_vm, workbook, newSheet);

                    SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);

                    //生成压缩文件
                    ExcelHelper.Zip(outRealDir + "/PDFAndExcel");
                    return outFileList;
                }
                else if (QuotProductTypeEnum.ToString() == QuotProductTypeEnum.S05.ToString())
                {
                    foreach (var item in list_vm)
                    {
                        workbook = new XSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                        ISheet sheetQuoteSheet2 = (XSSFSheet)workbook.GetSheetAt(1);

                        Bind_S05(item, workbook, sheetQuoteSheet2);

                        SaveFile(outFileList, outRealDir, workbook, item.No, QuotProductTypeEnum, 2, 2);
                    }

                    //生成压缩文件
                    ExcelHelper.Zip(outRealDir + "/PDFAndExcel");
                    return outFileList;
                }
                else if (QuotProductTypeEnum.ToString() == QuotProductTypeEnum.DG.ToString())
                {
                    workbook = new XSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                    ISheet newSheet = (XSSFSheet)workbook.GetSheetAt(1);
                    foreach (var item in list_vm)
                    {
                        Bind_DG(item, workbook, newSheet);
                        SaveFile(outFileList, outRealDir, workbook, item.No, QuotProductTypeEnum);
                    }

                    //生成压缩文件
                    ExcelHelper.Zip(outRealDir + "/PDFAndExcel");
                    return outFileList;
                }

                workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                HSSFSheet sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);

                switch (QuotProductTypeEnum)
                {
                    //case QuotProductTypeEnum.F20:
                    //    Bind_F20(list_vm, workbook, sheetQuoteSheet);
                    //    SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                    //    break;

                    case QuotProductTypeEnum.F90:
                        Bind_F90(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    case QuotProductTypeEnum.Form1:
                        Bind_Form1(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    case QuotProductTypeEnum.S13:
                        Bind_S13(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    case QuotProductTypeEnum.S135:
                        Bind_S135(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    case QuotProductTypeEnum.S164:
                        Bind_S164(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    //case QuotProductTypeEnum.S188:
                    //    break;

                    case QuotProductTypeEnum.S56:
                        Bind_S56(list_vm, workbook);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    case QuotProductTypeEnum.S56_2:
                        Bind_S56_2(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    case QuotProductTypeEnum.S220:
                        Bind_S220(list_vm, workbook, sheetQuoteSheet);
                        SaveFile(outFileList, outRealDir, workbook, list_vm.FirstOrDefault().No, QuotProductTypeEnum);
                        break;

                    default:

                        foreach (var item in list_vm)
                        {
                            workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                            HSSFSheet sheetQuoteSheet2 = (HSSFSheet)workbook.GetSheetAt(0);

                            switch (QuotProductTypeEnum)
                            {
                                case QuotProductTypeEnum.S288:
                                    Bind_S288(item, workbook, sheetQuoteSheet2);
                                    break;

                                case QuotProductTypeEnum.S239:
                                    Bind_S239(item, workbook, sheetQuoteSheet2);
                                    break;

                                case QuotProductTypeEnum.S237:
                                    Bind_S237(item, workbook, sheetQuoteSheet2);
                                    break;

                                case QuotProductTypeEnum.S235:

                                    Bind_S235(item, workbook, sheetQuoteSheet2);
                                    break;

                                case QuotProductTypeEnum.S165:
                                    Bind_S165(item, workbook, sheetQuoteSheet2);
                                    break;

                                case QuotProductTypeEnum.Form2:
                                    Bind_Form2(item, workbook, sheetQuoteSheet2);
                                    break;

                                //case QuotProductTypeEnum.DG:
                                //    Bind_DG(item, workbook, sheetQuoteSheet);
                                //    break;

                                case QuotProductTypeEnum.S60:
                                    sheetQuoteSheet2 = (HSSFSheet)workbook.GetSheetAt(1);
                                    Bind_S60(item, workbook, sheetQuoteSheet2);
                                    break;

                                case QuotProductTypeEnum.DT://TODO 暂定

                                    Bind_S235(item, workbook, sheetQuoteSheet2);
                                    break;

                                default:
                                    break;
                            }
                            SaveFile(outFileList, outRealDir, workbook, item.No, QuotProductTypeEnum);
                        }
                        break;
                }

                //生成压缩文件
                ExcelHelper.Zip(outRealDir + "/PDFAndExcel");
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
            return outFileList;
        }

        #region 生成报价单模板

        private static void Bind_S288(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            DateTime now = DateTime.Now;
            string dateString = now.ToString(@"MMM. dd\t\h, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string after6MDateStr = now.AddMonths(6).ToString(@"MMM. dd\t\h, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            #region 填充数据

            // BUYER
            ICell cell = sheetQuoteSheet.GetRow(1).GetCell(1);
            cell.SetCellValue("BUYER: " + quote.Buyer);
            // DATE
            cell = sheetQuoteSheet.GetRow(1).GetCell(3);
            cell.SetCellValue("DATE: " + dateString);

            cell = sheetQuoteSheet.GetRow(3).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellValue(quote.Factory_EnglishName);

            cell = sheetQuoteSheet.GetRow(4).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellValue(quote.Factory_EnglishAddress);

            // after six month date
            cell = sheetQuoteSheet.GetRow(10).GetCell(7);
            cell.SetCellValue(after6MDateStr);
            // (CY shipments) FOB Port:
            cell = sheetQuoteSheet.GetRow(10).GetCell(2);
            cell.SetCellValue(quote.PortEnName.ToString());
            // Minimum quantity required
            cell = sheetQuoteSheet.GetRow(11).GetCell(7);
            cell.SetCellValue(quote.MOQEn.ToString());
            // Vendor Item #
            cell = sheetQuoteSheet.GetRow(25).GetCell(2);
            cell.SetCellValue("ITEM/ STYLE # " + quote.No);
            // Detailed Description:
            cell = sheetQuoteSheet.GetRow(16).GetCell(1);
            cell.SetCellValue(quote.Desc);
            // Material Composition (including % breakdown):
            cell = sheetQuoteSheet.GetRow(20).GetCell(1);
            cell.SetCellValue(quote.IngredientEn);
            // Dimensions/Size in inches  (include height, width, and length):
            cell = sheetQuoteSheet.GetRow(18).GetCell(1);
            cell.SetCellValue(string.Format("{0} \"H X {1}\"L X {2} \"W", quote.HeightIN.Round(1), quote.LengthIN.Round(1), quote.WidthIN.Round(1)));
            // Color/Colors:
            cell = sheetQuoteSheet.GetRow(22).GetCell(1);
            cell.SetCellValue("");
            // Number of styles included in the item quoted:
            cell = sheetQuoteSheet.GetRow(24).GetCell(1);
            cell.SetCellValue(quote.StyleName);
            // Master Carton/Case Pack:
            cell = sheetQuoteSheet.GetRow(31).GetCell(2);
            cell.SetCellValue(quote.OuterBoxRate.ToString());
            // Inner Carton/Case Pack:
            cell = sheetQuoteSheet.GetRow(31).GetCell(4);
            cell.SetCellValue(quote.InnerBoxRate.ToString());
            // Master Carton Cube: (In cbft)
            cell = sheetQuoteSheet.GetRow(32).GetCell(2);
            cell.SetCellValue(quote.OuterVolume.Round().ToString());
            // Master Carton Wt: ( In lbs)
            cell = sheetQuoteSheet.GetRow(32).GetCell(4);
            cell.SetCellValue(quote.OuterWeightGrossLBS.Round().ToString());
            // 31.1 "L X 9.06 "W X 12.6 "H
            cell = sheetQuoteSheet.GetRow(33).GetCell(2);
            cell.SetCellValue(string.Format("{0} \"L X {1} \"W X {2} \"H",
                quote.OuterLengthIN.Round(1),
                quote.OuterWidthIN.Round(1),
                quote.OuterHeightIN.Round(1)));
            // 5.91 "L X 5.91 "W X 30 "H
            cell = sheetQuoteSheet.GetRow(34).GetCell(2);
            cell.SetCellValue(string.Format("{0} \"L X {1} \"W X {2} \"H",
                 quote.LengthIN.Round(1),
                 quote.WidthIN.Round(1),
                 quote.HeightIN.Round(1)));
            // FOB ( USD) Cost:
            cell = sheetQuoteSheet.GetRow(41).GetCell(9);
            cell.SetCellValue("$" + quote.Cost.Round(2));
            // Duty: Rate %:
            cell = sheetQuoteSheet.GetRow(43).GetCell(7);
            cell.SetCellValue(quote.DutyPercent.ToString() + "%");

            cell = sheetQuoteSheet.GetRow(47).GetCell(7);
            cell.SetCellValue("$" + quote.FreightRate.ToString());

            // Product Partner suggested 10 digit HTS # (required)
            cell = sheetQuoteSheet.GetRow(54).GetCell(9);
            cell.SetCellValue(quote.HTS_Name);
            // 40GP
            cell = sheetQuoteSheet.GetRow(37).GetCell(2);
            cell.SetCellFormula(cell.CellFormula);
            // 40HC
            cell = sheetQuoteSheet.GetRow(37).GetCell(3);
            cell.SetCellFormula(cell.CellFormula);
            // 45'
            cell = sheetQuoteSheet.GetRow(37).GetCell(4);
            cell.SetCellFormula(cell.CellFormula);
            // Amount
            cell = sheetQuoteSheet.GetRow(43).GetCell(9);
            cell.SetCellFormula(cell.CellFormula);
            // Ocean Freight
            cell = sheetQuoteSheet.GetRow(46).GetCell(9);
            cell.SetCellFormula(cell.CellFormula);
            // Estimated Landed Cost: (USD)
            cell = sheetQuoteSheet.GetRow(50).GetCell(9);
            cell.SetCellFormula(cell.CellFormula);

            #endregion 填充数据

            #region 插入图片

            string imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                using (Image img = System.Drawing.Image.FromFile(imgPath))
                {
                    bool horizontal = img.Width > img.Height;

                    HSSFClientAnchor anchor = horizontal ?
                        new HSSFClientAnchor(75, 0, 800, 0, 6, 15, 10, 33) :
                            new HSSFClientAnchor(600, 0, 0, 0, 6, 15, 10, 33);

                    //插入图片
                    IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                    int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                    HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }
            }

            #endregion 插入图片
        }

        private static void Bind_S239(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            #region QuoteSheet

            sheetQuoteSheet.GetRow(7).GetCell(7).SetCellValue(DateTime.Now.Date);

            sheetQuoteSheet.GetRow(9).GetCell(6).SetCellValue(quote.LengthIN.ToString());
            sheetQuoteSheet.GetRow(10).GetCell(6).SetCellValue(quote.WidthIN.ToString());
            sheetQuoteSheet.GetRow(11).GetCell(6).SetCellValue(quote.HeightIN.ToString());
            sheetQuoteSheet.GetRow(17).GetCell(1).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(18).GetCell(1).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(20).GetCell(1).SetCellValue(quote.PackingMannerEnName);
            sheetQuoteSheet.GetRow(23).GetCell(0).SetCellValue(quote.OuterBoxRate.ToString());
            sheetQuoteSheet.GetRow(23).GetCell(1).SetCellValue(quote.InnerBoxRate.ToString());
            sheetQuoteSheet.GetRow(23).GetCell(2).SetCellValue(quote.OuterVolume.ToString());
            sheetQuoteSheet.GetRow(28).GetCell(3).SetCellValue(quote.Cost.ToString());
            sheetQuoteSheet.GetRow(34).GetCell(2).SetCellValue(quote.PortEnName.ToString());
            sheetQuoteSheet.GetRow(40).GetCell(1).SetCellValue(quote.UPC);
            sheetQuoteSheet.GetRow(44).GetCell(5).SetCellValue(quote.Remarks);
            sheetQuoteSheet.GetRow(51).GetCell(10).SetCellValue(quote.HTS_Name);
            sheetQuoteSheet.GetRow(52).GetCell(10).SetCellValue(quote.IngredientEn);
            if (quote.IngredientEn != null)
            {
                if (quote.IngredientEn.ToLower().Contains("metal") && quote.IngredientEn.Contains("100"))
                {
                    sheetQuoteSheet.GetRow(54).GetCell(5).SetCellValue("Iron");
                }
            }

            sheetQuoteSheet.GetRow(29).GetCell(3).SetCellFormula("B30/F45");
            sheetQuoteSheet.GetRow(30).GetCell(3).SetCellFormula("D29*B31");
            sheetQuoteSheet.GetRow(31).GetCell(3).SetCellFormula("D29*B32");
            sheetQuoteSheet.GetRow(32).GetCell(3).SetCellFormula("SUM(D29:D32)");
            sheetQuoteSheet.GetRow(25).GetCell(3).SetCellFormula("D29*A24");
            sheetQuoteSheet.GetRow(26).GetCell(3).SetCellFormula("C24/A24");

            var imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                //插入图片
                IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                int imageId = ExcelHelper.LoadImage(imgPath, workbook);
                HSSFClientAnchor anchor = new HSSFClientAnchor(10, 10, 317, 0, 5, 20, 10, 42);
                //判断图片的大小
                var img = System.Drawing.Image.FromFile(imgPath);
                if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)//横形
                {
                    anchor = new HSSFClientAnchor(0, 10, 1000, 220, 4, 20, 10, 41);
                }
                HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                picture.LineStyle = LineStyle.Solid;
            }

            #endregion QuoteSheet
        }

        private static void Bind_S237(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            #region QuoteSheet

            sheetQuoteSheet.GetRow(14).GetCell(ExcelHelper.GetNumByChar('M')).SetCellValue(quote.OuterBoxRate.ToString());
            sheetQuoteSheet.GetRow(15).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(15).GetCell(ExcelHelper.GetNumByChar('M')).SetCellValue(quote.InnerBoxRate.ToString());
            sheetQuoteSheet.GetRow(16).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(17).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue(quote.HeightIN.ToString());
            sheetQuoteSheet.GetRow(17).GetCell(ExcelHelper.GetNumByChar('G')).SetCellValue(quote.LengthIN.ToString());
            sheetQuoteSheet.GetRow(17).GetCell(ExcelHelper.GetNumByChar('I')).SetCellValue(quote.WidthIN.ToString());

            sheetQuoteSheet.GetRow(18).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.PortEnName.ToString());
            sheetQuoteSheet.GetRow(18).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue(quote.MOQEn.ToString());
            //sheetQuoteSheet.GetRow(19).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.UPC);
            sheetQuoteSheet.GetRow(19).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue(quote.OuterVolume.ToString());

            sheetQuoteSheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.HTS_Name);
            sheetQuoteSheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('G')).SetCellValue(quote.OuterLengthIN.ToString());
            sheetQuoteSheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('J')).SetCellValue(quote.OuterWidthIN.ToString());
            sheetQuoteSheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('L')).SetCellValue(quote.OuterHeightIN.ToString());

            sheetQuoteSheet.GetRow(23).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue(quote.Cost.ToString());
            sheetQuoteSheet.GetRow(24).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.DutyPercent.ToString());
            sheetQuoteSheet.GetRow(37).GetCell(ExcelHelper.GetNumByChar('A')).SetCellValue(quote.IngredientEn);

            sheetQuoteSheet.GetRow(24).GetCell(ExcelHelper.GetNumByChar('E')).SetCellFormula("SUM(E24*D25)");
            sheetQuoteSheet.GetRow(25).GetCell(ExcelHelper.GetNumByChar('E')).SetCellFormula("SUM((K20*D26)/M16)");
            sheetQuoteSheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('E')).SetCellFormula("ROUND((C27*E24),2)");
            sheetQuoteSheet.GetRow(27).GetCell(ExcelHelper.GetNumByChar('E')).SetCellFormula("SUM(E24:E27)");
            sheetQuoteSheet.GetRow(29).GetCell(ExcelHelper.GetNumByChar('E')).SetCellFormula("SUM((E28/0.4))");
            sheetQuoteSheet.GetRow(30).GetCell(ExcelHelper.GetNumByChar('D')).SetCellFormula("SUM((E31-E28)/E31)");

            var imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                //插入图片
                IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                int imageId = ExcelHelper.LoadImage(imgPath, workbook);
                HSSFClientAnchor anchor = new HSSFClientAnchor(200, 5, 1000, 240, 6, 19, 10, 33);
                //判断图片的大小
                var img = System.Drawing.Image.FromFile(imgPath);
                if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)//横形
                {
                    anchor = new HSSFClientAnchor(0, 80, 1000, 151, 5, 20, 12, 32);
                }
                HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                picture.LineStyle = LineStyle.Solid;
            }

            #endregion QuoteSheet
        }

        private static void Bind_S235(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Book Antiqua", 10, false);
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle.WrapText = false;

            #region QuoteSheet

            DateTime now = DateTime.Now;
            string dateString = now.ToString(@"MMM-dd-yy", System.Globalization.CultureInfo.InvariantCulture);

            sheetQuoteSheet.GetRow(25).GetCell(8).SetCellValue("");
            sheetQuoteSheet.GetRow(15).GetCell(3).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(15).GetCell(14).SetCellValue(dateString);
            sheetQuoteSheet.GetRow(19).GetCell(3).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(21).GetCell(3).SetCellValue(quote.PackingMannerEnName);
            sheetQuoteSheet.GetRow(22).GetCell(7).SetCellValue(quote.HeightIN.ToString());
            sheetQuoteSheet.GetRow(22).GetCell(10).SetCellValue(quote.WidthIN.ToString());
            sheetQuoteSheet.GetRow(22).GetCell(14).SetCellValue(quote.LengthIN.ToString());

            StringBuilder sb_Ingredient1 = new StringBuilder();
            StringBuilder sb_Ingredient2 = new StringBuilder();

            int index = 1;
            foreach (var item in quote.list_ProductIngredient)
            {
                string temp = "Component #" + index + " - " + item.IngredientPercent + "%  " + item.IngredientName.Trim() + "  ";
                if (index <= 3)
                {
                    sb_Ingredient1.Append(temp);
                }
                else
                {
                    sb_Ingredient2.Append(temp);
                }

                index++;
            }

            ExcelHelper.SetCellValue(sheetQuoteSheet, 24, ExcelHelper.GetNumByChar('F'), sb_Ingredient1.ToString(), cellStyle);
            ExcelHelper.SetCellValue(sheetQuoteSheet, 25, ExcelHelper.GetNumByChar('A'), sb_Ingredient2.ToString(), cellStyle);

            sheetQuoteSheet.GetRow(28).GetCell(5).SetCellValue("$" + quote.Cost.ToString());
            sheetQuoteSheet.GetRow(39).GetCell(5).SetCellValue(quote.DutyPercent.ToString() + "%");
            sheetQuoteSheet.GetRow(40).GetCell(5).SetCellValue(quote.HTS_Name);
            if (quote.InnerBoxRate.HasValue && quote.InnerBoxRate.Value > 0)
            {
                sheetQuoteSheet.GetRow(30).GetCell(5).SetCellValue(quote.InnerBoxRate.ToString());
                sheetQuoteSheet.GetRow(30).GetCell(7).SetCellValue(quote.InnerBoxRate.ToString());
            }
            else
            {
                sheetQuoteSheet.GetRow(30).GetCell(5).SetCellValue(quote.OuterBoxRate.ToString());
                sheetQuoteSheet.GetRow(30).GetCell(7).SetCellValue(quote.OuterBoxRate.ToString());
            }
            sheetQuoteSheet.GetRow(31).GetCell(5).SetCellValue(quote.OuterWeightGrossLBS.ToString());

            sheetQuoteSheet.GetRow(32).GetCell(5).SetCellValue(quote.OuterHeightIN.ToString());
            sheetQuoteSheet.GetRow(32).GetCell(6).SetCellValue(quote.OuterWidthIN.ToString());
            sheetQuoteSheet.GetRow(32).GetCell(7).SetCellValue(quote.OuterLengthIN.ToString());

            sheetQuoteSheet.GetRow(33).GetCell(4).SetCellValue(quote.OuterVolume.Round().ToString());
            sheetQuoteSheet.GetRow(34).GetCell(5).SetCellValue(quote.InnerWeightLBS.Round().ToString());

            ICell cell = sheetQuoteSheet.GetRow(41).GetCell(5);
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(42).GetCell(5);
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(43).GetCell(5);
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(44).GetCell(5);
            cell.SetCellFormula(cell.CellFormula);

            sheetQuoteSheet.GetRow(45).GetCell(4).SetCellValue(quote.PortEnName.ToString());

            var imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                //插入图片
                IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                int imageId = ExcelHelper.LoadImage(imgPath, workbook);
                //判断图片的大小
                var img = System.Drawing.Image.FromFile(imgPath);

                bool horizontal = img.Width > img.Height;

                HSSFClientAnchor anchor = horizontal ?
                    new HSSFClientAnchor(20, 10, 1000, 70, 8, 27, 16, 46) :
                        new HSSFClientAnchor(1000, 0, 0, 0, 8, 27, 16, 46);

                HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                picture.LineStyle = LineStyle.Solid;
            }

            #endregion QuoteSheet
        }

        private static void Bind_S165(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            #region QuoteSheet

            sheetQuoteSheet.GetRow(2).GetCell(9).SetCellValue(DateTime.Now.ToShortDateString());
            sheetQuoteSheet.GetRow(3).GetCell(9).SetCellValue(DateTime.Now.ToShortDateString());
            sheetQuoteSheet.GetRow(6).GetCell(1).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(7).GetCell(1).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(8).GetCell(1).SetCellValue((quote.LengthIN == 0 ? "  ??  " : quote.LengthIN.Round() + "\"") + " x " + (quote.WidthIN == 0 ? "  ??  " : quote.WidthIN.Round() + "\"") + " x " + (quote.HeightIN == 0 ? "  ??  " : quote.HeightIN.Round() + "\""));
            sheetQuoteSheet.GetRow(9).GetCell(1).SetCellValue(quote.IngredientEn);
            sheetQuoteSheet.GetRow(11).GetCell(2).SetCellValue(quote.Cost.ToString());
            sheetQuoteSheet.GetRow(13).GetCell(1).SetCellValue(quote.DutyPercent.ToString());
            sheetQuoteSheet.GetRow(18).GetCell(3).SetCellValue(quote.OuterVolume.Round().ToString());
            sheetQuoteSheet.GetRow(19).GetCell(2).SetCellValue(quote.OuterBoxRate.ToString());
            sheetQuoteSheet.GetRow(21).GetCell(2).SetCellValue(quote.InnerBoxRate.ToString());
            sheetQuoteSheet.GetRow(23).GetCell(2).SetCellValue(quote.HTS_Name);
            sheetQuoteSheet.GetRow(26).GetCell(2).SetCellValue(quote.PortEnName.ToString());
            sheetQuoteSheet.GetRow(28).GetCell(3).SetCellValue(quote.MOQEn.ToString());
            sheetQuoteSheet.GetRow(43).GetCell(1).SetCellValue(quote.FOBUS.ToString());
            sheetQuoteSheet.GetRow(12).GetCell(2).SetCellFormula("B13/D28");
            sheetQuoteSheet.GetRow(13).GetCell(2).SetCellFormula("B14*C12");
            sheetQuoteSheet.GetRow(14).GetCell(2).SetCellFormula("B15*C12");
            sheetQuoteSheet.GetRow(15).GetCell(2).SetCellFormula("B16*C12");
            sheetQuoteSheet.GetRow(16).GetCell(2).SetCellFormula("SUM(C12:C16)");
            sheetQuoteSheet.GetRow(18).GetCell(2).SetCellFormula("D19/35.315");
            sheetQuoteSheet.GetRow(27).GetCell(3).SetCellFormula("2400/D19*C20");

            var imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                //插入图片
                IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                int imageId = ExcelHelper.LoadImage(imgPath, workbook);
                HSSFClientAnchor anchor = new HSSFClientAnchor(640, 30, 1000, 230, 5, 10, 8, 28);
                //判断图片的大小
                var img = System.Drawing.Image.FromFile(imgPath);
                if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)//横形
                {
                    anchor = new HSSFClientAnchor(0, 97, 1000, 152, 5, 11, 9, 27);
                }
                HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                picture.LineStyle = LineStyle.Solid;
            }

            #endregion QuoteSheet
        }

        private static void Bind_Form2(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            #region QuoteSheet

            sheetQuoteSheet.GetRow(11).GetCell(2).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(11).GetCell(10).SetCellValue(quote.PortEnName.ToString());
            sheetQuoteSheet.GetRow(12).GetCell(2).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(12).GetCell(10).SetCellValue(quote.PackingMannerEnName);
            sheetQuoteSheet.GetRow(13).GetCell(2).SetCellValue(quote.IngredientEn);
            sheetQuoteSheet.GetRow(13).GetCell(10).SetCellValue(quote.MOQEn.ToString());
            sheetQuoteSheet.GetRow(14).GetCell(2).SetCellValue(quote.LengthIN.ToString());
            sheetQuoteSheet.GetRow(14).GetCell(3).SetCellValue(quote.WidthIN.ToString());
            sheetQuoteSheet.GetRow(14).GetCell(4).SetCellValue(quote.HeightIN.ToString());
            sheetQuoteSheet.GetRow(16).GetCell(2).SetCellValue(quote.OuterLengthIN.ToString());
            sheetQuoteSheet.GetRow(16).GetCell(3).SetCellValue(quote.OuterWidthIN.ToString());
            sheetQuoteSheet.GetRow(16).GetCell(4).SetCellValue(quote.OuterHeightIN.ToString());
            sheetQuoteSheet.GetRow(16).GetCell(10).SetCellValue(quote.UPC);
            sheetQuoteSheet.GetRow(19).GetCell(2).SetCellValue(quote.InnerBoxRate.ToString());
            sheetQuoteSheet.GetRow(20).GetCell(2).SetCellValue(quote.OuterBoxRate.ToString());
            sheetQuoteSheet.GetRow(23).GetCell(2).SetCellValue(quote.Cost.ToString());
            sheetQuoteSheet.GetRow(24).GetCell(2).SetCellValue(quote.Freight.ToString());
            sheetQuoteSheet.GetRow(28).GetCell(2).SetCellValue(quote.Retail.ToString());
            sheetQuoteSheet.GetRow(29).GetCell(2).SetCellValue(quote.HTS_Name);
            sheetQuoteSheet.GetRow(30).GetCell(2).SetCellValue(quote.DutyPercent.ToString());

            sheetQuoteSheet.GetRow(21).GetCell(2).SetCellFormula("C17*D17*E17/1728");
            sheetQuoteSheet.GetRow(25).GetCell(2).SetCellFormula("C25*C22/C21");
            sheetQuoteSheet.GetRow(26).GetCell(2).SetCellFormula("C24*0.06");
            sheetQuoteSheet.GetRow(27).GetCell(2).SetCellFormula("C24+C26+C27");
            sheetQuoteSheet.GetRow(31).GetCell(2).SetCellFormula("1000/C22*C21");
            sheetQuoteSheet.GetRow(31).GetCell(2).SetCellFormula("1000/C22*C21");
            sheetQuoteSheet.GetRow(32).GetCell(2).SetCellFormula("2050/C22*C21");
            sheetQuoteSheet.GetRow(33).GetCell(2).SetCellFormula("2400/C22*C21");

            var imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                //插入图片
                IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                int imageId = ExcelHelper.LoadImage(imgPath, workbook);
                HSSFClientAnchor anchor = new HSSFClientAnchor(582, 5, 666, 238, 8, 19, 12, 35);
                //判断图片的大小
                var img = System.Drawing.Image.FromFile(imgPath);
                if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)
                {
                    anchor = new HSSFClientAnchor(90, 5, 360, 238, 7, 19, 14, 35);
                }
                HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                picture.LineStyle = LineStyle.Solid;
            }

            #endregion QuoteSheet
        }

        private static void Bind_DG(VMQuoteProduct quote, IWorkbook workbook, ISheet sheetQuoteSheet)
        {
            #region QuoteSheet

            sheetQuoteSheet.GetRow(5).GetCell(26).SetCellValue(quote.PortEnName.ToString());
            sheetQuoteSheet.GetRow(27).GetCell(7).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(31).GetCell(9).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(32).GetCell(31).SetCellValue(quote.Cost.ToString());
            sheetQuoteSheet.GetRow(33).GetCell(9).SetCellValue(quote.DutyPercent.ToString());
            sheetQuoteSheet.GetRow(34).GetCell(9).SetCellValue(quote.HTS_Name);
            sheetQuoteSheet.GetRow(35).GetCell(9).SetCellValue(quote.Freight.ToString());
            sheetQuoteSheet.GetRow(56).GetCell(34).SetCellValue(quote.PDQLengthIN + " X " + quote.PDQWidthIN + " X " + quote.PDQHeightIN);

            sheetQuoteSheet.GetRow(24).GetCell(48).SetCellValue(quote.LengthIN.ToString() + " X " + quote.WidthIN.ToString() + " X " + quote.HeightIN.ToString());
            sheetQuoteSheet.GetRow(25).GetCell(48).SetCellValue(quote.WeightLBS.ToString());
            sheetQuoteSheet.GetRow(27).GetCell(48).SetCellValue(quote.PackingMannerEnName);

            sheetQuoteSheet.GetRow(13).GetCell(46).SetCellValue(quote.OuterLengthIN.ToString());
            sheetQuoteSheet.GetRow(15).GetCell(46).SetCellValue(quote.OuterWidthIN.ToString());
            sheetQuoteSheet.GetRow(17).GetCell(46).SetCellValue(quote.OuterHeightIN.ToString());
            sheetQuoteSheet.GetRow(19).GetCell(46).SetCellValue(quote.OuterWeightGrossLBS.Round().ToString());

            sheetQuoteSheet.GetRow(13).GetCell(51).SetCellValue(quote.InnerLengthIN.ToString());
            sheetQuoteSheet.GetRow(15).GetCell(51).SetCellValue(quote.InnerWidthIN.ToString());
            sheetQuoteSheet.GetRow(17).GetCell(51).SetCellValue(quote.InnerHeightIN.ToString());

            sheetQuoteSheet.GetRow(13).GetCell(56).SetCellValue(quote.LengthIN.ToString());
            sheetQuoteSheet.GetRow(15).GetCell(56).SetCellValue(quote.WidthIN.ToString());
            sheetQuoteSheet.GetRow(17).GetCell(56).SetCellValue(quote.HeightIN.ToString());

            var imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                //插入图片
                IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                try
                {
                    HSSFClientAnchor anchor = new HSSFClientAnchor(0, 20, 0, 230, 39, 32, 64, 48);
                    HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);

                    picture.LineStyle = LineStyle.Solid;
                }
                catch
                {
                    XSSFClientAnchor anchor = new XSSFClientAnchor(0, 20, 0, 230, 39, 32, 64, 48);
                    XSSFPicture picture = (XSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }
            }

            #endregion QuoteSheet
        }

        private static void Bind_S60(VMQuoteProduct quote, IWorkbook workbook, HSSFSheet sheetQuoteSheet)
        {
            #region QuoteSheet

            if (!string.IsNullOrEmpty(quote.SeasonDepartmentNumber) && quote.SeasonDepartmentNumber.Length > 2)
            {
                sheetQuoteSheet.GetRow(2).GetCell(ExcelHelper.GetNumByChar('A')).SetCellValue(quote.SeasonDepartmentNumber.Substring(0, 3));//截取前3位
            }
            sheetQuoteSheet.GetRow(2).GetCell(ExcelHelper.GetNumByChar('B')).SetCellValue(quote.Buyer);

            // 插入数据
            sheetQuoteSheet.GetRow(6).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue((quote.OuterLengthIN ?? 0).Round().ToString());
            sheetQuoteSheet.GetRow(6).GetCell(ExcelHelper.GetNumByChar('M')).SetCellValue((quote.LengthIN ?? 0).Round().ToString());

            sheetQuoteSheet.GetRow(7).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue((quote.OuterWidthIN ?? 0).Round().ToString());
            sheetQuoteSheet.GetRow(7).GetCell(ExcelHelper.GetNumByChar('M')).SetCellValue((quote.WidthIN ?? 0).Round().ToString());

            sheetQuoteSheet.GetRow(8).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue((quote.OuterHeightIN ?? 0).Round().ToString());
            sheetQuoteSheet.GetRow(8).GetCell(ExcelHelper.GetNumByChar('M')).SetCellValue((quote.HeightIN ?? 0).Round().ToString());

            sheetQuoteSheet.GetRow(15).GetCell(ExcelHelper.GetNumByChar('B')).SetCellValue(quote.Desc);
            sheetQuoteSheet.GetRow(16).GetCell(ExcelHelper.GetNumByChar('M')).SetCellValue(quote.PackingMannerEnName);
            sheetQuoteSheet.GetRow(18).GetCell(ExcelHelper.GetNumByChar('B')).SetCellValue(quote.IngredientEn);
            sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('A')).SetCellValue(quote.No);
            sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.OuterBoxRate.ToString());
            sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue(quote.InnerBoxRate.ToString());
            sheetQuoteSheet.GetRow(24).GetCell(ExcelHelper.GetNumByChar('F')).SetCellValue((quote.Cost).Round().ToString());
            sheetQuoteSheet.GetRow(25).GetCell(ExcelHelper.GetNumByChar('B')).SetCellValue(quote.HTS_Name);
            sheetQuoteSheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('B')).SetCellValue((quote.DutyPercent ?? 0).Round().ToString());

            ICell cell = sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(45).GetCell(ExcelHelper.GetNumByChar('I', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(45).GetCell(ExcelHelper.GetNumByChar('J', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(45).GetCell(ExcelHelper.GetNumByChar('K', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(46).GetCell(ExcelHelper.GetNumByChar('I', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(46).GetCell(ExcelHelper.GetNumByChar('J', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(46).GetCell(ExcelHelper.GetNumByChar('K', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(47).GetCell(ExcelHelper.GetNumByChar('I', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(47).GetCell(ExcelHelper.GetNumByChar('J', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(47).GetCell(ExcelHelper.GetNumByChar('K', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(48).GetCell(ExcelHelper.GetNumByChar('I', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(48).GetCell(ExcelHelper.GetNumByChar('J', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(48).GetCell(ExcelHelper.GetNumByChar('K', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(49).GetCell(ExcelHelper.GetNumByChar('I', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(49).GetCell(ExcelHelper.GetNumByChar('J', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(49).GetCell(ExcelHelper.GetNumByChar('K', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(50).GetCell(ExcelHelper.GetNumByChar('I', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(50).GetCell(ExcelHelper.GetNumByChar('J', 'A'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(50).GetCell(ExcelHelper.GetNumByChar('K', 'A'));
            cell.SetCellFormula(cell.CellFormula);

            //cell = sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('H'));
            //cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(25).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('E'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(28).GetCell(ExcelHelper.GetNumByChar('E'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(29).GetCell(ExcelHelper.GetNumByChar('E'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(30).GetCell(ExcelHelper.GetNumByChar('E'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(31).GetCell(ExcelHelper.GetNumByChar('E'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(32).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(33).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(35).GetCell(ExcelHelper.GetNumByChar('F'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheetQuoteSheet.GetRow(35).GetCell(ExcelHelper.GetNumByChar('B'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(36).GetCell(ExcelHelper.GetNumByChar('B'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(37).GetCell(ExcelHelper.GetNumByChar('B'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(38).GetCell(ExcelHelper.GetNumByChar('B'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(39).GetCell(ExcelHelper.GetNumByChar('B'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheetQuoteSheet.GetRow(40).GetCell(ExcelHelper.GetNumByChar('B'));
            cell.SetCellFormula(cell.CellFormula);

            #region 插入图片

            string imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                using (Image img = System.Drawing.Image.FromFile(imgPath))
                {
                    int col1 = ExcelHelper.GetNumByChar('G');
                    int row1 = 25;// 从该行开始
                    int row2 = 45;  // 结尾行
                    int col2 = ExcelHelper.GetNumByChar('P');

                    bool horizontal = img.Width > img.Height;

                    HSSFClientAnchor anchor = horizontal ?
                        new HSSFClientAnchor(475, 0, 200, 0, col1, row1, col2, row2) :
                            new HSSFClientAnchor(0, 0, 300, 0, ExcelHelper.GetNumByChar('I'), row1, ExcelHelper.GetNumByChar('M'), row2);

                    //插入图片
                    IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                    int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                    HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }
            }

            #endregion 插入图片
            
            #endregion QuoteSheet
        }

        private static void Bind_F20(List<VMQuoteProduct> quoteList, IWorkbook workbook, ISheet sheet)
        {
            #region QuoteSheet

            var font = ExcelHelper.GetFontStyle((XSSFWorkbook)workbook, "Arial", 10, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, font, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            cellStyle.IsLocked = false;

            int thisRow = 4;

            IRow row = null;
            for (int i = 0; i < quoteList.Count; i++)
            {
                var quote = quoteList[i];

                row = sheet.GetRow(thisRow);
                if (row == null)
                    continue;
                row.Height = 2000;

                row.CreateCell(ExcelHelper.GetNumByChar('H')).SetCellValue(quote.Desc + "/" + quote.No);

                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), quote.Desc + "/" + quote.No, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('P'), "", cellStyle);

                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), "Size:" + quote.LengthIN + "X" + quote.WidthIN + "X" + quote.HeightIN + "in Material:" + ExcelHelper.GetStrIngredient(quote.list_ProductIngredient), cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), "SHANGHAI Jet CRAFTS,INC.", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), quote.PackingMannerEnName, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('S'), 90, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('Y'), quote.InnerBoxRate, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('Z'), quote.OuterBoxRate, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A', 'A'), quote.OuterLengthIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B', 'A'), quote.OuterWidthIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C', 'A'), quote.OuterHeightIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G', 'A'), "$" + quote.Cost, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I', 'A'), quote.DutyPercent + "%", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('W', 'A'), "$" + quote.Retail, cellStyle);

                #region 插入图片

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    using (Image img = System.Drawing.Image.FromFile(imgPath))
                    {
                        bool horizontal = img.Width > img.Height;

                        XSSFClientAnchor anchor = horizontal ?
                            new XSSFClientAnchor(75, 0, 800, 0, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('D'), thisRow + 1) :
                                new XSSFClientAnchor(600, 0, 0, 0, ExcelHelper.GetNumByChar('C'), thisRow, ExcelHelper.GetNumByChar('D'), thisRow + 1);

                        //插入图片
                        IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                        int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                        XSSFPicture picture = (XSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                        picture.LineStyle = LineStyle.Solid;
                    }
                }

                #endregion 插入图片

                ++thisRow;
            }

            #endregion QuoteSheet

            sheet.ForceFormulaRecalculation = true;//设置公式


            ISheet newSheet3 = (XSSFSheet)workbook.GetSheetAt(3);
            for (int i = 1; i < 968; i++)
            {
                ICell cell = newSheet3.GetRow(i).GetCell(ExcelHelper.GetNumByChar('C'));
                cell.SetCellFormula(cell.CellFormula);
            }

            ISheet newSheet4 = (XSSFSheet)workbook.GetSheetAt(4);
            for (int i = 1; i < 89; i++)
            {
                ICell cell = newSheet4.GetRow(i).GetCell(ExcelHelper.GetNumByChar('C'));
                cell.SetCellFormula(cell.CellFormula);
            }

            for (int i = 2; i <= 6; i++)
            {
                workbook.SetSheetHidden(i, SheetState.Hidden);
            }
        }

        private static void Bind_F90(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            #region QuoteSheet

            int startIndex = 5;

            //sheet.GetRow(4).GetCell(13).SetCellValue(DateTime.Now.ToShortDateString());
            IRow row = sheet.CreateRow(startIndex + 21 * (int)Math.Ceiling(quoteList.Count() / 1.0));
            row.CreateCell(5).SetCellValue("");

            int colIndex = 0;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "", 10);
            var dataFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "", 10, false);
            var grayColor = ExcelHelper.GetXLColour((HSSFWorkbook)workbook, Color.Gray);

            foreach (var quote in quoteList)
            {
                sheet.AddMergedRegion(new CellRangeAddress(startIndex, startIndex + 21, colIndex, colIndex + 4));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex, startIndex, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 1, startIndex + 1, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 2, startIndex + 3, colIndex + 5, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 4, startIndex + 4, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 5, startIndex + 5, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 6, startIndex + 6, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 7, startIndex + 7, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 8, startIndex + 8, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 9, startIndex + 9, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 10, startIndex + 10, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 11, startIndex + 11, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 13, startIndex + 13, colIndex + 6, colIndex + 7));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 14, startIndex + 14, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 15, startIndex + 15, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 16, startIndex + 16, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 17, startIndex + 17, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 18, startIndex + 18, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 19, startIndex + 19, colIndex + 6, colIndex + 8));
                sheet.AddMergedRegion(new CellRangeAddress(startIndex + 20, startIndex + 20, colIndex + 6, colIndex + 8));

                row = sheet.GetRow(startIndex);
                if (row == null)
                    row = sheet.CreateRow(startIndex);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Item No.:");
                row.CreateCell(colIndex + 6).SetCellValue(quote.No);

                row = sheet.GetRow(startIndex + 1);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 1);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Description:");

                row = sheet.GetRow(startIndex + 2);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 2);
                row.CreateCell(colIndex + 5).SetCellValue(quote.Desc);
                row.HeightInPoints = 12.95f;

                row = sheet.GetRow(startIndex + 4);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 4);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Item size (cm):");
                row.CreateCell(colIndex + 6).SetCellValue(quote.Length + "L X " + quote.Width + "W X " + quote.Height + "H ");

                row = sheet.GetRow(startIndex + 5);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 5);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Color / Assortment:");
                row.CreateCell(colIndex + 6).SetCellValue(quote.StyleName);

                row = sheet.GetRow(startIndex + 6);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 6);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Material Composition:");
                row.CreateCell(colIndex + 6).SetCellValue(quote.IngredientEn);

                row = sheet.GetRow(startIndex + 7);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 7);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Individual Packaging");
                row.CreateCell(colIndex + 6).SetCellValue(quote.PackingMannerEnName);

                row = sheet.GetRow(startIndex + 8);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 8);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Inner Box (pcs)");
                row.CreateCell(colIndex + 6).SetCellValue(quote.InnerBoxRate.ToString());

                row = sheet.GetRow(startIndex + 9);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 9);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Export Carton(pcs)  ");
                row.CreateCell(colIndex + 6).SetCellValue(quote.OuterBoxRate.ToString());

                row = sheet.GetRow(startIndex + 10);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 10);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("CBM of ctn");
                row.CreateCell(colIndex + 6).SetCellValue((quote.OuterLength * quote.OuterWidth * quote.OuterHeight / 1000000).ToString());

                row = sheet.GetRow(startIndex + 11);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 11);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Carton dimension (cm)");
                row.CreateCell(colIndex + 6).SetCellValue(quote.OuterLength + "L X " + quote.OuterWidth + "W X " + quote.OuterHeight + "H ");

                row = sheet.GetRow(startIndex + 12);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 12);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("G.W.  (Kgs)");
                row.CreateCell(colIndex + 6).SetCellValue(quote.OuterWeightGross.ToString());
                row.CreateCell(colIndex + 7).SetCellValue("N.W. (Kgs)");
                row.CreateCell(colIndex + 8).SetCellValue(quote.OuterWeightNet.ToString());

                row = sheet.GetRow(startIndex + 13);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 13);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Unit Price (US$):");
                row.CreateCell(colIndex + 6).SetCellValue(quote.Cost.ToString());
                row.CreateCell(colIndex + 8).SetCellValue("per pc ");

                row = sheet.GetRow(startIndex + 14);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 14);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("F.O.B. Port");
                row.CreateCell(colIndex + 6).SetCellValue(quote.PortEnName.ToString());

                row = sheet.GetRow(startIndex + 15);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 15);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Country of origin");
                row.CreateCell(colIndex + 6).SetCellValue("China");

                row = sheet.GetRow(startIndex + 16);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 16);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("UPC");
                row.CreateCell(colIndex + 6).SetCellValue(quote.UPC);

                row = sheet.GetRow(startIndex + 17);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 17);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("'Min. Order Qty. :");
                row.CreateCell(colIndex + 6).SetCellValue(quote.MOQEn.ToString());

                row = sheet.GetRow(startIndex + 18);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 18);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Qty. for 1x20':");

                row = sheet.GetRow(startIndex + 19);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 19);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Qty. for 1x40':");

                row = sheet.GetRow(startIndex + 20);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 20);
                row.HeightInPoints = 12.95f;
                row.CreateCell(colIndex + 5).SetCellValue("Remarks:");
                row.CreateCell(colIndex + 6).SetCellValue(quote.Remarks);

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    //插入图片
                    IDrawing patriarch = sheet.CreateDrawingPatriarch();
                    //second picture
                    int imageId2 = ExcelHelper.LoadImage(imgPath, workbook);
                    HSSFClientAnchor anchor2 = new HSSFClientAnchor(325, 3, 1000, 250, colIndex, startIndex, colIndex + 3, startIndex + 20);
                    //判断图片的大小
                    var img = System.Drawing.Image.FromFile(imgPath);
                    if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)
                    {
                        anchor2 = new HSSFClientAnchor(0, 3, 1000, 230, colIndex, startIndex + 2, colIndex + 4, startIndex + 17);
                    }
                    HSSFPicture picture2 = (HSSFPicture)patriarch.CreatePicture(anchor2, imageId2);
                    picture2.LineStyle = LineStyle.Solid;
                }

                startIndex += 21;
            }

            #endregion QuoteSheet
        }

        private static void Bind_Form1(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            #region QuoteSheet

            int startIndex = 5;

            sheet.GetRow(4).GetCell(13).SetCellValue(DateTime.Now.ToShortDateString());
            IRow row = sheet.CreateRow(startIndex + 25 * (int)Math.Ceiling(quoteList.Count() / 2.0));
            row.CreateCell(0).SetCellValue("");

            int colIndex = 0;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "", 10);
            var dataFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "", 10, false);
            var grayColor = ExcelHelper.GetXLColour((HSSFWorkbook)workbook, Color.Gray);

            foreach (var quote in quoteList)
            {
                sheet.AddMergedRegion(new CellRangeAddress(startIndex, startIndex + 10, colIndex, colIndex + 8));

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    //插入图片
                    IDrawing patriarch = sheet.CreateDrawingPatriarch();
                    //create the anchor
                    //del by hxg 暂时删除 HSSFClientAnchor anchor = new HSSFClientAnchor(500, 200, 0, 0, 2, 2, 4, 7);
                    //anchor.AnchorType = 2;
                    //load the picture and get the picture index in the workbook
                    //first picture
                    //int imageId =  ExcelHelper. ExcelHelper.LoadImage( Path.Combine(this.txtImagePath.Text,"JK23922.jpg"), workbook);
                    //HSSFPicture picture = (HSSFPicture)patriarch.CreatePicture(anchor, imageId);
                    //Reset the image to the original size.
                    //picture.Resize();   //Note: Resize will reset client anchor you set.
                    //del by hxg 暂时删除 picture.LineStyle = LineStyle.DashDotGel;

                    int imageId2 = ExcelHelper.LoadImage(imgPath, workbook);
                    HSSFClientAnchor anchor2 = new HSSFClientAnchor(0, 5, 420, 0, colIndex + 1, startIndex, colIndex + 5, startIndex + 11);
                    //判断图片的大小
                    var img = System.Drawing.Image.FromFile(imgPath);
                    if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)
                    {
                        anchor2 = new HSSFClientAnchor(462, 20, 842, 0, colIndex, startIndex, colIndex + 6, startIndex + 11);
                    }

                    //second picture
                    HSSFPicture picture2 = (HSSFPicture)patriarch.CreatePicture(anchor2, imageId2);
                    picture2.LineStyle = LineStyle.Solid;
                }

                row = sheet.GetRow(startIndex + 11);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 11);

                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8));
                row.CreateCell(colIndex).SetCellValue("Item#:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue(quote.No);

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 12);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 12);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8));
                row.CreateCell(colIndex).SetCellValue("Description:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue(quote.Desc);

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 13);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 13);
                row.CreateCell(colIndex).SetCellValue("Product Size:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 2).SetCellValue("L  X");
                row.CreateCell(colIndex + 1).SetCellValue(quote.LengthIN.Round().ToString());
                row.CreateCell(colIndex + 4).SetCellValue("W  X");
                row.CreateCell(colIndex + 3).SetCellValue(quote.WidthIN.Round().ToString());
                row.CreateCell(colIndex + 6).SetCellValue("H (inch)");
                row.CreateCell(colIndex + 5).SetCellValue(quote.HeightIN.Round().ToString());

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 14);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 14);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 7));
                row.CreateCell(colIndex).SetCellValue("Inner Pack:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                //取得Inner Pack的值的前面以为数字
                //var innerSize = "0";
                //if (!string.IsNullOrWhiteSpace(quote.InnerSize))
                //{
                //    if (quote.InnerSize.Contains("/") && !quote.InnerSize.StartsWith("/"))
                //    {
                //        innerSize = quote.InnerSize.Substring(0,quote.InnerSize.IndexOf("/"));
                //    }
                //    else
                //    {
                //        var ss=0;
                //        int.TryParse(quote.InnerSize,out ss) ;
                //        innerSize = ss.ToString();
                //    }
                //}

                row.CreateCell(colIndex + 1).SetCellValue(quote.InnerBoxRate.ToString());
                row.CreateCell(colIndex + 8).SetCellValue("pcs");

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 15);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 15);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 7));
                row.CreateCell(colIndex).SetCellValue("Case Pack:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue(quote.OuterBoxRate.ToString());
                row.CreateCell(colIndex + 8).SetCellValue("pcs");

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 16);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 16);
                row.CreateCell(colIndex).SetCellValue("Case Dimensions:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 2).SetCellValue("L  X");
                row.CreateCell(colIndex + 1).SetCellValue(quote.OuterLengthIN.Round().ToString());
                row.CreateCell(colIndex + 4).SetCellValue("W  X");
                row.CreateCell(colIndex + 3).SetCellValue(quote.OuterWidthIN.Round().ToString());
                row.CreateCell(colIndex + 6).SetCellValue("H (inch)");
                row.CreateCell(colIndex + 5).SetCellValue(quote.OuterHeightIN.Round().ToString());

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 17);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 17);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 7));
                row.CreateCell(colIndex).SetCellValue("Cube:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue(quote.OuterVolume.Round().ToString());
                row.CreateCell(colIndex + 8).SetCellValue("cbft");

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 18);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 18);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 7));
                row.CreateCell(colIndex).SetCellValue("FOB NINGBO  ");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue("$" + quote.Cost ?? 0.ToString("F2"));
                row.CreateCell(colIndex + 8).SetCellValue("/pc");

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 19);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 19);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8));
                row.CreateCell(colIndex).SetCellValue("Packaging Type:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue(quote.PackingMannerEnName);

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                row = sheet.GetRow(startIndex + 20);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 20);
                sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 7));
                row.CreateCell(colIndex).SetCellValue("MOQ:");
                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.CreateCell(colIndex + 1).SetCellValue(quote.MOQEn.ToString());
                row.CreateCell(colIndex + 8).SetCellValue("pcs");

                sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, colIndex + 1, colIndex + 8), NPOI.SS.UserModel.BorderStyle.Thin, NPOI.HSSF.Util.HSSFColor.Black.Index);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Center, VerticalAlignment.Center, true);

                //新起一行作两列之间的间隔
                row = sheet.GetRow(startIndex + 21);
                if (row == null)
                    row = sheet.CreateRow(startIndex + 21);
                row.HeightInPoints = 16f;
                //sheet.SetBorderBottomOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, 0, 17), NPOI.SS.UserModel.BorderStyle.Thick, NPOI.HSSF.Util.HSSFColor.Black.Index);
                //sheet.SetBorderTopOfRegion(new CellRangeAddress(row.RowNum, row.RowNum, 0, 17), NPOI.SS.UserModel.BorderStyle.Thick, NPOI.HSSF.Util.HSSFColor.Black.Index);
                //sheet.AddMergedRegion(new CellRangeAddress(row.RowNum, row.RowNum, 0, 17));
                //var cellStyle =  ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, dataFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false, grayColor);
                ////row.RowStyle = cellStyle;
                //row.CreateCell(colIndex).CellStyle = cellStyle;

                if (colIndex == 9)
                {
                    if (startIndex < 25)
                    {
                        startIndex += 23;
                    }
                    else
                    {
                        startIndex += 25;
                    }
                    colIndex = 0;
                }
                else
                {
                    colIndex = 9;
                }
            }

            #endregion QuoteSheet
        }

        private static void Bind_S13(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            #region QuoteSheet

            sheet = (HSSFSheet)workbook.GetSheetAt(1);
            HSSFSheet sheetImg = (HSSFSheet)workbook.GetSheetAt(1);

            ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), Utils.DateTimeToStr(DateTime.Now));

            int startIndex = 19;
            foreach (var quote in quoteList)
            {
                #region QuoteSheet

                IRow row = sheet.GetRow(startIndex);
                if (row == null)
                    row = sheet.CreateRow(startIndex);

                int colIndex = 1;
                row.GetCell(colIndex).SetCellValue(quote.No);
                row.GetCell(colIndex + 1).SetCellValue(quote.Desc);
                row.GetCell(colIndex + 2).SetCellValue(quote.ColorEngName);
                row.GetCell(colIndex + 3).SetCellValue(quote.StyleName);
                row.GetCell(colIndex + 4).SetCellValue(quote.PackingMannerEnName);
                row.GetCell(colIndex + 5).SetCellValue("$" + quote.Cost.ToString());
                //row.GetCell(colIndex + 6).SetCellValue(quote.UnitName.ToString());
                row.GetCell(colIndex + 7).SetCellValue(quote.OuterBoxRate.ToString());
                row.GetCell(colIndex + 8).SetCellValue(quote.InnerBoxRate.ToString());
                row.GetCell(colIndex + 9).SetCellValue(quote.INR.ToString());
                row.GetCell(colIndex + 10).SetCellValue(quote.OuterLengthIN.ToString());
                row.GetCell(colIndex + 11).SetCellValue(quote.OuterWidthIN.ToString());
                row.GetCell(colIndex + 12).SetCellValue(quote.OuterHeightIN.ToString());
                row.GetCell(colIndex + 13).SetCellValue(quote.InnerLengthIN.ToString());
                row.GetCell(colIndex + 14).SetCellValue(quote.InnerWidthIN.ToString());
                row.GetCell(colIndex + 15).SetCellValue(quote.InnerHeightIN.ToString());
                row.GetCell(colIndex + 16).SetCellValue(quote.LengthIN.ToString());
                row.GetCell(colIndex + 17).SetCellValue(quote.WidthIN.ToString());
                row.GetCell(colIndex + 18).SetCellValue(quote.HeightIN.ToString());
                row.GetCell(colIndex + 19).SetCellFormula(string.Format("(L{0}/12)*(M{0}/12)*(N{0}/12)", startIndex + 1));
                row.GetCell(colIndex + 20).SetCellValue(quote.OuterWeightGrossLBS.ToString());
                row.GetCell(colIndex + 21).SetCellValue(quote.WeightLBS.ToString());
                row.GetCell(colIndex + 22).SetCellValue(quote.PortEnName.ToString());

                row.GetCell(colIndex + 23).SetCellValue(ExcelHelper.GetStrIngredient(quote.list_ProductIngredient));
                row.GetCell(colIndex + 25).SetCellValue(quote.MOQEn.ToString());
                row.GetCell(colIndex + 26).SetCellValue("60 Days");
                row.GetCell(colIndex + 27).SetCellValue("No");
                row.GetCell(colIndex + 28).SetCellValue("No");
                row.GetCell(colIndex + 29).SetCellValue("Quotation Only");
                row.GetCell(colIndex + 30).SetCellValue("This is extra space to write reference notes about the item to the buyer.");
                row.GetCell(colIndex + 31).SetCellValue("This is extra space to write reference notes about the item to the buyer.");
                row.GetCell(colIndex + 32).SetCellValue(quote.Buyer);
                row.GetCell(colIndex + 35).SetCellValue("SE");
                row.GetCell(colIndex + 38).SetCellValue(quote.DutyPercent.ToString());
                row.GetCell(colIndex + 39).SetCellFormula("IF($B$2=\"Import\",$AO$19,IF($B$2=\"Domestic\",0,$AO$19))");
                row.GetCell(colIndex + 40).SetCellFormula(string.Format("(U{0}*AO{0})/I{0}", startIndex + 1));
                row.GetCell(colIndex + 41).SetCellFormula(string.Format("G{0}*AN{0}", startIndex + 1));
                row.GetCell(colIndex + 42).SetCellFormula(string.Format("G{0}+AP{0}+AQ{0}", startIndex + 1));

                #endregion QuoteSheet

                #region 图片

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    sheetImg = (HSSFSheet)workbook.GetSheetAt(startIndex - 16);
                    //插入图片
                    IDrawing patriarch1 = sheetImg.CreateDrawingPatriarch();
                    int imageId = ExcelHelper.LoadImage(imgPath, workbook);
                    HSSFClientAnchor anchor = new HSSFClientAnchor(0, 5, 1000, 240, 9, 19, 12, 34);
                    //判断图片的大小
                    var img = System.Drawing.Image.FromFile(imgPath);
                    if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)
                    {
                        anchor = new HSSFClientAnchor(5, 5, 500, 240, 7, 19, 14, 34);
                    }
                    HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }

                #endregion 图片

                startIndex++;

                if (startIndex == 29)
                {
                    startIndex = 19;
                }
            }

            ICell cell = null;

            sheet = (HSSFSheet)workbook.GetSheetAt(2);
            for (int i = 2; i <= 34; i++)
            {
                if (i != 6 && i != 17 && i != 18 && i != 19 && i != 24)
                {
                    char[] arr = { 'B', 'F', 'J', 'N', 'R' };
                    for (int j = 0; j < arr.Length; j++)
                    {
                        cell = sheet.GetRow(i).GetCell(ExcelHelper.GetNumByChar(arr[j]));
                        cell.SetCellFormula(cell.CellFormula);
                        if (i == 11)
                        {
                            cell = sheet.GetRow(i).GetCell(ExcelHelper.GetNumByChar((char)(arr[j] + 1)));
                            cell.SetCellFormula(cell.CellFormula);
                        }
                    }
                }
            }

            for (int i = 0; i < quoteList.Count; i++)
            {
                sheet = (HSSFSheet)workbook.GetSheetAt(i + 3);

                ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), Utils.DateTimeToStr(DateTime.Now));

                cell = sheet.GetRow(12).GetCell(ExcelHelper.GetNumByChar('K'));
                cell.SetCellFormula(cell.CellFormula);

                cell = sheet.GetRow(12).GetCell(ExcelHelper.GetNumByChar('N'));
                cell.SetCellFormula(cell.CellFormula);

                cell = sheet.GetRow(16).GetCell(ExcelHelper.GetNumByChar('N'));
                cell.SetCellFormula(cell.CellFormula);

                for (int j = 3; j <= 14; j++)
                {
                    if (j != 10)
                    {
                        cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('C'));
                        cell.SetCellFormula(cell.CellFormula);
                    }

                    cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('K'));
                    cell.SetCellFormula(cell.CellFormula);
                }

                for (int j = 15; j <= 17; j++)
                {
                    cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('C'));
                    cell.SetCellFormula(cell.CellFormula);

                    cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('D'));
                    cell.SetCellFormula(cell.CellFormula);

                    cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('E'));
                    cell.SetCellFormula(cell.CellFormula);

                    cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('K'));
                    cell.SetCellFormula(cell.CellFormula);
                }

                cell = sheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('C'));
                cell.SetCellFormula(cell.CellFormula);

                cell = sheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('C'));
                cell.SetCellFormula(cell.CellFormula);

                cell = sheet.GetRow(24).GetCell(ExcelHelper.GetNumByChar('C'));
                cell.SetCellFormula(cell.CellFormula);

                cell = sheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('C'));
                cell.SetCellFormula(cell.CellFormula);

                for (int j = 28; j <= 32; j++)
                {
                    cell = sheet.GetRow(j).GetCell(ExcelHelper.GetNumByChar('C'));
                    cell.SetCellFormula(cell.CellFormula);
                }

                cell = sheet.GetRow(33).GetCell(ExcelHelper.GetNumByChar('A'));
                cell.SetCellFormula(cell.CellFormula);

                cell = sheet.GetRow(34).GetCell(ExcelHelper.GetNumByChar('A'));
                cell.SetCellFormula(cell.CellFormula);
            }

            #endregion QuoteSheet
        }

        private static ICell GetCell(ISheet sheet, IRow row, int col)
        {
            ICell cell = row.GetCell(col) ?? row.CreateCell(col);
            if (row.RowNum > 3)
                cell.CellStyle = sheet.GetRow(3).GetCell(col).CellStyle;

            return cell;
        }

        private static void Bind_S135(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            #region QuoteSheet

            int startIndex = 3;
            // Item Photo高150像素
            foreach (var quote in quoteList)
            {
                var row = sheet.GetRow(startIndex);
                if (row == null)
                {
                    row = sheet.CreateRow(startIndex);
                    row.HeightInPoints = 150;
                }
                GetCell(sheet, row, ExcelHelper.GetNumByChar('A')).SetCellValue(quote.No);
                GetCell(sheet, row, ExcelHelper.GetNumByChar('B')).SetCellValue(quote.Desc);
                GetCell(sheet, row, ExcelHelper.GetNumByChar('D')).SetCellValue(quote.StyleName);
                GetCell(sheet, row, ExcelHelper.GetNumByChar('E'));
                GetCell(sheet, row, ExcelHelper.GetNumByChar('F')).SetCellValue(quote.LengthIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('G')).SetCellValue(quote.LengthIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('H')).SetCellValue(quote.HeightIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('I'));
                GetCell(sheet, row, ExcelHelper.GetNumByChar('J')).SetCellValue(quote.InnerLengthIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('K')).SetCellValue(quote.InnerWidthIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('L')).SetCellValue(quote.InnerHeightIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('M')).SetCellValue(quote.InnerVolume.ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('N')).SetCellValue(quote.OuterBoxRate.ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('O')).SetCellValue(quote.OuterLengthIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('P')).SetCellValue(quote.OuterWidthIN.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('Q')).SetCellValue(quote.OuterBoxRate.ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('R')).SetCellValue(quote.OuterVolume.ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('S'));
                GetCell(sheet, row, ExcelHelper.GetNumByChar('T')).SetCellValue(quote.Cost.Round().ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('U')).SetCellValue(quote.PortEnName.ToString());
                GetCell(sheet, row, ExcelHelper.GetNumByChar('V')).SetCellValue(quote.Remarks);

                //ICellStyle cellStyle = workbook.GetCellStyleAt(0);
                //cellStyle.WrapText = true;
                //GetCell(sheet, row,  ExcelHelper.GetNumByChar('B')).CellStyle = cellStyle;

                #region 图片

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    using (Image img = System.Drawing.Image.FromFile(imgPath))
                    {
                        bool horizontal = img.Width > img.Height;

                        HSSFClientAnchor anchor = horizontal ?
                            new HSSFClientAnchor(125, 0, 975, 0, ExcelHelper.GetNumByChar('C'), startIndex, ExcelHelper.GetNumByChar('D'), startIndex + 1) :
                                new HSSFClientAnchor(0, 0, 750, 0, ExcelHelper.GetNumByChar('C'), startIndex, ExcelHelper.GetNumByChar('D'), startIndex + 1);

                        //插入图片
                        IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                        int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                        HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                        picture.LineStyle = LineStyle.Solid;
                    }
                }

                #endregion 图片

                startIndex++;
            }

            #endregion QuoteSheet
        }

        private static void Bind_S164(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            int startIndex = 2;
            sheet.GetRow(0).CreateCell(14).SetCellValue("Date:" + DateTime.Now.ToShortDateString());
            IRow row = sheet.CreateRow(startIndex + quoteList.Count());
            row.CreateCell(0).SetCellValue("");

            int colIndex = 0;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "", 12);
            var dataFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "", 12, false);
            //dataFont8.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
            //dataFont8.Boldweight = 1;
            var grayColor = ExcelHelper.GetXLColour((HSSFWorkbook)workbook, Color.Gray);

            foreach (var quote in quoteList)
            {
                #region QuoteSheet

                row = sheet.GetRow(startIndex);
                if (row == null)
                    row = sheet.CreateRow(startIndex);
                row.HeightInPoints = 95f;

                colIndex = 1;
                row.CreateCell(colIndex).SetCellValue(quote.No);
                row.CreateCell(colIndex + 1).SetCellValue(quote.Desc);
                row.CreateCell(colIndex + 2).SetCellValue(quote.HeightIN.Round() + "\" x " + quote.LengthIN.Round() + "\" x " + quote.WidthIN.Round() + "\"");
                row.CreateCell(colIndex + 3).SetCellValue(quote.InnerBoxRate.ToString());
                row.CreateCell(colIndex + 4).SetCellValue(quote.OuterBoxRate.ToString());
                row.CreateCell(colIndex + 5).SetCellValue(quote.OuterHeightIN.Round() + "\" x " + quote.OuterLengthIN.Round() + "\" x " + quote.OuterWidthIN.Round() + "\"");
                row.CreateCell(colIndex + 6).SetCellValue("factory overseas");
                //row.CreateCell(colIndex + 7).SetCellValue(quote.TruckloadQty);
                row.CreateCell(colIndex + 8).SetCellValue(quote.PortEnName.ToString());
                row.CreateCell(colIndex + 9).SetCellValue(quote.FinalFOB.ToString());
                row.CreateCell(colIndex + 10).SetCellValue(quote.Cost.ToString());
                row.CreateCell(colIndex + 11).SetCellValue(quote.Retail.ToString());
                //row.CreateCell(colIndex + 12).SetCellValue(quote.AW_Material);

                row.GetCell(colIndex).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 1).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 2).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 3).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 4).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 5).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 6).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 8).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 9).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 10).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);
                row.GetCell(colIndex + 11).CellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, false);

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    //插入图片
                    IDrawing patriarch = sheet.CreateDrawingPatriarch();
                    //second picture
                    int imageId2 = ExcelHelper.LoadImage(imgPath, workbook);
                    HSSFClientAnchor anchor2 = new HSSFClientAnchor(276, 3, 615, 250, colIndex + 13, startIndex, colIndex + 13, startIndex);
                    //判断图片的大小
                    var img = System.Drawing.Image.FromFile(imgPath);
                    if (img.Width > 1150 && img.Width < 1250 && img.Height > 850 && img.Height < 950)
                    {
                        anchor2 = new HSSFClientAnchor(160, 3, 810, 250, colIndex + 13, startIndex, colIndex + 13, startIndex);
                    }
                    HSSFPicture picture2 = (HSSFPicture)patriarch.CreatePicture(anchor2, imageId2);
                    picture2.LineStyle = LineStyle.Solid;
                }

                startIndex++;

                #endregion QuoteSheet
            }
        }

        private static void Bind_S188(List<VMQuoteProduct> quoteList, IWorkbook workbook, ISheet sheet)
        {
            #region QuoteSheet

            int startIndex = 15;
            var boldFont = ExcelHelper.GetFontStyle((XSSFWorkbook)workbook, "", 12, false);

            foreach (var quote in quoteList)
            {
                #region QuoteSheet

                IRow row = sheet.GetRow(startIndex);
                if (row == null)
                {
                    row = sheet.CreateRow(startIndex);
                }
                row.Height = 3000;
                row.GetCell(1).SetCellValue(quote.Desc);
                row.GetCell(3).SetCellValue(quote.No);
                row.GetCell(4).SetCellValue(quote.InnerBoxRate.ToString());
                row.GetCell(5).SetCellValue(quote.OuterBoxRate.ToString());
                row.GetCell(6).SetCellValue(quote.OuterVolume.ToString());
                row.GetCell(7).SetCellValue(quote.PackingMannerEnName);
                row.GetCell(14).SetCellValue(quote.HTS_Name);
                row.GetCell(15).SetCellValue(quote.DutyPercent.ToString());
                row.GetCell(19).SetCellValue((quote.Retail.Round() - 0.01m).ToString());//SPR = Retail.Round() - 0.01m
                row.GetCell(21).SetCellValue(quote.PortEnName);

                row.GetCell(1).CellStyle = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, true);
                row.GetCell(7).CellStyle = ExcelHelper.GetCellStyle((XSSFWorkbook)workbook, boldFont, NPOI.SS.UserModel.HorizontalAlignment.Left, VerticalAlignment.Center, true);

                //TODO 计算列有问题
                ICell cell = row.GetCell(9);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(16);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(17);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(18);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(20);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(31);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(34);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(35);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(36);
                //cell.SetCellFormula(cell.CellFormula);
                //cell = row.GetCell(37);
                //cell.SetCellFormula(cell.CellFormula);

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    using (Image img = System.Drawing.Image.FromFile(imgPath))
                    {
                        bool horizontal = img.Width > img.Height;

                        XSSFClientAnchor anchor = horizontal ?
                            new XSSFClientAnchor(20, 10, 1000, 70, 0, startIndex, 1, startIndex + 1) :
                                new XSSFClientAnchor(1000, 0, 0, 1, 0, startIndex, 1, startIndex + 1);

                        //插入图片
                        IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                        int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                        XSSFPicture picture = (XSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                        picture.LineStyle = LineStyle.Solid;
                    }
                }

                ++startIndex;

                #endregion QuoteSheet
            }

            #endregion QuoteSheet
        }

        private static void Bind_S56(List<VMQuoteProduct> quoteList, IWorkbook workbook)
        {
            DateTime now = DateTime.Now;
            string dateString = now.ToString(@"MMM. dd\t\h, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            int curQuote = 0;
            foreach (var quote in quoteList)
            {
                ++curQuote;
                ISheet sheetQuoteSheet = workbook.GetSheetAt(curQuote + 1);

                //if (curQuote < quoteList.Count)
                //{
                //    workbook.CloneSheet(workbook.NumberOfSheets - 1);
                //    workbook.SetSheetName(workbook.NumberOfSheets - 1, (workbook.NumberOfSheets - 2).ToString());
                //}

                #region 填充数据

                // DATE
                ICell cell = sheetQuoteSheet.GetRow(1).GetCell(ExcelHelper.GetNumByChar('O'));
                cell.SetCellValue(dateString);

                // Tariff Number:
                sheetQuoteSheet.GetRow(13).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue(quote.HTS_Name);

                // Factory Item #
                sheetQuoteSheet.GetRow(14).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.No);

                // F.O.B. Port:
                sheetQuoteSheet.GetRow(14).GetCell(ExcelHelper.GetNumByChar('J')).SetCellValue(quote.PortEnName.ToString());

                // Factory ID:
                sheetQuoteSheet.GetRow(11).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue(quote.Factory_EnglishName);

                // Description
                sheetQuoteSheet.GetRow(15).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.Desc);

                // Material
                sheetQuoteSheet.GetRow(16).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(ExcelHelper.GetStrIngredient(quote.list_ProductIngredient));

                // Type of Packing
                sheetQuoteSheet.GetRow(18).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.PackingMannerEnName);

                // Item Meas.(inches) H:
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.HeightIN.Round().ToString());
                // Item Meas.(inches) W:
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue(quote.WidthIN.Round().ToString());
                // Item Meas.(inches) D:
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('G')).SetCellValue(quote.LengthIN.Round().ToString());

                // Carton Meas. (inches) L:
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('I')).SetCellValue(quote.OuterHeightIN.ToString());
                // Carton Meas. (inches) W:
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('J')).SetCellValue(quote.OuterWidthIN.ToString());
                // Carton Meas. (inches) H:
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('K')).SetCellValue(quote.OuterLengthIN.ToString());

                // Carton Weight (lbs)
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('L')).SetCellValue(quote.OuterWeightGrossLBS.Round().ToString());

                // Cube ( cft )
                sheetQuoteSheet.GetRow(20).GetCell(ExcelHelper.GetNumByChar('N')).SetCellValue(quote.OuterVolume.Round().ToString());

                // Inner Bag(Qty)
                sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('F')).SetCellValue(quote.InnerBoxRate.ToString());

                // Master (Qty)
                sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('J')).SetCellValue(quote.OuterBoxRate.ToString());

                // Minimum Order Quantity:
                sheetQuoteSheet.GetRow(22).GetCell(ExcelHelper.GetNumByChar('N')).SetCellValue(quote.MOQEn.ToString());

                // F.O.B. Cost $
                sheetQuoteSheet.GetRow(25).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.Cost.ToString());

                // Duty $
                sheetQuoteSheet.GetRow(27).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue((quote.DutyPercent).ToString());


                sheetQuoteSheet.ForceFormulaRecalculation = true;

                #endregion 填充数据

                #region 插入图片

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    using (Image img = System.Drawing.Image.FromFile(imgPath))
                    {
                        bool horizontal = img.Width > img.Height;

                        HSSFClientAnchor anchor = horizontal ?
                            new HSSFClientAnchor(125, 0, 975, 0, 6, 24, 14, 38) :
                                new HSSFClientAnchor(0, 0, 750, 0, 8, 24, 13, 38);

                        //插入图片
                        IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                        int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                        HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                        picture.LineStyle = LineStyle.Solid;
                    }
                }

                #endregion 插入图片

            }

            #region 索引表

            ISheet indexSheet = workbook.GetSheetAt(1);
            int rowStart = 5;
            int colSbar = 2;
            int colFactory = 4;
            for (int i = 0; i < 50; i++)
            {
                int rowIndex = rowStart + i;
                IRow row = indexSheet.GetRow(rowIndex);
                row.GetCell(colSbar).SetCellValue("");
                if (i >= curQuote)
                {
                    row.GetCell(colFactory).SetCellValue("");
                    continue;
                }
                row.GetCell(colSbar).SetCellFormula("'" + (i + 1) + "'!C7");
                row.GetCell(colFactory).SetCellFormula("'" + (i + 1) + "'!C15");
            }

            #endregion 索引表


            for (int i = quoteList.Count + 2; i <= 51; i++)
            {
                workbook.SetSheetHidden(i, SheetState.Hidden);
            }
        }

        private static void Bind_S56_2(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            DateTime now = DateTime.Now;
            string dateString = now.ToString(@"MMM. dd\t\h, yyyy", System.Globalization.CultureInfo.InvariantCulture);

            int curQuote = 0;
            foreach (var quote in quoteList)
            {
                ++curQuote;
                ISheet sheetQuoteSheet = workbook.GetSheetAt(workbook.NumberOfSheets - 1);
                if (curQuote < quoteList.Count)
                {
                    workbook.CloneSheet(workbook.NumberOfSheets - 1);
                    workbook.SetSheetName(workbook.NumberOfSheets - 1, (workbook.NumberOfSheets - 2).ToString());
                }

                #region 填充数据

                // DATE
                ICell cell = sheetQuoteSheet.GetRow(0).GetCell(14);
                cell.SetCellValue(dateString);
                // Factory Item #
                cell = sheetQuoteSheet.GetRow(17).GetCell(2);
                cell.SetCellValue(quote.No);
                // Factory ID:
                cell = sheetQuoteSheet.GetRow(17).GetCell(11);
                cell.SetCellValue(quote.Factory_EnglishName);
                // Description
                sheetQuoteSheet.GetRow(18).GetCell(2).SetCellValue(quote.Desc);
                // Material
                sheetQuoteSheet.GetRow(19).GetCell(2).SetCellValue(quote.IngredientEn);
                // Item Meas.(inches) H:
                sheetQuoteSheet.GetRow(21).GetCell(3).SetCellValue(quote.HeightIN.Round().ToString());
                // Item Meas.(inches) W:
                sheetQuoteSheet.GetRow(21).GetCell(5).SetCellValue(quote.LengthIN.Round().ToString());
                // Item Meas.(inches) D:
                sheetQuoteSheet.GetRow(21).GetCell(7).SetCellValue(quote.WidthIN.Round().ToString());
                // Minimum Order Quantity:
                sheetQuoteSheet.GetRow(22).GetCell(2).SetCellValue(quote.MOQEn.ToString());
                // Tariff Number:
                // sheetQuoteSheet.GetRow(22).GetCell(13).SetCellValue(quote.HTS);
                sheetQuoteSheet.GetRow(22).GetCell(13).SetCellValue("");
                // F.O.B. Port:
                sheetQuoteSheet.GetRow(23).GetCell(2).SetCellValue(quote.PortEnName.ToString());
                // Type of Packing
                sheetQuoteSheet.GetRow(25).GetCell(2).SetCellValue(quote.PackingMannerEnName);
                // Inner Bag(Qty)
                sheetQuoteSheet.GetRow(27).GetCell(4).SetCellValue(quote.InnerBoxRate.ToString());
                // Master (Qty)
                sheetQuoteSheet.GetRow(27).GetCell(5).SetCellValue(quote.OuterBoxRate.ToString());
                // Carton Meas. (inches) L:
                sheetQuoteSheet.GetRow(27).GetCell(12).SetCellValue(quote.OuterLengthIN.ToString());
                // Carton Meas. (inches) W:
                sheetQuoteSheet.GetRow(27).GetCell(10).SetCellValue(quote.OuterWidthIN.ToString());
                // Carton Meas. (inches) H:
                sheetQuoteSheet.GetRow(27).GetCell(8).SetCellValue(quote.OuterHeightIN.ToString());
                // Cube ( cft )
                sheetQuoteSheet.GetRow(26).GetCell(14).SetCellValue(quote.OuterVolume.Round().ToString());
                // Carton Weight (lbs)
                sheetQuoteSheet.GetRow(29).GetCell(2).SetCellValue(quote.OuterWeightGrossLBS.Round().ToString());
                // F.O.B. Cost $
                sheetQuoteSheet.GetRow(31).GetCell(2).SetCellValue(quote.Cost.ToString());
                // Duty $
                sheetQuoteSheet.GetRow(33).GetCell(4).SetCellValue((quote.DutyPercent).ToString());

                // Calccuft
                cell = sheetQuoteSheet.GetRow(27).GetCell(14);
                cell.SetCellFormula(cell.CellFormula);
                // Freight $
                cell = sheetQuoteSheet.GetRow(32).GetCell(2);
                cell.SetCellFormula(cell.CellFormula);
                // Duty $
                cell = sheetQuoteSheet.GetRow(33).GetCell(2);
                cell.SetCellFormula(cell.CellFormula);
                // Landed Cost $
                cell = sheetQuoteSheet.GetRow(34).GetCell(2);
                cell.SetCellFormula(cell.CellFormula);
                // Landed Cost $ 第二行第三列
                cell = sheetQuoteSheet.GetRow(35).GetCell(2);
                cell.SetCellFormula(cell.CellFormula);
                // Landed Cost $ 第二行第二列
                cell = sheetQuoteSheet.GetRow(35).GetCell(1);
                cell.SetCellFormula(cell.CellFormula);
                // Retail
                cell = sheetQuoteSheet.GetRow(37).GetCell(2);
                cell.SetCellFormula(cell.CellFormula);
                // Margin
                cell = sheetQuoteSheet.GetRow(38).GetCell(2);
                cell.SetCellFormula(cell.CellFormula);
                // DOMESTIC COST:
                cell = sheetQuoteSheet.GetRow(43).GetCell(6);
                cell.SetCellFormula(cell.CellFormula);

                #endregion 填充数据

                #region 插入图片

                string imgPath = ExcelHelper.GetImage(quote.Image);
                if (File.Exists(imgPath))
                {
                    using (Image img = System.Drawing.Image.FromFile(imgPath))
                    {
                        bool horizontal = img.Width > img.Height;

                        HSSFClientAnchor anchor = horizontal ?
                            new HSSFClientAnchor(125, 0, 975, 0, 6, 28, 13, 41) :
                                new HSSFClientAnchor(0, 0, 750, 0, 8, 28, 12, 41);

                        //插入图片
                        IDrawing patriarch1 = sheetQuoteSheet.CreateDrawingPatriarch();
                        int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                        HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                        picture.LineStyle = LineStyle.Solid;
                    }
                }

                #endregion 插入图片
            }

            #region 索引表

            ISheet indexSheet = workbook.GetSheetAt(1);
            int rowStart = 5;
            int colSbar = 2;
            int colFactory = 4;
            for (int i = 0; i < 50; i++)
            {
                int rowIndex = rowStart + i;
                IRow row = indexSheet.GetRow(rowIndex);
                row.GetCell(colSbar).SetCellValue("");
                if (i >= curQuote)
                {
                    row.GetCell(colFactory).SetCellValue("");
                    continue;
                }
                row.GetCell(colSbar).SetCellFormula("'" + (i + 1) + "'!C2");
                row.GetCell(colFactory).SetCellFormula("'" + (i + 1) + "'!C15");
            }

            #endregion 索引表
        }

        private static void Bind_S220(List<VMQuoteProduct> quoteList, IWorkbook workbook, HSSFSheet sheet)
        {
            ISheet sheetQuoteSheet = workbook.GetSheetAt(0);

            var font = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);

            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, font, HorizontalAlignment.Left, VerticalAlignment.Center, true);

            int thisRow = 2;
            foreach (var item in quoteList)
            {
                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                #region 填充数据

                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle2);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.Desc, cellStyle2);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.UnitEngName, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), item.StyleName, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.Length, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.LengthIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.Width, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.WidthIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.Height, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('K'), item.HeightIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('L'), item.InnerBoxRate, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('M'), item.OuterBoxRate, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('N'), item.OuterLength, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('O'), item.OuterLengthIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('P'), item.OuterWidth, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('Q'), item.OuterWidthIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('R'), item.OuterHeight, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('S'), item.OuterHeightIN, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('T'), item.OuterVolume.Round(), cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('U'), item.Cost, cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('V'), item.PortEnName + ", China", cellStyle);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('W'), item.PackingMannerEnName, cellStyle2);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('X'), item.MOQEn, cellStyle2);
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('Y'), item.Remarks, cellStyle2);


                #endregion 填充数据


                #region 插入图片

                string imgPath = ExcelHelper.GetImage(item.Image);
                if (File.Exists(imgPath))
                {
                    using (Image img = System.Drawing.Image.FromFile(imgPath))
                    {
                        bool horizontal = img.Width > img.Height;

                        HSSFClientAnchor anchor = horizontal ?
                            new HSSFClientAnchor(30, 0, 1000, 0, ExcelHelper.GetNumByChar('B'), thisRow, ExcelHelper.GetNumByChar('B'), thisRow + 1) :
                                new HSSFClientAnchor(600, 0, 0, 0, ExcelHelper.GetNumByChar('B'), thisRow, ExcelHelper.GetNumByChar('B'), thisRow + 1);

                        //插入图片
                        IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                        int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                        HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                        picture.LineStyle = LineStyle.Solid;
                    }
                }

                #endregion 插入图片

                ++thisRow;
            }
        }

        private static void Bind_S05(VMQuoteProduct quote, IWorkbook workbook, ISheet sheet)
        {
            #region QuoteSheet

            sheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.No);

            sheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('F')).SetCellValue(quote.PortEnName + ", CHINA");

            sheet.GetRow(21).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.No);

            sheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.HeightIN.ToString());

            sheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('F')).SetCellValue(quote.WidthIN.ToString());

            sheet.GetRow(26).GetCell(ExcelHelper.GetNumByChar('H')).SetCellValue(quote.LengthIN.ToString());

            sheet.GetRow(27).GetCell(ExcelHelper.GetNumByChar('J')).SetCellValue("$" + quote.Cost.Round(3).ToString());

            sheet.GetRow(28).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.Desc);

            sheet.GetRow(29).GetCell(ExcelHelper.GetNumByChar('J')).SetCellValue(quote.DutyPercent.ToString());

            sheet.GetRow(35).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(ExcelHelper.GetStrIngredient(quote.list_ProductIngredient));
            sheet.GetRow(36).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.HTS_Name);

            sheet.GetRow(43).GetCell(ExcelHelper.GetNumByChar('I')).SetCellValue(quote.PackingMannerEnName);
            sheet.GetRow(44).GetCell(ExcelHelper.GetNumByChar('I')).SetCellValue("MOQ:" + quote.MOQEn + "PCS");

            sheet.GetRow(58).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.OuterBoxRate ?? 0);
            sheet.GetRow(58).GetCell(ExcelHelper.GetNumByChar('G')).SetCellValue(quote.InnerBoxRate ?? 0);
            sheet.GetRow(60).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue(quote.OuterWeightGrossLBS.Round().ToString());

            sheet.GetRow(60).GetCell(ExcelHelper.GetNumByChar('H')).SetCellValue(quote.InnerWeightGrossLBS.Round().ToString());

            sheet.GetRow(62).GetCell(ExcelHelper.GetNumByChar('C')).SetCellValue(quote.OuterHeightIN.ToString());

            sheet.GetRow(62).GetCell(ExcelHelper.GetNumByChar('D')).SetCellValue(quote.OuterWidthIN.ToString());

            sheet.GetRow(62).GetCell(ExcelHelper.GetNumByChar('E')).SetCellValue(quote.OuterLengthIN.ToString());

            sheet.GetRow(62).GetCell(ExcelHelper.GetNumByChar('F')).SetCellValue(quote.InnerHeightIN.ToString());

            sheet.GetRow(62).GetCell(ExcelHelper.GetNumByChar('G')).SetCellValue(quote.InnerWidthIN.ToString());

            sheet.GetRow(62).GetCell(ExcelHelper.GetNumByChar('H')).SetCellValue(quote.InnerLengthIN.ToString());

            #region 插入图片

            string imgPath = ExcelHelper.GetImage(quote.Image);
            if (File.Exists(imgPath))
            {
                using (Image img = System.Drawing.Image.FromFile(imgPath))
                {
                    bool horizontal = img.Width > img.Height;

                    XSSFClientAnchor anchor = horizontal ?
                        new XSSFClientAnchor(20, 10, 1000, 70, ExcelHelper.GetNumByChar('I'), 10, ExcelHelper.GetNumByChar('M'), 26) :
                            new XSSFClientAnchor(1000, 0, 0, 0, ExcelHelper.GetNumByChar('I'), 10, ExcelHelper.GetNumByChar('M'), 26);

                    //插入图片
                    IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                    int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                    XSSFPicture picture = (XSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }
            }

            #endregion 插入图片

            ICell cell = sheet.GetRow(59).GetCell(ExcelHelper.GetNumByChar('E'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheet.GetRow(27).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheet.GetRow(28).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(29).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(30).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(34).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(39).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheet.GetRow(56).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(57).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(58).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);
            cell = sheet.GetRow(59).GetCell(ExcelHelper.GetNumByChar('L'));
            cell.SetCellFormula(cell.CellFormula);

            cell = sheet.GetRow(31).GetCell(ExcelHelper.GetNumByChar('J'));
            cell.SetCellFormula(cell.CellFormula);

            #endregion QuoteSheet
        }

        #endregion 生成报价单模板

        #region HelperMethod

        private static void SaveFile(List<string> outFileList, string outRealDir, IWorkbook workbook, string fileName, QuotProductTypeEnum QuotProductTypeEnum, int page = 1, int type = 1)
        {
            if (!Directory.Exists(outRealDir + "/jpg"))
            {
                Directory.CreateDirectory(outRealDir + "/jpg");
            }
            if (!Directory.Exists(outRealDir + "/PDFAndExcel"))
            {
                Directory.CreateDirectory(outRealDir + "/PDFAndExcel");
            }

            string excelPath = outRealDir + "/PDFAndExcel/" + fileName + GetQuotTemplateExtension(QuotProductTypeEnum);
            string pdfPath = outRealDir + "/PDFAndExcel/" + fileName + ".pdf";
            string jpgPath = outRealDir + "/jpg/" + fileName + ".jpg";

            //生成excel文件
            FileStream swQuoteSheet = File.OpenWrite(excelPath);
            workbook.Write(swQuoteSheet);
            swQuoteSheet.Close();
            outFileList.Add(excelPath);

            AsposeX asposeX = new AsposeX();
            string errMsg;
            if (type == 1)
            {
                //生成pdf文件
                asposeX.ExcelToPdf(excelPath, pdfPath, out errMsg);
            }
            else
            {
                //生成pdf文件
                asposeX.ExcelToPdf(excelPath, pdfPath, out errMsg);
            }

            //生成图片
            asposeX.ExcelToJpg(pdfPath, jpgPath, out errMsg, page);
        }

        #endregion HelperMethod
    }
}