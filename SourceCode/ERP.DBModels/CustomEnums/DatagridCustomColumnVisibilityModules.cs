using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum DatagridCustomColumnVisibilityModules
    {
        /// <summary>
        /// 产品信息管理
        /// </summary>
        ProductManagement_List,

        /// <summary>
        /// 报价产品管理
        /// </summary>
        ProductManagement_QuoteProduct_Management,

        /// <summary>
        /// 选择产品页面
        /// </summary>
        ProductManagement_ProductSelection,

        /// <summary>
        /// 财务管理，产品信息
        /// </summary>
        FinanceManagement_ProductList,

        /// <summary>
        /// 财务管理，工厂往来账
        /// </summary>
        FinanceManagement_Factory,

        /// <summary>
        /// 财务管理，利润分析
        /// </summary>
        FinanceManagement_Analysis,

        /// <summary>
        /// 财务管理，自营出口明细
        /// </summary>
        FinanceManagement_SelfExportList,

        /// <summary>
        /// 财务管理，订单明细
        /// </summary>
        FinanceManagement_DetailList,

        /// <summary>
        /// 混装产品信息管理
        /// </summary>
        ProductMixedManagement_List,

    }
}
