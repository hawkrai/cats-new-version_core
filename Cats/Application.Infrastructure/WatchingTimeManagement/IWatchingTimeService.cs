﻿using System.Collections.Generic;
using LMP.Models;

namespace Application.Infrastructure.WatchingTimeManagement
{
    public interface IWatchingTimeService
    {
        void SaveWatchingTime(WatchingTime item);
        List<WatchingTime> GetAllRecords(int conceptId, int? studentId = null);
        WatchingTime GetByConceptSubject(int conceptId, int userId);
        int GetEstimatedTime(string container);
    }
}
