using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    public class VMQuoteProductHistory
    {
        public int QuotProductID { get; set; }

        public string Status { get; set; }

        public DateTime StatusDate { get; set; }

        public int ID { get; set; }

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
        /// 外盒率
        /// </summary>
        public int? OuterBoxRate { get; set; }

        /// <summary>
        /// 外盒长
        /// </summary>
        public decimal? OuterLength { get; set; }

        /// <summary>
        /// 外盒宽
        /// </summary>
        public decimal? OuterWidth { get; set; }

        /// <summary>
        /// 外盒高
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
        /// DutyPercent
        /// </summary>
        public decimal? DutyPercent { get; set; }

        /// <summary>
        /// FreightRate
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

        /// <summary>
        /// 价格输入日期
        /// </summary>
        public DateTime PriceInputDate { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime ValidDate { get; set; }

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
        /// OuterVolume
        /// </summary>
        public decimal? OuterVolume { get; set; }

        /// <summary>
        /// OuterWeightGrossLBS
        /// </summary>
        public decimal? OuterWeightGrossLBS { get; set; }

        /// <summary>
        /// OuterWeightNetLBS
        /// </summary>
        public decimal? OuterWeightNetLBS { get; set; }

        /// <summary>
        /// Fobfty
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
        /// Rate
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
        /// PcsPallet
        /// </summary>
        public decimal? PcsPallet { get; set; }

        /// <summary>
        /// PalletPc
        /// </summary>
        public decimal? PalletPc { get; set; }

        /// <summary>
        /// Duty
        /// </summary>
        public decimal? Duty { get; set; }

        /// <summary>
        /// Freight
        /// </summary>
        public decimal? Freight { get; set; }

        /// <summary>
        /// Commission
        /// </summary>
        public decimal? Commission { get; set; }

        /// <summary>
        /// CommissionAmount
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
        /// Elc
        /// </summary>
        public decimal? ELC { get; set; }

        /// <summary>
        /// Retail
        /// </summary>
        public decimal? Retail { get; set; }

        /// <summary>
        /// Mu
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

        public decimal? DDP { get; set; }

        public decimal? POE { get; set; }

        public int ProductID { get; set; }

        public string DT_CREATEDATEFormat { get; set; }

        /// <summary>
        /// HTS。不可修改。取HS CODE 编码
        /// </summary>
        public int? HTS { get; set; }

        public string HTS_Name { get; set; }

        /// <summary>
        /// HTCode
        /// </summary>
        public int? HSCode { get; set; }

        public string HSCode_Name { get; set; }

        /// <summary>
        /// OuterWeightGross
        /// </summary>
        public decimal? OuterWeightGross { get; set; }

        /// <summary>
        /// OuterWeightNet
        /// </summary>
        public decimal? OuterWeightNet { get; set; }

        public string QuoteTemplateFileName { get; set; }

        public decimal? ELCFill { get; set; }
        public decimal? InnerWeightGross { get; set; }
        public decimal? InnerWeightGrossLBS { get; set; }
        public int? FactoryID_ForQuote { get; set; }
        public int? INR { get; set; }
        public string TypeOfWood { get; set; }
    }
}