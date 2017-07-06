using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionCustoms
{
    public class VMOrderProducts
    {
        public int ID { get; set; }

        /// <summary>
        /// 报检自编号
        /// </summary>
        public int HSID { get; set; }

        /// <summary>
        /// 报关编码
        /// </summary>
        public string HsCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string HsName { get; set; }

        /// <summary>
        /// 报关英文品名
        /// </summary>
        public string HsEngName { get; set; }

        /// <summary>
        /// 产品箱数=产品数量/外箱率
        /// </summary>
        public int ProductsBoxNum { get; set; }

        /// <summary>
        /// 总毛重=单箱毛重×产品箱数
        /// </summary>
        public decimal SumOuterWeightGross { get; set; }

        /// <summary>
        /// 总净重=单箱净重×总箱数
        /// </summary>
        public decimal SumOuterWeightNet { get; set; }

        /// <summary>
        /// 产品体积
        /// </summary>
        public decimal? SumOuterVolume { get; set; }

        #region 报关的产品

        /// <summary>
        /// 报关的产品：单价
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 报关的产品：数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 报关的产品：金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 报关的产品：外箱率
        /// </summary>
        public int? OuterBoxRate { get; set; }

        #endregion 报关的产品

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
    }
}