//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventariz.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class shift
    {
        public System.Guid id { get; set; }
        public int unitid { get; set; }
        public System.DateTime dt { get; set; }
        public System.DateTime dtwrite { get; set; }
        public int shift1 { get; set; }
        public Nullable<double> vol { get; set; }
        public Nullable<double> volstart { get; set; }
        public Nullable<double> volend { get; set; }
        public Nullable<double> mas { get; set; }
        public Nullable<double> masstart { get; set; }
        public Nullable<double> masend { get; set; }
        public Nullable<double> t { get; set; }
        public Nullable<double> pres { get; set; }
        public Nullable<double> dens { get; set; }
        public Nullable<double> dens20 { get; set; }
        public Nullable<double> dens15 { get; set; }
        public Nullable<double> vol20 { get; set; }
        public Nullable<double> vol15 { get; set; }
        public Nullable<double> masnetto { get; set; }
        public Nullable<double> masballast { get; set; }
        public Nullable<System.DateTime> dtstart { get; set; }
        public string actnumber { get; set; }
        public string omniid { get; set; }
        public Nullable<double> visc { get; set; }
    }
}
