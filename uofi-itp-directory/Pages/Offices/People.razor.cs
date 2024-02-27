﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using uofi_itp_directory.ControlHelper;
using uofi_itp_directory.Controls;
using uofi_itp_directory_data.Cache;
using uofi_itp_directory_data.CampusService;
using uofi_itp_directory_data.Data;
using uofi_itp_directory_data.DataAccess;
using uofi_itp_directory_data.DataModels;
using uofi_itp_directory_data.Security;

namespace uofi_itp_directory.Pages.Offices {

    public partial class People {
        private MultiChoice? _multiChoice = default!;
        private List<AreaOfficeThinObject> _officeThinObjects = default!;
        public List<JobProfileThinObject> JobProfiles { get; set; } = default!;

        public string NewNetId { get; set; } = string.Empty;
        public Office Office { get; set; } = default!;

        [Parameter]
        public int? OfficeId { get; set; }

        public string OfficeTitle { get; set; } = "Office";
        public int SelectedProfile { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected DataWarehouseManager DataWarehouseManager { get; set; } = default!;

        [Inject]
        protected DirectoryRepository DirectoryRepository { get; set; } = default!;

        [Inject]
        protected JobProfileHelper JobProfileHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected NavigationManager NavManager { get; set; } = default!;

        [Inject]
        protected PersonOptionHelper PersonOptionHelper { get; set; } = default!;

        [Inject]
        protected SecurityEntryHelper SecurityEntryHelper { get; set; } = default!;

        public async Task AssignId() {
            OfficeId = _multiChoice?.SelectedId;
            OfficeTitle = _multiChoice?.SelectedTitle ?? "";
            await AssignTextFields();
        }

        public async Task Edit() {
            CacheHelper.SetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, GetThinObject().EmployeeId);
            NavManager.NavigateTo($"/profile/general");
        }

        public async Task LookupId() {
            if (!string.IsNullOrWhiteSpace(NewNetId)) {
                var name = await DataWarehouseManager.GetDataWarehouseItem(NewNetId);
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", string.IsNullOrWhiteSpace(name.Name) ? "No name found" : name.Name);
            }
        }

        public async Task Remove() {
            var thinObject = GetThinObject();
            if (await JsRuntime.InvokeAsync<bool>("confirm", $"This will remove the employee {thinObject.Display} from the profile list. Are you sure?")) {
                _ = await JobProfileHelper.RemoveJobProfile(thinObject.JobProfileId, thinObject.EmployeeId, thinObject.EmployeeNetId, await AuthenticationStateProvider.GetUser());
                JobProfiles.RemoveAll(jp => jp.JobProfileId == thinObject.JobProfileId);
                SelectedProfile = 0;
                StateHasChanged();
            }
        }

        public async Task Send() {
            if (OfficeId.HasValue && !string.IsNullOrWhiteSpace(NewNetId)) {
                var (employeeId, message) = await JobProfileHelper.GenerateJobProfile(OfficeId.Value, NewNetId, await AuthenticationStateProvider.GetUser());
                if (employeeId == 0) {
                    _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", message);
                } else {
                    CacheHelper.SetCachedEmployee(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder, employeeId);
                    NavManager.NavigateTo($"/profile/general");
                }
                StateHasChanged();
            } else {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Net ID needs to be filled out");
            }
        }

        protected override async Task OnInitializedAsync() {
            var cachedAreaThinObject = CacheHelper.GetCachedOffice(await AuthenticationStateProvider.GetAuthenticationStateAsync(), CacheHolder);
            if (cachedAreaThinObject != null) {
                OfficeId = cachedAreaThinObject.Id;
                OfficeTitle = cachedAreaThinObject.Title;
                await AssignTextFields();
            }
            _officeThinObjects = await AccessHelper.GetOffices(await AuthenticationStateProvider.GetAuthenticationStateAsync(), PersonOptionHelper);
            if (_officeThinObjects.IsSingle()) {
                OfficeId = _officeThinObjects.Single().Id;
                OfficeTitle = _officeThinObjects.Single().Title;
                await AssignTextFields();
            }
        }

        private async Task AssignTextFields() {
            if (OfficeId.HasValue) {
                Office = DirectoryRepository.Read(d => d.Offices.Single(a => a.Id == OfficeId));
                JobProfiles = (await JobProfileHelper.GetJobProfileThinObjects(OfficeId.Value)) ?? new List<JobProfileThinObject>();
            }
        }

        private JobProfileThinObject GetThinObject() => JobProfiles.Single(jp => jp.JobProfileId == SelectedProfile);
    }
}