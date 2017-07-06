using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    /// <summary>
    /// 页面类型
    /// </summary>
    public enum PageTypeEnum
    {
        /// <summary>
        /// 新建。值为0
        /// </summary>
        Add,

        /// <summary>
        /// 编辑。值为1
        /// </summary>
        Edit,

        /// <summary>
        /// 详细。值为2
        /// </summary>
        Details,

        /// <summary>
        /// 审核。值为3
        /// </summary>
        Check,

        /// <summary>
        /// 审批。值为4
        /// </summary>
        Approval,

        /// <summary>
        /// 复制报价单。值为5
        /// </summary>
        Copy,

        /// <summary>
        /// 列表。值为6
        /// </summary>
        List,

        /// <summary>
        /// 待审核列表。值为7
        /// </summary>
        PendingCheckList,

        /// <summary>
        /// 已审核列表。值为8
        /// </summary>
        PassedCheckList,

        /// <summary>
        /// 待审批列表。值为9
        /// </summary>
        PendingApproval,

        /// <summary>
        /// 已审批列表。值为10
        /// </summary>
        PassedApproval,

        /// <summary>
        /// 拉柜费用登记列表。值为11
        /// </summary>
        RegisterFeesList,

        /// <summary>
        /// 拉柜费用登记。值为12
        /// </summary>
        RegisterFees,

        /// <summary>
        /// 第三方验厂列表。值为13
        /// </summary>
        InspectionAuditNoticeList,

        /// <summary>
        /// 第三方检测列表。值为14
        /// </summary>
        InspectionDetectNoticeList,

        /// <summary>
        /// 第三方抽检列表。值为15
        /// </summary>
        InspectionSamplingNoticeList,

        /// <summary>
        /// 销售管理->寄样管理->待打样品列表
        /// </summary>
        ManufacturingPage,

        /// <summary>
        /// 销售管理->寄样管理->已打样品列表
        /// </summary>
        ManufacturedPage,

        /// <summary>
        /// 销售管理->寄样管理->待寄出样品列表
        /// </summary>
        SendingPage,

        /// <summary>
        /// 销售管理->寄样管理->已寄出样品列表
        /// </summary>
        SendedPage,

        /// <summary>
        /// 待维护列表
        /// </summary>
        PendingMaintenanceList,

        /// <summary>
        /// 工厂往来账查询
        /// </summary>
        Finance_Factory,

        /// <summary>
        /// 利润分析查询
        /// </summary>
        Finance_Analysis,

        /// <summary>
        /// 自营出口明细查询
        /// </summary>
        Finance_SelfExportList,

        /// <summary>
        /// 订单明细一览查询
        /// </summary>
        Finance_DetailList,

        Upload,

        /// <summary>
        /// 回复意见
        /// </summary>
        ReplySuggest,

        /// <summary>
        /// 待审核的中期QC
        /// </summary>
        PendingCheckList_Two,

        /// <summary>
        /// 待审核的尾期QC
        /// </summary>
        PendingCheckList_Three,
    }

    /// <summary>
    /// 报价单模板类型
    /// </summary>
    public enum QuotProductTypeEnum
    {
        //Public,
        DG,

        F20,
        F90,
        Form1,
        Form2,
        S13,
        S135,
        S164,
        S165,
        S188,
        S235,
        S237,
        S239,
        S288,
        S56,
        S56_2,
        S60,
        S220,
        S05,
        DT,
    }

    /// <summary>
    /// 客户选择的类型
    /// </summary>
    public enum SelectCustomerEnum
    {
        新客户,

        DG,
        F20,
        //F90,
        //Form1,
        //Form2,
        S13,
        S135,
        S164,
        //S165,
        S188,
        S235,
        //S237,
        //S239,
        S288,
        S56,
        //S56_2,
        S60,
        S05,
        S220,
        S10,
        S52,
        S259,
        DT,
    }

    /// <summary>
    /// 生成Excel类型
    /// </summary>
    public enum MakerTypeEnum
    {
        /// <summary>
        /// 采购合同
        /// </summary>
        PurchaseContract,

        /// <summary>
        /// 代购合同
        /// </summary>
        OutSourcingContract,

        /// <summary>
        /// 报检——装箱单
        /// </summary>
        InspectionReceipt_PackingList,

        /// <summary>
        /// 报检——发票
        /// </summary>
        InspectionReceipt_CommercialInvoice,

        /// <summary>
        /// 报检——销售合同
        /// </summary>
        InspectionReceipt_SaleContract,

        /// <summary>
        /// 报检——委托书
        /// </summary>
        InspectionReceipt_Commission,

        /// <summary>
        /// 报检——报检首页
        /// </summary>
        InspectionReceipt_Home,

        /// <summary>
        /// 报关——装箱单
        /// </summary>
        InspectionCustoms_PackingList,

        /// <summary>
        /// 报关——发票
        /// </summary>
        InspectionCustoms_CommercialInvoice,

        /// <summary>
        /// 报关——销售合同
        /// </summary>
        InspectionCustoms_SaleContract,

        /// <summary>
        /// 报关——委托书
        /// </summary>
        InspectionCustoms_Commission,

        /// <summary>
        /// 清关——发票
        /// </summary>
        InspectionClearance_CommercialInvoice,

        /// <summary>
        /// 清关——装箱单
        /// </summary>
        InspectionClearance_PackingList,

        /// <summary>
        /// 清关——信用证文件
        /// </summary>
        InspectionClearance_CreditNumber1,

        /// <summary>
        /// 清关——信用证文件
        /// </summary>
        InspectionClearance_CreditNumber2,

        /// <summary>
        /// 清关——信用证文件
        /// </summary>
        InspectionClearance_CreditNumber3,

        /// <summary>
        /// 清关——信用证文件
        /// </summary>
        InspectionClearance_CreditNumber4,

        /// <summary>
        /// 清关——信用证文件
        /// </summary>
        InspectionClearance_CreditNumber5,

        /// <summary>
        /// 清关——不含木质包装申明
        /// </summary>
        InspectionClearance_ExcludingWoodPackagingDeclaration,

        /// <summary>
        /// 清关——原产地证明
        /// </summary>
        InspectionClearance_CertificateOfOrigin,

        /// <summary>
        /// 清关——受益人声明
        /// </summary>
        InspectionClearance_BeneficiaryStatement,

        InspectionClearance_PackingListBuContainer,

        /// <summary>
        /// 结汇——发票
        /// </summary>
        InspectionExchange_CommercialInvoice,

        /// <summary>
        /// 结汇——装箱单
        /// </summary>
        InspectionExchange_PackingList,

        /// <summary>
        /// 结汇——不含木质包装申明
        /// </summary>
        InspectionExchange_ExcludingWoodPackagingDeclaration,

        /// <summary>
        /// 结汇——原产地证明
        /// </summary>
        InspectionExchange_CertificateOfOrigin,

        /// <summary>
        /// 结汇——受益人声明
        /// </summary>
        InspectionExchange_BeneficiaryStatement,

        InspectionExchange_PackingListBuContainer,

        /// <summary>
        /// 财务
        /// </summary>
        Finance,

        /// <summary>
        /// 单证索引——合约（报关的销售合同）
        /// </summary>
        DocumentsIndexing_SaleOrder,

        /// <summary>
        /// 单证索引——港杂费发票
        /// </summary>
        DocumentsIndexing_PortChargesInvoice,

        /// <summary>
        /// 单证索引——FCR
        /// </summary>
        DocumentsIndexing_FCR,

        /// <summary>
        /// 单证索引——配仓单
        /// </summary>
        DocumentsIndexing_Notification,

        /// <summary>
        /// 单证索引——合同附件
        /// </summary>
        DocumentsIndexing_Purchase,

        /// <summary>
        /// 单证索引——查看所有单据
        /// </summary>
        DocumentsIndexing_AllDocuments,

        /// <summary>
        /// 销售订单——Buying Confirmation
        /// </summary>
        Order_BuyingConfirmation,

        /// <summary>
        /// 单证索引——请款合同/货款明细
        /// </summary>
        DocumentsIndexing_Purchase_MakeMoney,

        /// <summary>
        /// 配件合同
        /// </summary>
        PurchaseContract_ProductFitting,
    }

    /// <summary>
    /// 代印类型
    /// </summary>
    public enum OutTypeEnum
    {
        S333,
    }

    /// <summary>
    /// 电子邮件类型
    /// </summary>
    public enum MailType
    {
        /// <summary>
        /// 报价单
        /// </summary>
        Quot,

        /// <summary>
        /// 寄样
        /// </summary>
        SendSample,

        /// <summary>
        /// 采购合同
        /// </summary>
        PurchaseContract,

        /// <summary>
        /// 三期QC
        /// </summary>
        ThreeTimesQC,
    }

    /// <summary>
    /// 第三方检测类型
    /// </summary>
    public enum InspectionTypeEnum
    {
        /// <summary>
        /// 验厂
        /// </summary>
        [Description("验厂")]
        AuditNotice = 1,

        /// <summary>
        /// 检测
        /// </summary>
        [Description("检测")]
        DetectNotice = 2,

        /// <summary>
        /// 抽检
        /// </summary>
        [Description("抽检")]
        SamplingNotice = 3,
    }

    /// <summary>
    /// 装柜类型
    /// </summary>
    public enum GatherTypeEnum
    {
        /// <summary>
        /// 拉柜
        /// </summary>
        PullCabinet = 1,

        /// <summary>
        /// 入仓
        /// </summary>
        PutInStorage = 2,
    }

    /// <summary>
    /// 销售订单的产品来源
    /// </summary>
    public enum OrderProductSource
    {

        /// <summary>
        /// 已经保存的销售订单产品
        /// </summary>
        Default = 0,

        /// <summary>
        /// 来源于大清单产品
        /// </summary>
        FromProduct = 1,

        /// <summary>
        /// 来源于报价单的产品
        /// </summary>
        FromQuoteProduct = 2,
    }

    /// <summary>
    /// 选择销售订单作为数据源的适用场合
    /// </summary>
    public enum SelectOrdersOccasion
    {
        /// <summary>
        /// 寄样管理->新建样品单
        /// </summary>
        NewSample = 1,

        /// <summary>
        /// 采购管理->新建采购合同
        /// </summary>
        NewPurContract = 2
    }

    /// <summary>
    /// 客户的FreightRate类型枚举
    /// </summary>
    public enum CustomerFreightRateTypeEnum
    {
        /// <summary>
        /// 全部港口
        /// </summary>

        AllPort = 0,

        /// <summary>
        /// 单一港口
        /// </summary>
        SinglePort = 1,
    }

    /// <summary>
    /// 数据标签枚举
    /// </summary>
    public enum DataFlagEnum
    {
        /// <summary>
        /// 产品标签
        /// </summary>
        [Description("产品标签")]
        Product = 2,

        /// <summary>
        /// 内盒标签
        /// </summary>
        [Description("内盒标签")]
        InnerPack = 3,

        /// <summary>
        /// 外箱标签
        /// </summary>
        [Description("外箱标签")]
        OuterPack = 4,
    }

    /// <summary>
    /// 选择报价单的类型
    /// </summary>
    public enum SelectQuoteOccasionEnum
    {
        /// <summary>
        /// 默认的：销售订单（查询的条件是：已确认的报价单）
        /// </summary>
        Default = 0,

        /// <summary>
        /// 寄样（查询的条件是：不包含草稿和已废除的报价单）
        /// </summary>
        Sample = 1,
    }

    /// <summary>
    /// 预录单状态枚举
    /// </summary>
    public enum PreRecordedStatusEnum
    {
        /// <summary>
        /// 空
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 是
        /// </summary>
        Yes = 1,

        /// <summary>
        /// 否
        /// </summary>
        No = 2,
    }

    /// <summary>
    /// 寄样管理付款方式
    /// </summary>
    public enum PaymentWay
    {
        /// <summary>
        /// 1=到付
        /// </summary>
        [Description("到付")]
        SampleStatus1 = 1,

        /// <summary>
        /// 2=预付
        /// </summary>
        [Description("预付")]
        SampleStatus2 = 2,
    }

    /// <summary>
    /// 工厂类型
    /// </summary>
    public enum FactoryDataFlagEnum
    {
        /// <summary>
        /// 产品。值为1
        /// </summary>
        [Description("产品工厂")]
        Product = 1,

        /// <summary>
        /// 代印工厂。值为1
        /// </summary>
        [Description("代印工厂")]
        OutSourcing = 2,
    }

    /// <summary>
    /// 运输方式（报关里面的）、出运方式（销售订单里面的）
    /// </summary>
    public enum TransportTypeEnum
    {
        /// <summary>
        /// BY OCEAN
        /// </summary>
        [Description("BY OCEAN")]
        ByOcean = 1,

        /// <summary>
        /// BY AIR
        /// </summary>
        [Description("BY AIR")]
        ByAir = 2,
    }

    /// <summary>
    /// 收款方式枚举
    /// </summary>
    public enum ReceivablesEnum
    {
        /// <summary>
        /// 预付（定金）+T/T
        /// </summary>
        [Description("预付（定金）+T/T")]
        Prepayment = 1,

        /// <summary>
        /// T/T
        /// </summary>
        [Description("T/T")]
        TT = 2,

        /// <summary>
        /// L/C+ 天
        /// </summary>
        [Description("L/C+ 天")]
        LC = 3,

        /// <summary>
        /// 支票托收
        /// </summary>
        [Description("支票托收")]
        CheckCollection = 4,
    }

    /// <summary>
    /// 报关单位枚举
    /// </summary>
    public enum CustomsUnitTypeEnum
    {
        /// <summary>
        /// 我司
        /// </summary>
        [Description("我司")]
        OurCompany = 1,

        /// <summary>
        /// 工厂
        /// </summary>
        [Description("工厂")]
        Factory = 2,
    }

    /// <summary>
    /// 产品版权枚举
    /// </summary>
    public enum ProductCopyRightTypeEnum
    {
        /// <summary>
        /// 我方
        /// </summary>
        [Description("我方")]
        OurCompany = 1,

        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        Customer = 2,

        /// <summary>
        /// 工厂
        /// </summary>
        [Description("工厂")]
        Factory = 3,
    }

    /// <summary>
    /// 付款方式枚举
    /// </summary>
    public enum PaymentTypeEnum
    {
        /// <summary>
        /// 结关后
        /// </summary>
        AfterClearance = 1,

        /// <summary>
        /// 进仓后
        /// </summary>
        AfterShipToStock = 2,

        /// <summary>
        /// 发货后
        /// </summary>
        AfterDelivery = 3,

        /// <summary>
        /// 收到正本单据后
        /// </summary>
        AfterReceivingTheOriginalDocuments = 4,

        /// <summary>
        /// 收到正本单据扫描件后
        /// </summary>
        AfterReceivingTheOriginalDocumentsScannedCopy = 5,

        /// <summary>
        /// L/C SIGHT
        /// </summary>
        LC_SIGHT = 6,

        /// <summary>
        /// 收到正本FCR后
        /// </summary>
        AfterReceiptOfTheOriginalFCR = 7,
    }

    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum CommonStatusEnum
    {
        /// <summary>
        /// 空
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 是
        /// </summary>
        Yes = 1,

        /// <summary>
        /// 否
        /// </summary>
        No = 2,
    }

    /// <summary>
    /// 国家
    /// </summary>
    public enum CountryEnum
    {
        /// <summary>
        /// United States
        /// </summary>
        [Description("United States")]
        USA = 4,
    }

    /// <summary>
    /// 订舱：整柜还是散货
    /// </summary>
    public enum CabinetTypeEnum
    {
        /// <summary>
        /// 整柜
        /// </summary>
        [Description("整柜")]
        WholeCabinet = 1,

        /// <summary>
        /// 散货
        /// </summary>
        [Description("散货")]
        BulkGoods = 2,
    }

    /// <summary>
    /// 模块类型
    /// </summary>
    public enum ModuleTypeEnum
    {
        /// <summary>
        /// 配件产品
        /// </summary>
        ProductFitting = 0,

        /// <summary>
        /// 产品
        /// </summary>
        Product = 1,

        /// <summary>
        /// 报价单
        /// </summary>
        Quote = 10,

        /// <summary>
        /// 销售订单
        /// </summary>
        Order = 20,

        /// <summary>
        /// 采购合同
        /// </summary>
        PurchaseContract = 30,

        ProductMixed = 40,
    }

    public enum PurchaseProduct_CarbinetTypeEnum
    {
        /// <summary>
        /// 标准箱
        /// </summary>
        [Description("标准箱")]
        StandardContainer = 1,

        /// <summary>
        /// 非标准箱
        /// </summary>
        [Description("非标准箱")]
        NonStandardContainer = 2,

        /// <summary>
        /// 小箱
        /// </summary>
        [Description("小箱")]
        SmallBox = 3,
    }

    /// <summary>
    /// 采购合同类型
    /// </summary>
    public enum ContractTypeEnum
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 配件合同
        /// </summary>
        ProductFitting = 1,

    }

    /// <summary>
    /// 单证索引类型
    /// </summary>
    public enum DocumentsIndexingTypeEnum
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,

        /// <summary>
        /// 配件合同
        /// </summary>
        ProductFitting = 1,

    }
}