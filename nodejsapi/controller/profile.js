const express = require('express');
const router = express.Router();
const { sql, poolConnect, pool } = require('../connection/DB');
router.get('/api/Profiles', async (req, res) => {
    await poolConnect;
    try {
        const request = pool.request();
        const result = await request.query('select Name, LastName, Email, Password from Profile')

        var send_data = result.recordset;
        res.json(send_data);
    } catch (err) {
        res.status(500);
        res.send(err.message);
    }
});
router.post('/api/Profile', async (req, res) => {
    poolConnect.then((pool) => {
        const _body = req.body;
        pool.request()
            .input("name", sql.VarChar(50), _body.name)
            .input("lastname", sql.VarChar(50), _body.lastname)
            .input("email", sql.VarChar(50), _body.email)
            .input("password", sql.VarChar(50), _body.password)
            .execute("InsertProfile").then(function (recordSet) {
                res.status(200).json({ status: "Success" });
            });

    }).catch(err => {
        res.status(400).json({ message: "invalid" });
        res.send(err.message);
    });
});
router.put('/api/Profile', async (req, res) => {
    poolConnect.then((pool) => {
        const _body = req.body;
        pool.request()
            .input("id", sql.VarChar(50), _body.id)
            .input("lastname", sql.VarChar(50), _body.lastname || null)
            .input("email", sql.VarChar(50), _body.email || null)
            .input("password", sql.VarChar(50), _body.password || null)
            .execute("UpdateProfile").then(function (recordSet) {
                res.status(200).json({ status: "Success" });
            });

    }).catch(err => {
        res.status(400).json({ message: "invalid" });
        res.send(err.message);
    });
});

module.exports = router