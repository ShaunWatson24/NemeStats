﻿using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;

namespace BusinessLogic.Components
{
    public abstract class DbComponentBase<TInput, TOutput>
    {
        public abstract TOutput Execute(TInput inputParameter, ApplicationUser currentUser, IDataContext dataContextWithTransaction);
        public abstract TOutput Execute(TInput inputParameter, ApplicationUser currentUser);

    }
}