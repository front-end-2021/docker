var express = require('express');
var bodyParser = require("body-parser");
const path = require('path');

const PORT = 8080;
const HOST = '0.0.0.0';

var app = express();
app.use(function (req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    res.header('Access-Control-Allow-Methods', 'PUT, POST, GET, DELETE, OPTIONS');
    next();
});
app.use(bodyParser.urlencoded({extended: true }));
app.use(bodyParser.json());
app.use(bodyParser.raw());
const ProfileData = require('./controller/profile');
app.use('/', ProfileData);

app.get('/', (req, res) => {
  //res.send('Hello World, I am Dainb_');
  res.sendFile(path.join(__dirname+'/index.html'));
});
app.listen(PORT, HOST);
console.log(`Running on ${getDomainName()}:${PORT}`);

function getDomainName(){
  return 'http://localhost'
}

// app.listen(5000, function () {
//     console.log('Server is running..');
// });