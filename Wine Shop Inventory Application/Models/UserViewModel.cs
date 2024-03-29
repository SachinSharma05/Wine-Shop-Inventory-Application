﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wine_Shop_Inventory_Application.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool IsAvtive { get; set; }
        public string PasswordResetToken { get; set; }
        public Nullable<System.DateTime> PasswordResetTokenExpiry { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }

    public class CurrentUser
    {
        public int UserId { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}