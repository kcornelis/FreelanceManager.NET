function ConfigClientsViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.clients = ko.observableArray([]);
    self.selectedClient = ko.observable();
    self.selectedProject = ko.observable();
    self.selectedProjectTasks = ko.observable();

    self.markBusy("Loading...");

    self.refreshClients = function () {
        self.clients([]);
        $.ajax("/read/clients", {
            type: "get", contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, item) {
                    self.clients.push(new ConfigClientModel(self, item));
                });
                self.refreshProjects();
            }
        });
    };

    self.refreshProjects = function () {

        $.each(self.clients(), function (index, client) {
            client.projects.removeAll();
        });

        $.ajax("/read/projects", {
            type: "get", contentType: "application/json",
            success: function (data) {

                $.each(data, function (index, project) {
                    var client = _.find(self.clients(), function (c) {
                        return project.ClientId === c.id();
                    });
                    if (client) {
                        client.projects.push(new ConfigProjectModel(self, client, project));
                    }
                });
                self.markNotBusy();
            }
        });
    };

    self.refreshClients();

    self.newClient = function () {
        self.selectedClient(new ConfigClientModel(self, null));
    };
}