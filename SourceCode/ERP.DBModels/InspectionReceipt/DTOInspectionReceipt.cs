using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.ShipmentOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.InspectionReceipt
{
    /// <summary>
    /// 报检单据信息明细视图模型
    /// </summary>
    public class DTOInspectionReceipt
    {
        /// <summary>
        /// 页面标题：1=查看报检单据；2=编辑报检单据；3=审核报检单据；
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 创建人自编号
        /// </summary>
        public int CreateUserID { get; set; }

        /// <summary>
        /// 审批流权限值
        /// </summary>
        public int? ApproverIndex { get; set; }

        /// <summary>
        /// 报检自编号
        /// </summary>
        public int InspectionReceiptID { get; set; }

        public int ShipmentOrderID { get; set; }

        /// <summary>
        /// 采购合同产品所在销售订单自编号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int ContractID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 工厂联系人
        /// </summary>
        public string FactoryContact { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户所在街道
        /// </summary>
        public string CustomerStreet { get; set; }

        /// <summary>
        /// 客户所在城市,地区,国家,邮编
        /// </summary>
        public string CustomerReg { get; set; }

        /// <summary>
        /// 装箱单报关出运港（装箱单）
        /// </summary>
        public int INPortID_1 { get; set; }

        /// <summary>
        /// 装箱单报关目的港
        /// </summary>
        public string INEndPortName { get; set; }

        /// <summary>
        /// 出运港（英文）
        /// </summary>
        public string INPortName { get; set; }

        /// <summary>
        /// 出运港（中文）
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 装箱单、发票Inv. No.:ZL+当前年份两位结尾数字+从201开始至999结束+A~Z
        /// (201~999是指同一年所产生报检数据逐条累加;A~Z是在同一销售订单前提下：按大写字母顺序对应不同的HSCODE)
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// 销售订单客户下单日期
        /// </summary>
        public DateTime CustomerDate { get; set; }

        public string CustomerDateFormatter { get; set; }

        /// <summary>
        /// 最迟凭条出据日期
        /// </summary>
        public string ClaimFaxDate { get; set; }

        /// <summary>
        /// S/C NO:SC +产品所在销售订单编号+A~Z(A~Z是在同一销售订单前提下：按大写字母顺序对应不同的HSCODE)
        /// </summary>
        public string SCNO { get; set; }

        /// <summary>
        /// 产品订单数量
        /// </summary>
        public int ProductsQuauity { get; set; }

        /// <summary>
        /// 产品单价（发票，可编辑）
        /// </summary>
        public decimal ProductPrice { get; set; }

        /// <summary>
        /// 产品订单箱数（求和）:采购合同->一个HSID对应的产品所在销售订单的订单箱数之和
        /// 产品箱数=产品数量/外箱率
        /// </summary>
        public int ProductsBoxNum { get; set; }

        /// <summary>
        /// 产品总价（发票）=产品单价×数量
        /// </summary>
        public decimal ProductAmount { get; set; }

        /// <summary>
        /// 总毛重=单箱毛重×产品箱数
        /// </summary>
        public decimal WeightGrossSum { get; set; }

        /// <summary>
        /// 总净重=单箱净重×总箱数
        /// </summary>
        public decimal WeightNetSum { get; set; }

        /// <summary>
        /// 产品体积=(外箱长×宽×高)/1000000 * 箱数，四舍五入，精确到小数点后两位
        /// </summary>
        public decimal CUFT { get; set; }

        /// <summary>
        /// Unit Retail（可编辑）
        /// </summary>
        public string UnitRetail { get; set; }

        /// <summary>
        /// CS PK（产品外箱率）
        /// </summary>
        public int OuterBoxRate { get; set; }

        /// <summary>
        /// 报检数据状态ID
        /// </summary>
        public int InspectionReceiptStatusID { get; set; }

        /// <summary>
        /// 报检明细数据状态
        /// </summary>
        public string InspectionReceiptStatus { get; set; }

        /// <summary>
        /// 报检自编号
        /// </summary>
        public int HSID { get; set; }

        /// <summary>
        /// 报检编号
        /// </summary>
        public string HSCodeName { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string HsName { get; set; }

        /// <summary>
        /// 报检类型：报检子表TagID与数据字典关联
        /// </summary>
        public string INTypeName { get; set; }

        /// <summary>
        /// 报检备注
        /// </summary>
        public string INRemark { get; set; }

        /// <summary>
        /// 销售合同：内容
        /// </summary>
        public string SaleContractContent { get; set; }

        /// <summary>
        /// 报检创建人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        public string CreateDateForamtter { get; set; }

        /// <summary>
        /// TheBuyer
        /// </summary>
        public string TheBuyer { get; set; }

        /// <summary>
        /// ShippingMark，可编辑（报检明细->销售合同选显卡）
        /// </summary>
        public string ShippingMark { get; set; }

        /// <summary>
        /// 获取客户的付款方式
        /// </summary>
        public string TeamOfThePayment { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 销售订单的客户PO,大于一个时为空
        /// </summary>
        public string SMCustoerPO { get; set; }

        /// <summary>
        /// 委托书（上传附件）
        /// </summary>
        public List<VMUpLoadFile> ReceiptCommission { get; set; }

        /// <summary>
        /// 上传凭条（上传附件）
        /// </summary>
        public List<VMUpLoadFile> UploadReceipt { get; set; }

        /// <summary>
        /// 报检历史记录
        /// </summary>
        public List<VMInspectionReceiptHis> InspectionReceiptHis { get; set; }

        public int? HSCodeID { get; set; }

        public string ContractIDList { get; set; }
        public string SCNOList { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string POID { get; set; }

        public List<VMShipmentOrderProduct> list_ShipmentOrderProduct { get; set; }
        public string HsEngName { get; set; }
        public string ShippingDateStart { get; set; }

        /// <summary>
        /// 贸易方式
        /// </summary>
        public int? TradeType { get; set; }

        /// <summary>
        /// 贸易方式名称
        /// </summary>
        public string TradeTypeName { get; set; }

        /// <summary>
        /// 目的港——国家
        /// </summary>
        public string DestinationPort_Country { get; set; }

        /// <summary>
        /// 目的港——省份
        /// </summary>
        public string DestinationPort_Province { get; set; }

        /// <summary>
        /// 目的港——城市
        /// </summary>
        public string DestinationPort_City { get; set; }

        /// <summary>
        /// 目的港——街道地址
        /// </summary>
        public string DestinationPort_StreetAddress { get; set; }

        /// <summary>
        /// 目的港——邮编
        /// </summary>
        public string DestinationPort_ZipCode { get; set; }

        /// <summary>
        /// 目的港——公司名称
        /// </summary>
        public string DestinationPort_CompanyName { get; set; }

        /// <summary>
        /// 目的港——地址。格式：City,State 邮编 US
        /// </summary>
        public string DestinationPort_Address { get; set; }

        public string SelectCustomer { get; set; }
        public string OrderDateStart { get; set; }
        public string SeasonPrefix { get; set; }
        public int InspectionReceiptListID { get; set; }
    }
}