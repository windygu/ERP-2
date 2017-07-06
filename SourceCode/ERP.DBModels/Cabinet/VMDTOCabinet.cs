using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Cabinet
{
    public class VMDTOCabinet : IndexPageBaseModel
    {
        /// <summary>
        /// id自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 柜型尺寸
        /// </summary>
        public string Size { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public Nullable<System.DateTime> DT_CREATEDATE { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public Nullable<int> ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public Nullable<System.DateTime> DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public Nullable<int> ST_MODIFYUSER { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAdress { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public Nullable<int> IsDelete { get; set; }


        /// <summary>
        /// 第二个尺寸
        /// </summary>
        public int Sizetwo { get; set; }


        /// <summary>
        /// 柜子长
        /// </summary>
        public string Length { get; set; }


        /// <summary>
        /// 柜子宽
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        ///柜子高
        /// </summary>
        public string Height { get; set; }
    }
}
