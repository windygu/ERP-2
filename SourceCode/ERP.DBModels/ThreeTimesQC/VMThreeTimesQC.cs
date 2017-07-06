using ERP.Models.Common;
using ERP.Models.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ThreeTimesQC
{
    public class VMThreeTimesQC
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// PurchaseContractID
        /// </summary>
        public int PurchaseContractID { get; set; }

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

        /// <summary>
        /// 出货样保管人
        /// </summary>
        public int? RecoveryUserID { get; set; }

        /// <summary>
        /// 收回日期
        /// </summary>
        public DateTime? RecoveryDate { get; set; }

        /// <summary>
        /// 出货样状态
        /// </summary>
        public int? RecoveryStatusID { get; set; }

        #endregion Model

        public CustomEnums.PageTypeEnum PageType { get; set; }

        public VMPurchase PurchaseContract { get; set; }

        /// <summary>
        /// 前期上传的文件列表
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList_One { get; set; }

        /// <summary>
        /// 中期上传的文件列表
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList_Two { get; set; }

        /// <summary>
        /// 尾期上传的文件列表
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList_Three { get; set; }

        /// <summary>
        /// 三期QC验货的历史记录
        /// </summary>
        public List<ThreeTimesQCHistory> list_history { get; set; }

        public string RecoveryDateFormatter { get; set; }

        /// <summary>
        /// 出货样存放地址
        /// </summary>
        public string RecoveryAddress { get; set; }
        public string Suggest { get; set; }
        public int ApprovalStatus { get; set; }
    }

    /// <summary>
    /// 三期QC验货的历史记录
    /// </summary>
    public class ThreeTimesQCHistory
    {
        #region Model

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 三期QC验货ID
        /// </summary>
        public int ThreeTimesQCID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string ST_CREATEUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string CheckSuggest { get; set; }

        #endregion Model
    }
}