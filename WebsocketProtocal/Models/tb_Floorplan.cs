//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebsocketProtocal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_Floorplan
    {
        public int ID { get; set; }
        public Nullable<int> SiteID { get; set; }
        public string FloorplanName { get; set; }
        public string PathLocation { get; set; }
        public Nullable<int> FlWidth { get; set; }
        public Nullable<int> FlHeight { get; set; }
        public Nullable<int> CanvasTop { get; set; }
        public Nullable<int> CanvasLeft { get; set; }
        public Nullable<int> CenterX { get; set; }
        public Nullable<int> CenterY { get; set; }
    }
}
