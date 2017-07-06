using ERP.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Order
{
    [Serializable]
    public class VMOrderProduct
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 订单数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 订单单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 核算意见的内容
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 订单编号（Sale.Order）
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public int? IsDelete { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int? ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int? ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// SkuNumber
        /// </summary>
        public string SkuNumber { get; set; }

        /// <summary>
        /// 产品UPC
        /// </summary>
        public string ProductUPC { get; set; }

        /// <summary>
        /// 内盒UPC
        /// </summary>
        public string InnerUPC { get; set; }

        /// <summary>
        /// OuterUPC
        /// </summary>
        public string OuterUPC { get; set; }

        /// <summary>
        /// 零售单价
        /// </summary>
        public decimal? RetailPrice { get; set; }

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
        public decimal PriceFactory { get; set; }

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
        /// 海关编码
        /// </summary>
        public int? HTS { get; set; }

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
        /// Deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// 货币类型 1：人民币、2：美元
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
        /// OuterWeightGrossLBS
        /// </summary>
        public decimal? OuterWeightGrossLBS { get; set; }

        /// <summary>
        /// OuterWeightNetLBS
        /// </summary>
        public decimal? OuterWeightNetLBS { get; set; }

        /// <summary>
        /// FOBFTY
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
        public decimal? Fobus { get; set; }

        /// <summary>
        /// Ddp
        /// </summary>
        public decimal? Ddp { get; set; }

        /// <summary>
        /// Poe
        /// </summary>
        public decimal? Poe { get; set; }

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
        /// ELC
        /// </summary>
        public decimal? ELC { get; set; }

        /// <summary>
        /// Retail
        /// </summary>
        public decimal? Retail { get; set; }

        /// <summary>
        /// Mu
        /// </summary>
        public decimal? Mu { get; set; }

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
        /// FOBChinaPort
        /// </summary>
        public decimal? FOBChinaPort { get; set; }

        public string UnitName { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal SumSalePrice { get; set; }

        public decimal SumPriceFactory { get; set; }

        public string CurrencySign { get; set; }

        /// <summary>
        /// 产品来源：1.产品、2.报价单
        /// </summary>
        public int ProductFrom { get; set; }

        public string PercentSign { get; set; }

        /// <summary>
        /// OuterWeightGross
        /// </summary>
        public decimal? OuterWeightGross { get; set; }

        /// <summary>
        /// 外箱净重
        /// </summary>
        public decimal? OuterWeightNet { get; set; }

        public int CurrentQty { get; set; }

        public int CurrentDispatchedAmount { get; set; }

        public string FactoryName { get; set; }

        public string StyleName { get; set; }

        #region 计算列

        /// <summary>
        /// 箱数
        /// </summary>
        public decimal Ctns { get; set; }

        /// <summary>
        /// 总材积
        /// </summary>
        public decimal? TotalCuft { get; set; }

        public decimal? TotalCBM { get; set; }

        /// <summary>
        /// 净价
        /// </summary>
        public decimal NetPrice { get; set; }

        /// <summary>
        /// 总净价
        /// </summary>
        public decimal SumNetPrice { get; set; }

        public decimal? MUPercent { get; set; }

        #endregion 计算列

        public string PriceFactoryFormatter { get; set; }

        public string SumPriceFactoryFormatter { get; set; }

        public string RateFormatter { get; set; }

        public int? SortIndex { get; set; }

        public string QuoteTemplateFileName { get; set; }

        public decimal? ELCFill { get; set; }
        public decimal? InspectionVerificationFee { get; set; }
        public decimal? InspectionVerificationFee_ForFactory { get; set; }

        #region 清关用到了

        public string CustomerCode { get; set; }
        public int? BoxQty { get; set; }

        /// <summary>
        /// 总净重
        /// </summary>
        public decimal? SumOuterWeightNet { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? SumOuterWeightGross { get; set; }

        /// <summary>
        /// 总体积
        /// </summary>
        public decimal? SumOuterVolume { get; set; }
        public string POID { get; set; }
        public bool? QualityAssuranceTesting { get; set; }
        public decimal? SumOuterWeightGrossLBS { get; set; }
        public decimal? SumOuterWeightNetLBS { get; set; }
        public decimal? SumOuterVolume_CUFT { get; set; }
        public string CategoryManager { get; set; }
        public string PortName { get; set; }
        public string PaymentType { get; set; }
        public DateTime OrderDateStart { get; set; }
        public DateTime OrderDateEnd { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public int? Department { get; set; }

        /// <summary>
        /// 报关编码
        /// </summary>
        public string HSCode { get; set; }
        public decimal HSCode_Cess { get; set; }

        /// <summary>
        /// 工厂的英文名
        /// </summary>
        public string Factory_EnglishName { get; set; }

        /// <summary>
        /// 工厂的英文地址
        /// </summary>
        public string Factory_EnglishAddress { get; set; }

        /// <summary>
        /// 结汇里面的总体积
        /// </summary>
        public decimal? InspectionExchange_SumOuterVolume { get; set; }

        public int CabinetID { get; set; }

        /// <summary>
        /// 季节全称：名称 别名
        /// </summary>
        public string SeasonFullName { get; set; }

        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public int? INR { get; set; }

        #endregion 清关用到了

        /// <summary>
        /// 季节
        /// </summary>
        public int? Season { get; set; }

        /// <summary>
        /// 季节全称：数据名称 数据别名
        /// </summary>
        public string SeasonName { get; set; }

        /// <summary>
        /// 季节：数据名称
        /// </summary>
        public string SeasonPrefix { get; set; }

        /// <summary>
        /// 季节：数据别名
        /// </summary>
        public string SeasonSuffix { get; set; }

        public string BuyerName { get; set; }
        public string BuyerTel { get; set; }
        public string BuyerEmail { get; set; }

        public string AssistantName { get; set; }
        public string AssistantTel { get; set; }
        public string AssistantEmail { get; set; }

        public string ShippingType { get; set; }

        /// <summary>
        /// 出运方式名称
        /// </summary>
        public string TransportTypeName { get; set; }

        /// <summary>
        /// 客户选择的季节前缀
        /// </summary>
        public string CustomerSeasonPrefix { get; set; }

        /// <summary>
        /// 检测标准文件名
        /// </summary>
        public string TestingStandardsFilename { get; set; }

        public string PortEngName { get; set; }

        /// <summary>
        /// 产品成分构成列表
        /// </summary>
        public List<VMProductIngredients> list_ProductIngredient { get; set; }

        public decimal? MiscPercent { get; set; }
        public int? InspectionClearance_FactoryID { get; set; }
        public int? InspectionExchange_FactoryID { get; set; }

        public string InspectionClearance_Factory_EnglishName { get; set; }
        public string InspectionClearance_Factory_EnglishAddress { get; set; }

        public string InspectionExchange_Factory_EnglishName { get; set; }
        public string InspectionExchange_Factory_EnglishAddress { get; set; }
        public int CabinetIndex { get; set; }

        /// <summary>
        /// 获取某个产品的产品成分列表。格式如：70% GLASS
        /// </summary>
        public List<string> list_Ingredient { get; set; }
        public string SkuCode { get; set; }
        public int OrderProductQty { get; set; }
        public string OfStoreReadyInnersEnclosed { get; set; }
        public string PreTicketed { get; set; }
        public string BoxNumber { get; set; }
        public string SealingNumber { get; set; }
        public string HsEngName { get; set; }
        public string DepartmentName { get; set; }
        public string StyleNumber { get; set; }
        public bool IsProductFitting { get; set; }
        public bool IsProductMixed { get; set; }
        public int? ParentProductMixedID { get; set; }
        public int Qty2 { get; set; }

        /// <summary>
        /// 海关编码名称
        /// </summary>
        public string HTS_Name { get; set; }

        /// <summary>
        /// 报关编码名称
        /// </summary>
        public string HSCode_Name { get; set; }
        public string DutyPercentFormatter { get; set; }
        public string CommissionFormatter { get; set; }
        public string MiscImportLoadFormatter { get; set; }
        public int? FactoryID_ForQuote { get; set; }
        public string ProductAmountFormatter { get; set; }
        public bool IsNeedInspection { get; set; }
        public string IsNeedInspectionName { get; set; }
        public decimal? InnerWeightGrossLBS { get; set; }
        public string PortEnName { get; set; }
        public string PackingMannerZhName { get; set; }
        public string PackingMannerEnName { get; set; }
        public string CurrencyName { get; set; }
        public int QuotProductID{ get; set; }
    }
}