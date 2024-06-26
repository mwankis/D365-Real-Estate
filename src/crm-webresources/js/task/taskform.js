function onLoad(executionContext) {
    var formContext = executionContext.getFormContext();
    hideOrShowContactsSection(formContext);
}

function onLinkToMultipleContactsChange(executionContext) {
    var formContext = executionContext.getFormContext();
    hideOrShowContactsSection(formContext);
}

function hideOrShowContactsSection(formContext) {
    var linkToMultipleContactsControl = formContext.getAttribute("erp_linktomultiplecontacts");
    if (linkToMultipleContactsControl) {
        var linkToMultipleContacts = linkToMultipleContactsControl.getValue();
        var section = formContext.ui.tabs.get("TASK_TAB").sections.get("task_contacts");
        if (linkToMultipleContacts) {
           section.setVisible(true);

        } else {
           section.setVisible(false);
        }
    }

}