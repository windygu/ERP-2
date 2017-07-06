using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.CustomEnums
{
    public enum DataPermissions : short
    {
        /// <summary>
        /// 只能查看自己的数据
        /// </summary>
        [Description("只能查看自己的数据")]
        SelfOnly = 0,

        /// <summary>
        /// 能查看当前部门中的数据
        /// </summary>
        [Description("能查看当前部门中的数据")]
        InnerHierachy = 10,

        /// <summary>
        /// 能查看当前部门中的数据，也可以查看部门所属的子级部门的数据
        /// </summary>
        [Description("能查看当前部门中的数据，也可以查看部门所属的子级部门的数据")]
        InnerAndSubHierachies = 20,

        /// <summary>
        /// 能查看所有同级部门的数据
        /// </summary>
        [Description("能查看所有同级部门的数据")]
        InnerAndSameLevelHierachies = 30,

        /// <summary>
        /// 能查看所有同级部门的数据，也可以查看各同级部门所属的子级部门的数据
        /// </summary>
        [Description("能查看所有同级部门的数据，也可以查看各同级部门所属的子级部门的数据")]
        InnerAndSameLevelAndAllSubHierachies = 40,

        /// <summary>
        /// 当前部门中的所有数据以及所有办事处的数据
        /// </summary>
        [Description("当前部门中的所有数据以及所有办事处的数据")]
        InnerHierachyAndAllAgencies = 50,

        /// <summary>
        /// 当前部门、所有子级部门中的所有数据以及所有办事处的数据
        /// </summary>
        [Description("当前部门、所有子级部门中的所有数据以及所有办事处的数据")]
        InnerAndSubHierachiesAndAllAgencies = 60,

        /// <summary>
        /// 当前部门、同级部门、所有子级部门中的所有数据以及所有办事处的数据
        /// </summary>
        [Description("当前部门、同级部门、所有子级部门中的所有数据以及所有办事处的数据")]
        InnerAndSameLevelAndSubHierachiesAndAllAgencies = 65,

        /// <summary>
        /// 当前部门、同级部门、所有子级部门中的所有数据以及所有业务部的数据
        /// </summary>
        [Description("当前部门、同级部门、所有子级部门中的所有数据以及所有业务部的数据")]
        InnerAndSameLevelAndSubHierachiesAndAllBusinessDept = 66,

        /// <summary>
        /// 当前部门中的所有数据、所有办事处的数据、所有设计部门的数据
        /// </summary>
        [Description("当前部门中的所有数据、所有办事处的数据、所有设计部门的数据")]
        InnerHierachyAndAllAgenciesAndAllDesigners = 70,

        /// <summary>
        /// 包含所有业务部、设计部、办事处的数据
        /// </summary>
        [Description("包含所有业务部、设计部、办事处的数据")]
        IncludeAllBusinessAndDesginDeptAndAgencies = 75,

        /// <summary>
        /// 包含当前部门以及所有业务部的数据
        /// </summary>
        [Description("包含当前部门以及所有业务部的数据")]
        IncludeAllBusinessDept = 80,
    }
}
