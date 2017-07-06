using ERP.BLL.ERP.Consts;
using ERP.BLL.ERP.Product;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.ProductManagement_Upload)]
    public class ProductUploadController : Controller
    {
        private ProductClassificationService _productClassificationServices = new ProductClassificationService();
        private ProductServices _productServices = new ProductServices();

        public ActionResult Classify()
        {
            return View(new DTOProductClassificationMoreInfo());
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Classify(IEnumerable<DTOBatchUploadProduct> allUploadedFiles)
        {
            int id = int.Parse(Request.QueryString["id"]);
            List<DTOBatchUploadProduct> list = new List<DTOBatchUploadProduct>();
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            List<string> list_Excel = new List<string>();
            DTOBatchUploadProduct dtobatch = new DTOBatchUploadProduct();
            string excelFileName = "";
            string excelName = "";

            foreach (var item in allUploadedFiles)
            {
                //得到文件名称
                string FileName = item.OldFileName;
                string[] arr = FileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                //得到路径
                string ImagePath = item.NewFilePath;

                if (arr.Last() == "xls" || arr.Last() == "xlsx")
                {
                    excelFileName = FileName;
                    excelName = arr.Last().ToString();

                    dtobatch.NewFilePath = item.NewFilePath;
                    dtobatch.OldFileName = item.OldFileName;
                    list_Excel.Add(FileName);
                }
                else
                {
                    DTOBatchUploadProduct dto = new DTOBatchUploadProduct();
                    dto.NewFilePath = item.NewFilePath;
                    dto.OldFileName = item.OldFileName;
                    list.Add(dto);
                }
            }

            if (list_Excel.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "请上传Excel文件！请重试！";

            }
            else if (list_Excel.Count == 1)
            {
                result = _productServices.BatchImport(CurrentUserServices.Me, id, excelFileName, excelName, list, dtobatch);
                result.Data = dtobatch.NewFilePath;
            }
            else
            {
                result.IsSuccess = false;
                result.Msg = "只能上传一个Excel文件！";
            }

            //result.IsSuccess =false;
            //result.Msg = "已经上传了文件";
            return Json(result);
        }
    }
}