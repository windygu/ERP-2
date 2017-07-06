using ERP.BLL.Consts;
using ERP.Models.Factory;
using ERP.Models.Order;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.Logs;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERP.BLL.ERP.Consts
{
    /// <summary>
    /// Excel的帮助方法
    /// </summary>
    public class ExcelHelper
    {
        #region 设置单元格的内容

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, int value)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value.ToString());
        }

        public static void SetCellValue(XSSFSheet sheet, int rowIndex, int cellIndex, int value)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value.ToString());
        }

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, decimal value)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value.ToString());
        }

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, string value)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value, null);
        }

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, IRichTextString value)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value, null);
        }

        public static void SetCellValue(XSSFSheet sheet, int rowIndex, int cellIndex, string value)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value, null);
        }

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, int value, ICellStyle cellStyle)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value.ToString(), cellStyle);
        }

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, decimal value, ICellStyle cellStyle)
        {
            SetCellValue(sheet, rowIndex, cellIndex, value.ToString(), cellStyle);
        }

        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, decimal? value, ICellStyle cellStyle)
        {
            string temp = null;
            if (value.HasValue)
            {
                temp = value.Value.ToString();
            }
            SetCellValue(sheet, rowIndex, cellIndex, temp, cellStyle);
        }

        /// <summary>
        /// 设置单元格的内容
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, IRichTextString value, ICellStyle cellStyle)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }
            row.CreateCell(cellIndex).SetCellValue(value);

            if (cellStyle != null)
            {
                row.GetCell(cellIndex).CellStyle = cellStyle;
            }
        }

        /// <summary>
        /// 设置单元格的内容
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        public static void SetCellValue(ISheet sheet, int rowIndex, int cellIndex, string value, ICellStyle cellStyle)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }
            row.CreateCell(cellIndex).SetCellValue(value);

            if (cellStyle != null)
            {
                row.GetCell(cellIndex).CellStyle = cellStyle;
            }
        }

        /// <summary>
        /// 设置单元格的内容
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        public static void SetCellValue(XSSFSheet sheet, int rowIndex, int cellIndex, string value, ICellStyle cellStyle)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }
            row.CreateCell(cellIndex).SetCellValue(value);

            if (cellStyle != null)
            {
                row.GetCell(cellIndex).CellStyle = cellStyle;
            }
        }

        /// <summary>
        /// 单元格合并行、合并列
        /// </summary>
        public static void AddMergedRegion(XSSFSheet sheet, int firstRow, int lastRow, int firstCol, int lastCol)
        {
            CellRangeAddress region = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            sheet.AddMergedRegion(region);
        }

        /// <summary>
        /// 单元格合并行、合并列
        /// </summary>
        public static void AddMergedRegion(HSSFSheet sheet, int firstRow, int lastRow, int firstCol, int lastCol)
        {
            CellRangeAddress region = new CellRangeAddress(firstRow, lastRow, firstCol, lastCol);
            sheet.AddMergedRegion(region);
        }


        /// <summary>
        /// 设置行的高度
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        public static void SetRowHeight(ISheet sheet, int rowIndex, int height)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }

            row.Height = (short)height;

        }

        /// <summary>
        /// 设置行的高度
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        public static void SetRowHeight(ISheet sheet, int rowIndex, float height)
        {
            IRow row = sheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheet.CreateRow(rowIndex);
            }

            row.HeightInPoints = height;

        }

        #endregion 设置单元格的内容

        #region 获取单元格的内容和样式

        public static ICellStyle GetCellStyle(HSSFWorkbook hssfworkbook, IFont font)
        {
            ICellStyle cellstyle = hssfworkbook.CreateCellStyle();
            if (font != null)
            {
                cellstyle.SetFont(font);
            }
            return cellstyle;
        }

        /// <summary>
        /// 设置单元格的样式
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="thisRow"></param>
        /// <param name="p"></param>
        /// <param name="cellStyle"></param>
        public static void GetCellStyle(HSSFSheet sheet, int rowIndex, int cellIndex, ICellStyle cellStyle)
        {
            sheet.GetRow(rowIndex).GetCell(cellIndex).CellStyle = cellStyle;
        }

        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <param name="fontfamily"></param>
        /// <param name="fontsize"></param>
        /// <param name="isBold"></param>
        /// <param name="isFontColorRed"></param>
        /// <returns></returns>
        public static IFont GetFontStyle(HSSFWorkbook hssfworkbook, string fontfamily, int fontsize, bool isBold = true, bool isFontColorRed = false)
        {
            IFont font1 = hssfworkbook.CreateFont();
            if (!string.IsNullOrEmpty(fontfamily))
            {
                font1.FontName = fontfamily;
            }
            else
            {
                font1.FontName = "Arial";
            }
            //font1.IsItalic = true;
            if (isBold)
            {
                font1.Boldweight = (short)FontBoldWeight.Bold;
            }
            font1.FontHeightInPoints = (short)fontsize;
            if (isFontColorRed)
            {
                font1.Color = GetXLColour(hssfworkbook, System.Drawing.Color.Red);
            }

            return font1;
        }

        public static short GetXLColour(HSSFWorkbook workbook, System.Drawing.Color SystemColour)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 64)
                    {
                        XlColour = XlPalette.AddColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    }
                    else
                    {
                        XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    }

                    s = XlColour.Indexed;
                }
            }
            else
                s = XlColour.Indexed;

            return s;
        }

        /// <summary>
        /// 获取单元格样式
        /// </summary>
        /// <param name="workbook">Excel操作类</param>
        /// <param name="font">单元格字体</param>
        /// <param name="fillForegroundColor">图案的颜色</param>
        /// <param name="fillPattern">图案样式</param>
        /// <param name="fillBackgroundColor">单元格背景</param>
        /// <param name="ha">垂直对齐方式</param>
        /// <param name="va">垂直对齐方式</param>
        /// <returns></returns>
        public static ICellStyle GetCellStyle(HSSFWorkbook workbook, IFont font, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, bool border = true, short BackgroundColor = -1)
        {
            ICellStyle cellstyle = workbook.CreateCellStyle();
            if (font != null)
            {
                cellstyle.SetFont(font);
            }

            cellstyle.Alignment = horizontalAlignment;
            cellstyle.VerticalAlignment = verticalAlignment;

            //有边框
            if (border)
            {
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            }
            if (BackgroundColor >= 0)
            {
                //cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                //cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thick;
                //cellstyle.FillPattern = FillPattern.AltBars;
                //cellstyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
            }

            cellstyle.WrapText = true;
            return cellstyle;
        }

        /// <summary>
        /// 获取单元格样式
        /// </summary>
        /// <param name="workbook">Excel操作类</param>
        /// <param name="font">单元格字体</param>
        /// <param name="fillForegroundColor">图案的颜色</param>
        /// <param name="fillPattern">图案样式</param>
        /// <param name="fillBackgroundColor">单元格背景</param>
        /// <param name="ha">垂直对齐方式</param>
        /// <param name="va">垂直对齐方式</param>
        /// <returns></returns>
        public static ICellStyle GetCellStyle(XSSFWorkbook workbook, IFont font, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, bool border = true, short BackgroundColor = -1)
        {
            ICellStyle cellstyle = workbook.CreateCellStyle();
            if (font != null)
            {
                cellstyle.SetFont(font);
            }

            cellstyle.Alignment = horizontalAlignment;
            cellstyle.VerticalAlignment = verticalAlignment;

            //有边框
            if (border)
            {
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            }
            if (BackgroundColor >= 0)
            {
                //cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                //cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thick;
                //cellstyle.FillPattern = FillPattern.AltBars;
                //cellstyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;
            }

            cellstyle.WrapText = true;
            return cellstyle;
        }

        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="hssfworkbook"></param>
        /// <param name="fontfamily"></param>
        /// <param name="fontsize"></param>
        /// <param name="isBold"></param>
        /// <param name="isFontColorRed"></param>
        /// <returns></returns>
        public static IFont GetFontStyle(XSSFWorkbook hssfworkbook, string fontfamily, int fontsize, bool isBold = true, bool isFontColorRed = false)
        {
            IFont font1 = hssfworkbook.CreateFont();
            if (!string.IsNullOrEmpty(fontfamily))
            {
                font1.FontName = fontfamily;
            }
            else
            {
                font1.FontName = "Arial";
            }
            //font1.IsItalic = true;
            if (isBold)
            {
                font1.Boldweight = (short)FontBoldWeight.Bold;
            }
            font1.FontHeightInPoints = (short)fontsize;
            if (isFontColorRed)
            {
                //font1.Color = GetXLColour(hssfworkbook, System.Drawing.Color.Red);
            }

            return font1;
        }

        #endregion 获取单元格的内容和样式

        #region 字符串处理的帮助方法

        /// <summary>
        /// 根据Html的长度和每一行的字符数，获取Excel的行数
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <param name="LineByteCount">每行的字符数</param>
        /// <returns></returns>
        public static int GetExcelRowCount(string htmlstring, int ByteCount)
        {
            int rowIndex = 0;
            if (htmlstring.Contains("\n"))
            {
                string[] arr = htmlstring.Split(new string[] { "\n" }, StringSplitOptions.None);
                foreach (var item in arr)
                {
                    int thisRowIndex = 1;
                    int byteCount = System.Text.Encoding.Default.GetByteCount(item);
                    if (byteCount > ByteCount)
                    {
                        thisRowIndex = (int)Math.Ceiling(byteCount / (decimal)ByteCount);
                    }
                    rowIndex += thisRowIndex;
                }
            }
            return rowIndex;
        }

        /// <summary>
        /// 设置Html标记转换为Excel的格式（只支持加粗）
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <param name="boldFont"></param>
        /// <returns></returns>
        public static IRichTextString SetHtmlToRichTextString(string htmlstring, IFont boldFont)
        {
            string str = htmlstring;
            List<string> boldFontList = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] arr = str.Split(new string[] { "<strong>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in arr)
                {
                    if (item.Contains("</strong>"))
                    {
                        string[] arr2 = item.Split(new string[] { "</strong>" }, StringSplitOptions.RemoveEmptyEntries);
                        boldFontList.Add(RemoveHtml(arr2[0]));
                    }
                }
            }
            str = RemoveHtml(htmlstring);
            IRichTextString richTextString = new HSSFRichTextString(str);
            foreach (var item in boldFontList)
            {
                int startIndex = str.IndexOf(item);
                int endIndex = startIndex + item.Length;
                richTextString.ApplyFont(startIndex, endIndex, boldFont);//设置加粗文字
            }
            return richTextString;
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <returns>已经去除后的文字</returns>
        public static string RemoveHtml(string htmlstring)
        {
            //删除脚本
            htmlstring =
                Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>",
                              "", RegexOptions.IgnoreCase);
            //删除HTML
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            //htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(ldquo);", "“", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(rdquo);", "”", RegexOptions.IgnoreCase);

            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            //htmlstring = htmlstring.Replace("\r\n", "");
            return htmlstring;
        }

        #endregion 字符串处理的帮助方法

        public static string SaveFile(string outRelativePath, string outRealDir, IWorkbook workbook, string fileName)
        {
            string excelPath = outRealDir + "/" + fileName + ".xls";

            //生成excel文件
            FileStream swQuoteSheet = File.OpenWrite(excelPath);
            workbook.Write(swQuoteSheet);
            swQuoteSheet.Close();
            return outRelativePath + "/" + fileName + ".xls";
        }

        public static int LoadImage(string path, IWorkbook wb)
        {
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);

                return wb.AddPicture(buffer, NPOI.SS.UserModel.PictureType.JPEG);
            }
        }

        /// <summary>
        /// 字符串必须大写
        /// </summary>
        /// <param name="c"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        public static int GetNumByChar(char c, char first = '0')
        {
            if (first == '0')
                return c - 65;
            else
                return (first - 64) * 26 + GetNumByChar(c);
        }

        public static string GetImage(string Image)
        {
            return ConstsMethod.ReplaceURLToLocalPath(Image);
        }

        /// <summary>
        /// 生成压缩文件，并下载。
        /// </summary>
        /// <param name="path"></param>
        public static string Zip(string path)
        {
            if (!File.Exists(path + ".zip"))
            {
                Zip(path, path + ".zip");
            }
            return path + ".zip";
        }

        /// <summary>
        /// 生成压缩文件，并下载。
        /// </summary>
        /// <param name="path"></param>
        public static string Zip(string path, string newPath)
        {
            try
            {
                //if (File.Exists(newPath))
                //{
                //    File.Delete(newPath);//TODO 暂时注释
                //}
                System.IO.Compression.ZipFile.CreateFromDirectory(path, newPath);//创建压缩文件
                //CommonCode.DownLoadFile(newPath, fileName + ".zip");//下载压缩文件

                return newPath;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return "";
        }

        public static void GetJPGPath(List<string> filesAsolutePathList, List<string> filesPhysicalPathList, string filePath)
        {
            string jpgPath = filePath.Replace("\\PDFAndExcel", "\\jpg").Replace(".xls", ".jpg");

            filesAsolutePathList.Add(jpgPath);
            filesPhysicalPathList.Add(ConstsMethod.ReplaceURLToLocalPath(jpgPath));
        }

        public static void GetJPGPath(List<string> filesAsolutePathList, List<string> filesPhysicalPathList, List<string> filePaths)
        {
            for (var i = 0; i < filePaths.Count; i++)
            {
                var filePath = filePaths[i];

                GetJPGPath(filesAsolutePathList, filesPhysicalPathList, filePath);
            }
        }

        /// <summary>
        /// 插入一条新行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="insertRowIndex">指定在第几行指入（插入行的位置）</param>
        /// <param name="insertRowCount">指定要插入多少行</param>
        /// <param name="row">源单元格格式的行</param>
        public static void MyInsertRow(ISheet sheet, int insertRowIndex, int insertRowCount, IRow row)
        {
            if (row == null)
            {
                row = sheet.CreateRow(insertRowIndex);
            }

            #region 批量移动行

            sheet.ShiftRows(
                insertRowIndex,                                 //--开始行
                sheet.LastRowNum,                      //--结束行
                insertRowCount,                             //--移动大小(行数)--往下移动
                true,                                  //是否复制行高
                false//,                               //是否重置行高
                     //true                                 //是否移动批注
            );

            #endregion 批量移动行

            #region 对批量移动后空出的空行插，创建相应的行，并以插入行的上一行为格式源(即：插入行-1的那一行)

            for (int i = insertRowIndex; i < insertRowIndex + insertRowCount - 1; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;

                targetRow = sheet.CreateRow(i + 1);

                for (int m = row.FirstCellNum; m < row.LastCellNum; m++)
                {
                    sourceCell = row.GetCell(m);
                    if (sourceCell == null)
                        continue;
                    targetCell = targetRow.CreateCell(m);

                    //targetCell..Encoding = sourceCell.Encoding;
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);
                }
                //CopyRow(sourceRow, targetRow);
                //Util.CopyRow(sheet, sourceRow, targetRow);
            }

            IRow firstTargetRow = sheet.GetRow(insertRowIndex);
            ICell firstSourceCell = null;
            ICell firstTargetCell = null;

            for (int m = row.FirstCellNum; m < row.LastCellNum; m++)
            {
                firstSourceCell = row.GetCell(m);
                if (firstSourceCell == null)
                    continue;
                firstTargetCell = firstTargetRow.CreateCell(m);

                //firstTargetCell.Encoding = firstSourceCell.Encoding;
                firstTargetCell.CellStyle = firstSourceCell.CellStyle;
                firstTargetCell.SetCellType(firstSourceCell.CellType);
            }

            #endregion 对批量移动后空出的空行插，创建相应的行，并以插入行的上一行为格式源(即：插入行-1的那一行)
        }

        /// <summary>
        /// 保存文件为PDF和Excel
        /// </summary>
        /// <param name="outFileList"></param>
        /// <param name="outRealDir"></param>
        /// <param name="workbook"></param>
        /// <param name="fileName"></param>
        public static string SaveFile_PDFAndExcel(List<string> outFileList, string outRealDir, IWorkbook workbook, string fileName, List<string> filesGenerated, string extension = "xls")
        {
            //if (!Directory.Exists(outRealDir + "\\jpg"))
            //{
            //    Directory.CreateDirectory(outRealDir + "\\jpg");
            //}
            if (!Directory.Exists(outRealDir + "\\PDFAndExcel"))
            {
                Directory.CreateDirectory(outRealDir + "\\PDFAndExcel");
            }

            string excelPath = outRealDir + "\\PDFAndExcel\\" + fileName + "." + extension;
            string pdfPath = outRealDir + "\\PDFAndExcel\\" + fileName + ".pdf";
            //string jpgPath = outRealDir + "\\jpg\\" + fileName + ".jpg";
            //string jpgPath = outRealDir + "\\jpg\\";

            //生成excel文件
            FileStream swQuoteSheet = File.OpenWrite(excelPath);
            workbook.Write(swQuoteSheet);
            swQuoteSheet.Close();
            outFileList.Add(excelPath);

            AsposeX asposeX = new AsposeX();
            string errMsg;

            //生成pdf文件
            asposeX.ExcelToPdf(excelPath, pdfPath, out errMsg);

            ////生成图片
            //asposeX.ExcelToJpg(pdfPath, jpgPath, fileName, out filesGenerated, out errMsg);

            filesGenerated.Add(pdfPath);

            return excelPath;
        }

        /// <summary>
        /// 保存文件为PDF和Excel
        /// </summary>
        /// <param name="outFileList"></param>
        /// <param name="outRealDir"></param>
        /// <param name="workbook"></param>
        /// <param name="fileName"></param>
        public static string SaveFile_PDFAndExcel(List<string> outFileList, string outRealDir, IWorkbook workbook, string fileName, string extension = "xls")
        {
            List<string> files = new List<string>();

            return SaveFile_PDFAndExcel(outFileList, outRealDir, workbook, fileName, files, extension);
        }

        /// <summary>
        /// 设置公章的图标
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        public static void CreatePicture_SaleContract(IWorkbook workbook, HSSFSheet sheet, int col1, int row1, int col2, int row2, int dx1 = 75, int dy1 = 0, int dx2 = 800, int dy2 = 0)
        {
            CreatePicture(workbook, sheet, "/images/SaleContract.png", dx1, dy1, dx2, dy2, col1, row1, col2, row2);
        }

        /// <summary>
        /// 设置公章的图标
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        public static void CreatePicture_SaleContract2(IWorkbook workbook, HSSFSheet sheet, int col1, int row1, int col2, int row2)
        {
            CreatePicture(workbook, sheet, "/images/JetSeal2.png", 75, 0, 800, 0, col1, row1, col2, row2);
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="path"></param>
        /// <param name="dx1"></param>
        /// <param name="dy1"></param>
        /// <param name="dx2"></param>
        /// <param name="dy2"></param>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        public static void CreatePicture(IWorkbook workbook, HSSFSheet sheet, string path, int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
        {
            #region 插入图片

            string imgPath = ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(path));
            if (File.Exists(imgPath))
            {
                using (Image img = Image.FromFile(imgPath))
                {
                    HSSFClientAnchor anchor = new HSSFClientAnchor(dx1, dy1, dx2, dy2, col1, row1, col2, row2);
                    //插入图片
                    IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                    int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                    HSSFPicture picture = (HSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }
            }

            #endregion 插入图片
        }

        public static void CreatePicture(IWorkbook workbook, ISheet sheet, string path, int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2)
        {
            #region 插入图片

            string imgPath = ConstsMethod.ReplaceURLToLocalPath(Utils.GetMapPath(path));
            if (File.Exists(imgPath))
            {
                using (Image img = Image.FromFile(imgPath))
                {
                    XSSFClientAnchor anchor = new XSSFClientAnchor(dx1, dy1, dx2, dy2, col1, row1, col2, row2);
                    //插入图片
                    IDrawing patriarch1 = sheet.CreateDrawingPatriarch();
                    int imageId = ExcelHelper.LoadImage(imgPath, workbook);

                    XSSFPicture picture = (XSSFPicture)patriarch1.CreatePicture(anchor, imageId);
                    picture.LineStyle = LineStyle.Solid;
                }
            }

            #endregion 插入图片
        }

        /// <summary>
        /// 插入工厂英文名和英文地址
        /// </summary>
        /// <param name="thisRow"></param>
        /// <param name="list_OrderProduct"></param>
        /// <param name="sheet"></param>
        /// <param name="cellStyle_boldFont"></param>
        /// <returns></returns>
        public static int Insert_FactoryEnglishName_Address(HSSFSheet sheet, List<VMOrderProduct> list_OrderProduct, int thisRow, ICellStyle cellStyle_boldFont, char c = 'A')
        {
            #region 设置工厂英文名和英文地址

            List<VMDTOFactory> list_Factory = new List<VMDTOFactory>();

            foreach (var item in list_OrderProduct)
            {
                VMDTOFactory vm_Factory = new VMDTOFactory();
                vm_Factory.ID = item.FactoryID ?? 0;
                vm_Factory.EnglishName = item.Factory_EnglishName;
                vm_Factory.EnglishAddress = item.Factory_EnglishAddress;

                if (list_Factory.Where(d => d.ID == item.FactoryID).Count() == 0)
                {
                    list_Factory.Add(vm_Factory);
                }
            }

            foreach (var item in list_Factory)
            {
                ++thisRow;
                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), item.EnglishName, cellStyle_boldFont);

                ++thisRow;
                ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), item.EnglishAddress, cellStyle_boldFont);

                ++thisRow;

                break;
            }

            #endregion 设置工厂英文名和英文地址

            return thisRow;
        }

        /// <summary>
        /// 插入工厂英文名和英文地址——清关
        /// </summary>
        /// <param name="thisRow"></param>
        /// <param name="list_OrderProduct"></param>
        /// <param name="sheet"></param>
        /// <param name="cellStyle_boldFont"></param>
        /// <returns></returns>
        public static int Insert_FactoryEnglishName_Address_Clearance(HSSFSheet sheet, List<VMOrderProduct> list_OrderProduct, int thisRow, ICellStyle cellStyle_boldFont, char c = 'A', bool IsShowSkuNumber = false)
        {
            #region 设置工厂英文名和英文地址

            List<VMDTOFactory> list_Factory = new List<VMDTOFactory>();

            foreach (var item in list_OrderProduct)
            {
                VMDTOFactory vm_Factory = new VMDTOFactory();
                vm_Factory.ID = item.InspectionClearance_FactoryID ?? 0;
                vm_Factory.EnglishName = item.InspectionClearance_Factory_EnglishName;
                vm_Factory.EnglishAddress = item.InspectionClearance_Factory_EnglishAddress;

                var query = list_Factory.Where(d => d.ID == item.InspectionClearance_FactoryID);
                if (query.Count() == 0)
                {
                    if (IsShowSkuNumber)
                    {
                        List<string> list_SkuNumber = new List<string>();
                        list_SkuNumber.Add(item.SkuNumber);

                        vm_Factory.list_SkuNumber = list_SkuNumber;
                    }

                    list_Factory.Add(vm_Factory);
                }
                else
                {
                    if (IsShowSkuNumber)
                    {
                        List<string> list_SkuNumber = query.First().list_SkuNumber;
                        if (!list_SkuNumber.Contains(item.SkuNumber) && !string.IsNullOrEmpty(item.SkuNumber))
                        {
                            list_SkuNumber.Add(item.SkuNumber);
                            query.First().list_SkuNumber = list_SkuNumber;
                        }
                    }
                }
            }

            foreach (var item in list_Factory)
            {
                if (!string.IsNullOrEmpty(item.EnglishName))
                {
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), item.EnglishName);
                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), item.EnglishAddress);
                    ++thisRow;

                    if (IsShowSkuNumber && list_Factory.Count > 1)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        if (item.list_SkuNumber != null && item.list_SkuNumber.Count > 0)
                        {
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), "(" + CommonCode.ListToString(item.list_SkuNumber, "/") + ")");
                        }
                        ++thisRow;
                    }
                    ++thisRow;
                }
            }

            #endregion 设置工厂英文名和英文地址

            return thisRow;
        }

        /// <summary>
        /// 插入工厂英文名和英文地址——结汇
        /// </summary>
        /// <param name="thisRow"></param>
        /// <param name="list_OrderProduct"></param>
        /// <param name="sheet"></param>
        /// <param name="cellStyle_boldFont"></param>
        /// <returns></returns>
        public static int Insert_FactoryEnglishName_Address_Exchange(HSSFSheet sheet, List<VMOrderProduct> list_OrderProduct, int thisRow, ICellStyle cellStyle_boldFont, char c = 'A', bool IsShowSkuNumber = false)
        {
            #region 设置工厂英文名和英文地址

            List<VMDTOFactory> list_Factory = new List<VMDTOFactory>();

            foreach (var item in list_OrderProduct)
            {
                VMDTOFactory vm_Factory = new VMDTOFactory();
                vm_Factory.ID = item.InspectionExchange_FactoryID ?? 0;
                vm_Factory.EnglishName = item.InspectionExchange_Factory_EnglishName;
                vm_Factory.EnglishAddress = item.InspectionExchange_Factory_EnglishAddress;

                var query = list_Factory.Where(d => d.ID == item.InspectionExchange_FactoryID);
                if (query.Count() == 0)
                {
                    if (IsShowSkuNumber)
                    {
                        List<string> list_SkuNumber = new List<string>();
                        list_SkuNumber.Add(item.SkuNumber);

                        vm_Factory.list_SkuNumber = list_SkuNumber;
                    }

                    list_Factory.Add(vm_Factory);
                }
                else
                {
                    if (IsShowSkuNumber)
                    {
                        List<string> list_SkuNumber = query.First().list_SkuNumber;
                        if (!list_SkuNumber.Contains(item.SkuNumber) && !string.IsNullOrEmpty(item.SkuNumber))
                        {
                            list_SkuNumber.Add(item.SkuNumber);
                            query.First().list_SkuNumber = list_SkuNumber;
                        }
                    }
                }
            }

            foreach (var item in list_Factory)
            {
                if (!string.IsNullOrEmpty(item.EnglishName))
                {
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), item.EnglishName);
                    ++thisRow;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), item.EnglishAddress);
                    ++thisRow;

                    if (IsShowSkuNumber && list_Factory.Count > 1)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                        if (item.list_SkuNumber.Count > 0)
                        {
                            ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar(c), "(" + CommonCode.ListToString(item.list_SkuNumber, "/") + ")");
                        }
                        ++thisRow;
                    }

                    ++thisRow;
                }
            }

            #endregion 设置工厂英文名和英文地址

            return thisRow;
        }

        /// <summary>
        /// 获取产品的产品成分列表。格式如：70% GLASS
        /// </summary>
        /// <param name="list_ProductIngredient"></param>
        /// <returns></returns>
        public static List<string> GetListIngredient(List<VMProductIngredients> list_ProductIngredient)
        {
            List<string> list_Ingredient = new List<string>();
            if (list_ProductIngredient != null)
            {
                List<VMProductIngredients> temp = list_ProductIngredient.OrderByDescending(d => d.IngredientPercent).ToList();
                for (int i = 0; i < temp.Count(); i++)
                {
                    var item4 = temp[i];

                    string Percent = "";
                    if (item4.IngredientPercent > 0)
                    {
                        Percent = item4.IngredientPercent + "% ";
                    }

                    string IngredientName = item4.IngredientName;
                    if (!string.IsNullOrEmpty(item4.IngredientName))
                    {
                        IngredientName = item4.IngredientName.Trim();
                    }
                    list_Ingredient.Add(Percent + IngredientName);
                }
            }
            return list_Ingredient;
        }

        /// <summary>
        /// 获取产品的产品成分列表。格式如：70% GLASS，30% AAA
        /// </summary>
        /// <param name="list_ProductIngredient"></param>
        /// <returns></returns>
        public static string GetStrIngredient(List<VMProductIngredients> list_ProductIngredient)
        {
            return CommonCode.ListToString(ExcelHelper.GetListIngredient(list_ProductIngredient));
        }
    }
}