//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public Product()
        {
            this.Product1 = new HashSet<Product>();
            this.Product11 = new HashSet<Product>();
            this.ProductFittings = new HashSet<ProductFitting>();
        }
    
        public int ID { get; set; }
        public int ClassifyID { get; set; }
        public int FactoryID { get; set; }
        public string No { get; set; }
        public string NoFactory { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int CustomerID { get; set; }
        public string Image { get; set; }
        public int UnitID { get; set; }
        public decimal Length { get; set; }
        public Nullable<decimal> LengthIN { get; set; }
        public decimal Height { get; set; }
        public Nullable<decimal> HeightIN { get; set; }
        public decimal Width { get; set; }
        public Nullable<decimal> WidthIN { get; set; }
        public int StyleID { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> WeightLBS { get; set; }
        public int PortID { get; set; }
        public int PackingMannerZhID { get; set; }
        public string IngredientZh { get; set; }
        public Nullable<int> MOQZh { get; set; }
        public Nullable<int> PackingMannerEnID { get; set; }
        public string IngredientEn { get; set; }
        public Nullable<int> PDQPackRate { get; set; }
        public Nullable<decimal> PDQLength { get; set; }
        public Nullable<decimal> PDQLengthIN { get; set; }
        public Nullable<decimal> PDQWidth { get; set; }
        public Nullable<decimal> PDQWidthIN { get; set; }
        public Nullable<decimal> PDQHeight { get; set; }
        public Nullable<decimal> PDQHeightIN { get; set; }
        public Nullable<int> InnerBoxRate { get; set; }
        public Nullable<decimal> InnerLength { get; set; }
        public Nullable<decimal> InnerLengthIN { get; set; }
        public Nullable<decimal> InnerWidth { get; set; }
        public Nullable<decimal> InnerWidthIN { get; set; }
        public Nullable<decimal> InnerHeight { get; set; }
        public Nullable<decimal> InnerHeightIN { get; set; }
        public Nullable<decimal> InnerWeight { get; set; }
        public Nullable<decimal> InnerWeightLBS { get; set; }
        public Nullable<decimal> InnerVolume { get; set; }
        public Nullable<int> OuterBoxRate { get; set; }
        public Nullable<decimal> OuterLength { get; set; }
        public Nullable<decimal> OuterLengthIN { get; set; }
        public Nullable<decimal> OuterWidth { get; set; }
        public Nullable<decimal> OuterWidthIN { get; set; }
        public Nullable<decimal> OuterHeight { get; set; }
        public Nullable<decimal> OuterHeightIN { get; set; }
        public Nullable<decimal> OuterVolume { get; set; }
        public Nullable<decimal> OuterWeightGross { get; set; }
        public Nullable<decimal> OuterWeightGrossLBS { get; set; }
        public Nullable<decimal> OuterWeightNet { get; set; }
        public Nullable<decimal> OuterWeightNetLBS { get; set; }
        public decimal PriceFactory { get; set; }
        public Nullable<decimal> FOBFTY { get; set; }
        public Nullable<decimal> FOBNET { get; set; }
        public Nullable<decimal> FOBC5 { get; set; }
        public Nullable<decimal> FOBChinaPort { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> FinalFOB { get; set; }
        public Nullable<decimal> PcsPallet { get; set; }
        public Nullable<decimal> PalletPc { get; set; }
        public Nullable<decimal> Duty { get; set; }
        public Nullable<decimal> FOBChinaLCL { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public int Commission { get; set; }
        public Nullable<decimal> CommissionAmount { get; set; }
        public Nullable<decimal> MiscImportLoadAmount { get; set; }
        public Nullable<decimal> MiscImportLoad { get; set; }
        public Nullable<decimal> Agent { get; set; }
        public Nullable<decimal> SRP { get; set; }
        public Nullable<decimal> CtnsPallet { get; set; }
        public Nullable<decimal> DutyPercent { get; set; }
        public Nullable<decimal> FreightRate { get; set; }
        public string Remarks { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> PriceInputDate { get; set; }
        public Nullable<System.DateTime> ValidDate { get; set; }
        public Nullable<int> MOQEn { get; set; }
        public bool Deleted { get; set; }
        public string UPC { get; set; }
        public int CurrencyType { get; set; }
        public System.DateTime DT_CREATEDATE { get; set; }
        public int ST_CREATEUSER { get; set; }
        public Nullable<System.DateTime> DT_MODIFYDATE { get; set; }
        public Nullable<int> ST_MODIFYUSER { get; set; }
        public string IPAddress { get; set; }
        public Nullable<decimal> Allowance { get; set; }
        public Nullable<int> HSCode { get; set; }
        public Nullable<int> HTS { get; set; }
        public Nullable<int> ParentProductID { get; set; }
        public Nullable<int> RootProductID { get; set; }
        public short Status { get; set; }
        public Nullable<int> ProductCopyRight { get; set; }
        public Nullable<int> Season { get; set; }
        public Nullable<int> ColorID { get; set; }
        public Nullable<decimal> InnerWeightGross { get; set; }
        public Nullable<decimal> InnerWeightGrossLBS { get; set; }
        public bool IsProductFitting { get; set; }
        public bool IsProductMixed { get; set; }
        public Nullable<int> ParentProductMixedID { get; set; }
        public int Qty { get; set; }
    
        public virtual SystemUser SystemUser { get; set; }
        public virtual Orders_Customers Orders_Customers { get; set; }
        public virtual Classify Classify { get; set; }
        public virtual Factory Factory { get; set; }
        public virtual ICollection<Product> Product1 { get; set; }
        public virtual Product Product2 { get; set; }
        public virtual ICollection<Product> Product11 { get; set; }
        public virtual Product Product3 { get; set; }
        public virtual ICollection<ProductFitting> ProductFittings { get; set; }
    }
}
