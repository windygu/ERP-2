using ERP.BLL.Consts;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.Tools;
using ERP.Tools.Logs;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    public class FileUploaderController : Controller
    {
        private static List<string> _validExtension = new List<string>();

        static FileUploaderController()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["validUploadFileExtensions"]))
            {
                _validExtension = ConfigurationManager.AppSettings["validUploadFileExtensions"].Split(new char[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        private string SaveFile(UploadFileType fileType, string subFolderName, string fileName)
        {
            return SaveFile(fileType, subFolderName, fileName, AdminConsts.RESOURCE_SERVER_CONFIGURATION_PHYSICSPATH);
        }

        private string SaveFile(UploadFileType fileType, string subFolderName, string fileName, string resourceSitePhysicsPath)
        {
            DateTime dtNow = DateTime.Now;

            // 过滤文件
            string extension = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (_validExtension.Contains(extension.ToLower()))
            {
                string fileFolderName = fileType.ToString();
                if (fileType != UploadFileType.Images)
                {
                    if (string.IsNullOrEmpty(subFolderName))
                    {
                        subFolderName = AdminConsts.FILE_FOLDER_NAME;
                    }
                }

                string savePath = string.Format("{0}\\{1}\\{2}\\{3}\\{4}\\",
                    ConfigurationManager.AppSettings[resourceSitePhysicsPath],
                    fileFolderName,
                    subFolderName,
                    dtNow.Year.ToString(),
                    dtNow.ToString("MM-dd"));

                if (!System.IO.Directory.Exists(savePath))
                {
                    System.IO.Directory.CreateDirectory(savePath);
                }
                var file = HttpContext.Request.Files[0];
                string uniqueFileName = Guid.NewGuid() + fileName.Substring(fileName.LastIndexOf("."));
                file.SaveAs(savePath + uniqueFileName);
                return string.Format("{0}/{1}/{2}/{3}/{4}/{5}",
                    ConfigurationManager.AppSettings[AdminConsts.RESOURCE_SERVER_CONFIGURATION_VIRTUALPATH],
                    fileFolderName,
                    subFolderName,
                    dtNow.Year.ToString(),
                    dtNow.ToString("MM-dd"),
                    uniqueFileName);
            }
            else
            {
                return string.Empty;
            }
        }

        public ActionResult ClassificationImg()
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                string fileName = Utils.UrlDecode(HttpContext.Request.Headers["X_FILENAME"]);
                string path = SaveFile(UploadFileType.Images, AdminConsts.CLASSIFY_IMAGE_FOLDER_NAME, fileName);
                result.IsSuccess = true;
                result.Data = new { imgPath = path };
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return new CustomJsonResult(result);
        }

        public ActionResult ProductImg()
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                string fileName = Utils.UrlDecode(HttpContext.Request.Headers["X_FILENAME"]);
                string path = SaveFile(UploadFileType.Images, AdminConsts.PRODUCT_IMAGE_FOLDER_NAME, fileName);
                result.IsSuccess = true;
                result.Data = new { imgPath = path };
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return new CustomJsonResult(result);
        }

        public ActionResult ProductByBatchImg()
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                string fileName = HttpContext.Request.Files["Filedata"].FileName;
                string path = SaveFile(UploadFileType.Images, AdminConsts.PRODUCT_IMAGE_FOLDER_NAME, fileName);
                result.IsSuccess = true;
                result.Data = new DTOBatchUploadProduct() { OldFileName = fileName, NewFilePath = path };
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return new CustomJsonResult(result);
        }

        public ActionResult UploadFiles(UploadFileType type, string id)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                string fileName = HttpContext.Request.Files["Filedata"].FileName;
                string path = SaveFile(type, id, fileName);

                result.IsSuccess = true;
                result.Data = new DTOBatchUploadProduct() { OldFileName = fileName, NewFilePath = path };
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return new CustomJsonResult(result);
        }

        //public ActionResult EmailUploadFiles(UploadFileType type, string id)
        //{
        //    VMAjaxProcessResult result = new VMAjaxProcessResult();
        //    try
        //    {
        //        string fileName = HttpContext.Request.Files["Filedata"].FileName;
        //        string path = SaveFile(type, id, fileName, AdminConsts.RESOURCE_SERVER_CONFIGURATION_PHYSICSPATH_EmailAttachs);
        //        result.IsSuccess = true;
        //        result.Data = new DTOBatchUploadProduct() { OldFileName = fileName, NewFilePath = path };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.WriteError(ex);
        //    }
        //    return new CustomJsonResult(result);
        //}


        public ActionResult PacksImg(UploadFileType type, string id)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            try
            {
                string fileName = Utils.UrlDecode(HttpContext.Request.Headers["X_FILENAME"]);
                string path = SaveFile(type, id, fileName);

                result.IsSuccess = true;
                result.Data = new { ServerFileName = path, DisplayFileName = fileName };
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return new CustomJsonResult(result);
        }
    }
}