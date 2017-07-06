using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Purchase
{
    /// <summary>
    /// 采购合同——产品
    /// </summary>
    public class VMPurchaseProduct
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 产品货号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 包装名称
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// 工厂价格
        /// </summary>
        public decimal PriceFactory { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal ProductAmount { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        public int ProductID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string CurrentSign { get; set; }

        /// <summary>
        /// 内盒率
        /// </summary>
        public int? InnerBoxRate { get; set; }

        /// <summary>
        /// 外箱率
        /// </summary>
        public int? OuterBoxRate { get; set; }

        /// <summary>
        /// PDQ装箱率
        /// </summary>
        public int? PDQPackRate { get; set; }

        /// <summary>
        /// 包装方式
        /// </summary>
        public int? PackingMannerZhID { get; set; }

        /// <summary>
        /// 款式名称
        /// </summary>
        public string StyleName { get; set; }

        public string FactoryNo { get; set; }

        /// <summary>
        /// 混装方式
        /// </summary>
        public string MixedMode { get; set; }

        /// <summary>
        /// 产品其它要求备注
        /// </summary>
        public string OtherComment { get; set; }

        public int? UnitID { get; set; }

        public int? StyleID { get; set; }

        public string PriceFactoryFormatter { get; set; }

        public string ProductAmountFormatter { get; set; }

        public string SkuNumber { get; set; }

        public string SkuCode { get; set; }
        //S135唛头需要用到的SKU code
        
        public int? CustomerID { get; set; }

        public string CustomerCode { get; set; }

        public string POID { get; set; }

        public decimal? OuterWeightGross { get; set; }

        public decimal? OuterWeightNet { get; set; }

        public decimal? OuterVolume { get; set; }

        public string Desc { get; set; }

        public decimal? LengthIN { get; set; }

        public decimal? WidthIN { get; set; }

        public decimal? HeightIN { get; set; }

        public decimal? InnerLengthIN { get; set; }

        public decimal? InnerWidthIN { get; set; }

        public decimal? InnerHeightIN { get; set; }

        public string UPC { get; set; }

        public decimal? WeightLBS { get; set; }

        public int? DestinationPortID { get; set; }

        public string DestinationPortName { get; set; }
        public int? IsFragile { get; set; }

        /// <summary>
        /// 季节
        /// </summary>
        public int? Season { get; set; }

        /// <summary>
        /// 季节全称：数据名称 数据别名
        /// </summary>
        public string SeasonName { get; set; }

        public bool? IsFragile2 { get; set; }
        public string CartonBarcodeLabel_Image { get; set; }
        public string IsFragile3 { get; set; }
        public decimal? OuterLengthIN { get; set; }
        public decimal? OuterWidthIN { get; set; }
        public decimal? OuterHeightIN { get; set; }

        public decimal? OuterLength { get; set; }
        public decimal? OuterWidth { get; set; }
        public decimal? OuterHeight { get; set; }


        public decimal? InnerLength { get; set; }
        public decimal? InnerWidth { get; set; }
        public decimal? InnerHeight { get; set; }


        public decimal? OuterWeightGrossLBS { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public int? Department { get; set; }

        public string DepartmentName { get; set; }

        public decimal? InnerGrossWeightLBS { get; set; }

        public decimal? InnerWeightGross { get; set; }

        public decimal? InnerNetWeightLBS { get; set; }

        public decimal? InnerWeightNet { get; set; }

        /// <summary>
        /// 产品箱数
        /// </summary>
        public int? CaseOrderQty { get; set; }

        /// <summary>
        /// 季节：数据名称
        /// </summary>
        public string SeasonPrefix { get; set; }

        /// <summary>
        /// 季节：数据别名
        /// </summary>
        public string SeasonSuffix { get; set; }

        /// <summary>
        /// 季节：Dept#
        /// </summary>
        public string SeasonDepartmentNumber { get; set; }
        
        /// <summary>
        /// 外箱净重LBS
        /// </summary>
        public decimal? OuterWeightNetLBS { get; set; }

        /// <summary>
        /// 箱子类型 //1=标准箱，2=非标准箱，3=小箱；从生成唛头时获取值
        /// </summary>
        public int? CarbinetType { get; set; }

        /// <summary>
        /// 产品资料中的产品UPC
        /// </summary>
        public string ProductUPC { get; set; }
        
        /// <summary>
        /// 外箱UPC
        /// </summary>
        public string OuterPKUPC { get; set; }

        /// <summary>
        /// 内盒UPC
        /// </summary>
        public string InnerPKUPC { get; set; }

        /// <summary>
        /// 产品颜色
        /// </summary>
        public string ColorName { get; set; }
        public string SeasonZhName { get; set; }

        public string CompanyName { get; set; }
        //唛头中的客户地址：公司名称,取指定的客户收货地址的公司名称
        public string Address1{ get; set; }
        //唛头中的客户地址：街道地址
        public string Address2 { get; set; }
        //唛头中的客户地址：州、城市、邮编
        public string Address3 { get; set; }
        //唛头中的客户地址：国家

        public string StoreReadyInners { get; set; }
        //S10需要用到的字段，在核算单中会输入
        public string PreTicketed { get; set; }
        public string UnitEngName { get; set; }

        public string AcceptInformation_StreetAddress { get; set; }
        public string AcceptInformation_CustomerReg { get; set; }
        public string AcceptInformation_CompanyName { get; set; }
        public string OfStoreReadyInnersEnclosed { get; set; }
        public string AcceptInformation_Comment { get; set; }
        public bool IsProductMixed { get; set; }

        //S10需要用到的字段，在核算单中会输入

        public int OrderProductID { get; set; }



        //public DateTime PurchaseOrderDate { get; set; }
    }
}