var fs = require('fs')
  , path = require('path');

module.exports = function () {
  var logger = path.join(__dirname, 'Strider.MsBuild.Logger.dll');
  if (fs.existsSync(logger)) {
    return logger;
  }

  return null;
};