/// <binding AfterBuild='copy-modules' />
"use strict";

var gulp = require("gulp"),
    clean = require("gulp-clean");


var paths = {
    devModules: "../Modules/",
    hostModules: "./Modules/",
    webroot: "./wwwroot/"
};

paths.css = paths.webroot + "css/";
paths.js = paths.webroot + "js/";

var modules = [
    "Lumle.Module.CMS",
    "Lumle.Module.Core",
    "Lumle.Module.Blog",
    "Lumle.Module.Authorization",
    "Lumle.Module.Audit",
    'Lumle.Module.Localization',
    'Lumle.Module.Calendar',
    'Lumle.Module.AdminConfig',
    'Lumle.Module.PublicUser',
    "Lumle.Module.Schedular"
];

gulp.task("clean-module", function () {
    return gulp.src([paths.hostModules + "*"], { read: false })
        .pipe(clean());
});

gulp.task("copy-modules", ["clean-module"], function () {
    modules.forEach(function (module) {
        gulp.src([paths.devModules + module + "/Views/**/*.*"], { base: module })
            .pipe(gulp.dest(paths.hostModules + module));

        gulp.src(paths.devModules + module + "/wwwroot/css/*.*")
            .pipe(gulp.dest(paths.css));

        gulp.src(paths.devModules + module + "/wwwroot/js/*.*")
            .pipe(gulp.dest(paths.js));

        gulp.src(paths.devModules + module + "/bin/Debug/netcoreapp2.0/**/*.*")
            .pipe(gulp.dest(paths.hostModules + module + "/bin"));
    });
});

gulp.task("copy-ms", function () {
    modules.forEach(function (module) {
        gulp.src([paths.devModules + module + "/Views/**/*.*", paths.devModules + module + "/wwwroot/**/*.*"], { base: module })
            .pipe(gulp.dest(paths.hostModules + module));
    });
});
