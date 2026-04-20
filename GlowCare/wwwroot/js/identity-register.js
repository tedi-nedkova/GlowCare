function setupPasswordToggle(buttonId, inputId) {
            const toggleButton = document.getElementById(buttonId);
            const passwordInput = document.getElementById(inputId);

            if (!toggleButton || !passwordInput) {
                return;
            }

            toggleButton.addEventListener("click", function () {
                const isHidden = passwordInput.type === "password";
                const icon = this.querySelector("i");
                const text = this.querySelector(".auth-password-toggle-text");

                passwordInput.type = isHidden ? "text" : "password";

                if (icon) {
                    icon.className = isHidden ? "fa fa-eye-slash" : "fa fa-eye";
                }

                if (text) {
                    text.textContent = isHidden ? "Скрий" : "Покажи";
                }

                this.setAttribute("aria-label", isHidden ? "Скрий паролата" : "Покажи паролата");
                this.setAttribute("aria-pressed", isHidden ? "true" : "false");
            });
        }

        setupPasswordToggle("toggleRegisterPassword", "registerPasswordInput");
        setupPasswordToggle("toggleRegisterConfirmPassword", "registerConfirmPasswordInput");
