using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Rep
{
    public class VMCommission
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// RepID
        /// </summary>
        public int RepID { get; set; }

        /// <summary>
        /// OCID
        /// </summary>
        public int OCID { get; set; }

        /// <summary>
        /// Commission
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        #endregion Model

        public string CustomerCode { get; set; }
    }
}