using ERP.BLL.Documents;
using ERP.BLL.ERP.Documents;
using ERP.Models.CustomEnums;
using ERP.Tools;
using ERP.Tools.Logs;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace ERP.BLL.ERP.Consts
{
    /// <summary>
    /// 单证的帮助类
    /// </summary>
    public class MakerExcel_Documents
    {
        public static string Maker<T>(T model, MakerTypeEnum MakerTypeEnum, string SelectCustomer, string id, string customIndex = null)
        {
            List<string> filesGenerated = new List<string>();
            return Maker(model, MakerTypeEnum, SelectCustomer, id, filesGenerated, customIndex);
        }

        /// <summary>
        /// 根据excel模板生成excel
        /// </summary>
        /// <returns></returns>
        public static string Maker<T>(T model, MakerTypeEnum MakerTypeEnum, string SelectCustomer, string id, List<string> filesGenerated, string customIndex = null)
        {
            if (model == null)
            {
                return null;
            }
            List<string> outFileList = new List<string>();
            try
            {
                string templatePath = Utils.GetMapPath("~/data/Documents/" + SelectCustomer + "/" + MakerTypeEnum.ToString() + ".xls");

                if (SelectCustomer == SelectCustomerEnum.S05.ToString())
                {
                    switch (MakerTypeEnum)
                    {
                        case MakerTypeEnum.InspectionClearance_CreditNumber1:
                            templatePath = Utils.GetMapPath("~/data/Documents/" + SelectCustomer + "/Beneficiaries-ZL15097E.xls");
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber2:
                            templatePath = Utils.GetMapPath("~/data/Documents/" + SelectCustomer + "/CERTIFICATION-ZL15097E-8 point.xls");
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber3:
                            templatePath = Utils.GetMapPath("~/data/Documents/" + SelectCustomer + "/CERTIFICATION-ZL15097E.xls");
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber4:
                            templatePath = Utils.GetMapPath("~/data/Documents/" + SelectCustomer + "/CERTIFICATION OF TRANSSHIPMENT-ZL15097E.xls");
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber5:
                            templatePath = Utils.GetMapPath("~/data/Documents/" + SelectCustomer + "/CO-ZL15097E.xls");
                            break;
                    }
                }
                if (!File.Exists(templatePath))
                {
                    throw new Exception("模板文件不存在！" + templatePath);
                }

                //生成的Excel文件的文件夹
                string outDir = "/data/Template/Out/" + MakerTypeEnum.ToString().Split('_')[0] + "/" + id;
                string outRealDir = HttpContext.Current.Server.MapPath("~" + outDir);
                if (!Directory.Exists(outRealDir))
                {
                    Directory.CreateDirectory(outRealDir);
                }

                //Excel文件
                string fileName = string.Format("{0}{1}", MakerTypeEnum.ToString(), customIndex == null ? string.Empty : "_" + customIndex);
                string OutExcelName = string.Empty;

                if (SelectCustomer == SelectCustomerEnum.S05.ToString())
                {
                    switch (MakerTypeEnum)
                    {
                        case MakerTypeEnum.InspectionClearance_CreditNumber1:
                            fileName = "Beneficiaries";
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber2:
                            fileName = "CERTIFICATION-8 point";
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber3:
                            fileName = "CERTIFICATION";
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber4:
                            fileName = "CERTIFICATION OF TRANSSHIPMENT";
                            break;

                        case MakerTypeEnum.InspectionClearance_CreditNumber5:
                            fileName = "CO";
                            break;
                    }
                }

                fileName = fileName.Replace("InspectionReceipt_", "").Replace("InspectionCustoms_", "").Replace("InspectionClearance_", "").Replace("InspectionExchange_", "");//移除文件名的前缀

                //OutExcelName = string.Format("{0}{1}", fileName, ".xls");

                //string excelPath = outRealDir + "/" + OutExcelName;

                IWorkbook workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));

                HSSFSheet sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);
                SelectCustomerEnum customerCodeEnum = (SelectCustomerEnum)Tools.EnumHelper.EnumHelper.GetCustomEnum(typeof(SelectCustomerEnum), SelectCustomer);

                switch (customerCodeEnum)
                {
                    case SelectCustomerEnum.S188:
                        MakerExcel_Documents_S188.Bind_InspectionReceipt_S188(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S188.Bind_InspectionCustoms_S188(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S188.Bind_InspectionExchange_S188(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        break;

                    case SelectCustomerEnum.S60:
                        MakerExcel_Documents_S60.Bind_InspectionReceipt_S60(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S60.Bind_InspectionCustoms_S60(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S60.Bind_InspectionExchange_S60(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        break;

                    case SelectCustomerEnum.S288:
                        MakerExcel_Documents_S288.Bind_InspectionReceipt_S288(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S288.Bind_InspectionCustoms_S288(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S288.Bind_InspectionClearance_S288(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S288.Bind_InspectionExchange_S288(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S13:
                        MakerExcel_Documents_S13.Bind_InspectionReceipt_S13(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S13.Bind_InspectionCustoms_S13(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S13.Bind_InspectionClearance_S13(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S13.Bind_InspectionExchange_S13(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S05:
                        MakerExcel_Documents_S05.Bind_InspectionReceipt_S05(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S05.Bind_InspectionCustoms_S05(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S05.Bind_InspectionClearance_S05(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S05.Bind_InspectionExchange_S05(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        MakerExcel_Documents_S05.Bind_InspectionClearance_CreditNumber_S05(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.DG:
                        MakerExcel_Documents_DG.Bind_InspectionReceipt_DG(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_DG.Bind_InspectionCustoms_DG(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_DG.Bind_InspectionClearance_DG(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_DG.Bind_InspectionExchange_DG(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S220:
                        MakerExcel_Documents_S220.Bind_InspectionReceipt_S220(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S220.Bind_InspectionCustoms_S220(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S220.Bind_InspectionClearance_S220(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S220.Bind_InspectionExchange_S220(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S235:
                        MakerExcel_Documents_S235.Bind_InspectionReceipt_S235(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S235.Bind_InspectionCustoms_S235(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S235.Bind_InspectionClearance_S235(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S235.Bind_InspectionExchange_S235(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S10:
                        MakerExcel_Documents_S10.Bind_InspectionReceipt_S10(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S10.Bind_InspectionCustoms_S10(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S10.Bind_InspectionClearance_S10(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S10.Bind_InspectionExchange_S10(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S52:
                        MakerExcel_Documents_S52.Bind_InspectionReceipt_S52(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S52.Bind_InspectionCustoms_S52(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S52.Bind_InspectionClearance_S52(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S52.Bind_InspectionExchange_S52(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S56:
                        MakerExcel_Documents_S56.Bind_InspectionReceipt_S56(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S56.Bind_InspectionCustoms_S56(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S56.Bind_InspectionClearance_S56(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S56.Bind_InspectionExchange_S56(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S164:
                        MakerExcel_Documents_S164.Bind_InspectionReceipt_S164(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S164.Bind_InspectionCustoms_S164(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S164.Bind_InspectionClearance_S164(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S164.Bind_InspectionExchange_S164(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    case SelectCustomerEnum.S135:
                        MakerExcel_Documents_S135.Bind_InspectionReceipt_S135(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S135.Bind_InspectionCustoms_S135(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S135.Bind_InspectionClearance_S135(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_S135.Bind_InspectionExchange_S135(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;
                    case SelectCustomerEnum.F20:
                        MakerExcel_Documents_F20.Bind_InspectionReceipt_F20(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_F20.Bind_InspectionCustoms_F20(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_F20.Bind_InspectionClearance_F20(model, workbook, sheetQuoteSheet, MakerTypeEnum);
                        MakerExcel_Documents_F20.Bind_InspectionExchange_F20(model, workbook, sheetQuoteSheet, MakerTypeEnum);

                        break;

                    default:
                        break;
                }

                return ExcelHelper.SaveFile_PDFAndExcel(outFileList, outRealDir, workbook, fileName, filesGenerated);

                ////生成excel文件
                //FileStream swSheet = File.OpenWrite(excelPath);
                //workbook.Write(swSheet);
                //swSheet.Close();

                //return outDir + "/PDFAndExcel/" + OutExcelName + ".xls";
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                throw;
            }
        }
    }
}