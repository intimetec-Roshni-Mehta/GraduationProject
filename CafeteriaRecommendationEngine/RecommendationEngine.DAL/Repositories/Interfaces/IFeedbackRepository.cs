﻿using RecommendationEngine.DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.DAL.Repositories.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<Feedback> GetFeedbackAsync(int userId, int itemId);
        Task AddAsync(Feedback feedback);
    }
}
