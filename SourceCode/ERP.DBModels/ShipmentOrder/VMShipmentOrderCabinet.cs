using ERP.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ShipmentOrder
{
    /// <summary>
    /// 订舱管理——柜型
    /// </summary>
    public class VMShipmentOrderCabinet
    {
        #region Model

        /// <summary>
        /// 订舱柜型编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 订舱信息编号
        /// </summary>
        public int? ShipmentOrderID { get; set; }

        /// <summary>
        /// 柜型
        /// </summary>
        public int? Shipment_CabinetID { get; set; }

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
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 产品总外箱体积
        /// </summary>
        public decimal? SumVolume { get; set; }

        /// <summary>
        /// 产品总外箱毛重
        /// </summary>
        public decimal? SumWeightGross { get; set; }

        /// <summary>
        /// 产品总外箱净重
        /// </summary>
        public decimal? SumWeightNet { get; set; }

        /// <summary>
        /// 箱柜数量
        /// </summary>
        public int? CabinetQty { get; set; }

        /// <summary>
        /// 货柜剩余容量
        /// </summary>
        public decimal? CabinetRemainderVolume { get; set; }

        /// <summary>
        /// 装柜时间
        /// </summary>
        public DateTime? GatherDate { get; set; }

        /// <summary>
        /// 装柜时间
        /// </summary>
        public string GatherDateFormatter { get; set; }

        /// <summary>
        /// 装柜地点
        /// </summary>
        public string GatherAddress { get; set; }

        /// <summary>
        /// 装柜类型（1：拉柜、2：入仓）
        /// </summary>
        public int? GatherType { get; set; }

        /// <summary>
        /// 装柜描述
        /// </summary>
        public string GatherDescription { get; set; }

        /// <summary>
        /// 是否给工厂发送通知
        /// </summary>
        public bool IsSendEmailToFactory { get; set; }

        /// <summary>
        /// 登记费用
        /// </summary>
        public decimal? RegisterFees { get; set; }

        #endregion Model

        public List<VMShipmentOrderProduct> list_product { get; set; }

        public string CabinetName { get; set; }

        /// <summary>
        /// 货柜体积
        /// </summary>
        public decimal? CabinetVolume { get; set; }

        public string CabinetSize { get; set; }

        public int CabinetID { get; set; }

        /// <summary>
        /// 上传文件的列表
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList { get; set; }

        public int? ShipToPortID { get; set; }

        public string ShipToPortName { get; set; }

        public int CarbinetIndex { get; set; }
        public string GatherEndDateFormatter { get; set; }

        /// <summary>
        /// 发货日期——开始
        /// </summary>
        public string ShippingDateStartFormatter { get; set; }

        /// <summary>
        /// 发货日期——结束
        /// </summary>
        public string ShippingDateEndFormatter { get; set; }

        /// <summary>
        /// 装箱类型
        /// </summary>
        public int? CabinetType { get; set; }

        /// <summary>
        /// 柜号
        /// </summary>
        public string CabinetNumber { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxNumber { get; set; }

        /// <summary>
        /// 封箱号
        /// </summary>
        public string SealingNumber { get; set; }

        /// <summary>
        /// 航名航次
        /// </summary>
        public string OceanVessel { get; set; }
    }
}