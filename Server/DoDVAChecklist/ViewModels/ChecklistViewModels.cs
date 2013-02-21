using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DoDVAChecklist.ViewModels
{
    public class ChecklistHistoryEntryViewModel
    {
        public DateTime TransferDate { get; set; }
        public String Transferrer { get; set; }
        public String Recipient { get; set; }
    }

    public class ChecklistViewModel
    {
        public int ChecklistId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ChecklistDetailsViewModel Details { get; set; }
        public string Data { get; set; }
        public List<ChecklistHistoryEntryViewModel> HistoryEntries { get; set; }
    }

    public class ChecklistDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string PhoneAlternate { get; set; }
        public string Category { get; set; }
        public List<ChecklistQuestionGroupViewModel> QuestionGroups { get; set; }
        public List<ChecklistDomainViewModel> Domains { get; set; }

        public ChecklistDetailsViewModel(JObject json)
        {
            JToken value;

            FirstName = json.TryGetValue("first_name", out value) ? value.ToString() : "";
            LastName = json.TryGetValue("last_name", out value) ? value.ToString() : "";
            Service = json.TryGetValue("service", out value) ? value.ToString() : "";
            Status = json.TryGetValue("service_status", out value) ? value.ToString() : "";
            Category = json.TryGetValue("category", out value) ? value.ToString() : "";
            Phone = json.TryGetValue("phone_number", out value) ? value.ToString() : "";
            PhoneAlternate = json.TryGetValue("secondary_phone", out value) ? value.ToString() : "";

            QuestionGroups = new List<ChecklistQuestionGroupViewModel>();
            Domains = new List<ChecklistDomainViewModel>();

            ParseQuestions(json);

            ParseDomains(json);
        }

        private void ParseQuestions(JObject json)
        {
            JToken value;

            ChecklistQuestionGroupViewModel questionGroup;

            questionGroup = new ChecklistQuestionGroupViewModel();
            questionGroup.Name = "Part I: Initial/New Lead Coordinator";
            questionGroup.Questions = new List<ChecklistQuestionViewModel> {
                new ChecklistQuestionViewModel("Review of patients medical and / or non-medical record", json.TryGetValue("review_of_patient_medical_records", out value) ? true : false),
                new ChecklistQuestionViewModel("Authorization(s) for care received (i.e. TRICARE, M MSO) if applicable", json.TryGetValue("auth_care_received", out value) ? true : false),
                new ChecklistQuestionViewModel("Ensure warm hand-off between sending and receiving PCMs and case managers", json.TryGetValue("handoff_case_managers", out value) ? true : false),
                new ChecklistQuestionViewModel("Introduce lead coordinator role to servicemember / veteran and family", json.TryGetValue("intro_coordinator_servicemember", out value) ? true : false),
                new ChecklistQuestionViewModel("Conduct introductions of the care team to servicememeber/veteran and family", json.TryGetValue("care_team_servicemember", out value) ? true : false),
                new ChecklistQuestionViewModel("Assessment / requirements for servicemember / veteran / family immediate needs (health care, childcare, transportation, financial, lodging, etc.)", json.TryGetValue("assess_immediate_needs", out value) ? true : false),
                new ChecklistQuestionViewModel("Schedule initial meeting with Medical Management Team (MTF providers and VA liaison) and determine frequency of follow on meetings (include family as necessary)", json.TryGetValue("schedule_initial_meeting", out value) ? true : false),
                new ChecklistQuestionViewModel("Schedule initial meeting with the Recovering Warrior Service Program team and determine frequency of follow on meetings (include family as necessary)", json.TryGetValue("schedule_initial_recovering", out value) ? true : false),
                new ChecklistQuestionViewModel("Ensure Comprehensive Plan is up to date", json.TryGetValue("ensure_plan_up_to_date", out value) ? true : false)
            };
            QuestionGroups.Add(questionGroup);

            questionGroup = new ChecklistQuestionGroupViewModel();
            questionGroup.Name = "Part II: Ongoing Tasks";
            questionGroup.Questions = new List<ChecklistQuestionViewModel> {
                new ChecklistQuestionViewModel("Weekly updates for Medical Management Team", json.TryGetValue("weekly_updates_for_MMT", out value) ? true : false),
                new ChecklistQuestionViewModel("Weekly updates with non-medical Recovering Warrior Program team", json.TryGetValue("weekly_updates_recovering", out value) ? true : false),
                new ChecklistQuestionViewModel("If active duty servicemember admitted to VAMC inpatient rehab or receiving outpatient VA health care, VA lead coordinator providing at least monthly communication/clinical updates to military PoC", json.TryGetValue("admitted_to_VAMC_inpatient", out value) ? true : false),
                new ChecklistQuestionViewModel("Communicate with the PEBLO (Physical Evaluation Board Liaison Officer)", json.TryGetValue("communicate_w_PEBLO", out value) ? true : false),
                new ChecklistQuestionViewModel("Complete the Master Comprehensive Plan Elements list", json.TryGetValue("master_comprehensive_plan_elements", out value) ? true : false)
            };
            QuestionGroups.Add(questionGroup);

            questionGroup = new ChecklistQuestionGroupViewModel();
            questionGroup.Name = "Part III: Transfer to New Lead Coordinator";
            questionGroup.Questions = new List<ChecklistQuestionViewModel> {
                new ChecklistQuestionViewModel("Contact receiving facility / unit / VA liaison", json.TryGetValue("contact_receiving_liaison", out value) ? true : false),
                new ChecklistQuestionViewModel("Provide packet to receiving facility / unit including copy (printed or electronic) of current Comprehensive Plan", json.TryGetValue("provide_packet_to_facility", out value) ? true : false),
                new ChecklistQuestionViewModel("Discuss potential referrals to other medical activities / providers", json.TryGetValue("discuss_potential_referrals", out value) ? true : false),
                new ChecklistQuestionViewModel("Ensure initial follow-up appointments/admission schedule at next facility of care", json.TryGetValue("ensure_follow-up_appointments_scheduled", out value) ? true : false),
                new ChecklistQuestionViewModel("Ensure servicemember has suficient supply of medications until initial follow-up appointment", json.TryGetValue("ensure_sufficient_medications", out value) ? true : false),
                new ChecklistQuestionViewModel("Ensure authorizations for care are requested / received", json.TryGetValue("ensure_authorizations", out value) ? true : false),
                new ChecklistQuestionViewModel("Ensure new lead coordinator has been identified and communicated to servicemember / veteran / family / team", json.TryGetValue("ensure_new_coordinator_identified", out value) ? true : false),
            };
            QuestionGroups.Add(questionGroup);
        }

        private void ParseDomains(JObject json)
        {
            JToken value;
            JArray domains, categories;
            ChecklistDomainViewModel domain;
            ChecklistDomainCategoryViewModel category;

            Dictionary<string, ChecklistDomainViewModel> domainDict = new Dictionary<string, ChecklistDomainViewModel> {
                {"Care", new ChecklistDomainViewModel { Name="Career", Description="Please complete the date addressed, note the status (i.e. not started, in progres, completed, or N/A), and the responsible party for the elements of the Comprehensive Plan. This summary of the elements will inform the new lead coordinator to consult the complete Interagency Comprehensive Plan for complete ations and details of that element." }},
                {"Dail", new ChecklistDomainViewModel { Name="Daily Living" }},
                {"Fami", new ChecklistDomainViewModel { Name="Family" }},
                {"Fina", new ChecklistDomainViewModel { Name="Finances" }},
                {"Heal", new ChecklistDomainViewModel { Name="Health" }},
                {"Lega", new ChecklistDomainViewModel { Name="Legal" }},
                {"Mili", new ChecklistDomainViewModel { Name="Military" }},
                {"Spir", new ChecklistDomainViewModel { Name="Spirituality" }},
            };

            foreach (ChecklistDomainViewModel dom in domainDict.Values)
            {
                Domains.Add(dom);
            }

            Dictionary<string, List<ChecklistDomainCategoryViewModel>> catsDict = new Dictionary<string, List<ChecklistDomainCategoryViewModel>>();
            catsDict["Care"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Career Counseling"),
                new ChecklistDomainCategoryViewModel("Education"),
                new ChecklistDomainCategoryViewModel("Employment"),
                new ChecklistDomainCategoryViewModel("Licensure, Certification and Security Clearances"),
                new ChecklistDomainCategoryViewModel("Vocational Rehabilitation & Employment (VBA, VHA, Compensated Work Therapy, etc.)"),
                new ChecklistDomainCategoryViewModel("Referral & Coordination of Career Services with Department of Labor"),
            };
            catsDict["Dail"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Communication & Translation"),
                new ChecklistDomainCategoryViewModel("Adaptive Equipment & Assistive Technology"),
                new ChecklistDomainCategoryViewModel("Driving Privileges"),
                new ChecklistDomainCategoryViewModel("Housing Adaptations (SAH / SHA / HISA)"),
                new ChecklistDomainCategoryViewModel("Auto Grant"),
                new ChecklistDomainCategoryViewModel("Clothing Allowance Finances"),
                new ChecklistDomainCategoryViewModel("Community Re-integration (NGO / VSO Support, etc."),
                new ChecklistDomainCategoryViewModel("Emergency & Disaster Planning"),
                new ChecklistDomainCategoryViewModel("Home Care Services"),
                new ChecklistDomainCategoryViewModel("Homemaker & Home Health Aide (HHA) Services"),
                new ChecklistDomainCategoryViewModel("Housing (Permanant / Temporary / Homeless)"),
                new ChecklistDomainCategoryViewModel("Independent Living"),
                new ChecklistDomainCategoryViewModel("Self-Care"),
                new ChecklistDomainCategoryViewModel("Special VA Programs (CWT, HBPC, MHICM, TBI/AL, Medical Foster Homes, Veteran Directed Home, Community Based Programs)"),
                new ChecklistDomainCategoryViewModel("Transportation"),
            };
            catsDict["Fami"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Spouse / Family Orders (Lodging, Travel, Meals, etc.)"),
                new ChecklistDomainCategoryViewModel("Caregiver Support"),
                new ChecklistDomainCategoryViewModel("Family Medical Leave Act"),
                new ChecklistDomainCategoryViewModel("Respite Care"),
                new ChecklistDomainCategoryViewModel("Client Death"),
                new ChecklistDomainCategoryViewModel("Emotional Support"),
                new ChecklistDomainCategoryViewModel("Family Assistance"),
            };
            catsDict["Fina"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Life Insurance (TSGLI, VGLI, FGLI, SDVI, etc.)"),
                new ChecklistDomainCategoryViewModel("Non-Medical Bills / Debt"),
                new ChecklistDomainCategoryViewModel("Transition from Military to VA Compensation Process"),
                new ChecklistDomainCategoryViewModel("Benefits Counseling"),
                new ChecklistDomainCategoryViewModel("Disability Compensation"),
                new ChecklistDomainCategoryViewModel("Federal State & Income Tax"),
                new ChecklistDomainCategoryViewModel("Fiduciary"),
                new ChecklistDomainCategoryViewModel("Financial Counseling & Plan of Action (POA)"),
                new ChecklistDomainCategoryViewModel("Health Insurance (SSDI, CHAMPVA, TRICARE, etc.)"),
                new ChecklistDomainCategoryViewModel("Military Combat Specialty Pay & Pay and Allowance Continuation (PAC)"),
                new ChecklistDomainCategoryViewModel("Military Pay"),
                new ChecklistDomainCategoryViewModel("Social Security Benefits"),
                new ChecklistDomainCategoryViewModel("Special Compensation for Assistance with Activities of Daily Living (SCAADL)"),
                new ChecklistDomainCategoryViewModel("VBA Loan Guaranty"),
                new ChecklistDomainCategoryViewModel("Property Taxes"),
                new ChecklistDomainCategoryViewModel("Emergency Financial Relief Resources"),
                new ChecklistDomainCategoryViewModel("Unemployment Compensation"),
                new ChecklistDomainCategoryViewModel("Veteran Compensation, Pension & VA Claims"),
            };
            catsDict["Heal"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Advance Directives / Health Care Plan of Action (POA)"),
                new ChecklistDomainCategoryViewModel("Integrative Medicine"),
                new ChecklistDomainCategoryViewModel("Clinical Case Management"),
                new ChecklistDomainCategoryViewModel("Behavioral Health (inc. Suicidal Health, Homicidal Issues)"),
                new ChecklistDomainCategoryViewModel("Referral & Coordination with the Veterans Health Center (VHA)"),
                new ChecklistDomainCategoryViewModel("Substance Abuse"),
                new ChecklistDomainCategoryViewModel("Traumatic Brain Injury Care"),
                new ChecklistDomainCategoryViewModel("Spinal Cord Injuries / Disorder Care"),
                new ChecklistDomainCategoryViewModel("Hearing / Audiology"),
                new ChecklistDomainCategoryViewModel("Vision (Ophthamology / Optometry"),
                new ChecklistDomainCategoryViewModel("Prosthetics"),
                new ChecklistDomainCategoryViewModel("Recreation Therapy & Adaptive Sports / Reconditioning"),
                new ChecklistDomainCategoryViewModel("Rehabilitation (Kinesiotherapy, Physical Therapy, Occupational Therapy, Speech Therapy, Blindness/Vision Rehab.)"),
                new ChecklistDomainCategoryViewModel("Therapy & Service Dogs"),
                new ChecklistDomainCategoryViewModel("Dental Coverage"),
                new ChecklistDomainCategoryViewModel("Nutrition"),
                new ChecklistDomainCategoryViewModel("Referral to VHA for Functional Capacity Evaluation (Return to Work Assessment)"),
                new ChecklistDomainCategoryViewModel("Transition to VA Care"),
                new ChecklistDomainCategoryViewModel("PDHA / PDHRA"),
            };
            catsDict["Lega"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("DES-JAG (Judge Advocate General Counseling)"),
                new ChecklistDomainCategoryViewModel("Civilian Court"),
                new ChecklistDomainCategoryViewModel("Conservatorship (Property)"),
                new ChecklistDomainCategoryViewModel("Family Court"),
                new ChecklistDomainCategoryViewModel("Guardianship / Power of Attorney (POA)"),
                new ChecklistDomainCategoryViewModel("Military Court"),
                new ChecklistDomainCategoryViewModel("Veterans Court"),
                new ChecklistDomainCategoryViewModel("Citizenship (Service Member, Veteran, Caregiver, etc.)"),
                new ChecklistDomainCategoryViewModel("Referral & Coordination with Veterans Service Organizations"),
            };
            catsDict["Mili"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Non-Clinical Case Managemen"),
                new ChecklistDomainCategoryViewModel("Awards & Decorations"),
                new ChecklistDomainCategoryViewModel("Continue on Active Duty or Active Reserve (COAD / COAR)"),
                new ChecklistDomainCategoryViewModel("Line of Duty"),
                new ChecklistDomainCategoryViewModel("Military & Personal Belongings"),
                new ChecklistDomainCategoryViewModel("Service Member Orders"),
                new ChecklistDomainCategoryViewModel("Disability Evaluation"),
                new ChecklistDomainCategoryViewModel("Non-Medical Retirement & Separation"),
                new ChecklistDomainCategoryViewModel("Promotions"),
                new ChecklistDomainCategoryViewModel("Reserve / Guard (Transition Assistance Advisors, etc.)"),
                new ChecklistDomainCategoryViewModel("Command Issues (Ongoing Communication with Servicemember Command)"),
                new ChecklistDomainCategoryViewModel("Other Military Considerations"),
            };
            catsDict["Spir"] = new List<ChecklistDomainCategoryViewModel> {
                new ChecklistDomainCategoryViewModel("Retreats"),
                new ChecklistDomainCategoryViewModel("Chaplin / Religious Services"),
                new ChecklistDomainCategoryViewModel("Counseling"),
                new ChecklistDomainCategoryViewModel("Support Groups"),
            };

            foreach (KeyValuePair<string, List<ChecklistDomainCategoryViewModel>> pair in catsDict)
            {
                domainDict[pair.Key].Categories = pair.Value;
            }

            if (json.TryGetValue("domains", out value))
            {
                domains = value as JArray;
                if (domains != null)
                {
                    foreach (JObject domainJson in domains)
                    {
                        domainJson.TryGetValue("categories", out value);
                        categories = value as JArray;
                        if (categories != null)
                        {
                            foreach (JObject catJson in categories)
                            {
                                string name = (string)catJson["name"];
                                string[] split = name.Split('-');
                                string domainKey = split[0];
                                int catIndex = int.Parse(split[3]);

                                domain = domainDict[domainKey];
                                category = domain.Categories[catIndex];

                                string dateStr = (string)catJson["date"];
                                if (!String.IsNullOrEmpty(dateStr))
                                {
                                    category.Date = DateTime.ParseExact(dateStr, "yyyy-MM-dd", null);
                                }
                                category.Status = (string)catJson["status"];
                                category.ResponsibleParty = (string)catJson["responsible_party"];
                            }
                        }
                    }
                }
            }
        }
    }

    public class ChecklistQuestionGroupViewModel
    {
        public string Name { get; set; }
        public List<ChecklistQuestionViewModel> Questions { get; set; }
    }

    public class ChecklistQuestionViewModel
    {
        public string Question { get; set; }
        public bool Completed { get; set; }

        public ChecklistQuestionViewModel(string question, bool completed)
        {
            Question = question;
            Completed = completed;
        }
    }

    public class ChecklistDomainViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ChecklistDomainCategoryViewModel> Categories { get; set; }
    }

    public class ChecklistDomainCategoryViewModel
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public string ResponsibleParty { get; set; }

        public ChecklistDomainCategoryViewModel(string name)
        {
            Name = name;
        }
    }
}