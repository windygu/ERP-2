using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ThreeTimesQC
{
    /// <summary>
    /// 三期QC的
    /// </summary>
    public class DTOPurchaseContract:PendingApproveBasePage
    {
        #region Model

        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 结关后的天数
        /// </summary>
        public int AfterDate { get; set; }

        /// <summary>
        /// 采购合同总金额
        /// </summary>
        public decimal AllAmount { get; set; }

        /// <summary>
        /// 交货地
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 审核成功后自动发送邮件给工厂（0：否，1：是）
        /// </summary>
        public string IsImmediatelySend { get; set; }

        /// <summary>
        /// 第三方验货（0：否，1：是）
        /// </summary>
        public string IsThirdVerification { get; set; }

        /// <summary>
        /// 第三方验厂（0：否，1：是）
        /// </summary>
        public string IsThirdAudits { get; set; }

        /// <summary>
        /// 第三方检测（0：否，1：是）
        /// </summary>
        public string IsThirdTest { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public string IsDelete { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public string DT_MODIFYDATE { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 核算意见
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public int? ApproverIndex { get; set; }

        /// <summary>
        /// 开始交货日期
        /// </summary>
        public string DateStart { get; set; }

        /// <summary>
        /// 结束交货日期
        /// </summary>
        public string DateEnd { get; set; }

        /// <summary>
        /// 是否代印（代印为1，不代印为0）
        /// </summary>
        public string IsOutsourcing { get; set; }

        /// <summary>
        /// 采购合同——审核状态
        /// </summary>
        public string PurchaseStatus { get; set; }

        /// <summary>
        /// 采购合同——审核状态编号
        /// </summary>
        public int PurchaseStatusID { get; set; }

        /// <summary>
        /// 包装资料——审核状态
        /// </summary>
        public string PacksStatus { get; set; }

        /// <summary>
        /// 包装资料名称
        /// </summary>
        public string PacksName { get; set; }

        #endregion Model

        /// <summary>
        /// 前期上传状态
        /// </summary>
        public string UpLoadStatus_One { get; set; }

        /// <summary>
        /// 中期上传状态
        /// </summary>
        public string UpLoadStatus_Two { get; set; }

        /// <summary>
        /// 尾期上传状态
        /// </summary>
        public string UpLoadStatus_Three { get; set; }

        public int? RecoveryStatusID { get; set; }

        public string RecoveryStatusName { get; set; }

        public string RecoveryModifyDate { get; set; }
        public int? CustomerID { get; set; }
        public string StatusName { get; set; }
        public bool IsHasApprovalPermission { get; set; }
        public int StatusID { get; set; }
    }
}