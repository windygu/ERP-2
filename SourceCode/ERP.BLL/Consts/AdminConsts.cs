using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Consts
{
    public static class AdminConsts
    {
        public const string SESSION_NAME_VALIDATE_CODE = "SESSION_NAME_VALIDATE_CODE";

        /// <summary>
        /// 最大递归深度
        /// </summary>
        public const int RECURSIVE_MAX_DEPTH = 100;

        public const string RESOURCE_SERVER_IMAGE_FOLDER_NAME = "Images";

        /// <summary>
        /// 上传附件的物理路径
        /// </summary>
        public const string RESOURCE_SERVER_CONFIGURATION_PHYSICSPATH = "resourceSitePhysicsPath";

        /// <summary>
        /// 上传附件的虚拟路径
        /// </summary>
        public const string RESOURCE_SERVER_CONFIGURATION_VIRTUALPATH = "resourceSiteVirtualPath";

        /// <summary>
        /// Email上传附件的物理路径
        /// </summary>
        public const string RESOURCE_SERVER_CONFIGURATION_PHYSICSPATH_EmailAttachs = "resourceSitePhysicsPath_EmailAttachs";

        public const string CLASSIFY_IMAGE_FOLDER_NAME = "classify";
        public const string PRODUCT_IMAGE_FOLDER_NAME = "product";
        public const string FILE_FOLDER_NAME = "file";

        public const string PRODUCT_EXPORT_PATH = "data/ExportFiles/";
    }
}