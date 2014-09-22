﻿using System.Web;
using System.Web.Http;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Controllers
{
    public partial class PlayerController : Controller
    {
        public static readonly int NUMBER_OF_RECENT_GAMES_TO_RETRIEVE = 10;

        internal IDataContext dataContext;
        internal IPlayerRepository playerRepository;
        internal IGameResultViewModelBuilder builder;
        internal IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        internal IPlayerSaver playerSaver;
        internal IPlayerRetriever playerRetriever;
        
        public PlayerController(IDataContext dataContext, 
            IPlayerRepository playerRepository, 
            IGameResultViewModelBuilder builder,
            IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder,
            IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
            IPlayerSaver playerSaver,
            IPlayerRetriever playerRetriever)
        {
            this.dataContext = dataContext;
            this.playerRepository = playerRepository;
            this.builder = builder;
            this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
            this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
            this.playerSaver = playerSaver;
            this.playerRetriever = playerRetriever;
        }

        // GET: /Player/Details/5
        [UserContextAttribute(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int? id, ApplicationUser currentUser)
        {
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PlayerDetails player = playerRepository.GetPlayerDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);

            if (player == null)
            {
                return new HttpNotFoundResult();
            }

            PlayerDetailsViewModel playerDetailsViewModel = playerDetailsViewModelBuilder.Build(player, currentUser);

            ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
                NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, 
                player.PlayerGameResults.Count);

            return View(MVC.Player.Views.Details, playerDetailsViewModel);
        }

        // GET: /Player/SavePlayer
        [System.Web.Mvc.Authorize]
        public virtual ActionResult SavePlayer()
        {
            return View(MVC.Player.Views._CreateOrUpdatePartial, new Player());
        }

        // GET: /Player/Create
        [System.Web.Mvc.Authorize]
        public virtual ActionResult Create()
        {
            return View(MVC.Player.Views.Create, new Player());
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [UserContextAttribute]
        public virtual ActionResult Save(Player model, ApplicationUser currentUser)
        {
            if (!Request.IsAjaxRequest()) 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(ModelState.IsValid)
            {
                Player player = playerSaver.Save(model, currentUser);
                return Json(player, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotModified);
        }

        // GET: /Player/Edit/5
        [System.Web.Mvc.Authorize]
        public virtual ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDetails player;
            try
            {
                player = playerRepository.GetPlayerDetails(id.Value, 0);
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }catch(KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(MVC.Player.Views.Edit, player);
        }

        // POST: /Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult Edit([Bind(Include = "Id,Name,Active,GamingGroupId")] Player player, ApplicationUser currentUser)
        {
            if (ModelState.IsValid)
            {
                dataContext.Save<Player>(player, currentUser);
                dataContext.CommitAllChanges();
                return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                          + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS);
            }
            return View(MVC.Player.Views.Edit, player);
        }

        // GET: /Player/Delete/5
        [System.Web.Mvc.Authorize]
        public virtual ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDetails playerDetails;
            try
            {
                playerDetails = playerRepository.GetPlayerDetails(id.Value, 0);
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }catch (KeyNotFoundException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            
            return View(MVC.Player.Views.Delete, playerDetails);
        }

        // POST: /Player/Delete/5
        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [UserContextAttribute]
        public virtual ActionResult DeleteConfirmed(int id, ApplicationUser currentUser)
        {
            try
            {
                dataContext.DeleteById<Player>(id, currentUser);
                dataContext.CommitAllChanges();
            }catch(UnauthorizedAccessException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
                                          + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
