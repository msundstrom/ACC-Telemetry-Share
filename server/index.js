const express = require('express');
const cors = require('cors');
const app = express();
app.use(cors());
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server, {
    cors: {
        origin: '*',
        methods: ['GET', 'POST']
    }
});

var values = { };

const dataSourceNameSpace = io.of("/data");
const clientNameSpace = io.of('/view');

app.get('/', (req, res) => {  
    res.send('<h1>Hello world</h1>');
});

server.listen(3000, () => {  
    console.log('listening on *:3000');
});

dataSourceNameSpace.on('connection', (socket) => {  
    console.log('a data source connected');

    
    socket.on('acc update', (msg) => {
        console.log(`UPDATE: ${JSON.stringify(msg, null, 3)}`)
        values = msg;
    });

    socket.on('disconnect', () => {
        console.log('a data source disconnected');
    });
});

clientNameSpace.on('connection', (socket) => {
    console.log('a web interface connected');

    socket.on('disconnect', () => {
        console.log('a web interface disconnected');
    });
});

setInterval(() => {
    clientNameSpace.emit('FromAPI', values);
}, 100);