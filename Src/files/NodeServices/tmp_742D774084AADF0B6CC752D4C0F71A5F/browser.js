module.exports = function (callback, request) {
    _launch(request)
        .then(result => callback(/* error */ null, result))
        .catch(error => callback(error));
};

async function _launch(request) {
    const puppeteer = require('puppeteer');
    const browser = await puppeteer.launch({
        ignoreHTTPSErrors: true, // ignore HTTPS errors during navigation
        // Command line switches https://kapeli.com/cheat_sheets/Chromium_Command_Line_Switches.docset/Contents/Resources/Documents/index
        args: [
            '--ignore-certificate-errors', // Ignores certificate-related errors.
            '--no-sandbox', // otherwise call from .net will hang
            '--disable-setuid-sandbox', // to avoid hang when running in docker/linux
            '--disable-breakpad', // Disables the crash reporting
        ],
        executablePath: request.browserExecutablePath || undefined,
    });

    return {
        wsEndpoint: browser.wsEndpoint(),
        // keep the browser's process Id so we will kill it if puppeteer cannot invoke browser.close()
        processId: browser.process().pid,
    };
}