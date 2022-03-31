module.exports = function (callback, request) {
    _generate(request)
        .then(result => callback(/* error */ null, result))
        .catch(error => callback(error));
};

async function _generate(request) {
    if (!request || !request.request || !request.url || !request.wsEndpoint) {
        return null;
    }
    const puppeteer = require('puppeteer');
    const defaultViewport = {
        height: 768,
        width: 1024
    };
    const browser = await puppeteer.connect({ browserWSEndpoint: request.wsEndpoint });
    const result = {
        pages: [],
        blocks: [],
        browserClosed: false,
    };
    try {
        const page = await browser.newPage();
        page.setDefaultNavigationTimeout(request.navigationTimeoutMilliseconds || 30000);
        await page.setCookie({ name: request.cookieName, value: request.cookieValue, url: request.url });
        const response = await page.goto(request.url);
        // wait for video tags to start, otherwise we will get blank, and black image which usually the first frame of the video
        await page.waitForTimeout(1000);
        const screenshotOption = {
            type: 'jpeg',
            quality: 70,
            encoding: 'base64',
        }
        result.ok = response.ok();

        const bodyHandle = await page.$('body');
        const bodyBoundingBox = await bodyHandle.boundingBox();
        const newViewport = {
            width: Math.max(defaultViewport.width, Math.ceil(bodyBoundingBox.width)),
            height: Math.max(defaultViewport.height, Math.ceil(bodyBoundingBox.height)),
        };

        await page.setViewport(Object.assign({}, defaultViewport, newViewport));
        if (request.request.pages) {
            for (const pageId of request.request.pages) {
                result.pages.push({
                    systemId: pageId,
                    data: await page.screenshot({
                        ...screenshotOption,
                        clip: {
                            x: 0,
                            y: 0,
                            width: defaultViewport.width,
                            // capture page's thumbnail in rectangle to
                            // avoid having too tall page's thumbnail which leads to a
                            // bad thumbnail quality issue
                            height: defaultViewport.width,
                        },
                    }),
                });
            }
        }
        if (request.request.blocks) {
            for (const blockId of request.request.blocks) {
                const elementHandle = await page.$(`[data-litium-block-id="${blockId}"]`);
                if (elementHandle) {
                    let data = null;
                    const boundingBox = await elementHandle.boundingBox(); //check boundingBox height to avoid error 'Node has 0 height.' then set height to 1 if height is zero
                    if (boundingBox && boundingBox.height > 0) {
                        data = await elementHandle.screenshot({
                            ...screenshotOption,
                        });
                    }
                    result.blocks.push({
                        systemId: blockId,
                        data: data,
                    });
                }
            }
        }
    } catch (ex) {
        result.error = ex.message || ex.toString();
    } finally {
        try {
            await browser.close();
            result.browserClosed = true; // since we can close it now, set to true so .Net won't kill it anymore
        } catch (ex) {} // if we can't close it, ignore it, .Net will kill it
    }

    return result;
}