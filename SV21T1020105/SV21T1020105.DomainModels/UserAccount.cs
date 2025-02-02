﻿namespace SV21T1020105.DomainModels
{
    public class UserAccount
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string RoleNames { get; set; } = "";
        public bool IsLocked { get; set; }
    }
}
