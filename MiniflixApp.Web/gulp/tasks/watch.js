var gulp = require('gulp');
var browserSync = require('browser-sync').create();
var $ = require('gulp-load-plugins')();
var autoprefixer = require('autoprefixer');
var webpack = require('webpack');
var sourcemaps = require('gulp-sourcemaps');

var sassPaths = [
    'node_modules/foundation-sites/scss',
    'node_modules/motion-ui/src'
];

function sass() {
    return gulp.src('scss/app.scss')
        .pipe(sourcemaps.init())
        .pipe($.sass({
            includePaths: sassPaths,
            outputStyle: 'compressed' // if css compressed **file size**
        })
            .on('error', $.sass.logError))
        .pipe($.autoprefixer())
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest('Css/dist/'))
        .pipe(browserSync.stream());
};

function watch() {
    browserSync.init({
        server: "./"
    });

    gulp.watch("scss/**/*.scss", sass);
    gulp.watch(["js/app.js", "js/vendor.js"], script);
}

function script(callback) {
    webpack(require('../../webpack.config.js'), function (err, stats) {
        if (err) {
            console.log(err.toString());
        }
        console.log(stats.toString());
        callback();
    });
}

gulp.task('sass', sass);
gulp.task('script', script);

gulp.task('scripts', gulp.series('script', watch));
gulp.task('watch', gulp.series('sass', watch));
gulp.task('default', gulp.series('sass', watch));
