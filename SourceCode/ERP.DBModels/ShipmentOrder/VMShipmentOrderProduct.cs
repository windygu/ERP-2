using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ShipmentOrder
{
    /// <summary>
    /// 订舱管理——产品
    /// </summary>
    public class VMShipmentOrderProduct
    {
        #region Model

        /// <summary>
        /// 订舱柜型编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 订舱箱柜编号
        /// </summary>
        public int ShipmentOrderCabinetID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// OrderProductID
        /// </summary>
        public int OrderProductID { get; set; }

        #endregion Model

        public string No { get; set; }

        public string SkuNumber { get; set; }

        public string FactoryAbbreviation { get; set; }

        public string Desc { get; set; }

        public decimal? OuterBoxRate { get; set; }

        public decimal? OuterLength { get; set; }

        public decimal? OuterWidth { get; set; }

        /// <summary>
        /// 单箱毛重
        /// </summary>
        public decimal? OuterWeightGross { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? SumOuterWeightGross { get; set; }

        /// <summary>
        /// 单箱净重
        /// </summary>
        public decimal? OuterWeightNet { get; set; }

        /// <summary>
        /// 总净重
        /// </summary>
        public decimal? SumOuterWeightNet { get; set; }

        public int? InnerBoxRate { get; set; }

        public decimal? OuterHeight { get; set; }

        public int ProductID { get; set; }

        public string Image { get; set; }

        /// <summary>
        /// 外箱材积
        /// </summary>
        public decimal? OuterVolume { get; set; }

        /// <summary>
        /// 总体积
        /// </summary>
        public decimal? SumOuterVolume { get; set; }

        /// <summary>
        /// 总出运箱数
        /// </summary>
        public decimal? SumBoxQty { get; set; }

        /// <summary>
        /// 可订舱箱数
        /// </summary>
        public int? RemainedBoxQty { get; set; }

        /// <summary>
        /// 当前订舱箱数
        /// </summary>
        public int? SelectBoxQty { get; set; }

        /// <summary>
        /// 当前订舱体积
        /// </summary>
        public decimal? SelectVolume { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HTS { get; set; }

        public int OrderID { get; set; }
        public string OrderNumber { get; set; }

        /// <summary>
        /// 当前订舱的产品数量
        /// </summary>
        public int? SelectQty { get; set; }

        public string HSCode { get; set; }
        public decimal SumPrice { get; set; }

        /// <summary>
        /// 产品单价
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 英文报关品名
        /// </summary>
        public string HsEngName { get; set; }

        public string SeasonPrefix { get; set; }
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 是否需要报检
        /// </summary>
        public bool IsNeedInspection { get; set; }

        /// <summary>
        /// 是否需要报检
        /// </summary>
        public string IsNeedInspectionName { get; set; }
        public string CurrencyName { get; set; }
        public decimal SelectSumOuterWeightGross { get; set; }
        public decimal SelectSumOuterWeightNet { get; set; }
        public string SCNo { get; set; }
        public string InvoiceNo { get; set; }
        public int? HSID { get; set; }
        public string HSCodeName { get; set; }

        public string list_UploadReceipt_ID { get; set; }
        public string HSCodeEngName { get; set; }
        public bool IsProductMixed { get; set; }
    }
}