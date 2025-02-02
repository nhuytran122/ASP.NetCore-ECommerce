﻿namespace SV21T1020105.DomainModels
{
    public class Category
    {
        public int CategoryID { get; set; } 
        public string CategoryName { get; set; } = string.Empty;        
        public string Description { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
    }
}
