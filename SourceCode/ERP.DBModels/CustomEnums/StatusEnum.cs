using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    /// <summary>
    /// 产品状态枚举
    /// </summary>
    public enum ProductStatusEnum : short
    {
        /// <summary>
        /// 正常在使用状态枚举
        /// </summary>
        Normal,

        /// <summary>
        /// 草稿状态枚举
        /// </summary>
        Draft,
    }

    /// <summary>
    /// 报价单状态枚举
    /// </summary>
    public enum QuoteStatusEnum
    {
        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 重新报价。值为6
        /// </summary>
        [Description("重新报价")]
        ReQutes = 6,

        /// <summary>
        /// 已发送客户。值为7
        /// </summary>
        [Description("已发送客户")]
        HadSend = 7,

        /// <summary>
        /// 已确认。值为8
        /// </summary>
        [Description("已确认")]
        HadConfirm = 8,

        /// <summary>
        /// 已作废。值为9
        /// </summary>
        [Description("已作废")]
        HadInvalid = 9,
    }

    /// <summary>
    /// 寄样管理数据状态枚举
    /// </summary>
    public enum SampleStatus
    {
        /// <summary>
        /// 1=待接单
        /// </summary>
        [Description("待接单")]
        SampleStatus1 = 1,

        /// <summary>
        /// 2=已接单
        /// </summary>
        [Description("已接单")]
        SampleStatus2 = 2,

        /// <summary>
        /// 3=正在生产
        /// </summary>
        [Description("正在生产")]
        SampleStatus3 = 3,

        /// <summary>
        /// 4=生产完成
        /// </summary>
        [Description("生产完成")]
        SampleStatus4 = 4,

        /// <summary>
        /// 5=样品已确认
        /// </summary>
        [Description("样品已确认")]
        SampleStatus5 = 5,

        /// <summary>
        /// 6=待寄出
        /// </summary>
        [Description("待寄出")]
        SampleStatus6 = 6,

        /// <summary>
        /// 7=汇集中
        /// </summary>
        [Description("汇集中")]
        SampleStatus7 = 7,

        /// <summary>
        /// 8=已寄出
        /// </summary>
        [Description("已寄出")]
        SampleStatus8 = 8,

        /// <summary>
        /// 9=未签收
        /// </summary>
        [Description("未签收")]
        SampleStatus9 = 9,

        /// <summary>
        /// 10=已签收
        /// </summary>
        [Description("已签收")]
        SampleStatus10 = 10,
    }

    /// <summary>
    /// 销售订单状态枚举
    /// </summary>
    public enum OrderStatusEnum
    {
        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        ///// <summary>
        ///// 待核算。值为2
        ///// </summary>
        //[Description("待核算")]
        //PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        ///// <summary>
        ///// 核算未通过。值为4
        ///// </summary>
        //[Description("核算未通过")]
        //NotPassCheck = 4,

        /// <summary>
        /// 待审批。值为5
        /// </summary>
        [Description("待审批")]
        PendingApproval = 5,

        ///// <summary>
        ///// 审批中。值为6
        ///// </summary>
        //[Description("审批中")]
        //ApprovalCheck = 6,

        /// <summary>
        /// 审批不通过。值为7
        /// </summary>
        [Description("审批不通过")]
        NotPassApproval = 7,

        /// <summary>
        /// 审批已通过。值为8
        /// </summary>
        [Description("审批已通过")]
        PassedApproval = 8,
    }

    /// <summary>
    /// 采购合同状态枚举
    /// </summary>
    public enum PurchaseStatusEnum
    {
        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 合同已上传。值为6
        /// </summary>
        [Description("合同已上传")]
        ContractUploaded = 6,

        /// <summary>
        /// 合同已发送。值为7
        /// </summary>
        [Description("合同已发送")]
        ContractSent = 7,
    }

    /// <summary>
    /// 包装资料状态枚举
    /// </summary>
    public enum PurchasePacksStatusEnum
    {
        /// <summary>
        /// 待维护
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 1,

        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        OutLine = 2,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        PendingCheck = 3,

        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 标签已通知
        /// </summary>
        [Description("标签已通知")]
        HadNotification = 6,

        /// <summary>
        /// 标签已确认
        /// </summary>
        [Description("标签已确认")]
        HadConfirm = 7,

        /// <summary>
        /// 大货印刷完成
        /// </summary>
        [Description("大货印刷完成")]
        HadFinish = 8,

        /// <summary>
        /// 已上传
        /// </summary>
        [Description("已上传")]
        HadUploaded = 9,
    }

    /// <summary>
    /// 代购合同状态枚举
    /// </summary>
    public enum OutContractStatusEnum
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 3,

        /// <summary>
        /// 审核已通过
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 4,

        /// <summary>
        /// 合同已发送
        /// </summary>
        [Description("合同已发送")]
        ContractSent = 5,

        /// <summary>
        /// 合同已发送
        /// </summary>
        [Description("合同已上传")]
        ContractUploaded = 6,
    }

    /// <summary>
    /// 出运管理->出运明细列表数据状态枚举
    /// </summary>
    public enum EncasementStatusEnum
    {
        /// <summary>
        /// 待维护
        /// </summary>
        [Description("待维护")]
        PendingEdit = 1,

        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 2,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        PendingCheck = 3,

        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5
    }

    /// <summary>
    /// 订舱信息状态枚举
    /// </summary>
    public enum ShipmentOrderStatusEnum
    {
        /// <summary>
        /// 待订舱。值为0
        /// </summary>
        [Description("待订舱")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 保存柜号和封箱号。值为6
        /// </summary>
        [Description("审核已通过")]
        Save = 6,
    }

    /// <summary>
    /// 出运通知状态枚举
    /// </summary>
    public enum ShipmentNotificationStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,
    }

    /// <summary>
    /// 拉柜费用登记状态枚举
    /// </summary>
    public enum ShipmentRegisterFeesStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        ///// <summary>
        ///// 待审核。值为2
        ///// </summary>
        //[Description("待审核")]
        //PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        ///// <summary>
        ///// 审核未通过。值为4
        ///// </summary>
        //[Description("审核未通过")]
        //NotPassCheck = 4,

        /// <summary>
        /// 已登记。值为5
        /// </summary>
        [Description("已登记")]
        PassedCheck = 5,
    }

    /// <summary>
    /// 第三方检测状态枚举
    /// </summary>
    public enum InspectionStatusEnum
    {
        /// <summary>
        /// 待录入
        /// </summary>
        [Description("待录入")]
        PendingInput = 0,

        ///// <summary>
        ///// 草稿
        ///// </summary>
        //[Description("草稿")]
        //OutLine = 1,

        /// <summary>
        ///待发送
        /// </summary>
        [Description("待发送")]
        PendingSent = 2,

        /// <summary>
        /// 已发送
        /// </summary>
        [Description("已发送")]
        AlreadySent = 3,

        /// <summary>
        /// 未通过
        /// </summary>
        [Description("未通过")]
        NotPass = 4,

        /// <summary>
        /// 通过
        /// </summary>
        [Description("通过")]
        Passed = 5,
    }

    /// <summary>
    /// 第三方验货状态枚举
    /// </summary>
    public enum ThirdPartyVerificationStatusEnum
    {
        /// <summary>
        /// 待上传。值为0
        /// </summary>
        [Description("待上传")]
        PendingUpload = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 已上传。值为2
        /// </summary>
        [Description("已上传")]
        HadUpload = 2,
    }

    /// <summary>
    /// 生产计划的状态枚举
    /// </summary>
    public enum ProducePlanStatusEnum
    {
        /// <summary>
        /// 待上传
        /// </summary>
        [Description("待上传")]
        PendingUpload = 1,

        /// <summary>
        /// 待提交
        /// </summary>
        [Description("待提交")]
        PendingSubmit = 2,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        PendingCheck = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        ///已上传
        /// </summary>
        [Description("已上传")]
        HadUpload = 6,
    }

    /// <summary>
    /// 三期QC验货状态枚举
    /// </summary>
    public enum ThreeTimesQCStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,
        
        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 已上传。值为6
        /// </summary>
        [Description("已上传")]
        HadUpload = 6,

        /// <summary>
        /// 查看PDF文件。值为7
        /// </summary>
        [Description("查看PDF文件")]
        ViewPDF = 7,

        /// <summary>
        /// 回复意见。值为8
        /// </summary>
        [Description("回复意见")]
        ReplySuggest = 8,
        
        /// <summary>
        /// 合同已发送。值为9
        /// </summary>
        [Description("合同已发送")]
        ContractSent = 9,
    }

    /// <summary>
    /// 出货样状态枚举
    /// </summary>
    public enum ShipmentSampleStatusEnum
    {
        /// <summary>
        /// 待收回。值为0
        /// </summary>
        [Description("待收回")]
        PendingRecovery = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 已收回。值为2
        /// </summary>
        [Description("已收回")]
        HadRecovery = 2,
    }

    /// <summary>
    /// 唛头资料状态枚举
    /// </summary>
    public enum ShippingMarkStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 发给客户确认
        /// </summary>
        [Description("发给客户确认")]
        SendToCustomer = 6,

        /// <summary>
        /// 客户确认排版
        /// </summary>
        [Description("客户确认排版")]
        CustomerConfirm = 7,

        /// <summary>
        /// 通知工厂排版
        /// </summary>
        [Description("通知工厂排版")]
        NotificationFactory = 8,

        /// <summary>
        /// 我司确认排版
        /// </summary>
        [Description("我司确认排版")]
        OurConfirm = 9,

        /// <summary>
        /// 通知大货印刷
        /// </summary>
        [Description("通知大货印刷")]
        NotificationPrinted = 10,

        /// <summary>
        /// 已上传
        /// </summary>
        [Description("已上传")]
        UpLoaded = 11,
    }

    /// <summary>
    /// 财务状态枚举
    /// </summary>
    public enum FinanceStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 已维护。值为2
        /// </summary>
        [Description("已维护")]
        HadMaintenance = 2,
    }

    /// <summary>
    /// 报检状态枚举
    /// </summary>
    public enum InspectionReceiptStatusEnum
    {
        /// <summary>
        /// 待报检
        /// </summary>
        [Description("待报检")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 已发送
        /// </summary>
        [Description("已发送")]
        Sended = 6,

        /// <summary>
        /// 已上传
        /// </summary>
        [Description("已上传")]
        Uploaded = 7,
    }

    /// <summary>
    /// 报关状态枚举
    /// </summary>
    public enum InspectionCustomsStatusEnum
    {
        /// <summary>
        /// 待报关
        /// </summary>
        [Description("待报关")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,
    }

    /// <summary>
    /// 清关状态枚举
    /// </summary>
    public enum InspectionClearanceStatusEnum
    {
        /// <summary>
        /// 待清关
        /// </summary>
        [Description("待清关")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,

        /// <summary>
        /// 已上传FCR
        /// </summary>
        [Description("已上传FCR")]
        UploadedFCR = 6,
    }

    /// <summary>
    /// 结汇状态枚举
    /// </summary>
    public enum InspectionExchangeStatusEnum
    {
        /// <summary>
        /// 待结汇
        /// </summary>
        [Description("待结汇")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,
    }

    /// <summary>
    /// 单证索引状态枚举
    /// </summary>
    public enum DocumentsIndexingStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        /// <summary>
        /// 待审核。值为2
        /// </summary>
        [Description("待审核")]
        PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        /// <summary>
        /// 审核未通过。值为4
        /// </summary>
        [Description("审核未通过")]
        NotPassCheck = 4,

        /// <summary>
        /// 审核已通过。值为5
        /// </summary>
        [Description("审核已通过")]
        PassedCheck = 5,
    }


    /// <summary>
    /// 港杂费发票状态枚举
    /// </summary>
    public enum PortChargesInvoiceStatusEnum
    {
        /// <summary>
        /// 待维护。值为0
        /// </summary>
        [Description("待维护")]
        PendingMaintenance = 0,

        /// <summary>
        /// 草稿。值为1
        /// </summary>
        [Description("草稿")]
        OutLine = 1,

        ///// <summary>
        ///// 待审核。值为2
        ///// </summary>
        //[Description("待审核")]
        //PendingCheck = 2,

        ///// <summary>
        ///// 审核中。值为3
        ///// </summary>
        //[Description("审核中")]
        //Checking = 3,

        ///// <summary>
        ///// 审核未通过。值为4
        ///// </summary>
        //[Description("审核未通过")]
        //NotPassCheck = 4,

        /// <summary>
        /// 已维护。值为5
        /// </summary>
        [Description("已维护")]
        PassedCheck = 5,
    }
}