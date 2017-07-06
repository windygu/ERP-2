using ERP.Models.Common;
using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ERP.Models.Sample
{
    /// <summary>
    /// 寄样管理VM
    /// </summary>
    public class DTOSample
    {
        /// <summary>
        /// 页面类型：1=新建；2=查看；3=编辑；4=审核；5=编辑样品单
        /// </summary>
        public int PageTypeID { get; set; }

        /// <summary>
        /// 页面标题：1=新建；2=查看；3=编辑；4=审核；5=编辑样品单
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 寄样信息自编号
        /// </summary>
        public int SSID { get; set; }

        /// <summary>
        /// 报价单自编号
        /// </summary>
        public int QTID { get; set; }

        /// <summary>
        /// 销售订单自编号
        /// </summary>
        public int PHID { get; set; }

        /// <summary>
        /// 创建方式：1=手工创建；2=报价；3=销售订单
        /// </summary>
        public int CreateWay { get; set; }

        /// <summary>
        /// 工厂自编号
        /// </summary>
        public int FactureID { get; set; }

        /// <summary>
        /// 客户自编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 样品状态ID
        /// </summary>
        public int SampleStatusID { get; set; }

        /// <summary>
        /// 样品状态
        /// </summary>
        public string SampleStatus { get; set; }

        /// <summary>
        /// 工厂样品单号
        /// </summary>
        public string FacManufactureID { get; set; }

        /// <summary>
        /// 样品修改次数
        /// </summary>
        public int SampleModNum { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 下发日期
        /// </summary>
        public string IssueDate { get; set; }

        /// <summary>
        /// 报价单号
        /// </summary>
        public string QuotNumber { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 要求完成日期
        /// </summary>
        public string ClaimFinishDate { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public string PlanFinishDate { get; set; }

        /// <summary>
        /// 完成日期
        /// </summary>
        public string FinishDate { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string EmailAdress { get; set; }

        /// <summary>
        /// 办事处内勤
        /// </summary>
        public string OfficePerson { get; set; }

        /// <summary>
        /// 样品所在办事处
        /// </summary>
        public string ProductsOffice { get; set; }

        /// <summary>
        /// 样品确认日期
        /// </summary>
        public string AffirmDate { get; set; }

        /// <summary>
        /// 样品完成日期
        /// </summary>
        public string ActualFinishDate { get; set; }

        /// <summary>
        /// 要求寄出日期
        /// </summary>
        public string PlanSendDate { get; set; }

        /// <summary>
        /// 付款方式ID
        /// </summary>
        public int PaymentWayID { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PaymentWay { get; set; }

        /// <summary>
        /// 寄样管理->打样生产订单
        /// </summary>
        public List<VMManufacture> Manufactures { get; set; }

        /// <summary>
        /// 生产跟踪历史记录
        /// </summary>
        public List<VMSendSampleHis> SendSampleHis { get; set; }

        /// <summary>
        /// 是否延迟完成生产
        /// </summary>
        public int IsDelayFinsh { get; set; }

        /// <summary>
        /// 选择报价单的使用场合
        /// </summary>
        public int FunctionNum { get; set; }
        public string UpdateDateFormatter { get; set; }
    }

    /// <summary>
    /// 寄样管理->筛选条件VM
    /// </summary>
    public class VMFilterSample : IndexPageBaseModel
    {
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 样品列表数据是否允许多选
        /// </summary>
        public bool SingleSelect { get; set; }

        /// <summary>
        /// 样品状态
        /// </summary>
        public int SampleStatus { get; set; }

        /// <summary>
        /// 样品状态：寄样信息列表数据默认查询条件边界上限
        /// </summary>
        public int StatusIntervalStart { get; set; }

        /// <summary>
        /// 样品状态：寄样信息列表数据默认查询条件边界下限
        /// </summary>
        public int StatusIntervalEnd { get; set; }

        /// <summary>
        /// 工厂样品单号
        /// </summary>
        public string FacManufactureID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 下发日期-开始
        /// </summary>
        public string IssueDateStart { get; set; }

        /// <summary>
        /// 下发日期-结束
        /// </summary>
        public string IssueDateEnd { get; set; }

        /// <summary>
        /// 要求完成日期-开始
        /// </summary>
        public string PFDateStart { get; set; }

        /// <summary>
        /// 要求完成日期-结束
        /// </summary>
        public string PFDateEnd { get; set; }

        /// <summary>
        /// 要求寄出日期-开始
        /// </summary>
        public string PSDateStart { get; set; }

        /// <summary>
        /// 要求寄出日期-结束
        /// </summary>
        public string PSDateEnd { get; set; }

        /// <summary>
        /// 报价日期-开始
        /// </summary>
        public string QuotDateStart { get; set; }

        /// <summary>
        /// 报价日期-结束
        /// </summary>
        public string QuotDateEnd { get; set; }

        /// <summary>
        /// 销售订单日期-开始
        /// </summary>
        public string OrderDateStart { get; set; }

        /// <summary>
        /// 销售订单日期-结束
        /// </summary>
        public string OrderDateEnd { get; set; }

        /// <summary>
        /// 完成日期-开始
        /// </summary>
        public string FinishDateStart { get; set; }

        /// <summary>
        /// 完成日期-结束
        /// </summary>
        public string FinishDateEnd { get; set; }

        /// <summary>
        /// 寄出日期-开始
        /// </summary>
        public string SendDateStart { get; set; }

        /// <summary>
        /// 寄出日期-结束
        /// </summary>
        public string SendDateEnd { get; set; }

        /// <summary>
        /// 收货日期-开始
        /// </summary>
        public string AcceptedDateStart { get; set; }

        /// <summary>
        /// 收货日期-结束
        /// </summary>
        public string AcceptedDateEnd { get; set; }

        /// <summary>
        /// 样品所在办事处
        /// </summary>
        public string ProductsOffice { get; set; }

        /// <summary>
        /// 产品筛选：模糊匹配寄样信息对应的产品品名
        /// </summary>
        public string Products { get; set; }
    }

    /// <summary>
    /// 寄样管理->生产跟踪历史记录
    /// </summary>
    public class VMSendSampleHis
    {
        /// <summary>
        /// 寄样履历自编号
        /// </summary>
        public int SSHID { get; set; }

        /// <summary>
        /// 寄样信息自编号
        /// </summary>
        public int SSID { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public string AuditCreateDate { get; set; }

        /// <summary>
        /// 样品状态
        /// </summary>
        public string SampleStatus { get; set; }
    }

    /// <summary>
    /// 寄样管理->打样生产订单
    /// </summary>
    public class VMManufacture
    {
        /// <summary>
        /// 寄样信息自编号
        /// </summary>
        public int SSID { get; set; }

        /// <summary>
        /// 报价单自编号
        /// </summary>
        public int QTID { get; set; }

        /// <summary>
        /// 销售订单自编号
        /// </summary>
        public int PHID { get; set; }

        /// <summary>
        /// 创建方式：1=手工创建；2=报价；3=销售订单
        /// </summary>
        public int CreateWay { get; set; }

        /// <summary>
        /// 工厂自编号
        /// </summary>
        public int FactureID { get; set; }

        /// <summary>
        /// 客户自编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 样品状态ID
        /// </summary>
        public int SampleStatusID { get; set; }

        /// <summary>
        /// 样品状态
        /// </summary>
        public string SampleStatus { get; set; }

        /// <summary>
        /// 工厂样品单号
        /// </summary>
        public string FacManufactureID { get; set; }

        /// <summary>
        /// 样品修改次数
        /// </summary>
        public int SampleModNum { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 下发日期（样品单创建日期）
        /// </summary>
        public string IssueDate { get; set; }

        /// <summary>
        /// 报价单号
        /// </summary>
        public string QuotNumber { get; set; }

        /// <summary>
        /// 采购单号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 要求完成日期
        /// </summary>
        public string ClaimFinishDate { get; set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public string PlanFinishDate { get; set; }

        /// <summary>
        /// 样品确认日期
        /// </summary>
        public string AffirmDate { get; set; }

        /// <summary>
        /// 样品工厂实际完成日期
        /// </summary>
        public string FactoryActualFinishDate { get; set; }

        /// <summary>
        /// 样品完成日期
        /// </summary>
        public string ActualFinishDate { get; set; }

        /// <summary>
        /// 要求寄出日期
        /// </summary>
        public string PlanSendDate { get; set; }

        /// <summary>
        /// 实际寄出日期
        /// </summary>
        public string ActualSendDate { get; set; }

        /// <summary>
        /// 预计到达日期
        /// </summary>
        public string ExpectedArrivalDate { get; set; }

        /// <summary>
        /// 样品收货日期
        /// </summary>
        public string ActualAcceptedDate { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string EmailAdress { get; set; }

        /// <summary>
        /// 办事处内勤
        /// </summary>
        public string OfficePerson { get; set; }

        /// <summary>
        /// 样品所在办事处
        /// </summary>
        public string ProductsOffice { get; set; }

        /// <summary>
        /// 生产单备注
        /// </summary>
        public string ManufactureNote { get; set; }

        /// <summary>
        /// 当前阶段描述
        /// </summary>
        public string AlterStageIdea { get; set; }

        /// <summary>
        /// 上传附件
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 产品是否有需要修改
        /// </summary>
        public int IsMod { get; set; }

        /// <summary>
        /// 寄样标签
        /// </summary>
        public string SamplePacks { get; set; }

        /// <summary>
        /// 寄样发票
        /// </summary>
        public string SampleInvoice { get; set; }

        /// <summary>
        /// 寄样箱唛
        /// </summary>
        [AllowHtml]
        public string SampleBoxMark { get; set; }

        /// <summary>
        /// 付款方式ID：1=到付(含客户快递公司、客户账号)；2=预付；
        /// </summary>
        public int PaymentWayID { get; set; }

        /// <summary>
        /// 客户快递公司
        /// </summary>
        public string ExpressToLTD { get; set; }

        /// <summary>
        /// 客户账号
        /// </summary>
        public string ToAcount { get; set; }

        /// <summary>
        /// 收件方式：与客户的收货地址表关联
        /// </summary>
        public int AcceptedID { get; set; }

        /// <summary>
        /// 收件方式：地址详情
        /// </summary>
        public string AcceptedDetail { get; set; }

        /// <summary>
        /// 寄件信息备注
        /// </summary>
        public string SendRemark { get; set; }

        /// <summary>
        /// 寄件方快递公司
        /// </summary>
        public string ExpressFromLTD { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressID { get; set; }

        /// <summary>
        /// 寄出箱数
        /// </summary>
        public int SendPieceNum { get; set; }

        /// <summary>
        /// 快递费用
        /// </summary>
        public decimal ExpressCost { get; set; }

        /// <summary>
        /// 签收ID：1=未签收；2=已签收；
        /// </summary>
        public int SignStatusID { get; set; }

        /// <summary>
        /// 产品数据列表
        /// </summary>
        public List<VMProducts> Products { get; set; }

        /// <summary>
        /// 样品单中的上传附件
        /// </summary>
        public VMUpLoad UploadFiles { get; set; }
    }

    /// <summary>
    /// 产品数据
    /// </summary>
    public class VMProducts
    {
        /// <summary>
        /// 寄样产品信息自编号
        /// </summary>
        public int PSID { get; set; }

        /// <summary>
        /// 寄样信息自编号
        /// </summary>
        public int SSID { get; set; }

        /// <summary>
        /// 产品信息所在表自编号
        /// </summary>
        public int PDID { get; set; }

        /// <summary>
        /// 产品信息所在产品管理主表自编号
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// 生产单需求产品信息备注
        /// </summary>
        public string NoteA { get; set; }

        /// <summary>
        /// 货号
        /// </summary>
        public string ProductNo { get; set; }

        /// <summary>
        /// 工厂货号
        /// </summary>
        public string FactoryNo { get; set; }

        /// <summary>
        /// 产品数量（新建样品单时，默认为1）
        /// </summary>
        public int ProductNum { get; set; }

        /// <summary>
        /// 款式
        /// </summary>
        public string StyleName { get; set; }

        /// <summary>
        /// 内盒率
        /// </summary>
        public int InnerBoxRate { get; set; }

        /// <summary>
        /// 外箱率
        /// </summary>
        public int OuterBoxRate { get; set; }

        /// <summary>
        /// 是否修改
        /// </summary>
        public int IsMod { get; set; }

        /// <summary>
        /// 修改意见
        /// </summary>
        public string ModIdea { get; set; }

        /// <summary>
        /// 打样产品备注
        /// </summary>
        public string SampleProductNote { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string ProductDescription { get; set; }

        /// <summary>
        /// 产品UPC
        /// </summary>
        public string ProductUPC { get; set; }

        /// <summary>
        /// 产品计划完成日期
        /// </summary>
        public string ProductPlanFinshDate { get; set; }
    }
}