//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCustomerSupport
    {
        public string Email { get; set; }
        public int Consumer_ID { get; set; }
        public string Concern { get; set; }
        public int ID { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedMessage { get; set; }
        public System.DateTime RaisedDate { get; set; }
        public System.DateTime ResolvedDate { get; set; }
        public int Severity { get; set; }
    
        public virtual tblConsumer tblConsumer { get; set; }
    }
}
