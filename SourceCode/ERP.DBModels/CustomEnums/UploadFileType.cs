using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    /// <summary>
    /// 上传文件的类型
    /// </summary>
    public enum UploadFileType
    {
        /// <summary>
        /// 采购合同
        /// </summary>
        PurchaseContract = 0,

        /// <summary>
        /// 代购合同
        /// </summary>
        OutContract = 1,

        /// <summary>
        /// 寄样管理上传附件
        /// </summary>
        SampleUpLoad = 2,

        /// <summary>
        /// 第三方验厂
        /// </summary>
        InspectionAuditNotice = 3,

        /// <summary>
        /// 生产计划
        /// </summary>
        ProducePlan = 4,

        /// <summary>
        /// 图片文件
        /// </summary>
        Images = 5,

        /// <summary>
        /// 单证管理->报检管理->委托书
        /// </summary>
        InspectionReceiptCommission = 6,

        /// <summary>
        /// 第三方检验
        /// </summary>
        InspectionDetectNotice = 7,

        /// <summary>
        /// 报价单
        /// </summary>
        Quote = 8,

        /// <summary>
        /// 包装资料管理
        /// </summary>
        Packs = 9,

        /// <summary>
        /// 单证管理->报检管理->上传凭条
        /// </summary>
        InspectionReceiptUploadReceipt = 10,

        /// <summary>
        /// 第三方验货
        /// </summary>
        ThirdPartyVerification = 11,

        /// <summary>
        /// 三期QC验货——前期
        /// </summary>
        ThreeTimesQC_One = 12,

        /// <summary>
        /// 三期QC验货——中期
        /// </summary>
        ThreeTimesQC_Two = 13,

        /// <summary>
        /// 三期QC验货——尾期
        /// </summary>
        ThreeTimesQC_Three = 14,

        /// <summary>
        /// 单证管理->报检管理->发送工厂
        /// </summary>
        InspectionReceiptUploadSended = 15,

        /// <summary>
        /// 单证管理->报关管理->申报要素
        /// </summary>
        InspectionCustomsDeclareElements = 16,

        /// <summary>
        /// 单证管理->报关管理->报关委托书
        /// </summary>
        InspectionCustomsCommission = 17,

        /// <summary>
        /// 单证管理->清关管理->附件
        /// </summary>
        InspectionClearance = 18,

        /// <summary>
        /// 单证管理->结汇管理->附件
        /// </summary>
        InspectionExchange = 19,

        /// <summary>
        /// 包装资料->已审核数据列表->标签上传附件
        /// </summary>
        PacksUploadFiles = 20,

        /// <summary>
        /// 出运通知——配仓单
        /// </summary>
        ShipmentNotification_DeliveryNotification = 21,

        /// <summary>
        /// 包装资料——上传的唛头资料
        /// </summary>
        ShippingMark = 22,

        /// <summary>
        /// 包装资料——生成的唛头资料JPG。LinkID = 采购合同的产品ID
        /// </summary>
        ShippingMark_CreateJPG = 23,

        /// <summary>
        /// 包装资料——生成的唛头资料PDF。LinkID = 采购合同ID
        /// </summary>
        ShippingMark_CreatePDF = 24,

        /// <summary>
        /// 第三方抽检
        /// </summary>
        InspectionSamplingNotice = 25,

        /// <summary>
        /// 港杂费发票
        /// </summary>
        PortChargesInvoice = 26,


        /// <summary>
        /// 单证管理->清关管理->FCR附件
        /// </summary>
        InspectionClearance_FCR = 27,

        ShippingMark_For188 = 28,

        BuyingConfirmation = 29,

        /// <summary>
        /// 清关文件修改的
        /// </summary>
        InspectionClearance_Modify = 30,

        /// <summary>
        /// 结汇文件修改的
        /// </summary>
        InspectionExchange_Modify = 31,

        /// <summary>
        /// 采购合同：请款合同
        /// </summary>
        PurchaseContract_MakeMoney = 32,

        /// <summary>
        /// 上传销售合同
        /// </summary>
        Order = 33,

        /// <summary>
        /// 单证管理->报关管理->报检凭条
        /// </summary>
        InspectionCustomsUploadReceipt = 34,
    }
}