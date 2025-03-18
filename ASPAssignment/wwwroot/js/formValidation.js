document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("createProjectForm").addEventListener("submit", function (event) {
        let valid = true;

        function validateField(id, errorId, errorMessage) {
            const field = document.getElementById(id);
            if (!field.value.trim()) {
                document.getElementById(errorId).textContent = errorMessage;
                valid = false;
            } else {
                document.getElementById(errorId).textContent = "";
            }
        }

        validateField("projectName", "projectNameError", "Project name is required");
        validateField("clientName", "clientNameError", "Client name is required");
        validateField("description", "descriptionError", "Description is required");
        validateField("members", "membersError", "At least one member is required");
        validateField("budget", "budgetError", "Budget is required");

        if (!valid) {
            event.preventDefault();
        }
    });
});

document.getElementById("loginForm").addEventListener("submit", function (event) {
    let valid = true;

    function validateField(id, errorId, errorMessage) {
        const field = document.getElementById(id);
        if (!field.value.trim()) {
            document.getElementById(errorId).textContent = errorMessage;
            valid = false;
        } else {
            document.getElementById(errorId).textContent = "";
        }
    }

    validateField("email", "emailError", "Email is required");
    validateField("password", "passwordError", "Password is required");

    if (!valid) {
        event.preventDefault();
    }
});
