strider-msbuild-logger
======================

A logger for msbuild that can be used to prettify msbuild's output for strider. 
It outputs to stdout and stderr, and automatically colorizes its output 
(msbuild won't do so to buffers that are not consoles).

Example usage: 

```
var spawn = require('child_process').spawn;

try {
  var logger = require(msbuild-logger)();
} catch (err) {
  logger = null;
}

var args = ['project-path'];
if (logger) {
  // shut off the standard console logger, otherwise the output will be logged twice
  args.push('/noconsolelogger', '/logger:' + logger);
}

spawn('msbuild', args);
```
