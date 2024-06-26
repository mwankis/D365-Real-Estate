function completeAndNew(primaryControl) {
    var record = {};
    record.statuscode = 5;
    record.statecode = 1;
    var dirtyRecordId = primaryControl.data.entity.getId();
    var recordId = dirtyRecordId.replace("{", "").replace("}", "");
    var regardingobjectidControl = primaryControl.getAttribute("regardingobjectid");
    Xrm.WebApi.updateRecord("task", recordId, record).then(
        function success(result) {
            var entityFormOptions = {};
            entityFormOptions["entityName"] = "task";

            var formParameters = {};
            if (regardingobjectidControl) {
                var regardingObject = regardingobjectidControl.getValue();
                formParameters["regardingobjectid"] = regardingObject;
            }

            Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                function (success) {
                    console.log(success);
                },
                function (error) {
                    console.log(error);
                });
        },
        function (error) {
            console.log(error.message);
        }
    );
}

async function createRegardingEmail(primaryControl) {
    var dirtyRecordId = primaryControl.data.entity.getId();
    var recordId = dirtyRecordId.replace("{", "").replace("}", "");
    var regardingobjectidControl = primaryControl.getAttribute("regardingobjectid");
    var regardingObject = regardingobjectidControl.getValue();
    if (regardingObject == null) {
        var confirmStrings = {
            confirmButtonLabel: "OK",
            text: "Rquired regarding field is empty on this meeting.",
            title: "Create Email"
        };
        var confirmOptions = { height: 200, width: 450 };
        Xrm.Navigation.openAlertDialog(confirmStrings, confirmOptions).then(
            () => { },
            () => { });
    }
    var data = {
        "regardingobjectid": regardingObject
    };

    if (regardingObject[0].entityType == "contact") {
        var partlist = new Array();
        partlist[0] = new Object();
        partlist[0].id = regardingObject[0].id;
        partlist[0].name = regardingObject[0].name;
        partlist[0].entityType = "contact";
        data.to = partlist;
    }

    if (regardingObject[0].entityType == "opportunity") {
        {

            var regardingOpportunityId = regardingObject[0].id;
            var regardingOpportunity = await Xrm.WebApi.retrieveRecord("opportunity", regardingOpportunityId, "?$select=_parentcontactid_value");
            var contactId = regardingOpportunity["_parentcontactid_value"];
            var contactFullName = regardingOpportunity["_parentcontactid_value@OData.Community.Display.V1.FormattedValue"];
            if (contactId) {
                var partlist = new Array();
                partlist[0] = new Object();
                partlist[0].id = contactId;
                partlist[0].name = contactFullName;
                partlist[0].entityType = "contact";
                data.to = partlist;
            }
        }
    }

    var pageInput = {
        pageType: "entityrecord",
        entityName: "email",
        data: data
    };
    var navigationOptions = {
        target: 2,
        height: { value: 90, unit: "%" },
        width: { value: 85, unit: "%" },
        position: 1
    };
    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
        function success(result) {
            console.log("Record created with ID: " + result.savedEntityReference[0].id +
                " Name: " + result.savedEntityReference[0].name)
            // Handle dialog closed
        },
        function error() {
            // Handle errors
        }
    );
}

async function captureNotes(primaryControl) {
    debugger;
    var regardingobjectidControl = primaryControl.getAttribute("regardingobjectid");
    var regardingObject = regardingobjectidControl.getValue();
    if (regardingObject == null) {
        var confirmStrings = {
            confirmButtonLabel: "OK",
            text: "Rquired regarding field is empty on this meeting.",
            title: "Capture Notes"
        };
        var confirmOptions = { height: 200, width: 450 };
        Xrm.Navigation.openAlertDialog(confirmStrings, confirmOptions).then(
            () => { },
            () => { });
    }

    var recordId = regardingObject[0].id;
    var recordType = regardingObject[0].entityType;
    await setMakeNotesTabActiveToYes(recordId, recordType);
    var pageInput = {
        pageType: "entityrecord",
        entityName: regardingObject[0].entityType,
        entityId: regardingObject[0].id
    };
    var navigationOptions = {
        target: 2,
        height: { value: 90, unit: "%" },
        width: { value: 85, unit: "%" },
        position: 1
    };
    Xrm.Navigation.navigateTo(pageInput, navigationOptions).then(
        function success(result) {
            try {
            } catch (e) {

            }
        },
        function error() {
            // Handle errors
        }
    );
}

async function setMakeNotesTabActiveToYes(recordId, recordType) {
    try {
        var record = {};
        record.erp_focusnotestab = true;
        await Xrm.WebApi.updateRecord(recordType, recordId, record);
    } catch (e) {

    }

}
