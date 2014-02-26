function accountIndexViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.accounts = ko.observableArray([]);
    self.selectedAccount = ko.observable();
    self.accountToChangePassword = ko.observable();

    self.markBusy("Loading accounts...");

    $.ajax("/read/admin/accounts", {
        type: "get", contentType: "application/json",
        success: function (data) {
            var accounts = $.map(data, function (item) {
                return new accountViewModel(self, item);
            });
            self.accounts(accounts);
            self.markNotBusy();
        }
    });

    self.newAccount = function () {
        self.selectedAccount(new accountViewModel(self, null));
    };
}