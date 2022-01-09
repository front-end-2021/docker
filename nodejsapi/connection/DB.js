// DB.js config for your database  
const sql = require('mssql/msnodesqlv8');

const config = {
    driver: "msnodesqlv8",
    server: "localhost",
    database: "dainb",
    options: {
        trustServerCertificate: true, // change to true for local dev / self-signed certs
        trustedConnection: true // change to true for local dev / self-signed certs
    }
}

const pool = new sql.ConnectionPool(config);
const poolConnect = pool.connect();

pool.on('error', err => {
    console.log('Database Connection Failed! Bad Config: ', err);
})
module.exports = {
    sql, poolConnect, pool
}
