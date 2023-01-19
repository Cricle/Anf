var gulp = require('gulp');
var gzip = require('gulp-gzip');

const srcs=[
  './dist/**/*.js',
  './dist/**/*.html',
  './dist/**/*.css',
];

gulp.task('compress', function() {
  return gulp.src(srcs)
      .pipe(gzip())
      .pipe(gulp.dest('./dist'));
});