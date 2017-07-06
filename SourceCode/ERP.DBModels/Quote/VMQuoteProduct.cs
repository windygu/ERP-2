using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Models.Product;

namespace ERP.Models.Quote
{
    public class VMQuoteProduct
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// QuotID
        /// </summary>
        public int QuotID { get; set; }

        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 工厂名
        /// </summary>
        public string NoFactory { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerID { get; set; }

        /// <summary>
        /// 工厂编号
        /// </summary>
        public int? FactoryID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public int? UnitID { get; set; }

        /// <summary>
        /// 长
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public decimal? Width { get; set; }

        /// <summary>
        /// 款式id
        /// </summary>
        public int? StyleID { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 港口id
        /// </summary>
        public int? PortID { get; set; }

        /// <summary>
        /// 中文包装方式
        /// </summary>
        public int? PackingMannerZhID { get; set; }

        /// <summary>
        /// 中文成分
        /// </summary>
        public string IngredientZh { get; set; }

        /// <summary>
        /// MOQ
        /// </summary>
        public int? MOQZh { get; set; }

        /// <summary>
        /// 英文包装方式
        /// </summary>
        public int? PackingMannerEnID { get; set; }

        /// <summary>
        /// 英文成分
        /// </summary>
        public string IngredientEn { get; set; }

        /// <summary>
        /// PDQ装箱率
        /// </summary>
        public int? PDQPackRate { get; set; }

        /// <summary>
        /// PDQ长
        /// </summary>
        public decimal? PDQLength { get; set; }

        /// <summary>
        /// PDQ宽
        /// </summary>
        public decimal? PDQWidth { get; set; }

        /// <summary>
        /// PDQ高
        /// </summary>
        public decimal? PDQHeight { get; set; }

        /// <summary>
        /// 内盒率
        /// </summary>
        public int? InnerBoxRate { get; set; }

        /// <summary>
        /// 内盒长
        /// </summary>
        public decimal? InnerLength { get; set; }

        /// <summary>
        /// 内盒宽
        /// </summary>
        public decimal? InnerWidth { get; set; }

        /// <summary>
        /// 内盒高
        /// </summary>
        public decimal? InnerHeight { get; set; }

        /// <summary>
        /// 内盒重量
        /// </summary>
        public decimal? InnerWeight { get; set; }

        /// <summary>
        /// 外箱率
        /// </summary>
        public int? OuterBoxRate { get; set; }

        /// <summary>
        /// 外箱长
        /// </summary>
        public decimal? OuterLength { get; set; }

        /// <summary>
        /// 外箱宽
        /// </summary>
        public decimal? OuterWidth { get; set; }

        /// <summary>
        /// 外箱高
        /// </summary>
        public decimal? OuterHeight { get; set; }

        /// <summary>
        /// 工厂价格
        /// </summary>
        public decimal? PriceFactory { get; set; }

        /// <summary>
        /// MiscImportLoad
        /// </summary>
        public decimal? MiscImportLoad { get; set; }

        /// <summary>
        /// Agent
        /// </summary>
        public decimal? Agent { get; set; }

        /// <summary>
        /// SRP
        /// </summary>
        public decimal? SRP { get; set; }

        /// <summary>
        /// CtnsPallet
        /// </summary>
        public decimal? CtnsPallet { get; set; }

        /// <summary>
        /// DutyPercent。不可修改。取HS CODE里面的税率。
        /// </summary>
        public decimal? DutyPercent { get; set; }

        /// <summary>
        /// FreightRate 取客户资料的值
        /// </summary>
        public decimal? FreightRate { get; set; }

        /// <summary>
        /// 备注（英文）
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 备注（中文）
        /// </summary>
        public string Comment { get; set; }

        ///// <summary>
        ///// 价格输入日期
        ///// </summary>
        //public DateTime PriceInputDate { get; set; }

        ///// <summary>
        ///// 有效期
        ///// </summary>
        //public DateTime ValidDate { get; set; }

        /// <summary>
        /// MOQEn
        /// </summary>
        public int? MOQEn { get; set; }

        /// <summary>
        /// CurrencyType
        /// </summary>
        public int? CurrencyType { get; set; }

        /// <summary>
        /// LengthIN
        /// </summary>
        public decimal? LengthIN { get; set; }

        /// <summary>
        /// HeightIN
        /// </summary>
        public decimal? HeightIN { get; set; }

        /// <summary>
        /// WidthIN
        /// </summary>
        public decimal? WidthIN { get; set; }

        /// <summary>
        /// WeightLBS
        /// </summary>
        public decimal? WeightLBS { get; set; }

        /// <summary>
        /// PDQLengthIN
        /// </summary>
        public decimal? PDQLengthIN { get; set; }

        /// <summary>
        /// PDQWidthIN
        /// </summary>
        public decimal? PDQWidthIN { get; set; }

        /// <summary>
        /// PDQHeightIN
        /// </summary>
        public decimal? PDQHeightIN { get; set; }

        /// <summary>
        /// InnerLengthIN
        /// </summary>
        public decimal? InnerLengthIN { get; set; }

        /// <summary>
        /// InnerWidthIN
        /// </summary>
        public decimal? InnerWidthIN { get; set; }

        /// <summary>
        /// InnerHeightIN
        /// </summary>
        public decimal? InnerHeightIN { get; set; }

        /// <summary>
        /// InnerVolume
        /// </summary>
        public decimal? InnerVolume { get; set; }

        /// <summary>
        /// InnerWeightLBS
        /// </summary>
        public decimal? InnerWeightLBS { get; set; }

        /// <summary>
        /// OuterLengthIN
        /// </summary>
        public decimal? OuterLengthIN { get; set; }

        /// <summary>
        /// OuterWidthIN
        /// </summary>
        public decimal? OuterWidthIN { get; set; }

        /// <summary>
        /// OuterHeightIN
        /// </summary>
        public decimal? OuterHeightIN { get; set; }

        /// <summary>
        /// 外箱材积
        /// </summary>
        public decimal? OuterVolume { get; set; }

        /// <summary>
        /// 外箱毛重LBS
        /// </summary>
        public decimal? OuterWeightGrossLBS { get; set; }

        /// <summary>
        /// 外箱净重LBS
        /// </summary>
        public decimal? OuterWeightNetLBS { get; set; }

        /// <summary>
        /// Fobfty($) 。当工厂价格是美金报价，Fobfty=工厂价格。当工厂价格是人民币报价，Fobfty=工厂价格/汇率。
        /// </summary>
        public decimal? FOBFTY { get; set; }

        /// <summary>
        /// FOBNET
        /// </summary>
        public decimal? FOBNET { get; set; }

        /// <summary>
        /// FinalFOB
        /// </summary>
        public decimal? FinalFOB { get; set; }

        /// <summary>
        /// Rate 。如果产品的工厂价格是人民币，取人民币的换汇。如果产品的工厂价格是美元，取美元的换汇。
        /// </summary>
        public decimal? Rate { get; set; }

        /// <summary>
        /// Cost
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// FOBChinaLCL
        /// </summary>
        public decimal? FOBChinaLCL { get; set; }

        /// <summary>
        /// Fobus
        /// </summary>
        public decimal? FOBUS { get; set; }

        /// <summary>
        /// Ddp
        /// </summary>
        public decimal? DDP { get; set; }

        /// <summary>
        /// Poe
        /// </summary>
        public decimal? POE { get; set; }

        /// <summary>
        /// PcsPallet
        /// </summary>
        public decimal? PcsPallet { get; set; }

        /// <summary>
        /// PalletPc
        /// </summary>
        public decimal? PalletPc { get; set; }

        /// <summary>
        /// Duty。Duty Amount = FOB FTY*DUTY%(当terms选择DDP/POE/FOB US时用此公式)
        ///                   = FOB*duty%(当terms选择FOB/FOB CHINA LCL时用此公式)
        ///                   duty amount 不可修改，标题增加$符号
        /// </summary>
        public decimal? Duty { get; set; }

        /// <summary>
        /// Freight  Freight Amount = Freight rate*外箱材积/外箱率，不可修改，标题增加$符号
        /// </summary>
        public decimal? Freight { get; set; }

        /// <summary>
        /// Commission
        /// </summary>
        public decimal? Commission { get; set; }

        /// <summary>
        /// CommissionAmount。Commission Amount=FOB CN*Commission%
        /// </summary>
        public decimal? CommissionAmount { get; set; }

        /// <summary>
        /// MiscImportLoadAmount
        /// </summary>
        public decimal? MiscImportLoadAmount { get; set; }

        /// <summary>
        /// AgentAmount
        /// </summary>
        public decimal? AgentAmount { get; set; }

        /// <summary>
        /// Elc。ELC = FOB FTY + DUTY AMOUNT + FREIGHT AMOUNT(当terms选择DDP/POE/FOB US时用此公式)
        ///      ELC = FOB + DUTY AMOUNT + FREIGHT AMOUNT(当terms选择FOB/FOB CHINA LCL时用此公式)
        /// </summary>
        public decimal? ELC { get; set; }

        /// <summary>
        /// Retail  Retail = ELC/(1-mu%)
        /// </summary>
        public decimal? Retail { get; set; }

        /// <summary>
        /// Mu mu%取客户资料
        /// </summary>
        public decimal? MU { get; set; }

        /// <summary>
        /// UPC
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// Allowance
        /// </summary>
        public decimal? Allowance { get; set; }

        /// <summary>
        /// 5种价格的类型
        /// FOB = 工厂价格（￥）/人民币换汇/客户资料commission%/客户资料allowance%
        /// FOB = 工厂价格（$）/(1-美元换汇)/客户资料commission%/客户资料allowance%
        /// FOB CHINA LCL公式与FOB公式一样
        /// FOB US = ELC
        /// DDP = (FOB FTY + DUTY AMOUNT + FREIGHT AMOUNT) / 换汇 / Commission% / Allowance%       (比较常用）
        /// POE = (FOB FTY + DUTY AMOUNT + FREIGHT AMOUNT) / 换汇 / Commission% / Allowance%       (比较常用）
        /// </summary>
        public int? TermsID { get; set; }

        /// <summary>
        /// PortEnID
        /// </summary>
        public int? PortEnID { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        public string CustomerNo { get; set; }

        public string CustomerName { get; set; }

        public string FactoryName { get; set; }

        public string StyleName { get; set; }

        public string StyleNumber { get; set; }

        public string PortEnName { get; set; }

        public string PortName { get; set; }

        public string PackingMannerZhName { get; set; }

        public string PackingMannerEnName { get; set; }

        public string UnitName { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySign { get; set; }

        public string DateFormattedPriceInputDate { get; set; }

        public string DateFormattedValidDate { get; set; }

        public string DateFormattedDT_MODIFYDATE { get; set; }

        public decimal FOBChinaPort { get; set; }

        /// <summary>
        /// 海关编码ID
        /// </summary>
        public int? HTS { get; set; }

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

        public DateTime? ValidDate { get; set; }

        public DateTime? PriceInputDate { get; set; }

        /// <summary>
        /// 外箱毛重
        /// </summary>
        public decimal? OuterWeightGross { get; set; }

        /// <summary>
        /// 外箱净重
        /// </summary>
        public decimal? OuterWeightNet { get; set; }

        #region 销售订单的计算列

        public decimal Ctns { get; set; }

        public decimal? TotalCuft { get; set; }

        public decimal? TotalCBM { get; set; }

        public decimal? MUPercent { get; set; }

        public string SkuNumber { get; set; }

        public int Qty { get; set; }

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
        /// 购买者。取客户管理 Buyer = LAST NAME + 空格 + FIRST NAME（如果该客户的联系人只有1条数据时，取该联系人的Buyer。如果该客户的联系人有多条数据时，取默认联系人的Buyer）
        /// </summary>
        public string Buyer { get; set; }

        /// <summary>
        /// 报价单模板
        /// </summary>
        public string QuoteTemplateFileName { get; set; }

        /// <summary>
        /// ELC补差%
        /// </summary>
        public decimal? ELCFill { get; set; }

        public string DutyPercentFormatter { get; set; }

        public string CommissionFormatter { get; set; }

        public string MiscImportLoadFormatter { get; set; }
        public int? Season { get; set; }
        public int? ProductCopyRight { get; set; }
        public int? ColorID { get; set; }
        public decimal? InnerWeightGross { get; set; }
        public decimal? InnerWeightGrossLBS { get; set; }

        /// <summary>
        /// 季节对应的客户代码
        /// </summary>
        public string SeasonDepartmentNumber { get; set; }

        public string SkuCode { get; set; }
        public List<VMProductIngredients> list_ProductIngredient { get; set; }
        public int? FactoryID_ForQuote { get; set; }
        public string Factory_EnglishName { get; set; }
        public string Factory_EnglishAddress { get; set; }
        public int? INR { get; set; }
        public string TypeOfWood { get; set; }
        public string ColorEngName { get; set; }
        public string UnitEngName { get; set; }


        public string ColorName { get; set; }
        public string OfStoreReadyInnersEnclosed { get; set; }
        public string PreTicketed { get; set; }
        public int? Department { get; set; }
        public bool IsProductFitting { get; set; }
        public bool IsProductMixed { get; set; }
        public int? ParentProductMixedID { get; set; }
        public string ProductAmountFormatter { get; set; }
        public int Qty2 { get; set; }
        public bool IsNeedInspection { get; set; }
        public string IsNeedInspectionName { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? CurrencyExchangeUSD { get; set; }
        public decimal? CurrencyExchangeRMB { get; set; }
    }
}