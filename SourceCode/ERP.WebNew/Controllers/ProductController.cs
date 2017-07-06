using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.BLL.ERP.Customer;
using ERP.BLL.ERP.Dictionary;
using ERP.BLL.ERP.Factory;
using ERP.BLL.ERP.HS;
using ERP.BLL.ERP.Product;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Models.Dictionary;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.Logs;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.ProductManagement_Info)]
    public class ProductController : Controller
    {
        private const string QUERY_PRODUCTNO = "ProductNO";
        private const string QUERY_CUSTOMERNO = "CustomerNo";
        private const string QUERY_FACTORYNAME = "FactoryName";
        private const string QUERY_KEYWORD = "KeyWord";
        private const string QUERY_ISQUOTEPRODUCT = "isquoteproduct";

        private ProductServices _productServices = new ProductServices();
        private OrderCustomerServices _orderCustomerServices = new OrderCustomerServices();
        private FactoryServices _factoryServices = new FactoryServices();
        private ProductClassificationService _productClassificationServices = new ProductClassificationService();
        private DictionaryServices _dictionaryServices = new DictionaryServices();
        private UserServices _userService = new UserServices();
        private HSService _hSService = new HSService();

        #region HelperMethod

        /// <summary>
        /// 下载标签——生成图片
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public void ExportJpg(int ProductID, string ItemNo, string Size, string Packing, string Cost, string path)
        {
            Gma.QrCodeNet.Encoding.QrEncoder encoder = new Gma.QrCodeNet.Encoding.QrEncoder(Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.H);
            Gma.QrCodeNet.Encoding.QrCode qrCode = encoder.Encode(Newtonsoft.Json.JsonConvert.SerializeObject(
                new DTOQRCode()
                {
                    Type = QRCodeType.ProductID,
                    Value = ProductID
                }));
            MemoryStream ms = new Tools.QRCode.QRRenderer(5, Brushes.Black, Brushes.White).CreateImageFile(qrCode.Matrix, ImageFormat.Png);
            Image image = Image.FromStream(ms);

            Bitmap bmp = new Bitmap(Utils.GetMapPath("/images/productLabel.jpg"));
            Graphics g = Graphics.FromImage(bmp);

            SolidBrush sb = new SolidBrush(Color.Black);
            g.DrawString(ItemNo, new Font("arial", 9, FontStyle.Regular), sb, 118, 24);
            g.DrawString(Size, new Font("arial", 9, FontStyle.Regular), sb, 118, 76);
            g.DrawString(Packing, new Font("arial", 9, FontStyle.Regular), sb, 118, 128);
            g.DrawString(Cost, new Font("arial", 9, FontStyle.Regular), sb, 118, 180);
            g.DrawImage(image, 240, 120, 150, 150);
            g.Save();
            if (!Directory.Exists(Utils.GetMapPath(path)))
            {
                Directory.CreateDirectory(Utils.GetMapPath(path));
            }
            bmp.Save(Utils.GetMapPath(path + "/" + ItemNo.Replace("/", "") + ".jpg"), ImageFormat.Jpeg);
        }

        /// <summary>
        /// 下载标签——生成压缩文件，并下载。
        /// </summary>
        /// <param name="path"></param>
        public void ZipJpg(string path)
        {
            try
            {
                string sourcePath = Utils.GetMapPath(path);
                string newPath = Utils.GetMapPath(path) + ".zip";
                ZipFile.CreateFromDirectory(sourcePath, newPath);//创建压缩文件
                Utils.DeleteDirectory(path);//删除生成的文件夹
                CommonCode.DownLoadFile(newPath, DateTime.Now.ToString("label_yyyyMMdd_HHmmss_") + new Random().Next(10000) + ".zip");//下载压缩文件

                #region 删除一天之前的压缩文件

                string[] arr = Directory.GetFiles(Utils.GetMapPath("/images/productLabel"));
                for (int i = 0; i < arr.Length; i++)
                {
                    FileInfo f = new FileInfo(arr[i]);
                    string fileName = f.Name;
                    double aa = Utils.StrToDouble(fileName.Substring(0, 8), 0);
                    double now = Utils.StrToDouble(DateTime.Now.ToString("yyyyMMdd"), 0);
                    if (aa < now - 1)
                    {
                        if (System.IO.File.Exists(arr[i]))
                        {
                            System.IO.File.Delete(arr[i]);
                        }
                    }
                }

                #endregion 删除一天之前的压缩文件
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
        }

        #endregion HelperMethod

        #region UserMethod

        public ActionResult Index()
        {
            VMProductSearchModel searchModel = new VMProductSearchModel()
            {
                PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info),
                ProductNO = HttpContext.Request[QUERY_PRODUCTNO],
                CustomerNO = HttpContext.Request[QUERY_CUSTOMERNO],
                FactoryName = HttpContext.Request[QUERY_FACTORYNAME],
                Keyword = HttpContext.Request[QUERY_KEYWORD]
            };

            var settings = _userService.GetUserCustomPageSettings(CurrentUserServices.Me.UserID, DatagridCustomColumnVisibilityModules.ProductManagement_List);
            if (settings != null && !string.IsNullOrEmpty(settings.DatagridColumnVisibility))
            {
                ViewBag.ColumnsVisible = ((Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(settings.DatagridColumnVisibility)).ToObject(typeof(List<string>));
            }
            return View(searchModel);
        }

        private string ReplaceNames(string name)
        {
            string dbColumnName = string.Empty;

            switch (name)
            {
                case "FactoryName":
                    dbColumnName = "FactoryID";
                    break;

                case "CustomerNo":
                    dbColumnName = "Orders_Customers.CustomerCode";
                    break;

                case "StyleName":
                    dbColumnName = "StyleID";
                    break;

                case "PortName":
                    dbColumnName = "PortID";
                    break;

                case "PackingMannerZhName":
                    dbColumnName = "PackingMannerZhID";
                    break;

                case "UnitName":
                    dbColumnName = "UnitID";
                    break;

                case "CurrencyName":
                    dbColumnName = "CurrencyType";
                    break;

                case "PriceInputDateFormat":
                    dbColumnName = "PriceInputDate";
                    break;

                case "ValidDateFormat":
                    dbColumnName = "ValidDate";
                    break;

                default:
                    dbColumnName = name;
                    break;
            }
            return dbColumnName;
        }

        public ActionResult GetAll()
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            string productNO = HttpContext.Request[QUERY_PRODUCTNO];
            string customerNo = HttpContext.Request[QUERY_CUSTOMERNO];
            string factoryName = HttpContext.Request[QUERY_FACTORYNAME];
            string keyWord = HttpContext.Request[QUERY_KEYWORD];
            bool isQuoteProduct = false;
            bool.TryParse(HttpContext.Request[QUERY_ISQUOTEPRODUCT], out isQuoteProduct);

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES]))
            {
                var sortColumnsNamesTmp = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // 需要替换转换过的属性
                foreach (var n in sortColumnsNamesTmp)
                {
                    sortColumnsNames.Add(ReplaceNames(n));
                }
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS]))
            {
                sortColumnOrders = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            List<string> productNOs = new List<string>();
            if (!string.IsNullOrEmpty(productNO))
            {
                productNOs = productNO.Split(new string[] { ";", "；", ",", "，", " ", Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            List<VMProductInfo> products = _productServices.GetAll(CurrentUserServices.Me, productNOs,
                customerNo, factoryName, keyWord, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows, isQuoteProduct);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = products });
        }

        public ActionResult Details(int ID)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)ProductListElementPrivileges.ViewProduct) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            VMProductInfo product = _productServices.GetProductByID(CurrentUserServices.Me, ID, false);
            if (product == null)
            {
                return RedirectToAction("PageNotFind", "Account");
            }

            var customer = _orderCustomerServices.GetAllCustomersKeyInfo(CurrentUserServices.Me.UserID).FirstOrDefault(p => p.OCID == product.CustomerID);
            ViewBag.CustomerName = customer != null ? customer.CustomerCode : string.Empty;

            var factory = _factoryServices.GetSupplierSelectList(CurrentUserServices.Me.UserID, 1).FirstOrDefault(p => p.ID == product.FactoryID);
            ViewBag.FactoryName = factory != null ? factory.Abbreviation : string.Empty;

            var productClassification = _productClassificationServices.GetAllLeafNodes(CurrentUserServices.Me).FirstOrDefault(p => p.ID == product.ClassifyID);
            ViewBag.ProductClassificationName = productClassification != null ? productClassification.Text : string.Empty;

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);

            var unit = dictionaries.FirstOrDefault(p => p.TableKind == (int)DictionaryTableKind.ProductUnit && p.Code == product.UnitID);
            ViewBag.UnitName = unit != null ? unit.Name : string.Empty;

            var style = dictionaries.FirstOrDefault(p => p.TableKind == (int)DictionaryTableKind.ProductStyle && p.Code == product.StyleID);
            ViewBag.StyleName = style != null ? style.Name : string.Empty;

            var port = dictionaries.FirstOrDefault(p => p.TableKind == (int)DictionaryTableKind.OutPort && p.Code == product.PortID);
            ViewBag.PortName = port != null ? port.Name : string.Empty;

            var packings = dictionaries.FirstOrDefault(p => p.TableKind == (int)DictionaryTableKind.Packing && p.Code == product.PackingMannerZhID);
            ViewBag.PackingName = packings != null ? packings.Name : string.Empty;

            var currency = dictionaries.FirstOrDefault(p => p.TableKind == (int)DictionaryTableKind.Currency && p.Code == product.CurrencyType);
            ViewBag.CurrencyName = currency != null ? currency.Name : string.Empty;

            if (product.HTS.HasValue)
            {
                var HTS = _hSService.selectByID(product.HTS.Value);
                ViewBag.HTSName = HTS != null ? HTS.HSCode : string.Empty;
            }

            if (product.HSCode.HasValue)
            {
                var HSCode = _hSService.selectByID(product.HSCode.Value);
                ViewBag.HSCodeName = HSCode != null ? HSCode.HSCode : string.Empty;
            }

            return View(product);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createAsQuote">创建报价单产品</param>
        /// <param name="openCached">是否打开缓存的数据</param>
        /// <returns></returns>
        public ActionResult Edit(int id, bool createAsQuote = false, bool openCached = false)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, new List<int>() { (int)ERPPage.ProductManagement_QuoteProduct_Management, (int)ERPPage.ProductManagement_Info });

            if (createAsQuote)
            {
                if ((pageElementPrivileges & (int)ProductListElementPrivileges.CreateAsQuote) == 0)
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
            }
            else
            {
                if ((id == 0 && (pageElementPrivileges & (int)ProductListElementPrivileges.CreateProduct) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)ProductListElementPrivileges.EditProduct) == 0))
                {
                    return RedirectToAction("NoAuthPop", "Account");
                }
            }

            if (createAsQuote)
            {
                ViewBag.Title = "新建报价产品";
            }
            else
            {
                ViewBag.Title = id == 0 ? "添加产品信息" : "修改产品信息";
            }
            ViewData["IsView"] = false;
            ViewBag.CreateAsQuote = createAsQuote;
            ViewBag.ProductSingleSelection = true;

            ViewBag.CustomerInfos = _orderCustomerServices.GetAllCustomersKeyInfo(CurrentUserServices.Me.UserID);
            ViewBag.FactoryInfos = _factoryServices.GetSupplierSelectList(CurrentUserServices.Me.UserID, 1);
            ViewBag.ProductClassifications = _productClassificationServices.GetAllLeafNodes(CurrentUserServices.Me,true);
            ViewBag.HSCode = _hSService.GetHSCodeSelectList(CurrentUserServices.Me.UserID, 1);//报关
            ViewBag.HTS = _hSService.GetHSCodeSelectList(CurrentUserServices.Me.UserID, 2);

            var dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
            ViewBag.Units = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.ProductUnit).ToList();
            ViewBag.Styles = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.ProductStyle).ToList();
            ViewBag.Ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
            ViewBag.Packings = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Packing).OrderBy(p => p.Name).ToList();
            ViewBag.Currencies = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Currency).ToList();
            ViewBag.Color = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Color).ToList();

            VMProductInfo productInfo = null;
            if (openCached)
            {
                productInfo = _productServices.GetOneCachedProduct(CurrentUserServices.Me.UserID, id);
            }
            else
            {
                if (id == 0)
                {
                    productInfo = new VMProductInfo();
                    productInfo.InnerVolume = 0;
                    productInfo.OuterVolume = 0;
                }
                else
                {
                    productInfo = _productServices.GetProductByID(CurrentUserServices.Me, id, createAsQuote);
                    if (productInfo == null)
                    {
                        return RedirectToAction("PageNotFind", "Account");
                    }
                    //else
                    //{
                    //    // 不同部门之间不能修改产品数据，添加报价产品除外
                    //    if (!CurrentUserServices.Me.IsSuperAdmin && productInfo.UserHierachyID != CurrentUserServices.Me.HierachyID)
                    //    {
                    //        if (!createAsQuote)
                    //        {
                    //            return RedirectToAction("NoAuthPop", "Account");
                    //        }
                    //    }
                    //}
                }
            }


            ViewBag.SeasonList = _productServices.GetSelectCustomer_SeasonList(productInfo.CustomerID, productInfo.Season);

            return View(productInfo);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productInfo">生成报价单产品</param>
        /// <param name="createAsQuote"></param>
        /// <param name="extraNo">由于在生成报价单产品时，需要将No设为Disable，因此无法将No值传到后台，需要手动传值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id, [System.Web.Http.FromBody]VMProductInfo productInfo)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((id == 0 && (pageElementPrivileges & (int)ProductListElementPrivileges.CreateProduct) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)ProductListElementPrivileges.EditProduct) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            try
            {
                DBOperationStatus result = default(DBOperationStatus);
                productInfo.PackingMannerEnID = productInfo.PackingMannerZhID;
                productInfo.MOQEn = productInfo.MOQZh;
                if (productInfo.createAsQuote.HasValue && productInfo.createAsQuote.Value && productInfo.ParentProductID != null && productInfo.extraCustomerID.HasValue && productInfo.extraCustomerID.Value != 0)
                {
                    productInfo.CustomerID = productInfo.extraCustomerID.Value;
                }

                bool isCreateQuote = false;
                if (productInfo.createAsQuote.HasValue && productInfo.createAsQuote.Value && productInfo.ParentProductID == null)
                {
                    isCreateQuote = true;
                    productInfo.ParentProductID = id;
                    productInfo.RootProductID = productInfo.RootProductID.HasValue ? productInfo.RootProductID : id;
                }

                if (id == 0 || isCreateQuote)
                {
                    productInfo.ID = 0;
                    result = _productServices.Create(CurrentUserServices.Me, CurrentUserServices.GetCurrentRequestIPAddress(), productInfo, productInfo.createAsQuote.HasValue && productInfo.createAsQuote.Value);
                }
                else
                {
                    result = _productServices.Save(CurrentUserServices.Me, CurrentUserServices.GetCurrentRequestIPAddress(), productInfo);
                }

                // 删除缓存数据
                if (result != DBOperationStatus.Failed)
                {
                    _productServices.RemoveOneCachedProduct(CurrentUserServices.Me.UserID, productInfo.ID);
                }

                return new CustomJsonResult((short)result);
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productInfo"></param>
        /// <param name="createAsQuote">生成报价单产品</param>
        /// <param name="extraNo">由于在生成报价单产品时，需要将No设为Disable，因此无法将No值传到后台，需要手动传值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDraft(int id, [System.Web.Http.FromBody]VMProductInfo productInfo)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((id == 0 && (pageElementPrivileges & (int)ProductListElementPrivileges.CreateProduct) == 0) ||
                (id != 0 && (pageElementPrivileges & (int)ProductListElementPrivileges.EditProduct) == 0))
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            try
            {
                productInfo.PackingMannerEnID = productInfo.PackingMannerZhID;
                productInfo.MOQEn = productInfo.MOQZh;
                if (productInfo.createAsQuote.HasValue && productInfo.createAsQuote.Value && productInfo.extraCustomerID.HasValue && productInfo.extraCustomerID.Value != 0)
                {
                    productInfo.CustomerID = productInfo.extraCustomerID ?? 0;
                }
                if (productInfo.createAsQuote.HasValue && productInfo.createAsQuote.Value)
                {
                    productInfo.ParentProductID = id;
                    productInfo.RootProductID = productInfo.RootProductID.HasValue ? productInfo.RootProductID : id;
                    productInfo.ID = 0;
                    productInfo.No = productInfo.extraNo;
                }

                if (productInfo.PriceInputDate.HasValue)
                {
                    productInfo.PriceInputDateFormat = Utils.DateTimeToStr(productInfo.PriceInputDate);
                }
                if (productInfo.ValidDate.HasValue)
                {
                    productInfo.ValidDateFormat = Utils.DateTimeToStr(productInfo.ValidDate);
                }
                bool result = _productServices.CacheOneProduct(CurrentUserServices.Me.UserID, productInfo);

                return new CustomJsonResult(result);
            }
            catch
            {
                return new CustomJsonResult(false);
            }
        }

        [HttpPost]
        public ActionResult Delete([System.Web.Http.FromBody]string idList)
        {
            List<int> deleteList = null;
            if (!string.IsNullOrEmpty(idList))
            {
                deleteList = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if (deleteList != null && deleteList.Count == 1 && (pageElementPrivileges & (int)ProductListElementPrivileges.DeleteProduct) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }
            if (deleteList != null && deleteList.Count > 1 && (pageElementPrivileges & (int)ProductListElementPrivileges.BatchDelete) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            DBOperationStatus result = _productServices.Delete(CurrentUserServices.Me, deleteList, CurrentUserServices.GetCurrentRequestIPAddress());
            return new CustomJsonResult((short)result);
        }

        //查看产品目录
        public ActionResult ViewProductList()
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)ProductListElementPrivileges.ViewProductCatelog) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string idList = DTRequest.GetQueryString("idList");
            List<int> list = null;
            if (!string.IsNullOrEmpty(idList))
            {
                list = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Utils.StrToInt(p, 0)).ToList();
            }
            List<VMViewProductList> list_vm = _productServices.GetViewProductList(CurrentUserServices.Me, list);
            ViewBag.idList = idList;
            return View(list_vm);
        }

        //查看标签目录
        public ActionResult ViewLabelList()
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)ProductListElementPrivileges.ViewTagCatelog) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string idList = DTRequest.GetQueryString("idList");
            List<int> list = null;
            if (!string.IsNullOrEmpty(idList))
            {
                list = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            List<VMViewLabelList> list_vm = _productServices.GetViewLabelList(CurrentUserServices.Me, list);
            return View(list_vm);
        }

        //下载标签
        public ActionResult DownLoadLabelList()
        {
            #region 添加权限

            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)ProductListElementPrivileges.DownloadCatelog) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            #endregion 添加权限

            string idList = DTRequest.GetQueryString("idList");
            List<int> list = null;
            if (!string.IsNullOrEmpty(idList))
            {
                list = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            List<VMViewLabelList> list_vm = _productServices.GetViewLabelList(CurrentUserServices.Me, list);

            string path = "/images/productLabel/" + DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (var item in list_vm)
            {
                string itemNo = item.No + " " + item.StyleName;
                string itemNo_sub = Utils.SubString(itemNo, 17);

                string Packing = item.InnerBoxRate + "/" + item.OuterBoxRate + "/" + item.OuterVolume + "'";
                string Size = item.LengthIN + "*" + item.WidthIN + "*" + item.HeightIN + "''";
                string Cost = item.CurrencySign + CommonCode.GetPriceCode(item.PriceFactory.ToString());

                ExportJpg(item.ProductID, itemNo, Size, Packing, Cost, path);
            }
            ZipJpg(path);
            return View();
        }

        //导出产品资料
        [HttpPost]
        public ActionResult Export(FormCollection collectons)
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)ProductListElementPrivileges.ExportProductExcel) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            if (collectons != null && !string.IsNullOrEmpty(collectons["SelectKeys"]))
            {
                List<int> productIDs = collectons["SelectKeys"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                List<VMProductInfo> products = _productServices.GetProductByIDs(CurrentUserServices.Me, productIDs);
                List<DTODictionary> dictionaries = _dictionaryServices.GetDictionaryInfos(CurrentUserServices.Me.UserID, null);
                List<DTODictionary> units = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.ProductUnit).ToList();
                List<DTODictionary> ports = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.OutPort).ToList();
                List<DTODictionary> packings = dictionaries.Where(p => p.TableKind == (int)DictionaryTableKind.Packing).ToList();

                if (products != null && dictionaries != null)
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    //基本信息
                    dataTable.Columns.Add("货号");
                    dataTable.Columns.Add("客户");
                    dataTable.Columns.Add("工厂货号");
                    dataTable.Columns.Add("工厂");
                    //产品信息
                    dataTable.Columns.Add("品名");
                    dataTable.Columns.Add("Product Description");
                    dataTable.Columns.Add("单位");
                    dataTable.Columns.Add("长/CM");
                    dataTable.Columns.Add("长/in");
                    dataTable.Columns.Add("宽/CM");
                    dataTable.Columns.Add("宽/in");
                    dataTable.Columns.Add("高/CM");
                    dataTable.Columns.Add("高/in");
                    dataTable.Columns.Add("产品重量KGS");
                    dataTable.Columns.Add("产品重量LBS");
                    //PDQ信息
                    dataTable.Columns.Add("PDQ装率");
                    dataTable.Columns.Add("PDQ长/CM");
                    dataTable.Columns.Add("PDQ长/in");
                    dataTable.Columns.Add("PDQ宽/CM");
                    dataTable.Columns.Add("PDQ宽/in");
                    dataTable.Columns.Add("PDQ高/CM");
                    dataTable.Columns.Add("PDQ高/in");
                    //内盒信息
                    dataTable.Columns.Add("内盒率");
                    dataTable.Columns.Add("内盒长/CM");
                    dataTable.Columns.Add("内盒长/in");
                    dataTable.Columns.Add("内盒宽/CM");
                    dataTable.Columns.Add("内盒宽/in");
                    dataTable.Columns.Add("内盒高/CM");
                    dataTable.Columns.Add("内盒高/in");
                    dataTable.Columns.Add("内盒材积Cuf");
                    dataTable.Columns.Add("内盒重量KGS");
                    dataTable.Columns.Add("内盒重量LBS");
                    //外箱信息
                    dataTable.Columns.Add("外箱率");
                    dataTable.Columns.Add("外箱长/CM");
                    dataTable.Columns.Add("外箱长/in");
                    dataTable.Columns.Add("外箱宽/CM");
                    dataTable.Columns.Add("外箱宽/in");
                    dataTable.Columns.Add("外箱高/CM");
                    dataTable.Columns.Add("外箱高/in");
                    dataTable.Columns.Add("外箱材积Cuf");
                    dataTable.Columns.Add("外箱毛重KGS");
                    dataTable.Columns.Add("外箱毛重LBS");
                    dataTable.Columns.Add("外箱净重KGS");
                    dataTable.Columns.Add("外箱净重LBS");
                    //价格信息
                    dataTable.Columns.Add("工厂价格");
                    dataTable.Columns.Add("币种");
                    dataTable.Columns.Add("出运港");
                    dataTable.Columns.Add("包装方式");
                    dataTable.Columns.Add("产品成分构成");
                    dataTable.Columns.Add("起订量");
                    dataTable.Columns.Add("备注");
                    //报价有效期
                    dataTable.Columns.Add("价格输入日期");
                    dataTable.Columns.Add("有效期");

                    foreach (var prod in products)
                    {
                        var unit = units.FirstOrDefault(p => p.Code == prod.UnitID);
                        var port = ports.FirstOrDefault(p => p.Code == prod.PortID);
                        var packing = packings.FirstOrDefault(p => p.Code == prod.PackingMannerZhID);
                        var dr = dataTable.NewRow();
                        dr["货号"] = prod.No;
                        dr["客户"] = prod.CustomerNo;
                        dr["工厂货号"] = prod.NoFactory;
                        dr["工厂"] = prod.FactoryName;
                        dr["品名"] = prod.Name;
                        dr["Product Description"] = prod.Desc;
                        dr["单位"] = unit == null ? string.Empty : unit.Name;
                        dr["长/CM"] = prod.Length;
                        dr["长/in"] = prod.LengthIN;
                        dr["宽/CM"] = prod.Width;
                        dr["宽/in"] = prod.WidthIN;
                        dr["高/CM"] = prod.Height;
                        dr["高/in"] = prod.HeightIN;
                        dr["产品重量KGS"] = prod.Weight;
                        dr["产品重量LBS"] = prod.WeightLBS;
                        dr["PDQ装率"] = prod.PDQPackRate;
                        dr["PDQ长/CM"] = prod.PDQLength;
                        dr["PDQ长/in"] = prod.PDQLengthIN;
                        dr["PDQ宽/CM"] = prod.PDQWidth;
                        dr["PDQ宽/in"] = prod.PDQWidthIN;
                        dr["PDQ高/CM"] = prod.PDQHeight;
                        dr["PDQ高/in"] = prod.PDQHeightIN;
                        dr["内盒率"] = prod.InnerBoxRate;
                        dr["内盒长/CM"] = prod.InnerLength;
                        dr["内盒长/in"] = prod.InnerLengthIN;
                        dr["内盒宽/CM"] = prod.InnerWidth;
                        dr["内盒宽/in"] = prod.InnerWidthIN;
                        dr["内盒高/CM"] = prod.InnerHeight;
                        dr["内盒高/in"] = prod.InnerHeightIN;
                        dr["内盒材积Cuf"] = prod.InnerVolume;
                        dr["内盒重量KGS"] = prod.InnerWeight;
                        dr["内盒重量LBS"] = prod.InnerWeightLBS;
                        dr["外箱率"] = prod.OuterBoxRate;
                        dr["外箱长/CM"] = prod.OuterLength;
                        dr["外箱长/in"] = prod.OuterLengthIN;
                        dr["外箱宽/CM"] = prod.OuterWidth;
                        dr["外箱宽/in"] = prod.OuterWidthIN;
                        dr["外箱高/CM"] = prod.OuterHeight;
                        dr["外箱高/in"] = prod.OuterHeightIN;
                        dr["外箱材积Cuf"] = prod.OuterVolume;
                        dr["工厂价格"] = prod.PriceFactory;
                        dr["币种"] = prod.CurrencyName;
                        dr["外箱毛重KGS"] = prod.OuterWeightGross;
                        dr["外箱毛重LBS"] = prod.OuterWeightGrossLBS;
                        dr["外箱净重KGS"] = prod.OuterWeightNet;
                        dr["外箱净重LBS"] = prod.OuterWeightNetLBS;
                        dr["出运港"] = port == null ? string.Empty : port.Name;
                        dr["包装方式"] = packing == null ? string.Empty : packing.Name;
                        dr["产品成分构成"] = prod.IngredientZh;
                        dr["起订量"] = prod.MOQZh;
                        dr["备注"] = prod.Comment;
                        dr["价格输入日期"] = prod.PriceInputDateFormat;
                        dr["有效期"] = prod.ValidDateFormat;

                        dataTable.Rows.Add(dr);
                    }

                    string sExportResult = ProductExportHelper.ExportDataToExcel(dataTable, "产品信息列表");
                    return new CustomJsonResult(new VMAjaxProcessResult() { IsSuccess = sExportResult.StartsWith("/"), Data = new { filepath = sExportResult } });
                }
            }

            return new CustomJsonResult(false);
        }

        public ActionResult Quotes()
        {
            VMProductSearchModel searchModel = new VMProductSearchModel()
            {
                PageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_QuoteProduct_Management),
                ProductNO = HttpContext.Request[QUERY_PRODUCTNO],
                CustomerNO = HttpContext.Request[QUERY_CUSTOMERNO],
                FactoryName = HttpContext.Request[QUERY_FACTORYNAME],
                Keyword = HttpContext.Request[QUERY_KEYWORD]
            };

            //用户设置，目前只有自定义列的功能
            var settings = _userService.GetUserCustomPageSettings(CurrentUserServices.Me.UserID, DatagridCustomColumnVisibilityModules.ProductManagement_QuoteProduct_Management);
            if (settings != null && !string.IsNullOrEmpty(settings.DatagridColumnVisibility))
            {
                ViewBag.ColumnsVisible = ((Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(settings.DatagridColumnVisibility)).ToObject(typeof(List<string>));
            }
            settings = _userService.GetUserCustomPageSettings(CurrentUserServices.Me.UserID, DatagridCustomColumnVisibilityModules.ProductManagement_ProductSelection);
            if (settings != null && !string.IsNullOrEmpty(settings.DatagridColumnVisibility))
            {
                ViewBag.ColumnsVisibleForSelection = ((Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(settings.DatagridColumnVisibility)).ToObject(typeof(List<string>));
            }

            return View(searchModel);
        }

        #endregion UserMethod

        #region PublicMethod

        /// <summary>
        /// 验证请求
        /// </summary>
        /// <returns></returns>
        private string ValidRequest()
        {
            if (CurrentUserServices.Me == null)
            {
                return "未登陆！";
            }
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)QuoteElementPrivileges.View) == 0)
            {
                return "没权限！";
            }
            return null;
        }

        public ActionResult SelectProduct()
        {
            int pageElementPrivileges = _userService.GetPageElementsPrivileges(CurrentUserServices.Me, (int)ERPPage.ProductManagement_Info);
            if ((pageElementPrivileges & (int)ProductListElementPrivileges.ViewProduct) == 0)
            {
                return RedirectToAction("NoAuthPop", "Account");
            }

            int currentPage = DTRequest.GetQueryInt(EasyuiPagenationConsts.CURRENT_PAGE, 1);
            int pageSize = DTRequest.GetQueryInt(EasyuiPagenationConsts.PAGE_SIZE, Keys.DEFAULT_PAGE_SIZE);
            int totalRows = 0;

            #region 排序

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES)))
            {
                var sortColumnsNamesTmp = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_NAMES).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // 需要替换转换过的属性
                foreach (var n in sortColumnsNamesTmp)
                {
                    sortColumnsNames.Add(ReplaceNames(n));
                }
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS)))
            {
                sortColumnOrders = DTRequest.GetQueryString(EasyuiPagenationConsts.SORT_COLUMN_ORDERS).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 排序

            #region 筛选条件

            string No = DTRequest.GetQueryString("No");
            string strCustomerID = DTRequest.GetQueryString("CustomerID");
            string FactoryName = DTRequest.GetQueryString("FactoryName");
            bool includeQuoteProducts = Utils.StrToBool(DTRequest.GetQueryString("IncludeQuoteProducts"), false);
            bool IsProductMixed = Utils.StrToBool(DTRequest.GetQueryString("IsProductMixed"), false);

            int? customerID = null;
            int tmp = 0;
            if (int.TryParse(strCustomerID, out tmp))
            {
                customerID = tmp;
            }

            List<string> productNOs = null;
            if (!string.IsNullOrEmpty(No))
            {
                productNOs = No.Split(new string[] { ";", "；", ",", "，", " ", Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            #endregion 筛选条件

            List<VMProductInfo> listModel = _productServices.SelectProduct(CurrentUserServices.Me, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows,
             productNOs, customerID, FactoryName, includeQuoteProducts,  IsProductMixed );

            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = listModel });
        }

        [HttpGet]
        public JsonResult GetProductInfoByQuote(VMProductInfoByQuote vm)
        {
            #region 添加权限

            string validMsg = ValidRequest();
            if (!string.IsNullOrEmpty(validMsg))
            {
                return Json(validMsg, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            List<VMProductInfo> list_vm = _productServices.GetProductInfoByQuote(CurrentUserServices.Me, vm);
            return Json(list_vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductInfoByOrder(string idList, int CustomerID, int PortID)
        {
            #region 添加权限

            string validMsg = ValidRequest();
            if (!string.IsNullOrEmpty(validMsg))
            {
                return Json(validMsg, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            List<int> idArray = null;
            if (!string.IsNullOrEmpty(idList))
            {
                idArray = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            List<VMProductInfo> list_vm = _productServices.GetProductInfoByOrder(CurrentUserServices.Me, idArray, CustomerID, PortID);
            return Json(list_vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductInfoByMixed(string idList)
        {
            #region 添加权限

            string validMsg = ValidRequest();
            if (!string.IsNullOrEmpty(validMsg))
            {
                return Json(validMsg, JsonRequestBehavior.AllowGet);
            }

            #endregion 添加权限

            List<int> idArray = null;
            if (!string.IsNullOrEmpty(idList))
            {
                idArray = idList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            }
            List<VMProductInfo> list_vm = _productServices.GetProductInfoByMixed(CurrentUserServices.Me, idArray);
            return Json(list_vm, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetFactoryCurrencyType(int FactoryID)
        {
            int? CurrencyType = _productServices.GetFactoryCurrencyType(FactoryID);
            return Json(CurrencyType, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSelectCustomer_SeasonList(int CustomerID)
        {
            var list = _productServices.GetSelectCustomer_SeasonList(CustomerID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDutyPercentList(int ID)
        {
            var list = _hSService.GetDutyPercentList(ID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion PublicMethod
    }
}