var recording = false;

function checkButton() {
    if (recording) {
        chrome.browserAction.setIcon({ path: "autobot_stop.png" });
        chrome.browserAction.setTitle({ title: "Stop Recording" });
    } else {
        chrome.browserAction.setIcon({ path: "autobot_start.png" });
        chrome.browserAction.setTitle({ title: "Start Recording" });
    }
}

chrome.runtime.onStartup.addListener(checkButton);
chrome.runtime.onInstalled.addListener(checkButton);

chrome.browserAction.onClicked.addListener(function(tab) {
    recording = !recording;

    var payload = {
        recording: recording
    };

    $.ajax({
        type: "POST",
        url: "http://localhost:8055/",
        data: JSON.stringify(payload),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
    
    checkButton();
});

checkButton();