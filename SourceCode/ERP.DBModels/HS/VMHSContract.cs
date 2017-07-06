using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.HS
{
    public class VMHSContract : IndexPageBaseModel
    {
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 海关税则编号id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 海关税则编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal Cess { get; set; }

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
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 编码名称
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// 海关税则编码
        /// </summary>
        public int HSID { get; set; }

        /// <summary>
        /// 是否报检
        /// </summary>
        public string IsCheck { get; set; }

        /// <summary>
        /// 数据字典id
        /// </summary>
        public string TagID { get; set; }

        /// <summary>
        /// HScode类型（1是报关的HSCODE类型，进口Hscode类型是2）
        /// </summary>
        public int DataFlag { get; set; }


        /// <summary>
        /// 报关类型
        /// </summary>
        public string DataType { get; set; }
    }
}