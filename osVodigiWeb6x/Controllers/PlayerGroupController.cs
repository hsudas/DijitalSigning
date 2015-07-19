using System;
using System.Collections.Generic;
using System.Web.Mvc;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class PlayerGroupController : Controller
    {
        IPlayerGroupRepository repository;

        public PlayerGroupController()
            : this(new EntityPlayerGroupRepository())
        { }

        public PlayerGroupController(IPlayerGroupRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /PlayerGroup/

        public ActionResult Index()
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                // Initialize or get the page state using session
                PlayerGroupPageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                //if (Request.Form["lstAscDesc"] != null)
                //{
                pagestate.AccountID = accountid;
                if (Request.Form["txtPlayerGroupName"] != null)
                {
                    pagestate.PlayerGroupName = Request.Form["txtPlayerGroupName"].ToString().Trim();
                }
                if (Request.Form["txtDescription"] != null)
                {
                    pagestate.Description = Request.Form["txtDescription"].ToString().Trim();
                }
                if (Request.Form["chkIncludeInactive"] != null)
                {
                    if (Request.Form["chkIncludeInactive"].ToLower().StartsWith("true"))
                        pagestate.IncludeInactive = true;
                    else
                        pagestate.IncludeInactive = false;
                }

                //pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                //pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                if (Request.Form["txtPageNumber"] != null)
                {
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                }
                SavePageState(pagestate);
                //}

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["PlayerGroupName"] = pagestate.PlayerGroupName;
                ViewData["Description"] = pagestate.Description;
                ViewData["IncludeInactive"] = pagestate.IncludeInactive;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.GetPlayerGroupRecordCount(pagestate.AccountID, pagestate.PlayerGroupName, pagestate.Description, pagestate.IncludeInactive);

                // Determine the page count
                int pagecount = 1;
                if (recordcount > 0)
                {
                    pagecount = recordcount / Constants.PageSize;
                    if (recordcount % Constants.PageSize != 0) // Add a page if there are more records
                    {
                        pagecount = pagecount + 1;
                    }
                }

                // Make sure the current page is not greater than the page count
                if (pagestate.PageNumber > pagecount)
                {
                    pagestate.PageNumber = pagecount;
                    SavePageState(pagestate);
                }

                // Set the page number and account in viewdata
                ViewData["PageNumber"] = Convert.ToString(pagestate.PageNumber);
                ViewData["PageCount"] = Convert.ToString(pagecount);
                ViewData["RecordCount"] = Convert.ToString(recordcount);

                ViewResult result = View(repository.GetPlayerGroupPage(pagestate.AccountID, pagestate.PlayerGroupName, pagestate.Description, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroup", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /PlayerGroup/Create

        public ActionResult Create()
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewPlayerGroup());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroup", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /PlayerGroup/Create

        [HttpPost]
        public ActionResult Create(PlayerGroup playergroup)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    playergroup = FillNulls(playergroup);
                    playergroup.AccountID = Convert.ToInt32(Session["UserAccountID"]);

                    string validation = ValidateInput(playergroup);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(playergroup);
                    }

                    repository.CreatePlayerGroup(playergroup);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Player Group", "Add",
                            "Player grup eklendi '" + playergroup.PlayerGroupName + "' - ID: " + playergroup.PlayerGroupID.ToString());

                    return RedirectToAction("Index");
                }

                return View(playergroup);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroup", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /PlayerGroup/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                PlayerGroup playergroup = repository.GetPlayerGroup(id);
                ViewData["ValidationMessage"] = String.Empty;

                return View(playergroup);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroup", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /PlayerGroup/Edit/5

        [HttpPost]
        public ActionResult Edit(PlayerGroup playergroup)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    playergroup = FillNulls(playergroup);

                    string validation = ValidateInput(playergroup);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(playergroup);
                    }

                    repository.UpdatePlayerGroup(playergroup);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Player Group", "Edit",
                            "Player grup güncellendi '" + playergroup.PlayerGroupName + "' - ID: " + playergroup.PlayerGroupID.ToString());

                    return RedirectToAction("Index");
                }

                return View(playergroup);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroup", "Edit POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Player Group Name";
            sortitem1.Value = "PlayerGroupName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Description";
            sortitem2.Value = "PlayerGroupDescription";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "AktifMi";
            sortitem3.Value = "IsActive";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);
            sortitems.Add(sortitem3);

            return sortitems;
        }

        private List<SelectListItem> BuildAscDescList()
        {
            // Build the asc desc list
            List<SelectListItem> ascdescitems = new List<SelectListItem>();

            SelectListItem ascdescitem1 = new SelectListItem();
            ascdescitem1.Text = "Asc";
            ascdescitem1.Value = "Asc";

            SelectListItem ascdescitem2 = new SelectListItem();
            ascdescitem2.Text = "Desc";
            ascdescitem2.Value = "Desc";

            ascdescitems.Add(ascdescitem1);
            ascdescitems.Add(ascdescitem2);

            return ascdescitems;
        }

        private PlayerGroupPageState GetPageState()
        {
            try
            {
                PlayerGroupPageState pagestate = new PlayerGroupPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["PlayerGroupPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.PlayerGroupName = String.Empty;
                    pagestate.Description = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "PlayerGroupName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["PlayerGroupPageState"] = pagestate;
                }
                else
                {
                    pagestate = (PlayerGroupPageState)Session["PlayerGroupPageState"];
                }
                return pagestate;
            }
            catch { return new PlayerGroupPageState(); }
        }

        private void SavePageState(PlayerGroupPageState pagestate)
        {
            Session["PlayerGroupPageState"] = pagestate;
        }

        private PlayerGroup FillNulls(PlayerGroup playergroup)
        {
            if (playergroup.PlayerGroupDescription == null) playergroup.PlayerGroupDescription = String.Empty;

            return playergroup;
        }

        private PlayerGroup CreateNewPlayerGroup()
        {
            PlayerGroup playergroup = new PlayerGroup();
            playergroup.PlayerGroupID = 0;
            playergroup.AccountID = 0;
            playergroup.PlayerGroupName = String.Empty;
            playergroup.PlayerGroupDescription = String.Empty;
            playergroup.IsActive = true;

            return playergroup;
        }

        private string ValidateInput(PlayerGroup playergroup)
        {
            if (playergroup.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(playergroup.PlayerGroupName))
                return "Player Group Name zorunludur.";

            return String.Empty;
        }
    }
}
