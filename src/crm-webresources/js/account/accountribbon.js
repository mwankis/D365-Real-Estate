function createTaskActivitySeries(primaryControl) {
    var numberOfyears = null;
    var confirmStrings = {
        text: "You have selected to create activity series",
        title: "Confirmation Activity Series"
    };
    var confirmOptions = { height: 200, width: 450 };
    Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {
            if (success.confirmed) {
                var dirtyRecordId = primaryControl.data.entity.getId();
                var recordId = dirtyRecordId.replace("{", "").replace("}", "");
                var execute_erp_ACCreateActivitySeries_Request = {
                    entity: { entityType: "account", id: recordId }, // entity
                    NumberOfNumbers: numberOfyears, // Edm.Int32

                    getMetadata: function () {
                        return {
                            boundParameter: "entity",
                            parameterTypes: {
                                entity: { typeName: "mscrm.account", structuralProperty: 5 },
                                NumberOfNumbers: { typeName: "Edm.Int32", structuralProperty: 1 }
                            },
                            operationType: 0, operationName: "erp_ACCreateActivitySeries"
                        };
                    }
                };

                Xrm.Utility.showProgressIndicator("Creating activity series please wait...")
                Xrm.WebApi.execute(execute_erp_ACCreateActivitySeries_Request).then(
                    function success(response) {
                        if (response.ok) { return response.json(); }
                    }
                ).then(function (responseBody) {
                   Xrm.Utility.closeProgressIndicator();
                   primaryControl.data.refresh(false).then(() => { },() => { });
                }).catch(function (error) {
                    Xrm.Utility.closeProgressIndicator();
                });
            }
            else {
                console.log("Dialog closed using Cancel button or X.");
            }
    });

}