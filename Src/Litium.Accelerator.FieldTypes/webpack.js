const ModuleFederationPlugin = require("webpack/lib/container/ModuleFederationPlugin");
const angularCorePkJson = require('@angular/core/package.json');
const angularCommonPkJson = require('@angular/common/package.json');
const angularRouterPkJson = require('@angular/router/package.json');
const angularFormPkJson = require('@angular/forms/package.json');
const angularReduxPkJson = require('@angular-redux/store/package.json');
const ngxTranslatePkJson = require('@ngx-translate/core/package.json');

module.exports = {
    optimization: {
        runtimeChunk: false,
    },
    plugins: [
        new ModuleFederationPlugin({
            name: "Accelerator",
            filename: "remoteEntry.js",
            exposes: {
                Accelerator:
                "./Litium.Accelerator.FieldTypes/src/Accelerator/extensions.ts",
                FieldEditorFilterFieldsComponent: "./Litium.Accelerator.FieldTypes/src/Accelerator/components/field-editor-filter-fields/field-editor-filter-fields.component",
            },
            shared: {
                "@angular/core": { eager: false, singleton: true, requiredVersion:  angularCorePkJson.version },
                "@angular/common": { eager: false, singleton: true, requiredVersion: angularCommonPkJson.version },
                "@angular/router": { eager: false, singleton: true, requiredVersion: angularRouterPkJson.version },
                "@angular/forms": { eager: false, singleton: true, requiredVersion: angularFormPkJson.version },
                "@angular/common/http": { eager: false, singleton: true, requiredVersion: angularCommonPkJson.version },
                "@angular-redux/store": { eager: false, singleton: true, requiredVersion: angularReduxPkJson.version },
                "@ngx-translate/core": { eager: false, singleton: true, requiredVersion: ngxTranslatePkJson.version },
                "litium-ui": { eager: false, singleton: true },
            },
        }),
    ]
}