function accountViewModel(parent, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.parent = parent;
    self.original = data;

    self.name = ko.observable().extend({ required: true });
    self.firstName = ko.observable().extend({ required: true });
    self.lastName = ko.observable().extend({ required: true });
    self.email = ko.observable('').extend({ required: true, email: true });
    self.password = ko.observable();

    self.errors = ko.validation.group(self);

    self.populate = function (data) {
        if (data) {
            self.id(data.Id);
            self.name(data.Name);
            self.firstName(data.FirstName);
            self.lastName(data.LastName);
            self.email(data.Email);
        }
    }

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }

    self.select = function () {
        if (self.parent.selectedAccount() != null) {
            self.parent.selectedAccount().selected(false);
        }

        self.parent.selectedAccount(self);
        self.selected(true);
    };

    self.changePassword = function () {
        self.password(null);
        self.parent.accountToChangePassword(null);
        self.parent.accountToChangePassword(self);
    };

    self.cancelNewPassword = function () {
        self.password(null);
        self.parent.accountToChangePassword(null);
    };

    self.saveNewPassword = function () {

        var model = new Object();
        model.Password = self.password();

        self.markBusy("Saving the account...");

        $.ajax("/write/admin/account/" + self.id() + "/newpassword", {
            data: JSON.stringify(model),
            type: "post", contentType: "application/json",
            success: function () {
                self.markNotBusy();
                self.parent.accountToChangePassword(null);
            }
        });
    };

    self.cancelSave = function () {
        self.populate(self.original);
        self.selected(false);
        self.parent.selectedAccount(null);
    };
    
    self.save = function () {
        if (self.errors().length != 0) {
            self.errors.showAllMessages();
            return;
        }

        var dto = self.toDto();
        self.markBusy("Saving the account...");

        var url = self.isNew() ? "/write/admin/account/create" : ("/write/admin/account/update/" + self.id())

        $.ajax(url, {
            data: dto,
            type: 'post', contentType: "application/json",
            success: function (result) {
                self.markNotBusy();
                if (self.isNew()) {
                    self.parent.accounts.push(new accountViewModel(self.parent, result.Account));
                    alert("Pasword for the account: " + result.Password);
                } else {
                    self.original = result.Account;
                    self.populate(self.original);
                }
                self.parent.selectedAccount().selected(false);
                self.parent.selectedAccount(null);
            }
        });
    };

    self.canSave = ko.computed(function () {
        return true;
    }, self);

    self.toDto = function () {

        var model = new Object();
        model.Name = self.name();
        model.FirstName = self.firstName();
        model.LastName = self.lastName();
        model.Email = self.email();
        return JSON.stringify(model);
    };
}