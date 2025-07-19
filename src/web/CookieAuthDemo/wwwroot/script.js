function getBaseUrl(){
    return document.getElementById("input_base_url").value;
}

function displayStep3() {
    const step3 = document.getElementById("step3");
    step3.style.display = 'block';
}

function displayStep4() {
    const step4 = document.getElementById("step4");
    step4.style.display = 'block';
}

async function healthCheck() {

    const endpointPath = "platform/health";

    const response = await fetch(`${getBaseUrl()}${endpointPath}`, {
        method: 'GET'
    });

    const status = document.getElementById("step2_status");

    if (response.ok) {

        var health = await response.json()
        
        status.className = "text-success";
        status.innerHTML = health.Status;

        displayStep3();
    }
    else {
        status.className = "text-danger";
        status.innerHTML = "The API is <strong>not</strong> online.";
    }
}

async function generateCookie() {

    const endpointPath = "security/cookies/generate";

    const token = document.getElementById("input_cookie_value").value;

    const response = await fetch(`${getBaseUrl()}${endpointPath}`, {
        method: 'POST',
        credentials: 'include', // ensure cookies are sent/received
        headers: {
            'Content-Type': 'text/plain'
        },
        body: token
    });

    // Wait for the cookie to fully generate.
    await new Promise(resolve => setTimeout(resolve, 500));

    const status = document.getElementById("step3_status");

    if (response.ok) {

        var cookie = await response.json()
        
        status.className = "text-success";
        status.innerHTML = "Cookie generation succeeded. Use the Developer Tools in your browser to confirm it now has a matching cookie."

        const display = document.getElementById("cookie_display");
        display.innerHTML = JSON.stringify(cookie);

        displayStep4();
    }
    else {
        status.className = "text-danger";
        status.innerHTML = "Cookie generation failed. An unexpected error occurred.";
    }
}

async function useCookie() {

    const endpointPath = document.getElementById("input_endpoint_path").value;

    const status = document.getElementById("step4_status");

    const response = await fetch(`${getBaseUrl()}${endpointPath}`, {
        method: 'GET',
        credentials: 'include' // ensure cookies are sent/received
    });

    const display = document.getElementById("introspection_display");

    if (response.ok) {

        var result = await response.json()

        status.className = "text-success";
        status.innerHTML = "Cookie introspection succeeded. A detailed report is available below."

        display.innerHTML = JSON.stringify(result, null, 2);
    }
    else {

        status.className = "text-danger";
        status.innerHTML = `Cookie introspection failed with HTTP ${response.status}.`;

        if (response.status == 401) {
            var error = await response.json();
            display.innerHTML = JSON.stringify(error, null, 2);
        }
    }
}

// healthCheck();
// generateCookie();
// useCookie();