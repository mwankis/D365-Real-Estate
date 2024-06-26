function onLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    showHideRelatedCompanies(formContext);
    formContext.data.addOnLoad(makeNotesTabActive);
}

function onParentAccountChange(executionContext) {
    var formContext = executionContext.getFormContext();
    showHideRelatedCompanies(formContext);
}

function showHideRelatedCompanies(formContext) {
    var erp_parentAccountControl = formContext.getAttribute("parentaccountid");
    if (erp_parentAccountControl) {
        var parentAccount = erp_parentAccountControl.getValue();
        if (parentAccount != null) {
            formContext.ui.tabs.get("tab_overview").sections.get("tab_child_related_companies").setVisible(true);
            formContext.ui.tabs.get("tab_overview").sections.get("tab_section_related_companies").setVisible(false);
        } else {
            formContext.ui.tabs.get("tab_overview").sections.get("tab_child_related_companies").setVisible(false);
            formContext.ui.tabs.get("tab_overview").sections.get("tab_section_related_companies").setVisible(true);
        }
    }
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

            Xrm.WebApi.updateRecord("account", recordId, record).then(
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