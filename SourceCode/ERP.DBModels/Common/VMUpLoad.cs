using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Common
{
    /// <summary>
    /// 上传附件
    /// </summary>
    public class VMUpLoad
    {
        /// <summary>
        /// 编号描述
        /// </summary>
        public string No_Description { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 字段1
        /// </summary>
        public string Field1 { get; set; }

        /// <summary>
        /// 字段1描述
        /// </summary>
        public string Field1_Description { get; set; }

        /// <summary>
        /// 字段2
        /// </summary>
        public string Field2 { get; set; }

        /// <summary>
        /// 字段2描述
        /// </summary>
        public string Field2_Description { get; set; }

        /// <summary>
        /// 字段3
        /// </summary>
        public string Field3 { get; set; }

        /// <summary>
        /// 字段3描述
        /// </summary>
        public string Field3_Description { get; set; }

        /// <summary>
        /// 字段3
        /// </summary>
        public string Field4 { get; set; }

        /// <summary>
        /// 字段3描述
        /// </summary>
        public string Field4_Description { get; set; }

        /// <summary>
        /// 字段3
        /// </summary>
        public string Field5 { get; set; }

        /// <summary>
        /// 字段3描述
        /// </summary>
        public string Field5_Description { get; set; }

        /// <summary>
        /// 附件集合描述
        /// </summary>
        public string FileName_Description { get; set; }

        /// <summary>
        /// 所在模块主表自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 保存按钮区分
        /// </summary>
        public int ButtonNum { get; set; }
        /// <summary>
        /// 上传文件的类型。主要放置文件的路径
        /// </summary>
        public UploadFileType UpLoadType { get; set; }

        /// <summary>
        /// 上传文件的列表
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList { get; set; }
        public string SelectCustomer { get; set; }
    }

    public class VMUpLoadFile
    {
        #region Model

        /// <summary>
        /// 上传附件表的自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 服务器端文件名称
        /// </summary>
        public string ServerFileName { get; set; }

        /// <summary>
        /// 文件显示名称
        /// </summary>
        public string DisplayFileName { get; set; }

        /// <summary>
        /// 上传模块类型（0：采购合同，1：待印合同，2：寄样管理上传附件）
        /// </summary>
        public int ModuleType { get; set; }

        /// <summary>
        /// 所在模块主表自编号
        /// </summary>
        public int LinkID { get; set; }

        /// <summary>
        /// 是否删除
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
        public string DT_CREATEDATE_Formatter { get; set; }

        #endregion Model
    }
}