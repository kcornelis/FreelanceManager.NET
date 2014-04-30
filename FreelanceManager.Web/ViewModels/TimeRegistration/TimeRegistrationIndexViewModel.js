function TimeRegistrationIndexViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.timeRegistrations = ko.observableArray([]);
    self.clients = ko.observableArray([]);
    self.selectedTimeRegistration = ko.observable();
    self.selectedDate = ko.observable(new moment(new moment()));
    self.hasNoTimeRegistrations = ko.computed(function () { return self.timeRegistrations().length == 0; }, self);
    self.dateSelector = ko.observable(new moment().format("YYYY-MM-DD")); // for the date picker
    self.dateSelector.subscribe(function () {
        self.selectedDate(new moment(self.dateSelector()));
        self.refreshTimeRegistrations();
    });

    self.markBusy("Loading...");

    self.refreshTimeRegistrations = function () {
        self.timeRegistrations([]);
        $.ajax("/read/timeregistrations/getfordate/" + self.selectedDate().format("YYYY-MM-DD"), {
            type: "get", contentType: "application/json",
            success: function (data) {
                var timeRegistrations = $.map(data, function (item) {
                    return new TimeRegistrationModel(self, item);
                });
                self.timeRegistrations(timeRegistrations);
                self.markNotBusy();
            }
        });
    };
    self.refreshTimeRegistrations();

    self.refreshClientsIfEmpty = function (completed) {
        if (self.clients().length == 0) {
            self.clients([]);
            $.ajax("/read/clients", {
                type: "get", contentType: "application/json",
                success: function (clients) {
                    $.ajax("/read/projects/getactive", {
                        type: "get", contentType: "application/json",
                        success: function (projects) {

                            $.each(clients, function (clientIndex, client) {
                                var clientViewModel = new TimeRegistrationClientModel(self, client);
                                var hasProjects = false;
                                $.each(projects, function (projectIndex, project) {
                                    if (project.ClientId == client.Id) {
                                        clientViewModel.projects.push(new TimeRegistrationProjectModel(self, project));
                                        hasProjects = true;
                                    }
                                });

                                if (hasProjects)
                                    self.clients.push(clientViewModel);
                            });

                            if (completed)
                                completed();
                        }
                    });
                }
            });
        }
        else {
            if (completed)
                completed();
        }
    };
   
    self.newTimeRegistration = function () {
        var newItem = new TimeRegistrationModel(self, null);
        newItem.select();
    };

    self.nextDate = function () {
        self.selectedDate(self.selectedDate().add('days', 1));
        //self.refreshTimeRegistrations();
        self.dateSelector(self.selectedDate().format("YYYY-MM-DD"));
    };

    self.previousDate = function () {
        self.selectedDate(self.selectedDate().subtract('days', 1));
        //self.refreshTimeRegistrations();
        self.dateSelector(self.selectedDate().format("YYYY-MM-DD"));
    };

    self.deleteTimeRegistration = function (item) {
        self.timeRegistrations.remove(item);
    };
}

