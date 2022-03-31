const MiniCssExtractPlugin = require('mini-css-extract-plugin');

const path = require('path');
const ROOT = path.resolve(__dirname, '../');
const BUILD_DIR = path.resolve(ROOT, '../wwwroot/ui');

module.exports = {
    context: ROOT,
    output: {
        path: BUILD_DIR,
        publicPath: '/ui/',
        filename: '[name].js',
        clean: true,
    },
    devtool: 'source-map',
    module: {
        rules: [
            {
                test: /\.css$/,
                include: /node_modules/,
                use: [
                    {
                        loader: 'style-loader',
                    },
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                        },
                    },
                ],
            },
            {
                test: /\.scss$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader,
                    },
                    {
                        loader: 'css-loader', // translates CSS into CommonJS
                        options: {
                            sourceMap: true,
                        },
                    },
                    {
                        loader: 'resolve-url-loader', // resolve relative url of assets in scss files if they are imported from node_modules
                    },
                    {
                        loader: 'sass-loader', // compiles Sass to CSS, using Dart Sass
                        options: {
                            sourceMap: true,
                        },
                    },
                ],
            },
            {
                test: /\.svg$/,
                exclude: /(\/fonts)/,
                loader: 'svg-url-loader',
            },
            {
                test: /\.(png|jpg|jpeg|gif|ico)/i,
                loader: 'file-loader',
                exclude: /(\/fonts)/,
                options: {
                    name: '[name].[ext]',
                    outputPath: '../images',
                },
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)/i,
                loader: 'file-loader',
                include: [/fonts/, /node_modules/],
                options: {
                    name: '[name].[ext]',
                    outputPath: '../fonts',
                },
            },
        ],
    },
    plugins: [],
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
};
