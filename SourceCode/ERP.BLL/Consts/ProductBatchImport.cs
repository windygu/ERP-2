using ERP.Models.Product;
using ERP.Tools;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Consts
{
    /// <summary>
    /// 产品批量导入
    /// </summary>
    class ProductBatchImport
    {
        /// <summary>
        /// 批量导入产品
        /// </summary>
        /// <returns></returns>
        public static List<VMProductInfo> BatchImport(List<string> list_No, int classifyID, string excelFileName, List<string> list_UPC, List<DAL.Com_DataDictionary> list_Com_DataDictionary, List<DAL.Factory> list_Factory, List<DAL.Orders_Customers> list_Orders_Customers, string excelName, List<DTOBatchUploadProduct> list, DTOBatchUploadProduct dtobatch, out int AllCount2)
        {
            ERP.Dictionary.DictionaryServices _dictionaryServices = new ERP.Dictionary.DictionaryServices();
            string templateFile = dtobatch.NewFilePath.Replace(ConfigurationManager.AppSettings[AdminConsts.RESOURCE_SERVER_CONFIGURATION_VIRTUALPATH], string.Empty);
            string templatePath = ConfigurationManager.AppSettings[AdminConsts.RESOURCE_SERVER_CONFIGURATION_PHYSICSPATH] + templateFile;
            if (!File.Exists(templatePath))
            {
                throw new Exception("文件不存在！" + templatePath);
            }
            IWorkbook workbook = null;

            ISheet sheetQuoteSheet = null;
            ISheet newSheet = null;
            List<VMProductInfo> list_vm = new List<VMProductInfo>();
            int AllCount = 0;
            int SuccessCount = 0;

            List<int> list_success = new List<int>();//验证通过的列索引
            string temp = "";
            ICellStyle cellStyle = null;
            #region 获取字段的索引列

            int No = 0, CustomerNo = 1, NoFactory = 2, FactoryName = 3;
            //int classifyYear = 4, classifySeason = 5, classifyArea = 6, classifyName = 7;
            int Name = 4, Desc = 5, UnitName = 6, StyleName = 7;
            int Length = 8, Width = 10, Height = 12, Weight = 14;
            int PDQPackRate = 16, PDQLength = 17, PDQWidth = 19, PDQHeight = 21;
            int InnerBoxRate = 23, InnerLength = 24, InnerWidth = 26, InnerHeight = 28, InnerVolume = 30, InnerWeight = 31;
            int OuterBoxRate = 33, OuterLength = 34, OuterWidth = 36, OuterHeight = 38, OuterVolume = 40;
            int PriceFactory = 41;
            int OuterWeightGross = 42, OuterWeightNet = 44;
            int PortName = 46, PackingMannerZhName = 47, IngredientZh = 48, MOQZh = 49, Comment = 50, PriceInputDate = 51, ValidDate = 52, UPC = 53;
            int NoHadExist = 54;

            #endregion 获取字段的索引列

            if (excelName == "xls")
            {
                #region xls文件的操作
                workbook = new HSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                sheetQuoteSheet = (HSSFSheet)workbook.GetSheetAt(0);
                int thisSheetIndex = workbook.GetSheetIndex("导入失败的产品");
                if (thisSheetIndex > 0)
                {
                    workbook.RemoveSheetAt(thisSheetIndex);
                }
                newSheet = (HSSFSheet)workbook.GetSheetAt(0).CopySheet("导入失败的产品");
                newSheet.IsActive = true;
                cellStyle = GetCellStyle((HSSFWorkbook)workbook, null, HorizontalAlignment.Justify, VerticalAlignment.Justify);
                cellStyle.FillForegroundColor = HSSFColor.Red.Index;
                cellStyle.FillForegroundColor = HSSFColor.Red.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
                #endregion xls文件的操作
            }
            else if (excelName == "xlsx")
            {
                #region xlsx文件的操作
                workbook = new XSSFWorkbook(new FileStream(templatePath, FileMode.Open, FileAccess.Read));
                sheetQuoteSheet = (XSSFSheet)workbook.GetSheetAt(0);
                int thisSheetIndexx = workbook.GetSheetIndex("导入失败的产品");
                if (thisSheetIndexx > 0)
                {
                    workbook.RemoveSheetAt(thisSheetIndexx);
                }
                newSheet = (XSSFSheet)workbook.GetSheetAt(0).CopySheet("导入失败的产品");
                newSheet.IsActive = true;
                cellStyle = GetCellStylee((XSSFWorkbook)workbook, null, HorizontalAlignment.Justify, VerticalAlignment.Justify);
                cellStyle.FillForegroundColor = HSSFColor.Red.Index;
                cellStyle.FillForegroundColor = HSSFColor.Red.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
                #endregion xlsx文件的操作
            }

            #region
            for (int i = 2; i < sheetQuoteSheet.LastRowNum + 1; i++)
            {
                if (!string.IsNullOrEmpty(GetCellValue(workbook, sheetQuoteSheet, i, No)))
                {
                    #region 验证Model

                    bool isValid = true;//true 验证通过

                    //基本信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, No);// No
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellStyle(newSheet, i, No, cellStyle);
                        isValid = false;
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, Name);// Name
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, Name, "", cellStyle);
                        isValid = false;
                    }

                    //2016.03.31 产品批量导入Desc改为非必填

                    //temp = GetCellValue(workbook,sheetQuoteSheet, i, Desc);// Desc
                    //if (string.IsNullOrEmpty(temp))// 必填
                    //{
                    //    SetCellValue(newSheet, i, Desc, "", cellStyle);
                    //    isValid = false;
                    //}

                    //产品尺寸
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, Length);// Length
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, Length, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, Length, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, Width);// Width
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, Width, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, Width, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, Height);// Height
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, Height, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, Height, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, Weight);// Weight
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, Weight, cellStyle);
                            isValid = false;
                        }
                    }

                    //PDQ信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PDQPackRate);// PDQPackRate
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsInt(temp))
                        {
                            SetCellStyle(newSheet, i, PDQPackRate, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PDQLength);// PDQLength
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, PDQLength, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PDQWidth);// PDQWidth
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, PDQWidth, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PDQHeight);// PDQHeight
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, PDQHeight, cellStyle);
                            isValid = false;
                        }
                    }

                    //内盒信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, InnerBoxRate);// InnerBoxRate
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsInt(temp))
                        {
                            SetCellStyle(newSheet, i, InnerBoxRate, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, InnerLength);// InnerLength
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, InnerLength, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, InnerWidth);// InnerWidth
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, InnerWidth, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, InnerHeight);// InnerHeight
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, InnerHeight, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, InnerWeight);// InnerWeight
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, InnerWeight, cellStyle);
                            isValid = false;
                        }
                    }

                    //外箱信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, OuterBoxRate);// OuterBoxRate
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsInt(temp))
                        {
                            SetCellStyle(newSheet, i, OuterBoxRate, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, OuterLength);// OuterLength
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, OuterLength, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, OuterWidth);// OuterWidth
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, OuterWidth, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, OuterHeight);// OuterHeight
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, OuterHeight, cellStyle);
                            isValid = false;
                        }
                    }

                    //价格信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PriceFactory);// PriceFactory
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, PriceFactory, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, PriceFactory, cellStyle);
                            isValid = false;
                        }
                    }

                    //外箱信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, OuterWeightGross);// OuterWeightGross
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, OuterWeightGross, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, OuterWeightNet);// OuterWeightNet
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsDouble(temp))
                        {
                            SetCellStyle(newSheet, i, OuterWeightNet, cellStyle);
                            isValid = false;
                        }
                    }

                    //其他信息
                    temp = GetCellValue(workbook, sheetQuoteSheet, i, MOQZh);// MOQZh
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (!Utils.IsInt(temp))
                        {
                            SetCellStyle(newSheet, i, MOQZh, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetPriceInputDate(workbook, sheetQuoteSheet, i, PriceInputDate);// PriceInputDate
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, PriceInputDate, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (!Utils.IsDateTime(temp))
                        {
                            SetCellStyle(newSheet, i, PriceInputDate, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, No);// No
                    if (list_No.Contains(temp.StrTrim()))
                    {
                        SetCellValue(newSheet, i, NoHadExist, "该货号已存在", cellStyle);
                        SetCellStyle(newSheet, i, No, cellStyle);
                        isValid = false;
                    }

                    #endregion 验证Model

                    #region 给Model赋值

                    VMProductInfo vm = new VMProductInfo();
                    vm.ClassifyID = classifyID;

                    //基本信息
                    vm.No = GetCellValue(workbook, sheetQuoteSheet, i, No).StrTrim();
                    vm.Image = "";
                    if (list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            //截取数组中存储到服务器端的文件 arrary[5]是新文件地址
                            string[] arrary = item.NewFilePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                            //截取jkNumber qrr[0]是excel中的货号
                            string[] qrr = item.OldFileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                            if (qrr[0] == vm.No)
                            {
                                vm.Image = item.NewFilePath;
                                //vm.Image = "/data/Images/product/" + arrary[4] + "/" + arrary[5] + "/" + arrary[6];
                            }
                        }
                    }
                    vm.CustomerNo = GetCellValue(workbook, sheetQuoteSheet, i, CustomerNo);
                    vm.NoFactory = GetCellValue(workbook, sheetQuoteSheet, i, NoFactory);
                    vm.FactoryName = GetCellValue(workbook, sheetQuoteSheet, i, FactoryName);

                    //产品信息
                    vm.Name = GetCellValue(workbook, sheetQuoteSheet, i, Name);
                    vm.Desc = GetCellValue(workbook, sheetQuoteSheet, i, Desc);
                    vm.UnitName = GetCellValue(workbook, sheetQuoteSheet, i, UnitName);
                    vm.StyleName = GetCellValue(workbook, sheetQuoteSheet, i, StyleName);

                    //产品尺寸
                    vm.Length = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, Length), 0);
                    vm.Width = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, Width), 0);
                    vm.Height = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, Height), 0);
                    vm.Weight = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, Weight), 0);

                    //PDQ信息
                    vm.PDQPackRate = Utils.StrToInt(GetCellValue(workbook, sheetQuoteSheet, i, PDQPackRate), 0);
                    vm.PDQLength = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, PDQLength), 0);
                    vm.PDQWidth = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, PDQWidth), 0);
                    vm.PDQHeight = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, PDQHeight), 0);

                    //内盒信息
                    vm.InnerBoxRate = Utils.StrToInt(GetCellValue(workbook, sheetQuoteSheet, i, InnerBoxRate), 0);
                    vm.InnerLength = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, InnerLength), 0);
                    vm.InnerWidth = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, InnerWidth), 0);
                    vm.InnerHeight = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, InnerHeight), 0);
                    //vm.InnerVolume = Utils.StrToDecimal(GetCellValue(workbook,sheetQuoteSheet, i, InnerVolume), 0);
                    vm.InnerWeight = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, InnerWeight), 0);

                    //外箱信息
                    vm.OuterBoxRate = Utils.StrToInt(GetCellValue(workbook, sheetQuoteSheet, i, OuterBoxRate), 0);
                    vm.OuterLength = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, OuterLength), 0);
                    vm.OuterWidth = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, OuterWidth), 0);
                    vm.OuterHeight = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, OuterHeight), 0);
                    //vm.OuterVolume = Utils.StrToDecimal(GetCellValue(workbook,sheetQuoteSheet, i, OuterVolume), 0);

                    //价格信息
                    vm.PriceFactory = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, PriceFactory), 0);

                    //外箱信息
                    vm.OuterWeightGross = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, OuterWeightGross), 0);
                    vm.OuterWeightNet = Utils.StrToDecimal(GetCellValue(workbook, sheetQuoteSheet, i, OuterWeightNet), 0);

                    //其他信息
                    vm.PortName = GetCellValue(workbook, sheetQuoteSheet, i, PortName);
                    vm.PortEnName = GetCellValue(workbook, sheetQuoteSheet, i, PortName);
                    vm.PackingMannerZhName = GetCellValue(workbook, sheetQuoteSheet, i, PackingMannerZhName);
                    vm.PackingMannerEnName = GetCellValue(workbook, sheetQuoteSheet, i, PackingMannerZhName);
                    vm.IngredientZh = GetCellValue(workbook, sheetQuoteSheet, i, IngredientZh);
                    vm.IngredientEn = GetCellValue(workbook, sheetQuoteSheet, i, IngredientZh);
                    vm.MOQZh = Utils.StrToInt(GetCellValue(workbook, sheetQuoteSheet, i, MOQZh), 0);
                    vm.MOQEn = Utils.StrToInt(GetCellValue(workbook, sheetQuoteSheet, i, MOQZh), 0);
                    vm.Comment = GetCellValue(workbook, sheetQuoteSheet, i, Comment);

                    DateTime dtPriceInputDate = Utils.StrToDateTime(GetPriceInputDate(workbook, sheetQuoteSheet, i, PriceInputDate));

                    vm.PriceInputDate = dtPriceInputDate;
                    if (GetCellValue(workbook, sheetQuoteSheet, i, ValidDate) == "半年")
                    {
                        vm.ValidDate = dtPriceInputDate.AddMonths(6);
                    }
                    vm.UPC = GetCellValue(workbook, sheetQuoteSheet, i, UPC);

                    //设置默认值
                    vm.Agent = 2.5m;
                    vm.CtnsPallet = 40;
                    vm.FreightRate = 2.5m;
                    vm.MiscImportLoad = 1.5m;

                    //给ID赋值
                    vm.StyleID = _dictionaryServices.GetCode_StyleID(vm.StyleName, list_Com_DataDictionary);
                    vm.UnitID = _dictionaryServices.GetCode_UnitID(vm.UnitName, list_Com_DataDictionary);
                    vm.PortID = _dictionaryServices.GetCode_PortID(vm.PortName, list_Com_DataDictionary);
                    vm.PackingMannerZhID = _dictionaryServices.GetCode_PackingMannerZhID(vm.PackingMannerZhName, list_Com_DataDictionary);
                    vm.PackingMannerEnID = _dictionaryServices.GetCode_PackingMannerZhID(vm.PackingMannerZhName, list_Com_DataDictionary);

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PriceFactory);// PriceFactory
                    if (!string.IsNullOrEmpty(temp))
                    {
                        string CurrencyType = GetCellValue(workbook, sheetQuoteSheet, i, PriceFactory).ToString();

                        IRow row = sheetQuoteSheet.GetRow(i);
                        if (row != null)
                        {
                            ICell cell = row.GetCell(PriceFactory);
                            if (cell != null)
                            {
                                vm.CurrencyType = _dictionaryServices.GetCode_CurrencyType(Keys.RMB, list_Com_DataDictionary);

                                var strFormat = cell.CellStyle.GetDataFormatString();
                                if (strFormat.IndexOf("$") != -1)
                                {
                                    vm.CurrencyType = _dictionaryServices.GetCode_CurrencyType(Keys.USD, list_Com_DataDictionary);
                                }
                                if (strFormat.IndexOf("￥") != -1 || strFormat.IndexOf("¥") != -1)
                                {
                                    vm.CurrencyType = _dictionaryServices.GetCode_CurrencyType(Keys.RMB, list_Com_DataDictionary);
                                }
                            }
                        }
                    }

                    if (list_Factory.Where(d => d.Abbreviation == vm.FactoryName).Count() > 0)
                    {
                        vm.FactoryID = list_Factory.Where(d => d.Abbreviation == vm.FactoryName).First().ID;
                    }
                    else
                    {
                        vm.FactoryID = -1;
                    }

                    if (list_Orders_Customers.Where(d => d.CustomerCode == vm.CustomerNo).Count() > 0)
                    {
                        vm.CustomerID = list_Orders_Customers.Where(d => d.CustomerCode == vm.CustomerNo).First().OCID;
                    }
                    else
                    {
                        vm.CustomerID = -1;
                    }

                    #endregion 给Model赋值

                    #region 验证下拉框、其他

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, CustomerNo);// CustomerNo
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, CustomerNo, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.CustomerID < 0)
                        {
                            SetCellStyle(newSheet, i, CustomerNo, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, FactoryName);// FactoryName
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, FactoryName, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.FactoryID < 0)
                        {
                            SetCellStyle(newSheet, i, FactoryName, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, UnitName);// UnitName
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, UnitName, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.UnitID < 0)
                        {
                            SetCellStyle(newSheet, i, UnitName, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, StyleName);// StyleName
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, StyleName, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.StyleID < 0)
                        {
                            SetCellStyle(newSheet, i, StyleName, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PortName);// PortName
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, PortName, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.PortID < 0)
                        {
                            SetCellStyle(newSheet, i, PortName, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, PackingMannerZhName);// PackingMannerZhName
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, PackingMannerZhName, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.PackingMannerZhID < 0)
                        {
                            SetCellStyle(newSheet, i, PackingMannerZhName, cellStyle);
                            isValid = false;
                        }
                    }

                    temp = GetCellValue(workbook, sheetQuoteSheet, i, IngredientZh);// IngredientZh
                    if (string.IsNullOrEmpty(temp))// 必填
                    {
                        SetCellValue(newSheet, i, IngredientZh, "", cellStyle);
                        isValid = false;
                    }
                    else
                    {
                        if (vm.PackingMannerZhID < 0)
                        {
                            SetCellStyle(newSheet, i, IngredientZh, cellStyle);
                            isValid = false;
                        }
                    }

                    #endregion 验证下拉框、其他

                    #region 验证UPC

                    //temp = GetCellValue(workbook,sheetQuoteSheet, i, UPC);// UPC
                    //if (string.IsNullOrEmpty(temp))
                    //{
                    //    string temp_UPC = list_UPC.Max();
                    //    int maxUPC = Utils.StrToInt(temp_UPC.Substring(8, 4), 6000) + 1;//从6001开始
                    //    if (maxUPC < 6000)
                    //    {
                    //        maxUPC = 6001;
                    //    }
                    //    string calUPC = CalculateUPC("69526588" + maxUPC);
                    //    SetCellValue(newSheet, i, UPC, calUPC, cellStyle);
                    //    list_UPC.Add(calUPC);
                    //    vm.UPC = calUPC;
                    //    //isValid = false;
                    //}

                    #endregion 验证UPC

                    if (isValid)
                    {
                        ++SuccessCount;
                        list_vm.Add(vm);
                        list_success.Add(i);
                    }
                    ++AllCount;
                }
            }

            list_success.Reverse();

            foreach (var item in list_success)
            {
                IRow row = newSheet.GetRow(item);
                newSheet.RemoveRow(row);
            }
            using (FileStream fs = new FileStream(templatePath, FileMode.Create))
            {
                workbook.Write(fs);
            }
            #endregion

            if (list_vm != null && list_vm.Count > 0)
            {
                list_vm.First().AllCount = AllCount;
            }
            AllCount2 = AllCount;
            return list_vm;
        }

        #region HelperMethod

        #region 设置单元格的内容

        private static void SetCellValue(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, int value)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value.ToString());
        }

        private static void SetCellValue(XSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex, int value)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value.ToString());
        }

        private static void SetCellValue(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, decimal value)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value.ToString());
        }

        private static void SetCellValue(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, string value)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value, null);
        }

        private static void SetCellValue(XSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex, string value)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value, null);
        }

        private static void SetCellValue(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, int value, ICellStyle cellStyle)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value.ToString(), cellStyle);
        }

        private static void SetCellValue(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, decimal value, ICellStyle cellStyle)
        {
            SetCellValue(sheetQuoteSheet, rowIndex, cellIndex, value.ToString(), cellStyle);
        }

        /// <summary>
        /// 设置单元格的内容
        /// </summary>
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        private static void SetCellValue(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, string value, ICellStyle cellStyle)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheetQuoteSheet.CreateRow(rowIndex);
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
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        private static void SetCellValue(XSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex, string value, ICellStyle cellStyle)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheetQuoteSheet.CreateRow(rowIndex);
            }
            row.CreateCell(cellIndex).SetCellValue(value);

            if (cellStyle != null)
            {
                row.GetCell(cellIndex).CellStyle = cellStyle;
            }
        }

        #endregion 设置单元格的内容

        #region 获取单元格的内容

        /// <summary>
        /// 获取单元格的内容
        /// </summary>
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        private static string GetCellValueToString(ISheet sheetQuoteSheet, int rowIndex, int cellIndex)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row != null)
            {
                if (row.GetCell(cellIndex) != null)
                {
                    return row.GetCell(cellIndex).ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单元格的内容
        /// </summary>
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        private static string GetCellValueToString(XSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row != null)
            {
                if (row.GetCell(cellIndex) != null)
                {
                    return row.GetCell(cellIndex).ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取单元格的内容
        /// </summary>
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="value"></param>
        private static string GetCellValue(IWorkbook workbook, ISheet sheetQuoteSheet, int rowIndex, int cellIndex)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row != null)
            {
                ICell cell = row.GetCell(cellIndex);
                if (cell != null)
                {
                    return GetCellForamtValue(cell, workbook);
                }
            }
            return null;
        }

        public static string GetCellForamtValue(ICell cell, IWorkbook workbook)
        {
            string value = "";
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型处理
                    value = "";
                    break;

                case CellType.String: //字符串类型
                    value = cell.StringCellValue;
                    break;

                case CellType.Numeric: //数字类型
                    if (Utils.IsDouble(cell.NumericCellValue.ToString()))
                    {
                        value = cell.NumericCellValue.ToString();
                    }
                    else
                    {
                        value = cell.DateCellValue.ToString();
                    }
                    break;

                case CellType.Formula:
                    if (workbook.GetType() == typeof(XSSFWorkbook))
                    {
                        XSSFFormulaEvaluator e = new XSSFFormulaEvaluator(workbook);
                        value = e.Evaluate(cell).NumberValue.ToString();
                    }
                    else
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(workbook);
                        value = e.Evaluate(cell).NumberValue.ToString();
                    }
                    //value = cell.CellFormula;
                    break;

                default:
                    value = "";
                    break;
            }
            return value;
        }

        #endregion 获取单元格的内容

        public static void SetCellStyle(ISheet sheetQuoteSheet, int rowIndex, int cellIndex, ICellStyle cellStyle)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheetQuoteSheet.CreateRow(rowIndex);
            }

            if (cellStyle != null)
            {
                row.GetCell(cellIndex).CellStyle = cellStyle;
            }
        }

        public static void SetCellStyle(XSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex, ICellStyle cellStyle)
        {
            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row == null)
            {
                row = sheetQuoteSheet.CreateRow(rowIndex);
            }

            if (cellStyle != null)
            {
                row.GetCell(cellIndex).CellStyle = cellStyle;
            }
        }

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
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="thisRow"></param>
        /// <param name="p"></param>
        /// <param name="cellStyle"></param>
        private static void GetCellStyle(HSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex, ICellStyle cellStyle)
        {
            sheetQuoteSheet.GetRow(rowIndex).GetCell(cellIndex).CellStyle = cellStyle;
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
        public static ICellStyle GetCellStylee(XSSFWorkbook workbook, IFont font, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, bool border = true, short BackgroundColor = -1)
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
        /// 获取价格输入日期的值
        /// </summary>
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public static string GetPriceInputDate(IWorkbook workbook, ISheet sheetQuoteSheet, int rowIndex, int cellIndex)
        {
            string PriceInputDate = GetCellValue(workbook, sheetQuoteSheet, rowIndex, cellIndex);
            if (string.IsNullOrEmpty(PriceInputDate))
            {
                return "";
            }
            if (Utils.IsDateTime(PriceInputDate))
            {
                return PriceInputDate;
            }

            IRow row = sheetQuoteSheet.GetRow(rowIndex);
            if (row != null)
            {
                ICell cell = row.GetCell(cellIndex);
                if (cell != null)
                {
                    try
                    {
                        PriceInputDate = cell.DateCellValue.ToString();
                        if (Utils.IsDateTime(PriceInputDate))
                        {
                            return PriceInputDate;
                        }
                    }
                    catch (Exception)
                    {
                        return PriceInputDate;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 设置单元格的样式
        /// </summary>
        /// <param name="sheetQuoteSheet"></param>
        /// <param name="thisRow"></param>
        /// <param name="p"></param>
        /// <param name="cellStyle"></param>
        private static void GetCellStyle(XSSFSheet sheetQuoteSheet, int rowIndex, int cellIndex, ICellStyle cellStyle)
        {
            sheetQuoteSheet.GetRow(rowIndex).GetCell(cellIndex).CellStyle = cellStyle;
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

        #endregion HelperMethod
    }
}