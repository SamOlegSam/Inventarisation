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
    
    public partial class AuthActivity
    {
        public int id { get; set; }
        public int userid { get; set; }
        public System.DateTime lastactivity { get; set; }
        public bool islogged { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
