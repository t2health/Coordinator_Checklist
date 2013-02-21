using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoDVAChecklist.Models;

namespace DoDVAChecklist.ViewModels
{
    public class HomeChecklistViewModel
    {
        public string Title { get; set; }
        public int Id { get; set; }
    }

    public class PendingUserViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }

    public class HomeViewModel
    {
        public List<PendingUserViewModel> ApprovalUsers { get; set; }
        public List<HomeChecklistViewModel> Checklists { get; set; }
    }
}