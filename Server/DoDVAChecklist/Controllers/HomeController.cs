using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoDVAChecklist.Filters;
using DoDVAChecklist.Models;
using DoDVAChecklist.ViewModels;
using WebMatrix.WebData;
using System.Data.Entity;
using System.Web.Security;

namespace DoDVAChecklist.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class HomeController : Controller
    {
        [UnapprovedUserRedirect]
        public ActionResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles
                    .Include(u => u.Checklists.Select(c => c.Creator))
                    .FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);

                if (Roles.IsUserInRole("Admin"))
                {
                    string[] pendingUsernames = Roles.GetUsersInRole("Pending");
                    viewModel.ApprovalUsers = (from u in db.UserProfiles.Where(u => pendingUsernames.Contains(u.UserName))
                                               select new PendingUserViewModel
                                               {
                                                   UserId = u.UserId,
                                                   FirstName = u.FirstName,
                                                   LastName = u.LastName,
                                                   UserName = u.UserName
                                               }).ToList();
                }

                if (user != null)
                {
                    viewModel.Checklists = (from c in user.Checklists select new HomeChecklistViewModel { Id = c.ChecklistId, Title = c.Title }).ToList();

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(viewModel);
        }

        public ActionResult PendingAuth()
        {
            return View();
        }

    }

}
