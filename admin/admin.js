const API_URL = "http://localhost:5008";

const loginView = document.getElementById("loginView");
const dashboardView = document.getElementById("dashboardView");

const loginForm = document.getElementById("loginForm");
const loginButton = document.getElementById("loginButton");
const loginError = document.getElementById("loginError");

const onlineToggle = document.getElementById("onlineToggle");
const onlineText = document.getElementById("onlineText");
const statusDot = document.getElementById("statusDot");

const currentMode = document.getElementById("currentMode");
const modeButtons = document.querySelectorAll(".mode-button");

const guestCountElement = document.getElementById("guestCount");
const houseMessage = document.getElementById("houseMessage");
const messageLength = document.getElementById("messageLength");

const saveButton = document.getElementById("saveButton");
const saveMessage = document.getElementById("saveMessage");
const lastUpdated = document.getElementById("lastUpdated");

let selectedMode = "Chill";
let guestCount = 0;


function getToken() {
    return sessionStorage.getItem("houseXylooToken");
}

function isTokenExpired(token) {
    try {
        const payload = JSON.parse(
            atob(token.split(".")[1])
        );

        return Date.now() >= payload.exp * 1000;
    } catch {
        return true;
    }
}


function showLogin() {
    loginView.classList.remove("hidden");
    dashboardView.classList.add("hidden");
}


function showDashboard() {
    loginView.classList.add("hidden");
    dashboardView.classList.remove("hidden");
}


loginForm.addEventListener("submit", async (event) => {

    event.preventDefault();

    loginError.textContent = "";
    loginButton.disabled = true;
    loginButton.textContent = "AUTHENTICATING...";

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    try {

        const response = await fetch(
            `${API_URL}/api/auth/login`,
            {
                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify({
                    email,
                    password
                })
            }
        );

        if (!response.ok) {
            throw new Error("Invalid credentials");
        }

        const data = await response.json();

        sessionStorage.setItem(
            "houseXylooToken",
            data.token
        );

        showDashboard();

        await loadHouseStatus();

    } catch (error) {

        loginError.textContent =
            "Access denied. Check your credentials.";

    } finally {

        loginButton.disabled = false;
        loginButton.textContent = "ENTER HOUSE";
    }
});


async function loadHouseStatus() {

    try {

        const response = await fetch(
            `${API_URL}/api/house/status`
        );

        if (!response.ok) {
            throw new Error();
        }

        const status = await response.json();

        onlineToggle.checked = status.online;

        selectedMode = status.mode;
        guestCount = status.guestCount;

        houseMessage.value = status.message ?? "";

        updateInterface();

        if (status.updatedAt) {

            const date = new Date(status.updatedAt);

            lastUpdated.textContent =
                `Last updated ${date.toLocaleString()}`;
        }

    } catch {

        saveMessage.textContent =
            "Could not connect to House Xyloo.";
    }
}


function updateInterface() {

    currentMode.textContent =
        formatMode(selectedMode);

    guestCountElement.textContent =
        guestCount;

    messageLength.textContent =
        houseMessage.value.length;

    modeButtons.forEach(button => {

        button.classList.toggle(
            "active",
            button.dataset.mode === selectedMode
        );
    });

    if (onlineToggle.checked) {

        onlineText.textContent = "ONLINE";
        statusDot.classList.remove("offline");

    } else {

        onlineText.textContent = "OFFLINE";
        statusDot.classList.add("offline");
    }
}


function formatMode(mode) {

    if (mode === "AfterHours") {
        return "AFTER HOURS";
    }

    return mode.toUpperCase();
}


modeButtons.forEach(button => {

    button.addEventListener("click", () => {

        selectedMode = button.dataset.mode;

        updateInterface();
    });
});


document
    .getElementById("increaseGuests")
    .addEventListener("click", () => {

        guestCount++;

        updateInterface();
    });


document
    .getElementById("decreaseGuests")
    .addEventListener("click", () => {

        if (guestCount > 0) {
            guestCount--;
        }

        updateInterface();
    });


onlineToggle.addEventListener(
    "change",
    updateInterface
);


houseMessage.addEventListener(
    "input",
    updateInterface
);


saveButton.addEventListener("click", async () => {

    const token = getToken();

    if (!token) {
        showLogin();
        return;
    }

    saveButton.disabled = true;
    saveButton.textContent = "SAVING...";
    saveMessage.textContent = "";

    try {

        const response = await fetch(
            `${API_URL}/api/house/status`,
            {
                method: "PUT",

                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },

                body: JSON.stringify({
                    online: onlineToggle.checked,
                    mode: selectedMode,
                    message: houseMessage.value.trim(),
                    guestCount
                })
            }
        );

        if (response.status === 401) {

            sessionStorage.removeItem(
                "houseXylooToken"
            );

            showLogin();

            return;
        }

        if (!response.ok) {
            throw new Error();
        }

        const status = await response.json();

        saveMessage.textContent =
            "House updated successfully.";

        const date = new Date(status.updatedAt);

        lastUpdated.textContent =
            `Last updated ${date.toLocaleString()}`;

    } catch {

        saveMessage.textContent =
            "Something went wrong.";

    } finally {

        saveButton.disabled = false;
        saveButton.textContent = "SAVE CHANGES";
    }
});


document
    .getElementById("logoutButton")
    .addEventListener("click", () => {

        sessionStorage.removeItem(
            "houseXylooToken"
        );

        showLogin();
    });


const token = getToken();

if (token && !isTokenExpired(token)) {

    showDashboard();
    loadHouseStatus();

} else {

    sessionStorage.removeItem("houseXylooToken");
    showLogin();
}