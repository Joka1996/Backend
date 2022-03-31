import React from 'react';
import DynamicComponent from '../DynamicComponent';
const scriptPattern = /<script\b[^>]*>([\s\S]*?)<\/script>/gi;
const scriptFilePattern = /<script.*?src=["'](.*?)["']/gi;

const extractScripts = (domString) => {
    let matches,
        html = domString;
    const scripts = [],
        scriptFiles = [];
    while ((matches = scriptPattern.exec(domString)) !== null) {
        html = html.replace(matches[0], '');
        matches[1] && matches[1].trim() !== '' && scripts.push(matches[1]);
    }
    while ((matches = scriptFilePattern.exec(domString)) !== null) {
        matches[1] && matches[1].trim() !== '' && scriptFiles.push(matches[1]);
    }

    return {
        html,
        scripts,
        scriptFiles,
    };
};

const executeScript = (domId, scriptContent) => {
    const script = document.createElement('script');
    script.type = 'text/javascript';
    try {
        script.appendChild(document.createTextNode(scriptContent));
    } catch (e) {
        // to support IE
        script.text = scriptContent;
    }
    document.getElementById(domId).appendChild(script);
};

const includeScript = (domId, srciptUrl) => {
    const script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = srciptUrl;
    document.getElementById(domId).appendChild(script);
};

const PaymentWidget = React.memo(function PaymentWidget({ responseString }) {
    const renderWidget = (paymentSession) => {
        const WidgetCheckout = DynamicComponent({
            loader: () => import('./CheckoutWidget'),
        });
        const args = {
            paymentSession,
            extractScripts,
            executeScript,
            includeScript,
        };
        return <WidgetCheckout {...args} />;
    };

    return renderWidget(responseString);
});

export default PaymentWidget;
