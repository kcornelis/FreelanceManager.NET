function ConfigClientModel(view, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;
    self.original = data;

    self.projects = ko.observableArray([]);
    self.name = ko.observable().extend({ required: true });

    self.errors = ko.validation.group(self);

    self.populate = function (data) {
        self.id(data.Id);
        self.name(data.Name);
    }

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }

    self.select = function () {
        if (self.view.selectedClient() != null) {
            self.view.selectedClient().selected(false);
        }

        self.view.selectedClient(self);
        self.selected(true);
    };

    self.newProject = function () {
        self.view.selectedProject(new ConfigProjectModel(self.view, self, null));
    };

    self.cancelSave = function () {
        if (!self.isNew()) {
            self.populate(self.original);
        }
        self.selected(false);
        self.view.selectedClient(null);
    };

    self.save = function () {
        if (self.errors().length != 0) {
            self.errors.showAllMessages();
            return;
        }

        var data = self.toDto();
        self.markBusy("Saving...");

        var url = self.isNew() ? "/write/client/create" : ("/write/client/update/" + self.id());

        $.ajax(url, {
            data: data,
            type: "post", contentType: "application/json",
            success: function (result) {
                self.markNotBusy();

                if (self.isNew()) {
                    self.view.clients.push(new ConfigClientModel(self.view, result.Client));
                }
                else {
                    self.original = result.Client;
                    self.populate(self.original);
                }

                self.selected(false);
                self.view.selectedClient(null);
            }
        });
    };

    self.canSave = ko.computed(function () {
        return true;
    }, self);

    self.toDto = function () {
        var model = new Object();
        model.Name = self.name();
        return JSON.stringify(model);
    };
}