using ERP.Models.Common;
using ERP.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ThirdPartyVerification
{
    public class VMThirdPartyVerification
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// OrderID
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 第三方验货状态
        /// </summary>
        public int StatusID { get; set; }

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
        /// IPAddress
        /// </summary>
        public string IPAddress { get; set; }

        public VMOrderEdit Order { get; set; }

        #endregion Model

        public CustomEnums.PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 上传文件的列表
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList { get; set; }
        public decimal? InspectionVerificationFee { get; set; }
        public decimal? InspectionVerificationFee_ForFactory { get; set; }


        public List<VMThirdPartyVerification_Product> list_OrderProduct { get; set; }
    }

    public class VMThirdPartyVerification_Product
    {
        public int ID { get; set; }
        public decimal? InspectionVerificationFee { get; set; }
        public decimal? InspectionVerificationFee_ForFactory { get; set; }
    }
}