using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class DTOAdminUserMenus
    {
        [JsonProperty("id")]
        public int MenuID { get; set; }

        public int? ParentMenuID { get; set; }

        [JsonProperty("text")]
        public string Name { get; set; }

        public string PageURL { get; set; }

        public string WinSize { get; set; }

        public string WinType { get; set; }

        public string Icons { get; set; }

        [JsonProperty("children")]
        public List<DTOAdminUserMenus> SubMenus { get; set; }

        [JsonProperty("checked")]
        public bool CanView { get; set; }

        public Type PageElementEnumType { get; set; }

        public IEnumerable<DTOAdminUserMenus> DescendantsAndSelf()
        {
            yield return this;
            //foreach (var item in SubMenus.SelectMany(x => x.DescendantsAndSelf()))
            //{
            //    yield return item;
            //}
        }
    }
}
