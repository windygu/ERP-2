using ERP.BLL.ERP.Product;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.ProductManagement_Classification)]
    public class ProductClassificationController : Controller
    {
        private ProductClassificationService _productClassificationServices = new ProductClassificationService();

        public ActionResult Classify()
        {
            return View(new DTOProductClassificationMoreInfo());
        }

        [AllowAnonymous]
        public ActionResult Get(string pID, bool? show)
        {
            List<DTOProductClassifications> productClassifications = new List<DTOProductClassifications>();
            if (pID == "#")
            {
                DTOProductClassifications defaultClassification = new DTOProductClassifications()
                {
                    ID = -1,
                    Children = true,
                    Text = "分类列表",
                    Icon = "fa fa-folder-o"
                };
                productClassifications.Add(defaultClassification);
            }
            else
            {
                int classificationID = 0;
                int.TryParse(pID, out classificationID);

                productClassifications = _productClassificationServices.GetAll(CurrentUserServices.Me, classificationID <= 0 ? (int?)null : classificationID, show);
                foreach (var c in productClassifications)
                {
                    c.Icon = (c.Children ? "fa fa-folder" : "fa fa-folder-o");
                }
            }
            return new CustomJsonResult(productClassifications);
        }

        [AllowAnonymous]
        public ActionResult MoreInfo(int ID, bool? show)
        {
            DTOProductClassificationMoreInfo moreInfo = _productClassificationServices.GetMoreInfo(CurrentUserServices.Me, ID, show);
            return new CustomJsonResult(moreInfo);
        }

        public ActionResult Update([System.Web.Http.FromBody]DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);
            result = _productClassificationServices.IsNameExist(CurrentUserServices.Me, model.ID, model.Name, model.ParentID);
            if (result != DBOperationStatus.ExistingRecord)
            {
                result = _productClassificationServices.Update(CurrentUserServices.Me, model);
            }
            return new CustomJsonResult((short)result);
        }

        public ActionResult Create([System.Web.Http.FromBody]DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);
            result = _productClassificationServices.IsNameExist(CurrentUserServices.Me, model.ID, model.Name, model.ParentID);
            if (result != DBOperationStatus.ExistingRecord)
            {
                int id = _productClassificationServices.Create(CurrentUserServices.Me, model);
                return new CustomJsonResult(id);
            }
            else
            {
                return new CustomJsonResult(-1);
            }
        }

        public ActionResult Rename([System.Web.Http.FromBody]DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);
            result = _productClassificationServices.IsNameExist(CurrentUserServices.Me, model.ID, model.Name, model.ParentID);
            if (result != DBOperationStatus.ExistingRecord)
            {
                result = _productClassificationServices.Rename(CurrentUserServices.Me, model);
            }
            return new CustomJsonResult((short)result);
        }

        public ActionResult Move([System.Web.Http.FromBody]DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = _productClassificationServices.Move(CurrentUserServices.Me, model);
            return new CustomJsonResult((short)result);
        }

        public ActionResult Delete([System.Web.Http.FromBody]DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = _productClassificationServices.Delete(CurrentUserServices.Me, model);
            return new CustomJsonResult((short)result);
        }

        public ActionResult ViewProductList(int id)
        {
            List<VMViewProductList> list_vm = _productClassificationServices.GetViewProductList(CurrentUserServices.Me, id);
            ViewBag.id = id;
            return View(list_vm);
        }

        public JsonResult IsExitViewProductList(int id)
        {
            List<VMViewProductList> list_vm = _productClassificationServices.GetViewProductList(CurrentUserServices.Me, id);
            return Json(list_vm.Count);
        }
    }
}