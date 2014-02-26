function TimeRegistrationModel(view, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;
    self.original = data;

    self.locked = ko.observable(true);
    self.clientId = ko.observable();
    self.clientDisplay = ko.observable();
    self.projectId = ko.observable();
    self.projectDisplay = ko.observable();
    self.task = ko.observable();
    self.taskDisplay = ko.observable();
    self.taskRate = ko.observable();
    self.correctedIncome = ko.observable();
    self.correctedIncomeMessage = ko.observable();
    self.description = ko.observable('');
    self.date = ko.observable();
    self.from = ko.observable().extend({
        required: true,
        pattern: {
            message: "The field has an invalid format (hh:mm)",
            params: /^(?:[0-5]\d):(?:[0-5]\d)$/
        }
    });
    self.to = ko.observable().extend({
        required: true,
        pattern: {
            message: "The field has an invalid format (hh:mm)",
            params: /^(?:[0-5]\d):(?:[0-5]\d)$/
        }
    });
    self.timespan = ko.observable();
    
    self.errors = ko.validation.group(self);

    self.projects = ko.observableArray([]);
    self.tasks = ko.observableArray([]);

    self.findClient = function (clientId) {
        return ko.utils.arrayFirst(self.view.clients(), function (item) {
            return clientId === item.id();
        });
    };

    self.findSelectedTask = function () {
        var selectedClient = self.findClient(self.clientId());
        var selectedProject = selectedClient.findProject(self.projectId());
        return selectedProject.findTask(self.task());
    };

    self.refreshProjects = function () {
        if (self.locked())
            return;

        self.projects.removeAll();

        var selectedClient = self.findClient(self.clientId());

        if (!selectedClient) return;

        $.each(selectedClient.projects(), function (projectIndex, project) {
            self.projects.push(project);
        });
    };

    self.refreshTasks = function () {
        if (self.locked())
            return;

        self.tasks.removeAll();

        var selectedClient = self.findClient(self.clientId());

        if (!selectedClient) return;

        var selectedProject = selectedClient.findProject(self.projectId());

        if (!selectedProject) return;

        $.each(selectedProject.tasks(), function (taskIndex, task) {
            self.tasks.push(task);
        });
    };

    self.clientId.subscribe(function () {
        self.refreshProjects();
    });

    self.projectId.subscribe(function () {
        self.refreshTasks();
    });

    self.refreshLocked = function () {

        if (self.isNew()){
            self.locked(false);
            return;
        }

        self.locked(false);

        var selectedClient = self.findClient(self.clientId());
        if (!selectedClient) {
            self.locked(true);
            return;
        }

        var selectedProject = selectedClient.findProject(self.projectId());
        if (!selectedProject) {
            self.locked(true);
            return;
        }

        var selectedTask = selectedProject.findTask(self.task());
        if (!selectedTask) {
            self.locked(true);
        }
    };

    self.populate = function (data) {
        self.id(data.Id);
        self.clientId(data.ClientId);
        self.clientDisplay(data.ClientName);
        self.projectId(data.ProjectId);
        self.projectDisplay(data.ProjectName);
        self.task(data.Task);
        self.taskDisplay(data.Task);
        self.taskRate(data.Rate.Value);
        self.correctedIncome(data.CorrectedIncome ? data.CorrectedIncome.Value : '');
        self.correctedIncomeMessage(data.CorrectedIncomeMessage);
        self.description(data.Description);
        self.date(new moment(data.Date.Display));
        self.from(data.From.Display);
        self.to(data.To.Display);
        self.timespan(data.To != undefined ? new moment(data.To.Display, 'HH:mm').subtract(new moment(data.From.Display, 'HH:mm')) : new moment().subtract(new moment(data.From.Display, 'HH:mm')));
    }

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }

    self.select = function () {

        self.markBusy("Loading data...");
        self.view.refreshClientsIfEmpty(function () {
            self.markNotBusy();

            if (self.view.selectedTimeRegistration() != null) {
                self.view.selectedTimeRegistration().selected(false);
            }

            self.refreshLocked();
            self.refreshProjects();
            self.refreshTasks();

            self.view.selectedTimeRegistration(self);
            self.selected(true);
        });
    };

    self.cancelSave = function () {
        if (!self.isNew()) {
            self.populate(self.original);
        }
        self.selected(false);
        self.view.selectedTimeRegistration(null);
    };

    self.save = function () {
        if (self.errors().length != 0) {
            self.errors.showAllMessages();
            return;
        }

        var data = self.toDto();
        self.markBusy("Saving...");

        var url = self.isNew() ? "/write/timeregistration/create" : ("/write/timeregistration/update/" + self.id());

        $.ajax(url, {
            data: data,
            type: "post", contentType: "application/json",
            success: function (result) {
                self.markNotBusy();

                if (self.isNew()) {
                    self.view.timeRegistrations.push(new TimeRegistrationModel(self.view, result.TimeRegistration));
                }
                else {
                    self.original = result.TimeRegistration;
                    self.populate(self.original);
                }

                self.selected(false);
                self.view.selectedTimeRegistration(null);
            }
        });
    };

    self.executeDelete = function () {

        self.markBusy("Deleting...");

        var url = "/write/timeregistration/delete/" + self.id();

        $.ajax(url, {
            type: "post", contentType: "application/json",
            success: function (result) {
                self.markNotBusy();

                self.selected(false);
                self.view.selectedTimeRegistration(null);

                self.view.deleteTimeRegistration(self);
            }
        });
    };

    self.canSave = ko.computed(function () {
        return true;
    }, self);

    self.toDto = function () {

        var model = new Object();
        model.ClientId = !self.locked() ? self.clientId() : self.original.ClientId;
        model.ProjectId = !self.locked() ? self.projectId() : self.original.ProjectId;
        model.Task = !self.locked() ? self.task() : self.original.Task;
        model.Description = self.description();
        model.Date = self.view.selectedDate().format("YYYY-MM-DD");
        model.From = self.from();
        model.To = self.to();
        model.CorrectedIncome = self.correctedIncome();
        model.CorrectedIncomeMessage = self.correctedIncomeMessage();

        if (!self.isNew() &&
            self.original.Rate.Value !=
            self.findSelectedTask().rate()) {
            if (confirm('The rate has changed from ' + self.original.Rate.Value + ' to ' +
                self.findSelectedTask().rate() + '. Do you want to apply the new rate?')) {
                model.RefreshRate = true;
            }
            else {
                model.RefreshRate = false;
            }
        }

        return JSON.stringify(model);
    };
}