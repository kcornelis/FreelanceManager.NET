function TimeRegistrationImportProjectModel(view, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;

    self.name = ko.observable();
    self.clientId = ko.observable();
    self.clientName = ko.observable();

    self.populate = function (data) {
        self.id(data.Id);
        self.name(data.Name);
        self.clientId(data.ClientId);
        self.clientName(data.ClientName);
    }

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }
}