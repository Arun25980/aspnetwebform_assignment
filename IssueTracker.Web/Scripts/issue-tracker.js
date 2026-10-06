/**
 * Issue Tracker - Modal Helpers & AJAX Lifecycle Management
 * Location: Scripts/issue-tracker.js
 */

/**
 * Opens the Bootstrap 5 Add/Edit Modal
 */
function openModal() {
    var modalEl = document.getElementById('addEditModal');
    if (modalEl) {
        var myModal = bootstrap.Modal.getOrCreateInstance(modalEl);
        myModal.show();
    }
}

/**
 * Closes the Bootstrap 5 Add/Edit Modal
 */
function closeModal() {
    var modalEl = document.getElementById('addEditModal');
    if (modalEl) {
        var myModal = bootstrap.Modal.getInstance(modalEl);
        if (myModal) {
            myModal.hide();
        }
    }
}

/**
 * Resets modal fields and opens modal for creating a new issue
 */
function openModalForNew() {
    clearForm();
    openModal();
}

/**
 * Clears form fields inside the modal UI
 */
function clearForm() {
    var hfId = document.querySelector('[id$="hfIssueID"]');
    var txtTitle = document.querySelector('[id$="txtTitle"]');
    var txtDesc = document.querySelector('[id$="txtDescription"]');
    var ddlPriority = document.querySelector('[id$="ddlPriority"]');
    var txtAssigned = document.querySelector('[id$="txtAssignedTo"]');
    var errLabel = document.querySelector('[id$="lblModalError"]');

    if (hfId) hfId.value = '';
    if (txtTitle) txtTitle.value = '';
    if (txtDesc) txtDesc.value = '';
    if (ddlPriority) ddlPriority.selectedIndex = 0;
    if (txtAssigned) txtAssigned.value = '';

    if (errLabel) {
        errLabel.style.display = 'none';
        errLabel.innerText = '';
    }
}

/**
 * Ensures backdrop elements and body scrolling states are properly restored on hidden modal
 */
function cleanupBackdrop() {
    document.querySelectorAll('.modal-backdrop').forEach(function (el) {
        el.remove();
    });
    document.body.classList.remove('modal-open');
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
}

/**
 * Attaches event listeners for modal cleanup on DOM load and ASP.NET ScriptManager postbacks
 */
function initModalListeners() {
    var modalEl = document.getElementById('addEditModal');
    if (modalEl && !modalEl.dataset.listenerAttached) {
        modalEl.addEventListener('hidden.bs.modal', cleanupBackdrop);
        modalEl.dataset.listenerAttached = 'true';
    }
}

// Attach listener on initial page load
document.addEventListener('DOMContentLoaded', initModalListeners);

// Re-attach listener after ASP.NET UpdatePanel partial postbacks
if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initModalListeners);
}