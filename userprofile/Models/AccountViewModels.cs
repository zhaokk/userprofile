﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace userprofile.Models
{


    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        
        public HttpPostedFileBase upload { get; set; }
        
        public string photoDir { get; set; }

    

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]        
        public string Email { get; set; }
        [Required]
        public int phoneNum { get; set; }

        [Required]
        public string country { get; set; }

        [Required]
        public int postcode { get; set; }

        [Required]
        public int streetNumber { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public string dob { get; set; }
        [Required]
        public string street { get; set; }
        
     
        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                photoDir=this.photoDir,
                UserName = this.UserName,
                firstName = this.FirstName,
                lastName = this.LastName,
                email = this.Email,
               country = this.country,
            dob = this.dob,
           phoneNum =  this.phoneNum,
            state = this.state,
            street = this.street,
            streetNumber = this.streetNumber


            };
            return user;
        }
    }

    public class EditUserViewModel
    {
        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;
            this.FirstName = user.firstName;
            this.LastName = user.lastName;
            this.Email = user.email;
            this.country = user.country;
            this.dob = user.dob;
            this.phoneNum = user.phoneNum;
            this.state = user.state;
            this.street = user.street;
            this.streetNumber = user.streetNumber;
        }


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public int phoneNum { get; set; }
        [Required]
        public string UserName { get; set; }

        [Required]
        public string country { get; set; }

        [Required]
        public int postcode { get; set; }

        [Required]
        public int streetNumber { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public string dob { get; set; }
        [Required]
        public string street { get; set; }
    }

    public class SelectUserRolesViewModel
    {
        public SelectUserRolesViewModel()
        {
            this.Roles = new List<SelectRoleEditorViewModel>();
        }


        // Enable initialization with an instance of ApplicationUser:
        public SelectUserRolesViewModel(ApplicationUser user)
            : this()
        {
            this.UserName = user.UserName;
            this.FirstName = user.firstName;
            this.LastName = user.lastName;

            var Db = new ApplicationDbContext();

            // Add all available roles to the list of EditorViewModels:
            var allRoles = Db.Roles;
            foreach (var role in allRoles)
            {
                // An EditorViewModel will be used by Editor Template:
                var rvm = new SelectRoleEditorViewModel(role);
                this.Roles.Add(rvm);
            }

            // Set the Selected property to true for those roles for 
            // which the current user is a member:
            foreach (var userRole in user.Roles)
            {
                var checkUserRole =
                    this.Roles.Find(r => r.RoleName == userRole.Role.Name);
                checkUserRole.Selected = true;
            }
        }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<SelectRoleEditorViewModel> Roles { get; set; }
    }

    // Used to display a single role with a checkbox, within a list structure:
    public class SelectRoleEditorViewModel
    {
        public SelectRoleEditorViewModel() { }
        public SelectRoleEditorViewModel(IdentityRole role)
        {
            this.RoleName = role.Name;
        }

        public bool Selected { get; set; }

        [Required]
        public string RoleName { get; set; }
    }

    public class logindetialViewModel
    {
        public logindetialViewModel(ApplicationUser user){
            this.UserName = user.UserName;
            this.photo = user.photoDir;
    
    }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "profile")]
        public string photo { get; set; }
    
    }
}