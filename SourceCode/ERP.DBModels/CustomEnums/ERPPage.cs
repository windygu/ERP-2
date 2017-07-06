using ERP.Models.CustomAttribute;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum ERPPage
    {
        #region 产品管理

        /// <summary>
        /// 产品管理
        /// </summary>
        [Description("产品管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        ProductManagement = 10,

        /// <summary>
        /// 大清单产品
        /// </summary>
        [Description("大清单产品"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Product", ParentID = (int)ERPPage.ProductManagement, PageElementEnumType = typeof(ProductListElementPrivileges))]
        ProductManagement_Info = 1001,

        /// <summary>
        /// 产品分类管理
        /// </summary>
        [Description("产品分类管理"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProductClassification/Classify", WinType = "idialog", WinSize = "1010,680", ParentID = (int)ERPPage.ProductManagement)]
        ProductManagement_Classification = 1002,

        /// <summary>
        /// 产品批量导入
        /// </summary>
        [Description("产品批量导入"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProductUpload/Classify", WinType = "idialog", WinSize = "1010,680", ParentID = (int)ERPPage.ProductManagement)]
        ProductManagement_Upload = 1003,

        /// <summary>
        /// 客户选样产品
        /// </summary>
        [Description("客户选样产品"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Product/Quotes", ParentID = (int)ERPPage.ProductManagement, PageElementEnumType = typeof(ProductListElementPrivileges))]
        ProductManagement_QuoteProduct_Management = 1004,

        /// <summary>
        /// 配件产品
        /// </summary>
        [Description("配件产品"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProductFitting", ParentID = (int)ERPPage.ProductManagement, PageElementEnumType = typeof(ProductFittingElementPrivileges))]
        ProductManagement_ProductFitting = 1005,

        /// <summary>
        /// 混装产品
        /// </summary>
        [Description("混装产品"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProductMixed", ParentID = (int)ERPPage.ProductManagement, PageElementEnumType = typeof(ProductMixedElementPrivileges))]
        ProductManagement_ProductMixed = 1006,

        #endregion 产品管理

        #region 销售管理

        /// <summary>
        /// 销售管理
        /// </summary>
        [Description("销售管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        SaleManagement = 20,

        /// <summary>
        /// 报价单管理
        /// </summary>
        [Description("报价单管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.SaleManagement)]
        SaleManagement_QuoteManagement = 2001,

        /// <summary>
        /// 报价单列表
        /// </summary>
        [Description("报价单列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Quote", ParentID = (int)ERPPage.SaleManagement_QuoteManagement, PageElementEnumType = typeof(QuoteElementPrivileges))]
        SaleManagement_QuoteManagement_QuoteList = 200101,

        /// <summary>
        /// 报价单审核
        /// </summary>
        [Description("报价单审核"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Quote/PassedCheck", ParentID = (int)ERPPage.SaleManagement_QuoteManagement, PageElementEnumType = typeof(QuoteElementPrivileges))]
        SaleManagement_QuoteManagement_QuoteApproval = 200102,

        /// <summary>
        /// 寄样管理
        /// </summary>
        [Description("寄样管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.SaleManagement)]
        SaleManagement_SampleManagement = 2002,

        /// <summary>
        /// 待打样列表
        /// </summary>
        [Description("待打样列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Sample/Manufacturing", ParentID = (int)ERPPage.SaleManagement_SampleManagement, PageElementEnumType = typeof(SampleElementPrivileges))]
        SaleManagement_SampleManagement_Manufacturing = 200201,

        /// <summary>
        /// 已打样列表
        /// </summary>
        [Description("已打样列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Sample/Manufactured", ParentID = (int)ERPPage.SaleManagement_SampleManagement, PageElementEnumType = typeof(SampleElementPrivileges))]
        SaleManagement_SampleManagement_Manufactured = 200202,

        /// <summary>
        /// 待寄样列表
        /// </summary>
        [Description("待寄样列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Sample/Sending", ParentID = (int)ERPPage.SaleManagement_SampleManagement, PageElementEnumType = typeof(SampleElementPrivileges))]
        SaleManagement_SampleManagement_Sending = 200203,

        /// <summary>
        /// 已寄样列表
        /// </summary>
        [Description("已寄样列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Sample/Sended", ParentID = (int)ERPPage.SaleManagement_SampleManagement, PageElementEnumType = typeof(SampleElementPrivileges))]
        SaleManagement_SampleManagement_Sent = 200204,

        /// <summary>
        /// 销售订单管理
        /// </summary>
        [Description("销售订单管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.SaleManagement)]
        SaleManagement_OrderManagement = 2003,

        /// <summary>
        /// 待审批销售订单
        /// </summary>
        [Description("待审批销售订单"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Order", ParentID = (int)ERPPage.SaleManagement_OrderManagement, PageElementEnumType = typeof(OrderElementPrivileges))]
        SaleManagement_OrderManagement_Index = 200301,

        /// <summary>
        /// 已审批销售订单
        /// </summary>
        [Description("已审批销售订单"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Order/PassedApproval", ParentID = (int)ERPPage.SaleManagement_OrderManagement, PageElementEnumType = typeof(OrderElementPrivileges))]
        SaleManagement_OrderManagement_PassedApproval = 200303,

        #endregion 销售管理

        #region 采购管理

        /// <summary>
        /// 采购管理
        /// </summary>
        [Description("采购管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        PurchaseManagement = 30,

        /// <summary>
        /// 采购合同管理
        /// </summary>
        [Description("采购合同管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        PurchaseManagement_ContractManagement = 3001,

        /// <summary>
        /// 待审核的采购合同
        /// </summary>
        [Description("待审核的采购合同"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/PurchaseContract", ParentID = (int)ERPPage.PurchaseManagement_ContractManagement, PageElementEnumType = typeof(PurchaseContractElementPrivileges))]
        PurchaseManagement_ContractManagement_Index = 300101,

        /// <summary>
        /// 已审核的采购合同
        /// </summary>
        [Description("已审核的采购合同"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/PurchaseContract/PassedCheck", ParentID = (int)ERPPage.PurchaseManagement_ContractManagement, PageElementEnumType = typeof(PurchaseContractElementPrivileges))]
        PurchaseManagement_ContractManagement_PassedCheck = 300102,

        /// <summary>
        /// 包装资料管理
        /// </summary>
        [Description("包装资料管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        PurchaseManagement_PackManagement = 3002,

        /// <summary>
        /// 待审核的标签资料
        /// </summary>
        [Description("待审核的标签资料"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Packs", ParentID = (int)ERPPage.PurchaseManagement_PackManagement, PageElementEnumType = typeof(PacksElementsPrivileges))]
        PurchaseManagement_Packs_PendingApproval = 300201,

        /// <summary>
        /// 已审核的标签资料
        /// </summary>
        [Description("已审核的标签资料"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Packs/PassedApproval", ParentID = (int)ERPPage.PurchaseManagement_PackManagement, PageElementEnumType = typeof(PacksElementsPrivileges))]
        PurchaseManagement_Packs_PassedApproval = 300202,

        /// <summary>
        /// 待审核的唛头资料
        /// </summary>
        [Description("待审核的唛头资料"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShippingMark", ParentID = (int)ERPPage.PurchaseManagement_PackManagement, PageElementEnumType = typeof(ShippingMarkElementPrivileges))]
        PurchaseManagement_ShippingMark_Index = 300203,

        /// <summary>
        /// 已审核的唛头资料
        /// </summary>
        [Description("已审核的唛头资料"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShippingMark/PassedCheck", ParentID = (int)ERPPage.PurchaseManagement_PackManagement, PageElementEnumType = typeof(ShippingMarkElementPrivileges))]
        PurchaseManagement_ShippingMark_PassedCheck = 300204,

        /// <summary>
        /// 代购合同管理
        /// </summary>
        [Description("代购合同管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        PurchaseManagement_OutSourcing = 3003,

        /// <summary>
        /// 待审核代购合同
        /// </summary>
        [Description("待审核代购合同"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Outsourcing", ParentID = (int)ERPPage.PurchaseManagement_OutSourcing, PageElementEnumType = typeof(OutSourcingElementPrivileges))]
        PurchaseManagement_OutSourcing_Index = 300301,

        /// <summary>
        /// 已审核代购合同
        /// </summary>
        [Description("已审核代购合同"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Outsourcing/PassedApproval", ParentID = (int)ERPPage.PurchaseManagement_OutSourcing, PageElementEnumType = typeof(OutSourcingElementPrivileges))]
        PurchaseManagement_OutSourcing_PassedApproval = 300302,

        /// <summary>
        /// 第三方检验
        /// </summary>
        [Description("第三方检验"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        ThirdParty_Inspection = 3004,

        /// <summary>
        /// 第三方验厂
        /// </summary>
        [Description("第三方验厂"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Inspection", ParentID = (int)ERPPage.ThirdParty_Inspection, PageElementEnumType = typeof(InspectionAuditElementPrivileges))]
        ThirdParty_Inspection_AuditNotice = 300401,

        /// <summary>
        /// 第三方检测
        /// </summary>
        [Description("第三方检测"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Inspection/DetectNoticeList", ParentID = (int)ERPPage.ThirdParty_Inspection, PageElementEnumType = typeof(InspectionDetectElementPrivileges))]
        ThirdParty_Inspection_DetectNotice = 300402,

        /// <summary>
        /// 第三方抽检
        /// </summary>
        [Description("第三方抽检"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Inspection/SamplingNoticeList", ParentID = (int)ERPPage.ThirdParty_Inspection, PageElementEnumType = typeof(InspectionSamplingElementPrivileges))]
        ThirdParty_Inspection_SamplingNotice = 300403,

        /// <summary>
        /// 第三方验货
        /// </summary>
        [Description("第三方验货"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ThirdPartyVerification", ParentID = (int)ERPPage.ThirdParty_Inspection, PageElementEnumType = typeof(ThirdPartyVerificationElementPrivileges))]
        ThirdParty_Inspection_Verification = 300404,

        /// <summary>
        /// 生产计划管理
        /// </summary>
        [Description("生产计划管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        ProduceManagement_Plan = 3005,

        /// <summary>
        /// 上传生产计划
        /// </summary>
        [Description("上传生产计划"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProducePlan", ParentID = (int)ERPPage.ProduceManagement_Plan, PageElementEnumType = typeof(ProducePlanElementPricileges))]
        ProduceManagement_Index = 300501,

        /// <summary>
        /// 待审核的生产计划
        /// </summary>
        [Description("待审核的生产计划"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProducePlan/ApprovalList", ParentID = (int)ERPPage.ProduceManagement_Plan, PageElementEnumType = typeof(ProducePlanElementPricileges))]
        ProduceManagement_ApprovalList = 300502,

        /// <summary>
        /// 已审核的生产计划
        /// </summary>
        [Description("已审核的生产计划"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ProducePlan/PassedApprovalList", ParentID = (int)ERPPage.ProduceManagement_Plan, PageElementEnumType = typeof(ProducePlanElementPricileges))]
        ProduceManagement_PassedApprovalList = 300503,

        /// <summary>
        /// 三期QC管理
        /// </summary>
        [Description("三期QC管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        PurchaseManagement_ThreeTimesQCManagement = 3006,

        /// <summary>
        /// 待审核的前期QC
        /// </summary>
        [Description("待审核的前期QC"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ThreeTimesQC", ParentID = (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement, PageElementEnumType = typeof(ThreeTimesQCElementPricileges))]
        PurchaseManagement_ThreeTimesQCManagement_Index = 30060101,

        /// <summary>
        /// 待审核的中期QC
        /// </summary>
        [Description("待审核的中期QC"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ThreeTimesQC/Two", ParentID = (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement, PageElementEnumType = typeof(ThreeTimesQCElementPricileges))]
        PurchaseManagement_ThreeTimesQCManagement_Two = 30060102,

        /// <summary>
        /// 待审核的尾期QC
        /// </summary>
        [Description("待审核的尾期QC"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ThreeTimesQC/Three", ParentID = (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement, PageElementEnumType = typeof(ThreeTimesQCElementPricileges))]
        PurchaseManagement_ThreeTimesQCManagement_Three = 30060103,

        /// <summary>
        /// 已审核的三期QC
        /// </summary>
        [Description("已审核的三期QC"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ThreeTimesQC/PassedApproval", ParentID = (int)ERPPage.PurchaseManagement_ThreeTimesQCManagement, PageElementEnumType = typeof(ThreeTimesQCElementPricileges))]
        PurchaseManagement_ThreeTimesQCManagement_PassedApproval = 30060104,

        /// <summary>
        /// 出货样收回登记
        /// </summary>
        [Description("出货样收回登记"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.PurchaseManagement)]
        PurchaseManagement_ShipmentSampleManagement = 3007,

        /// <summary>
        /// 待收回的出货样
        /// </summary>
        [Description("待收回的出货样"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentSample", ParentID = (int)ERPPage.PurchaseManagement_ShipmentSampleManagement, PageElementEnumType = typeof(ShipmentSampleElementPrivileges))]
        PurchaseManagement_ShipmentSampleManagement_Index = 300701,

        /// <summary>
        /// 已收回的出货样
        /// </summary>
        [Description("已收回的出货样"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentSample/HadRecoveredList", ParentID = (int)ERPPage.PurchaseManagement_ShipmentSampleManagement, PageElementEnumType = typeof(ShipmentSampleElementPrivileges))]
        PurchaseManagement_ShipmentSampleManagement_HadRecovered = 300702,

        #endregion 采购管理

        #region 出运管理

        /// <summary>
        /// 出运管理
        /// </summary>
        [Description("出运管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        DeliveryManagement = 35,

        /// <summary>
        /// 出运明细管理
        /// </summary>
        [Description("出运明细管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DeliveryManagement)]
        DeliveryManagement_Details = 3501,

        /// <summary>
        /// 待审核出运明细列表
        /// </summary>
        [Description("待审核的出运明细"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Encasement", ParentID = (int)ERPPage.DeliveryManagement_Details, PageElementEnumType = typeof(EncasementElementPrivileges))]
        DeliveryManagement_Details_Index = 350101,

        /// <summary>
        /// 已审核出运明细列表
        /// </summary>
        [Description("已审核的出运明细"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Encasement/PassedApprovalList", ParentID = (int)ERPPage.DeliveryManagement_Details, PageElementEnumType = typeof(EncasementElementPrivileges))]
        DeliveryManagement_Details_PassedApprovalList = 350102,

        /// <summary>
        /// 订舱管理
        /// </summary>
        [Description("订舱管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DeliveryManagement)]
        DeliveryManagement_ShipmentOrder = 3502,

        /// <summary>
        /// 待审核的订舱信息
        /// </summary>
        [Description("待审核的订舱信息"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentOrder", ParentID = (int)ERPPage.DeliveryManagement_ShipmentOrder, PageElementEnumType = typeof(ShipmentOrderElementPrivileges))]
        DeliveryManagement_ShipmentOrder_Index = 350201,

        /// <summary>
        /// 已审核的订舱信息
        /// </summary>
        [Description("已审核的订舱信息"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentOrder/PassedApprovalList", ParentID = (int)ERPPage.DeliveryManagement_ShipmentOrder, PageElementEnumType = typeof(ShipmentOrderElementPrivileges))]
        DeliveryManagement_ShipmentOrder_PassedApprovalList = 350202,

        /// <summary>
        /// 出运通知管理
        /// </summary>
        [Description("出运通知管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DeliveryManagement)]
        DeliveryManagement_ShipmentNotification = 3503,

        /// <summary>
        /// 待审核的出运通知
        /// </summary>
        [Description("待审核的出运通知"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentNotification", ParentID = (int)ERPPage.DeliveryManagement_ShipmentNotification, PageElementEnumType = typeof(ShipmentNotificationElementPrivileges))]
        DeliveryManagement_ShipmentNotification_Index = 350301,

        /// <summary>
        /// 已审核的出运通知
        /// </summary>
        [Description("已审核的出运通知"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentNotification/PassedApprovalList", ParentID = (int)ERPPage.DeliveryManagement_ShipmentNotification, PageElementEnumType = typeof(ShipmentNotificationElementPrivileges))]
        DeliveryManagement_ShipmentNotification_PassedApprovalList = 350302,

        /// <summary>
        /// 拉柜费用登记列表
        /// </summary>
        [Description("拉柜费用登记列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentNotification/RegisterFeesList", ParentID = (int)ERPPage.DeliveryManagement_ShipmentNotification, PageElementEnumType = typeof(ShipmentNotificationElementPrivileges))]
        DeliveryManagement_ShipmentNotification_RegisterFees = 350304,

        #endregion 出运管理

        #region 供应商管理

        /// <summary>
        /// 供应商管理
        /// </summary>
        [Description("供应商管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        FactoryManagement = 40,

        /// <summary>
        /// 工厂信息列表
        /// </summary>
        [Description("工厂信息列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Factory", ParentID = (int)ERPPage.FactoryManagement, PageElementEnumType = typeof(FactoryElementPrivileges))]
        FactoryManagement_List = 4001,

        #endregion 供应商管理

        #region 单证管理

        /// <summary>
        /// 单证管理
        /// </summary>
        [Description("单证管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        DocumentsManagement = 45,

        /// <summary>
        /// 报检单据管理
        /// </summary>
        [Description("报检单据管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DocumentsManagement)]
        DocumentsManagement_InspectionReceipt = 4501,

        /// <summary>
        /// 待审核报检单据列表
        /// </summary>
        [Description("待审核报检单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionReceipt", ParentID = (int)ERPPage.DocumentsManagement_InspectionReceipt, PageElementEnumType = typeof(InspectionReceiptElementPrivileges))]
        DocumentsManagement_InspectionReceipt_Index = 450101,

        /// <summary>
        /// 已审核报检单据列表
        /// </summary>
        [Description("已审核报检单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionReceipt/PassedApprovalList", ParentID = (int)ERPPage.DocumentsManagement_InspectionReceipt, PageElementEnumType = typeof(InspectionReceiptElementPrivileges))]
        DocumentsManagement_InspectionReceipt_PassedApprovalList = 450102,

        /// <summary>
        /// 报关单据管理
        /// </summary>
        [Description("报关单据管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DocumentsManagement)]
        DocumentsManagement_InspectionCustoms = 4502,

        /// <summary>
        /// 待审核报关单据列表
        /// </summary>
        [Description("待审核报关单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionCustoms", ParentID = (int)ERPPage.DocumentsManagement_InspectionCustoms, PageElementEnumType = typeof(InspectionCustomsElementPrivileges))]
        DocumentsManagement_InspectionCustoms_Index = 450201,

        /// <summary>
        /// 已审核报关单据列表
        /// </summary>
        [Description("已审核报关单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionCustoms/PassedApprovalList", ParentID = (int)ERPPage.DocumentsManagement_InspectionCustoms, PageElementEnumType = typeof(InspectionCustomsElementPrivileges))]
        DocumentsManagement_InspectionCustoms_PassedApprovalList = 450202,

        /// <summary>
        /// 清关单据管理
        /// </summary>
        [Description("清关单据管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DocumentsManagement)]
        DocumentsManagement_InspectionClearance = 4503,

        /// <summary>
        /// 待审核清关单据列表
        /// </summary>
        [Description("待审核清关单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionClearance", ParentID = (int)ERPPage.DocumentsManagement_InspectionClearance, PageElementEnumType = typeof(InspectionClearanceElementPrivileges))]
        DocumentsManagement_InspectionClearance_Index = 450301,

        /// <summary>
        /// 已审核清关单据列表
        /// </summary>
        [Description("已审核清关单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionClearance/PassedApprovalList", ParentID = (int)ERPPage.DocumentsManagement_InspectionClearance, PageElementEnumType = typeof(InspectionClearanceElementPrivileges))]
        DocumentsManagement_InspectionClearance_PassedApprovalList = 450302,

        /// <summary>
        /// 结汇单据管理
        /// </summary>
        [Description("结汇单据管理"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.DocumentsManagement)]
        DocumentsManagement_InspectionExchange = 4504,

        /// <summary>
        /// 待审核结汇单据列表
        /// </summary>
        [Description("待审核结汇单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionExchange", ParentID = (int)ERPPage.DocumentsManagement_InspectionExchange, PageElementEnumType = typeof(InspectionExchangeElementPrivileges))]
        DocumentsManagement_InspectionExchange_Index = 450401,

        /// <summary>
        /// 已审核结汇单据列表
        /// </summary>
        [Description("已审核结汇单据列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/InspectionExchange/PassedApprovalList", ParentID = (int)ERPPage.DocumentsManagement_InspectionExchange, PageElementEnumType = typeof(InspectionExchangeElementPrivileges))]
        DocumentsManagement_InspectionExchange_PassedApprovalList = 450402,

        /// <summary>
        /// 港杂费发票
        /// </summary>
        [Description("港杂费发票"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/PortChargesInvoice", ParentID = (int)ERPPage.DocumentsManagement, PageElementEnumType = typeof(PortChargesInvoiceElementPrivileges))]
        DocumentsManagement_PortChargesInvoice = 4505,

        #endregion 单证管理

        #region 财务管理

        /// <summary>
        /// 财务管理
        /// </summary>
        [Description("财务管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        FinanceManagement = 47,

        /// <summary>
        /// 财务数据维护
        /// </summary>
        [Description("财务数据维护"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.FinanceManagement)]
        FinanceManagement_Maintain = 4701,

        /// <summary>
        /// 维护财务数据
        /// </summary>
        [Description("维护财务数据"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Finance", ParentID = (int)ERPPage.FinanceManagement_Maintain, PageElementEnumType = typeof(FinanceElementPrivileges))]
        FinanceManagement_Maintain_Index = 470101,

        /// <summary>
        /// 维护配件的财务数据
        /// </summary>
        [Description("维护配件的财务数据"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Finance_ProductFitting", ParentID = (int)ERPPage.FinanceManagement_Maintain, PageElementEnumType = typeof(Finance_ProductFittingElementPrivileges))]
        FinanceManagement_Maintain_ProductFitting_Index = 470102,

        /// <summary>
        /// 财务统计表
        /// </summary>
        [Description("财务统计表"), PageMenu(IconClass = "fa fa-list", URL = "", ParentID = (int)ERPPage.FinanceManagement)]
        FinanceManagement_StatisticsList = 4702,

        /// <summary>
        /// 工厂往来账查询
        /// </summary>
        [Description("工厂往来账查询"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Finance/Finance_Factory", ParentID = (int)ERPPage.FinanceManagement_StatisticsList, PageElementEnumType = typeof(FinanceElementPrivileges))]
        FinanceManagement_StatisticsList_Factory = 470201,

        /// <summary>
        /// 利润分析查询
        /// </summary>
        [Description("利润分析查询"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Finance/Finance_Analysis", ParentID = (int)ERPPage.FinanceManagement_StatisticsList, PageElementEnumType = typeof(FinanceElementPrivileges))]
        FinanceManagement_StatisticsList_Analysis = 470202,

        /// <summary>
        /// 自营出口明细查询
        /// </summary>
        [Description("自营出口明细查询"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Finance/Finance_SelfExportList", ParentID = (int)ERPPage.FinanceManagement_StatisticsList, PageElementEnumType = typeof(FinanceElementPrivileges))]
        FinanceManagement_StatisticsList_SelfExportList = 470203,

        /// <summary>
        /// 订单明细一览查询
        /// </summary>
        [Description("订单明细一览查询"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Finance/Finance_DetailList", ParentID = (int)ERPPage.FinanceManagement_StatisticsList, PageElementEnumType = typeof(FinanceElementPrivileges))]
        FinanceManagement_StatisticsList_DetailList = 470204,

        #endregion 财务管理

        #region 单证索引

        /// <summary>
        /// 单证索引
        /// </summary>
        [Description("单证索引"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        DocumentsIndexingManagement = 48,

        /// <summary>
        /// 待维护的单证索引
        /// </summary>
        [Description("待维护的单证索引"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/DocumentsIndexing", ParentID = (int)ERPPage.DocumentsIndexingManagement, PageElementEnumType = typeof(DocumentsIndexingElementPrivileges))]
        DocumentsIndexingManagement_Maintain = 4801,

        /// <summary>
        /// 待审核的单证索引
        /// </summary>
        [Description("待审核的单证索引"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/DocumentsIndexing/PendingApprovalList", ParentID = (int)ERPPage.DocumentsIndexingManagement, PageElementEnumType = typeof(DocumentsIndexingElementPrivileges))]
        DocumentsIndexingManagement_PendingApprovalList = 4802,

        /// <summary>
        /// 已审核的单证索引
        /// </summary>
        [Description("已审核的单证索引"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/DocumentsIndexing/PassedApprovalList", ParentID = (int)ERPPage.DocumentsIndexingManagement, PageElementEnumType = typeof(DocumentsIndexingElementPrivileges))]
        DocumentsIndexingManagement_PassedApprovalList = 4803,

        #endregion 单证索引

        #region 配件单证索引

        /// <summary>
        /// 配件单证索引
        /// </summary>
        [Description("配件单证索引"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        DocumentsIndexingManagement_ProductFitting = 49,

        /// <summary>
        /// 待维护的配件单证索引
        /// </summary>
        [Description("待维护的配件单证索引"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/DocumentsIndexing_ProductFitting", ParentID = (int)ERPPage.DocumentsIndexingManagement_ProductFitting, PageElementEnumType = typeof(DocumentsIndexingElementPrivileges))]
        DocumentsIndexingManagement_ProductFitting_Maintain = 4901,

        /// <summary>
        /// 待审核的配件单证索引
        /// </summary>
        [Description("待审核的配件单证索引"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/DocumentsIndexing_ProductFitting/PendingApprovalList", ParentID = (int)ERPPage.DocumentsIndexingManagement_ProductFitting, PageElementEnumType = typeof(DocumentsIndexingElementPrivileges))]
        DocumentsIndexingManagement_ProductFitting_PendingApprovalList = 4902,

        /// <summary>
        /// 已审核的配件单证索引
        /// </summary>
        [Description("已审核的配件单证索引"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/DocumentsIndexing_ProductFitting/PassedApprovalList", ParentID = (int)ERPPage.DocumentsIndexingManagement_ProductFitting, PageElementEnumType = typeof(DocumentsIndexingElementPrivileges))]
        DocumentsIndexingManagement_ProductFitting_PassedApprovalList = 4903,

        #endregion 配件单证索引

        #region 客户管理

        /// <summary>
        /// 客户管理
        /// </summary>
        [Description("客户管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        CustomerManagement = 50,

        /// <summary>
        /// 客户信息列表
        /// </summary>
        [Description("客户信息列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Customer", ParentID = (int)ERPPage.CustomerManagement, PageElementEnumType = typeof(CustomerListElementPrivileges))]
        CustomerManagement_List = 5001,

        /// <summary>
        /// Rep列表
        /// </summary>
        [Description("Rep列表"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Rep", ParentID = (int)ERPPage.CustomerManagement, PageElementEnumType = typeof(RepElementPrivileges))]
        RepManagement_List = 5002,

        #endregion 客户管理

        #region 船代管理

        /// <summary>
        /// 船代管理
        /// </summary>
        [Description("船代管理"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        ShipmentManagement = 55,

        /// <summary>
        /// 船代公司信息管理
        /// </summary>
        [Description("船代公司信息管理"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/ShipmentAgency", ParentID = (int)ERPPage.ShipmentManagement, PageElementEnumType = typeof(ShipmentAgencyElementPrivileges))]
        ShipmentManagement_Agencies = 5501,

        /// <summary>
        /// 箱柜设置
        /// </summary>
        [Description("箱柜设置"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Cabinet", ParentID = (int)ERPPage.ShipmentManagement, PageElementEnumType = typeof(CabinetElementPrivileges))]
        ShipmentManagement_Cabinet = 5502,

        #endregion 船代管理

        #region 系统设置

        /// <summary>
        /// 系统设置
        /// </summary>
        [Description("系统设置"), PageMenu(IconClass = "glyphicon glyphicon-th-large")]
        SystemManagement = 60,

        /// <summary>
        /// 数据字典
        /// </summary>
        [Description("数据字典"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Dictionary", ParentID = (int)ERPPage.SystemManagement)]
        SystemManagement_Dictionaries = 6001,

        /// <summary>
        /// HS Code设置
        /// </summary>
        [Description("HS Code设置"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/HS", ParentID = (int)ERPPage.SystemManagement, PageElementEnumType = typeof(HsElementsPrivileges))]
        SystemManagement_HSCode = 6002,

        /// <summary>
        /// 后台用户管理
        /// </summary>
        [Description("后台用户管理"), PageMenu(IconClass = "fa fa-list", ParentID = (int)ERPPage.SystemManagement)]
        SystemManagement_SystemUsers = 6003,

        /// <summary>
        /// 账号管理
        /// </summary>
        [Description("账号管理"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Auth/Admin", ParentID = (int)ERPPage.SystemManagement_SystemUsers)]
        SystemManagement_SystemUsers_Accounts = 600301,

        /// <summary>
        /// 角色管理
        /// </summary>
        [Description("角色管理"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Auth/Role", ParentID = (int)ERPPage.SystemManagement_SystemUsers)]
        SystemManagement_SystemUsers_Roles = 600302,

        /// <summary>
        /// 日志查看
        /// </summary>
        [Description("日志查看"), PageMenu(IconClass = "fa fa-file-text-o", URL = "/Account/Logs", ParentID = (int)ERPPage.SystemManagement_SystemUsers)]
        SystemManagement_SystemUsers_Logs = 600303,

        #endregion 系统设置
    }
}