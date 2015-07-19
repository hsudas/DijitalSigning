using System;
using System.Web.Mvc;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class SurveyQuestionOptionController : Controller
    {
        ISurveyQuestionOptionRepository repository;

        public SurveyQuestionOptionController()
            : this(new EntitySurveyQuestionOptionRepository())
        { }

        public SurveyQuestionOptionController(ISurveyQuestionOptionRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /SurveyQuestionOption/Create/surveyquestionid

        public ActionResult Create(int id)
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

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(id);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewSurveyQuestionOption(id));
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /SurveyQuestionOption/Create/surveyquestionid

        [HttpPost]
        public ActionResult Create(int id, SurveyQuestionOption surveyquestionoption)
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

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(id);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                if (ModelState.IsValid)
                {
                    surveyquestionoption.SurveyQuestionID = id;
                    // Note: Proper sort order is applied in the repository

                    string validation = ValidateInput(surveyquestionoption);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(surveyquestionoption);
                    }
                    else
                    {
                        repository.CreateSurveyQuestionOption(surveyquestionoption);

                        CommonMethods.CreateActivityLog((User)Session["User"], "SurveyQuestionOption", "Add",
                            "Added survey question option '" + surveyquestionoption.SurveyQuestionOptionText + "' - ID: " + surveyquestionoption.SurveyQuestionOptionID.ToString());

                        return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
                    }
                }
                return View(surveyquestionoption);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SurveyQuestionOption/Edit/5

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

                SurveyQuestionOption surveyquestionoption = repository.GetSurveyQuestionOption(id);

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(surveyquestionoption.SurveyQuestionID);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                ViewData["ValidationMessage"] = String.Empty;

                return View(surveyquestionoption);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /SurveyQuestionOption/Edit/5

        [HttpPost]
        public ActionResult Edit(SurveyQuestionOption surveyquestionoption)
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

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(surveyquestionoption.SurveyQuestionID);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                if (ModelState.IsValid)
                {
                    string validation = ValidateInput(surveyquestionoption);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(surveyquestionoption);
                    }

                    repository.UpdateSurveyQuestionOption(surveyquestionoption);

                    CommonMethods.CreateActivityLog((User)Session["User"], "SurveyQuestionOption", "Edit",
                                                    "Edited survey question option '" + surveyquestionoption.SurveyQuestionOptionText + "' - ID: " + surveyquestionoption.SurveyQuestionOptionID.ToString());

                    return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
                }

                return View(surveyquestionoption);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "Edit POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }


        //
        // GET: /SurveyQuestionOption/Delete/5

        public ActionResult Delete(int id)
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

                SurveyQuestionOption surveyquestionoption = repository.GetSurveyQuestionOption(id);

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(surveyquestionoption.SurveyQuestionID);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                repository.DeleteSurveyQuestionOption(surveyquestionoption);

                return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "Delete", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SurveyQuestion/MoveUp/5

        public ActionResult MoveUp(int id)
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

                SurveyQuestionOption surveyquestionoption = repository.GetSurveyQuestionOption(id);

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(surveyquestionoption.SurveyQuestionID);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                repository.MoveSurveyQuestionOption(surveyquestionoption, true);

                return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "MoveUp", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SurveyQuestion/MoveDown/5

        public ActionResult MoveDown(int id)
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

                SurveyQuestionOption surveyquestionoption = repository.GetSurveyQuestionOption(id);

                // Get the survey id for redirection
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                SurveyQuestion surveyquestion = qrep.GetSurveyQuestion(surveyquestionoption.SurveyQuestionID);
                ViewData["SurveyID"] = surveyquestion.SurveyID;

                repository.MoveSurveyQuestionOption(surveyquestionoption, false);

                return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SurveyQuestionOption", "MoveDown", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // Support Methods

        private SurveyQuestionOption CreateNewSurveyQuestionOption(int id)
        {
            SurveyQuestionOption surveyquestionoption = new SurveyQuestionOption();
            surveyquestionoption.SurveyQuestionOptionID = 0;
            surveyquestionoption.SurveyQuestionID = 0;
            surveyquestionoption.SurveyQuestionOptionText = String.Empty;
            surveyquestionoption.SortOrder = 1;

            return surveyquestionoption;
        }

        private string ValidateInput(SurveyQuestionOption surveyquestionoption)
        {
            if (surveyquestionoption.SurveyQuestionID == 0)
                return "Survey Question ID is not valid.";

            if (String.IsNullOrEmpty(surveyquestionoption.SurveyQuestionOptionText))
                return "Survey Question Option Text zorunludur.";

            return String.Empty;
        }
    }
}
