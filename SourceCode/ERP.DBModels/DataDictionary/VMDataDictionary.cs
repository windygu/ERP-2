using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.DataDictionary
{
    public class VMDataDictionary : IndexPageBaseModel
    {
        public PageTypeEnum PageType { get; set; }
        /// <summary>
        /// 自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 数据id
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 数据名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型id
        /// </summary>
        public int TableKind { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string AttrName { get; set; }

        /// <summary>
        /// 类型别名
        /// </summary>
        public string Alias { get; set; }

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
        public string IPAdress { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }

        public int? Country { get; set; }
        public int? Province { get; set; }
        public int? City { get; set; }
        public string StreetAddress { get; set; }
    }
}