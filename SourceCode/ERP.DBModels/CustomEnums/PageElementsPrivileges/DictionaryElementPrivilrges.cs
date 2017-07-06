using ERP.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums.PageElementsPrivileges
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public enum DictionaryElementPrivilrges
    {
        /// <summary>
        /// 批量删除
        /// </summary>
        [Description("批量删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_Dictionaries })]
        BatchDelete = 1 << 0,

        ///<summary>
        ///添加
        ///</summary>
        [Description("添加")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_Dictionaries })]
        Add = 1 << 1,

        ///<summary>
        ///编辑
        ///</summary>
        [Description("编辑")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_Dictionaries })]
        Edit = 1 << 2,

        ///<summary>
        ///删除
        ///</summary>
        [Description("删除")]
        [PageElementImpactOnPages(Pages = new[] { ERPPage.SystemManagement_Dictionaries })]
        Delete = 1 << 3,
    }
}