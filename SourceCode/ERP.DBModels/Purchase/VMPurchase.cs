using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Factory;
using ERP.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Purchase
{
    /// <summary>
    /// 采购合同
    /// </summary>
    public class VMPurchase
    {
        public int ID { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 其它额外费用
        /// </summary>
        public decimal? OtherFee { get; set; }

        /// <summary>
        /// 采购合同总金额
        /// </summary>
        public decimal AllAmount { get; set; }

        /// <summary>
        /// 工厂编号
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 工厂全称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 联络人
        /// </summary>
        public string CallPeople { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 工厂货号
        /// </summary>
        public string FactoryNo { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 交货港口
        /// </summary>
        public int PortID { get; set; }

        /// <summary>
        /// 开始交货日期
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 开始交货日期
        /// </summary>
        public string DateStartFormatter { get; set; }

        /// <summary>
        /// 结束交货日期
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 结束交货日期
        /// </summary>
        public string DateEndFormatter { get; set; }

        /// <summary>
        /// 合同条款
        /// </summary>
        public string ContractTerms { get; set; }

        /// <summary>
        /// 采购合同审核状态
        /// </summary>
        public int PurchaseStatus { get; set; }

        /// <summary>
        ///结关后的天数
        /// </summary>
        public int? AfterDate { get; set; }

        /// <summary>
        /// 审核成功后自动发送邮件给工厂（0：否，1：是）
        /// </summary>
        public bool IsImmediatelySend { get; set; }

        /// <summary>
        /// 第三方验货（0：否，1：是）
        /// </summary>
        public bool IsThirdVerification { get; set; }

        /// <summary>
        /// 第三方验厂（0：否，1：是）
        /// </summary>
        public bool IsThirdAudits { get; set; }

        /// <summary>
        /// 第三方检测（0：否，1：是）
        /// </summary>
        public bool IsThirdTest { get; set; }

        /// <summary>
        /// 第三方抽检（0：否，1：是）
        /// </summary>
        public bool IsThirdSampling { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string CurrentSign { get; set; }

        /// <summary>
        /// 总体积。用于计算合同条款
        /// </summary>
        public decimal AllVolume { get; set; }

        /// <summary>
        /// 总数量。用于计算合同条款
        /// </summary>
        public int AllQty { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        public string PortName { get; set; }

        public string FactoryEmail { get; set; }

        /// <summary>
        /// 批次列表
        /// </summary>
        public List<VMPurchaseBatch> list_batch { get; set; }

        /// <summary>
        /// 上传文件的列表
        /// </summary>
        public List<VMUpLoadFile> list_UpLoadFile { get; set; }

        /// <summary>
        /// 历史记录列表
        /// </summary>
        public List<VMPurchaseHistory> list_history { get; set; }

        public string PurchaseDate { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// 交货地
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentType { get; set; }

        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 采购合同——审批流索引
        /// </summary>
        public int? ApproverIndexPurchaseContract { get; set; }

        public int? ApproverIndexPacks { get; set; }

        public int? ApproverIndexEncasement { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public string OrderNumber { get; set; }

        public bool isCurrentSign_RMB { get; set; }

        public bool isCurrentSign_USD { get; set; }

        public string POID { get; set; }

        public int? DestinationPortID { get; set; }

        public string DestinationPortName { get; set; }

        public int ShippingMark_StatusID { get; set; }

        public int? ShippingMark_CreateUser { get; set; }

        public DateTime? ShippingMark_ModifyDate { get; set; }

        public int? ShippingMark_CustomerID { get; set; }

        public int? ApproverIndexShippingMark { get; set; }

        public string ShippingMark_StatusName { get; set; }

        public string ShippingMark_ModifyDateFormatter { get; set; }
        public string ShippingMark_Comment { get; set; }

        public string ShippingMark_PDF { get; set; }

        public List<VMUpLoadFile> ShippingMark_UpLoadFileList { get; set; }
        public string SelectCustomer { get; set; }
        public string ImageList_ServerFileName { get; set; }
        public string ImageList_DisplayFileName { get; set; }
        public string list_ToEmailAddress { get; set; }
        public string list_CallName { get; set; }
        public string EmailSign { get; set; }

        public int? ShippingMark_AcceptInformationID { get; set; }

        /// <summary>
        /// 合同类型（0：采购合同，1：配件合同）
        /// </summary>
        public int ContractType { get; set; }
    }
}