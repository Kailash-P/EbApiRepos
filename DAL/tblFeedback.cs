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
    
    public partial class tblFeedback
    {
        public int Consumer_ID { get; set; }
        public string Headline { get; set; }
        public string Feedback { get; set; }
        public string ConsumerName { get; set; }
        public byte[] ConsumerProfilePicture { get; set; }
        public int ID { get; set; }
    
        public virtual tblConsumer tblConsumer { get; set; }
    }
}