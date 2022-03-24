﻿
namespace ProjectManagerAppLibrary.DataAccess;

public interface IStatusData
{
   Task CreateStatus(StatusModel status);
   Task<List<StatusModel>> GetAllStatuses();
}