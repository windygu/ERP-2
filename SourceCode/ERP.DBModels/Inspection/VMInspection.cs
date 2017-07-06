using ERP.Models.Common;
using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Inspection
{
    /// <summary>
    /// 第三方验厂
    /// </summary>
    public class VMInspection
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同id
        /// </summary>
        public int PurchaseID { get; set; }

        /// <summary>
        /// 第三方验厂时间——开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 第三方验厂时间——结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 第三方验厂结果的备注
        /// </summary>
        public string InspectionContent { get; set; }

        /// <summary>
        /// 第三方验厂状态
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// 第三方验厂费用——我司承担费用
        /// </summary>
        public decimal InspectionAuditFee { get; set; }

        /// <summary>
        /// 第三方验厂费用——工厂承担费用
        /// </summary>
        public decimal? InspectionAuditFee_ForFactory { get; set; }

        /// <summary>
        /// 第三方检测费用——我司承担费用
        /// </summary>
        public decimal InspectionDetectFee { get; set; }

        /// <summary>
        /// 第三方检测费用——工厂承担费用
        /// </summary>
        public decimal? InspectionDetectFee_ForFactory { get; set; }

        /// <summary>
        /// 第三方抽检费用——我司承担费用
        /// </summary>
        public decimal? InspectionSamplingFee { get; set; }

        /// <summary>
        /// 第三方抽检费用——工厂承担费用
        /// </summary>
        public decimal? InspectionSamplingFee_ForFactory { get; set; }

        /// <summary>
        /// 第三方验厂名称
        /// </summary>
        public string InspectionName { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string InspectionEmail { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string InspectionPhoneNumber { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string InspectionFax { get; set; }

        /// <summary>
        /// DT_CREATEDATE
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// ST_CREATEUSER
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// DT_MODIFYDATE
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// ST_MODIFYUSER
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// IPAdress
        /// </summary>
        public string IPAdress { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 1：验厂 2：检测 3：抽检
        /// </summary>
        public int? TypeID { get; set; }

        #endregion Model

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        public string StatusName { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public PageTypeEnum PageType { get; set; }

        public VMSendEmail SendEmail { get; set; }

        /// <summary>
        /// 第三方验厂
        /// </summary>
        public List<VMInspectionAuditNotice> list_InspectionAuditNotice { get; set; }

        /// <summary>
        /// 第三方检测
        /// </summary>
        public List<VMInspectionDetectNotice> list_InspectionDetectNotice { get; set; }

        /// <summary>
        /// 第三方抽检
        /// </summary>
        public List<VMInspectionSamplingNotice> list_InspectionSamplingNotice { get; set; }

        /// <summary>
        /// 第三方验厂上传附件列表
        /// </summary>
        public List<VMUpLoadFile> list_UpLoadFile { get; set; }

        /// <summary>
        /// 第三方验厂——历史记录
        /// </summary>
        public List<VMInspectionAuditNoticeHistory> list_InspectionAuditNoticeHistory { get; set; }

        /// <summary>
        /// 第三方检测——历史记录
        /// </summary>
        public List<VMInspectionDetectNoticeHistory> list_InspectionDetectNoticeHistory { get; set; }

        /// <summary>
        /// 第三方抽检——历史记录
        /// </summary>
        public List<VMInspectionSamplingNoticeHistory> list_InspectionSamplingNoticeHistory { get; set; }

        public string FactoryEmail { get; set; }

        public int? CurrencyType { get; set; }

        /// <summary>
        /// 币种：人民币
        /// </summary>
        public string CurrencySign { get; set; }
    }

    /// <summary>
    /// 第三方验厂
    /// </summary>
    public class VMInspectionAuditNotice
    {
        #region Model

        /// <summary>
        /// 第三方验厂通知编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 第三方验厂编号
        /// </summary>
        public int InspectionID { get; set; }

        /// <summary>
        /// 验厂项目
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// DT_CREATEDATE
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// ST_CREATEUSER
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// DT_MODIFYDATE
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// ST_MODIFYUSER
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// IPAdress
        /// </summary>
        public string IPAdress { get; set; }

        #endregion Model

        public bool editing { get; set; }
    }

    /// <summary>
    /// 第三方检测
    /// </summary>
    public class VMInspectionDetectNotice
    {
        #region Model

        /// <summary>
        /// 第三方检测编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 第三方检验编号
        /// </summary>
        public int InspectionID { get; set; }

        /// <summary>
        /// 产品编号：检测样品名称
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 检测样品数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 我司承担费用
        /// </summary>
        public decimal? ProductDetectFee { get; set; }

        /// <summary>
        /// 工厂承担费用
        /// </summary>
        public decimal? ProductDetectFee_ForFactory { get; set; }

        #endregion Model

        public bool editing { get; set; }
        public string No { get; set; }
    }

    /// <summary>
    /// 第三方验厂——历史记录
    /// </summary>
    public class VMInspectionAuditNoticeHistory
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 第三方验厂通知编号
        /// </summary>
        public int InspectionID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// DT_CREATEDATE
        /// </summary>
        public string DT_CREATEDATE { get; set; }

        /// <summary>
        /// ST_CREATEUSER
        /// </summary>
        public string ST_CREATEUSER { get; set; }

        /// <summary>
        /// IPAddress
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        #endregion Model

        public string StatusName { get; set; }
    }

    /// <summary>
    /// 第三方检测——历史记录
    /// </summary>
    public class VMInspectionDetectNoticeHistory
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 第三方检测编号
        /// </summary>
        public int InspectionID { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// 样品数量
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// DT_CREATEDATE
        /// </summary>
        public string DT_CREATEDATE { get; set; }

        /// <summary>
        /// ST_CREATEUSER
        /// </summary>
        public string ST_CREATEUSER { get; set; }

        /// <summary>
        /// IPAddress
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        #endregion Model

        public string StatusName { get; set; }
    }

    /// <summary>
    /// 第三方抽检
    /// </summary>
    public class VMInspectionSamplingNotice
    {
        #region Model

        /// <summary>
        /// 第三方抽检通知编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 第三方抽检编号
        /// </summary>
        public int InspectionID { get; set; }

        /// <summary>
        /// 抽检项目
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// DT_CREATEDATE
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// ST_CREATEUSER
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// DT_MODIFYDATE
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// ST_MODIFYUSER
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// IPAdress
        /// </summary>
        public string IPAdress { get; set; }

        #endregion Model

        public bool editing { get; set; }
    }


    /// <summary>
    /// 第三方抽检——历史记录
    /// </summary>
    public class VMInspectionSamplingNoticeHistory
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 第三方抽检通知编号
        /// </summary>
        public int InspectionID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// DT_CREATEDATE
        /// </summary>
        public string DT_CREATEDATE { get; set; }

        /// <summary>
        /// ST_CREATEUSER
        /// </summary>
        public string ST_CREATEUSER { get; set; }

        /// <summary>
        /// IPAddress
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        #endregion Model

        public string StatusName { get; set; }
    }
}