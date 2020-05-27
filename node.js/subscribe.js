#!/usr/bin/env node

/* jslint node: true */
'use strict';

var nats = require('nats').connect("nats://krv:var753ma@localhost:4222");
var args = process.argv.slice(2)

nats.on('error', function(e) {
    console.log('Error [' + nats.options.url + ']: ' + e);
    process.exit();
});

nats.on('close', function() {
    console.log('CLOSED');
    process.exit();
});

var subject = args[0]

if (!subject) {
    console.log('Usage: node-sub <subject>');
    process.exit();
}

console.log('Listening on [' + subject + ']');

nats.subscribe(subject, function(msg) {
    console.log('Received "' + msg + '"');
});