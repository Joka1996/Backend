import React, { useEffect, useRef } from 'react';

const CheckoutWidget = ({
    extractScripts,
    executeScript,
    includeScript,
    paymentSession,
}) => {
    const extractScriptsResult = extractScripts(paymentSession);
    const firstRender = useRef(true);

    useEffect(() => {
        // Make sure it is executed only once.
        // firstRender is used to prevent eslint warning when having empty dependencies array
        if (!firstRender.current) {
            return;
        }
        firstRender.current = false;
        extractScriptsResult.scripts &&
            extractScriptsResult.scripts.forEach((script) =>
                executeScript('checkout-widget', script)
            );
        extractScriptsResult.scriptFiles &&
            extractScriptsResult.scriptFiles.forEach((url) =>
                includeScript('checkout-widget', url)
            );
    }, [executeScript, includeScript, extractScriptsResult]);

    return (
        <div
            id="checkout-widget"
            dangerouslySetInnerHTML={{ __html: extractScriptsResult.html }}
        />
    );
};

export default CheckoutWidget;
