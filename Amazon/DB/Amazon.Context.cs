﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Amazon.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AmazonEntities : DbContext
    {
        public AmazonEntities()
            : base("name=AmazonEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<admin_master> admin_master { get; set; }
        public virtual DbSet<order_details> order_details { get; set; }
        public virtual DbSet<product_category> product_category { get; set; }
        public virtual DbSet<product_master> product_master { get; set; }
        public virtual DbSet<user_feedback> user_feedback { get; set; }
        public virtual DbSet<user_master> user_master { get; set; }
        public virtual DbSet<wishlist_product> wishlist_product { get; set; }
    }
}
