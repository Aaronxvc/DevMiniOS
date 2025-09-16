// This file contains the JavaScript code to interact with the
// MiniModelController API endpoints.

// 1. trainModel(): Sends a POST request to the /api/minilm/train endpoint
//    to train the mini-LM with the text entered in the trainingText textarea.
//    It updates the trainStatus div with the training status.
// 2. sampleModel(): Sends a GET request to the /api/minilm/sample endpoint
//    with the prefix and maxTokens parameters.
//    It updates the completionResult div with the generated completion and
//    the tokenResult div with the list of tokens.
// 3. Error handling: Displays graceful error messages in the
//    completionResult and trainStatus divs if the API requests fail.

function trainModel() {
    const trainingText = document.getElementById("trainingText").value;
    const trainStatusDiv = document.getElementById("trainStatus");

    trainStatusDiv.innerHTML = "Training..."

    fetch('/api/minilm/train', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(trainingText) //Sending the trainingText as the body
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`)
            }
            return response.json();
        })
        .then(data => {
            if (data.ok) {
                trainStatusDiv.innerHTML = "Training complete!";
            } else {
                trainStatusDiv.innerHTML = `Training failed: ${data.error}`;
            }
        })
        .catch(error => {
            trainStatusDiv.innerHTML = `Error: ${error}`
        })
}


function sampleModel() {
    const prefix = document.getElementById("prefix").value;
    const maxTokens = document.getElementById("maxTokens").value;
    const completionResultDiv = document.getElementById("completionResult");
    const tokenResultDiv = document.getElementById("tokenResult");

    completionResultDiv.innerHTML = "Generating completion...";
    tokenResultDiv.innerHTML = "";

    fetch(`/api/minilm/sample?prefix=${prefix}&maxTokens=${maxTokens}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            completionResultDiv.innerHTML = data.completion;
            tokenResultDiv.innerHTML = data.tokens.join(", ");
        })
        .catch(error => {
            completionResultDiv.innerHTML = `Error: ${error}`;
            tokenResultDiv.innerHTML = "";
        });
}