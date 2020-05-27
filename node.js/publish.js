#!/usr/bin/env node

/* jslint node: true */
'use strict';

var nats = require('nats').connect("nats://<username>:<password>@localhost:4222");
var args = process.argv.slice(2)

nats.on('error', function(e) {
    console.log('Error [' + nats.options.url + ']: ' + e);
    process.exit();
});

var subject = args[0];
var msg = args[1];

if (!subject) {
    console.log('Usage: node-pub <subject> [msg]');
    process.exit();
}

nats.publish(subject, msg, function() {
    console.log('Published [' + subject + '] : "' + msg + '"');
    process.exit();
});