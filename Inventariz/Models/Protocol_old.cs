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
    
    public partial class Protocol_old
    {
        public System.Guid id { get; set; }
        public Nullable<int> filialid { get; set; }
        public int num { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
        public Nullable<System.DateTime> testdt { get; set; }
        public Nullable<int> customerid { get; set; }
        public string testobject { get; set; }
        public int samplenum { get; set; }
        public System.DateTime sampledt { get; set; }
        public int samplerid { get; set; }
        public int samplesourceid { get; set; }
        public int samplelocationid { get; set; }
        public string samplelocationstr { get; set; }
        public string sampletnpa { get; set; }
        public int laborantid { get; set; }
        public int responsibleid { get; set; }
        public Nullable<System.DateTime> startdt { get; set; }
        public Nullable<System.DateTime> enddt { get; set; }
        public Nullable<int> sampletypeid { get; set; }
        public bool isnotforpassport { get; set; }
        public Nullable<System.DateTime> endtestdt { get; set; }
    }
}