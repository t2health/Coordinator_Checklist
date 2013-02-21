using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoDVAChecklist.Filters;
using DoDVAChecklist.Models;
using DoDVAChecklist.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;

namespace DoDVAChecklist.Controllers
{

    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ServiceController : Controller
    {

        [HttpPost]
        [UnapprovedUserRedirect]
        public ActionResult ChecklistUpdate()
        {
            string data = Request.Form["data"];
            int checklistId = int.Parse(Request.Form["id"]);

            if (data == null || data.Length == 0)
            {
                return new HttpStatusCodeResult(400);
            }

            dynamic json = JObject.Parse(data);

            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                Checklist checklist = (from cl in user.Checklists where cl.ChecklistId == checklistId select cl).SingleOrDefault();

                if (checklist == null)
                {
                    return new HttpStatusCodeResult(400);
                }

                DateTime now = DateTime.Now;
                checklist.Data = data;
                checklist.VeteranFirstName = json.first_name;
                checklist.VeteranLastName = json.last_name;
                checklist.ModifiedDate = now;

                if (db.SaveChanges() == 1)
                {
                    return new HttpStatusCodeResult(200);
                }
                else
                {
                    return new HttpStatusCodeResult(500);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    });
                    Roles.AddUserToRole(model.UserName, "Pending");
                    WebSecurity.Login(model.UserName, model.Password);
                    return new HttpStatusCodeResult(200);
                }
                catch (MembershipCreateUserException e)
                {
                    return Json(new { Message = AccountController.ErrorCodeToString(e.StatusCode) });
                }
            }

            return new HttpStatusCodeResult(500);
        }

        [HttpPost]
        [UnapprovedUserRedirect]
        public ActionResult ChecklistAdd()
        {
            string data = Request.Form["data"];

            if (data == null || data.Length == 0)
            {
                return new HttpStatusCodeResult(400);
            }

            dynamic json = JObject.Parse(data);

            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                DateTime now = DateTime.Now;
                Checklist checklist = new Checklist
                {
                    CreatedDate = now,
                    Creator = user,
                    Data = data,
                    VeteranFirstName = json.first_name,
                    VeteranLastName = json.last_name,
                    ModifiedDate = now,
                };
                user.Checklists.Add(checklist);

                if (db.SaveChanges() > 0)
                {
                    return new HttpStatusCodeResult(200);
                }
                else
                {
                    return new HttpStatusCodeResult(500);
                }
            }
        }

        [HttpPost]
        [UnapprovedUserRedirect]
        public ActionResult ChecklistTransfer()
        {
            int checklistId = int.Parse(Request.Form["checklist_id"]);
            int recipientId = int.Parse(Request.Form["recipient_id"]);

            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                UserProfile recipient = db.UserProfiles.FirstOrDefault(u => u.UserId == recipientId);
                Checklist checklist = (from cl in user.Checklists where cl.ChecklistId == checklistId select cl).SingleOrDefault();

                if (checklist == null || recipient == null)
                {
                    return new HttpStatusCodeResult(400);
                }

                user.Checklists.Remove(checklist);
                checklist.PreviousUser = user;
                checklist.ModifiedDate = DateTime.Now;
                checklist.HistoryEntries.Add(new ChecklistHistoryEntry { Recipient = recipient, Transferrer = user, TransferDate = DateTime.Now });
                recipient.Checklists.Add(checklist);

                if (db.SaveChanges() > 0)
                {
                    return new HttpStatusCodeResult(200);
                }
                else
                {
                    return new HttpStatusCodeResult(500);
                }
            }
        }


        [UnapprovedUserRedirect]
        public ActionResult Checklist(int? id = null)
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);

                if (id != null)
                {
                    var checklist = (from cl in user.Checklists
                                     where cl.ChecklistId == id
                                     select new
                                     {
                                         id = cl.ChecklistId,
                                         created_date = cl.CreatedDate,
                                         modified_date = cl.ModifiedDate,
                                         data = cl.Data,
                                         title = cl.Title
                                     }).SingleOrDefault();

                    if (checklist == null)
                    {
                        return new HttpStatusCodeResult(400);
                    }

                    return new ContentResult { Content = JsonConvert.SerializeObject(checklist, Formatting.Indented), ContentType = "application/json", ContentEncoding = System.Text.Encoding.UTF8 };
                }
                else
                {
                    var checklists = (from c in user.Checklists
                                      select new
                                      {
                                          id = c.ChecklistId,
                                          modified_date = c.ModifiedDate,
                                          created_date = c.CreatedDate,
                                          title = c.VeteranLastName + ", " + c.VeteranFirstName + " " + c.ModifiedDate.ToString("g")
                                      }).OrderByDescending(c => c.modified_date);
                    return new ContentResult { Content = JsonConvert.SerializeObject(checklists, Formatting.Indented), ContentType = "application/json", ContentEncoding = System.Text.Encoding.UTF8 };
                }
            }
        }


        [UnapprovedUserRedirect]
        public ActionResult PendingAccount()
        {
            using (UsersContext db = new UsersContext())
            {
                if (Roles.IsUserInRole("Admin"))
                {
                    string[] pendingUsernames = Roles.GetUsersInRole("Pending");
                    var users = (from u in db.UserProfiles.Where(u => pendingUsernames.Contains(u.UserName))
                                                        select new
                                                        {
                                                            id = u.UserId,
                                                            first_name = u.FirstName,
                                                            last_name = u.LastName,
                                                            user_name = u.UserName
                                                        }).ToList();

                    return new ContentResult { Content = JsonConvert.SerializeObject(users, Formatting.Indented), ContentType = "application/json", ContentEncoding = System.Text.Encoding.UTF8 };
                }
            }
            return new HttpStatusCodeResult(400);
        }



        [UnapprovedUserRedirect]
        public ActionResult Recipient(string search)
        {
            if (search == null || search.Length < 3)
            {
                return new HttpStatusCodeResult(400);
            }

            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                string[] userNames = Roles.GetUsersInRole("User");
                var users = db.UserProfiles.Where(u => (u.FirstName.Contains(search) || u.LastName.Contains(search) || u.UserName.Contains(search)) && u.UserName != WebSecurity.CurrentUserName).ToList();
                users = users.Where(u => userNames.Contains(u.UserName)).ToList();
                var results = (from u in users select new { id = u.UserId, first_name = u.FirstName, last_name = u.LastName }).OrderBy(u => u.last_name);
                return new ContentResult { Content = JsonConvert.SerializeObject(results, Formatting.Indented), ContentType = "application/json", ContentEncoding = System.Text.Encoding.UTF8 };
            }
        }

        //
        // POST: /Account/ClientLogin


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                string[] roles = Roles.GetRolesForUser(model.UserName);
                if (!roles.Contains("Admin") && !roles.Contains("User"))
                {
                    return new HttpStatusCodeResult(488, "Account Pending Approval");
                }
                return new HttpStatusCodeResult(200);
            }

            Server.ClearError();
            return new HttpStatusCodeResult(409, "Invalid Login Credentials");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Approve(int id)
        {
            if (Roles.IsUserInRole("Admin"))
            {
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.Find(id);
                    if (user != null)
                    {
                        Roles.AddUserToRole(user.UserName, "User");
                        Roles.RemoveUserFromRole(user.UserName, "Pending");
                        return new HttpStatusCodeResult(200);
                    }
                }

            }
            return new HttpStatusCodeResult(400);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Decline(int id)
        {
            if (Roles.IsUserInRole("Admin"))
            {
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.Find(id);
                    if (user != null)
                    {
                        Roles.RemoveUserFromRoles(user.UserName, Roles.GetRolesForUser(user.UserName));
                        ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(user.UserName);
                        ((SimpleMembershipProvider)Membership.Provider).DeleteUser(user.UserName, true);
                        return new HttpStatusCodeResult(200);
                    }
                }

            }
            return new HttpStatusCodeResult(400);
        }
    }
}
