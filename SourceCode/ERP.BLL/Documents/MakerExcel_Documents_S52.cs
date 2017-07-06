using ERP.BLL.ERP.Consts;
using ERP.Models.CustomEnums;
using ERP.Models.InspectionClearance;
using ERP.Models.InspectionCustoms;
using ERP.Models.InspectionExchange;
using ERP.Models.InspectionReceipt;
using ERP.Tools;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Documents
{
    class MakerExcel_Documents_S52
    {
        /// <summary>
        /// S52的报检
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionReceipt_S52<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            DTOInspectionReceipt vm = model as DTOInspectionReceipt;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);//加粗，罗马字体
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);//不加粗，罗马字体
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);//加粗，Arial
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 14);//加粗，Arial
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体          
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，不加粗，罗马字体

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，加粗，罗马字体
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，加粗，罗马字体

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行
            cellStyle5.WrapText = false;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行,靠左
            cellStyle6.WrapText = false;

            ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,换行,靠左


            //无边框，加粗，Arial字体，不换行，靠左 11
            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle7.WrapText = false;
            //无边框，加粗，Arial字体，换行，靠左 11
            ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            //无边框，加粗，Arial字体，不换行，靠左 14
            ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle9.WrapText = false;
            //有边框，居中，加粗,Arial字体
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);

            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;

            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);//加粗罗马字体，斜体

            int thisRow = 0;

            switch (makerTypeEnum)
            {
                case MakerTypeEnum.InspectionReceipt_Home:

                    #region 报检——报检首页

                    ExcelHelper.SetCellValue(sheet, 0, ExcelHelper.GetNumByChar('A'), "To:" + vm.FactoryName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 1, ExcelHelper.GetNumByChar('A'), "ATTN:" + vm.FactoryContact, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 2, ExcelHelper.GetNumByChar('A'), "关于S52客人" + vm.PurchaseNumber + vm.HsName + "定单PO#:", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 3, ExcelHelper.GetNumByChar('A'), vm.POID + "的报植检资料如下：", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 5, ExcelHelper.GetNumByChar('B'), vm.HsName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('B'), vm.HSCodeName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('B'), vm.InvNo, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('B'), vm.ProductsQuauity, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('B'), vm.ProductsBoxNum, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), "USD" + vm.ProductAmount, cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('A'), "由", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), vm.PortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('C'), "运到", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('D'), "美国", cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "请于" + vm.ClaimFaxDate + "办理" + vm.INTypeName + "并换通关单", cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('D'), "制单人：" + vm.CreateUserName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 16, ExcelHelper.GetNumByChar('D'), "日期：" + CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    #endregion 报检——报检首页                    #endregion 报检——报检首页

                    break;

                case MakerTypeEnum.InspectionReceipt_PackingList:

                    #region 报检——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), vm.InvNo, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), vm.POID, cellStyle10);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('I'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle6);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.INPortName + " ,CHINA", cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), "" + vm.INEndPortName, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 15, ExcelHelper.GetNumByChar('B'), "   FOB " + vm.INPortName + " ,CHINA", cellStyle5);




                    int? SelectQty = 0;
                    int? SelectBoxQty = 0;
                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;

                    thisRow = 19;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SelectBoxQty += item.SelectBoxQty;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SelectBoxQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterWeightGross, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.SumOuterWeightNet, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.SumOuterVolume, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SelectBoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), SumOuterVolume, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('H'), thisRow, ExcelHelper.GetNumByChar('I'), thisRow + 6);

                    #endregion 报检——装箱单

                    break;

                case MakerTypeEnum.InspectionReceipt_CommercialInvoice:

                    #region 报检——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('H'), vm.InvNo, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('H'), vm.POID, cellStyle10);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('H'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle6);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), "" + vm.INPortName + " ,CHINA", cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.INEndPortName, cellStyle6);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('B'), "   FOB " + vm.INPortName + " ,CHINA", cellStyle5);

                    thisRow = 18;

                    SelectQty = 0;
                    decimal SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + item.ProductPrice, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + item.SumPrice, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + SumPrice, cellStyle2);

                    thisRow += 6;
                    ExcelHelper.CreatePicture_SaleContract(workbook, sheet, ExcelHelper.GetNumByChar('H'), thisRow, ExcelHelper.GetNumByChar('I'), thisRow + 6);

                    #endregion 报检——发票

                    break;

                case MakerTypeEnum.InspectionReceipt_SaleContract:

                    #region 报检——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('E'), "S/C NO.:" + vm.InvNo, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('E'), "PO No.:" + vm.POID, cellStyle11);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('E'), "DATE:" + CommonCode.GetDateTime3(vm.CustomerDate), cellStyle7);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('A'), "Loading port: " + vm.INPortName + " ,CHINA", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('A'), "Destination: " + vm.INEndPortName, cellStyle7);

                    thisRow = 20;

                    SelectQty = 0;
                    SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.SelectQty, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.ProductPrice, cellStyle8);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + item.SumPrice, cellStyle8);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SelectQty, cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle8);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + SumPrice, cellStyle8);

                    thisRow += 3;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1. SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle7);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERM OF THE PAYMENT : " + vm.TeamOfThePayment, cellStyle7);

                    thisRow += 2;
                    for (int i = 0; i < vm.list_ShipmentOrderProduct.Count + 3; i++)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    }
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle9);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle9);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('B'), thisRow + 1, ExcelHelper.GetNumByChar('C'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S52/KIM.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 5);

                    #endregion 报检——销售合同

                    break;

                case MakerTypeEnum.InspectionReceipt_Commission:
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// S52的报关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionCustoms_S52<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
            VMInspectionCustoms vm = model as VMInspectionCustoms;

            var boldFont = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);//加粗，罗马字体
            var boldFont2 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12, false);//不加粗，罗马字体
            var boldFont3 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 11);//加粗，Arial
            var boldFont4 = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Arial", 14);//加粗，Arial
            ICellStyle cellStyle = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体          
            ICellStyle cellStyle3 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，不加粗，罗马字体

            ICellStyle cellStyle2 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, true);//有边框，加粗，罗马字体
            ICellStyle cellStyle4 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，加粗，罗马字体

            ICellStyle cellStyle5 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行
            cellStyle5.WrapText = false;

            ICellStyle cellStyle10 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，加粗，罗马字体.不换行
            cellStyle10.WrapText = false;

            ICellStyle cellStyle13 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，加粗，罗马字体.不换行,靠左
            cellStyle13.WrapText = false;

            ICellStyle cellStyle6 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Left, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体,不换行,靠左
            cellStyle6.WrapText = false;

            //无边框，加粗，Arial字体，不换行，靠左  11号
            ICellStyle cellStyle7 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle7.WrapText = false;
            //无边框，加粗，Arial字体，换行，靠左  11号
            ICellStyle cellStyle14 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, false);

            //有边框，加粗，Arial字体，不换行，靠左  11号
            ICellStyle cellStyle12 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Left, VerticalAlignment.Center, true);
            cellStyle12.WrapText = false;
            //无边框，加粗，Arial字体，不换行，靠左  14号
            ICellStyle cellStyle9 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont4, HorizontalAlignment.Left, VerticalAlignment.Center, false);
            cellStyle9.WrapText = false;
            //有边框，居中，加粗,Arial字体
            ICellStyle cellStyle8 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont3, HorizontalAlignment.Center, VerticalAlignment.Center, true);
            ICellStyle cellStyle11 = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont2, HorizontalAlignment.Center, VerticalAlignment.Center, false);//无边框，不加粗，罗马字体，靠左，不换行          
            IFont boldFont_IsItalic = ExcelHelper.GetFontStyle((HSSFWorkbook)workbook, "Times New Roman", 12);
            boldFont_IsItalic.IsItalic = true;

            ICellStyle cellStyle_boldFont_IsItalic = ExcelHelper.GetCellStyle((HSSFWorkbook)workbook, boldFont_IsItalic);//加粗罗马字体，斜体
            int thisRow = 0;

            switch (makerTypeEnum)
            {
                #region 报关

                case MakerTypeEnum.InspectionCustoms_PackingList:

                    #region 报关——装箱单

                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('J'), vm.InvoiceNO, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('J'), vm.POID, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('J'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), "" + vm.PortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.DestinationPortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 14, ExcelHelper.GetNumByChar('B'), " FOB " + vm.PortName + " ,CHINA", cellStyle5);

                    int? SelectQty = 0;
                    int? SelectBoxQty = 0;
                    decimal? SumOuterWeightGross = 0;
                    decimal? SumOuterWeightNet = 0;
                    decimal? SumOuterVolume = 0;

                    thisRow = 18;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SelectBoxQty += item.SelectBoxQty;
                        SumOuterWeightGross += item.SumOuterWeightGross;
                        SumOuterWeightNet += item.SumOuterWeightNet;
                        SumOuterVolume += item.SumOuterVolume;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), item.SelectBoxQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), item.SumOuterWeightGross, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), item.SumOuterWeightNet, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), item.SumOuterVolume, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('C'), ExcelHelper.GetNumByChar('E'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), SelectBoxQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), SumOuterWeightGross, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), SumOuterWeightNet, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('J'), SumOuterVolume, cellStyle2);

                    thisRow = thisRow + 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "境内货源地：" + vm.TerritoryOfOriginOfGoods, cellStyle6);

                    #endregion 报关——装箱单

                    break;

                case MakerTypeEnum.InspectionCustoms_CommercialInvoice:

                    #region 报关——发票

                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 6, ExcelHelper.GetNumByChar('I'), vm.InvoiceNO, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 7, ExcelHelper.GetNumByChar('I'), vm.POID, cellStyle11);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('I'), CommonCode.GetDateTime3(vm.CreateDate), cellStyle5);

                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('B'), "" + vm.PortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 11, ExcelHelper.GetNumByChar('B'), "" + vm.DestinationPortName, cellStyle5);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), " FOB " + vm.PortName + " ,CHINA", cellStyle5);

                    thisRow = 17;

                    SelectQty = 0;
                    decimal SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.SelectQty, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.HsEngName, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), Keys.USD_Sign + item.ProductPrice, cellStyle3);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + item.SumPrice, cellStyle3);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                    ExcelHelper.AddMergedRegion(sheet, thisRow, thisRow, ExcelHelper.GetNumByChar('D'), ExcelHelper.GetNumByChar('G'));

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "TOTAL:", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), SelectQty, cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('G'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('H'), "", cellStyle2);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('I'), Keys.USD_Sign + SumPrice, cellStyle2);

                    thisRow += 2;
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), " 境内货源地：" + vm.TerritoryOfOriginOfGoods, cellStyle13);

                    #endregion 报关——发票

                    break;

                case MakerTypeEnum.InspectionCustoms_SaleContract:

                    #region 报关——销售合同

                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('A'), "TO:" + vm.CustomerName, cellStyle_boldFont_IsItalic);
                    ExcelHelper.SetCellValue(sheet, 8, ExcelHelper.GetNumByChar('F'), vm.InvoiceNO, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('A'), vm.CustomerStreet, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 9, ExcelHelper.GetNumByChar('F'), vm.POID, cellStyle14);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('A'), vm.CustomerReg, cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 10, ExcelHelper.GetNumByChar('F'), CommonCode.GetDateTime3(vm.CustomerDate), cellStyle7);

                    ExcelHelper.SetCellValue(sheet, 12, ExcelHelper.GetNumByChar('B'), "" + vm.PortName + " ,CHINA", cellStyle7);
                    ExcelHelper.SetCellValue(sheet, 13, ExcelHelper.GetNumByChar('B'), "" + vm.DestinationPortName, cellStyle7);

                    thisRow = 21;

                    SelectQty = 0;
                    SumPrice = 0;

                    foreach (var item in vm.list_ShipmentOrderProduct)
                    {
                        SelectQty += item.SelectQty;
                        SumPrice += item.SumPrice;

                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));

                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), item.No, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), item.SkuNumber, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), item.HsEngName, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), item.SelectQty, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), Keys.USD_Sign + item.ProductPrice, cellStyle12);
                        ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + item.SumPrice, cellStyle12);

                        ++thisRow;
                    }

                    ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('B'), "", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('C'), "TOTAL:", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), SelectQty, cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('E'), "", cellStyle12);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('F'), Keys.USD_Sign + SumPrice, cellStyle12);

                    thisRow += 3;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "1. SHIPMENT DATE:" + vm.ShippingDateStart, cellStyle7);

                    thisRow += 2;

                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "3. TERM OF THE PAYMENT : " + vm.TeamOfThePayment, cellStyle7);

                    thisRow += 2;
                    for (int i = 0; i < vm.list_ShipmentOrderProduct.Count + 3; i++)
                    {
                        ExcelHelper.MyInsertRow(sheet, thisRow, 1, sheet.GetRow(thisRow));
                    }
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('A'), "THE  SELLERS :", cellStyle9);
                    ExcelHelper.SetCellValue(sheet, thisRow, ExcelHelper.GetNumByChar('D'), "THE BUYERS :", cellStyle9);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S188/tom.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('B'), thisRow + 1, ExcelHelper.GetNumByChar('C'), thisRow + 4);

                    ExcelHelper.CreatePicture(workbook, sheet, "/data/Documents/S52/KIM.png", 75, 0, 800, 0, ExcelHelper.GetNumByChar('F'), thisRow, ExcelHelper.GetNumByChar('F'), thisRow + 5);

                    #endregion 报关——销售合同

                    break;

                #endregion 报关

                default:
                    break;
            }
        }


        /// <summary>
        /// S52的清关
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionClearance_S52<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {

        }


        /// <summary>
        /// S52的结汇
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="makerTypeEnum"></param>
        public static void Bind_InspectionExchange_S52<T>(T model, IWorkbook workbook, HSSFSheet sheet, MakerTypeEnum makerTypeEnum)
        {
        }
    }
}
