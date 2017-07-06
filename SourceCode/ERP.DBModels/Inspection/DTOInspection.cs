using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Inspection
{
    public class DTOInspection
    {
        /// <summary>
        ///  检验编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同id
        /// </summary>
        public string PurchaseID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string InspectionDesc { get; set; }

        /// <summary>
        /// 状态id
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态text
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public decimal InspectionMoney { get; set; }

        /// <summary>
        /// 第三方名称
        /// </summary>
        public string YanFactoryName { get; set; }

        /// <summary>
        /// 第三方邮箱
        /// </summary>
        public string YanEmail { get; set; }

        /// <summary>
        /// 第三方电话
        /// </summary>
        public string YanPhone { get; set; }

        /// <summary>
        /// 第三方传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 验厂结果
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 检验类别
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 检测数量
        /// </summary>
        public int InspectionNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAdress { get; set; }

        public List<VMInspectionAuditNotice> Contacts { get; set; }

        /// <summary>
        /// 历史编号
        /// </summary>
        public int HistoryID { get; set; }

        /// <summary>
        /// 检验id
        /// </summary>
        public int HistoryInspectionID { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime HistoryfinishTime { get; set; }

        /// <summary>
        /// 验厂内容/采购订单id
        /// </summary>
        public string Commant { get; set; }

        /// <summary>
        /// 验厂结果/检测结果
        /// </summary>
        public int HistoryResult { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int HistoryInspectionNum { get; set; }

        /// <summary>
        /// 验厂结束时间
        /// </summary>
        public string InspectionEndTime { get; set; }

        public string InspectionContent { get; set; }

        public int StatusID { get; set; }

        public string InspectionName { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// 第三方验厂费用——我司承担费用
        /// </summary>
        public string InspectionAuditFeeFormatter { get; set; }

        /// <summary>
        /// 第三方验厂费用——工厂承担费用
        /// </summary>
        public string InspectionAuditFee_ForFactoryFormatter { get; set; }

        /// <summary>
        /// 第三方检测费用——我司承担费用
        /// </summary>
        public string InspectionDetectFeeFormatter { get; set; }

        /// <summary>
        /// 第三方检测费用——工厂承担费用
        /// </summary>
        public string InspectionDetectFee_ForFactoryFormatter { get; set; }
        public string InspectionSamplingFeeFormatter { get; set; }
        public string InspectionSamplingFee_ForFactoryFormatter { get; set; }
    }
}