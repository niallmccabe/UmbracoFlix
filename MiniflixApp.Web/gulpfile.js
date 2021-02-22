/// <binding BeforeBuild='default' Clean='clean' />

const gulp = require('gulp');
const gulpLoadPlugins = require('gulp-load-plugins');
const browserSync = require('browser-sync').create();
const del = require('del');
const runSequence = require('run-sequence');
const browserify = require('browserify');
const babelify = require('babelify');
const buffer = require('vinyl-buffer');
const source = require('vinyl-source-stream');
const browserifyShim = require('browserify-shim');
const gutil = require('gulp-util');
const path = require('path');

const $ = gulpLoadPlugins();
const reload = browserSync.reload;

const fileRevPaths = [
    '@(css|scripts|img)/dist/**/*',
    '!@(css|scripts|img)/dist/norev/**/*',
];

const copyAssets = [
    {
        src: "img/src/**/*",
        dist: "/img/dist/"
    }
    /*{
      src:"node_modules/awesome-package/image.png",
      dist:"/"
    }*/
];

let dev = true;

gulp.task('styles', () => {
    return gulp.src(['css/src/**/*.scss', '!css/src/**/_*.scss'])
        .pipe($.plumber())
        .pipe($.if(dev, $.sourcemaps.init()))
        .pipe($.sass.sync({
            outputStyle: 'expanded',
            precision: 10,
            includePaths: ['node_modules']
        }).on('error', $.sass.logError))
        .pipe($.cleanCss({
            format: 'beautify',
            inline: ['all'],
            level: 0
        })) // inline any css import statements
        .pipe($.autoprefixer({ browsers: ['> 1%', 'last 2 versions', 'Firefox ESR'] }))
        .pipe($.if(dev, $.sourcemaps.write()))
        .pipe(gulp.dest('css/dist'))
        .pipe(reload({ stream: true }));
});

gulp.task('scripts', gulp.series('lint'), () => {
    const b = browserify({
        entries: "scripts/src/main.js",
        transform: [babelify],
        debug: true
    });

    return b.bundle()
        .pipe(source('bundle.js'))
        .pipe($.plumber())
        .pipe(buffer())
        .pipe($.if(dev, $.sourcemaps.init({ loadMaps: true })))
        .pipe($.if(dev, $.sourcemaps.write('.')))
        .pipe(gulp.dest('scripts/dist'))
        .pipe(reload({ stream: true }));
});

function lint(files) {
    return gulp.src(files)
        .pipe($.eslint({ fix: true }))
        .pipe(reload({ stream: true, once: true }))
        .pipe($.eslint.format())
        .pipe($.if(!browserSync.active, $.eslint.failAfterError()));
}

gulp.task('lint', () => {
    return lint('scripts/src/**/*.js')
        .pipe(gulp.dest('scripts/src'));
});

gulp.task('sprite', () => {
    return gulp.src('img/src/sprite/*.svg')
        .pipe($.svgmin(function (file) {
            var prefix = path.basename(file.relative, path.extname(file.relative));
            return {
                plugins: [{
                    cleanupIDs: {
                        prefix: prefix + '-', // prefix IDs with their filename to prevent ID conflicts once in the sprite
                        minify: true
                    }
                }]
            }
        }))
        .pipe($.svgstore())
        .pipe(gulp.dest('img/dist'));
});

gulp.task('minifyStyles', gulp.series('styles'), () => {
    return gulp.src('css/dist/**/*.css')
        .pipe($.cleanCss({
            level: 2
        }))
        .pipe(gulp.dest('css/dist'));
});

gulp.task('minifyScripts', gulp.series('scripts'), () => {
    return gulp.src('scripts/dist/**/*.js')
        .pipe($.uglify({ compress: { drop_console: true } }).on('error', function (err) {
            gutil.log(gutil.colors.red('[Error]'), err.toString());
            this.emit('end');
        }))
        .pipe(gulp.dest('scripts/dist'));
});

gulp.task('minifyImages', gulp.series('sprite'), () => {
    return gulp.src(['img/src/**/*', 'img/dist/sprite.svg', '!img/src/sprite/**/*.svg'])
        .pipe($.cache($.imagemin([
            $.imagemin.gifsicle({ interlaced: true }),
            $.imagemin.jpegtran({ progressive: true }),
            $.imagemin.optipng({ optimizationLevel: 5 }),
            $.imagemin.svgo({ plugins: [{ cleanupIDs: false }, { removeUselessDefs: false }] })
        ])))
        .pipe(gulp.dest('img/dist'));
});

gulp.task('revision', () => {
    return gulp.src(fileRevPaths)
        .pipe($.rev())
        .pipe($.revDeleteOriginal()) // remove original (pre-revved) files
        .pipe(gulp.dest('.'))
        .pipe($.rev.manifest())
        .pipe(gulp.dest('.'));
});

gulp.task("revreplace", gulp.series("revision"), function () {
    var manifest = gulp.src('./rev-manifest.json');
    return gulp.src('@(css|scripts)/dist/**/*.@(js|css)')
        .pipe($.revReplace({ manifest: manifest }))
        .pipe(gulp.dest('.'));
});

gulp.task('minify', () => {
    return new Promise(resolve => {
        runSequence(['minifyImages', 'minifyStyles', 'minifyScripts'], 'revreplace', resolve);
    });
});

gulp.task('copy', () => {
    return copyAssets.map(function (assets) {
        return gulp.src(assets.src, {
            dot: true
        }).pipe(gulp.dest(assets.dist));
    });
});

gulp.task('clean', del.bind(null, ['@(css|scripts|img)/dist', './rev-manifest.json']));

gulp.task('watch', () => {
    runSequence('clean', ['copy', 'styles', 'scripts', 'sprite'], () => {
        /*browserSync.init({
          proxy: "localhost:12345"
        });*/

        gulp.watch([
            'Views/**/*.cshtml',
            'img/src/**/*',
            '!img/src/sprite/**/*.svg'
        ]).on('change', reload);

        gulp.watch('img/src/sprite/**/*.svg', ['sprite', 'reload']);
        gulp.watch('css/src/**/*.scss', ['styles']);
        gulp.watch('scripts/src/**/*.js', ['scripts']);
    });
});

gulp.task('compress', () => {
    return new Promise(resolve => {
        dev = false;
        runSequence('clean', ['minify', 'copy'], resolve);
    });
});

gulp.task('default', gulp.series('watch'));
