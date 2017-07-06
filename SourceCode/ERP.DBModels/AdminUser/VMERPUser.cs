using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class VMERPUser
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }

        public string Portrait { get; set; }

        public string Password { get; set; }

        public HierachyType HierachyType { get; set; }

        public int HierachyID { get; set; }

        public string HierachyName { get; set; }

        //public int GroupID { get; set; }
        //public int GroupLevel { get; set; }
        public bool IsSuperAdmin { get; set; }

        public AdminUserStatus Status { get; set; }

        public List<string> RoleNames { get; set; }

        public List<int> Roles { get; set; }

        public List<int> Customers { get; set; }

        public DataPermissions DataPermissionForCustomer { get; set; }

        public DataPermissions DataPermissionForFactory { get; set; }

        public DataPermissions DataPermissionForOrder { get; set; }

        public DataPermissions DataPermissionForOutsourcing { get; set; }

        public DataPermissions DataPermissionForPacks { get; set; }

        public DataPermissions DataPermissionForProduct { get; set; }

        public DataPermissions DataPermissionForPurchase { get; set; }

        public DataPermissions DataPermissionForQuote { get; set; }

        public DataPermissions DataPermissionForSample { get; set; }

        public DataPermissions DataPermissionForShipmentAgency { get; set; }

        public DataPermissions DataPermissionForDelivery { get; set; }

        public DataPermissions DataPermissionForProducePlan { get; set; }

        /// <summary>
        /// 单证管理->报检管理
        /// </summary>
        public DataPermissions ForInspectionReceipt { get; set; }

        /// <summary>
        /// 单证管理->报关管理
        /// </summary>
        public DataPermissions ForInspectionCustoms { get; set; }

        /// <summary>
        /// 单证管理->清关管理
        /// </summary>
        public DataPermissions ForInspectionClearance { get; set; }

        /// <summary>
        /// 单证管理->结汇管理
        /// </summary>
        public DataPermissions ForInspectionExchange { get; set; }

        /// <summary>
        /// 第三方检验
        /// </summary>
        public DataPermissions ForThirdParty { get; set; }

        /// <summary>
        /// 包装资料——唛头资料
        /// </summary>
        public DataPermissions ForShippingMark { get; set; }
        
        /// <summary>
        /// 单证索引
        /// </summary>
        public DataPermissions ForDocumentsIndexing { get; set; }
 
        /// <summary>
        /// 财务
        /// </summary>
        public DataPermissions ForFinance { get; set; }

        /// <summary>
        /// 邮件签名
        /// </summary>
        public string EmailSign { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string EmailPassword { get; set; }
    }
}