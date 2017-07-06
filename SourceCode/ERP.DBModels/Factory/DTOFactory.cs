using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Factory
{
    public class DTOFactory
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string Name { get; set; }

        public string No { get; set; }

        /// <summary>
        /// 城市（工厂区域）
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string CallPeople { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Cellphone { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string F_Address { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EmailAdress { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 省id
        /// </summary>
        public int ProvinceID { get; set; }

        /// <summary>
        /// 县
        /// </summary>
        public string CityArea { get; set; }

        /// <summary>
        /// 县ID
        /// </summary>
        public int CityAreaID { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public Nullable<System.DateTime> DT_CREATEDATE { get; set; }

        /// <summary>
        /// 所有地址
        /// </summary>
        public string AllAdress { get; set; }

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
        /// 工厂类别
        /// </summary>
        public int DataFlag { get; set; }

        /// <summary>
        /// 办事处id
        /// </summary>
        public Nullable<int> Hierachy { get; set; }

        /// <summary>
        /// 拉柜费
        /// </summary>
        public decimal? RegisterFees { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 英文地址
        /// </summary>
        public string EnglishAddress { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public int? CurrencyType { get; set; }
        
        public string CurrencyName { get; set; }

        /// <summary>
        /// 是否保函出货
        /// </summary>
        public bool? IsGuaranteeShipments { get; set; }
    }
}