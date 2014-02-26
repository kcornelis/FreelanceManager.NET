function ConfigProjectModel(view, client, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;
    self.client = client;
    self.original = data;

    self.name = ko.observable().extend({ required: true });
    self.description = ko.observable();
    self.hidden = ko.observable();
    self.tasks = ko.observableArray([]);

    self.errors = ko.validation.group(self);

    self.populate = function (data) {
        self.id(data.Id);
        self.name(data.Name);
        self.description(data.Description);
        self.hidden(data.Hidden);

        self.tasks.removeAll();
        for (var i = 0; i < data.Tasks.length; i++) {
            self.tasks.push(new ConfigProjectTaskModel(self.view, data.Tasks[i]));
        }
    }

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }

    self.select = function () {
        if (self.view.selectedProject() != null) {
            self.view.selectedProject().selected(false);
        }

        self.view.selectedProject(self);
        self.selected(true);
    };

    self.selectTasks = function () {
        if (self.view.selectedProjectTasks() != null) {
            self.view.selectedProjectTasks().selected(false);
        }

        self.view.selectedProjectTasks(self);
        self.selected(true);
    };

    self.cancelSave = function () {
        if (!self.isNew()) {
            self.populate(self.original);
        }
        self.selected(false);
        self.view.selectedProject(null);
    };

    self.save = function () {
        if (self.errors().length != 0) {
            self.errors.showAllMessages();
            return;
        }

        var data = self.toDto();
        self.markBusy("Saving...");

        var url = self.isNew() ? "/write/project/create" : ("/write/project/update/" + self.id());

        $.ajax(url, {
            data: data,
            type: "post", contentType: "application/json",
            success: function (result) {
                self.markNotBusy();

                if (self.isNew()) {
                    self.client.projects.push(new ConfigProjectModel(self.view, self.client, result.Project));
                }
                else {
                    self.original = result.Project;
                    self.populate(self.original);
                }

                self.selected(false);
                self.view.selectedProject(null);
            }
        });
    };

    self.cancelSaveTasks = function () {
        if (!self.isNew()) {
            self.populate(self.original);
        }
        self.selected(false);
        self.view.selectedProjectTasks(null);
    };

    self.saveTasks = function () {
        if (self.errors().length != 0) {
            self.errors.showAllMessages();
            return;
        }

        var data = self.toTasksDto();
        self.markBusy("Saving...");

        var url = "/write/project/updatetasks/" + self.id();

        $.ajax(url, {
            data: data,
            type: "post", contentType: "application/json",
            success: function (result) {
                self.markNotBusy();

                self.original = result.Project;
                self.populate(self.original);

                self.selected(false);
                self.view.selectedProjectTasks(null);
            }
        });
    };

    self.canSave = ko.computed(function () {
        return true;
    }, self);

    self.canSaveTasks = ko.computed(function () {
        return true;
    }, self);

    self.toDto = function () {
        var model = new Object();
        model.Id = self.id();
        model.ClientId = self.client.id();
        model.Name = self.name();
        model.Description = self.description();
        model.Hidden = self.hidden();

        return JSON.stringify(model);
    };

    self.toTasksDto = function () {
        var model = new Object();
        model.Id = self.id();
        model.Tasks = [];

        for (var i = 0; i < self.tasks().length; i++) {
            model.Tasks.push({
                Name: self.tasks()[i].name(),
                Rate: self.tasks()[i].rate(),
            })
        }

        return JSON.stringify(model);
    };
}