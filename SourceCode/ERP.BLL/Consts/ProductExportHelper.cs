using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ERP.BLL.Consts
{
    /// <summary>
    /// 导出产品资料
    /// </summary>
    public class ProductExportHelper
    {
        private static void WriteToFile(HSSFWorkbook workbook, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                workbook.Write(fs);
                //fs.Close();
            }
        }

        private static HSSFWorkbook InitializeWorkbook()
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "Gitzhome Company";
            workbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Jet系统";
            si.Title = "数据列表导出";
            si.CreateDateTime = System.DateTime.Now;
            workbook.SummaryInformation = si;
            return workbook;
        }

        private static IDictionary<String, ICellStyle> CreateCellStyle(HSSFWorkbook workbook)
        {
            IDictionary<String, ICellStyle> dictStyles = new Dictionary<String, ICellStyle>();
            IDataFormat df = workbook.CreateDataFormat();
            //自定义颜色
            HSSFPalette palette = workbook.GetCustomPalette();
            palette.SetColorAtIndex(HSSFColor.Pink.Index, 242, 240, 242); //统计行背景色
            palette.SetColorAtIndex(HSSFColor.BlueGrey.Index, 220, 230, 241); //标题行背景色

            //标题行样式
            ICellStyle style;
            IFont titleFont = workbook.CreateFont();
            titleFont.FontHeightInPoints = 18;
            titleFont.Boldweight = 700;
            titleFont.Color = IndexedColors.DarkBlue.Index;
            style = CreateBorderedStyle(workbook);
            style.Alignment = HorizontalAlignment.Center;
            style.SetFont(titleFont);
            dictStyles.Add("title", style);

            //列头行样式
            style = CreateBorderedStyle(workbook);
            style.Alignment = HorizontalAlignment.Center;
            style.FillForegroundColor = HSSFColor.BlueGrey.Index;
            style.FillPattern = FillPattern.SolidForeground;
            HSSFFont headerFont = (HSSFFont)workbook.CreateFont();
            headerFont.FontHeightInPoints = 11;
            headerFont.Boldweight = 700;
            style.SetFont(headerFont);
            dictStyles.Add("header", style);

            //数据行样式
            style = CreateBorderedStyle(workbook);
            style.WrapText = true;//文字自动换行
            HSSFFont cellFont = (HSSFFont)workbook.CreateFont();
            cellFont.FontHeightInPoints = 10;
            style.SetFont(cellFont);
            dictStyles.Add("cell", style);

            return dictStyles;
        }

        private static ICellStyle CreateBorderedStyle(HSSFWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            Int16 borderColor = IndexedColors.Grey50Percent.Index;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BottomBorderColor = borderColor;
            style.LeftBorderColor = borderColor;
            style.RightBorderColor = borderColor;
            style.TopBorderColor = borderColor;
            style.VerticalAlignment = VerticalAlignment.Center;
            return style;
        }

        private static int CalcTextLength(string text)
        {
            return Encoding.UTF8.GetBytes(text).Length;
        }

        private static void PageSetting(ISheet inSheet)
        {
            //turn off gridlines
            inSheet.DisplayGridlines = (false);
            inSheet.IsPrintGridlines = (false);
            inSheet.FitToPage = (true);
            inSheet.HorizontallyCenter = (true);
            IPrintSetup printSetup = inSheet.PrintSetup;
            printSetup.Landscape = (true);
            //the following three statements are required only for HSSF
            inSheet.Autobreaks = (true);
            printSetup.FitHeight = 1;
            printSetup.FitWidth = 1;
        }

        public static string ExportDataToExcel(DataTable dt, string sSheetTitle)
        {
            try
            {
                HSSFWorkbook workbook = InitializeWorkbook();
                string fileTitle = sSheetTitle;
                //生成.xls文件完整路径名
                string sFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls";
                string sFilePath = HttpContext.Current.Server.MapPath("~/" + AdminConsts.PRODUCT_EXPORT_PATH);
                if (!System.IO.Directory.Exists(sFilePath))
                {
                    System.IO.Directory.CreateDirectory(sFilePath);
                }
                sFilePath += sFileName;

                IDictionary<String, ICellStyle> dictStyles = CreateCellStyle(workbook);
                ISheet sheet;
                //sSheetTitle=""表示不设置标题行
                if (String.IsNullOrEmpty(sSheetTitle))
                {
                    sheet = workbook.CreateSheet();
                }
                else
                {
                    sheet = workbook.CreateSheet(sSheetTitle);
                }

                //重新生成DataTable
                DataTable dtNew = dt.DefaultView.ToTable("", false, new string[] { });
                //累计当前行
                int rowIndex = 0;
                int columnCount = dtNew.Columns.Count - 1;//作为索引界限，所以减一
                //取得列宽
                int[] arrColWidth = new int[columnCount + 1];//

                //页面设定
                PageSetting(sheet);

                HSSFSheet realSheet = (HSSFSheet)sheet;
                realSheet.AlternativeFormula = true;
                realSheet.AlternativeExpression = true;
                realSheet.ForceFormulaRecalculation = true;

                //the title row: centered text in 18pt font
                //title <> "":表示不设置Excel表格表名行
                if (!string.IsNullOrEmpty(sSheetTitle))
                {
                    IRow titleRow = sheet.CreateRow(rowIndex);
                    titleRow.HeightInPoints = 40;
                    ICell titleCell = titleRow.CreateCell(0);
                    titleCell.SetCellValue(sSheetTitle);
                    titleCell.CellStyle = dictStyles["title"];//
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, columnCount));

                    rowIndex++;
                }

                //the header row: centered text in 18pt font
                IRow headerRow = sheet.CreateRow(rowIndex);
                headerRow.HeightInPoints = 21;
                ICellStyle headerStyle = dictStyles["header"];

                foreach (DataColumn column in dtNew.Columns)
                {
                    ICell headCell = headerRow.CreateCell(column.Ordinal);
                    headCell.SetCellValue(column.ColumnName);
                    headCell.CellStyle = headerStyle;
                    //设置列宽
                    arrColWidth[headCell.ColumnIndex] = CalcTextLength(column.ColumnName);
                }

                //Data
                rowIndex++;
                foreach (DataRow row in dtNew.Rows)
                {
                    IRow newRow = sheet.CreateRow(rowIndex);
                    newRow.HeightInPoints = 18;

                    for (int i = 0; i <= columnCount; i++)
                    {
                        string cellVal = row[i].ToString();
                        Regex newLinePattern = new Regex("<br>|<br/>");
                        cellVal = newLinePattern.Replace(cellVal, "\r\n").Replace("</br>", "").Replace(" 0:00:00", "");

                        ICell newCell = newRow.CreateCell(i);
                        newCell.SetCellValue(cellVal);
                        newCell.CellStyle = dictStyles["cell"];
                        //列宽 取最大值
                        int colWidth = CalcTextLength(cellVal);
                        if (colWidth > arrColWidth[i])
                        {
                            arrColWidth[i] = colWidth;
                        }
                    }
                    rowIndex++;
                }

                //设置列宽
                for (int i = 0; i <= columnCount; i++)
                {
                    if (arrColWidth[i] > 70)
                    {
                        arrColWidth[i] = 70;
                    }
                    sheet.SetColumnWidth(i, arrColWidth[i] * 256);
                }

                WriteToFile(workbook, sFilePath);
                return HttpContext.Current.Request.ApplicationPath + AdminConsts.PRODUCT_EXPORT_PATH + sFileName;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}