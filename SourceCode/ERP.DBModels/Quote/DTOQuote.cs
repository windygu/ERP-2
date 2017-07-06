using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    public class DTOQuote : PendingApproveBasePage
    {
        /// <summary>
        /// 报价单表
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 报价单编号
        /// </summary>
        public string QuotNumber { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 报价日期
        /// </summary>
        public string QuotDate { get; set; }

        /// <summary>
        /// 报价有效期
        /// </summary>
        public string ValidDate { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public int AuthorID { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// 审核通过后，是否立即发送报价给客户
        /// </summary>
        public bool IsImmediatelySend { get; set; }

        /// <summary>
        /// 报价次数
        /// </summary>
        public int QuotTimes { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public string DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 判断待审核页面是否显示审批按钮
        /// </summary>
        public bool IsHasApprovalPermission { get; set; }

        public int? ApproverIndex { get; set; }
    }
}