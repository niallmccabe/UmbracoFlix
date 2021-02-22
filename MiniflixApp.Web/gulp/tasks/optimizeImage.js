var gulp = require('gulp'),
    imagemin = require('gulp-imagemin');

gulp.task('optimizeImages', function () {
    return gulp.src('./images/**/*')
        .pipe(imagemin({
            progressive: true,
            interlaced: true,
            multipass: true
        }))
        .pipe(gulp.dest("./images/dist"));
});