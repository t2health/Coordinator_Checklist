using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoDVAChecklist.Filters;
using DoDVAChecklist.Models;
using DoDVAChecklist.ViewModels;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;

namespace DoDVAChecklist.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    [UnapprovedUserRedirect]
    public class ChecklistController : Controller
    {

        public ActionResult View(int id)
        {
            ChecklistViewModel viewModel = new ChecklistViewModel();
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                Checklist checklist = (from cl in user.Checklists where cl.ChecklistId == id select cl).SingleOrDefault();
                if (checklist == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var history = (from e in checklist.HistoryEntries select new ChecklistHistoryEntryViewModel { Transferrer = e.Transferrer.FullName, Recipient = e.Recipient.FullName, TransferDate = e.TransferDate }).OrderByDescending(c => c.TransferDate);
                viewModel.Details = new ChecklistDetailsViewModel(JObject.Parse(checklist.Data));
                viewModel.ChecklistId = checklist.ChecklistId;
                viewModel.CreatedDate = checklist.CreatedDate;
                viewModel.ModifiedDate = checklist.ModifiedDate;
                viewModel.Data = checklist.Data;
                viewModel.HistoryEntries = history.ToList();
            }

            return View(viewModel);
        }

        public ActionResult Print(int id)
        {
            ChecklistViewModel viewModel = new ChecklistViewModel();
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                Checklist checklist = (from cl in user.Checklists where cl.ChecklistId == id select cl).SingleOrDefault();
                if (checklist == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var history = (from e in checklist.HistoryEntries select new ChecklistHistoryEntryViewModel { Transferrer = e.Transferrer.FullName, Recipient = e.Recipient.FullName, TransferDate = e.TransferDate }).OrderByDescending(c => c.TransferDate);
                viewModel.Details = new ChecklistDetailsViewModel(JObject.Parse(checklist.Data));
                viewModel.ChecklistId = checklist.ChecklistId;
                viewModel.CreatedDate = checklist.CreatedDate;
                viewModel.ModifiedDate = checklist.ModifiedDate;
                viewModel.Data = checklist.Data;
                viewModel.HistoryEntries = history.ToList();
            }

            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                Font headerFont = new Font(Font.HELVETICA, 16, Font.BOLD);
                Font valueFont = new Font(Font.HELVETICA, 11, Font.NORMAL);
                Font subHeaderFont = new Font(Font.HELVETICA, 11, Font.BOLD);

                document.Open();

                Paragraph headerParagraph = new Paragraph(new Chunk());
                Paragraph subHeaderParagraph = new Paragraph(new Chunk());
                Paragraph valueParagraph = new Paragraph(new Chunk());

                headerParagraph.SpacingBefore = 20;

                headerParagraph[0] = new Chunk("Servicemember / Veteran", headerFont);
                document.Add(headerParagraph);
                valueParagraph[0] = new Chunk(viewModel.Details.LastName + ", " + viewModel.Details.FirstName, valueFont);
                document.Add(valueParagraph);

                if (!String.IsNullOrEmpty(viewModel.Details.Phone))
                {
                    valueParagraph[0] = new Chunk(viewModel.Details.Phone, valueFont);
                    document.Add(valueParagraph);
                }

                if (!String.IsNullOrEmpty(viewModel.Details.PhoneAlternate))
                {
                    valueParagraph[0] = new Chunk(viewModel.Details.PhoneAlternate, valueFont);
                    document.Add(valueParagraph);
                }

                headerParagraph[0] = new Chunk("Service", headerFont);
                document.Add(headerParagraph);
                valueParagraph[0] = new Chunk(viewModel.Details.Service, valueFont);
                document.Add(valueParagraph);

                headerParagraph[0] = new Chunk("Service Status", headerFont);
                document.Add(headerParagraph);
                valueParagraph[0] = new Chunk(viewModel.Details.Status, valueFont);
                document.Add(valueParagraph);

                headerParagraph[0] = new Chunk("Category", headerFont);
                document.Add(headerParagraph);
                valueParagraph[0] = new Chunk(viewModel.Details.Category, valueFont);
                document.Add(valueParagraph);

                valueParagraph.Leading = 12;
                valueParagraph.SpacingAfter = 15;
                headerParagraph.SpacingAfter = 10;
                // int color = 0;
                foreach (ChecklistQuestionGroupViewModel group in viewModel.Details.QuestionGroups)
                {
                    headerParagraph[0] = new Chunk(group.Name, headerFont);
                    document.Add(headerParagraph);

                    foreach (ChecklistQuestionViewModel question in group.Questions.OrderBy(q => q.Completed))
                    {
                        valueFont.SetStyle(question.Completed ? Font.STRIKETHRU : Font.NORMAL);
                        // color = question.Completed ? 150 : 0;
                        // valueFont.SetColor(color, color, color);
                        valueParagraph[0] = new Chunk(question.Question, valueFont);
                        document.Add(valueParagraph);
                    }

                }


                headerParagraph.SpacingAfter = 0;
                // valueFont.SetColor(0, 0, 0);
                valueParagraph.Leading = 16;
                valueParagraph.SpacingAfter = 0;
                valueFont.SetStyle(Font.NORMAL);

                foreach (ChecklistDomainViewModel domain in viewModel.Details.Domains)
                {
                    headerParagraph[0] = new Chunk(domain.Name, headerFont);
                    document.Add(headerParagraph);

                    subHeaderParagraph.SpacingBefore = 0;
                    foreach (ChecklistDomainCategoryViewModel category in domain.Categories)
                    {
                        subHeaderParagraph[0] = new Chunk(category.Name, subHeaderFont);
                        document.Add(subHeaderParagraph);

                        if (category.Date.HasValue)
                        {
                            valueParagraph[0] = new Chunk(category.Date.Value.Date.ToShortDateString() + " — " + category.Status, valueFont);
                            document.Add(valueParagraph);

                            if (!String.IsNullOrEmpty(category.ResponsibleParty))
                            {
                                valueParagraph[0] = new Chunk(category.ResponsibleParty, valueFont);
                                document.Add(valueParagraph);
                            }
                        }
                        subHeaderParagraph.SpacingBefore = 15;
                    }
                }


                document.Close();

                byte[] bytes = ms.ToArray();

                return File(bytes, "application/pdf", "checklist.pdf");
            }
        }

        public ActionResult CompleteTransfer(int checklistId, int recipientId)
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                UserProfile recipient = db.UserProfiles.FirstOrDefault(u => u.UserId == recipientId);
                Checklist checklist = (from cl in user.Checklists where cl.ChecklistId == checklistId select cl).SingleOrDefault();

                if (checklist == null || recipient == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                user.Checklists.Remove(checklist);
                checklist.PreviousUser = user;
                checklist.ModifiedDate = DateTime.Now;
                checklist.HistoryEntries.Add(new ChecklistHistoryEntry { Recipient = recipient, Transferrer = user, TransferDate = DateTime.Now });
                recipient.Checklists.Add(checklist);

                int result = db.SaveChanges();

                TempData["Message"] = "Transferred checklist from " + user.UserName + " to " + recipient.UserName;
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Transfer(int id)
        {
            using (UsersContext db = new UsersContext())
            {
                UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
                Checklist checklist = (from cl in user.Checklists where cl.ChecklistId == id select cl).SingleOrDefault();
                if (checklist == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                string[] userNames = Roles.GetUsersInRole("User");
                var users = db.UserProfiles.Where(u => u.UserId != WebSecurity.CurrentUserId).ToList();
                users = users.Where(u => userNames.Contains(u.UserName)).ToList();
                ViewBag.Checklist = checklist;
                ViewBag.Users = users;
            }

            return View();
        }

    }

}
