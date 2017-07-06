using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.ProductFitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Product
{
    public class VMProductInfo
    {
        public int ID { get; set; }

        public int ClassifyID { get; set; }

        public int FactoryID { get; set; }

        public string No { get; set; }

        public string NoFactory { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public int CustomerID { get; set; }

        public string Image { get; set; }

        public int UnitID { get; set; }

        public Nullable<decimal> Length { get; set; }

        public Nullable<decimal> LengthIN { get; set; }

        public Nullable<decimal> Height { get; set; }

        public Nullable<decimal> HeightIN { get; set; }

        public Nullable<decimal> Width { get; set; }

        public Nullable<decimal> WidthIN { get; set; }

        public int StyleID { get; set; }

        public Nullable<decimal> Weight { get; set; }

        public Nullable<decimal> WeightLBS { get; set; }

        public int PortID { get; set; }

        public int PackingMannerZhID { get; set; }

        public string IngredientZh { get; set; }

        public Nullable<int> MOQZh { get; set; }

        public Nullable<int> PackingMannerEnID { get; set; }

        public string IngredientEn { get; set; }

        public Nullable<int> PDQPackRate { get; set; }

        public Nullable<decimal> PDQLength { get; set; }

        public Nullable<decimal> PDQLengthIN { get; set; }

        public Nullable<decimal> PDQWidth { get; set; }

        public Nullable<decimal> PDQWidthIN { get; set; }

        public Nullable<decimal> PDQHeight { get; set; }

        public Nullable<decimal> PDQHeightIN { get; set; }

        public Nullable<int> InnerBoxRate { get; set; }

        public Nullable<decimal> InnerLength { get; set; }

        public Nullable<decimal> InnerLengthIN { get; set; }

        public Nullable<decimal> InnerWidth { get; set; }

        public Nullable<decimal> InnerWidthIN { get; set; }

        public Nullable<decimal> InnerHeight { get; set; }

        public Nullable<decimal> InnerHeightIN { get; set; }

        /// <summary>
        /// 内盒毛重
        /// </summary>
        public Nullable<decimal> InnerWeight { get; set; }

        /// <summary>
        /// 内盒毛重LBS
        /// </summary>
        public Nullable<decimal> InnerWeightLBS { get; set; }

        public Nullable<decimal> InnerVolume { get; set; }

        public Nullable<int> OuterBoxRate { get; set; }

        public Nullable<decimal> OuterLength { get; set; }

        public Nullable<decimal> OuterLengthIN { get; set; }

        public Nullable<decimal> OuterWidth { get; set; }

        public Nullable<decimal> OuterWidthIN { get; set; }

        public Nullable<decimal> OuterHeight { get; set; }

        public Nullable<decimal> OuterHeightIN { get; set; }

        public Nullable<decimal> OuterVolume { get; set; }

        /// <summary>
        /// 外箱毛重
        /// </summary>
        public Nullable<decimal> OuterWeightGross { get; set; }

        public Nullable<decimal> OuterWeightGrossLBS { get; set; }

        /// <summary>
        /// 外箱净重
        /// </summary>
        public Nullable<decimal> OuterWeightNet { get; set; }

        public Nullable<decimal> OuterWeightNetLBS { get; set; }

        public Nullable<decimal> PriceFactory { get; set; }

        public Nullable<decimal> FOBFTY { get; set; }

        public Nullable<decimal> FOBNET { get; set; }

        public Nullable<decimal> FOBC5 { get; set; }

        public Nullable<decimal> FOBChinaPort { get; set; }

        public Nullable<decimal> Rate { get; set; }

        public Nullable<decimal> FinalFOB { get; set; }

        public Nullable<decimal> PcsPallet { get; set; }

        /// <summary>
        /// Pallet $/pc = 客户信息中的Pallet价格/70*外箱材积/外箱率，进行四舍五入
        /// </summary>
        public Nullable<decimal> PalletPc { get; set; }

        public Nullable<decimal> Duty { get; set; }

        public Nullable<decimal> FOBChinaLCL { get; set; }

        public Nullable<decimal> Cost { get; set; }

        /// <summary>
        /// Commission%
        /// </summary>
        public Nullable<decimal> Commission { get; set; }

        public Nullable<decimal> CommissionAmount { get; set; }

        public Nullable<decimal> MiscImportLoadAmount { get; set; }

        /// <summary>
        /// MiscImportLoad%
        /// </summary>
        public Nullable<decimal> MiscImportLoad { get; set; }

        public Nullable<decimal> Agent { get; set; }

        public Nullable<decimal> SRP { get; set; }

        public Nullable<decimal> CtnsPallet { get; set; }

        /// <summary>
        /// 取HSCode对应的表HarmonizedSystem中的税率
        /// </summary>
        public Nullable<decimal> DutyPercent { get; set; }

        public Nullable<decimal> FreightRate { get; set; }

        public string Remarks { get; set; }

        public string Comment { get; set; }

        public Nullable<System.DateTime> PriceInputDate { get; set; }

        public Nullable<System.DateTime> ValidDate { get; set; }

        public Nullable<int> MOQEn { get; set; }

        public Nullable<bool> Deleted { get; set; }

        public string UPC { get; set; }

        public int CurrencyType { get; set; }

        public Nullable<System.DateTime> DT_CREATEDATE { get; set; }

        public Nullable<int> ST_CREATEUSER { get; set; }

        public string DT_MODIFYDATE { get; set; }

        public Nullable<int> ST_MODIFYUSER { get; set; }

        public string IPAddress { get; set; }

        public Nullable<decimal> Allowance { get; set; }

        // 通过计算所得列 报价单用到了，暂时不注释
        public decimal FOBUS { get; set; }

        public decimal DDP { get; set; }

        public decimal POE { get; set; }

        public decimal MU { get; set; }

        public decimal AgentAmount { get; set; }

        public decimal ELC { get; set; }

        public decimal Retail { get; set; }

        public decimal Freight { get; set; }

        public string FactoryName { get; set; }

        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string StyleName { get; set; }

        public string StyleNumber { get; set; }

        public string PortName { get; set; }

        public string PortEnName { get; set; }

        public string PackingMannerZhName { get; set; }

        public string PackingMannerEnName { get; set; }

        public string UnitName { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySign { get; set; }

        public int ProductID { get; set; }

        public int TermsID { get; set; }

        public string PriceInputDateFormat { get; set; }

        public string ValidDateFormat { get; set; }

        /// <summary>
        /// 海关编码ID
        /// </summary>
        public Nullable<int> HTS { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HTS_Name { get; set; }

        /// <summary>
        /// 报关编码ID
        /// </summary>
        public Nullable<int> HSCode { get; set; }

        /// <summary>
        /// 报关编码
        /// </summary>
        public string HSCode_Name { get; set; }

        public int? ParentProductID { get; set; }

        public string ParentNo { get; set; }

        public int? RootProductID { get; set; }

        public string RootNo { get; set; }

        public ProductStatusEnum Status { get; set; }

        /// <summary>
        /// 数据记录对应的用户部门
        /// </summary>
        public int UserHierachyID { get; set; }

        #region 销售订单的计算列

        public decimal Ctns { get; set; }

        public decimal? TotalCuft { get; set; }

        public decimal? TotalCBM { get; set; }

        public decimal? MUPercent { get; set; }

        public string SkuNumber { get; set; }

        public int Qty { get; set; }

        public int Qty2 { get; set; }

        public decimal SalePrice { get; set; }

        public decimal SumSalePrice { get; set; }

        public decimal NetPrice { get; set; }

        public decimal SumNetPrice { get; set; }

        public decimal SumPriceFactory { get; set; }

        public decimal RetailPrice { get; set; }

        public int ProductFrom { get; set; }

        public string PercentSign { get; set; }

        public string PriceFactoryFormatter { get; set; }

        public string SumPriceFactoryFormatter { get; set; }

        public string RateFormatter { get; set; }

        #endregion 销售订单的计算列

        /// <summary>
        /// 产品版权
        /// </summary>
        public int? ProductCopyRight { get; set; }

        /// <summary>
        /// 客户的报价单模板
        /// </summary>
        public string QuoteTemplateFileName { get; set; }

        /// <summary>
        /// ELC补差%
        /// </summary>
        public decimal? ELCFill { get; set; }

        public string DutyPercentFormatter { get; set; }

        public string CommissionFormatter { get; set; }

        public string MiscImportLoadFormatter { get; set; }

        public bool IsNeedInspection { get; set; }

        public int? Season { get; set; }
        public string SeasonName { get; set; }

        /// <summary>
        /// 批量上传的总数量
        /// </summary>
        public int AllCount { get; set; }
        /// <summary>
        /// 颜色编号
        /// </summary>
        public int? ColorID { get; set; }

        /// <summary>
        /// 颜色名称
        /// </summary>
        public string ColorName { get; set; }

        public bool? createAsQuote { get; set; }

        public string extraNo { get; set; }

        public int? extraCustomerID { get; set; }

        /// <summary>
        /// 产品成分构成列表
        /// </summary>
        public List<VMProductIngredients> list_ProductIngredient { get; set; }

        /// <summary>
        /// 内盒净重
        /// </summary>
        public decimal? InnerWeightGross { get; set; }

        /// <summary>
        /// 内盒净重LBS
        /// </summary>
        public decimal? InnerWeightGrossLBS { get; set; }

        public string SkuCode { get; set; }
        public int? FactoryID_ForQuote { get; set; }

        public int? INR { get; set; }
        public string TypeOfWood { get; set; }
        public string OfStoreReadyInnersEnclosed { get; set; }
        public string PreTicketed { get; set; }
        public int? Department { get; set; }

        /// <summary>
        /// 产品配件列表
        /// </summary>
        public List<VMProductFittingInfo> list_ProductFitting { get; set; }

        /// <summary>
        /// 是否有配件产品
        /// </summary>
        public bool IsProductFitting { get; set; }
        public string IsProductFittingFormatter { get; set; }

        public bool IsProductMixed { get; set; }
        public int? ParentProductMixedID { get; set; }
        public List<VMProductInfo> list_ProductMixed { get; set; }
        public string ProductAmountFormatter { get; set; }
        public string IsNeedInspectionName { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal CurrencyExchangeUSD { get; set; }
        public decimal CurrencyExchangeRMB { get; set; }

        public int QuotProductID { get; set; }
    }

}