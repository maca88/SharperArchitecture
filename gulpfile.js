var gulp = require('gulp'),
    flatten = require('gulp-flatten'),
    copy = require('gulp-copy');
    
    
var basePath = './Source/';

var getPaPath = function(name, ext){
    ext = ext || 'dll';
    return basePath + 'PowerArhitecture.' + name + '/bin/Debug/PowerArhitecture.' + name + '.' + ext;
};

var paFiles = [
    getPaPath('Authentication'),
    getPaPath('Authentication', 'pdb'),
    getPaPath('Breeze'),
    getPaPath('Breeze', 'pdb'),
    getPaPath('CodeList'),
    getPaPath('CodeList', 'pdb'),
    getPaPath('Common'),
    getPaPath('Common', 'pdb'),
    getPaPath('DataAccess'),
    getPaPath('DataAccess', 'pdb'),
    getPaPath('Domain'),
    getPaPath('Domain', 'pdb'),
    getPaPath('Validation'),
    getPaPath('Validation', 'pdb'),
    getPaPath('Notifications'),
    getPaPath('Notifications', 'pdb')
];

var outBasePath = './build/';

gulp.task('copy', function() {
    return gulp.src(paFiles)
    .pipe(flatten())
    .pipe(gulp.dest(outBasePath))
    .pipe(gulp.dest('C:/Workspace/Git/Bar/packages/PowerArhitecture/'));
});