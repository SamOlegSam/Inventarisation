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
    
    public partial class trl_tanktype
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public trl_tanktype()
        {
            this.trl_tank = new HashSet<trl_tank>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<short> IsSteel { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<trl_tank> trl_tank { get; set; }
    }
}
