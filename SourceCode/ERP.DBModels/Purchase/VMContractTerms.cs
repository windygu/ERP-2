using System.Collections.Generic;

namespace ERP.Models.Purchase
{
    /// <summary>
    /// 采购合同
    /// </summary>
    public class VMContractTerms
    {
        public int HSCodeID { get; set; }

        public string HSCode { get; set; }

        public List<string> list_No { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string HSCodeName { get; set; }

        /// <summary>
        /// 报检项目
        /// </summary>
        public string ProjectName { get; set; }

    }
}