using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    public class VMQuoteEdit
    {
        #region Model

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
        public DateTime ValidDate { get; set; }

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

        #endregion Model

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 人民币换汇
        /// </summary>
        public decimal? CurrencyExchangeRMB { get; set; }

        /// <summary>
        /// 美元换汇
        /// </summary>
        public decimal? CurrencyExchangeUSD { get; set; }

        /// <summary>
        /// 预期市场汇率
        /// </summary>
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// Terms
        /// </summary>
        public int? TermsID { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int? PortID { get; set; }

        /// <summary>
        /// 客户资料里面的Commission%
        /// </summary>
        public decimal CommissionPercent { get; set; }

        /// <summary>
        /// 客户资料里面的Allowance%
        /// </summary>
        public decimal AllowancePercent { get; set; }

        /// <summary>
        /// 客户资料里面的Palletpc $
        /// </summary>
        public decimal? PalletPc { get; set; }

        /// <summary>
        /// 客户资料里面的Misc%/Import Load%
        /// </summary>
        public decimal MiscImportLoadPercent { get; set; }

        public string ValidDateFormat { get; set; }

        public int NewStatusID { get; set; }

        public int? ApproverIndex { get; set; }

        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 是否是复制报价单页面
        /// </summary>
        public bool IsCopy { get; set; }

        /// <summary>
        /// 报价单的产品列表
        /// </summary>
        public List<VMQuoteProduct> listProducts { get; set; }

        /// <summary>
        /// 报价单的历史记录列表
        /// </summary>
        public List<VMQuoteHistory> listHistory { get; set; }

        /// <summary>
        /// 报价单的混装产品列表
        /// </summary>
        public List<VMQuoteProduct> listProducts_Mixed { get; set; }
    }

    public class ApiResult
    {
        public bool Success { get; set; }

        public string Info { get; set; }

        public string Identity { get; set; }

        public ApiResult()
        {
            Success = true;
            Info = "OK";
        }
    }


}