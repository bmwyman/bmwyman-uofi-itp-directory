﻿using Microsoft.EntityFrameworkCore;
using uofi_itp_directory_data.CampusService;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory_data.DataAccess {

    public class SecurityEntryHelper(DirectoryRepository directoryRepository, DataWarehouseManager dataWarehouseManager, LogHelper logHelper) {
        private readonly DataWarehouseManager _dataWarehouseManager = dataWarehouseManager;
        private readonly DirectoryRepository _directoryRepository = directoryRepository;
        private readonly LogHelper _logHelper = logHelper;

        public async Task<(SecurityEntry?, string)> CreateSecurityEntry(string netid, int? areaId, int? officeId, string changedByNetId) {
            var editOtherPeople = false;
            if (areaId != null)
                editOtherPeople = await _directoryRepository.ReadAsync(a => a.AreaSettings.First(a => a.AreaId == areaId).AllowAdministratorsAccessToPeople);
            else if (officeId != null)
                editOtherPeople = await _directoryRepository.ReadAsync(a => a.Offices.Include(o => o.Area).ThenInclude(a => a.AreaSettings).First(o => o.Id == officeId).Area.AreaSettings.AllowAdministratorsAccessToPeople);

            var name = await _dataWarehouseManager.GetDataWarehouseItem(netid);
            if (!name.IsValid)
                return (null, $"Net ID '{netid}' not found");
            var securityEntry = new SecurityEntry(netid, name.FirstName, name.LastName, areaId, officeId, editOtherPeople);
            _ = await _directoryRepository.CreateAsync(securityEntry);
            return (securityEntry, $"Net ID '{netid}' created");
        }

        public async Task<List<SecurityEntry>> Get(int? areaId, int? officeId) {
            if (officeId != null)
                return [.. (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.OfficeId == officeId).OrderBy(c => c.ListedNameLast)))];
            else if (areaId != null)
                return [.. (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.AreaId == areaId).OrderBy(c => c.ListedNameLast)))];
            return [.. (await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.IsFullAdmin).OrderBy(c => c.ListedNameLast)))];
        }

        public async Task<int> SetAccessToOtherPeople(int areaId, bool allowAccess) {
            var returnValue = 0;
            var officeIds = (await _directoryRepository.ReadAsync(c => c.Offices.Where(o => o.AreaId == areaId).Select(o => o.Id))).ToList();
            foreach (var securityAccess in await _directoryRepository.ReadAsync(c => c.SecurityEntries.Where(se => se.AreaId == areaId || officeIds.Contains(se.OfficeId ?? -1)))) {
                securityAccess.CanEditAllPeopleInUnit = allowAccess;
                returnValue += await _directoryRepository.UpdateAsync(securityAccess);
            }
            return returnValue;
        }
    }
}