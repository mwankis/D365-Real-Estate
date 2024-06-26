// JavaScript source code

function onLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.data.addOnLoad(makeNotesTabActive);
}

function makeNotesTabActive(executionContext) {
    var formContext = executionContext.getFormContext();
    var focusnotestabControl = formContext.getAttribute("erp_focusnotestab");
    if (focusnotestabControl) {
        var focusnotestab = focusnotestabControl.getValue();
        if (focusnotestab) {
            formContext.ui.tabs.get("Notes").setFocus();
            var dirtyRecordId = formContext.data.entity.getId();
            var recordId = dirtyRecordId.replace("{", "").replace("}", "");
            var record = {};
            record.erp_focusnotestab = false; 

            Xrm.WebApi.updateRecord("opportunity", recordId, record).then(
                function success(result) {
                    var updatedId = result.id;
                    console.log(updatedId);
                },
                function (error) {
                    console.log(error.message);
                }
            );
        }
    }
}
